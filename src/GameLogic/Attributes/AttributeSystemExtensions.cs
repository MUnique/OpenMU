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
    /// Creates a new element on this attribute system with the specified power up value.
    /// </summary>
    /// <param name="attributeSystem">The attribute system.</param>
    /// <param name="value">The value.</param>
    /// <returns>The added element.</returns>
    public static IElement CreateElement(this IAttributeSystem attributeSystem, PowerUpDefinitionValue value)
    {
        var relations = value.RelatedValues;
        var result = value.ConstantValue;
        if (relations?.Any() ?? false)
        {
            var elements = relations
                .Select(r => new AttributeRelationshipElement(
                    new[] { attributeSystem.GetOrCreateAttribute(r.InputAttribute ?? throw new InvalidOperationException($"InputAttribute value not set for AttributeRelationship {r.GetId()}.")) },
                    r.GetOperandElement(attributeSystem),
                    r.InputOperator))
                .Cast<IElement>();
            if (value.ConstantValue != null)
            {
                elements = elements.Concat(value.ConstantValue.GetAsEnumerable());
            }

            result = new AttributeRelationshipElement(elements.ToList(), new ConstantElement(1.0F), InputOperator.Multiply);
        }

        if (result is null)
        {
            throw new ArgumentException($"The passed {nameof(PowerUpDefinitionValue)} doesn't have a constant value or related values.", nameof(value));
        }

        return result;
    }
}