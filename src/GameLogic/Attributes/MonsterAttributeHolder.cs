﻿// <copyright file="MonsterAttributeHolder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The attribute system for monsters, which is considering monster definitions.
/// </summary>
public class MonsterAttributeHolder : IAttributeSystem
{
    private static readonly IDictionary<AttributeDefinition, Func<Monster, float>> StatMapping =
        new Dictionary<AttributeDefinition, Func<Monster, float>>
        {
            { Stats.CurrentHealth, m => m.Health },
            { Stats.DefensePvm, m => m.Attributes.GetValueOfAttribute(Stats.DefenseBase) },
            { Stats.DefensePvp, m => m.Attributes.GetValueOfAttribute(Stats.DefenseBase) },
            { Stats.DamageReceiveDecrement, m => 1.0f },
            { Stats.AttackDamageIncrease, m => 1.0f },
            { Stats.ShieldBypassChance, m => 1.0f },
        };

    private static readonly IDictionary<AttributeDefinition, Action<Monster, float>> SetterMapping =
        new Dictionary<AttributeDefinition, Action<Monster, float>>
        {
            { Stats.CurrentHealth, (m, v) => m.Health = (int)v },
        };

    private static readonly IDictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>> MonsterStatAttributesCache = new Dictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>>();

    private readonly Monster _monster;

    private readonly IDictionary<AttributeDefinition, float> _statAttributes;

    private readonly object _attributesLock = new ();

    /// <summary>
    /// Attribute dictionary of a monster instance.
    /// Most monster instances don't have additional attributes, so we just instantiate one if needed.
    /// </summary>
    private IDictionary<AttributeDefinition, IComposableAttribute>? _attributes;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterAttributeHolder"/> class.
    /// </summary>
    /// <param name="monster">The monster.</param>
    public MonsterAttributeHolder(Monster monster)
    {
        this._monster = monster;
        this._statAttributes = GetStatAttributeOfMonster(monster.Definition);
    }

    /// <inheritdoc/>
    public float this[AttributeDefinition attributeDefinition]
    {
        get => this.GetValueOfAttribute(attributeDefinition);

        set
        {
            if (SetterMapping.TryGetValue(attributeDefinition, out var setAction))
            {
                setAction(this._monster, value);
            }
        }
    }

    /// <inheritdoc/>
    public float GetValueOfAttribute(AttributeDefinition attributeDefinition)
    {
        if (this._attributes != null && this._attributes.TryGetValue(attributeDefinition, out var attribute))
        {
            return attribute.Value;
        }

        if (this._statAttributes.TryGetValue(attributeDefinition, out float value))
        {
            return value;
        }

        if (StatMapping.TryGetValue(attributeDefinition, out var mappingFunction))
        {
            return mappingFunction(this._monster);
        }

        return 0;
    }

    /// <inheritdoc/>
    public void AddElement(IElement element, AttributeDefinition targetAttribute)
    {
        var attributeDictionary = this.GetAttributeDictionary();
        if (!attributeDictionary.TryGetValue(targetAttribute, out var attribute))
        {
            attribute = new ComposableAttribute(targetAttribute);
            var attrValue = this.GetValueOfAttribute(targetAttribute);
            var nullValue = element.AggregateType == AggregateType.Multiplicate ? 1 : 0;
            attrValue = Math.Abs(attrValue) < 0.01f ? nullValue : attrValue;
            attribute.AddElement(new SimpleElement { Value = attrValue });
            attributeDictionary.Add(targetAttribute, attribute);
        }

        attribute.AddElement(element);
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element, AttributeDefinition targetAttribute)
    {
        var attributeDictionary = this._attributes;
        if (attributeDictionary != null)
        {
            if (attributeDictionary.TryGetValue(targetAttribute, out var attribute))
            {
                attribute.RemoveElement(element);
                if (attribute.Elements.Skip(1).Take(1).Any())
                {
                    attributeDictionary.Remove(targetAttribute);
                }
            }

            if (attributeDictionary.Count == 0)
            {
                lock (this._attributesLock)
                {
                    this._attributes = null;
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
        if (!MonsterStatAttributesCache.TryGetValue(monsterDef, out var result))
        {
            result = monsterDef.Attributes.ToDictionary(
                m => m.AttributeDefinition ?? throw Error.NotInitializedProperty(m, nameof(m.AttributeDefinition)),
                m => m.Value);
            MonsterStatAttributesCache.Add(monsterDef, result);
        }

        return result;
    }

    private IDictionary<AttributeDefinition, IComposableAttribute> GetAttributeDictionary()
    {
        if (this._attributes is null)
        {
            lock (this._attributesLock)
            {
                if (this._attributes is null)
                {
                    this._attributes = new Dictionary<AttributeDefinition, IComposableAttribute>();
                }
            }
        }

        return this._attributes;
    }
}