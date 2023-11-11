// <copyright file="LevelDependentDamage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a level dependent damage.
/// </summary>
[Cloneable]
public partial class LevelDependentDamage
{
    /// <summary>
    /// Gets or sets the level belonging to this damage value.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the damage belonging to this level.
    /// </summary>
    public int Damage { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Level {this.Level}: {this.Damage}";
    }
}