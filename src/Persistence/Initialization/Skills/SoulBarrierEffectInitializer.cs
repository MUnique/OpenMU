﻿// <copyright file="SoulBarrierEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the soul barrier buff effect.
/// </summary>
public class SoulBarrierEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoulBarrierEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SoulBarrierEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.SoulBarrier;
        magicEffect.Name = "Soul Barrier";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
        magicEffect.PowerUpDefinition.TargetAttribute = Stats.DamageReceiveDecrement.GetPersistent(this.GameConfiguration);
        magicEffect.PowerUpDefinition.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.PowerUpDefinition.Duration.ConstantValue.Value = 60;
        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 5f; // 5 energy adds 1 second duration
        magicEffect.PowerUpDefinition.Duration.RelatedValues.Add(durationPerEnergy);

        // one percent per 200 energy
        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = -0.01f / 200f;

        // one percent per 50 agility
        var boostPerAgility = this.Context.CreateNew<AttributeRelationship>();
        boostPerAgility.InputAttribute = Stats.TotalAgility.GetPersistent(this.GameConfiguration);
        boostPerAgility.InputOperator = InputOperator.Multiply;
        boostPerAgility.InputOperand = -0.01f / 50f;

        // Soul barrier % = 10 + (Agility/50) + (Energy/200)
        magicEffect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.PowerUpDefinition.Boost.ConstantValue.Value = -0.10f;
        magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
        magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerAgility);
    }
}