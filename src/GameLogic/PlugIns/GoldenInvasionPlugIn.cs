// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Describe the state of invasion.
/// </summary>
public enum MobsInvasionState : byte
{
    NotStarted,
    Initialized,
    Started,
}

/// <summary>
/// Abstract configuration for periodic invasions.
/// </summary>
public abstract class PeriodicInvasionConfiguration
{
    public bool IsActive { get; set; } = true;
    public List<TimeOnly> Timetable { get; set; } = new(GenerateTimeSequence(TimeSpan.FromMinutes(1)));
    public TimeSpan EventDuration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan PreStartMessageDelay { get; set; } = TimeSpan.FromSeconds(3);
    public string? Message { get; set; } = "Invasion's been started!";

    /// <summary>
    /// Generate a sequnce of time points like [00:00, 00:01, ...].
    /// </summary>
    /// <param name="duration">The duration.</param>
    public static IEnumerable<TimeOnly> GenerateTimeSequence(TimeSpan duration)
    {
        var limit = TimeSpan.FromDays(1);
        var current = TimeSpan.FromDays(0);

        while (current < limit)
        {
            yield return TimeOnly.FromTimeSpan(current);
            current = current.Add(duration);
        }
    }

    /// <summary>
    /// Check if current time is OK for starting an invasion.
    /// </summary>
    /// <returns>Returns true if the invasion can be started.</returns>
    public bool IsItTimeToStartInvasion()
    {
        if (this.Timetable.Count == 0)
        {
            return true;
        }

        var nowTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        var erlier = nowTime.Add(TimeSpan.FromSeconds(-5));

        // For example, p = 00:00. Check that time between 00:00:00 and 00:00:05
        return this.Timetable.Any(p => p.IsBetween(erlier, nowTime));
    }
}

/// <summary>
/// Configuration for golden invasion.
/// </summary>
public class GoldenInvasionConfiguration : PeriodicInvasionConfiguration
{
    public GoldenInvasionConfiguration()
    {
        this.Message = "[{mapName}] Golden Invasion!";
    }
}

[PlugIn(nameof(GoldenInvasionPlugIn), "Handle Golden Invasion event")]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<GoldenInvasionConfiguration>
{
    private class GameServerState
    {
        public DateTime NextRunUtc { get; set; } = DateTime.UtcNow;

        public MobsInvasionState State { get; set; } = MobsInvasionState.NotStarted;

        public ushort MapId { get; set; } = LorenciaId;

        public readonly IGameContext Context;

        public GameMapDefinition Map => this.Context.Configuration.Maps.First(m => m.Number == this.MapId);

        public string MapName => this.Map.Name;

        public GameServerState(IGameContext ctx)
        {
            this.Context = ctx;
        }
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
    private static readonly ushort GoldenWheelId = 83; // ?
    private static readonly ushort GoldenTantallosId = 82;

    private static readonly Random _random = new();

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
    public GoldenInvasionConfiguration? Configuration { get; set; }

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

        if (!configuration.IsActive)
        {
            logger.LogWarning("plugin is not activated.");
            return;
        }

        switch (state.State)
        {
            case MobsInvasionState.NotStarted:
                {
                    if (!configuration.IsItTimeToStartInvasion())
                    {
                        return;
                    }

                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.PreStartMessageDelay);
                    state.MapId = PossibleMaps[_random.Next(0, PossibleMaps.Length)];
                    state.State = MobsInvasionState.Initialized;

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

            case MobsInvasionState.Initialized:
                {
                    logger.LogInformation($"{state.MapName}: spawning mobs ...");
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.EventDuration);
                    state.State = MobsInvasionState.Started;

                    await this.SpawnGeneralMobsAsync(state).ConfigureAwait(false);
                    await this.SpawnUniqueMobsAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.MapName}: spawning finished");

                    break;
                }

            case MobsInvasionState.Started:
                {
                    logger.LogInformation($"{state.MapName}: event finished");

                    state.State = MobsInvasionState.NotStarted;

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

                var flyingEnabled = state.State != MobsInvasionState.NotStarted;

                await this.TrySendFlyingDragonsAsync(player, flyingEnabled).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
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

                var tasks = Enumerable.Repeat(() => CreateMonsterAsync(gameContext, gameMap, monsterDefinition, 10, 240, 10, 240), mobsCount).Select(c => c());
                await Task.WhenAll(tasks).ConfigureAwait(false);
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

            var tasks = Enumerable.Repeat(() => CreateMonsterAsync(gameContext, gameMap, monsterDefinition, 10, 240, 10, 240), mobsCount).Select(c => c());
            await Task.WhenAll(tasks).ConfigureAwait(false);
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

    private static async Task<Monster> CreateMonsterAsync(IGameContext gameContext, GameMap gameMap, MonsterDefinition monsterDefinition, byte x1, byte x2, byte y1, byte y2)
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

        var intelligence = new BasicMonsterIntelligence();

        var monster = new Monster(area, monsterDefinition, gameMap, gameContext.DropGenerator, intelligence, gameContext.PlugInManager, gameContext.PathFinderPool);

        monster.Initialize();

        await gameMap.AddAsync(monster).ConfigureAwait(false);

        return monster;
    }
}
