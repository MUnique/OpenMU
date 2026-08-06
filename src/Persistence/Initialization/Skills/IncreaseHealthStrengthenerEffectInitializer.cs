// <copyright file="IncreaseHealthStrengthenerEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializer which initializes the increase health (stamina) strengthener effect.
/// </summary>
public class IncreaseHealthStrengthenerEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseHealthStrengthenerEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseHealthStrengthenerEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseHealthStrengthener;
        magicEffect.Name = "Increase Health Strengthener Skill Effect";

        var increaseHealthEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IncreaseHealth);
        magicEffect.InformObservers = increaseHealthEffect.InformObservers;
        magicEffect.SubType = increaseHealthEffect.SubType;
        magicEffect.SendDuration = increaseHealthEffect.SendDuration;
        magicEffect.StopByDeath = increaseHealthEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = increaseHealthEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = increaseHealthEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in increaseHealthEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in increaseHealthEffect.PowerUpDefinitions)
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
    }
}