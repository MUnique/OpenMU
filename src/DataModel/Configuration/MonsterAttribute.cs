// <copyright file="MonsterAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The attribute and value of a monster.
/// </summary>
/// <remarks>
/// Just needed for entity framework, because it does not support the mapping of dictionaries. May be removed in the future.
/// </remarks>
[Cloneable]
public partial class MonsterAttribute
{
    /// <summary>
    /// Gets or sets the attribute definition.
    /// </summary>
    public virtual AttributeDefinition? AttributeDefinition { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public float Value { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.AttributeDefinition}: {this.Value}";
    }
}