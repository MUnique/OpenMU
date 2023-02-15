// <copyright file="BaseInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for invasion plugins.
/// </summary>
/// <typeparam name="TConfiguration">Configuration.</typeparam>
public abstract class BaseInvasionPlugIn<TConfiguration> : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<TConfiguration>
    where TConfiguration : PeriodicInvasionConfiguration
{
    /// <summary>
    /// Lorencia.
    /// </summary>
    protected const ushort LorenciaId = 0;

    /// <summary>
    /// Devias.
    /// </summary>
    protected const ushort DeviasId = 2;

    /// <summary>
    /// Noria.
    /// </summary>
    protected const ushort NoriaId = 3;

    /// <summary>
    /// Game server state per event.
    /// </summary>
    protected class GameServerState
    {
        public GameServerState(IGameContext ctx)
        {
            this.Context = ctx;
        }

        public IGameContext Context { get; private set; }

        public DateTime NextRunUtc { get; set; } = DateTime.UtcNow;

        public InvasionEventState State { get; set; } = InvasionEventState.NotStarted;

        public ushort MapId { get; set; } = LorenciaId;

        public GameMapDefinition Map => this.Context.Configuration.Maps.First(m => m.Number == this.MapId);

        public string MapName => this.Map.Name;
    }

    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<IGameContext, GameServerState>> _states = new();

    /// <summary>
    /// Gets mobs which spawn on event starting only on the selected map.
    /// </summary>
    private readonly (ushort MonsterId, ushort Count)[] _mobsOnSelectedMap;

    /// <summary>
    /// Gets mobs which spawn on event starting.
    /// </summary>
    private readonly (ushort MapId, ushort MonsterId, ushort Count)[] _mobs;

    private readonly MapEventType? _mapEventType;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseInvasionPlugIn{TConfiguration}" /> class.
    /// </summary>
    /// <param name="mapEventType">Type of the map event.</param>
    /// <param name="mobs">The mobs which spawn on event starting only on the selected map.</param>
    /// <param name="mobsOnSelectedMap">The mobs which always spawn on event starting on the selected map.</param>
    protected BaseInvasionPlugIn(MapEventType? mapEventType, (ushort MapId, ushort MonsterId, ushort Count)[]? mobs, (ushort MonsterId, ushort Count)[] mobsOnSelectedMap)
    {
        this._mapEventType = mapEventType;
        this._mobs = mobs ?? Array.Empty<(ushort MapId, ushort MonsterId, ushort Count)>();
        this._mobsOnSelectedMap = mobsOnSelectedMap;
    }

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public TConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets possible maps for the event.
    /// </summary>
    protected virtual ushort[] PossibleMaps { get; } = { LorenciaId, NoriaId, DeviasId };

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);

        var state = this.GetStateByGameContext(gameContext);

        if (state.NextRunUtc > DateTime.UtcNow)
        {
            return;
        }

        var configuration = this.Configuration;

        if (configuration is null && this is ISupportDefaultCustomConfiguration defaultConfigSupporter)
        {
            logger.LogWarning("configuration is not set. Using default configuration.");
            this.Configuration = configuration = defaultConfigSupporter.CreateDefaultConfig() as TConfiguration;
        }

        if (configuration is null)
        {
            logger.LogError("no configuration available; can't execute invasion plugin.");
            return;
        }

        switch (state.State)
        {
            case InvasionEventState.NotStarted:
                {
                    if (!configuration.IsItTimeToStartInvasion())
                    {
                        return;
                    }

                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.PreStartMessageDelay);
                    state.MapId = this.PossibleMaps[Rand.NextInt(0, this.PossibleMaps.Length)];
                    state.State = InvasionEventState.Prepared;

                    await this.OnPreparedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.MapName}: event initialized");

                    break;
                }

            case InvasionEventState.Prepared:
                {
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.EventDuration);
                    state.State = InvasionEventState.Started;

                    await this.OnStartedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.MapName}: event started");

                    break;
                }

            case InvasionEventState.Started:
                {
                    state.State = InvasionEventState.NotStarted;

                    await this.OnFinishedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.MapName}: event finished");

                    break;
                }

            default:
                throw new NotImplementedException("Unknown state.");
        }
    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public virtual async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        try
        {
            if (this._mapEventType is null)
            {
                return;
            }

            if (addedObject is Player player)
            {
                var state = this.GetStateByGameContext(player.GameContext);
                var isEnabled = state.State != InvasionEventState.NotStarted;
                await this.TrySendMapEventStateUpdateAsync(player, isEnabled).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    /// <summary>
    /// Create a new monster.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="gameMap">The game map.</param>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="x1">The x1.</param>
    /// <param name="x2">The x2.</param>
    /// <param name="y1">The y1.</param>
    /// <param name="y2">The y2.</param>
    /// <param name="quantity">The quantity.</param>
    protected static async ValueTask CreateMonstersAsync(IGameContext gameContext, GameMap gameMap, MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2, ushort quantity)
    {
        var area = new MonsterSpawnArea
        {
            GameMap = gameMap.Definition,
            MonsterDefinition = monsterDefinition,
            SpawnTrigger = SpawnTrigger.Automatic,
            Quantity = 1,
            X1 = x1,
            X2 = x2,
            Y1 = y1,
            Y2 = y2,
        };

        while (quantity-- > 0)
        {
            var intelligence = new BasicMonsterIntelligence();

            var monster = new Monster(area, monsterDefinition, gameMap, gameContext.DropGenerator, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);

            monster.Initialize();

            await gameMap.AddAsync(monster).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawn mobs on the map.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="mapId">The map id.</param>
    /// <param name="mobs">The mobs.</param>
    protected async ValueTask SpawnMobsAsync(IGameContext gameContext, ushort mapId, IEnumerable<(ushort MonsterId, ushort Count)> mobs)
    {
        var gameMap = await gameContext.GetMapAsync(mapId).ConfigureAwait(false);

        if (gameMap is null)
        {
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType());
        foreach (var (mobId, mobsCount) in mobs)
        {
            if (gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == mobId) is { } monsterDefinition)
            {
                await CreateMonstersAsync(gameContext, gameMap, monsterDefinition, 10, 240, 10, 240, mobsCount).ConfigureAwait(false);
            }
            else
            {
                logger.LogDebug("Skipping spawning of monster with number {mobId}, because monster definition wasn't found.", mobId);
            }
        }
    }

    /// <summary>
    /// Get a unique state per GameContext.
    /// </summary>
    /// <param name="gameContext">GameContext.</param>
    protected GameServerState GetStateByGameContext(IGameContext gameContext)
    {
        var type = this.GetType();

        var statesPerType = _states.GetOrAdd(type, newType => new());

        return statesPerType.GetOrAdd(gameContext, gameCtx => new GameServerState(gameContext));
    }

    /// <summary>
    /// Returns true if the player stays on the map.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="checkForCurrentMap">True, if need to check current event map.</param>
    protected bool IsPlayerOnMap(Player player, bool checkForCurrentMap = false)
    {
        var state = this.GetStateByGameContext(player.GameContext);

        return player.CurrentMap is { } map
            && player.PlayerState.CurrentState != PlayerState.Disconnected
            && player.PlayerState.CurrentState != PlayerState.Finished
            && (!checkForCurrentMap || map.MapId == state.MapId);
    }

    /// <summary>
    /// Send a golden message to player's client.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="mapName">The map name.</param>
    protected async Task TrySendStartMessageAsync(Player player, string mapName)
    {
        var configuration = this.Configuration;

        if (configuration is null)
        {
            return;
        }

        var message = (configuration.Message ?? "[{mapName}] Invasion!").Replace("{mapName}", mapName, StringComparison.InvariantCulture);

        if (this.IsPlayerOnMap(player))
        {
            try
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, Interfaces.MessageType.GoldenCenter)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending start message.");
            }
        }
    }

    /// <summary>
    /// Calls after the state changed to Prepared.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask OnPreparedAsync(GameServerState state)
    {
        await state.Context.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p, state.MapName)).ConfigureAwait(false);

        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, true)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Calls after the state changed to Started.
    /// </summary>
    /// <param name="state">State.</param>
    protected virtual async ValueTask OnStartedAsync(GameServerState state)
    {
        await this.SpawnMobsOnSelectedMapAsync(state).ConfigureAwait(false);
        await this.SpawnMobsOnMapsAsync(state).ConfigureAwait(false);
    }

    /// <summary>
    /// Calls after the state changed to Finished.
    /// </summary>
    /// <param name="state">State.</param>
    protected virtual async ValueTask OnFinishedAsync(GameServerState state)
    {
        if (this._mapEventType is not null)
        {
            await state.Context.ForEachPlayerAsync(p => this.TrySendMapEventStateUpdateAsync(p, false)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Spawn mobs on the selected map.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask SpawnMobsOnSelectedMapAsync(GameServerState state)
    {
        var gameContext = state.Context;
        await SpawnMobsAsync(gameContext, state.MapId, this._mobsOnSelectedMap).ConfigureAwait(false);
    }

    /// <summary>
    /// Spawn mobs on the map.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask SpawnMobsOnMapsAsync(GameServerState state)
    {
        var gameContext = state.Context;

        foreach (var group in this._mobs.GroupBy(tuple => tuple.MapId))
        {
            var mapId = group.Key;
            var mobs = group.Select(group => (group.MonsterId, group.Count));

            await SpawnMobsAsync(gameContext, mapId, mobs).ConfigureAwait(false);
        }
    }

    private async Task TrySendMapEventStateUpdateAsync(Player player, bool enabled)
    {
        if (this._mapEventType is null || !this.IsPlayerOnMap(player, true))
        {
            return;
        }

        try
        {
            await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, this._mapEventType.Value)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, $"Unexpected error sending map event state update, event type {this._mapEventType}.");
        }
    }
}
