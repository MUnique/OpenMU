﻿// <copyright file="AttributeRelationship.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

using System.Globalization;

/// <summary>
/// The operator which is applied between the input attribute and the input operand.
/// </summary>
public enum InputOperator
{
    /// <summary>
    /// The <see cref="AttributeRelationship.InputAttribute"/> is multiplied with the <see cref="AttributeRelationship.InputOperand"/> before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    Multiply,

    /// <summary>
    /// The <see cref="AttributeRelationship.InputAttribute"/> is increased by the <see cref="AttributeRelationship.InputOperand"/> before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    Add,

    /// <summary>
    /// The <see cref="AttributeRelationship.InputAttribute"/> is exponentiated by the <see cref="AttributeRelationship.InputOperand"/> before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    Exponentiate,

    /// <summary>
    /// The <see cref="AttributeRelationship.InputOperand"/> is exponentiated by the <see cref="AttributeRelationship.InputAttribute"/> before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    ExponentiateByAttribute,

    /// <summary>
    /// The minimum between <see cref="AttributeRelationship.InputAttribute"/> and <see cref="AttributeRelationship.InputOperand"/> is taken before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    Minimum,

    /// <summary>
    /// The maximum between <see cref="AttributeRelationship.InputAttribute"/> and <see cref="AttributeRelationship.InputOperand"/> is taken before effecting the <see cref="AttributeRelationship.TargetAttribute"/>.
    /// </summary>
    Maximum,
}

/// <summary>
/// Describes a relationship between two attributes.
/// </summary>
public class AttributeRelationship
{
    private AttributeDefinition? _targetAttribute;
    private AttributeDefinition? _inputAttribute;
    private AttributeDefinition? _operandAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeRelationship"/> class.
    /// </summary>
    public AttributeRelationship()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeRelationship" /> class.
    /// </summary>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="inputOperand">The multiplier.</param>
    /// <param name="inputAttribute">The input attribute.</param>
    /// <param name="aggregateType">The type of the aggregate on the <paramref name="targetAttribute"/>.</param>
    /// <param name="stage">The calculation stage on the <paramref name="targetAttribute"/>.</param>
    public AttributeRelationship(AttributeDefinition targetAttribute, float inputOperand, AttributeDefinition inputAttribute, AggregateType aggregateType, byte stage)
        : this(targetAttribute, inputOperand, inputAttribute, InputOperator.Multiply, default, aggregateType, stage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeRelationship" /> class.
    /// </summary>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="inputOperand">The multiplier.</param>
    /// <param name="inputAttribute">The input attribute.</param>
    /// <param name="aggregateType">The type of the aggregate on the <paramref name="targetAttribute"/>.</param>
    /// <param name="stage">The calculation stage on the <paramref name="targetAttribute"/>.</param>
    public AttributeRelationship(AttributeDefinition targetAttribute, AttributeDefinition inputOperand, AttributeDefinition inputAttribute, AggregateType aggregateType, byte stage)
        : this(targetAttribute, 1, inputAttribute, InputOperator.Multiply, inputOperand, aggregateType, stage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeRelationship" /> class.
    /// </summary>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="inputOperand">The multiplier.</param>
    /// <param name="inputAttribute">The input attribute.</param>
    /// <param name="inputOperator">The input operator.</param>
    /// <param name="operandAttribute">The operand attribute.</param>
    /// <param name="aggregateType">The type of the aggregate on the <paramref name="targetAttribute"/>.</param>
    /// <param name="stage">The calculation stage on the <paramref name="targetAttribute"/>.</param>
    public AttributeRelationship(AttributeDefinition targetAttribute, float inputOperand, AttributeDefinition inputAttribute, InputOperator inputOperator, AttributeDefinition? operandAttribute = null, AggregateType aggregateType = AggregateType.AddRaw, byte stage = 0)
    {
        this.InputOperand = inputOperand;
        this.InputOperator = inputOperator;
        this.AggregateType = aggregateType;
        this.Stage = stage;
        this._targetAttribute = targetAttribute;
        this._inputAttribute = inputAttribute;
        this._operandAttribute = operandAttribute;
    }

    /// <summary>
    /// Gets or sets the target attribute which will be affected.
    /// </summary>
    public virtual AttributeDefinition? TargetAttribute
    {
        get => this._targetAttribute;
        set => this._targetAttribute = value;
    }

    /// <summary>
    /// Gets or sets the input attribute which provides the input value.
    /// </summary>
    public virtual AttributeDefinition? InputAttribute
    {
        get => this._inputAttribute;
        set => this._inputAttribute = value;
    }

    /// <summary>
    /// Gets or sets the operand attribute which replaces the <see cref="InputOperand"/>, if set.
    /// </summary>
    public virtual AttributeDefinition? OperandAttribute
    {
        get => this._operandAttribute;
        set => this._operandAttribute = value;
    }

    /// <summary>
    /// Gets or sets the operator which is applied between the input attribute and the input operand.
    /// </summary>
    public InputOperator InputOperator { get; set; }

    /// <summary>
    /// Gets or sets the operand which is applied to the input attribute before adding to the target attribute.
    /// Has only effect, when <see cref="OperandAttribute"/> is <see langword="null"/>.
    /// </summary>
    public float InputOperand { get; set; }

    /// <summary>
    /// Gets or sets the aggregate type with which the relationship should effect the target attribute.
    /// </summary>
    public AggregateType AggregateType { get; set; }

    /// <summary>
    /// Gets or sets the calculation stage at which the relationship should effect the target attribute.
    /// </summary>
    public byte Stage { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (this.TargetAttribute is null)
        {
            return $"{this.InputAttribute} {this.InputOperator.AsString()} {this.OperandAttribute?.ToString() ?? this.InputOperand.ToString(CultureInfo.InvariantCulture)}";
        }

        if (this.InputOperator == InputOperator.ExponentiateByAttribute)
        {
            return $"{this.TargetAttribute} {(this.AggregateType == AggregateType.Multiplicate ? "*" : "+")}= {this.OperandAttribute?.ToString() ?? this.InputOperand.ToString(CultureInfo.InvariantCulture)} {this.InputOperator.AsString()} {this.InputAttribute}";
        }

        return $"{this.TargetAttribute} {(this.AggregateType == AggregateType.Multiplicate ? "*" : "+")}= {this.InputAttribute} {this.InputOperator.AsString()} {this.OperandAttribute?.ToString() ?? this.InputOperand.ToString(CultureInfo.InvariantCulture)}";
    }
}