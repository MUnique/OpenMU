// <copyright file="LifeSwellProficiencyEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the life swell proficiency effect.
/// </summary>
public class LifeSwellProficiencyEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LifeSwellProficiencyEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public LifeSwellProficiencyEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.GreaterFortitude2;
        magicEffect.Name = "Life Swell Proficiency Skill Effect";

        var lifeSwellEffect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.GreaterFortitude);
        magicEffect.InformObservers = lifeSwellEffect.InformObservers;
        magicEffect.SubType = lifeSwellEffect.SubType;
        magicEffect.SendDuration = lifeSwellEffect.SendDuration;
        magicEffect.StopByDeath = lifeSwellEffect.StopByDeath;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = lifeSwellEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = lifeSwellEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in lifeSwellEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = this.Context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(this.GameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in lifeSwellEffect.PowerUpDefinitions)
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

        // one percent per party member in view
        var boostPerPartyMember = this.Context.CreateNew<AttributeRelationship>();
        boostPerPartyMember.InputAttribute = Stats.NearbyPartyMemberCount.GetPersistent(this.GameConfiguration);
        boostPerPartyMember.InputOperator = InputOperator.Multiply;
        boostPerPartyMember.InputOperand = 1f / 100;

        var manaPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaPowerUpDefinition);
        manaPowerUpDefinition.TargetAttribute = Stats.SwellLifeManaIncrease.GetPersistent(this.GameConfiguration);
        manaPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        manaPowerUpDefinition.Boost.RelatedValues.Add(boostPerPartyMember);
    }
}