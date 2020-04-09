// <copyright file="TrapAttributeHolder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.NPC;

    /// <summary>
    /// The attribute system for traps, which is considering monster definitions.
    /// </summary>
    public class TrapAttributeHolder : IAttributeSystem
    {
        private static readonly IDictionary<AttributeDefinition, Func<Trap, float>> StatMapping =
            new Dictionary<AttributeDefinition, Func<Trap, float>>
            {
                { Stats.AttackDamageIncrease, m => 1.0f },
                { Stats.ShieldBypassChance, m => 1.0f },
            };

        private static readonly IDictionary<AttributeDefinition, Action<Trap, float>> SetterMapping =
            new Dictionary<AttributeDefinition, Action<Trap, float>> { };

        private static readonly IDictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>> MonsterStatAttributesCache = new Dictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>>();

        private readonly Trap trap;

        private readonly IDictionary<AttributeDefinition, float> statAttributes;

        private readonly object attributesLock = new object();

        /// <summary>
        /// Attribute dictionary of a monster instance.
        /// Most monster instances don't have additional attributes, so we just instanciate one if needed.
        /// </summary>
        private IDictionary<AttributeDefinition, IComposableAttribute> attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapAttributeHolder"/> class.
        /// </summary>
        /// <param name="trap">The trap.</param>
        public TrapAttributeHolder(Trap trap)
        {
            this.trap = trap;
            this.statAttributes = GetStatAttributeOfMonster(trap.Definition);
        }

        /// <inheritdoc/>
        public float this[AttributeDefinition attributeDefinition]
        {
            get
            {
                return this.GetValueOfAttribute(attributeDefinition);
            }

            set
            {
                if (SetterMapping.TryGetValue(attributeDefinition, out Action<Trap, float> setAction))
                {
                    setAction(this.trap, value);
                }
            }
        }

        /// <inheritdoc/>
        public float GetValueOfAttribute(AttributeDefinition attributeDefinition)
        {
            if (this.attributes != null && this.attributes.TryGetValue(attributeDefinition, out IComposableAttribute attribute))
            {
                return attribute.Value;
            }

            if (this.statAttributes.TryGetValue(attributeDefinition, out float value))
            {
                return value;
            }

            if (StatMapping.TryGetValue(attributeDefinition, out Func<Trap, float> mappingFunction))
            {
                return mappingFunction(this.trap);
            }

            return 0;
        }

        /// <inheritdoc/>
        public void AddElement(IElement element, AttributeDefinition targetAttribute)
        {
            var attributeDictionary = this.GetAttributeDictionary();
            if (!attributeDictionary.TryGetValue(targetAttribute, out IComposableAttribute attribute))
            {
                attribute = new ComposableAttribute(targetAttribute);
                var attrValue = this.GetValueOfAttribute(targetAttribute);
                var nullValue = element.AggregateType == AggregateType.Multiplicate ? 1 : 0;
                attrValue = Math.Abs(attrValue) < 0.01f ? nullValue : attrValue;
                attribute.AddElement(new SimpleElement { Value = attrValue });
            }

            attribute.AddElement(element);
        }

        /// <inheritdoc/>
        public void RemoveElement(IElement element, AttributeDefinition targetAttribute)
        {
            var attributeDictionary = this.attributes;
            if (attributeDictionary != null)
            {
                if (attributeDictionary.TryGetValue(targetAttribute, out IComposableAttribute attribute))
                {
                    attribute.RemoveElement(element);
                    if (attribute.Elements.Skip(1).Take(1).Any())
                    {
                        attributeDictionary.Remove(targetAttribute);
                    }
                }

                if (attributeDictionary.Count == 0)
                {
                    lock (this.attributesLock)
                    {
                        this.attributes = null;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void AddAttributeRelationship(AttributeRelationship combination, IAttributeSystem sourceAttributeHolder)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IElement GetOrCreateAttribute(AttributeDefinition attributeDefinition)
        {
            throw new NotImplementedException();
        }

        private static IDictionary<AttributeDefinition, float> GetStatAttributeOfMonster(MonsterDefinition monsterDef)
        {
            if (!MonsterStatAttributesCache.TryGetValue(monsterDef, out IDictionary<AttributeDefinition, float> result))
            {
                result = monsterDef.Attributes.ToDictionary(m => m.AttributeDefinition, m => m.Value);
                MonsterStatAttributesCache.Add(monsterDef, result);
            }

            return result;
        }

        private IDictionary<AttributeDefinition, IComposableAttribute> GetAttributeDictionary()
        {
            if (this.attributes == null)
            {
                lock (this.attributesLock)
                {
                    if (this.attributes == null)
                    {
                        this.attributes = new Dictionary<AttributeDefinition, IComposableAttribute>();
                    }
                }
            }

            return this.attributes;
        }
    }
}