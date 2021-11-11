// <copyright file="InventoryStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.World;
using static OpenMU.GameLogic.InventoryConstants;

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
        : base(
            GetInventorySize(player),
            EquippableSlotsCount,
            0,
            new ItemStorageAdapter(player.SelectedCharacter?.Inventory ?? throw Error.NotInitializedProperty(player, "SelectedCharacter.Inventory"), FirstEquippableItemSlotIndex, GetInventorySize(player)))
    {
        this._player = player;
        this.EquippedItemsChanged += (sender, eventArgs) => this.UpdateItemsOnChange(eventArgs.Item);
        this._gameContext = context;
        this.InitializePowerUps();
    }

    /// <inheritdoc/>
    public event EventHandler<ItemEventArgs>? EquippedItemsChanged;

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

    /// <inheritdoc />
    /// <remarks>Additionally we need to make a temporary item persistent with the context of the player.</remarks>
    public override bool AddItem(byte slot, Item item)
    {
        Item? convertedItem = null;
        if (item is TemporaryItem temporaryItem)
        {
            convertedItem = temporaryItem.MakePersistent(this._player.PersistenceContext);
        }

        var success = base.AddItem(slot, convertedItem ?? item);
        if (!success && convertedItem != null)
        {
            this._player.PersistenceContext.Delete(convertedItem);
        }

        if (success)
        {
            var isEquippedItem = this.IsWearingSlot(slot);
            if (isEquippedItem)
            {
                this.EquippedItemsChanged?.Invoke(this, new ItemEventArgs(convertedItem ?? item));
            }
        }

        return success;
    }

    /// <inheritdoc />
    public override void RemoveItem(Item item)
    {
        var isEquippedItem = this.IsWearingSlot(item.ItemSlot);
        base.RemoveItem(item);
        if (isEquippedItem)
        {
            this.EquippedItemsChanged?.Invoke(this, new ItemEventArgs(item));
        }
    }

    private bool IsWearingSlot(int slot)
    {
        return slot >= FirstEquippableItemSlotIndex && slot <= LastEquippableItemSlotIndex;
    }

    private void UpdateItemsOnChange(Item item)
    {
        this._player.OnAppearanceChanged();
        this._player.ForEachObservingPlayer(p => p.ViewPlugIns.GetPlugIn<IAppearanceChangedPlugIn>()?.AppearanceChanged(this._player, item), false); // in my tests it was not needed to send the appearance to the own players client.
        if (this._player.Attributes is null)
        {
            return;
        }

        if (this._player.Attributes.ItemPowerUps.TryGetValue(item, out var itemPowerUps))
        {
            this._player.Attributes.ItemPowerUps.Remove(item);
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
            var factory = this._gameContext.ItemPowerUpFactory;
            this._player.Attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, this._player.Attributes).ToList());
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
                this._player.Attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, this._player.Attributes).ToList());
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
}