// <copyright file="ItemBasePowerUpDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Defines an item base power up definition.
/// </summary>
[Cloneable]
public partial class ItemBasePowerUpDefinition
{
    private ConstantElement? _baseValueElement;
    private float _baseValue;
    private AggregateType _aggregateType;

    /// <summary>
    /// Gets or sets the target attribute.
    /// </summary>
    public virtual AttributeDefinition? TargetAttribute { get; set; }

    /// <summary>
    /// Gets or sets the bonus per level.
    /// </summary>
    public virtual ItemLevelBonusTable? BonusPerLevelTable { get; set; }

    /// <summary>
    /// Gets or sets the base value.
    /// </summary>
    public float BaseValue
    {
        get => this._baseValue;
        set
        {
            this._baseValue = value;
            this._baseValueElement = null;
        }
    }

    /// <summary>
    /// Gets or sets the type of the aggregate.
    /// </summary>
    public AggregateType AggregateType
    {
        get => this._aggregateType;
        set
        {
            this._aggregateType = value;
            this._baseValueElement = null;
        }
    }

    /// <summary>
    /// Gets the durability adjusted element for <see cref="BaseValue"/>.
    /// </summary>
    /// <param name="durabilityFactor">The durability factor.</param>
    /// <returns>The durability adjusted element for <see cref="BaseValue"/>.</returns>
    public ConstantElement GetBaseValueElement(float durabilityFactor = 1)
    {
        if (durabilityFactor == 1)
        {
            return this._baseValueElement ??= new ConstantElement(this.BaseValue, this.AggregateType);
        }

        return new ConstantElement(this.BaseValue * durabilityFactor, this.AggregateType);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.BaseValue} {this.TargetAttribute} {this.AggregateType}";
    }
}