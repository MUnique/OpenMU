// <copyright file="TrapAttributeHolder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The attribute system for traps, which is considering monster definitions.
/// </summary>
public class TrapAttributeHolder : IAttributeSystem
{
    private static readonly IDictionary<AttributeDefinition, Func<Trap, float>> StatMapping =
        new Dictionary<AttributeDefinition, Func<Trap, float>>
        {
            { Stats.AttackDamageIncrease, _ => 1.0f },
            { Stats.ShieldBypassChance, _ => 1.0f },
        };

    private static readonly IDictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>> MonsterStatAttributesCache = new Dictionary<MonsterDefinition, IDictionary<AttributeDefinition, float>>();

    private readonly Trap _trap;

    private readonly IDictionary<AttributeDefinition, float> _statAttributes;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrapAttributeHolder"/> class.
    /// </summary>
    /// <param name="trap">The trap.</param>
    public TrapAttributeHolder(Trap trap)
    {
        this._trap = trap;
        this._statAttributes = GetStatAttributeOfMonster(trap.Definition);
    }

    /// <inheritdoc/>
    public float this[AttributeDefinition attributeDefinition]
    {
        get => this.GetValueOfAttribute(attributeDefinition);

        set
        {
            // can't set anything for Traps.
        }
    }

    /// <inheritdoc/>
    public float GetValueOfAttribute(AttributeDefinition attributeDefinition)
    {
        if (this._statAttributes.TryGetValue(attributeDefinition, out float value))
        {
            return value;
        }

        if (StatMapping.TryGetValue(attributeDefinition, out var mappingFunction))
        {
            return mappingFunction(this._trap);
        }

        return 0;
    }

    /// <inheritdoc/>
    public void AddElement(IElement element, AttributeDefinition targetAttribute)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element, AttributeDefinition targetAttribute)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void AddAttributeRelationship(AttributeRelationship combination, IAttributeSystem sourceAttributeHolder, AggregateType aggregateType)
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
                m => m.AttributeDefinition ?? throw Error.NotInitializedProperty(m, nameof(PowerUpDefinition.TargetAttribute)),
                m => m.Value);
            MonsterStatAttributesCache.Add(monsterDef, result);
        }

        return result;
    }
}