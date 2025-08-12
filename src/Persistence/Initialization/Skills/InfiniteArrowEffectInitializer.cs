// <copyright file="InfiniteArrowEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the infinity arrow effect.
/// </summary>
public class InfiniteArrowEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfiniteArrowEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public InfiniteArrowEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.InfiniteArrow;
        magicEffect.Name = "Infinity Arrow Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = true;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 600;

        // Multiply usage rate with 0, so no arrows are consumed anymore.
        var reduceAmmonitionUsage = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(reduceAmmonitionUsage);
        reduceAmmonitionUsage.TargetAttribute = Stats.AmmunitionConsumptionRate.GetPersistent(this.GameConfiguration);
        reduceAmmonitionUsage.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        reduceAmmonitionUsage.Boost.ConstantValue.Value = 0f;
        reduceAmmonitionUsage.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        // Placeholder for the Infinity Arrow Strengthener master skill
        var damageIncreaseByMasterSkill = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(damageIncreaseByMasterSkill);
        damageIncreaseByMasterSkill.TargetAttribute = Stats.AttackDamageIncrease.GetPersistent(this.GameConfiguration);
        damageIncreaseByMasterSkill.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
    }
}