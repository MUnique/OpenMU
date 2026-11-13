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
