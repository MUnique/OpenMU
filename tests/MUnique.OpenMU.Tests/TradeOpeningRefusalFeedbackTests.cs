// <copyright file="TradeOpeningRefusalFeedbackTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.IO;
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
public class TradeOpeningRefusalFeedbackTests
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
    public async Task OpeningRefusalNotifiesBothPeersAsync(bool fullOwnerIsRequester, int freeSlots)
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

            // Closing returns the tray to the inventory or the vault; only when both are full do the items stay.
            for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
            {
                var filler = owner.PersistenceContext.CreateNew<Item>();
                filler.Definition = definition;
                fillers.Add(filler);
                Assert.That(await inventory.AddItemAsync(slot, filler).ConfigureAwait(false), Is.True);
            }

            var vault = new Mock<ItemStorage>();
            vault.SetupAllProperties();
            vault.Setup(v => v.Items).Returns(new List<Item>());
            Mock.Get(owner.Account!).SetupProperty(account => account.Vault, vault.Object);
            var vaultStorage = new Storage(InventoryConstants.WarehouseSize, vault.Object);
            while (await vaultStorage.AddItemAsync(CreateVaultFiller(owner, definition)).ConfigureAwait(false))
            {
                // Fills the vault.
            }

            await owner.RunPersistenceExclusiveAsync(() => new CloseNpcDialogAction().CloseNpcDialogAsync(owner)).ConfigureAwait(false);
            Assert.That(owner.BackupInventory, Is.Null);
            Assert.That(temporary.Items, Is.EquivalentTo(ingredients));
            foreach (var filler in fillers.TakeLast(freeSlots).ToList())
            {
                await inventory.RemoveItemAsync(filler).ConfigureAwait(false);
                fillers.Remove(filler);
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
            Assert.Multiple(() =>
            {
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
                    Assert.That(ownerFrames, Is.EqualTo(new[] { "3D:00" }), "Refusing owner must receive terminal cancellation.");
                    Assert.That(peerFrames, Is.EqualTo(fullOwnerIsRequester ? new[] { "37:Accepted", "3D:00" } : new[] { "3D:00" }), "Peer must receive terminal cancellation after any success prefix.");
                }
                else
                {
                    Assert.That(ownerFrames, Is.EqualTo(new[] { "37:Accepted", "3D:00" }));
                    Assert.That(peerFrames, Is.EqualTo(new[] { "37:Accepted", "3D:00" }));
                }
            });
            if (refused)
            {
                foreach (var filler in fillers.TakeLast(2).ToList())
                {
                    await inventory.RemoveItemAsync(filler).ConfigureAwait(false);
                    fillers.Remove(filler);
                }

                Assert.That(await requester.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(requester, accepter)).ConfigureAwait(false), Is.True);
                await accepter.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true)).ConfigureAwait(false);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                await requester.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(requester)).ConfigureAwait(false);
                Assert.That(ingredients.All(item => inventory.Items.Contains(item)), Is.True);
            }
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

    private static Item CreateVaultFiller(Player owner, ItemDefinition definition)
    {
        var item = owner.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        return item;
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
