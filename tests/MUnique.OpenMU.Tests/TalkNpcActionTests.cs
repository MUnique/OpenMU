// <copyright file="TalkNpcActionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.GameLogic.Views.Vault;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Vault;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>NPC response continuations must not recreate a dialog closed by an independent lethal flow.</summary>
[TestFixture]
public class TalkNpcActionTests
{
    /// <summary>Merchant delay and open-response waits retain the same live dialog before sending further packets.</summary>
    /// <param name="lethal">Whether an independent attacker closes the dialog.</param>
    /// <param name="pauseOpen">Whether the first open result has already started instead of waiting in the initial delay.</param>
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task MerchantResponseCannotOutliveDialogAsync(bool lethal, bool pauseOpen)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 10;
        var npc = CreateNpc(NpcWindow.Merchant);
        var store = new Mock<ItemStorage>();
        store.Setup(s => s.Items).Returns(new List<Item> { new() { Definition = new ItemDefinition { Width = 1, Height = 1 } } });
        npc.Definition.MerchantStore = store.Object;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var opened = 0;
        var lists = 0;
        var visible = false;
        player.PlayerState.StateChanged += args =>
        {
            if (!pauseOpen && args.CurrentStateState == PlayerState.NpcDialogOpened)
            {
                entered.TrySetResult();
            }

            return ValueTask.CompletedTask;
        };
        Mock.Get(player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()!).Setup(v => v.OpenNpcWindowAsync(NpcWindow.Merchant)).Returns(async () =>
        {
            Interlocked.Increment(ref opened);
            visible = true;
            if (pauseOpen)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        });
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()!).Setup(v => v.ShowMerchantStoreItemListAsync(It.IsAny<ICollection<Item>>(), StoreKind.Normal)).Returns(() =>
        {
            Interlocked.Increment(ref lists);
            return ValueTask.CompletedTask;
        });
        Mock.Get(player.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>()!).Setup(v => v.DialogClosedAsync(npc.Definition)).Returns(() =>
        {
            visible = false;
            return ValueTask.CompletedTask;
        });
        var request = Task.Run(async () => await player.RunPersistenceExclusiveAsync(
            () => new TalkNpcAction().TalkToNpcAsync(player, npc)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (lethal)
            {
                await Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(
                    () => player.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            release.TrySetResult();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(opened, Is.EqualTo(lethal && !pauseOpen ? 0 : 1));
                Assert.That(lists, Is.EqualTo(lethal ? 0 : 1));
                Assert.That(visible, Is.EqualTo(!lethal));
                Assert.That(player.OpenedNpc, Is.SameAs(lethal ? null : npc));
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(lethal ? PlayerState.Dead : PlayerState.NpcDialogOpened));
                if (lethal)
                {
                    Assert.That(player.LastDeath?.KillerId, Is.EqualTo(attacker.Id));
                }
            });
        }
        finally
        {
            release.TrySetResult();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Vault metadata work after initial state publication cannot recreate a handle cleared by death.</summary>
    /// <param name="lethal">Whether an independent attacker closes the dialog while metadata is read.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task VaultPublicationCannotOutliveDialogAsync(bool lethal)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 10;
        var npc = CreateNpc(NpcWindow.VaultStorage);
        var persistentVault = new Mock<ItemStorage>();
        persistentVault.Setup(v => v.Items).Returns(new List<Item>());
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var reads = 0;
        Mock.Get(player.Account!).Setup(a => a.Vault).Returns(() =>
        {
            // A virtual persisted-storage getter may perform synchronous lazy loading after state publication.
            if (Interlocked.Increment(ref reads) == 1)
            {
                entered.TrySetResult();
                if (!release.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("Vault storage load was not released.");
                }
            }

            return persistentVault.Object;
        });
        var shows = 0;
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowVaultPlugIn>()!).Setup(v => v.ShowVaultAsync()).Returns(() =>
        {
            Interlocked.Increment(ref shows);
            return ValueTask.CompletedTask;
        });
        var request = Task.Run(async () => await player.RunPersistenceExclusiveAsync(
            () => new TalkNpcAction().TalkToNpcAsync(player, npc)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (lethal)
            {
                await Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(
                    () => player.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(player.OpenedNpc, Is.Null);
                Assert.That(player.Vault, Is.Null);
            }

            release.Set();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(shows, Is.EqualTo(lethal ? 0 : 1));
                Assert.That(player.Vault is null, Is.EqualTo(lethal));
                Assert.That(player.OpenedNpc, Is.SameAs(lethal ? null : npc));
                Assert.That(player.Account!.Vault, Is.SameAs(persistentVault.Object));
            });
        }
        finally
        {
            release.Set();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The concrete vault view stops its list, money and state continuations after an independent close.</summary>
    /// <param name="stage">The actual awaited view boundary.</param>
    /// <param name="lethal">Whether independent lethal flow closes the vault.</param>
    [TestCase("open", false)]
    [TestCase("open", true)]
    [TestCase("list", false)]
    [TestCase("list", true)]
    [TestCase("money", false)]
    [TestCase("money", true)]
    public async Task ConcreteVaultResponseStopsAfterCloseAsync(string stage, bool lethal)
    {
        var logger = new Mock<ILogger>();
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        using var game = CreateServerContext(factory.Object);
        var player = new VaultViewPlayer(game, new Mock<IConnection>().Object);
        await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        player.Attributes![Stats.CurrentHealth] = 10;
        var storage = new Mock<ItemStorage>();
        storage.Setup(s => s.Items).Returns(new List<Item>());
        Mock.Get(player.Account!).SetupProperty(a => a.Vault, storage.Object);
        var npc = CreateNpc(NpcWindow.VaultStorage);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var lists = 0;
        var money = 0;
        var states = 0;
        async ValueTask PauseAsync(string boundary)
        {
            if (boundary == stage)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        }

        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowVaultPlugIn>()!).Setup(v => v.ShowVaultAsync()).Returns(() => new ShowVaultPlugIn(player).ShowVaultAsync());
        Mock.Get(player.ViewPlugIns.GetPlugIn<IOpenNpcWindowPlugIn>()!).Setup(v => v.OpenNpcWindowAsync(NpcWindow.VaultStorage)).Returns(() => PauseAsync("open"));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()!).Setup(v => v.ShowMerchantStoreItemListAsync(It.IsAny<ICollection<Item>>(), StoreKind.Normal)).Returns(() =>
        {
            Interlocked.Increment(ref lists);
            return PauseAsync("list");
        });
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()!).Setup(v => v.UpdateVaultMoneyAsync(true)).Returns(() =>
        {
            Interlocked.Increment(ref money);
            return PauseAsync("money");
        });
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdateVaultStatePlugIn>()!).Setup(v => v.UpdateStateAsync()).Returns(() =>
        {
            Interlocked.Increment(ref states);
            return ValueTask.CompletedTask;
        });
        logger.Invocations.Clear();
        var request = Task.Run(async () => await player.RunPersistenceExclusiveAsync(
            () => new TalkNpcAction().TalkToNpcAsync(player, npc)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (lethal)
            {
                await Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(
                    () => player.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            release.TrySetResult();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(lists, Is.EqualTo(lethal && stage == "open" ? 0 : 1));
                Assert.That(money, Is.EqualTo(lethal && stage != "money" ? 0 : 1));
                Assert.That(states, Is.EqualTo(lethal ? 0 : 1));
                Assert.That(player.Vault is null, Is.EqualTo(lethal));
                Assert.That(logger.Invocations.Any(i => i.Arguments.Any(a => a is Exception)), Is.False, "Cleared storage must be skipped, not dereferenced and swallowed by InvokeViewPlugInAsync.");
            });
        }
        finally
        {
            release.TrySetResult();
            await request.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Legacy merchant state and legacy vault state remain compatible with guarded publication.</summary>
    /// <param name="vault">Whether the NPC opens the vault rather than a merchant.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task LegacyMerchantAndVaultRemainUsableAsync(bool vault)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var npc = CreateNpc(vault ? NpcWindow.VaultStorage : NpcWindow.Merchant);
        var storage = new Mock<ItemStorage>();
        storage.Setup(s => s.Items).Returns(new List<Item> { new() { Definition = new ItemDefinition { Width = 1, Height = 1 } } });
        if (vault)
        {
            Mock.Get(player.Account!).SetupProperty(a => a.Vault, storage.Object);
        }
        else
        {
            npc.Definition.MerchantStore = storage.Object;
        }

        try
        {
            await player.RunPersistenceExclusiveAsync(() => new TalkNpcAction075().TalkToNpcAsync(player, npc)).ConfigureAwait(false);
            Assert.That(player.OpenedNpc, Is.SameAs(npc));
            Assert.That(player.PlayerState.CurrentState, Is.EqualTo(vault ? PlayerState.NpcDialogOpened : PlayerState.EnteredWorld));
            Assert.That(player.Vault is not null, Is.EqualTo(vault));
            if (vault)
            {
                Mock.Get(player.ViewPlugIns.GetPlugIn<IShowVaultPlugIn>()!).Verify(v => v.ShowVaultAsync(), Times.Once);
            }
            else
            {
                Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()!).Verify(v => v.ShowMerchantStoreItemListAsync(It.IsAny<ICollection<Item>>(), StoreKind.Normal), Times.Once);
            }
        }
        finally
        {
            await player.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A delayed external merchant close cannot clear a successor dialog's context, vault or crafting snapshot.</summary>
    /// <param name="crafting">Whether the successor owns a crafting inventory backup rather than vault storage.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task ExternalCloseCannotClearSuccessorDialogAsync(bool crafting)
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var release = new ManualResetEventSlim();
        var first = 0;
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Debug && i.Arguments[2].ToString()!.Contains("closes NPC", StringComparison.Ordinal)
                    && Interlocked.Exchange(ref first, 1) == 0)
                {
                    entered.TrySetResult();
                    Assert.That(release.Wait(TimeSpan.FromSeconds(5)), Is.True);
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        using var game = CreateServerContext(factory.Object);
        var player = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        player.Money = 200;
        var oldNpc = CreateNpc(NpcWindow.Merchant);
        var next = CreateNpc(crafting ? NpcWindow.ChaosMachine : NpcWindow.VaultStorage);
        next.Definition.Number = 239;
        if (crafting)
        {
            next.Definition.ItemCraftings.Add(new MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting());
        }
        else
        {
            var storage = new Mock<ItemStorage>();
            storage.Setup(s => s.Items).Returns(new List<Item>());
            Mock.Get(player.Account!).SetupProperty(a => a.Vault, storage.Object);
        }

        await player.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(player, oldNpc)).ConfigureAwait(false);
        var closing = Task.Run(async () => await new CloseNpcDialogAction().CloseNpcDialogAsync(player).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(player, next)).ConfigureAwait(false);
            var dialog = player.OpenedNpcContext;
            var vault = player.Vault;
            var backup = player.BackupInventory;
            release.Set();
            await closing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.NpcDialogOpened));
                Assert.That(player.OpenedNpcContext, Is.SameAs(dialog));
                Assert.That(player.OpenedNpc, Is.SameAs(next));
                Assert.That(player.Vault, Is.SameAs(vault));
                Assert.That(player.BackupInventory, Is.SameAs(backup));
            });
            await player.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(player)).ConfigureAwait(false);
            Assert.That(player.BackupInventory, Is.Null);
            Assert.That(player.Vault, Is.Null);
            var inventory = player.Inventory!;
            player.Money = 300;
            await player.RemoveFromGameAsync().ConfigureAwait(false);
            Assert.That(inventory.ItemStorage.Money, Is.EqualTo(300), "Later character teardown cannot restore a successor's stale crafting snapshot.");
        }
        finally
        {
            release.Set();
            await closing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await player.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Every preparation exit restores normal NPC, party and trade admission without changing living flags.</summary>
    /// <param name="exit">The successful, refused or exceptional preparation exit.</param>
    [TestCase("success")]
    [TestCase("refused")]
    [TestCase("exception")]
    public async Task ForcedPreparationReleasesAdmissionAsync(string exit)
    {
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        var failure = new InvalidOperationException("close logging failed");
        var failed = 0;
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if (exit == "exception" && i.Arguments[2].ToString()!.Contains("closes NPC", StringComparison.Ordinal)
                    && Interlocked.Exchange(ref failed, 1) == 0)
                {
                    throw failure;
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        using var game = CreateServerContext(factory.Object);
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var npc = CreateNpc(NpcWindow.ChaosMachine);
        npc.Definition.Number = 239;
        try
        {
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
            if (exit == "refused")
            {
                var checks = 0;
                Assert.That(await target.PrepareForForcedPlacementAsync(() => Interlocked.Increment(ref checks) == 1).ConfigureAwait(false), Is.False);
            }
            else if (exit == "exception")
            {
                var observed = Assert.ThrowsAsync<InvalidOperationException>(async () => await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false));
                Assert.That(observed, Is.SameAs(failure));
            }
            else
            {
                await target.RespawnAtAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
            }

            Assert.That(target.IsPreparingForcedPlacement, Is.False);
            Assert.That(target.IsAlive, Is.True);
            if (target.OpenedNpc is not null)
            {
                await target.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(target)).ConfigureAwait(false);
            }

            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
            Assert.That(target.OpenedNpc, Is.SameAs(npc));
            await target.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(target)).ConfigureAwait(false);
            await peer.RunPersistenceExclusiveAsync(() => new PartyRequestAction().HandlePartyRequestAsync(peer, target)).ConfigureAwait(false);
            Assert.That(target.LastPartyRequester, Is.SameAs(peer));
            await target.RunPersistenceExclusiveAsync(() => new PartyResponseAction().HandleResponseAsync(target, false)).ConfigureAwait(false);
            Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, target)).ConfigureAwait(false), Is.True);
            await target.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(target)).ConfigureAwait(false);
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Old-life completion cannot release replacement-life preparation or admit new interactions into it.</summary>
    [Test]
    public async Task ReplacementPreparationKeepsItsAdmissionOwnerAsync()
    {
        var entered = new[] { new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously), new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously) };
        using var oldRelease = new ManualResetEventSlim();
        using var newRelease = new ManualResetEventSlim();
        var releases = new[] { oldRelease, newRelease };
        var closes = 0;
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if (i.Arguments[2].ToString()!.Contains("closes NPC", StringComparison.Ordinal))
                {
                    var index = Interlocked.Increment(ref closes) - 1;
                    if (index < entered.Length)
                    {
                        entered[index].TrySetResult();
                        Assert.That(releases[index].Wait(TimeSpan.FromSeconds(5)), Is.True);
                    }
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        using var game = CreateServerContext(factory.Object);
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var character = target.SelectedCharacter!;
        character.Name = "PrepLife";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var npc = CreateNpc(NpcWindow.ChaosMachine);
        npc.Definition.Number = 239;
        await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
        var gate = new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 };
        var original = Task.Run(async () => await target.RespawnAtAsync(gate).ConfigureAwait(false));
        var replacement = Task.CompletedTask;
        try
        {
            await entered[0].Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.IsPreparingForcedPlacement, Is.True);
            Assert.That(await target.PrepareForForcedPlacementAsync(() => true).ConfigureAwait(false), Is.False);
            Assert.That(target.IsPreparingForcedPlacement, Is.True, "Competing same-life refusal cannot release the active owner.");
            var oldAttributes = target.Attributes;
            await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
            await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
            await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
            Assert.That(target.Attributes, Is.Not.SameAs(oldAttributes));
            Assert.That(target.IsPreparingForcedPlacement, Is.False, "An old-life owner does not block a selected replacement life.");
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
            replacement = Task.Run(async () => await target.RespawnAtAsync(gate).ConfigureAwait(false));
            await entered[1].Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            oldRelease.Set();
            await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.IsPreparingForcedPlacement, Is.True);
            for (var attempt = 0; attempt < 8; attempt++)
            {
                await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
                await peer.RunPersistenceExclusiveAsync(() => new PartyRequestAction().HandlePartyRequestAsync(peer, target)).ConfigureAwait(false);
                Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, target)).ConfigureAwait(false), Is.False);
            }
            Assert.Multiple(() =>
            {
                Assert.That(target.OpenedNpc, Is.Null);
                Assert.That(target.LastPartyRequester, Is.Null);
                Assert.That(target.TradingPartner, Is.Null);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.IsAlive, Is.True);
            });
            newRelease.Set();
            await replacement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.IsPreparingForcedPlacement, Is.False);
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, npc)).ConfigureAwait(false);
            Assert.That(target.OpenedNpc, Is.SameAs(npc));
        }
        finally
        {
            oldRelease.Set();
            newRelease.Set();
            await Task.WhenAll(original, replacement).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Missing or stale expected identity cannot select and close an unrelated current opening.</summary>
    /// <param name="crafting">Whether the successor owns a crafting snapshot.</param>
    /// <param name="missing">Whether the caller supplied null instead of the original context.</param>
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task StaleExpectedClosePreservesCurrentDialogAsync(bool crafting, bool missing)
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var first = CreateNpc(NpcWindow.Merchant);
        var next = CreateNpc(crafting ? NpcWindow.ChaosMachine : NpcWindow.VaultStorage);
        next.Definition.Number = 239;
        if (crafting)
        {
            next.Definition.ItemCraftings.Add(new MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting());
        }
        else
        {
            var vault = new Mock<ItemStorage>();
            vault.Setup(s => s.Items).Returns(new List<Item>());
            Mock.Get(target.Account!).SetupProperty(a => a.Vault, vault.Object);
        }

        try
        {
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, first)).ConfigureAwait(false);
            var oldDialog = target.OpenedNpcContext;
            await target.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(target)).ConfigureAwait(false);
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, next)).ConfigureAwait(false);
            var current = target.OpenedNpcContext;
            var storage = target.Vault;
            var backup = target.BackupInventory;
            await new CloseNpcDialogAction().CloseNpcDialogAsync(target, missing ? null : oldDialog).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(target.OpenedNpcContext, Is.SameAs(current));
                Assert.That(target.Vault, Is.SameAs(storage));
                Assert.That(target.BackupInventory, Is.SameAs(backup));
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.NpcDialogOpened));
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Close starts before a successor opens; asynchronous completion/errors do not retain state exclusion.</summary>
    /// <param name="failure">Whether the old view fails immediately, after awaiting, or not at all.</param>
    [TestCase("none")]
    [TestCase("sync")]
    [TestCase("async")]
    public async Task CloseResponseIsStartedOnceOutsideCompletionScopeAsync(string failure)
    {
        var logger = new Mock<ILogger>();
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        using var game = CreateServerContext(factory.Object);
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var first = CreateNpc(NpcWindow.ChaosMachine);
        first.Definition.Number = 239;
        first.Definition.ItemCraftings.Add(new MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting());
        var next = CreateNpc(NpcWindow.Merchant);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var events = new List<string>();
        var error = new InvalidOperationException("close view failed");
        Mock.Get(target.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>()!).Setup(v => v.DialogClosedAsync(first.Definition)).Returns(() =>
        {
            events.Add("close-start");
            entered.TrySetResult();
            if (failure == "sync")
            {
                throw error;
            }

            return CompleteResponseAsync();
        });
        await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, first)).ConfigureAwait(false);
        var closing = Task.Run(async () => await new CloseNpcDialogAction().CloseNpcDialogAsync(target).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(target, next)).ConfigureAwait(false);
            events.Add("next-open");
            var current = target.OpenedNpcContext;
            release.TrySetResult();
            await closing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(events, Is.EqualTo(new[] { "close-start", "next-open" }));
                Assert.That(target.OpenedNpcContext, Is.SameAs(current));
                Assert.That(target.BackupInventory, Is.Null);
                Assert.That(logger.Invocations.Any(i => i.Arguments.Any(a => ReferenceEquals(a, error))), Is.EqualTo(failure != "none"));
            });
            Mock.Get(target.ViewPlugIns.GetPlugIn<INpcDialogClosedPlugIn>()!).Verify(v => v.DialogClosedAsync(first.Definition), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await closing.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
        }

        async ValueTask CompleteResponseAsync()
        {
            await release.Task.ConfigureAwait(false);
            if (failure == "async")
            {
                throw error;
            }
        }
    }

    private static GameServerContext CreateServerContext(ILoggerFactory loggerFactory)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        var game = new GameServerContext(
            new BasicModel.GameServerDefinition { GameConfiguration = configuration, ServerConfiguration = new BasicModel.GameServerConfiguration() },
            new Mock<IGuildServer>().Object, new Mock<IEventPublisher>().Object, new Mock<ILoginServer>().Object, new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(), initializer, loggerFactory, manager, NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = manager;
        initializer.PathFinderPool = game.PathFinderPool;
        return game;
    }

    private static NonPlayerCharacter CreateNpc(NpcWindow window)
    {
        var definition = new Mock<MonsterDefinition>();
        definition.SetupAllProperties();
        definition.Setup(d => d.ItemCraftings).Returns(new List<MUnique.OpenMU.DataModel.Configuration.ItemCrafting.ItemCrafting>());
        definition.Object.Number = 255;
        definition.Object.NpcWindow = window;
        return new NonPlayerCharacter(null!, definition.Object, null!);
    }
    private sealed class VaultViewPlayer(IGameServerContext context, IConnection connection) : RemotePlayer(context, connection, new ClientVersion(6, 3, ClientLanguage.English))
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new MockViewPlugInContainer();
    }
}
