// <copyright file="PeriodicInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Configuration data for a periodic invasion event.
/// Responsible only for describing the shape of the configuration (SRP).
/// Default values live in <see cref="InvasionConfigurationDefaults"/>.
/// </summary>
public class PeriodicInvasionConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PeriodicInvasionConfiguration"/> class.
    /// </summary>
    public PeriodicInvasionConfiguration()
    {
        this.StartMessage = "Invasion has started!";
        this.EndMessage = "Invasion has ended!";
    }

    /// <summary>
    /// Gets or sets a value indicating whether all mobs should spawn on a single map,
    /// determined from the first mob's <see cref="InvasionSpawnConfiguration.MapIds"/>.
    /// When <c>true</c>, every mob in the invasion is placed on the same randomly-selected map
    /// rather than distributing across maps per their individual configurations.
    /// </summary>
    [Display(Name = "Force Single Map", Order = 6)]
    public bool ForceSingleMap { get; set; }

    /// <summary>
    /// Gets or sets the monster spawns for this invasion.
    /// </summary>
    [Display(Name = "Monster Spawns", Order = 7)]
    public IList<InvasionSpawnConfiguration> Mobs { get; set; } = [];
}