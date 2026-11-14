// <copyright file="TradeOpeningFailureTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics.Metrics;
using System.IO;
using System.Threading;
using System.IO.Pipelines;
using System.Text.Json;
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
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.GameServer.RemoteView.Trade;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using ServerPackets = MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>Concrete trade replies must close an already opened peer when the second opener refuses.</summary>
[TestFixture]
public class TradeOpeningFailureTests
{
    /// <summary>Both ordinary opening orders retain items; a success prefix must receive its terminal cancellation.</summary>
    /// <param name="fullOwnerIsRequester">Whether the item owner is opened second.</param>
    /// <param name="freeSlots">Zero/one refuses the return; two permits an ordinary trade and cancel.</param>
    [TestCase(false, 0)]
    [TestCase(true, 0)]
    [TestCase(false, 1)]
    [TestCase(true, 1)]
    [TestCase(false, 2)]
    [TestCase(true, 2)]
    public async Task SecondOpeningRefusalTerminatesOpenedPeerAsync(bool fullOwnerIsRequester, int freeSlots)
    {
        using var game = CreateGameContext();
        var (owner, ownerOutput) = CreatePlayer(game);
        var (peer, peerOutput) = CreatePlayer(game);
        await PlayerTestHelper.EnterWorldAsync(owner, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        await PlayerTestHelper.EnterWorldAsync(peer, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        owner.Attributes![Stats.CurrentHealth] = 100;
        peer.Attributes![Stats.CurrentHealth] = 100;
        owner.SelectedCharacter!.Name = "ItemOwner";
        peer.SelectedCharacter!.Name = "TradePeer";
        var inventory = owner.Inventory!;
        var temporary = owner.TemporaryStorage!;
        var definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true, Durability = 9 };
        var ingredients = new[] { owner.PersistenceContext.CreateNew<Item>(), owner.PersistenceContext.CreateNew<Item>() };
        var fillers = new List<Item>();
        try
        {
            var npcDefinition = new Mock<MonsterDefinition>();
            npcDefinition.SetupAllProperties();
            npcDefinition.Setup(d => d.ItemCraftings).Returns(new List<ItemCrafting>
            {
                new() { SimpleCraftingSettings = owner.PersistenceContext.CreateNew<SimpleCraftingSettings>() },
            });
            npcDefinition.Object.Number = 238;
            npcDefinition.Object.NpcWindow = NpcWindow.ChaosMachine;
            var npc = new NonPlayerCharacter(null!, npcDefinition.Object, null!);
            await owner.RunPersistenceExclusiveAsync(() => new TalkNpcAction().TalkToNpcAsync(owner, npc)).ConfigureAwait(false);
            for (byte index = 0; index < ingredients.Length; index++)
            {
                var item = ingredients[index];
                item.Definition = definition;
                item.Level = 3;
                item.Durability = 7;
                Assert.That(item.GetId(), Is.Not.EqualTo(Guid.Empty));
                Assert.That(await inventory.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
                await owner.RunPersistenceExclusiveAsync(() => new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, index, Storages.ChaosMachine)).ConfigureAwait(false);
            }

            await owner.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(owner)).ConfigureAwait(false);
            Assert.That(owner.BackupInventory, Is.Null);
            for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex - freeSlots; slot++)
            {
                var filler = owner.PersistenceContext.CreateNew<Item>();
                filler.Definition = definition;
                fillers.Add(filler);
                Assert.That(await inventory.AddItemAsync(slot, filler).ConfigureAwait(false), Is.True);
            }

            owner.Money = 1234;
            peer.Money = 4321;
            var ids = inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order().ToArray();
            var requester = fullOwnerIsRequester ? owner : peer;
            var accepter = fullOwnerIsRequester ? peer : owner;
            Assert.That(await Task.Run(async () => await requester.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(requester, accepter)).ConfigureAwait(false)).ConfigureAwait(false), Is.True);
            await Task.Run(async () => await accepter.RunPersistenceExclusiveAsync(
                () => new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true)).ConfigureAwait(false)).ConfigureAwait(false);
            var refused = freeSlots < ingredients.Length;
            if (!refused)
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
            }

            var ownerFrames = ReadTradeFrames(ownerOutput);
            var peerFrames = ReadTradeFrames(peerOutput);
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                fullOwnerIsRequester,
                freeSlots,
                ownerFrames,
                peerFrames,
                ownerWire = Convert.ToHexString(ownerOutput.ToArray()),
                peerWire = Convert.ToHexString(peerOutput.ToArray()),
                ownerState = owner.PlayerState.CurrentState.ToString(),
                peerState = peer.PlayerState.CurrentState.ToString(),
                expectedIds = ids.Length,
                retainedIds = inventory.Items.Concat(temporary.Items).Count(),
                ownerMoney = owner.Money,
                peerMoney = peer.Money,
            }));
            // An ordinary later cancel packet cannot repair a view once server state is already World.
            await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(ReadTradeFrames(ownerOutput), Is.EqualTo(ownerFrames));
                Assert.That(ReadTradeFrames(peerOutput), Is.EqualTo(peerFrames));
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
                Assert.That(inventory.Items.Concat(temporary.Items).Select(i => i.GetId()).Order(), Is.EqualTo(ids));
                Assert.That(ingredients.All(i => i.Level == 3 && i.Durability == 7), Is.True);
                Assert.That(owner.Money, Is.EqualTo(1234));
                Assert.That(peer.Money, Is.EqualTo(4321));
                if (refused)
                {
                    Assert.That(ownerFrames.Count(f => f == "37:Accepted"), Is.Zero);
                    Assert.That(peerFrames.Count(f => f == "37:Accepted"), Is.EqualTo(fullOwnerIsRequester ? 1 : 0));
                    if (fullOwnerIsRequester)
                    {
                        Assert.That(ownerFrames, Is.EqualTo(new[] { "3D:00" }), "Requester must receive terminal cancellation after its opening refuses.");
                        Assert.That(peerFrames, Is.EqualTo(new[] { "37:Accepted", "3D:00" }), "Already-open accepter must receive cancellation after success, in order.");
                    }
                }
                else
                {
                    Assert.That(ownerFrames, Is.EqualTo(new[] { "37:Accepted", "3D:00" }));
                    Assert.That(peerFrames, Is.EqualTo(new[] { "37:Accepted", "3D:00" }));
                }
            });
        }
        finally
        {
            foreach (var filler in fillers)
            {
                await inventory.RemoveItemAsync(filler).ConfigureAwait(false);
            }

            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            ownerOutput.Dispose();
            peerOutput.Dispose();
        }
    }

    /// <summary>A generated reply awaiting real output cannot own a subsequently published pair.</summary>
    /// <param name="phase">Ordinary decline, failed-state reply or normal success/control.</param>
    /// <param name="replaceAccepter">Whether the replacement targets the original accepter.</param>
    [TestCase("decline", true)]
    [TestCase("decline", false)]
    [TestCase("failed-state", true)]
    [TestCase("failed-state", false)]
    [TestCase("normal-decline", true)]
    [TestCase("normal-invalid-state", true)]
    [TestCase("normal-accept", true)]
    public async Task StaleReplyPreservesReplacementPairAsync(string phase, bool replaceAccepter)
    {
        using var game = CreateGameContext();
        game.Configuration.InfoRange = 10;
        var (accepter, accepterOutput) = CreatePlayer(game);
        var (partner, partnerOutput) = CreatePlayer(game);
        var (third, thirdOutput) = CreatePlayer(game);
        foreach (var player in new[] { accepter, partner, third })
        {
            await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
            player.Attributes![Stats.CurrentHealth] = 100;
        }

        var requestHandler = new MUnique.OpenMU.GameServer.MessageHandler.Trade.TradeRequestHandlerPlugIn();
        var replyHandler = new MUnique.OpenMU.GameServer.MessageHandler.Trade.TradeAcceptHandlerPlugIn();
        async ValueTask RequestAsync(Player requester, Player target)
        {
            var bytes = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestRef.Length];
            _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestRef(bytes) { PlayerId = target.Id };
            await requester.RunPersistenceExclusiveAsync(() => requestHandler.HandlePacketAsync(requester, bytes)).ConfigureAwait(false);
            Assert.That(requester.TradingPartner, Is.SameAs(target));
            Assert.That(target.TradingPartner, Is.SameAs(requester));
        }

        var reply = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef(reply) { TradeAccepted = phase is "failed-state" or "normal-invalid-state" or "normal-accept" };
        await Task.Run(async () => await RequestAsync(partner, accepter).ConfigureAwait(false)).ConfigureAwait(false);
        if (phase is "failed-state" or "normal-invalid-state")
        {
            await accepter.RunPersistenceExclusiveAsync(() => replyHandler.HandlePacketAsync(accepter, reply)).ConfigureAwait(false);
            Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
        }

        var queued = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var replyConnection = partner.Connection ?? throw new InvalidOperationException("No connection.");
        var outputLock = replyConnection.OutputLock;
        IDisposable? held = await outputLock.LockAsync().ConfigureAwait(false);
        Mock.Get(replyConnection).SetupGet(c => c.OutputLock).Returns(() =>
        {
            queued.TrySetResult();
            return outputLock;
        });
        var original = Task.Run(async () => await accepter.RunPersistenceExclusiveAsync(() => replyHandler.HandlePacketAsync(accepter, reply)).ConfigureAwait(false));
        try
        {
            await queued.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(original.IsCompleted, Is.False);
            foreach (var player in new[] { accepter, partner })
            {
                await Task.Run(async () =>
                {
                    await using var probe = await player.PlayerState.TryBeginAdvanceToAsync(player.PlayerState.CurrentState).ConfigureAwait(false);
                    probe.Allowed = false;
                }).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            var target = replaceAccepter ? accepter : partner;
            if (!phase.StartsWith("normal-", StringComparison.Ordinal))
            {
                if (replaceAccepter)
                {
                    // Actual generic legacy placement closes the remaining old invitation without needing this blocked packet's persistence funnel.
                    await Task.Run(async () => await accepter.RespawnAtAsync(new ExitGate { Map = accepter.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false)).ConfigureAwait(false);
                    Assert.That(accepter.IsAlive, Is.True);
                    Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                }

                await Task.Run(async () => await RequestAsync(third, target).ConfigureAwait(false)).ConfigureAwait(false);
            }

            held.Dispose();
            held = null;
            await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new { phase, replaceAccepter, accepter = accepter.PlayerState.CurrentState.ToString(), partner = partner.PlayerState.CurrentState.ToString(), third = third.PlayerState.CurrentState.ToString(), targetPartnerIsThird = ReferenceEquals(target.TradingPartner, third), thirdPartnerIsTarget = ReferenceEquals(third.TradingPartner, target), accepterWire = Convert.ToHexString(accepterOutput.ToArray()), partnerWire = Convert.ToHexString(partnerOutput.ToArray()) }));
            if (phase == "normal-accept")
            {
                Assert.That(accepter.TradingPartner, Is.SameAs(partner));
                Assert.That(partner.TradingPartner, Is.SameAs(accepter));
                Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(ReadTradeFrames(accepterOutput).Last(), Is.EqualTo("37:Accepted"));
                Assert.That(ReadTradeFrames(partnerOutput).Last(), Is.EqualTo("37:Accepted"));
                await accepter.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(accepter)).ConfigureAwait(false);
            }
            else if (phase.StartsWith("normal-", StringComparison.Ordinal))
            {
                Assert.That(accepter.TradingPartner, Is.Null);
                Assert.That(partner.TradingPartner, Is.Null);
                Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(partner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(ReadTradeFrames(accepterOutput).Last(), Is.EqualTo("37:Declined").Or.EqualTo("3D:00"));
                Assert.That(ReadTradeFrames(partnerOutput).Last(), Is.EqualTo("37:Declined").Or.EqualTo("3D:00"));
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(target.TradingPartner, Is.SameAs(third));
                    Assert.That(third.TradingPartner, Is.SameAs(target));
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                    Assert.That(third.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                    Assert.That(ReadTradeFrames(accepterOutput), Does.Not.Contain("37:Declined"), "No unowned late false response to the original accepter.");
                    Assert.That(ReadTradeFrames(partnerOutput), Does.Not.Contain("37:Declined"), "No unowned late false response to the original partner.");
                });
            }
        }
        finally
        {
            held?.Dispose();
            await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await accepter.DisposeAsync().ConfigureAwait(false);
            await partner.DisposeAsync().ConfigureAwait(false);
            await third.DisposeAsync().ConfigureAwait(false);
            accepterOutput.Dispose();
            partnerOutput.Dispose();
            thirdOutput.Dispose();
        }
    }

    /// <summary>An unsolicited reply captures no invitation and sends no unowned response.</summary>
    /// <param name="accepted">The generated reply value.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task ReplyWithoutCapturedPartnerIsNoOpAsync(bool accepted)
    {
        using var game = CreateGameContext();
        var (player, output) = CreatePlayer(game);
        await PlayerTestHelper.EnterWorldAsync(player, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        var reply = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef(reply) { TradeAccepted = accepted };
        try
        {
            await player.RunPersistenceExclusiveAsync(() => new MUnique.OpenMU.GameServer.MessageHandler.Trade.TradeAcceptHandlerPlugIn().HandlePacketAsync(player, reply)).ConfigureAwait(false);
            Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(player.TradingPartner, Is.Null);
            Assert.That(output.Length, Is.Zero);
        }
        finally
        {
            await player.DisposeAsync().ConfigureAwait(false);
            output.Dispose();
        }
    }

    /// <summary>Decline uses both owned cancellation scopes even when a result or post-reset metric fails.</summary>
    /// <param name="notificationFailure">Whether failure comes from a view rather than the real cancellation metric.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeclineFinishesBothSidesAfterFailureAsync(bool notificationFailure)
    {
        var accepter = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var partner = await PlayerTestHelper.CreatePlayerAsync(accepter.GameContext).ConfigureAwait(false);
        Assert.That(await partner.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(partner, accepter)).ConfigureAwait(false), Is.True);
        var error = new InvalidOperationException("Decline cancellation failure.");
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
            if (active.Value && !notificationFailure)
            {
                throw error;
            }
        });
        listener.Start();
        if (notificationFailure)
        {
            Mock.Get(partner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(ValueTask.FromException(error));
        }

        var reply = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestResponseRef(reply) { TradeAccepted = false };
        try
        {
            var action = Task.Run(async () =>
            {
                active.Value = true;
                try
                {
                    await accepter.RunPersistenceExclusiveAsync(() => new MUnique.OpenMU.GameServer.MessageHandler.Trade.TradeAcceptHandlerPlugIn().HandlePacketAsync(accepter, reply)).ConfigureAwait(false);
                }
                finally
                {
                    active.Value = false;
                }
            });
            if (notificationFailure)
            {
                await action.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else
            {
                Assert.That(Assert.ThrowsAsync<InvalidOperationException>(async () => await action.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false)), Is.SameAs(error));
            }

            foreach (var player in new[] { accepter, partner })
            {
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(player.TradingPartner, Is.Null);
                Mock.Get(player.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            }
        }
        finally
        {
            await accepter.DisposeAsync().ConfigureAwait(false);
            await partner.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static List<string> ReadTradeFrames(MemoryStream output)
    {
        var bytes = output.ToArray();
        var frames = new List<string>();
        for (var offset = 0; offset < bytes.Length; offset += bytes[offset + 1])
        {
            Assert.That(bytes[offset], Is.EqualTo(0xC1));
            Assert.That(offset + bytes[offset + 1], Is.LessThanOrEqualTo(bytes.Length));
            Assert.That(bytes[offset + 2], Is.EqualTo(ServerPackets.TradeRequestAnswerRef.Code).Or.EqualTo(ServerPackets.TradeFinishedRef.Code));
            if (bytes[offset + 2] == ServerPackets.TradeRequestAnswerRef.Code)
            {
                Assert.That(bytes[offset + 1], Is.EqualTo(ServerPackets.TradeRequestAnswerRef.Length));
                ServerPackets.TradeRequestAnswerRef answer = bytes.AsSpan(offset, bytes[offset + 1]);
                frames.Add(answer.Accepted ? "37:Accepted" : "37:Declined");
            }
            else
            {
                Assert.That(bytes[offset + 1], Is.EqualTo(ServerPackets.TradeFinishedRef.Length));
                frames.Add($"3D:{bytes[offset + 3]:X2}");
            }
        }

        return frames;
    }

    private static (TradeWirePlayer Player, MemoryStream Output) CreatePlayer(IGameServerContext game)
    {
        var output = new MemoryStream();
        var writer = PipeWriter.Create(output, new StreamPipeWriterOptions(leaveOpen: true));
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(writer);
        connection.SetupGet(c => c.OutputLock).Returns(new AsyncLock());
        return (new TradeWirePlayer(game, connection.Object), output);
    }

    private static GameServerContext CreateGameContext()
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        configuration.MaximumInventoryMoney = 1_000_000;
        var manager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        var game = new GameServerContext(
            new BasicModel.GameServerDefinition { GameConfiguration = configuration, ServerConfiguration = new BasicModel.GameServerConfiguration() },
            new Mock<IGuildServer>().Object, new Mock<IEventPublisher>().Object, new Mock<ILoginServer>().Object, new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(), initializer, NullLoggerFactory.Instance, manager, NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = manager;
        initializer.PathFinderPool = game.PathFinderPool;
        return game;
    }

    private sealed class TradeWirePlayer(IGameServerContext context, IConnection connection) : RemotePlayer(context, connection, new ClientVersion(6, 3, ClientLanguage.English))
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new TradeWireViewContainer(this);
    }

    private sealed class TradeWireViewContainer(RemotePlayer player) : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _otherViews = new();
        private readonly ShowTradeRequestAnswerPlugIn _answer = new(player);
        private readonly TradeFinishedPlugIn _finished = new(player);

        public T GetPlugIn<T>()
            where T : class, IViewPlugIn
        {
            if (typeof(T) == typeof(IShowTradeRequestAnswerPlugIn))
            {
                return (T)(IViewPlugIn)this._answer;
            }

            return typeof(T) == typeof(ITradeFinishedPlugIn)
                ? (T)(IViewPlugIn)this._finished
                : this._otherViews.GetPlugIn<T>();
        }
    }
}
