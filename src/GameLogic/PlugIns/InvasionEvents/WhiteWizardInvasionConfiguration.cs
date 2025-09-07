// <copyright file="WhiteWizardInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Custom configuration for the White Wizard invasion plugin.
/// </summary>
public class WhiteWizardInvasionConfiguration : PeriodicInvasionConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether the invasion should spawn on all configured maps.
    /// If false, spawns only on a single selected map (random from <see cref="Maps"/>).
    /// </summary>
    public bool SpawnOnAllMaps { get; set; }

    /// <summary>
    /// Gets or sets the maps where the invasion can take place.
    /// Defaults to Lorencia (0), Noria (3) and Devias (2).
    /// </summary>
    public ushort[] Maps { get; set; } = new ushort[] { 0, 3, 2 };

    /// <summary>
    /// Gets or sets the number of White Wizard bosses to spawn per map.
    /// </summary>
    public ushort BossCountPerMap { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of Orc Archers to spawn per map.
    /// </summary>
    public ushort OrcArcherPerMap { get; set; } = 20;

    /// <summary>
    /// Gets or sets the number of Elite Orcs to spawn per map.
    /// </summary>
    public ushort EliteOrcPerMap { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether to use the <see cref="BossDropGroup"/> as custom drop during the invasion.
    /// If enabled, the drop group is temporarily attached to the White Wizard monster definition for the duration of the event.
    /// </summary>
    public bool UseCustomDrop { get; set; }

    /// <summary>
    /// Gets or sets the custom drop group which should be attached to the White Wizard during the invasion.
    /// </summary>
    public DropItemGroup? BossDropGroup { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use <see cref="SupportDropGroup"/> for support mobs (orcs).
    /// </summary>
    public bool UseSupportCustomDrop { get; set; }

    /// <summary>
    /// Gets or sets the custom drop group for support mobs (Orc Archer / Elite Orc) during the invasion.
    /// </summary>
    public DropItemGroup? SupportDropGroup { get; set; }
}
