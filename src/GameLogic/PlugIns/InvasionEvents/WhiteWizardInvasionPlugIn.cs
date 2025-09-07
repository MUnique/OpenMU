// <copyright file="WhiteWizardInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using MUnique.OpenMU.DataModel.Configuration;
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
        Message = "[{mapName}] White Wizard Invasion!",
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

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(InvasionGameServerState state)
    {
        var gameContext = state.Context;
        var cfg = this.Configuration ?? new WhiteWizardInvasionConfiguration();

        // Attach custom drop group to White Wizard, if configured
        if (cfg.UseCustomDrop && cfg.BossDropGroup is { })
        {
            var wizardDef = gameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == WhiteWizardId);
            if (wizardDef is not null && !wizardDef.DropItemGroups.Contains(cfg.BossDropGroup))
            {
                wizardDef.DropItemGroups.Add(cfg.BossDropGroup);
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

        // Determine boss id (fallback if not present)
        var hasWhiteWizard = gameContext.Configuration.Monsters.Any(m => m.Number == WhiteWizardId);
        var bossId = hasWhiteWizard ? WhiteWizardId : (ushort)66; // Cursed King

        foreach (var mapId in maps)
        {
            // Support mobs per map
            var supportMobs = new List<(ushort MonsterId, ushort Count)>();
            if (cfg.OrcArcherPerMap > 0)
            {
                supportMobs.Add((OrcArcherId, cfg.OrcArcherPerMap));
            }

            if (cfg.EliteOrcPerMap > 0)
            {
                supportMobs.Add((EliteOrcId, cfg.EliteOrcPerMap));
            }

            if (supportMobs.Count > 0)
            {
                await this.SpawnMobsAsync(gameContext, mapId, supportMobs).ConfigureAwait(false);
            }

            // Boss per map
            if (cfg.BossCountPerMap > 0)
            {
                var bosses = new (ushort MonsterId, ushort Count)[] { (bossId, cfg.BossCountPerMap) };
                await this.SpawnMobsAsync(gameContext, mapId, bosses).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnFinishedAsync(InvasionGameServerState state)
    {
        // First, finish and cleanup spawned mobs (base triggers Finished event handlers)
        await base.OnFinishedAsync(state).ConfigureAwait(false);

        // Detach custom drop group if we attached it
        if (this._customDropAttached && this._attachedDropGroup is { })
        {
            var wizardDef = state.Context.Configuration.Monsters.FirstOrDefault(m => m.Number == WhiteWizardId);
            if (wizardDef is not null)
            {
                wizardDef.DropItemGroups.Remove(this._attachedDropGroup);
            }

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
}
