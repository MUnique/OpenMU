// <copyright file="CastleSiegeNpcTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using RuntimeGuild = MUnique.OpenMU.Interfaces.Guild;

/// <summary>
/// Tests Castle Siege NPC spawning, lifecycle, actions, terrain, and persistence.
/// </summary>
[TestFixture]
public class CastleSiegeNpcTests
{
    private const uint OwnerRuntimeGuildId = 10;
    private const byte GateInstanceId = 1;
    private const byte StatueInstanceId = 1;
    private const byte GateX = 50;
    private const byte GateY = 50;

    /// <summary>
    /// Verifies that Castle Senior receives stable management indexes even when structures are not spawned.
    /// </summary>
    [Test]
    public async ValueTask SeniorListsUseConfiguredIndexesBeforeAndAfterSpawnAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
            var runtimeCount = fixture.Context.ActiveNpcs.Count;
            var stateCount = fixture.Context.SiegeData.NpcStates.Count;
            var unspawnedInfo = (await fixture.Context.NpcController
                    .GetDefenseStructureSnapshotAsync(CastleSiegeGate.MonsterNumber)
                    .ConfigureAwait(false))
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(unspawnedInfo.NpcIndex, Is.EqualTo(GateInstanceId));
                Assert.That(unspawnedInfo.CurrentHealth, Is.EqualTo(1_000));
                Assert.That(fixture.Context.ActiveNpcs, Has.Count.EqualTo(runtimeCount));
                Assert.That(fixture.Context.SiegeData.NpcStates, Has.Count.EqualTo(stateCount));
            });

            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var spawnedGate = fixture.Context.NpcController.FindDefenseStructure(
                (uint)CastleSiegeGate.MonsterNumber,
                GateInstanceId)!;
            var spawnedInfo = (await fixture.Context.NpcController
                    .GetDefenseStructureSnapshotAsync(CastleSiegeGate.MonsterNumber)
                    .ConfigureAwait(false))
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(spawnedInfo.NpcIndex, Is.EqualTo(GateInstanceId));
                Assert.That(spawnedGate.SpawnedInstance, Is.Not.Null);
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that a castle owner can repair a persistent structure during truce without spawning it.
    /// </summary>
    [Test]
    public async ValueTask SeniorRepairsUnspawnedStructureDuringTruceAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
            fixture.Context.CurrentState = CastleSiegeState.RegisterGuild;
            var gate = fixture.Context.NpcController.FindDefenseStructure(
                (uint)CastleSiegeGate.MonsterNumber,
                GateInstanceId)!;
            gate.PersistedState!.CurrentHp = 900;
            gate.IsAlive = true;
            fixture.Player.Money = 500;

            var result = await CastleSiegeNpcRepairAction
                .RepairAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId)
                .ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(CastleSiegeNpcOperationResult.Success));
                Assert.That(gate.PersistedState.CurrentHp, Is.EqualTo(1_000));
                Assert.That(gate.SpawnedInstance, Is.Null);
                Assert.That(fixture.Player.Money, Is.Zero);
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that only attacking participants can damage defense structures during battle.
    /// </summary>
    [Test]
    public async ValueTask DefenseStructuresAcceptOnlyBattleAttackerDamageAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        await AddSiegePlayerAsync(fixture, CastleSiegeJoinSide.Attack1, GateX - 1, GateY).ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var gate = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeGate>()
                .Single();

            Assert.That(
                await gate.AttackByAsync(fixture.Player, null, false).ConfigureAwait(false),
                Is.Null,
                "Structures must be protected outside the battle phase.");

            fixture.Context.CurrentState = CastleSiegeState.Start;
            SetPlayerJoinSide(fixture, CastleSiegeJoinSide.Defense);
            Assert.That(
                await gate.AttackByAsync(fixture.Player, null, false).ConfigureAwait(false),
                Is.Null,
                "Defenders must not damage their own structures.");

            SetPlayerJoinSide(fixture, CastleSiegeJoinSide.Attack1);
            Assert.That(
                await gate.AttackByAsync(fixture.Player, null, false).ConfigureAwait(false),
                Is.Not.Null,
                "An attacking participant must be able to damage a structure during battle.");
        }
        finally
        {
            await fixture.GameServerContext.RemovePlayerAsync(fixture.Player).ConfigureAwait(false);
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that a closed gate blocks a crossing route and synchronizes its terrain to an entrant.
    /// </summary>
    [Test]
    public async ValueTask ClosedGateBlocksCrossingRouteAndSynchronizesEntrantAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        await AddSiegePlayerAsync(fixture, CastleSiegeJoinSide.Attack1, GateX - 5, GateY).ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            await fixture.Context.NpcController.CloseGatesAsync().ConfigureAwait(false);
            var terrainView = Mock.Get(fixture.Player.ViewPlugIns.GetPlugIn<IChangeTerrainAttributesViewPlugin>()!);
            terrainView.Invocations.Clear();

            await fixture.Context.NpcController.SynchronizePlayerAsync(fixture.Player).ConfigureAwait(false);

            terrainView.Verify(
                view => view.ChangeAttributesAsync(
                    TerrainAttributeType.Blocked,
                    true,
                    It.IsAny<IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)>>()),
                Times.Once);

            var start = new Point(GateX - 5, GateY);
            var lastReachablePosition = new Point(GateX - 4, GateY);
            var target = new Point(GateX + 3, GateY);
            var steps = Enumerable.Range(start.X, target.X - start.X)
                .Select(x => new WalkingStep
                {
                    From = new Point((byte)x, GateY),
                    To = new Point((byte)(x + 1), GateY),
                    Direction = Direction.East,
                })
                .ToArray();
            var movementView = Mock.Get(fixture.Player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()!);
            movementView.Invocations.Clear();

            await fixture.Player.WalkToAsync(target, steps).ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);

            Assert.That(fixture.Player.Position, Is.EqualTo(lastReachablePosition));
            movementView.Verify(view => view.ObjectMovedAsync(fixture.Player, MoveType.Instant), Times.Never);
        }
        finally
        {
            await fixture.GameServerContext.RemovePlayerAsync(fixture.Player).ConfigureAwait(false);
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies structure and interaction-NPC creation, upgrade application, gate association, and terrain blocking.
    /// </summary>
    [Test]
    public async ValueTask ReadySpawnsConfiguredNpcsAndGateRestoresOriginalTerrainAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            fixture.Map.Terrain.WalkMap[GateX - 3, GateY] = false;
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            await fixture.Context.NpcController.CloseGatesAsync().ConfigureAwait(false);

            var gate = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeGate>()
                .Single();
            var lever = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeLever>()
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.ActiveNpcs, Has.Count.EqualTo(6));
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.One.InstanceOf<CastleSiegeGate>());
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.One.InstanceOf<CastleSiegeStatue>());
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.One.InstanceOf<CastleSiegeCrown>());
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.Exactly(2).InstanceOf<CastleSiegeSwitch>());
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.One.InstanceOf<CastleSiegeLever>());
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.None.InstanceOf<CastleSiegeMachine>());
                Assert.That(gate.MaximumHealth, Is.EqualTo(1_000));
                Assert.That(gate.Attributes[Stats.DefenseBase], Is.EqualTo(100));
                Assert.That(gate.IsClosed, Is.True);
                Assert.That(lever.Gate, Is.SameAs(gate));
            });

            for (var x = GateX - 3; x <= GateX + 2; x++)
            {
                for (var y = GateY; y <= GateY + 1; y++)
                {
                    Assert.That(fixture.Map.Terrain.WalkMap[x, y], Is.False);
                }
            }

            await gate.OpenAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(gate.IsClosed, Is.False);
                Assert.That(fixture.Map.Terrain.WalkMap[GateX - 3, GateY], Is.False);
                Assert.That(fixture.Map.Terrain.WalkMap[GateX - 2, GateY], Is.True);
                Assert.That(fixture.Map.Terrain.WalkMap[GateX + 2, GateY + 1], Is.True);
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies statue regeneration and battle-only siege-machine lifecycle.
    /// </summary>
    [Test]
    public async ValueTask StatueRegeneratesAndMachinesExistOnlyDuringStartAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var statue = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeStatue>()
                .Single();
            statue.Health = 1_000;
            statue.State.CurrentHp = statue.Health;

            Assert.That(statue.Regenerate(), Is.EqualTo(200));
            Assert.That(statue.Health, Is.EqualTo(1_200));
            Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.None.InstanceOf<CastleSiegeMachine>());

            await fixture.Context.NpcController.SpawnMachinesAsync().ConfigureAwait(false);
            Assert.That(
                fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance).OfType<CastleSiegeMachine>().ToList(),
                Has.Count.EqualTo(2));

            await fixture.Context.NpcController.DespawnMachinesAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.ActiveNpcs, Has.Count.EqualTo(6));
                Assert.That(fixture.Context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance), Has.None.InstanceOf<CastleSiegeMachine>());
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies the configured upgrade cost, item consumption, stat application, and restart persistence.
    /// </summary>
    [Test]
    public async ValueTask UpgradeConsumesCostAppliesStatsAndSurvivesRestartAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        CastleSiegeContext? restartedContext = null;
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            fixture.Player.Money = 100;
            for (byte slot = 20; slot < 22; slot++)
            {
                var jewel = fixture.Player.PersistenceContext.CreateNew<Item>();
                jewel.Definition = fixture.JewelOfGuardian;
                await fixture.Player.Inventory!.AddItemAsync(slot, jewel).ConfigureAwait(false);
            }

            var result = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    1)
                .ConfigureAwait(false);
            var gate = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeGate>()
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(CastleSiegeNpcOperationResult.Success));
                Assert.That(fixture.Player.Money, Is.Zero);
                Assert.That(fixture.Player.Inventory!.Items, Is.Empty);
                Assert.That(gate.DefenseLevel, Is.EqualTo(1));
                Assert.That(gate.Attributes[Stats.DefenseBase], Is.EqualTo(200));
            });

            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
            restartedContext = new CastleSiegeContext(fixture.GameServerContext, fixture.Configuration);
            await restartedContext.InitializeAsync(fixture.InitializationTimeUtc).ConfigureAwait(false);
            await restartedContext.NpcController.PrepareAsync().ConfigureAwait(false);
            var restartedGate = restartedContext.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeGate>()
                .Single();
            Assert.Multiple(() =>
            {
                Assert.That(restartedGate.DefenseLevel, Is.EqualTo(1));
                Assert.That(restartedGate.Attributes[Stats.DefenseBase], Is.EqualTo(200));
                Assert.That(restartedGate.Health, Is.EqualTo(1_000));
            });
        }
        finally
        {
            if (restartedContext is not null)
            {
                await restartedContext.NpcController.DespawnAllAsync().ConfigureAwait(false);
            }
            else
            {
                await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Verifies that overlapping gate terrain remains blocked until every covering gate has opened.
    /// </summary>
    [Test]
    public async ValueTask OverlappingGateTerrainUsesReferenceCountsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var firstArea = (StartX: (byte)47, StartY: (byte)50, EndX: (byte)52, EndY: (byte)51);
        var secondArea = (StartX: (byte)52, StartY: (byte)50, EndX: (byte)57, EndY: (byte)51);
        try
        {
            fixture.Context.NpcController.BlockGateTerrain(fixture.Map, firstArea);
            fixture.Context.NpcController.BlockGateTerrain(fixture.Map, secondArea);

            var stillBlocked = fixture.Context.NpcController.ReleaseGateTerrain(fixture.Map, firstArea);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Map.Terrain.WalkMap[47, 50], Is.True);
                Assert.That(fixture.Map.Terrain.WalkMap[52, 50], Is.False);
                Assert.That(stillBlocked, Does.Contain(((byte)52, (byte)50, (byte)52, (byte)50)));
            });

            fixture.Context.NpcController.ReleaseGateTerrain(fixture.Map, secondArea);
            Assert.That(fixture.Map.Terrain.WalkMap[52, 50], Is.True);
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that a server which starts during the battle restores closed gates before spawning siege machines.
    /// </summary>
    [Test]
    public async ValueTask ServerStartupDuringBattleClosesGatesAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        var battleTimeUtc = new DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc);
        var plugIn = new CastleSiegePlugIn(new FixedTimeProvider(battleTimeUtc));
        CastleSiegeContext? context = null;
        try
        {
            await plugIn.ExecuteTaskAsync(fixture.GameServerContext).ConfigureAwait(false);
            context = plugIn.GetContext(fixture.GameServerContext);
            var gate = context!.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeGate>()
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(context.CurrentState, Is.EqualTo(CastleSiegeState.Start));
                Assert.That(gate.IsClosed, Is.True);
                Assert.That(fixture.Map.Terrain.WalkMap[GateX, GateY], Is.False);
                Assert.That(
                    context.ActiveNpcs.Select(runtime => runtime.SpawnedInstance),
                    Has.Exactly(2).InstanceOf<CastleSiegeMachine>());
            });
        }
        finally
        {
            if (context is not null)
            {
                await context.NpcController.DespawnAllAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Verifies that authorization, battle-state, and maximum-level validation happen before charging the player.
    /// </summary>
    [Test]
    public async ValueTask UpgradeRejectsInvalidRequestsWithoutChargingAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            fixture.Player.Money = 100;

            fixture.Player.GuildStatus = new GuildMemberStatus(999, GuildPosition.GuildMaster);
            var unauthorized = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    1)
                .ConfigureAwait(false);

            fixture.Player.GuildStatus = new GuildMemberStatus(OwnerRuntimeGuildId, GuildPosition.GuildMaster);
            fixture.Context.CurrentState = CastleSiegeState.Start;
            var duringBattle = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    1)
                .ConfigureAwait(false);

            fixture.Context.CurrentState = CastleSiegeState.Ready;
            var runtime = fixture.Context.NpcController.FindDefenseStructure(
                (uint)CastleSiegeGate.MonsterNumber,
                GateInstanceId)!;
            runtime.PersistedState!.DefenseLevel = 1;
            var aboveMaximum = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    2)
                .ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(unauthorized, Is.EqualTo(CastleSiegeNpcOperationResult.NotAuthorized));
                Assert.That(duringBattle, Is.EqualTo(CastleSiegeNpcOperationResult.Failed));
                Assert.That(aboveMaximum, Is.EqualTo(CastleSiegeNpcOperationResult.InvalidUpgradeValue));
                Assert.That(fixture.Player.Money, Is.EqualTo(100));
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that insufficient Zen does not consume Jewels of Guardian.
    /// </summary>
    [Test]
    public async ValueTask UpgradeWithInsufficientZenDoesNotConsumeJewelsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            fixture.Player.Money = 99;
            await AddGuardianJewelsAsync(fixture, 2).ConfigureAwait(false);

            var result = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    1)
                .ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(CastleSiegeNpcOperationResult.InsufficientMoney));
                Assert.That(fixture.Player.Money, Is.EqualTo(99));
                Assert.That(fixture.Player.Inventory!.Items.Count(), Is.EqualTo(2));
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that insufficient Jewels of Guardian do not consume Zen.
    /// </summary>
    [Test]
    public async ValueTask UpgradeWithInsufficientJewelsDoesNotConsumeZenAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            fixture.Player.Money = 100;
            await AddGuardianJewelsAsync(fixture, 1).ConfigureAwait(false);

            var result = await CastleSiegeNpcUpgradeAction
                .UpgradeAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId,
                    CastleSiegeUpgradeType.Defense,
                    1)
                .ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(CastleSiegeNpcOperationResult.RequirementNotMet));
                Assert.That(fixture.Player.Money, Is.EqualTo(100));
                Assert.That(fixture.Player.Inventory!.Items.Count(), Is.EqualTo(1));
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that an alive defense structure cannot be bought again and no Zen is charged.
    /// </summary>
    [Test]
    public async ValueTask BuyAliveNpcFailsWithoutChargingAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            fixture.Player.Money = fixture.Configuration.GateBuyPrice;

            var result = await CastleSiegeNpcBuyAction
                .BuyAsync(
                    fixture.Player,
                    fixture.Context,
                    (uint)CastleSiegeGate.MonsterNumber,
                    GateInstanceId)
                .ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(CastleSiegeNpcOperationResult.RequirementNotMet));
                Assert.That(fixture.Player.Money, Is.EqualTo(fixture.Configuration.GateBuyPrice));
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies the repair formula and destroyed-structure purchase behavior.
    /// </summary>
    [Test]
    public async ValueTask RepairChargesFormulaAndBuyRespawnsDestroyedGateAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            var runtime = fixture.Context.ActiveNpcs.Single(candidate =>
                candidate.Definition.MonsterDefinition?.Number == CastleSiegeGate.MonsterNumber);
            var gate = (CastleSiegeGate)runtime.SpawnedInstance!;
            runtime.PersistedState!.DefenseLevel = 1;
            gate.ApplyPersistedUpgrades(false);
            gate.Health = 900;
            runtime.PersistedState.CurrentHp = gate.Health;
            fixture.Configuration.GateRepairCostPerHealthPoint = 2;
            fixture.Configuration.RepairCostPerUpgradeLevel = 100;
            fixture.Player.Money = 300;

            var repairResult = await CastleSiegeNpcRepairAction
                .RepairAsync(fixture.Player, fixture.Context, (uint)CastleSiegeGate.MonsterNumber, GateInstanceId)
                .ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(repairResult, Is.EqualTo(CastleSiegeNpcOperationResult.Success));
                Assert.That(fixture.Player.Money, Is.Zero);
                Assert.That(gate.Health, Is.EqualTo(gate.MaximumHealth));
            });

            await gate.OpenAsync().ConfigureAwait(false);
            await fixture.Map.RemoveAsync(gate).ConfigureAwait(false);
            gate.Dispose();
            runtime.SpawnedInstance = null;
            runtime.IsAlive = false;
            runtime.PersistedState.CurrentHp = 0;
            fixture.Player.Money = 500;

            var buyResult = await CastleSiegeNpcBuyAction
                .BuyAsync(fixture.Player, fixture.Context, (uint)CastleSiegeGate.MonsterNumber, GateInstanceId)
                .ConfigureAwait(false);
            var respawnedGate = (CastleSiegeGate)runtime.SpawnedInstance!;
            Assert.Multiple(() =>
            {
                Assert.That(buyResult, Is.EqualTo(CastleSiegeNpcOperationResult.Success));
                Assert.That(fixture.Player.Money, Is.Zero);
                Assert.That(runtime.IsAlive, Is.True);
                Assert.That(runtime.PersistedState.DefenseLevel, Is.Zero);
                Assert.That(runtime.PersistedState.LifeLevel, Is.Zero);
                Assert.That(respawnedGate.Health, Is.EqualTo(1_000));
                Assert.That(respawnedGate.IsClosed, Is.True);
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies Crown and switch proximity tracking without applying the later win-condition mechanics.
    /// </summary>
    [Test]
    public async ValueTask CrownAndSwitchTrackNearbyAlivePlayerAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            await fixture.GameServerContext.AddPlayerAsync(fixture.Player).ConfigureAwait(false);
            var crown = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeCrown>()
                .Single();
            var siegeSwitch = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeSwitch>()
                .First();
            fixture.Context.CurrentState = CastleSiegeState.Start;
            fixture.Context.IsCrownAvailable = true;
            fixture.Context.CrownAccumulatedTime = TimeSpan.FromSeconds(12);
            fixture.Player.IsAlive = true;
            await fixture.Player.WarpToAsync(new ExitGate
            {
                Map = fixture.SiegeMap,
                X1 = crown.Position.X,
                X2 = crown.Position.X,
                Y1 = crown.Position.Y,
                Y2 = crown.Position.Y,
                Direction = Direction.South,
            }).ConfigureAwait(false);
            await fixture.Player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Player.Position, Is.EqualTo(crown.Position));
                Assert.That(fixture.Player.CurrentMap, Is.SameAs(crown.CurrentMap));
                Assert.That(crown.CurrentMap.GetAttackablesInRange(crown.Position, 1), Does.Contain(fixture.Player));
            });

            using var crownIntelligence = new CastleSiegeCrownIntelligence(fixture.Context)
            {
                Npc = crown,
            };
            await crownIntelligence.TickAsync().ConfigureAwait(false);
            await crownIntelligence.TickAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.CrownUser, Is.SameAs(fixture.Player));
                Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(12)));
                Assert.That(crown.State, Is.EqualTo(CastleSiegeCrownState.Idle));
            });

            await fixture.Player.WarpToAsync(new ExitGate
            {
                Map = fixture.SiegeMap,
                X1 = siegeSwitch.Position.X,
                X2 = siegeSwitch.Position.X,
                Y1 = siegeSwitch.Position.Y,
                Y2 = siegeSwitch.Position.Y,
                Direction = Direction.South,
            }).ConfigureAwait(false);
            await fixture.Player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
            using var switchIntelligence = new CastleSiegeSwitchIntelligence(fixture.Context)
            {
                Npc = siegeSwitch,
            };
            await switchIntelligence.TickAsync().ConfigureAwait(false);
            Assert.That(fixture.Context.SwitchUsers[siegeSwitch.SwitchIndex], Is.SameAs(fixture.Player));

            fixture.Player.IsAlive = false;
            await crownIntelligence.TickAsync().ConfigureAwait(false);
            await switchIntelligence.TickAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(fixture.Context.CrownUser, Is.Null);
                Assert.That(fixture.Context.CrownAccumulatedTime, Is.EqualTo(TimeSpan.FromSeconds(12)));
                Assert.That(fixture.Context.SwitchUsers[siegeSwitch.SwitchIndex], Is.Null);
            });
        }
        finally
        {
            await fixture.GameServerContext.RemovePlayerAsync(fixture.Player).ConfigureAwait(false);
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies that a lever opens the gate interface and that a defender can operate its gate.
    /// </summary>
    [Test]
    public async ValueTask LeverInteractionOpensInterfaceAndAllowsDefenderOperationAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await fixture.Context.NpcController.PrepareAsync().ConfigureAwait(false);
            await fixture.Context.NpcController.CloseGatesAsync().ConfigureAwait(false);
            var lever = fixture.Context.ActiveNpcs
                .Select(runtime => runtime.SpawnedInstance)
                .OfType<CastleSiegeLever>()
                .Single();
            var gate = lever.Gate!;
            var plugIn = new CastleSiegeLeverTalkPlugIn();
            var eventArgs = new NpcTalkEventArgs();

            await plugIn.PlayerTalksToNpcAsync(fixture.Player, lever, eventArgs).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(eventArgs.HasBeenHandled, Is.True);
                Assert.That(eventArgs.LeavesDialogOpen, Is.True);
                Assert.That(gate.IsClosed, Is.True);
            });
            Mock.Get(fixture.Player.ViewPlugIns.GetPlugIn<ICastleSiegeNpcOperationResultPlugIn>()!)
                .Verify(
                    view => view.ShowGateInterfaceAsync(CastleSiegeNpcOperationResult.Success, gate.Id),
                    Times.Once);

            fixture.Context.CurrentState = CastleSiegeState.Start;
            fixture.Context.FinalGuildList[OwnerRuntimeGuildId] = new CastleSiegeGuildParticipant
            {
                GuildId = OwnerRuntimeGuildId,
                PersistentGuildId = fixture.OwnerPersistentGuildId,
                GuildName = "Owner",
                Side = CastleSiegeJoinSide.Defense,
            };
            var operationResult = await CastleSiegeGateOperateAction
                .OperateAsync(fixture.Player, fixture.Context, gate.Id, true)
                .ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(operationResult, Is.EqualTo(CastleSiegeNpcOperationResult.Success));
                Assert.That(gate.IsClosed, Is.False);
            });
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Verifies all client request identifiers required by the NPC issue.
    /// </summary>
    [Test]
    public void NpcRequestHandlersUseExpectedSubcodes()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new CastleSiegeDefenseBuyHandlerPlugIn().Key, Is.EqualTo(0x05));
            Assert.That(new CastleSiegeDefenseRepairHandlerPlugIn().Key, Is.EqualTo(0x06));
            Assert.That(new CastleSiegeDefenseUpgradeHandlerPlugIn().Key, Is.EqualTo(0x07));
            Assert.That(new CastleSiegeGateOperateHandlerPlugIn().Key, Is.EqualTo(0x12));
            Assert.That(new CastleSiegeGateListHandlerPlugIn().Key, Is.EqualTo(0x01));
            Assert.That(new CastleSiegeStatueListHandlerPlugIn().Key, Is.EqualTo(0x02));
        });
    }

    /// <summary>
    /// Verifies that field-bearing NPC handlers ignore truncated client packets.
    /// </summary>
    [Test]
    public async ValueTask NpcRequestHandlersIgnoreTruncatedPacketsAsync()
    {
        var fixture = await CreateFixtureAsync().ConfigureAwait(false);
        try
        {
            await new CastleSiegeDefenseBuyHandlerPlugIn()
                .HandlePacketAsync(fixture.Player, Memory<byte>.Empty)
                .ConfigureAwait(false);
            await new CastleSiegeDefenseRepairHandlerPlugIn()
                .HandlePacketAsync(fixture.Player, Memory<byte>.Empty)
                .ConfigureAwait(false);
            await new CastleSiegeDefenseUpgradeHandlerPlugIn()
                .HandlePacketAsync(fixture.Player, Memory<byte>.Empty)
                .ConfigureAwait(false);
            await new CastleSiegeGateOperateHandlerPlugIn()
                .HandlePacketAsync(fixture.Player, Memory<byte>.Empty)
                .ConfigureAwait(false);
        }
        finally
        {
            await fixture.Context.NpcController.DespawnAllAsync().ConfigureAwait(false);
        }
    }

    private static async ValueTask<TestFixture> CreateFixtureAsync()
    {
        var persistenceContextProvider = new InMemoryPersistenceContextProvider();
        BasicModel.GameConfiguration gameConfiguration;
        BasicModel.CastleSiegeConfiguration configuration;
        BasicModel.GameMapDefinition siegeMap;
        BasicModel.ItemDefinition jewelOfGuardian;
        Guid ownerPersistentGuildId;
        using (var persistenceContext = persistenceContextProvider.CreateNewContext())
        {
            gameConfiguration = persistenceContext.CreateNew<BasicModel.GameConfiguration>();
            var normalMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            normalMap.Number = 0;
            normalMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(normalMap);

            siegeMap = persistenceContext.CreateNew<BasicModel.GameMapDefinition>();
            siegeMap.Number = 30;
            siegeMap.TerrainData = new byte[65_539];
            gameConfiguration.Maps.Add(siegeMap);

            configuration = persistenceContext.CreateNew<BasicModel.CastleSiegeConfiguration>();
            configuration.Enabled = true;
            configuration.CastleSiegeMapDefinition = siegeMap;
            configuration.GateBuyPrice = 500;
            configuration.StatueBuyPrice = 400;
            configuration.CrownHoldTimeSeconds = 2;
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Ready,
                DayOfWeek = DayOfWeek.Monday,
            });
            configuration.StateSchedule.Add(new BasicModel.CastleSiegeStateScheduleEntry
            {
                State = CastleSiegeState.Start,
                DayOfWeek = DayOfWeek.Tuesday,
            });
            gameConfiguration.CastleSiegeConfiguration = configuration;

            AddUpgrade(configuration.GateDefenseUpgrades, 0, 0, 0, 100);
            AddUpgrade(configuration.GateDefenseUpgrades, 1, 2, 100, 200);
            AddUpgrade(configuration.GateLifeUpgrades, 0, 0, 0, 1_000);
            AddUpgrade(configuration.GateLifeUpgrades, 1, 1, 100, 1_500);
            AddUpgrade(configuration.StatueDefenseUpgrades, 0, 0, 0, 80);
            AddUpgrade(configuration.StatueDefenseUpgrades, 1, 1, 100, 160);
            AddUpgrade(configuration.StatueLifeUpgrades, 0, 0, 0, 2_000);
            AddUpgrade(configuration.StatueLifeUpgrades, 1, 1, 100, 3_000);
            AddUpgrade(configuration.StatueRegenUpgrades, 0, 0, 0, 0);
            AddUpgrade(configuration.StatueRegenUpgrades, 1, 1, 100, 10);

            var crown = AddMonster(216, NpcObjectKind.PassiveNpc);
            var firstSwitch = AddMonster(217, NpcObjectKind.PassiveNpc);
            var secondSwitch = AddMonster(218, NpcObjectKind.PassiveNpc);
            var lever = AddMonster(219, NpcObjectKind.PassiveNpc);
            var attackMachine = AddMonster(221, NpcObjectKind.PassiveNpc);
            var defenseMachine = AddMonster(222, NpcObjectKind.PassiveNpc);
            var gate = AddMonster(CastleSiegeGate.MonsterNumber, NpcObjectKind.Gate);
            var statue = AddMonster(CastleSiegeStatue.MonsterNumber, NpcObjectKind.Statue);
            AddAttributes(gate);
            AddAttributes(statue);

            AddNpc(configuration, crown, 1, false, 60, 60);
            AddNpc(configuration, firstSwitch, 1, false, 70, 60);
            AddNpc(configuration, secondSwitch, 1, false, 80, 60);
            AddNpc(configuration, lever, GateInstanceId, false, GateX, GateY + 4);
            AddNpc(configuration, attackMachine, 1, false, 20, 20);
            AddNpc(configuration, defenseMachine, 1, false, 30, 20);
            AddNpc(configuration, gate, GateInstanceId, true, GateX, GateY);
            AddNpc(configuration, statue, StatueInstanceId, true, 55, 55);

            var siegeData = persistenceContext.CreateNew<BasicModel.CastleSiegeData>();
            var gateState = persistenceContext.CreateNew<BasicModel.CastleSiegeNpcState>();
            gateState.MonsterNumber = CastleSiegeGate.MonsterNumber;
            gateState.InstanceId = GateInstanceId;
            gateState.CurrentHp = 1_000;
            siegeData.NpcStates.Add(gateState);
            var statueState = persistenceContext.CreateNew<BasicModel.CastleSiegeNpcState>();
            statueState.MonsterNumber = CastleSiegeStatue.MonsterNumber;
            statueState.InstanceId = StatueInstanceId;
            statueState.CurrentHp = 2_000;
            statueState.RegenLevel = 1;
            siegeData.NpcStates.Add(statueState);

            var ownerGuild = persistenceContext.CreateNew<BasicModel.Guild>();
            ownerGuild.Name = "Owner";
            ownerPersistentGuildId = ownerGuild.Id;
            siegeData.OwnerGuildId = ownerPersistentGuildId;

            jewelOfGuardian = persistenceContext.CreateNew<BasicModel.ItemDefinition>();
            jewelOfGuardian.Group = 14;
            jewelOfGuardian.Number = 31;
            jewelOfGuardian.Width = 1;
            jewelOfGuardian.Height = 1;
            gameConfiguration.Items.Add(jewelOfGuardian);
            await persistenceContext.SaveChangesAsync().ConfigureAwait(false);

            BasicModel.MonsterDefinition AddMonster(short number, NpcObjectKind objectKind)
            {
                var definition = persistenceContext.CreateNew<BasicModel.MonsterDefinition>();
                definition.Number = number;
                definition.ObjectKind = objectKind;
                gameConfiguration.Monsters.Add(definition);
                return definition;
            }
        }

        var guildServer = new Mock<IGuildServer>();
        guildServer
            .Setup(server => server.GetGuildAsync(OwnerRuntimeGuildId))
            .Returns(new ValueTask<RuntimeGuild?>(new RuntimeGuild { Name = "Owner" }));
        guildServer
            .Setup(server => server.GetGuildIdByNameAsync("Owner"))
            .Returns(new ValueTask<uint>(OwnerRuntimeGuildId));
        guildServer
            .Setup(server => server.GetPersistentGuildIdAsync(OwnerRuntimeGuildId))
            .Returns(new ValueTask<Guid?>(ownerPersistentGuildId));
        guildServer
            .Setup(server => server.GetPersistentAllianceMasterGuildIdAsync(OwnerRuntimeGuildId))
            .Returns(new ValueTask<Guid?>(ownerPersistentGuildId));

        var plugInManager = new PlugInManager([], NullLoggerFactory.Instance, null, null);
        var mapInitializer = new MapInitializer(
            gameConfiguration,
            new NullLogger<MapInitializer>(),
            NullDropGenerator.Instance,
            null);
        var gameServerContext = new GameServerContext(
            new BasicModel.GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new BasicModel.GameServerConfiguration(),
            },
            guildServer.Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            persistenceContextProvider,
            mapInitializer,
            NullLoggerFactory.Instance,
            plugInManager,
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServerContext.PlugInManager;
        mapInitializer.PathFinderPool = gameServerContext.PathFinderPool;

        var initializationTimeUtc = new DateTime(2026, 8, 3, 12, 0, 0, DateTimeKind.Utc);
        var context = new CastleSiegeContext(gameServerContext, configuration);
        await context.InitializeAsync(initializationTimeUtc).ConfigureAwait(false);
        context.CurrentState = CastleSiegeState.Ready;
        var player = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        player.GuildStatus = new GuildMemberStatus(OwnerRuntimeGuildId, GuildPosition.GuildMaster);
        var map = await gameServerContext.GetMapAsync(30).ConfigureAwait(false)
                  ?? throw new InvalidOperationException("The Castle Siege test map could not be initialized.");
        return new(
            persistenceContextProvider,
            configuration,
            siegeMap,
            jewelOfGuardian,
            gameServerContext,
            context,
            map,
            player,
            ownerPersistentGuildId,
            initializationTimeUtc);

        void AddUpgrade(
            ICollection<CastleSiegeUpgradeDefinition> definitions,
            byte level,
            int jewels,
            int zen,
            int value)
        {
            definitions.Add(new BasicModel.CastleSiegeUpgradeDefinition
            {
                Level = level,
                RequiredJewelOfGuardianCount = jewels,
                RequiredZen = zen,
                Value = value,
            });
        }

        void AddNpc(
            CastleSiegeConfiguration castleSiegeConfiguration,
            MonsterDefinition monster,
            byte instanceId,
            bool persisted,
            byte x,
            byte y)
        {
            castleSiegeConfiguration.NpcDefinitions.Add(new BasicModel.CastleSiegeNpcDefinition
            {
                MonsterDefinition = monster,
                InstanceId = instanceId,
                IsPersistedToDatabase = persisted,
                DefaultSide = CastleSiegeJoinSide.Defense,
                SpawnX = x,
                SpawnY = y,
                Direction = Direction.South,
            });
        }

        void AddAttributes(BasicModel.MonsterDefinition monster)
        {
            monster.Attributes.Add(new BasicModel.MonsterAttribute
            {
                AttributeDefinition = Stats.MaximumHealth,
                Value = 1,
            });
            monster.Attributes.Add(new BasicModel.MonsterAttribute
            {
                AttributeDefinition = Stats.DefenseBase,
                Value = 0,
            });
        }
    }

    private static async ValueTask AddSiegePlayerAsync(
        TestFixture fixture,
        CastleSiegeJoinSide side,
        byte x,
        byte y)
    {
        fixture.Player.IsAlive = true;
        await fixture.GameServerContext.AddPlayerAsync(fixture.Player).ConfigureAwait(false);
        await fixture.Player.WarpToAsync(new ExitGate
        {
            Map = fixture.SiegeMap,
            X1 = x,
            X2 = x,
            Y1 = y,
            Y2 = y,
            Direction = Direction.South,
        }).ConfigureAwait(false);
        await fixture.Player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        fixture.Context.TrackPlayer(fixture.Player, fixture.Map);
        SetPlayerJoinSide(fixture, side);
    }

    private static async ValueTask AddGuardianJewelsAsync(TestFixture fixture, int count)
    {
        for (byte offset = 0; offset < count; offset++)
        {
            var jewel = fixture.Player.PersistenceContext.CreateNew<Item>();
            jewel.Definition = fixture.JewelOfGuardian;
            await fixture.Player.Inventory!.AddItemAsync((byte)(20 + offset), jewel).ConfigureAwait(false);
        }
    }

    private static void SetPlayerJoinSide(TestFixture fixture, CastleSiegeJoinSide side)
    {
        fixture.Context.FinalGuildList[OwnerRuntimeGuildId] = new CastleSiegeGuildParticipant
        {
            GuildId = OwnerRuntimeGuildId,
            PersistentGuildId = fixture.OwnerPersistentGuildId,
            GuildName = "Owner",
            Side = side,
        };
    }

    private sealed record TestFixture(
        InMemoryPersistenceContextProvider PersistenceContextProvider,
        CastleSiegeConfiguration Configuration,
        GameMapDefinition SiegeMap,
        ItemDefinition JewelOfGuardian,
        GameServerContext GameServerContext,
        CastleSiegeContext Context,
        GameMap Map,
        Player Player,
        Guid OwnerPersistentGuildId,
        DateTime InitializationTimeUtc);

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = utcNow;

        /// <inheritdoc />
        public override DateTimeOffset GetUtcNow() => this._utcNow;
    }
}
