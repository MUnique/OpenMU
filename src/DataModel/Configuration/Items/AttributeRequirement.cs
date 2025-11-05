// <copyright file="AttributeRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Defines a requirement of an attribute with the specified value.
/// </summary>
[Cloneable]
public partial class AttributeRequirement
{
    /// <summary>
    /// Gets or sets the attribute which is required.
    /// </summary>
    [Required]
    public virtual AttributeDefinition? Attribute { get; set; }

    /// <summary>
    /// Gets or sets the minimum value the attribute needs to have.
    /// </summary>
    public int MinimumValue { get; set; }

    /// <summary>
    /// Gets or sets the additional requirement per item level.
    /// For example, if MinimumValue is 20 and MinimumValuePerItemLevel is 5,
    /// then level 0 requires 20, level 1 requires 25, level 2 requires 30, etc.
    /// </summary>
    public int MinimumValuePerItemLevel { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.MinimumValuePerItemLevel > 0)
        {
            return $"{this.Attribute}: {this.MinimumValue} (+{this.MinimumValuePerItemLevel}/level)";
        }

        return $"{this.Attribute}: {this.MinimumValue}";
    }
}