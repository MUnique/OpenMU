// <copyright file="InventoryStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;
using static MUnique.OpenMU.DataModel.InventoryConstants;

/// <summary>
/// The storage of an inventory of a player, which also contains equippable slots. This class also manages the powerups which get created by equipped items.
/// </summary>
public class InventoryStorage : Storage, IInventoryStorage
{
    private readonly IGameContext _gameContext;

    private readonly Player _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryStorage" /> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="context">The game context.</param>
    public InventoryStorage(Player player, IGameContext context)
        : base(GetInventorySize(0),
            EquippableSlotsCount,
            0,
            new ItemStorageAdapter(player.SelectedCharacter?.Inventory ?? throw Error.NotInitializedProperty(player, "SelectedCharacter.Inventory"), FirstEquippableItemSlotIndex, player.GetInventorySize()))
    {
        this._player = player;
        this.EquippedItemsChanged += async eventArgs => await this.UpdateItemsOnChangeAsync(eventArgs.Item).ConfigureAwait(false);
        this._gameContext = context;

        if (player.SelectedCharacter.InventoryExtensions > 0)
        {
            var extensions = new List<Storage>(player.SelectedCharacter.InventoryExtensions);

            var sizePerExtension = RowsOfOneExtension * RowSize;
            for (int i = 0; i < player.SelectedCharacter.InventoryExtensions; i++)
            {
                var offset = FirstExtensionItemSlotIndex + (i * sizePerExtension);
                var extension = new Storage(sizePerExtension, 0, offset, this.ItemStorage);
                extensions.Add(extension);
            }

            this.Extensions = extensions;
        }

        this.InitializePowerUps();
    }

    /// <inheritdoc/>
    public event AsyncEventHandler<ItemEventArgs>? EquippedItemsChanged;

    /// <inheritdoc/>
    public IEnumerable<Item> EquippedItems
    {
        get
        {
            for (int i = FirstEquippableItemSlotIndex; i <= LastEquippableItemSlotIndex; i++)
            {
                if (this.ItemArray[i] is not null)
                {
                    yield return this.ItemArray[i]!;
                }
            }
        }
    }

    /// <inheritdoc/>
    public Item? EquippedAmmunitionItem
    {
        get
        {
            if (this.ItemArray[LeftHandSlot] is { } leftItem && (leftItem.Definition?.IsAmmunition ?? false))
            {
                return leftItem;
            }

            if (this.ItemArray[RightHandSlot] is { } rightItem && (rightItem.Definition?.IsAmmunition ?? false))
            {
                return rightItem;
            }

            return null;
        }
    }

    /// <inheritdoc />
    /// <remarks>Additionally we need to make a temporary item persistent with the context of the player.</remarks>
    public override async ValueTask<bool> AddItemAsync(byte slot, Item item)
    {
        Item? convertedItem = null;
        if (item is TemporaryItem temporaryItem)
        {
            convertedItem = temporaryItem.MakePersistent(this._player.PersistenceContext);
        }

        var success = await base.AddItemAsync(slot, convertedItem ?? item).ConfigureAwait(false);
        if (!success && convertedItem != null)
        {
            this._player.PersistenceContext.Detach(convertedItem);
        }

        if (success)
        {
            var isEquippedItem = this.IsWearingSlot(slot);
            if (isEquippedItem && this.EquippedItemsChanged is { } eventHandler)
            {
                await eventHandler(new ItemEventArgs(convertedItem ?? item)).ConfigureAwait(false);
            }
        }

        return success;
    }

    /// <inheritdoc />
    public override async ValueTask RemoveItemAsync(Item item)
    {
        var isEquippedItem = this.IsWearingSlot(item.ItemSlot);
        await base.RemoveItemAsync(item).ConfigureAwait(false);
        if (isEquippedItem && this.EquippedItemsChanged is { } eventHandler)
        {
            await eventHandler(new ItemEventArgs(item)).ConfigureAwait(false);
        }
    }

    private bool IsWearingSlot(int slot)
    {
        return slot >= FirstEquippableItemSlotIndex && slot <= LastEquippableItemSlotIndex;
    }

    private async ValueTask UpdateItemsOnChangeAsync(Item item)
    {
        this._player.OnAppearanceChanged();
        await this._player.ForEachWorldObserverAsync<IAppearanceChangedPlugIn>(
            p => p.AppearanceChangedAsync(this._player, item),
            false).ConfigureAwait(false); // in my tests it was not needed to send the appearance to the own players client.

        if (this._player.Attributes is null)
        {
            return;
        }

        var attributes = this._player.Attributes;
        var factory = this._gameContext.ItemPowerUpFactory;
        bool itemIsJewelry = item.IsJewelry();

        if (attributes.ItemPowerUps.TryGetValue(item, out var itemPowerUps))
        {
            if (itemIsJewelry)
            { // Run this before remmoving power ups
                this.HandleJewelryBasePowerUps(item, true);
            }

            attributes.ItemPowerUps.Remove(item);

            foreach (var powerUp in itemPowerUps)
            {
                powerUp.Dispose();
            }
        }

        if (item.ItemSetGroups != null && item.ItemSetGroups.Any())
        {
            this.UpdateSetPowerUps();
        }

        var itemAdded = this.EquippedItems.Contains(item);
        if (itemAdded)
        {
            bool skipBasePowerUps = false;
            if (itemIsJewelry)
            {
                skipBasePowerUps = this.HandleJewelryBasePowerUps(item);
            }

            attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, attributes, skipBasePowerUps, false).ToList());

