// <copyright file="MonsterAttributeScalerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="MonsterAttributeScaler"/>.
/// </summary>
public class MonsterAttributeScalerConfiguration
{
    /// <summary>
    /// Gets or sets the percentage by which all monster base stats
    /// (attack rate, defense, defense rate, damage, health) are increased.
    /// </summary>
    public float Percentage { get; set; } = 25.0f;
}
