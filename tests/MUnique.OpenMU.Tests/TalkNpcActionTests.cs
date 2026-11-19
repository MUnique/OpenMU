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
