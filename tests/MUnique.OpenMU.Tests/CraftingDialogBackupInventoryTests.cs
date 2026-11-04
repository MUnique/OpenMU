// <copyright file="CraftingDialogBackupInventoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;

/// <summary>
/// Tests for the backup of the inventory which is taken when a crafting NPC (Chaos Goblin, Pet
/// Trainer, ...) is opened, and restored when the player disconnects before the mix.
/// </summary>
[TestFixture]
public class CraftingDialogBackupInventoryTests
{
    private const int GainedMoney = 1000;

    /// <summary>
    /// Verifies that what the player gains after closing the crafting dialog without a mix is kept
    /// on a disconnect, rather than rolled back to when the dialog was opened.
    /// </summary>
    [Test]
    public async ValueTask GainsAfterClosingTheCraftingDialogSurviveADisconnectAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = InventoryOf(player).ItemStorage;
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);

        var loot = CreateItem();
        Assert.That(await InventoryOf(player).AddItemAsync(loot).ConfigureAwait(false), Is.True);
        inventory.Money = GainedMoney;

        await player.RemoveFromGameAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items, Does.Contain(loot));
            Assert.That(inventory.Money, Is.EqualTo(GainedMoney));
        });
    }

    /// <summary>
    /// Verifies that an item left in the machine when its dialog was closed is returned to the
    /// inventory on a disconnect.
    /// </summary>
    [Test]
    public async ValueTask ItemLeftInTheMachineIsReturnedOnADisconnectAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = InventoryOf(player).ItemStorage;
        var ingredient = CreateItem();
        Assert.That(await InventoryOf(player).AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);
        await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);

        Assert.That(inventory.Items.Count(item => ReferenceEquals(item, ingredient)), Is.EqualTo(1));
    }

    /// <summary>
    /// Verifies that a disconnect while the crafting dialog is still open restores the backup, which
    /// puts the item in the machine back into its inventory slot.
    /// </summary>
    [Test]
    public async ValueTask DisconnectWhileTheCraftingDialogIsOpenRestoresTheBackupAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = InventoryOf(player).ItemStorage;
        var ingredient = CreateItem();
        Assert.That(await InventoryOf(player).AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        var ingredientSlot = ingredient.ItemSlot;
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);
        Assert.That(player.BackupInventory, Is.Not.Null);

        await player.RemoveFromGameAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items.Count(item => ReferenceEquals(item, ingredient)), Is.EqualTo(1));
            Assert.That(ingredient.ItemSlot, Is.EqualTo(ingredientSlot));
            Assert.That(player.BackupInventory, Is.Null);
        });
    }

    private static async ValueTask PutIntoTheMachineAsync(Player player, Item item)
    {
        await InventoryOf(player).RemoveItemAsync(item).ConfigureAwait(false);
        Assert.That(await TemporaryStorageOf(player).AddItemAsync(0, item).ConfigureAwait(false), Is.True);
    }

    private static IInventoryStorage InventoryOf(Player player)
    {
        return player.Inventory ?? throw new AssertionException("The player has no inventory.");
    }

    private static IStorage TemporaryStorageOf(Player player)
    {
        return player.TemporaryStorage ?? throw new AssertionException("The player has no temporary storage.");
    }

    private static NonPlayerCharacter CreateCraftingNpc()
    {
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<ItemCrafting> { new() });
        definition.Object.NpcWindow = NpcWindow.ChaosMachine;
        return new NonPlayerCharacter(null!, definition.Object, null!);
    }

    private static Item CreateItem()
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = new ItemDefinition { Width = 1, Height = 1, Durability = 1 };
        item.Object.Durability = 1;
        return item.Object;
    }
}
