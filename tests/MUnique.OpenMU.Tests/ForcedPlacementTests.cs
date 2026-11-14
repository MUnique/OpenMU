// <copyright file="ForcedPlacementTests.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer.MessageHandler.Character;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>Forced placement claims map change before detachment and preserves the current character/death across awaits.</summary>
[TestFixture]
public class ForcedPlacementTests
{
    /// <summary>Both protocol branches preserve rollback, new lethal ownership and teardown while placement is pending.</summary>
    /// <param name="legacy">Whether the client completes directly with its respawn packet.</param>
    /// <param name="stage">The production asynchronous boundary being held.</param>
    [TestCase(false, "ordinary")]
    [TestCase(true, "ordinary")]
    [TestCase(false, "cleanup")]
    [TestCase(true, "cleanup")]
    [TestCase(false, "remove")]
    [TestCase(true, "remove")]
    [TestCase(false, "packet")]
    [TestCase(true, "packet")]
    [TestCase(false, "remove-teardown")]
    [TestCase(true, "remove-teardown")]
    [TestCase(false, "remove-reselect")]
    [TestCase(true, "remove-reselect")]
    [TestCase(false, "teardown")]
    [TestCase(true, "teardown")]
    [TestCase(false, "reselect")]
    [TestCase(true, "reselect")]
    [TestCase(false, "compete")]
    [TestCase(true, "compete")]
    public async Task PlacementPreservesLifeAndInteractionAsync(bool legacy, string stage)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, legacy);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        peer.Attributes![Stats.CurrentHealth] = 100;
        target.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        target.Money = 200;
        var character = target.SelectedCharacter!;
        character.Name = "Placement";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var source = target.CurrentMap!;
        var definition = new Mock<GameMapDefinition>();
        definition.SetupAllProperties();
        definition.Setup(m => m.DropItemGroups).Returns(source.Definition.DropItemGroups);
        definition.Setup(m => m.MonsterSpawns).Returns(source.Definition.MonsterSpawns);
        definition.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        definition.Object.Number = 1;
        definition.Object.TerrainData = source.Definition.TerrainData;
        target.GameContext.Configuration.Maps.Add(definition.Object);
        var destination = (await target.GameContext.GetMapAsync(1).ConfigureAwait(false))!;
        var gate = new ExitGate { Map = definition.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true };
        definition.Object.ExitGates.Add(gate);
        source.Definition.SafezoneMap = definition.Object;
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Object.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await target.Inventory!.AddItemAsync(20, item.Object).ConfigureAwait(false), Is.True);
        Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, target)).ConfigureAwait(false), Is.True);
        await target.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(target, true)).ConfigureAwait(false);
        await target.RunPersistenceExclusiveAsync(async () =>
        {
            await new MoveItemAction().MoveItemAsync(target, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
            await new TradeMoneyAction().TradeMoneyAsync(target, 60).ConfigureAwait(false);
        }).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var packets = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        async ValueTask PauseAsync(string boundary)
        {
            if (stage == boundary || (boundary == "packet" && stage is "teardown" or "reselect" or "compete")
                || (boundary == "remove" && stage is "remove-teardown" or "remove-reselect"))
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        }

        Mock.Get(target.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(() => PauseAsync("cleanup"));
        source.ObjectRemoved += async args =>
        {
            if (ReferenceEquals(args.Object, target))
            {
                await PauseAsync("remove").ConfigureAwait(false);
            }
        };
        async ValueTask PacketAsync()
        {
            Interlocked.Increment(ref packets);
            await PauseAsync("packet").ConfigureAwait(false);
        }

        if (legacy)
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(PacketAsync);
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }
        else
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }

        var placement = Task.Run(async () => await peer.RunPersistenceExclusiveAsync(() => target.RespawnAtAsync(gate)).ConfigureAwait(false));
        DeathInformation? owned = null;
        try
        {
            if (stage != "ordinary")
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                if (stage is "cleanup" or "remove")
                {
                    await Task.Run(async () => await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    owned = target.LastDeath;
                }
                else if (stage == "packet")
                {
                    // Ordinary hits reject an off-map player; a real instant hazard may still reserve this life.
                    await Task.Run(async () => await target.KillInstantlyAsync().ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    owned = target.LastDeath;
                    Assert.That(deaths, Is.Zero, "Death is deferred without making its caller wait for future Ready.");
                }
                else if (stage is "teardown" or "remove-teardown")
                {
                    await target.DisconnectAsync().ConfigureAwait(false);
                }
                else if (stage is "reselect" or "remove-reselect")
                {
                    var oldAttributes = target.Attributes;
                    await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                    await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                    await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                    Assert.That(target.Attributes, Is.Not.SameAs(oldAttributes));
                }
                else
                {
                    await target.RespawnAtAsync(new ExitGate { Map = source.Definition, X1 = 2, X2 = 2, Y1 = 2, Y2 = 2 }).ConfigureAwait(false);
                    Assert.That(character.CurrentMap, Is.SameAs(definition.Object));
                    Assert.That(packets, Is.EqualTo(1), "A competing generic placement must refuse without disturbing the original stream.");
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.ChangingMap));
                }
            }

            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!legacy && stage is not ("cleanup" or "teardown" or "remove-teardown"))
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            Mock.Get(peer.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            Assert.Multiple(() =>
            {
                if (stage is "cleanup" or "remove-reselect")
                {
                    Assert.That(source.GetPlayers(), Does.Contain(target), "An abandoned placement must leave the current life on its map.");
                }
                else
                {
                    Assert.That(source.GetPlayers(), Does.Not.Contain(target));
                }
                Assert.That(peer.TradingPartner, Is.Null);
                if (stage is not ("teardown" or "remove-teardown"))
                {
                    Assert.That(target.TradingPartner, Is.Null);
                    Assert.That(target.BackupInventory, Is.Null);
                    Assert.That(target.TradingMoney, Is.Zero);
                    Assert.That(target.Money, Is.EqualTo(200));
                    Assert.That(target.Inventory!.Items.Count(i => ReferenceEquals(i, item.Object)), Is.EqualTo(1));
                }

                if (stage is "cleanup" or "remove" or "packet")
                {
                    Assert.That(target.LastDeath, Is.SameAs(owned));
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                    Assert.That(target.IsAlive, Is.False);
                    Assert.That(deaths, Is.EqualTo(1));
                }
                else if (stage is "teardown" or "remove-teardown")
                {
                    Assert.That(destination.GetPlayers(), Does.Not.Contain(target));
                    Assert.That(target.SelectedCharacter, Is.Null);
                }
                else
                {
                    var expectedMap = stage == "remove-reselect" ? source : destination;
                    Assert.That(target.CurrentMap, Is.SameAs(expectedMap));
                    Assert.That(expectedMap.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1));
                    Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(target.IsAlive, Is.True);
                }
            });
        }
        finally
        {
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Refusing a competing placement cannot drop an already elapsed original death timer.</summary>
    /// <param name="legacy">Whether the original placement completes server-side or through Ready.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task CompetingPlacementRetainsDueDeathAsync(bool legacy)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, legacy);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var source = target.CurrentMap!;
        var definition = new Mock<GameMapDefinition>();
        definition.SetupAllProperties();
        definition.Setup(m => m.DropItemGroups).Returns(source.Definition.DropItemGroups);
        definition.Setup(m => m.MonsterSpawns).Returns(source.Definition.MonsterSpawns);
        definition.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        definition.Object.Number = 1;
        definition.Object.TerrainData = source.Definition.TerrainData;
        peer.GameContext.Configuration.Maps.Add(definition.Object);
        var gate = new ExitGate { Map = definition.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true };
        definition.Object.ExitGates.Add(gate);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var restored = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var packets = 0;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        async ValueTask PacketAsync()
        {
            if (Interlocked.Increment(ref packets) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
            else
            {
                restored.TrySetResult();
            }
        }

        if (legacy)
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(PacketAsync);
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }
        else
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IMapChangePlugIn>()!).Setup(v => v.MapChangeAsync()).Returns(PacketAsync);
        }

        await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
        var owned = target.LastDeath;
        var placement = Task.Run(async () => await target.RespawnAtAsync(gate).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await Task.Delay(3500).ConfigureAwait(false);
            await target.RespawnAtAsync(new ExitGate { Map = source.Definition, X1 = 2, X2 = 2, Y1 = 2, Y2 = 2 }).ConfigureAwait(false);
            Assert.That(packets, Is.EqualTo(1));
            Assert.That(target.GetPlacementDeathSnapshot(), Is.EqualTo((owned, true)));
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.Zero);
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!legacy)
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            await restored.Task.WaitAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            if (!legacy)
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.LastDeath, Is.SameAs(owned));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(target.Attributes[Stats.MaximumHealth]));
            });
        }
        finally
        {
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A failed real timer gate selection may retry at an existing subsequent map arrival.</summary>
    /// <param name="legacy">Whether the arrival completes server-side or through Ready.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task FailedDeathRespawnRetriesOnMapArrivalAsync(bool legacy)
    {
        var configuration = PlayerTestHelper.CreateGameConfiguration();
        var failed = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(i =>
            {
                if ((LogLevel)i.Arguments[0] == LogLevel.Error && i.Arguments[2].ToString()!.Contains("Unexpected error during respawning", StringComparison.Ordinal))
                {
                    failed.TrySetResult((Exception)i.Arguments[3]);
                }
            }));
        var factory = new Mock<ILoggerFactory>();
        factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger.Object);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, new InMemoryPersistenceContextProvider(), initializer, factory.Object, new PlugInManager(null, factory.Object, null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var target = new PlacementPlayer(game, legacy);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var deaths = 0;
        target.Died += (_, _) => Interlocked.Increment(ref deaths);
        try
        {
            // Map0 has no spawn gate: the real timer's gate selection throws, without a fake continuation.
            await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
            var death = target.LastDeath;
            var error = await failed.Task.WaitAsync(TimeSpan.FromSeconds(6)).ConfigureAwait(false);
            Assert.That(error, Is.TypeOf<InvalidOperationException>());
            Assert.That(error.Message, Does.Contain("has no spawn gate"));
            var destination = await CreatePlacementMapAsync(game, 1).ConfigureAwait(false);
            Assert.That(target.GetPlacementDeathSnapshot(), Is.EqualTo((death, true)));
            Assert.That(target.Attributes[Stats.CurrentHealth], Is.Zero);
            await target.ApplyPoisonDamageAsync(peer, 1).ConfigureAwait(false);
            Assert.That(deaths, Is.EqualTo(1));

            // An ordinary forced placement supplies a valid gate; its existing arrival path owns retry.
            await peer.RunPersistenceExclusiveAsync(() => target.RespawnAtAsync(destination.SafeZoneSpawnGate!)).ConfigureAwait(false);
            if (!legacy)
            {
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
                if (target.PlayerState.CurrentState == PlayerState.ChangingMap)
                {
                    await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
                }
            }

            Assert.Multiple(() =>
            {
                Assert.That(target.IsAlive, Is.True);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(target.Attributes[Stats.CurrentHealth], Is.EqualTo(target.Attributes[Stats.MaximumHealth]));
                Assert.That(target.GetPlacementDeathSnapshot(), Is.EqualTo((death, false)));
                Assert.That(deaths, Is.EqualTo(1));
                Assert.That(destination.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1));
            });
        }
        finally
        {
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Real map-add registration precedes its awaited callbacks; late placement must preserve the current life.</summary>
    /// <param name="transition">The ordinary transition while the real add callback is held.</param>
    [TestCase("logout")]
    [TestCase("reselect")]
    [TestCase("warp")]
    public async Task RegisteredPlacementDoesNotLeaveOldMapEntryAsync(string transition)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, true);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        var character = target.SelectedCharacter!;
        character.Name = "AddLife";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var destination = await CreatePlacementMapAsync(peer.GameContext, 1).ConfigureAwait(false);
        var next = await CreatePlacementMapAsync(peer.GameContext, 2).ConfigureAwait(false);
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var additions = 0;
        destination.ObjectAdded += async args =>
        {
            if (ReferenceEquals(args.Object, target) && Interlocked.Increment(ref additions) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        };
        var placement = Task.Run(async () => await peer.RunPersistenceExclusiveAsync(() => target.RespawnAtAsync(destination.SafeZoneSpawnGate!)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(destination.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1));
            if (transition == "warp")
            {
                await target.RunPersistenceExclusiveAsync(() => target.WarpToAsync(next.SafeZoneSpawnGate!)).ConfigureAwait(false);
                await target.RunPersistenceExclusiveAsync(() => new CharacterClientReadyPacketHandlerPlugIn().HandlePacketAsync(target, CreateReadyPacket())).ConfigureAwait(false);
            }
            else
            {
                await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                Assert.That(destination.GetPlayers(), Does.Not.Contain(target));
                if (transition == "reselect")
                {
                    await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                    await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                }
            }

            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                if (transition == "reselect")
                {
                    Assert.That(target.CurrentMap, Is.SameAs(destination));
                    Assert.That(destination.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1), "Old placement must not remove replacement life from same map.");
                }
                else
                {
                    Assert.That(destination.GetPlayers(), Does.Not.Contain(target), "Real removal already removed the early registration; old add does not register it again.");
                }

                if (transition == "warp")
                {
                    Assert.That(target.CurrentMap, Is.SameAs(next));
                    Assert.That(next.GetPlayers().Count(p => ReferenceEquals(p, target)), Is.EqualTo(1));
                }
            });
        }
        finally
        {
            release.TrySetResult();
            await placement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>An exceptional attempt releases only its own still-current claim, never a concurrent replacement's.</summary>
    /// <param name="operation">The lifecycle operation while gate selection is awaiting.</param>
    [TestCase("concurrent")]
    [TestCase("teardown")]
    [TestCase("replacement")]
    public async Task ExceptionalRespawnPreservesClaimOwnershipAsync(string operation)
    {
        var peer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var target = new PlacementPlayer(peer.GameContext, true);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(peer.GameContext)).ConfigureAwait(false);
        var character = target.SelectedCharacter!;
        character.Name = "ClaimLife";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        var destination = await CreatePlacementMapAsync(peer.GameContext, 1).ConfigureAwait(false);
        var gate = destination.SafeZoneSpawnGate!;
        target.Attributes![Stats.CurrentHealth] = 10;
        await target.ApplyPoisonDamageAsync(peer, 10).ConfigureAwait(false);
        var oldDeath = target.LastDeath;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var replacementEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var replacementRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var failure = new InvalidOperationException("gate selection failed");
        var firstOld = 0;
        var firstNew = 0;
        var packets = 0;
        Mock.Get(target.ViewPlugIns.GetPlugIn<IRespawnAfterDeathPlugIn>()!).Setup(v => v.RespawnAsync()).Returns(() =>
        {
            Interlocked.Increment(ref packets);
            return ValueTask.CompletedTask;
        });
        var selection = new Mock<IPlayerSpawnGateSelectionPlugIn>();
        selection.Setup(p => p.SelectSpawnGateAsync(target, It.IsAny<SpawnGateSelectionArgs>())).Returns(async (Player _, SpawnGateSelectionArgs args) =>
        {
            var isOriginalDeath = ReferenceEquals(target.LastDeath, oldDeath);
            if (isOriginalDeath && Interlocked.Exchange(ref firstOld, 1) == 0)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
                throw failure;
            }

            if (!isOriginalDeath && operation == "replacement" && Interlocked.Exchange(ref firstNew, 1) == 0)
            {
                replacementEntered.TrySetResult();
                await replacementRelease.Task.ConfigureAwait(false);
            }

            args.Gate = gate;
        });
        target.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint<IPlayerSpawnGateSelectionPlugIn>(selection.Object);
        var original = Task.Run(async () => await target.RespawnAfterDeathAsync().ConfigureAwait(false));
        var replacement = Task.CompletedTask;
        DeathInformation? newDeath = null;
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (operation == "teardown")
            {
                await target.DisconnectAsync().ConfigureAwait(false);
            }
            else if (operation == "replacement")
            {
                await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
                await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                await target.KillInstantlyAsync().ConfigureAwait(false);
                newDeath = target.LastDeath;
                Assert.That(newDeath, Is.Not.SameAs(oldDeath));
                replacement = Task.Run(async () => await target.RespawnAfterDeathAsync().ConfigureAwait(false));
                await replacementEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else
            {
                await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
                Assert.That(packets, Is.Zero, "An active attempt retains its claim until it fails.");
            }

            release.TrySetResult();
            var observed = Assert.ThrowsAsync<InvalidOperationException>(async () => await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false));
            Assert.That(observed, Is.SameAs(failure), "Claim cleanup must preserve the original exception.");
            if (operation == "replacement")
            {
                await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
                Assert.That(packets, Is.Zero, "Old failure must not release the new death's in-flight claim.");
                Assert.That(target.GetPlacementDeathSnapshot(), Is.EqualTo((newDeath, true)));
                replacementRelease.TrySetResult();
                await replacement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(target.LastDeath, Is.SameAs(newDeath));
                Assert.That(target.IsAlive, Is.True);
            }
            else if (operation == "teardown")
            {
                await target.CompleteMapChangeDeathAsync().ConfigureAwait(false);
                Assert.That(target.SelectedCharacter, Is.Null);
                Assert.That(destination.GetPlayers(), Does.Not.Contain(target));
                Assert.That(packets, Is.Zero);
            }
            else
            {
                await target.RespawnAfterDeathAsync(gate).ConfigureAwait(false);
                Assert.That(target.LastDeath, Is.SameAs(oldDeath));
                Assert.That(target.IsAlive, Is.True);
                Assert.That(packets, Is.EqualTo(1));
            }
        }
        finally
        {
            release.TrySetResult();
            replacementRelease.TrySetResult();
            try
            {
                await original.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex) when (ReferenceEquals(ex, failure))
            {
                // Expected failing gate selection, also when an earlier assertion fails.
            }

            await replacement.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async Task<GameMap> CreatePlacementMapAsync(IGameContext game, short number)
    {
        var definition = new Mock<GameMapDefinition>();
        definition.SetupAllProperties();
        var source = game.Configuration.Maps.First();
        definition.Setup(m => m.DropItemGroups).Returns(source.DropItemGroups);
        definition.Setup(m => m.MonsterSpawns).Returns(source.MonsterSpawns);
        definition.Setup(m => m.ExitGates).Returns(new List<ExitGate>());
        definition.Object.Number = number;
        definition.Object.TerrainData = source.TerrainData;
        definition.Object.ExitGates.Add(new ExitGate { Map = definition.Object, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1, IsSpawnGate = true });
        game.Configuration.Maps.Add(definition.Object);
        return (await game.GetMapAsync((ushort)number).ConfigureAwait(false))!;
    }

    private static byte[] CreateReadyPacket()
    {
        var packet = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef.Length];
        _ = new MUnique.OpenMU.Network.Packets.ClientToServer.ClientReadyAfterMapChangeRef(packet);
        return packet;
    }

    private sealed class PlacementPlayer(IGameContext context, bool legacy) : Player(context)
    {
        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new PlacementViews(legacy);
    }

    private sealed class PlacementViews(bool legacy) : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly MockViewPlugInContainer _mocks = new();

        public T? GetPlugIn<T>()
            where T : class, IViewPlugIn
            => !legacy && typeof(T) == typeof(IRespawnAfterDeathPlugIn) ? null : this._mocks.GetPlugIn<T>();
    }
}
