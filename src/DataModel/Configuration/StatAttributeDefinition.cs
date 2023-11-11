// <copyright file="StatAttributeDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Defines a stat attribute, which may be increasable by the player.
/// </summary>
[Cloneable]
public partial class StatAttributeDefinition
{
    private AttributeDefinition? attribute = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatAttributeDefinition"/> class.
    /// </summary>
    public StatAttributeDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatAttributeDefinition" /> class.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="baseValue">The base value.</param>
    /// <param name="increasableByPlayer">if set to <c>true</c> it is increasable by the player.</param>
    public StatAttributeDefinition(AttributeDefinition attribute, float baseValue, bool increasableByPlayer)
    {
        this.attribute = attribute;
        this.BaseValue = baseValue;
        this.IncreasableByPlayer = increasableByPlayer;
    }

    /// <summary>
    /// Gets or sets the attribute definition of this stat attribute.
    /// </summary>
#pragma warning disable S2292 // When this would be an auto property, it would lead to a virtual member call in the constructor.
    public virtual AttributeDefinition? Attribute
#pragma warning restore S2292
    {
        get { return this.attribute; }
        set { this.attribute = value; }
    }

    /// <summary>
    /// Gets or sets the base value, which is the initial value without an increase of the player.
    /// </summary>
    public float BaseValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this stat is increasable by the player in any way.
    /// </summary>
    public bool IncreasableByPlayer { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Attribute}: {this.BaseValue}";
    }
}