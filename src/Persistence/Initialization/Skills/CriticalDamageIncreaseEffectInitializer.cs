// <copyright file="CriticalDamageIncreaseEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the critical damage increase effect.
/// </summary>
public class CriticalDamageIncreaseEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CriticalDamageIncreaseEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CriticalDamageIncreaseEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.CriticalDamageIncrease;
        magicEffect.Name = "Critical Damage Increase Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60f;

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 10f; // 10 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.CriticalDamageBonus.GetPersistent(this.GameConfiguration);

        // one damage per 30 energy
        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 30f;

        // one damage per 25 leadership
        var boostPerLeadership = this.Context.CreateNew<AttributeRelationship>();
        boostPerLeadership.InputAttribute = Stats.TotalLeadership.GetPersistent(this.GameConfiguration);
        boostPerLeadership.InputOperator = InputOperator.Multiply;
        boostPerLeadership.InputOperand = 1f / 25f;

        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
        powerUpDefinition.Boost.RelatedValues.Add(boostPerLeadership);
    }
}