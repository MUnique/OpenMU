// <copyright file="CriticalDamageIncreaseMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the critical damage increase mastery effect.
/// </summary>
public class CriticalDamageIncreaseMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CriticalDamageIncreaseMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CriticalDamageIncreaseMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.CriticalDamageIncreaseMastery;
        magicEffect.Name = "Critical Damage Increase Mastery Skill Effect";

        var critDmgIncEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.CriticalDamageIncrease);
        magicEffect.InformObservers = critDmgIncEffect.InformObservers;
        magicEffect.SubType = critDmgIncEffect.SubType;
        magicEffect.SendDuration = critDmgIncEffect.SendDuration;
        magicEffect.StopByDeath = critDmgIncEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = critDmgIncEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = critDmgIncEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in critDmgIncEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in critDmgIncEffect.PowerUpDefinitions)
        {
            var powerUpCopy = this.Context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
            powerUpCopy.TargetAttribute = powerUp.TargetAttribute!.GetPersistent(this.GameConfiguration);
            powerUpCopy.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpCopy.Boost.ConstantValue.Value = powerUp.Boost!.ConstantValue.Value;

            foreach (var boostRelatedValue in powerUp.Boost.RelatedValues)
            {
                var boostRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
                boostRelatedValueCopy.InputAttribute = boostRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
                boostRelatedValueCopy.InputOperator = boostRelatedValue.InputOperator;
                boostRelatedValueCopy.InputOperand = boostRelatedValue.InputOperand;
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        var critChancePowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUpDefinition);
        critChancePowerUpDefinition.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(this.GameConfiguration);
        critChancePowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        critChancePowerUpDefinition.Boost.ConstantValue.Value = 0;
    }
}