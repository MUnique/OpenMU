// <copyright file="LevelBonus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Defines a constant bonus, depending on item level.
/// </summary>
[Cloneable]
public partial class LevelBonus
{
    private float _additionalValue;
    private ConstantElement? _additionalValueElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelBonus"/> class.
    /// </summary>
    public LevelBonus()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LevelBonus"/> class.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="constantValue">The constant value.</param>
    public LevelBonus(int level, float constantValue)
    {
        this.Level = level;
        this.AdditionalValue = constantValue;
    }

    /// <summary>
    /// Gets or sets the level of the item.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the additional value to the base value.
    /// </summary>
    public float AdditionalValue
    {
        get => this._additionalValue;
        set
        {
            this._additionalValue = value;
            this._additionalValueElement = null;
        }
    }

    /// <summary>
    /// Gets the element which represents the <see cref="AdditionalValue"/>.
    /// </summary>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <returns>The element which represents the <see cref="AdditionalValue"/>.</returns>
    public IElement GetAdditionalValueElement(AggregateType aggregateType)
    {
        return this._additionalValueElement ??= new ConstantElement(this.AdditionalValue, aggregateType);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Level: {this.Level}: {this.AdditionalValue}";
    }
}