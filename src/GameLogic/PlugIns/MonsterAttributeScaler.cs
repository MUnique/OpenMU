// <copyright file="MonsterAttributeScaler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A feature plugin which scales all monster base stats by a configurable percentage.
/// Uses shared multiplicative IElement instances. Configuration changes take effect immediately.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.MonsterAttributeScaler_Name), Description = nameof(PlugInResources.MonsterAttributeScaler_Description), ResourceType = typeof(PlugInResources))]
[Guid("59E5B45F-0A3D-4BAF-8B6C-9E1D2F3A4B5C")]
public class MonsterAttributeScaler : IFeaturePlugIn, IObjectAddedToMapPlugIn, IObjectRemovedFromMapPlugIn,
    ISupportCustomConfiguration<MonsterAttributeScalerConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    private readonly SimpleElement _damageMultiplier = new(1.0f, AggregateType.Multiplicate);
    private readonly SimpleElement _attackRateMultiplier = new(1.0f, AggregateType.Multiplicate);
    private readonly SimpleElement _defenseRateMultiplier = new(1.0f, AggregateType.Multiplicate);
    private readonly SimpleElement _defenseMultiplier = new(1.0f, AggregateType.Multiplicate);
    private readonly SimpleElement _healthMultiplier = new(1.0f, AggregateType.Multiplicate);

    private MonsterAttributeScalerConfiguration? _configuration;

    /// <inheritdoc />
    public MonsterAttributeScalerConfiguration? Configuration
    {
        get => this._configuration;
        set
        {
            this._configuration = value;

            if (value is null)
            {
                this._damageMultiplier.Value = 1.0f;
                this._attackRateMultiplier.Value = 1.0f;
                this._defenseRateMultiplier.Value = 1.0f;
                this._defenseMultiplier.Value = 1.0f;
                this._healthMultiplier.Value = 1.0f;
                return;
            }

            this._damageMultiplier.Value = value.DamagePercentage > 0 ? 1.0f + (value.DamagePercentage / 100.0f) : 1.0f;
            this._attackRateMultiplier.Value = value.AttackRatePercentage > 0 ? 1.0f + (value.AttackRatePercentage / 100.0f) : 1.0f;
            this._defenseRateMultiplier.Value = value.DefenseRatePercentage > 0 ? 1.0f + (value.DefenseRatePercentage / 100.0f) : 1.0f;
            this._defenseMultiplier.Value = value.DefensePercentage > 0 ? 1.0f + (value.DefensePercentage / 100.0f) : 1.0f;
            this._healthMultiplier.Value = value.HealthPercentage > 0 ? 1.0f + (value.HealthPercentage / 100.0f) : 1.0f;
        }
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new MonsterAttributeScalerConfiguration();
    }

    /// <inheritdoc />
    public ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is not AttackableNpcBase monster)
        {
            return ValueTask.CompletedTask;
        }

        monster.Attributes.AddElement(this._damageMultiplier, Stats.MinimumPhysBaseDmg);
        monster.Attributes.AddElement(this._damageMultiplier, Stats.MaximumPhysBaseDmg);
        monster.Attributes.AddElement(this._attackRateMultiplier, Stats.AttackRatePvm);
        monster.Attributes.AddElement(this._defenseRateMultiplier, Stats.DefenseRatePvm);
        monster.Attributes.AddElement(this._defenseMultiplier, Stats.DefenseBase);
        monster.Attributes.AddElement(this._healthMultiplier, Stats.MaximumHealth);

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is not AttackableNpcBase monster)
        {
            return ValueTask.CompletedTask;
        }

        monster.Attributes.RemoveElement(this._damageMultiplier, Stats.MinimumPhysBaseDmg);
        monster.Attributes.RemoveElement(this._damageMultiplier, Stats.MaximumPhysBaseDmg);
        monster.Attributes.RemoveElement(this._attackRateMultiplier, Stats.AttackRatePvm);
        monster.Attributes.RemoveElement(this._defenseRateMultiplier, Stats.DefenseRatePvm);
        monster.Attributes.RemoveElement(this._defenseMultiplier, Stats.DefenseBase);
        monster.Attributes.RemoveElement(this._healthMultiplier, Stats.MaximumHealth);

        return ValueTask.CompletedTask;
    }
}
