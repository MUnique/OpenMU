// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
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
/// This plugin enables Golden Invasion feature.
/// </summary>
[PlugIn(nameof(GoldenInvasionPlugIn), "Handle Golden Invasion event")]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<PeriodicInvasionConfiguration>
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

    private static readonly ushort LorenciaId = 0;
    private static readonly ushort DeviasId = 2;
    private static readonly ushort NoriaId = 3;
    private static readonly ushort AtlansId = 7;
    private static readonly ushort TarkanId = 8;

    private static readonly ushort GoldenBudgeDragonId = 43;
    private static readonly ushort GoldenGoblinId = 78;
    private static readonly ushort GoldenSoldierId = 54;
    private static readonly ushort GoldenTitanId = 53;
    private static readonly ushort GoldenDragonId = 79;
    private static readonly ushort GoldenVeparId = 81;
    private static readonly ushort GoldenLizardKingId = 80;
    private static readonly ushort GoldenWheelId = 83;
    private static readonly ushort GoldenTantallosId = 82;

    private static readonly ushort[] PossibleMaps = new[] { LorenciaId, NoriaId, DeviasId };

    private static readonly Dictionary<ushort, ushort> GeneralMobsCountOnSelectedMap = new()
    {
         { GoldenDragonId, 10 },
    };

    private static readonly Dictionary<ushort, Dictionary<ushort, ushort>> UniqueMobsCountByMapId = new()
    {
        {
            LorenciaId,
            new()
            {
                { GoldenBudgeDragonId, 20 },
            }
        },
        {
            NoriaId,
            new()
            {
                { GoldenGoblinId, 20 },
            }
        },
        {
            DeviasId,
            new()
            {
                { GoldenSoldierId, 20 },
                { GoldenTitanId, 10 },
            }
        },
        {
            AtlansId,
            new()
            {
                { GoldenVeparId, 20 },
                { GoldenLizardKingId, 10 },
            }
        },
        {
            TarkanId,
            new()
            {
                { GoldenWheelId, 20 },
                { GoldenTantallosId, 10 },
            }
        },
    };

    private static readonly ConcurrentDictionary<IGameContext, GameServerState> _states = new();

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public PeriodicInvasionConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger<GoldenInvasionPlugIn>();

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
                    await this.SpawnUniqueMobsAsync(state).ConfigureAwait(false);

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

    private static async Task CreateMonstersAsync(IGameContext gameContext, GameMap gameMap, MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2, ushort quantity)
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

    private async Task SpawnUniqueMobsAsync(GameServerState state)
    {
        var gameContext = state.Context;

        foreach (var (mapId, mobs) in UniqueMobsCountByMapId)
        {
            var gameMap = await gameContext.GetMapAsync(mapId).ConfigureAwait(false);

            if (gameMap is null)
            {
                continue;
            }

            foreach (var (mobId, mobsCount) in mobs)
            {
                var monsterDefinition = gameContext.Configuration.Monsters.First(m => m.Number == mobId);
                await CreateMonstersAsync(gameContext, gameMap, monsterDefinition, 10, 240, 10, 240, mobsCount).ConfigureAwait(false);
            }
        }
    }

    private async Task SpawnGeneralMobsAsync(GameServerState state)
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

        var message = (configuration.Message ?? "[{mapName}] Golden Invasion!").Replace("{mapName}", mapName, StringComparison.InvariantCulture);

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
                await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, MapEventType.GoldenDragonInvasion)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending flying dragons update.");
            }
        }
    }
}
