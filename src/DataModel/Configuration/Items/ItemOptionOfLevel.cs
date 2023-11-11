// <copyright file="ItemOptionOfLevel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Attributes;

/// <summary>
/// The item option, depending on the specified item level.
/// </summary>
[Cloneable]
public partial class ItemOptionOfLevel
{
    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the required item level.
    /// </summary>
    public int RequiredItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the power up definition.
    /// </summary>
    [MemberOfAggregate]
    public virtual PowerUpDefinition? PowerUpDefinition { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Level {this.Level}: {this.PowerUpDefinition}";
    }
}