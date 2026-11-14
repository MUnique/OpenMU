// <copyright file="PlayerRemovalTradeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics.Metrics;
using System.Threading;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.PlugIns;

/// <summary>World removal excludes new trade publication without changing combat liveness or map-removal hooks.</summary>
[TestFixture]
public class PlayerRemovalTradeTests
{
    /// <summary>A held logout cannot accept a new request; its old peer can start a different trade safely.</summary>
    [Test]
    public async Task PendingLogoutRejectsInvitationAndPreservesPeersNewTradeAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var requester = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        peer.Money = 200;
        await OpenTradeAsync(owner, peer).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(owner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var logout = Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
            () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(owner.IsAlive, Is.True, "Departure must not masquerade as death or duel respawn.");
            Assert.That(owner.CurrentMap, Is.Not.Null);
            Assert.That(await Task.Run(async () => await requester.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(requester, owner)).ConfigureAwait(false)).ConfigureAwait(false), Is.False);
            await OpenTradeAsync(requester, peer).ConfigureAwait(false);
            var backup = peer.BackupInventory;
            await peer.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(peer, 60)).ConfigureAwait(false);
            release.TrySetResult();
            await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(owner.SelectedCharacter, Is.Null);
                Assert.That(peer.TradingPartner, Is.SameAs(requester));
                Assert.That(requester.TradingPartner, Is.SameAs(peer));
                Assert.That(peer.BackupInventory, Is.SameAs(backup));
                Assert.That(peer.Money, Is.EqualTo(140));
                Assert.That(peer.TradingMoney, Is.EqualTo(60));
            });
            await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
            Assert.That(peer.Money, Is.EqualTo(200));
        }
        finally
        {
            release.TrySetResult();
            await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await requester.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An early notification failure releases departure exclusion while preserving cleaned transaction data.</summary>
    [Test]
    public async Task FailedLogoutDoesNotPermanentlyBlockActiveCharacterAsync()
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var owner = new FailingNotificationPlayer(seed.GameContext);
        await PlayerTestHelper.EnterWorldAsync(owner, PlayerTestHelper.CreateCharacter(seed.GameContext)).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(seed.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        owner.Money = 200;
        peer.Money = 300;
        await OpenTradeAsync(owner, peer).ConfigureAwait(false);
        await owner.RunPersistenceExclusiveAsync(() => new TradeMoneyAction().TradeMoneyAsync(owner, 60)).ConfigureAwait(false);
        owner.Views.FailNextTradeResult = true;
        try
        {
            var failure = Assert.ThrowsAsync<InvalidOperationException>(async () => await owner.RunPersistenceExclusiveAsync(
                () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false));
            Assert.That(failure!.Message, Is.EqualTo("Expected departure notification failure."));
            Assert.Multiple(() =>
            {
                Assert.That(owner.IsAlive, Is.True);
                Assert.That(owner.SelectedCharacter, Is.Not.Null);
                Assert.That(owner.CurrentMap!.GetPlayers(), Does.Contain(owner));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(owner.Money, Is.EqualTo(200));
                Assert.That(owner.TradingMoney, Is.Zero);
                Assert.That(peer.Money, Is.EqualTo(300));
                Assert.That(peer.TradingPartner, Is.Null);
            });
            await OpenTradeAsync(owner, peer).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The actual duel map-removal plugin cancels a living duelist before character teardown.</summary>
    [Test]
    public async Task LivingLogoutStillEndsDuelThroughMapRemovalAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var map = owner.CurrentMap!;
        var safe = new Mock<GameMapDefinition>();
        safe.SetupAllProperties();
        safe.Setup(m => m.DropItemGroups).Returns(map.Definition.DropItemGroups);
        safe.Setup(m => m.MonsterSpawns).Returns(map.Definition.MonsterSpawns);
        var gates = new List<ExitGate>();
        safe.Setup(m => m.ExitGates).Returns(gates);
        safe.Object.Number = 1;
        safe.Object.TerrainData = map.Definition.TerrainData;
        gates.Add(new ExitGate { Map = safe.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        owner.GameContext.Configuration.Maps.Add(safe.Object);
        map.Definition.SafezoneMap = safe.Object;
        var gate = new ExitGate { Map = map.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 };
        var duelConfiguration = new Mock<DuelConfiguration>();
        duelConfiguration.Setup(c => c.DuelAreas).Returns(new List<DuelArea> { new() { Index = 0, FirstPlayerGate = gate, SecondPlayerGate = gate } });
        owner.GameContext.Configuration.DuelConfiguration = duelConfiguration.Object;
        ((GameContext)owner.GameContext).DuelRoomManager = new DuelRoomManager(duelConfiguration.Object);
        var room = (await owner.GameContext.DuelRoomManager.GetFreeDuelRoomAsync(owner, peer).ConfigureAwait(false))!;
        owner.DuelRoom = room;
        peer.DuelRoom = room;
        owner.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IObjectRemovedFromMapPlugIn>(new EndDuelWhenLeavingDuelMapPlugIn());
        var duel = room.RunDuelAsync();
        var observed = false;
        map.ObjectRemoved += async args =>
        {
            if (ReferenceEquals(args.Object, owner) && !observed)
            {
                observed = true;
                // This observer runs after the real plugin but before logout clears the selected character.
                await duel.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
        };
        try
        {
            Assert.That(room.State, Is.EqualTo(DuelState.DuelAccepted));
            await owner.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).AsTask().WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(observed, Is.True);
                Assert.That(duel.IsCompleted, Is.True);
                Assert.That(room.State, Is.EqualTo(DuelState.DuelCancelled));
                Assert.That(owner.DuelRoom, Is.Null);
                Assert.That(peer.DuelRoom, Is.Null);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
            });
        }
        finally
        {
            await room.CancelDuelAsync().ConfigureAwait(false);
            await duel.WaitAsync(TimeSpan.FromSeconds(8)).ConfigureAwait(false);
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Disconnect must exclude invitations while its previous cancellation result is pending.</summary>
    [Test]
    public async Task PendingDisconnectCannotStrandLateInvitationAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        var oldPeer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var requester = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var partyRecipient = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        await owner.RunPersistenceExclusiveAsync(() => new PartyRequestAction().HandlePartyRequestAsync(owner, partyRecipient)).ConfigureAwait(false);
        Assert.That(partyRecipient.LastPartyRequester, Is.SameAs(owner));
        await OpenTradeAsync(owner, oldPeer).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(owner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var disconnect = Task.Run(async () => await owner.DisconnectAsync().ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.IsDisposing, Is.False);
                Assert.That(owner.IsAlive, Is.True);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.TradingPartner, Is.Null);
            });
            await Task.Run(async () => await partyRecipient.RunPersistenceExclusiveAsync(
                () => new PartyResponseAction().HandleResponseAsync(partyRecipient, true)).ConfigureAwait(false)).ConfigureAwait(false);
            var admitted = await Task.Run(async () => await requester.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(requester, owner)).ConfigureAwait(false)).ConfigureAwait(false);
            release.TrySetResult();
            await disconnect.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(partyRecipient.Party, Is.Null, "A live-looking requester is departing before Disconnected.");
                Assert.That(owner.Party, Is.Null);
                Assert.That(admitted, Is.False);
                Assert.That(requester.TradingPartner, Is.Null, "No manual cancel packet should be required after peer disconnects.");
                Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(owner.SelectedCharacter, Is.Null);
            });
            await OpenTradeAsync(requester, oldPeer).ConfigureAwait(false);
            await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
            await disconnect.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await owner.DisposeAsync().ConfigureAwait(false);
            await requester.DisposeAsync().ConfigureAwait(false);
            await oldPeer.DisposeAsync().ConfigureAwait(false);
            await partyRecipient.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An independently completed logout cannot release the still-pending disconnect's admission count.</summary>
    [Test]
    public async Task NestedRemovalAndReselectionKeepDisconnectAdmissionAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var requester = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var character = owner.SelectedCharacter!;
        character.Name = "Reselect";
        Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        owner.Attributes![Stats.CurrentHealth] = 100;
        var life = owner.Attributes;
        await OpenTradeAsync(owner, peer).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(owner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var disconnect = Task.Run(async () => await owner.DisconnectAsync().ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
                () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.IsAlive, Is.True);
                Assert.That(owner.Attributes, Is.Not.SameAs(life));
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.SelectedCharacter, Is.SameAs(character));
            });
            Assert.That(await Task.Run(async () => await requester.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(requester, owner)).ConfigureAwait(false)).ConfigureAwait(false), Is.False);
            release.TrySetResult();
            await disconnect.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(owner.SelectedCharacter, Is.Null, "Disconnect still closes the session, including a concurrent reselection.");
            Assert.That(requester.TradingPartner, Is.Null);
            await OpenTradeAsync(requester, peer).ConfigureAwait(false);
            await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
            await disconnect.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await owner.DisposeAsync().ConfigureAwait(false);
            await requester.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A real post-reset cancellation failure releases only this disconnect's departure scope.</summary>
    [Test]
    public async Task FailedDisconnectReleasesAdmissionAfterCleanupAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var requester = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        await OpenTradeAsync(owner, peer).ConfigureAwait(false);
        var error = new InvalidOperationException("Cancellation metric failed.");
        var active = new AsyncLocal<bool>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, subscriber) =>
        {
            if (instrument.Meter.Name == BaseTradeAction.MeterName && instrument.Name == "CancelledTradesCount")
            {
                subscriber.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<int>((_, _, _, _) =>
        {
            if (active.Value)
            {
                throw error;
            }
        });
        listener.Start();
        try
        {
            var disconnect = Task.Run(async () =>
            {
                active.Value = true;
                try
                {
                    await owner.DisconnectAsync().ConfigureAwait(false);
                }
                finally
                {
                    active.Value = false;
                }
            });
            Assert.That(Assert.ThrowsAsync<InvalidOperationException>(async () => await disconnect.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false)), Is.SameAs(error));
            Assert.Multiple(() =>
            {
                Assert.That(owner.IsAlive, Is.True);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.SelectedCharacter, Is.Not.Null);
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
            });
            await OpenTradeAsync(requester, owner).ConfigureAwait(false);
            await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
            await owner.DisconnectAsync().ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await requester.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async ValueTask OpenTradeAsync(Player requester, Player accepter)
    {
        Assert.That(await Task.Run(async () => await requester.RunPersistenceExclusiveAsync(
            () => new TradeRequestAction().RequestTradeAsync(requester, accepter)).ConfigureAwait(false)).ConfigureAwait(false), Is.True);
        await Task.Run(async () => await accepter.RunPersistenceExclusiveAsync(
            () => new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true)).ConfigureAwait(false)).ConfigureAwait(false);
    }

    private sealed class FailingNotificationPlayer(IGameContext context) : Player(context)
    {
        public DepartureViews Views { get; } = new();

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => this.Views;
    }

    private sealed class DepartureViews : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _mocks = new();

        public bool FailNextTradeResult { get; set; }

        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn
        {
            if (typeof(T) == typeof(ITradeFinishedPlugIn) && this.FailNextTradeResult)
            {
                this.FailNextTradeResult = false;
                throw new InvalidOperationException("Expected departure notification failure.");
            }

            return this._mocks.GetPlugIn<T>();
        }
    }
}
