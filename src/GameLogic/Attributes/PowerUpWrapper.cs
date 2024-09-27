// <copyright file="PowerUpWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;

/// <summary>
/// A wrapper class which adapts power ups to <see cref="IElement"/> instances.
/// </summary>
public sealed class PowerUpWrapper : IElement, IDisposable
{
    private readonly IElement _element;

    private ComposableAttribute? _parentAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerUpWrapper"/> class.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    /// <param name="attributeHolder">The attribute holder.</param>
    public PowerUpWrapper(IElement element, AttributeDefinition targetAttribute, AttributeSystem attributeHolder)
    {
        this._parentAttribute = attributeHolder.GetComposableAttribute(targetAttribute);
        if (this._parentAttribute is null)
        {
            throw new ArgumentException($"Target attribute [{targetAttribute}] is not composable", nameof(targetAttribute));
        }

        this._element = element;
        this._parentAttribute.AddElement(this);
        this._element.ValueChanged += this.OnValueChanged;
    }

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;

    /// <inheritdoc/>
    public float Value => this._element.Value;

    /// <inheritdoc/>
    public AggregateType AggregateType => this._element.AggregateType;

    /// <summary>
    /// Creates elements by a <see cref="PowerUpDefinition"/>.
    /// </summary>
    /// <param name="powerUpDef">The power up definition.</param>
    /// <param name="attributeHolder">The attribute holder.</param>
    /// <returns>The elements which represent the power-up.</returns>
    public static IEnumerable<PowerUpWrapper> CreateByPowerUpDefinition(PowerUpDefinition powerUpDef, AttributeSystem attributeHolder)
    {
        if (powerUpDef.Boost?.ConstantValue != null)
        {
            yield return new PowerUpWrapper(
                powerUpDef.Boost.ConstantValue,
                powerUpDef.TargetAttribute ?? throw Error.NotInitializedProperty(powerUpDef, nameof(PowerUpDefinition.TargetAttribute)),
                attributeHolder);
        }

        if (powerUpDef.Boost?.RelatedValues != null)
        {
            foreach (var relationship in powerUpDef.Boost.RelatedValues)
            {
                var aggregateType = powerUpDef.Boost?.ConstantValue.AggregateType ?? AggregateType.AddRaw;
                yield return new PowerUpWrapper(
                    attributeHolder.CreateRelatedAttribute(relationship, attributeHolder, aggregateType),
                    powerUpDef.TargetAttribute ?? throw Error.NotInitializedProperty(powerUpDef, nameof(PowerUpDefinition.TargetAttribute)),
                    attributeHolder);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this._parentAttribute != null)
        {
            this._parentAttribute.RemoveElement(this);
            this.ValueChanged = null;
            this._parentAttribute = null;
            this._element.ValueChanged -= this.OnValueChanged;
        }
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        return $"{this._parentAttribute?.Definition.Designation}: {this._element}";
    }

    private void OnValueChanged(object? sender, EventArgs eventArgs)
    {
        this.ValueChanged?.Invoke(sender, eventArgs);
    }
}