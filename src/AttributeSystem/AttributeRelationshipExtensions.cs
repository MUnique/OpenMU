// <copyright file="AttributeRelationshipExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Extensions for <see cref="AttributeRelationship"/>.
/// </summary>
public static class AttributeRelationshipExtensions
{
    /// <summary>
    /// Gets the target attribute and throws an exception if it's not initialized yet.
    /// </summary>
    /// <param name="relationship">The attribute relationship.</param>
    /// <returns>The target attribute definition.</returns>
    /// <exception cref="InvalidOperationException">TargetAttribute not initialized.</exception>
    public static AttributeDefinition GetTargetAttribute(this AttributeRelationship relationship)
    {
        return relationship.TargetAttribute ?? throw new InvalidOperationException("TargetAttribute not initialized.");
    }

    /// <summary>
    /// Gets the input attribute and throws an exception if it's not initialized yet.
    /// </summary>
    /// <param name="relationship">The attribute relationship.</param>
    /// <returns>The input attribute definition.</returns>
    /// <exception cref="InvalidOperationException">TargetAttribute not initialized.</exception>
    public static AttributeDefinition GetInputAttribute(this AttributeRelationship relationship)
    {
        return relationship.InputAttribute ?? throw new InvalidOperationException("InputAttribute not initialized.");
    }

    /// <summary>
    /// Gets the operand attribute.
    /// </summary>
    /// <param name="relationship">The attribute relationship.</param>
    /// <param name="attributeSystem">The attribute system.</param>
    /// <returns>
    /// The input attribute definition.
    /// </returns>
    public static IElement GetOperandElement(this AttributeRelationship relationship, IAttributeSystem attributeSystem)
    {
        if (relationship.OperandAttribute is { } operandAttribute)
        {
            return attributeSystem.GetOrCreateAttribute(operandAttribute);
        }

        return new ConstantElement(relationship.InputOperand);
    }
}