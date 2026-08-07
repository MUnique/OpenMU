// <copyright file="CastleSiegeNpcController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Creates and synchronizes the configured Castle Siege NPCs.
/// </summary>
public sealed class CastleSiegeNpcController
{
    private readonly CastleSiegeContext _context;
    private readonly object _gateTerrainLock = new();
    private readonly Dictionary<Point, (bool WasWalkable, int ReferenceCount)> _gateTerrain = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeNpcController"/> class.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    public CastleSiegeNpcController(CastleSiegeContext context)
    {
        this._context = context;
    }

    /// <summary>
    /// Spawns persistent structures and static interactive NPCs for the Ready phase.
    /// Siege machines are intentionally excluded.
    /// </summary>
    /// <returns>A task that represents the asynchronous preparation.</returns>
    public async ValueTask PrepareAsync()
    {
        foreach (var definition in this._context.Configuration.NpcDefinitions
                     .Where(definition => !IsMachine(definition)))
        {
            var runtime = this.GetOrCreateRuntime(definition);
            if (runtime.Definition.IsPersistedToDatabase
                && runtime.PersistedState is { CurrentHp: <= 0 })
            {
                runtime.IsAlive = false;
                continue;
            }

            await this.EnsureSpawnedAsync(runtime).ConfigureAwait(false);
        }

        this.AssociateLevers();
    }

