// <copyright file="AttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The attribute system which holds all attributes of a character.
    /// </summary>
    public class AttributeSystem : IAttributeSystem
    {
        private readonly IDictionary<AttributeDefinition, IAttribute> attributes = new Dictionary<AttributeDefinition, IAttribute>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeSystem" /> class.
        /// </summary>
        /// <param name="statAttributes">The stat attributes. These attributes are added just as-is and are not wrapped by a <see cref="ComposableAttribute"/>.</param>
        /// <param name="baseAttributes">The initial base attributes. These attributes contain the base values which will be wrapped by a <see cref="ComposableAttribute"/>, so additional elements can contribute to the attributes value. Instead of providing them here, you could also add them to the system by calling <see cref="AddElement"/> later.</param>
        /// <param name="attributeRelationships">The initial attribute relationships. Instead of providing them here, you could also add them to the system by calling <see cref="AddAttributeRelationship(AttributeRelationship, IAttributeSystem)"/> later.</param>
        public AttributeSystem(IEnumerable<IAttribute> statAttributes, IEnumerable<IAttribute> baseAttributes, IEnumerable<AttributeRelationship> attributeRelationships)
        {
            foreach (var statAttribute in statAttributes)
            {
                this.attributes.Add(statAttribute.Definition, statAttribute);
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
        public float this[AttributeDefinition attributeDefinition]
        {
            get => this.GetValueOfAttribute(attributeDefinition);

            set => this.SetStatAttribute(attributeDefinition, value);
        }

        /// <inheritdoc/>
        public void AddAttributeRelationship(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder)
        {
            if (this.GetOrCreateAttribute(relationship.TargetAttribute) is IComposableAttribute targetAttribute)
            {
                var relatedElement = this.CreateRelatedAttribute(relationship, sourceAttributeHolder);
                targetAttribute.AddElement(relatedElement);
            }
        }

        /// <summary>
        /// Creates the related attribute.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="sourceAttributeHolder">The source attribute holder. May be the attribute system of another player.</param>
        /// <returns>The newly created relationship element.</returns>
        public IElement CreateRelatedAttribute(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder)
        {
            var inputElements = new[] { sourceAttributeHolder.GetOrCreateAttribute(relationship.InputAttribute) };
            return new AttributeRelationshipElement(inputElements, relationship.InputOperand, relationship.InputOperator);
        }

        /// <summary>
        /// Sets the stat attribute, if the <paramref name="attributeDefinition"/> is a stat attribute.
        /// </summary>
        /// <param name="attributeDefinition">The attribute definition.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The success.</returns>
        public bool SetStatAttribute(AttributeDefinition attributeDefinition, float newValue)
        {
            if (this.attributes.TryGetValue(attributeDefinition, out IAttribute attribute)
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
        public ComposableAttribute GetComposableAttribute(AttributeDefinition attributeDefinition)
        {
            return this.GetOrCreateAttribute(attributeDefinition) as ComposableAttribute;
        }

        /// <inheritdoc/>
        public float GetValueOfAttribute(AttributeDefinition attributeDefinition)
        {
            IElement element = this.GetAttribute(attributeDefinition);
            if (element != null)
            {
                return element.Value;
            }

            return 0;
        }

        /// <inheritdoc/>
        public void AddElement(IElement element, AttributeDefinition targetAttribute)
        {
            if (!this.attributes.TryGetValue(targetAttribute, out IAttribute attribute))
            {
                attribute = new ComposableAttribute(targetAttribute);
                this.attributes.Add(targetAttribute, attribute);
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
            if (this.attributes.TryGetValue(targetAttribute, out IAttribute attribute))
            {
                if (attribute is IComposableAttribute composableAttribute)
                {
                    composableAttribute.RemoveElement(element);
                    if (!composableAttribute.Elements.Any())
                    {
                        this.attributes.Remove(targetAttribute);
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
            foreach (var statAttribute in this.attributes.OfType<StatAttribute>())
            {
                stringBuilder.AppendLine(statAttribute.Value.ToString(CultureInfo.InvariantCulture));
            }

            stringBuilder.AppendLine("Others:");
            foreach (var attribute in this.attributes.OfType<IComposableAttribute>())
            {
                stringBuilder.AppendLine(attribute.Value.ToString(CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets or creates the element with the specified attribute.
        /// </summary>
        /// <param name="attributeDefinition">The attribute definition.</param>
        /// <returns>The element of the attribute.</returns>
        public IElement GetOrCreateAttribute(AttributeDefinition attributeDefinition)
        {
            IElement element = this.GetAttribute(attributeDefinition);
            if (element == null)
            {
                var composableAttribute = new ComposableAttribute(attributeDefinition);
                element = composableAttribute;
                this.attributes.Add(attributeDefinition, composableAttribute);
            }

            return element;
        }

        /// <summary>
        /// Adds the attribute relationship.
        /// </summary>
        /// <param name="combination">The combination.</param>
        private void AddAttributeRelationship(AttributeRelationship combination)
        {
            this.AddAttributeRelationship(combination, this);
        }

        private IElement GetAttribute(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition == null)
            {
                return null;
            }

            if (this.attributes.TryGetValue(attributeDefinition, out IAttribute attribute))
            {
                return attribute;
            }

            return null;
        }
    }
}
