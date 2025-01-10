// <copyright file="BerserkerEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the berserker buff effect.
/// </summary>
public class BerserkerEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BerserkerEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BerserkerEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Berserker;
        magicEffect.Name = "Berserker Buff Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 30; // 30 Seconds

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.BaseEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 20f; // 20 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Effect 1: mana (and damage) multiplier (increase)
        var manaPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaPowerUpDefinition);
        manaPowerUpDefinition.TargetAttribute = Stats.BerserkerManaMultiplier.GetPersistent(this.GameConfiguration);
        manaPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        var manaMultiplier = this.Context.CreateNew<AttributeRelationship>();
        manaMultiplier.InputAttribute = Stats.BaseEnergy.GetPersistent(this.GameConfiguration);
        manaMultiplier.InputOperator = InputOperator.Multiply;
        manaMultiplier.InputOperand = 1f / 3000f;
        manaPowerUpDefinition.Boost.RelatedValues.Add(manaMultiplier);

        // Effect 2: health multiplier (decrease)
        var healthPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(healthPowerUpDefinition);
        healthPowerUpDefinition.TargetAttribute = Stats.BerserkerHealthMultiplier.GetPersistent(this.GameConfiguration);
        healthPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        var healthMultiplier = this.Context.CreateNew<AttributeRelationship>();
        healthMultiplier.InputAttribute = Stats.BaseEnergy.GetPersistent(this.GameConfiguration);
        healthMultiplier.InputOperator = InputOperator.Multiply;
        healthMultiplier.InputOperand = 1f / 6000f;
        healthPowerUpDefinition.Boost.RelatedValues.Add(healthMultiplier);
    }
}