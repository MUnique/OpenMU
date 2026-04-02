// <copyright file="BaseInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for invasion plugins.
/// </summary>
/// <typeparam name="TConfiguration">Configuration.</typeparam>
public abstract class BaseInvasionPlugIn<TConfiguration> : PeriodicTaskBasePlugIn<TConfiguration, InvasionGameServerState>, IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<TConfiguration>
    where TConfiguration : PeriodicInvasionConfiguration
{
    private readonly MapEventType? _mapEventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseInvasionPlugIn{TConfiguration}" /> class.
    /// </summary>
    /// <param name="mapEventType">Type of the map event. If null, no map event state updates are sent.</param>
    protected BaseInvasionPlugIn(MapEventType? mapEventType = null)
    {
        this._mapEventType = mapEventType;
    }

    /// <summary>
    /// Occurs when the event has finished.
    /// </summary>
    public event Func<Task>? Finished;

    /// <summary>
    /// Gets the list of map IDs from which the event display map is randomly selected.
    /// When set, <see cref="InvasionGameServerState.MapId"/> is chosen from this list
    /// and the map event state UI is only shown on that single map.
    /// If null, <see cref="InvasionGameServerState.MapId"/> is chosen from <see cref="InvasionGameServerState.MapIds"/>.
    /// </summary>
    protected virtual IReadOnlyList<ushort>? EventDisplayMapIds => null;

    /// <inheritdoc />
    public virtual async ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (this._mapEventType is null)
        {
            return;
        }

        if (addedObject is Player player)
        {
            var state = this.GetStateByGameContext(player.GameContext);
            var isEnabled = state.State != PeriodicTaskState.NotStarted;
            await this.TrySendMapEventStateUpdateAsync(player, isEnabled).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawns the given quantity of a monster on the map.
    /// For each monster, picks a random walkable coordinate via a spiral search —
    /// no allocation, no exception on invalid terrain.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="gameMap">The game map.</param>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="quantity">The quantity.</param>
    /// <param name="x">The optional fixed X coordinate.</param>
    /// <param name="y">The optional fixed Y coordinate.</param>
    protected async ValueTask CreateMonstersAsync(IGameContext gameContext, GameMap gameMap, MonsterDefinition monsterDefinition, ushort quantity, byte? x = null, byte? y = null)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType());

        while (quantity-- > 0)
        {
            Point? spawnPoint;
            if (x.HasValue && y.HasValue)
            {
                spawnPoint = new Point(x.Value, y.Value);
            }
            else
            {
                spawnPoint = gameMap.Terrain.GetRandomWalkableCoordinate();
            }

            if (spawnPoint is null)
            {
                logger.LogDebug(
                    "Skipping one {monster} on {map}: no walkable cell found in rolled area.",
                    monsterDefinition.Designation,
                    gameMap.Definition.Name);
                continue;
            }

            var area = new MonsterSpawnArea
            {
                GameMap = gameMap.Definition,
                MonsterDefinition = monsterDefinition,
                SpawnTrigger = SpawnTrigger.OnceAtEventStart,
                Quantity = 1,
                X1 = spawnPoint.Value.X,
                X2 = spawnPoint.Value.X,
                Y1 = spawnPoint.Value.Y,
                Y2 = spawnPoint.Value.Y,
            };

            var intelligence = new BasicMonsterIntelligence();
            var monster = new Monster(area, monsterDefinition, gameMap, gameContext.DropGenerator, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);

            monster.Initialize();
            await gameMap.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();

            async Task CleanUpOnFinishAsync()
            {
                this.Finished -= CleanUpOnFinishAsync;
                if (monster is not null && !monster.IsDisposed)
                {
                    await monster.CurrentMap.RemoveAsync(monster).ConfigureAwait(false);
                    monster.Dispose();
                }
            }

            this.Finished += CleanUpOnFinishAsync;
            monster.Died += (_, _) => this.Finished -= CleanUpOnFinishAsync;
        }
    }

    /// <summary>
    /// Spawn mobs on a specific map.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="mapId">The map id.</param>
    /// <param name="spawns">The spawn configurations.</param>
    protected async ValueTask SpawnMobsAsync(IGameContext gameContext, ushort mapId, IEnumerable<InvasionSpawnConfiguration> spawns)
    {
        var gameMap = await gameContext.GetMapAsync(mapId).ConfigureAwait(false);

        if (gameMap is null)
        {
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType());
        foreach (var spawn in spawns)
        {
            if (gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == spawn.MonsterId) is { } monsterDefinition)
            {
                await this.CreateMonstersAsync(gameContext, gameMap, monsterDefinition, spawn.Count, spawn.X, spawn.Y).ConfigureAwait(false);
            }
            else
            {
                logger.LogDebug("Skipping spawning of monster with number {mobId}, because monster definition wasn't found.", spawn.MonsterId);
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnPrepareEventAsync(InvasionGameServerState state)
    {
        var config = this.Configuration;
        if (config?.Mobs is null || config.Mobs.Count == 0)
        {
            return;
        }

        foreach (var mob in config.Mobs)
        {
            if (mob.MapIds.Count == 0)
            {
                continue;
            }

            if (mob.IsSpawnOnAllMaps)
            {
                foreach (var mapId in mob.MapIds)
                {
                    state.MapIds.Add(mapId);
                }
            }
            else
            {
                var randomMapId = mob.MapIds[Rand.NextInt(0, mob.MapIds.Count)];
                state.MapIds.Add(randomMapId);
                state.SelectedMaps[mob.MonsterId] = randomMapId;
            }
        }

        if (this.EventDisplayMapIds is { Count: > 0 } displayMaps)
        {
            state.MapId = displayMaps[Rand.NextInt(0, displayMaps.Count)];
        }
        else if (state.MapIds.Count > 0)
        {
            state.MapId = state.MapIds.Min();
        }
    }

    /// <inheritdoc />
    protected override InvasionGameServerState CreateState(IGameContext gameContext)
    {
        return new InvasionGameServerState(gameContext);
    }

    /// <summary>
    /// Send a golden message to all online players.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="state">The server state.</param>
    protected async Task TrySendStartMessageAsync(Player player, InvasionGameServerState state)
    {
        var configuration = this.Configuration;

        if (configuration is null || state.MapIds.Count == 0)
        {
            return;
        }

        var mapName = state.Context.Configuration.Maps
                          .FirstOrDefault(m => m.Number == state.MapId)
                          ?.Name.GetTranslation(player.Culture)
                      ?? string.Empty;

        var message = (configuration.Message.GetTranslation(player.Culture)
                       ?? PlugInResources.BaseInvasionPlugIn_DefaultStartMessage).Replace("{mapName}", mapName, StringComparison.InvariantCulture);

        try
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, Interfaces.MessageType.GoldenCenter)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending start message.");
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnPreparedAsync(InvasionGameServerState state)
    {
        await state.Context.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p, state)).ConfigureAwait(false);

        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, true)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(InvasionGameServerState state)
    {
        await this.SpawnMobsOnMapsAsync(state).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnFinishedAsync(InvasionGameServerState state)
    {
        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, false)).ConfigureAwait(false);
        }

        if (this.Finished is not null)
        {
            await this.Finished.Invoke().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawn mobs based on pre-selected maps in the state.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask SpawnMobsOnMapsAsync(InvasionGameServerState state)
    {
        var config = this.Configuration;
        if (config?.Mobs is not { } spawns || spawns.Count == 0)
        {
            return;
        }

        var gameContext = state.Context;

        foreach (var spawn in spawns)
        {
            if (spawn.IsSpawnOnAllMaps)
            {
                foreach (var mapId in spawn.MapIds)
                {
                    await this.SpawnMobsAsync(gameContext, mapId, [spawn]).ConfigureAwait(false);
                }
            }
            else if (state.SelectedMaps.TryGetValue(spawn.MonsterId, out var selectedMapId))
            {
                await this.SpawnMobsAsync(gameContext, selectedMapId, [spawn]).ConfigureAwait(false);
            }
        }
    }

    private async Task TrySendMapEventStateUpdateAsync(Player player, bool enabled)
    {
        if (this._mapEventType is null || !this.IsPlayerOnMap(player))
        {
            return;
        }

        try
        {
            await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, this._mapEventType.Value)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending map event state update, event type: {MapEventType}", this._mapEventType);
        }
    }

    private bool IsPlayerOnMap(Player player)
    {
        var state = this.GetStateByGameContext(player.GameContext);

        return player.CurrentMap is { } map
               && !player.PlayerState.CurrentState.IsDisconnectedOrFinished()
               && map.MapId == state.MapId;
    }
}