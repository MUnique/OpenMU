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
    /// Gets or sets the monster spawns for this invasion.
    /// </summary>
    [Display(Name = "Monster Spawns", Order = 5)]
    public IList<InvasionSpawnConfiguration> Mobs { get; set; } = [];
}