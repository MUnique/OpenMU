// <copyright file="ReflectionEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the reflection buff effect.
/// </summary>
public class ReflectionEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ReflectionEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.Reflection;
        magicEffect.Name = "Reflection";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.DamageReflection.GetPersistent(this.GameConfiguration);
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60;
        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 5f; // 5 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // one percent per 42 energy
        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.ExponentiateByAttribute;
        boostPerEnergy.InputOperand = 1 - (0.01f / 42f);

        // Reflection % = 30 + (Energi / 42)
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 0.29f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
    }
}