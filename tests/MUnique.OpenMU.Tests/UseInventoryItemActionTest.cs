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
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<IInventoryItemUseChangedPlugIn>()!);

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
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var firstCharm = await AddCharmAsync(player).ConfigureAwait(false);
        var secondCharm = await AddCharmAsync(player, firstCharm.Definition).ConfigureAwait(false);

        await this._action.SetItemInUseAsync(player, firstCharm.ItemSlot, InUse).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, secondCharm.ItemSlot, InUse).ConfigureAwait(false);

        Assert.That(secondCharm.Durability, Is.EqualTo(NotInUse));
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));
    }

    /// <summary>
    /// The bonus is removed when the item is removed from the inventory, e.g. at the start of a move,
    /// and applies again when it's added while in use.
    /// </summary>
    [Test]
    public async Task BonusFollowsTheItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var health = player.Attributes![Stats.MaximumHealth];
        var charm = await AddCharmAsync(player).ConfigureAwait(false);
        await this._action.SetItemInUseAsync(player, charm.ItemSlot, InUse).ConfigureAwait(false);

        var slot = charm.ItemSlot;
        await player.Inventory!.RemoveItemAsync(charm).ConfigureAwait(false);
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health));

        Assert.That(await player.Inventory.AddItemAsync(slot, charm).ConfigureAwait(false), Is.True);
        Assert.That(player.Attributes[Stats.MaximumHealth], Is.EqualTo(health + CharmBonus));
    }

    /// <summary>
    /// An item which is moved out of the inventory, e.g. into the vault, stops being in use.
    /// </summary>
    [Test]
    public async Task MovingOutOfInventoryStopsUseAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
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
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
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

    private static async ValueTask<Item> AddCharmAsync(Player player, ItemDefinition? definition = null)
    {
        if (definition is null)
        {
            definition = new BasicModel.ItemDefinition { Group = 13, Number = 130, Width = 1, Height = 1, Durability = 255 };
            definition.BasePowerUpAttributes.Add(new BasicModel.ItemBasePowerUpDefinition { TargetAttribute = Stats.MaximumHealth, BaseValue = CharmBonus, AggregateType = AggregateType.AddRaw });
        }

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Durability = NotInUse;
        Assert.That(await player.Inventory!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        return item;
    }
}
