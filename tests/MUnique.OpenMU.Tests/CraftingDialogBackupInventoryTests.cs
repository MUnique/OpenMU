// <copyright file="CraftingDialogBackupInventoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

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

    /// <summary>
    /// An old crafting response cannot publish a backup after close or death, including reopening the same NPC.
    /// </summary>
    /// <param name="lethal">Whether a lethal hit closes the first interaction.</param>
    /// <param name="reopen">Whether the same NPC is reopened before the old response resumes.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task CancelledCraftingResponseCannotRestoreStaleInventoryAsync(bool lethal, bool reopen)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 10;
        var inventory = InventoryOf(player).ItemStorage;
        inventory.Money = 1234;
        var npc = CreateCraftingNpc();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var responses = 0;
        Mock.Get(player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()!)
            .Setup(v => v.OpenNpcWindowAsync(NpcWindow.ChaosMachine)).Returns(async () =>
            {
                if (Interlocked.Increment(ref responses) == 1)
                {
                    entered.TrySetResult();
                    await release.Task.ConfigureAwait(false);
                }
            });
        var response = new TalkNpcAction().TalkToNpcAsync(player, npc).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (lethal)
            {
                await player.ApplyPoisonDamageAsync(player, 10).ConfigureAwait(false);
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
            }
            else
            {
                await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);
            }

            Assert.That(player.OpenedNpcContext, Is.Null);
            Assert.That(player.BackupInventory, Is.Null);
            BackupItemStorage? newBackup = null;
            if (reopen)
            {
                if (lethal)
                {
                    await RespawnAsync(player).ConfigureAwait(false);
                }

                await new TalkNpcAction().TalkToNpcAsync(player, npc).ConfigureAwait(false);
                newBackup = player.BackupInventory;
                Assert.That(newBackup, Is.Not.Null);
            }

            release.TrySetResult();
            await response.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var backupAfterResponse = player.BackupInventory;
            if (reopen)
            {
                Assert.That(player.OpenedNpc, Is.SameAs(npc));
                await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);
            }
            else if (lethal)
            {
                await RespawnAsync(player).ConfigureAwait(false);
            }
            else
            {
                // Normal close already left this player alive in EnteredWorld.
            }

            var loot = CreateItem();
            Assert.That(await InventoryOf(player).AddItemAsync(loot).ConfigureAwait(false), Is.True);
            inventory.Money = GainedMoney;
            await player.RemoveFromGameAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(inventory.Items, Does.Contain(loot));
                Assert.That(inventory.Money, Is.EqualTo(GainedMoney));
                Assert.That(backupAfterResponse, Is.SameAs(newBackup));
                Assert.That(player.BackupInventory, Is.Null);
            });
        }
        finally
        {
            release.TrySetResult();
            await response.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A dying player cannot reopen the Chaos Goblin while ordinary closing persistence waits.</summary>
    [Test]
    public async Task ReservedDeathCannotReopenNpcDuringCloseSaveAsync()
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = seed.GameContext.Configuration;
        var provider = new PausingContextProvider(configuration);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, provider, initializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var npc = CreateCraftingNpc();
        npc.Definition.Number = 238;
        await new TalkNpcAction().TalkToNpcAsync(target, npc).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        ((PausingPlayerContext)target.PersistenceContext).BeforeSave = async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        };
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        var lethal = target.ApplyPoisonDamageAsync(target, 10).AsTask();
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            await new TalkNpcAction().TalkToNpcAsync(target, npc).ConfigureAwait(false);
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.OpenedNpcContext, Is.Null);
                Assert.That(target.BackupInventory, Is.Null);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(deaths, Is.EqualTo(1));
            });
        }
        finally
        {
            release.TrySetResult();
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static ValueTask RespawnAsync(Player player)
        => player.RespawnAfterDeathAsync(new ExitGate { Map = player.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });

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
    private sealed class PausingContextProvider(GameConfiguration expectedConfiguration) : InMemoryPersistenceContextProvider, IPersistenceContextProvider
    {
        IPlayerContext IPersistenceContextProvider.CreateNewPlayerContext(GameConfiguration configuration)
        {
            Assert.That(configuration, Is.SameAs(expectedConfiguration));
            return new PausingPlayerContext((InMemoryRepositoryProvider)this.RepositoryProvider);
        }
    }

    private sealed class PausingPlayerContext(InMemoryRepositoryProvider provider) : PlayerInMemoryContext(provider), IPlayerContext
    {
        public Func<ValueTask>? BeforeSave { get; set; }

        async ValueTask<bool> IContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (this.BeforeSave is { } beforeSave)
            {
                this.BeforeSave = null;
                await beforeSave().ConfigureAwait(false);
            }

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

}
