// <copyright file="CastleSiegeUpgradeDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines one level of an upgrade that the castle owner can apply to a gate or statue NPC.
/// </summary>
[Cloneable]
public partial class CastleSiegeUpgradeDefinition
{
    /// <summary>
    /// Gets or sets the upgrade level (0–3), where 0 represents the base/unupgraded state.
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    /// Gets or sets the number of Jewels of Guardian required to perform this upgrade.
    /// </summary>
    public int RequiredJewelOfGuardianCount { get; set; }

    /// <summary>
    /// Gets or sets the amount of Zen required to perform this upgrade.
    /// </summary>
    public int RequiredZen { get; set; }

    /// <summary>
    /// Gets or sets the resulting stat value granted by this upgrade level (defense or max HP).
    /// </summary>
    public int Value { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Level {this.Level}: Value={this.Value}, Jewels={this.RequiredJewelOfGuardianCount}, Zen={this.RequiredZen}";
    }
}
