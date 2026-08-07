// <copyright file="IncreaseBlockPowerUpEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the increase block power up effect.
/// </summary>
public class IncreaseBlockPowerUpEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseBlockPowerUpEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseBlockPowerUpEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockPowerUp;
        magicEffect.Name = "Increase Block Power Up Skill Effect";

        var increaseBlockEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IncreaseBlock);
        magicEffect.InformObservers = increaseBlockEffect.InformObservers;
        magicEffect.SubType = increaseBlockEffect.SubType;
        magicEffect.SendDuration = increaseBlockEffect.SendDuration;
        magicEffect.StopByDeath = increaseBlockEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = increaseBlockEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = increaseBlockEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in increaseBlockEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in increaseBlockEffect.PowerUpDefinitions)
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

        var increaseBlockPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(increaseBlockPowerUp);
        increaseBlockPowerUp.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(this.GameConfiguration);
        increaseBlockPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseBlockPowerUp.Boost.ConstantValue.Value = 0f;
        increaseBlockPowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
    }
}