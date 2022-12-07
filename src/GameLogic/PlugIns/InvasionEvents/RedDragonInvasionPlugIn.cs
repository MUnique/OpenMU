// <copyright file="RedDragonInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Red dRagon Invasion feature.
/// </summary>
[PlugIn(nameof(RedDragonInvasionPlugIn), "Handle red dragon invasion event")]
[Guid("548A76CC-242C-441C-BC9D-6C22745A2D72")]
public class RedDragonInvasionPlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<PeriodicInvasionConfiguration>
{
    private class GameServerState
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

    private const ushort LorenciaId = 0;
    private const ushort DeviasId = 2;
    private const ushort NoriaId = 3;

    private const ushort RedDragonId = 44;

    private static readonly ushort[] PossibleMaps = { LorenciaId, NoriaId, DeviasId };

    private static readonly List<(ushort MonsterId, ushort Count)> GeneralMobsCountOnSelectedMap = new()
    {
         (RedDragonId, 5),
    };

    private static readonly ConcurrentDictionary<IGameContext, GameServerState> _states = new();

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public PeriodicInvasionConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger<RedDragonInvasionPlugIn>();

        var state = GetStateByGameContext(gameContext);

        if (state.NextRunUtc > DateTime.UtcNow)
        {
            return;
        }

        var configuration = this.Configuration;

        if (configuration is null)
        {
            logger.LogWarning("configuration is not set.");
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
                    state.MapId = PossibleMaps[Rand.NextInt(0, PossibleMaps.Length)];
                    state.State = InvasionEventState.Prepared;

                    logger.LogInformation($"{state.MapName}: initialized");

                    try
                    {
                        await gameContext.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p, state.MapName)).ConfigureAwait(false);
                        await gameContext.ForEachPlayerAsync(p => this.TrySendFlyingDragonsAsync(p, true)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.Message, ex.StackTrace);
                    }

                    break;
                }

            case InvasionEventState.Prepared:
                {
                    logger.LogInformation($"{state.MapName}: spawning mobs ...");
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.EventDuration);
                    state.State = InvasionEventState.Started;

                    await this.SpawnGeneralMobsAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.MapName}: spawning finished");

                    break;
                }

            case InvasionEventState.Started:
                {
                    logger.LogInformation($"{state.MapName}: event finished");

                    state.State = InvasionEventState.NotStarted;

                    try
                    {
                        await gameContext.ForEachPlayerAsync(p => this.TrySendFlyingDragonsAsync(p, false)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.Message, ex.StackTrace);
                    }

                    break;
                }

            default:
                throw new NotImplementedException("Unknown state.");
        }
    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        try
        {
            if (addedObject is Player player)
            {
                var state = GetStateByGameContext(player.GameContext);

                var flyingEnabled = state.State != InvasionEventState.NotStarted;

                await this.TrySendFlyingDragonsAsync(player, flyingEnabled).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    private static async ValueTask CreateMonstersAsync(IGameContext gameContext, GameMap gameMap, MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2, ushort quantity)
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

    private static GameServerState GetStateByGameContext(IGameContext gameContext)
    {
        return _states.GetOrAdd(gameContext, gameCtx => new GameServerState(gameContext));
    }

    private async ValueTask SpawnGeneralMobsAsync(GameServerState state)
    {
        var gameContext = state.Context;

        var gameMap = await gameContext.GetMapAsync(state.MapId).ConfigureAwait(false);

        if (gameMap is null)
        {
            return;
        }

        foreach (var (mobId, mobsCount) in GeneralMobsCountOnSelectedMap)
        {
            var monsterDefinition = gameContext.Configuration.Monsters.First(m => m.Number == mobId);
            await CreateMonstersAsync(gameContext, gameMap, monsterDefinition, 10, 240, 10, 240, mobsCount).ConfigureAwait(false);
        }
    }

    private async Task TrySendStartMessageAsync(Player player, string mapName)
    {
        var configuration = this.Configuration;

        if (configuration is null)
        {
            return;
        }

        var message = (configuration.Message ?? "[{mapName}] Red Dragon Invasion!").Replace("{mapName}", mapName, StringComparison.InvariantCulture);

        if (player.CurrentMap is { } map
           && player.PlayerState.CurrentState != PlayerState.Disconnected
           && player.PlayerState.CurrentState != PlayerState.Finished)
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

    private async Task TrySendFlyingDragonsAsync(Player player, bool enabled)
    {
        var state = GetStateByGameContext(player.GameContext);

        if (player.CurrentMap is { } map
            && player.PlayerState.CurrentState != PlayerState.Disconnected
            && player.PlayerState.CurrentState != PlayerState.Finished
            && map.MapId == state.MapId)
        {
            try
            {
                await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, MapEventType.RedDragonInvasion)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending flying dragons update.");
            }
        }
    }
}
