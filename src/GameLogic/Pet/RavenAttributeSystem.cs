// <copyright file="RavenAttributeSystem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Pet;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// An <see cref="IAttributeSystem"/> which provides the attack attributes of the raven.
/// </summary>
public class RavenAttributeSystem : IAttributeSystem
{
    private static readonly Dictionary<AttributeDefinition, Func<Player, float>> StatMapping =
        new()
        {
            {
                Stats.MinimumPhysBaseDmg, player =>
                {
                    var minDamage = player.Attributes?[Stats.RavenMinimumDamage] ?? 0;
                    minDamage += minDamage * (player.Attributes?[Stats.RavenAttackDamageIncrease] ?? 1);
                    return minDamage;
                }
            },
            {
                Stats.MaximumPhysBaseDmg, player =>
                {
                    var maxDamage = player.Attributes?[Stats.RavenMaximumDamage] ?? 0;
                    maxDamage += maxDamage * (player.Attributes?[Stats.RavenAttackDamageIncrease] ?? 1);
                    return maxDamage;
                }
            },
            { Stats.RavenBonusDamage, player => player.Attributes?[Stats.RavenBonusDamage] ?? 0 },
            { Stats.AttackSpeed, player => player.Attributes?[Stats.RavenAttackSpeed] ?? 0 },
            { Stats.AttackRatePvm, player => player.Attributes?[Stats.RavenAttackRate] ?? 0 },
            { Stats.AttackRatePvp, player => player.Attributes?[Stats.AttackRatePvp] ?? 0 },
            { Stats.CriticalDamageChance, player => player.Attributes?[Stats.RavenCriticalDamageChance] ?? 0 },
            { Stats.ExcellentDamageChance, player => player.Attributes?[Stats.RavenExcDamageChance] ?? 0 },
            { Stats.DefenseIgnoreChance, player => player.Attributes?[Stats.DefenseIgnoreChance] ?? 0 },
            { Stats.ShieldBypassChance, player => player.Attributes?[Stats.ShieldBypassChance] ?? 0 },
            { Stats.ShieldDecreaseRateIncrease, player => player.Attributes?[Stats.ShieldDecreaseRateIncrease] ?? 0 },
            { Stats.AttackDamageIncrease, player => 1.0f },
        };

    private readonly Player _owner;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenAttributeSystem"/> class.
    /// </summary>
    /// <param name="owner">The owner of the pet.</param>
    public RavenAttributeSystem(Player owner)
    {
        this._owner = owner;
    }

    /// <inheritdoc />
    public float this[AttributeDefinition key]
    {
        get => this.GetValueOfAttribute(key);
        set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public float GetValueOfAttribute(AttributeDefinition attributeDefinition)
    {
        if (StatMapping.TryGetValue(attributeDefinition, out var mappingFunction))
        {
            return mappingFunction(this._owner);
        }

        return 0;
    }

    /// <inheritdoc />
    public void AddElement(IElement element, AttributeDefinition targetAttribute)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void RemoveElement(IElement element, AttributeDefinition targetAttribute)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void AddAttributeRelationship(AttributeRelationship relationship, IAttributeSystem sourceAttributeHolder, AggregateType aggregateType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IElement GetOrCreateAttribute(AttributeDefinition attributeDefinition)
    {
        throw new NotImplementedException();
    }
}