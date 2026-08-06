// <copyright file="IncreaseBlockMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the increase block mastery effect.
/// </summary>
public class IncreaseBlockMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseBlockMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseBlockMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockMastery;
        magicEffect.Name = "Increase Block Mastery Skill Effect";

        var increaseBlockPowerUpEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IncreaseBlockPowerUp);
        magicEffect.InformObservers = increaseBlockPowerUpEffect.InformObservers;
        magicEffect.SubType = increaseBlockPowerUpEffect.SubType;
        magicEffect.SendDuration = increaseBlockPowerUpEffect.SendDuration;
        magicEffect.StopByDeath = increaseBlockPowerUpEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = increaseBlockPowerUpEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = increaseBlockPowerUpEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in increaseBlockPowerUpEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in increaseBlockPowerUpEffect.PowerUpDefinitions)
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
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.DefenseFinal.GetPersistent(this.GameConfiguration);
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 0f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
    }
}