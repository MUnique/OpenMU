// <copyright file="PowerUpDefinitionValue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Attributes;

using System.ComponentModel;
using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The power up definition value which can consist of a constant value and several related values which are all added together to get the result.
/// </summary>
[Cloneable]
public partial class PowerUpDefinitionValue
{
    /// <summary>
    /// Gets or sets the constant value part of the value.
    /// </summary>
    [MemberOfAggregate]
    [Browsable(false)]
    public SimpleElement ConstantValue { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the related values.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<AttributeRelationship> RelatedValues { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.ConstantValue?.Value ?? 0} + {string.Join(" + ", this.RelatedValues.Select(v => $"({v})"))}";
    }
}