    /// <summary>
    /// Spawns the non-persistent siege machines at battle start.
    /// </summary>
    /// <returns>A task that represents the asynchronous spawn operation.</returns>
    public async ValueTask SpawnMachinesAsync()
    {
        foreach (var definition in this._context.Configuration.NpcDefinitions.Where(IsMachine))
        {
            var runtime = this.GetOrCreateRuntime(definition);
            await this.EnsureSpawnedAsync(runtime).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Closes every alive Castle Siege gate.
    /// </summary>
    /// <returns>A task that represents the asynchronous close operation.</returns>
    public async ValueTask CloseGatesAsync()
    {
        foreach (var gate in this._context.ActiveNpcs
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeGate>())
        {
            await gate.CloseAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Synchronizes persisted HP and alive flags with spawned NPC objects.
    /// </summary>
    public void SynchronizeNpcStates()
    {
        foreach (var runtime in this._context.ActiveNpcs)
        {
            if (runtime.SpawnedInstance is CastleSiegeAttackableNpc attackable)
            {
                runtime.IsAlive = attackable.IsAlive;
                if (runtime.PersistedState is { } state)
                {
                    state.CurrentHp = attackable.Health;
                }
            }
            else if (runtime.Definition.IsPersistedToDatabase)
            {
                runtime.IsAlive = runtime.PersistedState is { CurrentHp: > 0 };
            }
            else
            {
                runtime.IsAlive = runtime.SpawnedInstance is not null;
            }
        }
    }

    /// <summary>
    /// Gets an attackable NPC runtime by its packet identity.
    /// </summary>
    /// <param name="monsterNumber">The monster number.</param>
    /// <param name="npcIndex">The spawned object or configured instance identifier.</param>
    /// <returns>The matching runtime, or <see langword="null"/>.</returns>
    public CastleSiegeNpcRuntime? FindDefenseStructure(uint monsterNumber, uint npcIndex)
    {
        // Management packets use the configured instance id, while the gate interface returns the live map object id.
        return this._context.ActiveNpcs.FirstOrDefault(runtime =>
            runtime.Definition.MonsterDefinition?.Number == monsterNumber
            && (runtime.Definition.InstanceId == npcIndex || runtime.SpawnedInstance?.Id == npcIndex));
    }

    /// <summary>
    /// Gets an immutable management snapshot of one defense-structure type.
    /// </summary>
    /// <param name="monsterNumber">The gate or Guardian Statue monster number.</param>
    /// <returns>The configured defense structures in management-index order.</returns>
    public async ValueTask<IReadOnlyList<CastleSiegeNpcInfo>> GetDefenseStructureSnapshotAsync(short monsterNumber)
    {
        await this._context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return this.GetDefenseStructures(monsterNumber)
                .Select(this.CreateInfo)
                .ToList();
        }
        finally
        {
            this._context.ExecutionLock.Release();
        }
    }

    /// <summary>
    /// Gets the configured maximum health of a defense structure at its persisted life level.
    /// </summary>
    /// <param name="runtime">The defense-structure runtime.</param>
    /// <returns>The configured maximum health, or 0 when no matching level exists.</returns>
    public int GetMaximumHealth(CastleSiegeNpcRuntime runtime)
    {
        var monsterNumber = runtime.Definition.MonsterDefinition?.Number ?? 0;
        var lifeLevel = runtime.PersistedState?.LifeLevel ?? byte.MaxValue;
        return this._context.Configuration.GetUpgrades(monsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(lifeLevel) ?? 0;
    }

    /// <summary>
    /// Gets a gate by its spawned object or configured instance identifier.
    /// </summary>
    /// <param name="gateId">The gate identifier.</param>
    /// <returns>The matching gate, or <see langword="null"/>.</returns>
    public CastleSiegeGate? FindGate(ushort gateId)
    {
        return this._context.ActiveNpcs
            .Where(runtime => runtime.Definition.MonsterDefinition?.Number == CastleSiegeGate.MonsterNumber)
            .Where(runtime => runtime.Definition.InstanceId == gateId || runtime.SpawnedInstance?.Id == gateId)
            .Select(runtime => runtime.SpawnedInstance)
            .OfType<CastleSiegeGate>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Spawns a re-purchased defense structure.
    /// </summary>
    /// <param name="runtime">The structure runtime.</param>
    /// <returns>A task that represents the asynchronous respawn operation.</returns>
    public ValueTask RespawnAsync(CastleSiegeNpcRuntime runtime)
    {
        return this.EnsureSpawnedAsync(runtime);
    }

    /// <summary>
    /// Despawns all battle-only siege machines.
    /// </summary>
    /// <returns>A task that represents the asynchronous despawn operation.</returns>
    public async ValueTask DespawnMachinesAsync()
    {
        foreach (var runtime in this._context.ActiveNpcs.Where(runtime => IsMachine(runtime.Definition)).ToList())
        {
            await DespawnAsync(runtime).ConfigureAwait(false);
            this._context.ActiveNpcs.Remove(runtime);
        }
    }

    /// <summary>
    /// Despawns all Castle Siege-owned NPC objects.
    /// </summary>
    /// <returns>A task that represents the asynchronous despawn operation.</returns>
    public async ValueTask DespawnAllAsync()
    {
        foreach (var runtime in this._context.ActiveNpcs.ToList())
        {
            await DespawnAsync(runtime).ConfigureAwait(false);
        }

        this._context.ActiveNpcs.RemoveAll(runtime => !runtime.Definition.IsPersistedToDatabase);
    }

    /// <summary>
    /// Applies a reference-counted terrain block for one gate area.
    /// </summary>
    /// <param name="map">The Castle Siege map.</param>
    /// <param name="area">The gate terrain area.</param>
    internal void BlockGateTerrain(
        GameMap map,
        (byte StartX, byte StartY, byte EndX, byte EndY) area)
    {
        lock (this._gateTerrainLock)
        {
            for (var x = area.StartX; x <= area.EndX; x++)
            {
                for (var y = area.StartY; y <= area.EndY; y++)
                {
                    var point = new Point(x, y);
                    if (this._gateTerrain.TryGetValue(point, out var state))
                    {
                        this._gateTerrain[point] = (state.WasWalkable, state.ReferenceCount + 1);
                    }
                    else
                    {
                        this._gateTerrain[point] = (map.Terrain.WalkMap[x, y], 1);
                    }

                    map.Terrain.WalkMap[x, y] = false;
                    map.Terrain.UpdateAiGridValue(x, y);
                }
            }
        }
    }

    /// <summary>
    /// Releases one gate's terrain block and returns tiles which must remain blocked.
    /// </summary>
    /// <param name="map">The Castle Siege map.</param>
    /// <param name="area">The gate terrain area.</param>
    /// <returns>Tiles which were originally blocked or are still covered by another closed gate.</returns>
    internal IReadOnlyCollection<(byte StartX, byte StartY, byte EndX, byte EndY)> ReleaseGateTerrain(
        GameMap map,
        (byte StartX, byte StartY, byte EndX, byte EndY) area)
    {
        var blockedAreas = new List<(byte StartX, byte StartY, byte EndX, byte EndY)>();
        lock (this._gateTerrainLock)
        {
            for (var x = area.StartX; x <= area.EndX; x++)
            {
                for (var y = area.StartY; y <= area.EndY; y++)
                {
                    var point = new Point(x, y);
                    if (!this._gateTerrain.Remove(point, out var state))
                    {
                        continue;
                    }

                    if (state.ReferenceCount > 1)
                    {
                        this._gateTerrain[point] = (state.WasWalkable, state.ReferenceCount - 1);
                        map.Terrain.WalkMap[x, y] = false;
                    }
                    else
                    {
                        map.Terrain.WalkMap[x, y] = state.WasWalkable;
                    }

                    map.Terrain.UpdateAiGridValue(x, y);
                    if (!map.Terrain.WalkMap[x, y])
                    {
                        blockedAreas.Add((x, y, x, y));
                    }
                }
            }
        }

        return blockedAreas;
    }

    /// <summary>
    /// Initializes the persistent defense-structure runtimes after the siege data has been loaded.
    /// </summary>
    internal void InitializePersistentStructures()
    {
        foreach (var definition in this._context.Configuration.NpcDefinitions
                     .Where(definition => definition.IsPersistedToDatabase))
        {
            this.GetOrCreateRuntime(definition);
        }
    }

    /// <summary>
    /// Sends the current state of all Castle Siege structures which affect a player's client.
    /// </summary>
    /// <param name="player">The player entering the Castle Siege map.</param>
    /// <returns>A task that represents the asynchronous synchronization.</returns>
    internal async ValueTask SynchronizePlayerAsync(Player player)
    {
        foreach (var gate in this._context.ActiveNpcs
                     .Select(runtime => runtime.SpawnedInstance)
                     .OfType<CastleSiegeGate>())
        {
            await gate.SynchronizeStateAsync(player).ConfigureAwait(false);
        }
    }

    private static bool IsMachine(CastleSiegeNpcDefinition definition)
    {
        var monsterNumber = definition.MonsterDefinition?.Number;
        return monsterNumber == CastleSiegeMachine.AttackMonsterNumber
               || monsterNumber == CastleSiegeMachine.DefenseMonsterNumber;
    }

    private static async ValueTask RemoveConfiguredDuplicateAsync(GameMap map, CastleSiegeNpcRuntime runtime)
    {
        var position = new Point(
            runtime.Definition.SpawnX,
            runtime.Definition.SpawnY);
        var duplicates = map.GetNpcsInRange(position, 1)
            .Where(npc => npc is not ICastleSiegeNpc
                          && npc.Definition.Number == runtime.Definition.MonsterDefinition?.Number
                          && npc.Position == position)
            .ToList();
        foreach (var duplicate in duplicates)
        {
            await map.RemoveAsync(duplicate).ConfigureAwait(false);
            duplicate.Dispose();
        }
    }

    private static async ValueTask DespawnAsync(CastleSiegeNpcRuntime runtime)
    {
        if (runtime.SpawnedInstance is CastleSiegeGate gate)
        {
            await gate.OpenAsync().ConfigureAwait(false);
        }

        if (runtime.SpawnedInstance is NonPlayerCharacter npc)
        {
            await npc.CurrentMap.RemoveAsync(npc).ConfigureAwait(false);
            npc.Dispose();
        }

        runtime.SpawnedInstance = null;
        runtime.IsAlive = runtime.PersistedState is { CurrentHp: > 0 };
    }

    private List<CastleSiegeNpcRuntime> GetDefenseStructures(short monsterNumber)
    {
        if (monsterNumber != CastleSiegeGate.MonsterNumber
            && monsterNumber != CastleSiegeStatue.MonsterNumber)
        {
            return [];
        }

        return this._context.ActiveNpcs
            .Where(runtime => runtime.Definition.IsPersistedToDatabase
                              && runtime.Definition.MonsterDefinition?.Number == monsterNumber)
            .OrderBy(runtime => runtime.Definition.InstanceId)
            .ToList();
    }

    private CastleSiegeNpcInfo CreateInfo(CastleSiegeNpcRuntime runtime)
    {
        var state = runtime.PersistedState;
        var attackable = runtime.SpawnedInstance as CastleSiegeAttackableNpc;
        return new CastleSiegeNpcInfo(
            (uint)(runtime.Definition.MonsterDefinition?.Number ?? 0),
            runtime.Definition.InstanceId,
            state?.DefenseLevel ?? 0,
            state?.RegenLevel ?? 0,
            attackable?.MaximumHealth ?? this.GetMaximumHealth(runtime),
            attackable?.Health ?? state?.CurrentHp ?? 0,
            runtime.Definition.SpawnX,
            runtime.Definition.SpawnY,
            runtime.IsAlive);
    }

    private CastleSiegeNpcRuntime GetOrCreateRuntime(CastleSiegeNpcDefinition definition)
    {
        var runtime = this._context.ActiveNpcs.FirstOrDefault(candidate =>
            candidate.Definition.MonsterDefinition?.Number == definition.MonsterDefinition?.Number
            && candidate.Definition.InstanceId == definition.InstanceId);
        if (runtime is not null)
        {
            return runtime;
        }

        CastleSiegeNpcState? state = null;
        if (definition.IsPersistedToDatabase)
        {
            var monsterNumber = definition.MonsterDefinition?.Number
                                ?? throw Error.NotInitializedProperty(definition, nameof(definition.MonsterDefinition));
            state = this._context.SiegeData.NpcStates.FirstOrDefault(candidate =>
                candidate.MonsterNumber == monsterNumber
                && candidate.InstanceId == definition.InstanceId);
            if (state is null)
            {
                state = new CastleSiegeNpcState
                {
                    MonsterNumber = monsterNumber,
                    InstanceId = definition.InstanceId,
                    CurrentHp = this.GetInitialHealth(monsterNumber),
                };
                this._context.SiegeData.NpcStates.Add(state);
            }
        }

        runtime = new CastleSiegeNpcRuntime
        {
            Definition = definition,
            PersistedState = state,
            IsAlive = state?.CurrentHp > 0 || !definition.IsPersistedToDatabase,
        };
        this._context.ActiveNpcs.Add(runtime);
        return runtime;
    }

    private int GetInitialHealth(short monsterNumber)
    {
        if (monsterNumber != CastleSiegeGate.MonsterNumber
            && monsterNumber != CastleSiegeStatue.MonsterNumber)
        {
            throw new ArgumentOutOfRangeException(nameof(monsterNumber), monsterNumber, "Not a persistent Castle Siege structure.");
        }

        return this._context.Configuration.GetUpgrades(monsterNumber, CastleSiegeUpgradeType.Life)?.GetValue(0)
               ?? throw new InvalidOperationException($"Initial health for Castle Siege NPC {monsterNumber} is not configured.");
    }

    private async ValueTask EnsureSpawnedAsync(CastleSiegeNpcRuntime runtime)
    {
        if (runtime.SpawnedInstance is { })
        {
            runtime.IsAlive = true;
            return;
        }

        var mapNumber = this._context.Configuration.CastleSiegeMapDefinition?.Number
                        ?? throw Error.NotInitializedProperty(this._context.Configuration, nameof(this._context.Configuration.CastleSiegeMapDefinition));
        var map = await this._context.GameContext.GetMapAsync(checked((ushort)mapNumber)).ConfigureAwait(false)
                  ?? throw new InvalidOperationException($"Castle Siege map {mapNumber} is not hosted by this game context.");
        var definition = runtime.Definition;
        var monsterDefinition = definition.MonsterDefinition
                                ?? throw Error.NotInitializedProperty(definition, nameof(definition.MonsterDefinition));
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map.Definition,
            X1 = definition.SpawnX,
            X2 = definition.SpawnX,
            Y1 = definition.SpawnY,
            Y2 = definition.SpawnY,
            Direction = definition.Direction,
            Quantity = 1,
            SpawnTrigger = SpawnTrigger.ManuallyForEvent,
        };

        await RemoveConfiguredDuplicateAsync(map, runtime).ConfigureAwait(false);
        var npc = this.CreateNpc(spawnArea, map, runtime);
        npc.Initialize();
        await map.AddAsync(npc).ConfigureAwait(false);
        npc.OnSpawn();
        runtime.SpawnedInstance = npc;
        runtime.IsAlive = true;
    }

    private NonPlayerCharacter CreateNpc(
        MonsterSpawnArea spawnArea,
        GameMap map,
        CastleSiegeNpcRuntime runtime)
    {
        var definition = spawnArea.MonsterDefinition!;
        return definition.Number switch
        {
            var number when number == CastleSiegeGate.MonsterNumber => new CastleSiegeGate(
                spawnArea,
                definition,
                map,
                this._context,
                runtime,
                new CastleSiegeGateIntelligence(),
                this._context.GameContext.DropGenerator,
                this._context.GameContext.PlugInManager),
            var number when number == CastleSiegeStatue.MonsterNumber => new CastleSiegeStatue(
                spawnArea,
                definition,
                map,
                this._context,
                runtime,
                new CastleSiegeStatueIntelligence(),
                this._context.GameContext.DropGenerator,
                this._context.GameContext.PlugInManager),
            var number when number == CastleSiegeCrown.MonsterNumber => new CastleSiegeCrown(
                spawnArea,
                definition,
                map,
                runtime,
                new CastleSiegeCrownIntelligence(this._context)),
            var number when number == CastleSiegeSwitch.FirstMonsterNumber => new CastleSiegeSwitch(
                spawnArea,
                definition,
                map,
                runtime,
                new CastleSiegeSwitchIntelligence(this._context),
                0),
            var number when number == CastleSiegeSwitch.SecondMonsterNumber => new CastleSiegeSwitch(
                spawnArea,
                definition,
                map,
                runtime,
                new CastleSiegeSwitchIntelligence(this._context),
                1),
            var number when number == CastleSiegeLever.MonsterNumber => new CastleSiegeLever(
                spawnArea,
                definition,
                map,
                this._context,
                runtime,
                new CastleSiegeLeverIntelligence()),
            var number when number == CastleSiegeMachine.AttackMonsterNumber
                            || number == CastleSiegeMachine.DefenseMonsterNumber => new CastleSiegeMachine(
                spawnArea,
                definition,
                map,
                runtime,
                new CastleSiegeMachineIntelligence()),
            _ => throw new NotSupportedException($"Castle Siege NPC {definition.Number} is not supported."),
        };
    }

    private void AssociateLevers()
    {
        var gatesByInstance = this._context.ActiveNpcs
            .Where(runtime => runtime.Definition.MonsterDefinition?.Number == CastleSiegeGate.MonsterNumber)
            .Where(runtime => runtime.SpawnedInstance is CastleSiegeGate)
            .ToDictionary(runtime => runtime.Definition.InstanceId, runtime => (CastleSiegeGate)runtime.SpawnedInstance!);
        foreach (var runtime in this._context.ActiveNpcs
                     .Where(runtime => runtime.SpawnedInstance is CastleSiegeLever))
        {
            ((CastleSiegeLever)runtime.SpawnedInstance!).Gate =
                gatesByInstance.GetValueOrDefault(runtime.Definition.InstanceId);
        }
    }
}
