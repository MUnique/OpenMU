// <copyright file="WanderingMerchantsConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WanderingMerchants;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Configuration of wandering merchants.
/// </summary>
public class WanderingMerchantsConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets the minimum duration of the spawn.
    /// </summary>
    public TimeSpan MinimumSpawnDuration { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    /// Gets the maximum duration of the spawn.
    /// </summary>
    public TimeSpan MaximumSpawnDuration { get; set; } = TimeSpan.FromMinutes(180);
}