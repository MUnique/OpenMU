// <copyright file="IAttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute system which holds all attributes of a game object.
/// </summary>
public interface IAttributeSystem
{
    /// <summary>
    /// Gets or sets the value with the specified attribute definition.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The value of the specified attribute.</returns>
    float this[AttributeDefinition attributeDefinition] { get; set; }

    /// <summary>
    /// Gets the value of the attribute.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The value of the attribute.</returns>
    float GetValueOfAttribute(AttributeDefinition attributeDefinition);

    /// <summary>
    /// Adds the element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    void AddElement(IElement element, AttributeDefinition targetAttribute);

    /// <summary>
    /// Removes the element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="targetAttribute">The target attribute.</param>
    void RemoveElement(IElement element, AttributeDefinition targetAttribute);

    /// <summary>
    /// Adds the attribute relationship.
    /// </summary>
    /// <param name="relationship">The relationship.</param>
    /// <param name="sourceAttributeHolder">The source attribute holder. May be the attribute system of another player.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    void AddAttributeRelationship(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder, AggregateType aggregateType);

    /// <summary>
    /// Gets or creates the element with the specified attribute.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The element of the attribute.</returns>
    IElement GetOrCreateAttribute(AttributeDefinition attributeDefinition);
}