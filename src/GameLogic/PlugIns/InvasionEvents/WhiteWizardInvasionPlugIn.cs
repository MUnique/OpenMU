// <copyright file="WhiteWizardInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables a simplified White Wizard Invasion feature.
/// It spawns orc mobs on Lorencia, Noria and Devias and a boss on the selected map.
/// Note: The MapEventType has no dedicated entry for White Wizard, so no map-effect is shown.
/// </summary>
[PlugIn(nameof(WhiteWizardInvasionPlugIn), "Handle White Wizard invasion event")]
[Guid("FC0C9D75-1C9C-4B5A-8B9C-7C1B9B1B9E77")]
public class WhiteWizardInvasionPlugIn : BaseInvasionPlugIn<WhiteWizardInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    // Known classic mob ids in this data set
    // From DevilSquare initializers:
    private const ushort OrcArcherId = 64;
    private const ushort EliteOrcId = 65;

    // White Wizard id from the client data (NpcName_Eng.txt): 135
    private const ushort WhiteWizardId = 135;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteWizardInvasionPlugIn"/> class.
    /// </summary>
    public WhiteWizardInvasionPlugIn()
        : base(
            mapEventType: null, // MapEventType currently supports Red/Golden dragon only
            mobs: System.Array.Empty<(ushort MapId, ushort MonsterId, ushort Count)>(),
            mobsOnSelectedMap: System.Array.Empty<(ushort MonsterId, ushort Count)>())
    {
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new WhiteWizardInvasionConfiguration
    {
        // Typical White Wizard cadence can vary; default to every 4 hours.
        TaskDuration = TimeSpan.FromMinutes(10),
        PreStartMessageDelay = TimeSpan.FromSeconds(3),
        Message = "[{mapName}] ¡Invasión de White Wizard!",
        Timetable = WhiteWizardInvasionConfiguration.GenerateTimeSequence(TimeSpan.FromHours(4)).ToList(),
    };

    /// <inheritdoc />
    protected override async ValueTask SpawnMobsOnSelectedMapAsync(InvasionGameServerState state)
    {
        // Spawning is controlled in OnStartedAsync according to configuration.
        // We override OnStartedAsync, so this method is not used.
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets possible maps for selection (used for random map selection in base preparation).
    /// </summary>
    protected override ushort[] PossibleMaps
        => this.Configuration?.Maps?.Length > 0
            ? this.Configuration.Maps
            : new ushort[] { LorenciaId, NoriaId, DeviasId };

    private bool _customDropAttached;
    private DropItemGroup? _attachedDropGroup;
    private bool _supportDropAttached;
    private DropItemGroup? _attachedSupportDropGroup;
    private ushort _currentBossId = WhiteWizardId;

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(InvasionGameServerState state)
    {
        var gameContext = state.Context;
        var cfg = this.Configuration ?? new WhiteWizardInvasionConfiguration();

        // Determine boss id (fallback if not present) so we can attach drops to the correct monster.
        var hasWhiteWizard = gameContext.Configuration.Monsters.Any(m => m.Number == WhiteWizardId);
        this._currentBossId = hasWhiteWizard ? WhiteWizardId : (ushort)66; // Cursed King

        // Attach custom drop group to the invasion boss, if configured
        if (cfg.UseCustomDrop && cfg.BossDropGroup is { })
        {
            var bossDef = gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == this._currentBossId);
            if (bossDef is not null && !bossDef.DropItemGroups.Contains(cfg.BossDropGroup))
            {
                bossDef.DropItemGroups.Add(cfg.BossDropGroup);
                this._attachedDropGroup = cfg.BossDropGroup;
                this._customDropAttached = true;
            }
        }

        // Attach custom drop group to support mobs if configured
        if (cfg.UseSupportCustomDrop && cfg.SupportDropGroup is { })
        {
            var orcArcher = gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == OrcArcherId);
            var eliteOrc = gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == EliteOrcId);
            if (orcArcher is not null && !orcArcher.DropItemGroups.Contains(cfg.SupportDropGroup))
            {
                orcArcher.DropItemGroups.Add(cfg.SupportDropGroup);
                this._supportDropAttached = true;
                this._attachedSupportDropGroup = cfg.SupportDropGroup;
            }

            if (eliteOrc is not null && !eliteOrc.DropItemGroups.Contains(cfg.SupportDropGroup))
            {
                eliteOrc.DropItemGroups.Add(cfg.SupportDropGroup);
                this._supportDropAttached = true;
                this._attachedSupportDropGroup = cfg.SupportDropGroup;
            }
        }

        // Determine maps to spawn on
        var maps = cfg.SpawnOnAllMaps ? (cfg.Maps?.Length > 0 ? cfg.Maps : this.PossibleMaps) : new[] { state.MapId };

        var bossId = this._currentBossId;

        foreach (var mapId in maps)
        {
            var gameMap = await gameContext.GetMapAsync(mapId).ConfigureAwait(false);
            if (gameMap is null)
            {
                continue;
            }

            var (centerX, centerY) = this.FindClusterCenter(gameMap);
            var radius = cfg.ClusterRadius;

            // Support mobs per map, clustered around the center
            if (cfg.OrcArcherPerMap > 0)
            {
                await this.SpawnClusterAsync(gameContext, gameMap, OrcArcherId, cfg.OrcArcherPerMap, centerX, centerY, radius).ConfigureAwait(false);
            }

            if (cfg.EliteOrcPerMap > 0)
            {
                await this.SpawnClusterAsync(gameContext, gameMap, EliteOrcId, cfg.EliteOrcPerMap, centerX, centerY, radius).ConfigureAwait(false);
            }

            // Boss per map, clustered and with kill announce
            if (cfg.BossCountPerMap > 0)
            {
                var currentMapName = gameMap.Definition.Name;
                await this.SpawnBossClusterAsync(state, gameContext, currentMapName, gameMap, bossId, cfg.BossCountPerMap, centerX, centerY, radius).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnFinishedAsync(InvasionGameServerState state)
    {
        var cfg = this.Configuration ?? new WhiteWizardInvasionConfiguration();

        // Optional delay before cleanup to let players see the result briefly
        if (cfg.CleanupDelaySeconds > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(cfg.CleanupDelaySeconds)).ConfigureAwait(false);
        }

        // Then, finish and cleanup spawned mobs (base triggers Finished event handlers)
        await base.OnFinishedAsync(state).ConfigureAwait(false);

        // Detach custom drop group if we attached it
        if (this._customDropAttached && this._attachedDropGroup is { })
        {
            var bossDef = state.Context.Configuration.Monsters.FirstOrDefault(m => m.Number == this._currentBossId);
            bossDef?.DropItemGroups.Remove(this._attachedDropGroup);

            this._attachedDropGroup = null;
            this._customDropAttached = false;
        }

        if (this._supportDropAttached && this._attachedSupportDropGroup is { })
        {
            var orcArcher = state.Context.Configuration.Monsters.FirstOrDefault(m => m.Number == OrcArcherId);
            var eliteOrc = state.Context.Configuration.Monsters.FirstOrDefault(m => m.Number == EliteOrcId);
            orcArcher?.DropItemGroups.Remove(this._attachedSupportDropGroup);
            eliteOrc?.DropItemGroups.Remove(this._attachedSupportDropGroup);

            this._attachedSupportDropGroup = null;
            this._supportDropAttached = false;
        }
    }

    private (byte X, byte Y) FindClusterCenter(GameMap gameMap)
    {
        // Try to find a random walkable, non-safezone center
        for (int i = 0; i < 50; i++)
        {
            var x = (byte)Rand.NextInt(10, 246);
            var y = (byte)Rand.NextInt(10, 246);
            if (gameMap.Terrain.WalkMap[x, y] && !gameMap.Terrain.SafezoneMap[x, y])
            {
                return (x, y);
            }
        }

        // Fallback to safezone spawn gate center or (128, 128)
        var gate = gameMap.SafeZoneSpawnGate;
        if (gate is { })
        {
            return ((byte)gate.X1, (byte)gate.Y1);
        }

        return (128, 128);
    }

    private async ValueTask SpawnClusterAsync(IGameContext context, GameMap gameMap, ushort monsterId, ushort count, byte centerX, byte centerY, byte radius)
    {
        if (context.Configuration.Monsters.FirstOrDefault(m => m.Number == monsterId) is not { } monsterDefinition)
        {
            return;
        }

        var x1 = (byte)Math.Max(0, centerX - radius);
        var x2 = (byte)Math.Min(255, centerX + radius);
        var y1 = (byte)Math.Max(0, centerY - radius);
        var y2 = (byte)Math.Min(255, centerY + radius);

        await this.CreateMonstersAsync(context, gameMap, monsterDefinition, x1, x2, y1, y2, count).ConfigureAwait(false);
    }

    private async ValueTask SpawnBossClusterAsync(InvasionGameServerState state, IGameContext context, string mapName, GameMap gameMap, ushort bossId, ushort count, byte centerX, byte centerY, byte radius)
    {
        if (context.Configuration.Monsters.FirstOrDefault(m => m.Number == bossId) is not { } monsterDefinition)
        {
            return;
        }

        state.ActiveBossCount += count;

        var x1 = (byte)Math.Max(0, centerX - radius);
        var x2 = (byte)Math.Min(255, centerX + radius);
        var y1 = (byte)Math.Max(0, centerY - radius);
        var y2 = (byte)Math.Min(255, centerY + radius);

        var area = new MonsterSpawnArea
        {
            GameMap = gameMap.Definition,
            MonsterDefinition = monsterDefinition,
            SpawnTrigger = SpawnTrigger.OnceAtEventStart,
            Quantity = 1,
            X1 = x1,
            X2 = x2,
            Y1 = y1,
            Y2 = y2,
        };

        while (count-- > 0)
        {
            var intelligence = new BasicMonsterIntelligence();
            var monster = new Monster(area, monsterDefinition, gameMap, context.DropGenerator, intelligence, context.PlugInManager, context.PathFinderPool);
            monster.Initialize();
            await gameMap.AddAsync(monster).ConfigureAwait(false);
            monster.OnSpawn();

            // Cleanup hook
            this.Finished += CleanUpOnFinish;
            monster.Died += (_, _) => this.Finished -= CleanUpOnFinish;

            // Kill announce hook
            var configSnapshot = this.Configuration ?? new WhiteWizardInvasionConfiguration();
            monster.Died += (_, death) =>
            {
                var killer = string.IsNullOrWhiteSpace(death.KillerName) ? "Un jugador" : death.KillerName;
                var message = $"{killer} mató al White Wizard en {mapName}.";
                _ = context.SendGlobalMessageAsync(message, Interfaces.MessageType.GoldenCenter);

                // If this was the last boss, finish the invasion early.
                state.ActiveBossCount--;
                if (configSnapshot.FinishWhenAllBossesDead && state.ActiveBossCount <= 0)
                {
                    state.NextRunUtc = DateTime.UtcNow; // triggers early finish in next periodic tick
                    var endMsg = $"La invasión del White Wizard ha finalizado en {mapName}.";
                    _ = context.SendGlobalMessageAsync(endMsg, Interfaces.MessageType.GoldenCenter);
                }
            };

#pragma warning disable VSTHRD100
            async void CleanUpOnFinish(object? sender, EventArgs e)
#pragma warning restore VSTHRD100
            {
                try
                {
                    this.Finished -= CleanUpOnFinish;
                    if (monster is not null && !monster.IsDisposed)
                    {
                        await monster.CurrentMap.RemoveAsync(monster).ConfigureAwait(false);
                        monster.Dispose();
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
