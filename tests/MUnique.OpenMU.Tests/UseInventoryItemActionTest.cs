// <copyright file="UseInventoryItemActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="UseInventoryItemAction"/>.
/// </summary>
[TestFixture]
public class UseInventoryItemActionTest
{
    private const byte InUse = 254;
    private const byte NotInUse = 255;
    private const float CharmBonus = 50;

    private readonly UseInventoryItemAction _action = new();

    /// <summary>
    /// The bonus of a charm applies while it's in use.
    /// </summary>
    [Test]
    public async Task BonusAppliesWhileInUseAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IInventoryItemUseChangedPlugIn>()!);

        // A new item, which isn't stackable because its definition has a durability of 1, starts without being in use.
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));

        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);
        Assert.That(charm.Durability, Is.EqualTo(InUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));

        await this._action.SetItemInUseAsync(player, charm.ItemSlot, NotInUse).ConfigureAwait(false);
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));
        view.Verify(v => v.InventoryItemUseChangedAsync(charm), Times.Exactly(2));
    }

    /// <summary>
    /// The same item can only be in use once.
    /// </summary>
    [Test]
    public async Task SameItemCanOnlyBeInUseOnceAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var firstCharm = await AddCharmAsync(player).ConfigureAwait(false);
        var secondCharm = await AddCharmAsync(player, firstCharm.Definition).ConfigureAwait(false);

        await this._action.SetItemInUseAsync(player, firstCharm.ItemSlot, InUse).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, secondCharm.ItemSlot, InUse).ConfigureAwait(false);

        Assert.That(secondCharm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));
    }

    /// <summary>
    /// An item which leaves the inventory, e.g. when it's dropped, stops being in use.
    /// When it's added again, it's not in use, so that it can't add its bonus twice.
    /// </summary>
    [Test]
    public async Task RemovingStopsUseAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);

        var slot = charm.ItemSlot;
        await player.Inventory!.RemoveItemAsync(charm).ConfigureAwait(false);
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));

        Assert.That(await player.Inventory.AddItemAsync(slot, charm).ConfigureAwait(false), Is.True);
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));
    }

    /// <summary>
    /// An item which is moved inside the inventory stays in use, and keeps its bonus during the move,
    /// so that the current health isn't limited by a lower maximum health.
    /// </summary>
    [Test]
    public async Task MovingInsideInventoryKeepsUseAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);
        player.Attributes[Stats.CurrentHealth] = health + CharmBonus;
        var toSlot = (byte)(charm.ItemSlot + 1);

        await new MoveItemAction().MoveItemAsync(player, charm.ItemSlot, Storages.Inventory, toSlot, Storages.Inventory).ConfigureAwait(false);

        Assert.That(charm.ItemSlot, Is.EqualTo(toSlot));
        Assert.That(charm.Durability, Is.EqualTo(InUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));
        Assert.That(player.Attributes[Stats.CurrentHealth], Is.EqualTo(health + CharmBonus));
    }

    /// <summary>
    /// When an item is added which is in use, while another item of the same kind is in use,
    /// it's not in use anymore.
    /// </summary>
    [Test]
    public async Task AddedDuplicateIsNotInUseAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var firstCharm = await AddCharmAsync(player).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, firstCharm.ItemSlot, InUse).ConfigureAwait(false);

        var secondCharm = player.PersistenceContext.CreateNew<Item>();
        secondCharm.Definition = firstCharm.Definition;
        secondCharm.Durability = InUse;
        Assert.That(await player.Inventory!.AddItemAsync(secondCharm).ConfigureAwait(false), Is.True);

        Assert.That(secondCharm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));
    }

    /// <summary>
    /// The items which can be used inside the inventory are configured at the feature plug-in.
    /// </summary>
    [Test]
    public async Task UsableItemsAreConfiguredAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        var plugIn = player.GameContext.FeaturePlugIns.GetPlugIn<InventoryItemUseFeaturePlugIn>();
        Assert.That(plugIn, Is.Not.Null);
        plugIn!.Configuration = new InventoryItemUseConfiguration { Items = [] };

        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);

        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));
    }

    /// <summary>
    /// An item which is moved out of the inventory, e.g. into the vault, stops being in use.
    /// </summary>
    [Test]
    public async Task MovingOutOfInventoryStopsUseAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);
        var vaultStorage = new Mock<ItemStorage>();
        vaultStorage.Setup(i => i.Items).Returns(new List<Item>());
        player.Vault = new Storage(InventoryConstants.WarehouseSize, vaultStorage.Object);
        player.IsVaultLocked = false;
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { NpcWindow = NpcWindow.VaultStorage }, null!);
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false), Is.True);

        await new MoveItemAction().MoveItemAsync(player, charm.ItemSlot, Storages.Inventory, 0, Storages.Vault).ConfigureAwait(false);

        Assert.That(player.Vault!.Items, Does.Contain(charm));
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));
    }

    /// <summary>
    /// Other items and other durability values can't be used inside the inventory.
    /// </summary>
    [Test]
    public async Task OnlyCharmsCanBeUsedAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        var otherDefinition = new BasicModel.ItemDefinition { Group = 13, Number = 20, Width = 1, Height = 1, Durability = 255 };
        otherDefinition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition { TargetAttribute = Stats.MaximumHealth, BaseValue = CharmBonus, AggregateType = AggregateType.AddRaw });
        var other = await AddCharmAsync(player, otherDefinition).ConfigureAwait(false);

        await this._action.SetItemInUseAsync(player, other.ItemSlot, InUse).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, charm.ItemSlot, 100).ConfigureAwait(false);

        Assert.That(other.Durability, Is.EqualTo(NotInUse));
        Assert.That(charm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));
    }

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.FeaturePlugIns.AddPlugIn(new InventoryItemUseFeaturePlugIn(), true);
        return player;
    }

    private static async ValueTask<Item> AddCharmAsync(Player player, ItemDefinition? definition = null)
    {
        if (definition is null)
        {
            definition = new BasicModel.ItemDefinition { Group = 13, Number = 130, Width = 1, Height = 1, Durability = 1 };
            definition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition { TargetAttribute = Stats.MaximumHealth, BaseValue = CharmBonus, AggregateType = AggregateType.AddRaw });
        }

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Durability = definition.Durability;
        Assert.That(await player.Inventory!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        return item;
    }
}
