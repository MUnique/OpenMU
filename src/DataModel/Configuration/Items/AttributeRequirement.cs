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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Attribute}: {this.MinimumValue}";
    }
}