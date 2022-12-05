// <copyright file="GoldenInvasionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

/// <summary>
/// Configuration for golden invasion.
/// </summary>
public class GoldenInvasionConfiguration : PeriodicInvasionConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenInvasionConfiguration"/> class.
    /// </summary>
    public GoldenInvasionConfiguration()
    {
        this.Message = "[{mapName}] Golden Invasion!";
    }
}
