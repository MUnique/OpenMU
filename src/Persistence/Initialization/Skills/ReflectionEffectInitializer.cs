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

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Reflection;
        magicEffect.Name = "Reflection Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;

        // Duration = 30 + (Energy / 24)
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 30; // 30 Seconds
        magicEffect.Duration.MaximumValue = 180;

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 24; // 24 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Reflection % = 30 + (Energy / 42)
        var incReflectPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(incReflectPowerUpDefinition);
        incReflectPowerUpDefinition.TargetAttribute = Stats.DamageReflection.GetPersistent(this.GameConfiguration);
        incReflectPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        incReflectPowerUpDefinition.Boost.ConstantValue.Value = 0.3f; // 30% increase
        incReflectPowerUpDefinition.Boost.MaximumValue = 0.6f;

        var incReflectPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        incReflectPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        incReflectPerEnergy.InputOperator = InputOperator.Multiply;
        incReflectPerEnergy.InputOperand = 1f / 4200f; // 42 energy further increases 0.01
        incReflectPowerUpDefinition.Boost.RelatedValues.Add(incReflectPerEnergy);
    }
}