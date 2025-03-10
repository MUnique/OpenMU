// <copyright file="AttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

using System.Collections;

/// <summary>
/// The attribute system which holds all attributes of a character.
/// </summary>
public class AttributeSystem : IAttributeSystem, IEnumerable<IAttribute>
{
    private readonly IDictionary<AttributeDefinition, IAttribute> _attributes = new Dictionary<AttributeDefinition, IAttribute>();

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeSystem" /> class.
    /// </summary>
    /// <param name="statAttributes">The stat attributes. These attributes are added just as-is and are not wrapped by a <see cref="ComposableAttribute"/>.</param>
    /// <param name="baseAttributes">The initial base attributes. These attributes contain the base values which will be wrapped by a <see cref="ComposableAttribute"/>, so additional elements can contribute to the attributes value. Instead of providing them here, you could also add them to the system by calling <see cref="AddElement"/> later.</param>
    /// <param name="attributeRelationships">The initial attribute relationships. Instead of providing them here, you could also add them to the system by calling <see cref="AddAttributeRelationship(AttributeRelationship, IAttributeSystem, AggregateType)"/> later.</param>
    public AttributeSystem(IEnumerable<IAttribute> statAttributes, IEnumerable<IAttribute> baseAttributes, IEnumerable<AttributeRelationship> attributeRelationships)
    {
        foreach (var statAttribute in statAttributes)
        {
            this._attributes.Add(statAttribute.Definition, statAttribute);
        }

        foreach (var baseAttribute in baseAttributes)
        {
            this.AddElement(baseAttribute, baseAttribute.Definition);
        }

        foreach (var combination in attributeRelationships)
        {
            this.AddAttributeRelationship(combination);
        }
    }

    /// <inheritdoc/>
    public float this[AttributeDefinition? attributeDefinition]
    {
        get => this.GetValueOfAttribute(attributeDefinition);

        set => this.SetStatAttribute(attributeDefinition, value);
    }

    /// <inheritdoc/>
    public void AddAttributeRelationship(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder, AggregateType aggregateType)
    {
        if (this.GetOrCreateAttribute(relationship.GetTargetAttribute()) is IComposableAttribute targetAttribute)
        {
            var relatedElement = this.CreateRelatedAttribute(relationship, sourceAttributeHolder, aggregateType);
            targetAttribute.AddElement(relatedElement);
        }
    }

    /// <summary>
    /// Creates the related attribute.
    /// </summary>
    /// <param name="relationship">The relationship.</param>
    /// <param name="sourceAttributeHolder">The source attribute holder. This may be the attribute system of another player.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <returns>
    /// The newly created relationship element.
    /// </returns>
    public IElement CreateRelatedAttribute(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder, AggregateType aggregateType)
    {
        var inputElements = new[] { sourceAttributeHolder.GetOrCreateAttribute(relationship.GetInputAttribute()) };
        return new AttributeRelationshipElement(inputElements, relationship.GetOperandElement(sourceAttributeHolder), relationship.InputOperator)
        {
            AggregateType = aggregateType,
        };
    }

    /// <summary>
    /// Sets the stat attribute, if the <paramref name="attributeDefinition"/> is a stat attribute.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>The success.</returns>
    public bool SetStatAttribute(AttributeDefinition? attributeDefinition, float newValue)
    {
        if (attributeDefinition is null)
        {
            return false;
        }

        if (this._attributes.TryGetValue(attributeDefinition, out var attribute)
            && attribute is StatAttribute statAttribute)
        {
            statAttribute.Value = newValue;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the composable attribute.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The composable attribute.</returns>
    public ComposableAttribute? GetComposableAttribute(AttributeDefinition attributeDefinition)
    {
        return this.GetOrCreateAttribute(attributeDefinition) as ComposableAttribute;
    }

    /// <inheritdoc/>
    public float GetValueOfAttribute(AttributeDefinition? attributeDefinition)
    {
        var element = this.GetAttribute(attributeDefinition);
        if (element != null)
        {
            if (attributeDefinition?.MaximumValue is { } maximumValue && element.Value > maximumValue)
            {
                return maximumValue;
            }

            return element.Value;
        }

        return 0;
    }

    /// <inheritdoc/>
    public void AddElement(IElement element, AttributeDefinition targetAttribute)
    {
        if (!this._attributes.TryGetValue(targetAttribute, out var attribute))
        {
            attribute = new ComposableAttribute(targetAttribute);
            this._attributes.Add(targetAttribute, attribute);
            this.OnAttributeAdded(attribute);
        }

        if (attribute is IComposableAttribute composableAttribute)
        {
            composableAttribute.AddElement(element);
        }
        else
        {
            throw new ArgumentException($"Attribute {targetAttribute} is not a composable attribute.");
        }
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element, AttributeDefinition targetAttribute)
    {
        if (this._attributes.TryGetValue(targetAttribute, out var attribute))
        {
            if (attribute is IComposableAttribute composableAttribute)
            {
                composableAttribute.RemoveElement(element);
                if (!composableAttribute.Elements.Any())
                {
                    this._attributes.Remove(targetAttribute);
                }
            }
            else
            {
                throw new ArgumentException($"Attribute {targetAttribute} is not a composable attribute.");
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Stat Attributes:");
        foreach (var statAttribute in this._attributes.Values.OfType<StatAttribute>())
        {
            stringBuilder.AppendLine($"  {statAttribute.Definition}: {statAttribute.Value}");
        }

        stringBuilder.AppendLine("Others:");
        foreach (var attribute in this._attributes.Values.OfType<IComposableAttribute>())
        {
            stringBuilder.AppendLine($"  {attribute.Definition}: {attribute.Value}");
        }

        return stringBuilder.ToString();
    }

    /// <inheritdoc />
    public IEnumerator<IAttribute> GetEnumerator()
    {
        return this._attributes.Values.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Gets or creates the element with the specified attribute.
    /// </summary>
    /// <param name="attributeDefinition">The attribute definition.</param>
    /// <returns>The element of the attribute.</returns>
    public IElement GetOrCreateAttribute(AttributeDefinition attributeDefinition)
    {
        var element = this.GetAttribute(attributeDefinition);
        if (element is null)
        {
            var composableAttribute = new ComposableAttribute(attributeDefinition);
            element = composableAttribute;
            this._attributes.Add(attributeDefinition, composableAttribute);
            this.OnAttributeAdded(composableAttribute);
        }

        return element;
    }

    /// <summary>
    /// Called when an attribute was added to the system after the initial construction.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    protected virtual void OnAttributeAdded(IAttribute attribute)
    {
        // can be overwritten.
    }

    /// <summary>
    /// Called when an attribute was removed from the system.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    protected virtual void OnAttributeRemoved(IAttribute attribute)
    {
        // can be overwritten.
    }

    /// <summary>
    /// Adds the attribute relationship.
    /// </summary>
    /// <param name="combination">The combination.</param>
    private void AddAttributeRelationship(AttributeRelationship combination)
    {
        this.AddAttributeRelationship(combination, this, combination.AggregateType);
    }

    private IElement? GetAttribute(AttributeDefinition? attributeDefinition)
    {
        if (attributeDefinition is null)
        {
            return null;
        }

        if (this._attributes.TryGetValue(attributeDefinition, out var attribute))
        {
            return attribute;
        }

        return null;
    }
}