// <copyright file="AttributeSystemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Extensions for <see cref="IAttributeSystem"/>s.
/// </summary>
public static class AttributeSystemExtensions
{
    /// <summary>
    /// Gets the attribute for a dummy attribute to be used internally for durations.
    /// </summary>
    private static AttributeDefinition DurationDummy { get; } = new(new Guid("23D069C3-24D8-4277-8FDC-D82F0AF64037"), "Duration Dummy", "A dummy attribute to be used internally for durations.");

    /// <summary>
    /// Creates a new element on this attribute system with the specified power up value.
    /// </summary>
    /// <param name="attributeSystem">The attribute system.</param>
    /// <param name="powerUpDefinition">The power up definition.</param>
    /// <returns>The added element.</returns>
    public static IElement CreateDurationElement(this IAttributeSystem attributeSystem, PowerUpDefinitionValue powerUpDefinition)
    {
        return attributeSystem.CreateElement(powerUpDefinition, DurationDummy);
    }

    /// <summary>
    /// Creates a new element on this attribute system with the specified power up value.
    /// </summary>
    /// <param name="attributeSystem">The attribute system.</param>
    /// <param name="powerUpDefinition">The power up definition.</param>
    /// <returns>The added element.</returns>
    public static IElement CreateElement(this IAttributeSystem attributeSystem, PowerUpDefinition powerUpDefinition)
    {
        var value = powerUpDefinition.Boost ?? throw Error.NotInitializedProperty(powerUpDefinition, nameof(powerUpDefinition.Boost));
        var targetDefinition = powerUpDefinition.TargetAttribute ?? throw Error.NotInitializedProperty(powerUpDefinition, nameof(powerUpDefinition.TargetAttribute));
        return attributeSystem.CreateElement(value, targetDefinition);
    }

    /// <summary>
    /// Creates a new element on this attribute system with the specified power up value.
    /// </summary>
    /// <param name="attributeSystem">The attribute system.</param>
    /// <param name="value">The value.</param>
    /// <param name="targetDefinition">The attribute.</param>
    /// <returns>The added element.</returns>
    public static IElement CreateElement(this IAttributeSystem attributeSystem, PowerUpDefinitionValue value, AttributeDefinition targetDefinition)
    {
        var relations = value.RelatedValues;
        var result = value.ConstantValue;
        if (relations?.Any() ?? false)
        {
            var elements = relations
                .Select(r => new AttributeRelationshipElement(
                    new[] { attributeSystem.GetOrCreateAttribute(r.InputAttribute ?? throw new InvalidOperationException($"InputAttribute value not set for AttributeRelationship {r.GetId()}.")) },
                    r.GetOperandElement(attributeSystem),
                    r.InputOperator)
                {
                    AggregateType = result.AggregateType,
                })
                .Cast<IElement>();

            if (value.ConstantValue is not null)
            {
                elements = elements.Concat(value.ConstantValue.GetAsEnumerable());
            }

            var composableResult = new ComposableAttribute(targetDefinition, result.AggregateType);

            elements.ForEach(element => composableResult.AddElement(element));
            return composableResult;
        }

        if (result is null)
        {
            throw new ArgumentException($"The passed {nameof(PowerUpDefinitionValue)} doesn't have a constant value or related values.", nameof(value));
        }

        return result;
    }
}