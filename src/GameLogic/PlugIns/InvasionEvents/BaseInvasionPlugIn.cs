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
/// <typeparam name="TConfiguration">The concrete configuration type.</typeparam>
public abstract class BaseInvasionPlugIn<TConfiguration> : PeriodicTaskBasePlugIn<TConfiguration, InvasionGameServerState>, IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<TConfiguration>
    where TConfiguration : PeriodicInvasionConfiguration
{
    private readonly MapEventType? _mapEventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseInvasionPlugIn{TConfiguration}"/> class.
    /// </summary>
    /// <param name="mapEventType">
    /// The map-event type used for UI state broadcasts.
    /// Pass <c>null</c> to disable map-event state updates.
    /// </param>
    protected BaseInvasionPlugIn(MapEventType? mapEventType = null)
    {
        this._mapEventType = mapEventType;
    }

    /// <summary>
    /// Gets the list of map IDs from which the event display map is randomly selected.
    /// When non-empty, <see cref="InvasionGameServerState.MapId"/> is chosen from this
    /// list and map-event UI is shown only on that single map.
    /// When <c>null</c>, <see cref="InvasionGameServerState.MapId"/> falls back to the
    /// minimum of <see cref="InvasionGameServerState.MapIds"/>.
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
            await this.TrySendMapEventStateUpdateAsync(player, state).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawns <paramref name="quantity"/> instances of <paramref name="monsterDefinition"/> on
    /// <paramref name="gameMap"/>, each placed at a random walkable coordinate (or a fixed
    /// coordinate when <paramref name="x"/> and <paramref name="y"/> are provided).
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="gameMap">The game map.</param>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="quantity">The quantity.</param>
    /// <param name="x">The optional fixed X coordinate.</param>
    /// <param name="y">The optional fixed Y coordinate.</param>
    protected async ValueTask CreateMonstersAsync(IGameContext gameContext, ILogger logger, GameMap gameMap, MonsterDefinition monsterDefinition, ushort quantity, byte? x = null, byte? y = null)
    {
        for (var i = 0; i < quantity; i++)
        {
            Point? spawnPoint = (x.HasValue && y.HasValue)
                ? new Point(x.Value, y.Value)
                : gameMap.Terrain.RandomWalkableCoordinate;

            if (spawnPoint is null)
            {
                logger.LogDebug("Skipping one {Monster} on {Map}: no walkable cell found.", monsterDefinition.Designation, gameMap.Definition.Name);
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
            var monster = new Monster(
                area,
                monsterDefinition,
                gameMap,
                gameContext.DropGenerator,
                intelligence,
                gameContext.PlugInManager,
                gameContext.PathFinderPool);

            monster.Initialize();
            await gameMap.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();

            var state = this.GetStateByGameContext(gameContext);
            state.AddMonster(monster);
        }
    }

    /// <summary>
    /// Spawns all configured mobs for the given <paramref name="mapId"/>.
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
                await this.CreateMonstersAsync(gameContext, logger, gameMap, monsterDefinition, spawn.Count, spawn.X, spawn.Y).ConfigureAwait(false);
            }
            else
            {
                logger.LogDebug("Skipping monster {MobId}: definition not found.", spawn.MonsterId);
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnPrepareEventAsync(InvasionGameServerState state)
    {
        var config = this.Configuration;
        if (config?.Mobs is not { Count: > 0 } mobs)
        {
            return;
        }

        this.SelectSpawnMaps(mobs, state);
        this.SelectDisplayMap(state);
    }

    /// <inheritdoc />
    protected override InvasionGameServerState CreateState(IGameContext gameContext)
        => new(gameContext);

    /// <summary>
    /// Sends the invasion start message to a single player.
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

        var message = (configuration.StartMessage.GetTranslation(player.Culture)
                       ?? PlugInResources.BaseInvasionPlugIn_DefaultStartMessage)
            .Replace("{mapName}", mapName, StringComparison.InvariantCulture);

        try
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending invasion start message.");
        }
    }

    /// <summary>
    /// Sends the invasion end message to a single player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="state">The server state.</param>
    protected async Task TrySendEndMessageAsync(Player player, InvasionGameServerState state)
    {
        var configuration = this.Configuration;
        if (configuration is null || state.MapIds.Count == 0 || string.IsNullOrWhiteSpace(configuration.EndMessage))
        {
            return;
        }

        var mapName = state.Context.Configuration.Maps
                          .FirstOrDefault(m => m.Number == state.MapId)
                          ?.Name.GetTranslation(player.Culture)
                      ?? string.Empty;

        var message = (configuration.EndMessage.GetTranslation(player.Culture) ?? string.Empty)
            .Replace("{mapName}", mapName, StringComparison.InvariantCulture);

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        try
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.GoldenCenter)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending invasion end message.");
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnPreparedAsync(InvasionGameServerState state)
    {
        await state.Context.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p, state)).ConfigureAwait(false);

        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, state)).ConfigureAwait(false);
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
        await state.Context.ForEachPlayerAsync(p => this.TrySendEndMessageAsync(p, state)).ConfigureAwait(false);

        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, state)).ConfigureAwait(false);
        }

        await state.CleanUpMonstersAsync().ConfigureAwait(false);
        state.Reset();
    }

    /// <summary>
    /// Spawns mobs on the maps that were selected during <see cref="OnPrepareEventAsync"/>.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask SpawnMobsOnMapsAsync(InvasionGameServerState state)
    {
        var config = this.Configuration;
        if (config?.Mobs is not { Count: > 0 } spawns)
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
            else
            {
                // This indicates an unexpected state or configuration mismatch.
            }
        }
    }

    /// <summary>
    /// Iterates the mob configurations and registers the selected spawn maps into the state.
    /// For mobs configured with <see cref="SpawnMapStrategy.AllMaps"/>, all map IDs are registered.
    /// For mobs configured with <see cref="SpawnMapStrategy.RandomMap"/>, a single map is picked at random.
    /// </summary>
    /// <param name="mobs">The mob spawn configurations.</param>
    /// <param name="state">The current invasion state.</param>
    private void SelectSpawnMaps(IList<InvasionSpawnConfiguration> mobs, InvasionGameServerState state)
    {
        foreach (var mob in mobs)
        {
            if (mob.MapIds.Count == 0)
            {
                continue;
            }

            if (mob.IsSpawnOnAllMaps)
            {
                foreach (var mapId in mob.MapIds)
                {
                    state.RegisterMap(mapId);
                }
            }
            else
            {
                var randomMapId = mob.MapIds.Count == 1
                    ? mob.MapIds[0]
                    : mob.MapIds[Rand.NextInt(0, mob.MapIds.Count)];

                state.RegisterMap(randomMapId, mob.MonsterId);
            }
        }
    }

    /// <summary>
    /// Selects the single map used for UI event display and message broadcast from
    /// the maps that were actually selected for spawning during <see cref="SelectSpawnMaps"/>.
    /// When <see cref="EventDisplayMapIds"/> is configured, the display map is restricted
    /// to the intersection of <see cref="EventDisplayMapIds"/> and the selected spawn maps,
    /// ensuring the announced map always has active monsters.
    /// Falls back to the minimum map ID if no intersection exists or no display maps are configured.
    /// </summary>
    /// <param name="state">The current invasion state.</param>
    private void SelectDisplayMap(InvasionGameServerState state)
    {
        if (state.MapIds.Count == 0)
        {
            return;
        }

        if (this.EventDisplayMapIds is { Count: > 0 } displayMaps)
        {
            var eligible = state.MapIds
                .Where(displayMaps.Contains)
                .ToList();

            state.MapId = eligible.Count switch
            {
                0 => state.MapIds.Min(),
                1 => eligible[0],
                _ => eligible[Rand.NextInt(0, eligible.Count)],
            };
        }
        else
        {
            state.MapId = state.MapIds.Min();
        }
    }

    private bool IsPlayerOnRelevantMap(Player player, InvasionGameServerState state)
        => state.MapId.HasValue
           && player.CurrentMap is { } map
           && !player.PlayerState.CurrentState.IsDisconnectedOrFinished()
           && map.MapId == state.MapId.Value
           && (this.EventDisplayMapIds is null || this.EventDisplayMapIds.Contains(state.MapId.Value));

    private async Task TrySendMapEventStateUpdateAsync(Player player, InvasionGameServerState state)
    {
        if (this._mapEventType is null || !this.IsPlayerOnRelevantMap(player, state))
        {
            return;
        }

        try
        {
            var enabled = state.State != PeriodicTaskState.NotStarted;
            await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, this._mapEventType.Value)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(
                ex,
                "Unexpected error sending map event state update, event type: {MapEventType}",
                this._mapEventType);
        }
    }

}