            // reset player equipped ammunition amount
            if (this.EquippedAmmunitionItem is { } ammoItem)
            {
                attributes[Stats.AmmunitionAmount] = (float)ammoItem.Durability;
            }
        }
    }

    private void InitializePowerUps()
    {
        if (this._player.Attributes is null)
        {
            throw new InvalidOperationException("The players AttributeSystem is not set yet.");
        }

        foreach (var powerUp in this._player.Attributes.ItemPowerUps.Values.SelectMany(p => p).ToList())
        {
            powerUp.Dispose();
        }

        var factory = this._gameContext?.ItemPowerUpFactory;
        if (factory != null)
        {
            foreach (var item in this.EquippedItems)
            {
                bool skipBasePowerUps = false;
                if (item.IsJewelry())
                {
                    skipBasePowerUps = this.HandleJewelryBasePowerUps(item);
                }

                this._player.Attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, this._player.Attributes, skipBasePowerUps, false).ToList());
            }

            this.UpdateSetPowerUps();
        }
        else
        {
            this._player.Logger.LogError("item power up factory not available during initialization of the inventory.");
        }
    }

    private void UpdateSetPowerUps()
    {
        if (this._player.Attributes is null)
        {
            throw new InvalidOperationException("The players AttributeSystem is not set yet.");
        }

        if (this._player.Attributes.ItemSetPowerUps is not null)
        {
            foreach (var powerUp in this._player.Attributes.ItemSetPowerUps)
            {
                powerUp.Dispose();
            }
        }

        var factory = this._gameContext.ItemPowerUpFactory;
        this._player.Attributes.ItemSetPowerUps = factory.GetSetPowerUps(this.EquippedItems, this._player.Attributes, this._player.GameContext.Configuration).ToList();
    }

    /// <summary>
    /// Manages jewelry (pendant and rings) element resistance (fire, poison, ice, etc.) base power up attributes.
    /// </summary>
    /// <param name="item">The context item being (un)equipped.</param>
    /// <param name="onItemUnequip"><c>true</c> if the item is being unequipped; <c>false</c> if it is being equipped.</param>
    /// <returns><c>true</c> if the jewelry item's base power up should be skipped (on item equip); <c>false</c> otherwise.</returns>
    /// <remarks>All jewelry base power up attributes are comprised of one single element resistance. Only the highest resistance value applies (no stacking).</remarks>
    private bool HandleJewelryBasePowerUps(Item item, bool onItemUnequip = false)
    {
        var baseAttribute = item.Definition!.BasePowerUpAttributes.FirstOrDefault()?.TargetAttribute;
        if (baseAttribute is null) // Ring of Magic and Pendant of Ability have no base attributes
        {
            return true;
        }

        var attributes = this._player.Attributes;
        var jewelrySharingAttribute = this.EquippedItems
            .Where(eqItem => eqItem.IsJewelry() && eqItem != item && eqItem.Definition!.BasePowerUpAttributes.FirstOrDefault()?.TargetAttribute == baseAttribute);
        var highestItem = jewelrySharingAttribute.MaxBy(jsa => jsa.Level);

        if (onItemUnequip)
        {
            if (highestItem is not null && attributes!.ItemPowerUps[item].Any(powerUp => powerUp.ParentAttribute!.Definition == baseAttribute))
            { // Unequipped item had the resistance base power up active => load the new highest jewelry item's
                var factory = this._gameContext.ItemPowerUpFactory;
                attributes!.ItemPowerUps[highestItem] = attributes!.ItemPowerUps[highestItem].Concat(factory.GetPowerUps(highestItem, attributes, false, true).ToList()).ToList();
            }
        }
        else if (attributes!.GetComposableAttribute(baseAttribute!) is ComposableAttribute compAttr)
        {
            if (item.Level < highestItem?.Level || (item.Level == highestItem?.Level && compAttr.Elements.Any()))
            { // There is a ring or pendant with higher or equal resistance/level
                return true;
            }

            if (item.Level > highestItem?.Level)
            { // Equipped item has highest resistance
                // Remove all existing elements under the resistance composable attribute
                while (compAttr.Elements.Any())
                {
                    compAttr.RemoveElement(compAttr.Elements.Last());
                }

                // Remove active resistance base power up for previous highest jewelry item
                foreach (var jewelryItem in jewelrySharingAttribute)
                {
                    if (attributes!.ItemPowerUps.TryGetValue(jewelryItem, out IReadOnlyList<PowerUpWrapper>? powerUps) && powerUps.Any(pu => pu.ParentAttribute!.Definition == baseAttribute))
                    {
                        attributes!.ItemPowerUps[jewelryItem] = powerUps.Where(pu => pu.ParentAttribute!.Definition != baseAttribute).ToList();
                        break;
                    }
                }
            }
        }

        return false;
    }
}