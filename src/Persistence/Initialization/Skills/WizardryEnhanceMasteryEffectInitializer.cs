// <copyright file="WizardryEnhanceMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the wizardry enhance mastery effect.
/// </summary>
public class WizardryEnhanceMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WizardryEnhanceMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public WizardryEnhanceMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.WizEnhanceMastery;
        magicEffect.Name = "Wizardry Enhance Mastery Skill Effect";

        var wizardryEnhanceEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.WizEnhanceStrengthener);
        magicEffect.InformObservers = wizardryEnhanceEffect.InformObservers;
        magicEffect.SubType = wizardryEnhanceEffect.SubType;
        magicEffect.SendDuration = wizardryEnhanceEffect.SendDuration;
        magicEffect.StopByDeath = wizardryEnhanceEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = wizardryEnhanceEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = wizardryEnhanceEffect.Duration.MaximumValue;

        foreach (var powerUp in wizardryEnhanceEffect.PowerUpDefinitions)
        {
            var powerUpCopy = this.Context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
            powerUpCopy.TargetAttribute = powerUp.TargetAttribute!.GetPersistent(this.GameConfiguration);
            powerUpCopy.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpCopy.Boost.ConstantValue.Value = powerUp.Boost!.ConstantValue.Value;
            powerUpCopy.Boost.ConstantValue.AggregateType = powerUp.Boost.ConstantValue.AggregateType;
            powerUpCopy.Boost.MaximumValue = powerUp.Boost.MaximumValue;

            foreach (var boostRelatedValue in powerUp.Boost.RelatedValues)
            {
                var boostRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
                boostRelatedValueCopy.InputAttribute = boostRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
                boostRelatedValueCopy.InputOperator = boostRelatedValue.InputOperator;
                boostRelatedValueCopy.InputOperand = boostRelatedValue.InputOperand;
                boostRelatedValueCopy.AggregateType = boostRelatedValue.AggregateType;
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        var critChancePowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUp);
        critChancePowerUp.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(this.GameConfiguration);
        critChancePowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
    }
}