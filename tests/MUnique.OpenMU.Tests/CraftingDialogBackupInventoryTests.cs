// <copyright file="CraftingDialogBackupInventoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
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

            Assert.That(player.OpenedNpc, Is.Null);
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
                Assert.That(target.OpenedNpc, Is.Null);
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

    /// <summary>A real victim packet can enter during the ordinary close delay, but cannot reopen a reserved death.</summary>
    [Test]
    public async Task ReservedDeathRejectsNpcReopenThroughPersistenceFunnelAsync()
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = seed.GameContext.Configuration;
        var provider = new PausingContextProvider(configuration);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, provider, initializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        attacker.Id = 11;
        var npc = CreateCraftingNpc();
        npc.Definition.Number = 238;
        await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
        var closing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(target.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>()!).Setup(v => v.DialogClosedAsync(npc.Definition)).Returns(() =>
        {
            closing.TrySetResult();
            return ValueTask.CompletedTask;
        });
        var requestCompleted = 0;
        var requestPrecededSave = false;
        ((PausingPlayerContext)target.PersistenceContext).BeforeSave = () =>
        {
            requestPrecededSave = Volatile.Read(ref requestCompleted) == 1;
            return ValueTask.CompletedTask;
        };
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        // The attacker holds only its own packet funnel, not the victim's.
        var lethal = Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(
            () => target.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false));
        try
        {
            await closing.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            // Spawn from the independent test coordinator, never from the lock-owning callback.
            await Task.Run(async () => await target.RunPersistenceExclusiveAsync(
                () => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Volatile.Write(ref requestCompleted, 1);
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(requestPrecededSave, Is.True, "The victim request ran in the close-delay window, before saving held its funnel.");
                Assert.That(target.OpenedNpc, Is.Null);
                Assert.That(target.BackupInventory, Is.Null);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(target.LastDeath?.KillerId, Is.EqualTo(attacker.Id));
                Assert.That(deaths, Is.EqualTo(1));
            });
        }
        finally
        {
            await lethal.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A pending invitation never owns items left by a closed crafting window.</summary>
    /// <param name="departure">The actual lifecycle or ordinary trade completion.</param>
    [TestCase("logout")]
    [TestCase("disconnect")]
    [TestCase("death")]
    [TestCase("placement")]
    [TestCase("cancel")]
    [TestCase("accept")]
    [TestCase("no-request")]
    public async Task PendingInvitationPreservesClosedCraftingItemsAsync(string departure)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        owner.Attributes![Stats.CurrentHealth] = 10;
        peer.Attributes![Stats.CurrentHealth] = 100;
        var inventory = owner.Inventory!;
        var character = owner.SelectedCharacter!;
        owner.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = owner.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
        var ingredient = owner.PersistenceContext.CreateNew<Item>();
        ingredient.Definition = new ItemDefinition { Width = 1, Height = 1, Durability = 9, IsTradable = true };
        ingredient.Durability = 7;
        ingredient.Level = 3;
        var id = ingredient.GetId();
        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(await inventory.AddItemAsync(20, ingredient).ConfigureAwait(false), Is.True);
        var temporary = owner.TemporaryStorage!;
        try
        {
            var npc = CreateCraftingNpc();
            npc.Definition.Number = 238;
            npc.Definition.ItemCraftings.Single().SimpleCraftingSettings = owner.PersistenceContext.CreateNew<SimpleCraftingSettings>();
            await owner.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(owner, npc)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.ChaosMachine)).ConfigureAwait(false);
            Assert.That(temporary.Items.Single(), Is.SameAs(ingredient));
            await owner.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(owner)).ConfigureAwait(false);
            inventory.ItemStorage.Money = 1234; // Post-close gains must not roll back to the crafting snapshot.
            Assert.That(owner.BackupInventory, Is.Null);
            if (departure != "no-request")
            {
                Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false), Is.True);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                Assert.That(owner.BackupInventory, Is.Null);
            }

            switch (departure)
            {
                case "logout":
                    await owner.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                    break;
                case "disconnect":
                    Assert.That(owner.IsDisposing, Is.False);
                    await owner.DisconnectAsync().ConfigureAwait(false);
                    break;
                case "death":
                    await peer.RunPersistenceExclusiveAsync(() => owner.ApplyPoisonDamageAsync(peer, 10)).ConfigureAwait(false);
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                    break;
                case "placement":
                    await owner.RespawnAtAsync(new ExitGate { Map = owner.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
                    break;
                case "accept":
                    await owner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(owner, true)).ConfigureAwait(false);
                    Assert.That(owner.BackupInventory!.Items, Does.Contain(ingredient));
                    await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, ingredient.ItemSlot, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
                    await owner.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(owner, 60)).ConfigureAwait(false);
                    await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
                    break;
                case "cancel":
                    await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
                    break;
                case "no-request":
                    break;
                default:
                    throw new AssertionException($"Unknown {nameof(departure)}.");
            }

            // Check conservation before teardown can hide which cancellation erased the item.
            Assert.Multiple(() =>
            {
                Assert.That(inventory.Items.Concat(temporary.Items).Count(i => i.GetId() == id), Is.EqualTo(1));
                Assert.That(peer.Inventory!.Items.Concat(peer.TemporaryStorage!.Items).Any(i => i.GetId() == id), Is.False);
                Assert.That(ingredient.Level, Is.EqualTo(3));
                Assert.That(ingredient.Durability, Is.EqualTo(7));
                Assert.That(inventory.ItemStorage.Money, Is.EqualTo(1234));
                Assert.That(owner.BackupInventory, Is.Null);
                if (departure != "logout")
                {
                    Assert.That(owner.TradingPartner, Is.Null);
                    Assert.That(peer.TradingPartner, Is.Null);
                }
            });
            if (departure == "logout")
            {
                // Clearing the pending request of a player who leaves the world is lifecycle cleanup, not owned here:
                // the peer leaves the stale request by an ordinary cancel, which neither revives nor touches the owner.
                await peer.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(peer)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(peer.TradingPartner, Is.Null);
                    Assert.That(owner.PlayerState.CurrentState, Is.Not.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(inventory.Items.Concat(temporary.Items).Count(i => i.GetId() == id), Is.EqualTo(1));
                    Assert.That(peer.Inventory!.Items.Concat(peer.TemporaryStorage!.Items).Any(i => i.GetId() == id), Is.False);
                    Assert.That(inventory.ItemStorage.Money, Is.EqualTo(1234));
                });
            }

            if (owner.SelectedCharacter is not null)
            {
                await owner.RemoveFromGameAsync().ConfigureAwait(false);
            }

            Assert.That(character.Inventory!.Items.Count(i => i.GetId() == id), Is.EqualTo(1));
            Assert.That(character.Inventory.Money, Is.EqualTo(1234));
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A failed all-item return must not turn old crafting items into trade-owned escrow.</summary>
    /// <param name="freeSlots">Zero denies immediately; one exercises partial transfer and rollback.</param>
    [TestCase(0)]
    [TestCase(1)]
    public async Task FullInventoryRefusesTradeWithoutOwningCraftingItemsAsync(int freeSlots)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        var inventory = owner.Inventory!;
        var temporary = owner.TemporaryStorage!;
        var definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true, Durability = 9 };
        var ingredients = new[] { owner.PersistenceContext.CreateNew<Item>(), owner.PersistenceContext.CreateNew<Item>() };
        var fillers = new List<Item>();
        try
        {
            var npc = CreateCraftingNpc();
            npc.Definition.Number = 238;
            npc.Definition.ItemCraftings.Single().SimpleCraftingSettings = owner.PersistenceContext.CreateNew<SimpleCraftingSettings>();
            await owner.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(owner, npc)).ConfigureAwait(false);
            for (byte index = 0; index < ingredients.Length; index++)
            {
                var item = ingredients[index];
                item.Definition = definition;
                item.Level = 3;
                item.Durability = 7;
                Assert.That(await inventory.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
                await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, index, Storages.ChaosMachine)).ConfigureAwait(false);
            }

            // Closing returns the tray to the inventory or the vault; only when both are full do the items stay.
            for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
            {
                var filler = owner.PersistenceContext.CreateNew<Item>();
                filler.Definition = definition;
                fillers.Add(filler);
                Assert.That(await inventory.AddItemAsync(slot, filler).ConfigureAwait(false), Is.True);
            }

            await FillAsync(new Storage(InventoryConstants.WarehouseSize, SetUpVault(owner))).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(owner)).ConfigureAwait(false);
            Assert.That(temporary.Items, Is.EquivalentTo(ingredients));
            foreach (var filler in fillers.TakeLast(freeSlots).ToList())
            {
                await inventory.RemoveItemAsync(filler).ConfigureAwait(false);
                fillers.Remove(filler);
            }

            owner.Money = 1234;
            var ids = inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order().ToArray();
            Assert.That(ids.Distinct().Count(), Is.EqualTo(ids.Length));
            Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false), Is.True);
            await owner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(owner, true)).ConfigureAwait(false);
            var stateAfterAcceptance = owner.PlayerState.CurrentState;
            var backupAfterAcceptance = owner.BackupInventory;
            // A later real cancel must not erase items even if a failed return wrongly opened the trade.
            await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(stateAfterAcceptance, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(backupAfterAcceptance, Is.Null);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(owner.Money, Is.EqualTo(1234));
                Assert.That(inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order(), Is.EqualTo(ids));
                Assert.That(temporary.Items.Select(i => i.GetId()), Is.EquivalentTo(ingredients.Select(i => i.GetId())));
                Assert.That(ingredients.All(i => i.Level == 3 && i.Durability == 7), Is.True);
            });
        }
        finally
        {
            // Leave room for ordinary teardown only after all conservation checks.
            foreach (var filler in fillers)
            {
                await inventory.RemoveItemAsync(filler).ConfigureAwait(false);
            }

            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static ValueTask RespawnAsync(Player player)
        => player.RespawnAfterDeathAsync(new ExitGate { Map = player.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });

    /// <summary>
    /// Verifies that an item left in the machine is back in the inventory right after the dialog is
    /// closed, instead of staying hidden in the temporary storage until the next login.
    /// </summary>
    [Test]
    public async ValueTask ItemLeftInTheMachineIsReturnedWhenTheDialogIsClosedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = InventoryOf(player).ItemStorage;
        var ingredient = CreateItem();
        Assert.That(await InventoryOf(player).AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);

        await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.Items.Count(item => ReferenceEquals(item, ingredient)), Is.EqualTo(1));
            Assert.That(TemporaryStorageOf(player).Items, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies that an item left in the machine is not lost when the inventory got full in the meantime:
    /// it goes to the vault. A player lost an excellent staff this way, when he disconnected with
    /// the machine open after he moved other items into the inventory.
    /// </summary>
    [Test]
    public async ValueTask ItemLeftInTheMachineGoesToTheVaultWhenTheInventoryIsFullAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var vault = SetUpVault(player);
        var inventory = InventoryOf(player);
        var ingredient = CreateItem();
        Assert.That(await inventory.AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);
        player.BackupInventory = null; // only the items of the tray are left, as after a dialog which was closed
        await FillAsync(inventory).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.ItemStorage.Items, Does.Not.Contain(ingredient));
            Assert.That(vault.Items.Count(item => ReferenceEquals(item, ingredient)), Is.EqualTo(1));
            Assert.That(player.TemporaryStorage, Is.Null);
        });
    }

    /// <summary>
    /// Verifies that an item left in the machine goes to the vault when the dialog is closed while
    /// the inventory has no space for it (same placement as when the player disconnects).
    /// </summary>
    [Test]
    public async ValueTask ItemLeftInTheMachineGoesToTheVaultWhenTheDialogIsClosedWithAFullInventoryAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var vault = SetUpVault(player);
        var inventory = InventoryOf(player);
        var ingredient = CreateItem();
        Assert.That(await inventory.AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);
        await FillAsync(inventory).ConfigureAwait(false);

        await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.ItemStorage.Items, Does.Not.Contain(ingredient));
            Assert.That(vault.Items.Count(item => ReferenceEquals(item, ingredient)), Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Verifies that an item left in the machine is never dropped from the temporary storage when neither
    /// the inventory nor the vault have space for it, so that it can be returned later.
    /// </summary>
    [Test]
    public async ValueTask ItemLeftInTheMachineStaysInTheTemporaryStorageWhenNothingHasSpaceAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var vault = SetUpVault(player);
        var inventory = InventoryOf(player);
        var ingredient = CreateItem();
        Assert.That(await inventory.AddItemAsync(ingredient).ConfigureAwait(false), Is.True);
        await new TalkNpcAction().TalkToNpcAsync(player, CreateCraftingNpc()).ConfigureAwait(false);
        await PutIntoTheMachineAsync(player, ingredient).ConfigureAwait(false);
        player.BackupInventory = null; // only the items of the tray are left, as after a dialog which was closed
        await FillAsync(inventory).ConfigureAwait(false);
        await FillAsync(new Storage(InventoryConstants.WarehouseSize, vault)).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(inventory.ItemStorage.Items, Does.Not.Contain(ingredient));
            Assert.That(vault.Items, Does.Not.Contain(ingredient));
            Assert.That(TemporaryStorageOf(player).Items, Does.Contain(ingredient));
        });
    }

    private static ItemStorage SetUpVault(Player player)
    {
        var vaultMock = new Mock<ItemStorage>();
        vaultMock.SetupAllProperties();
        vaultMock.Setup(v => v.Items).Returns(new List<Item>());
        var vault = vaultMock.Object;
        Mock.Get(player.Account!).SetupProperty(account => account.Vault, vault);
        return vault;
    }

    private static async ValueTask FillAsync(IStorage storage)
    {
        while (await storage.AddItemAsync(CreateItem()).ConfigureAwait(false))
        {
            // Adds items until there is no space left.
        }
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
