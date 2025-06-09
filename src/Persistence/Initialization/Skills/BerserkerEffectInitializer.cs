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
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 20f; // 20 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Mana (and damage) multiplier (buff)
        var manaPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaPowerUpDefinition);
        manaPowerUpDefinition.TargetAttribute = Stats.BerserkerManaMultiplier.GetPersistent(this.GameConfiguration);
        manaPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        var manaMultiplier = this.Context.CreateNew<AttributeRelationship>();
        manaMultiplier.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        manaMultiplier.InputOperator = InputOperator.Multiply;
        manaMultiplier.InputOperand = 1f / 3000f;
        manaPowerUpDefinition.Boost.RelatedValues.Add(manaMultiplier);

        // Health (and defense) multiplier factor (debuff)
        var healthPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(healthPowerUpDefinition);
        healthPowerUpDefinition.TargetAttribute = Stats.BerserkerHealthDecrement.GetPersistent(this.GameConfiguration);
        healthPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        healthPowerUpDefinition.Boost.ConstantValue.Value = -0.4f;

        var healthMultiplier = this.Context.CreateNew<AttributeRelationship>();
        healthMultiplier.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        healthMultiplier.InputOperator = InputOperator.Multiply;
        healthMultiplier.InputOperand = 1f / 6000f;
        healthPowerUpDefinition.Boost.RelatedValues.Add(healthMultiplier);

        // Min physical damage bonus (later gets multiplied by the mana multiplier)
        var minPhysPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(minPhysPowerUpDefinition);
        minPhysPowerUpDefinition.TargetAttribute = Stats.BerserkerMinPhysDmgBonus.GetPersistent(this.GameConfiguration);
        minPhysPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        minPhysPowerUpDefinition.Boost.ConstantValue.Value = 140f;

        var minDmgPerStrengthAndAgility = this.Context.CreateNew<AttributeRelationship>();
        minDmgPerStrengthAndAgility.InputAttribute = Stats.TotalStrengthAndAgility.GetPersistent(this.GameConfiguration);
        minDmgPerStrengthAndAgility.InputOperator = InputOperator.Multiply;
        minDmgPerStrengthAndAgility.InputOperand = 1.0f / 50;
        minPhysPowerUpDefinition.Boost.RelatedValues.Add(minDmgPerStrengthAndAgility);

        // Max physical damage bonus (later gets multiplied by the mana multiplier)
        var maxPhysPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(maxPhysPowerUpDefinition);
        maxPhysPowerUpDefinition.TargetAttribute = Stats.BerserkerMaxPhysDmgBonus.GetPersistent(this.GameConfiguration);
        maxPhysPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        maxPhysPowerUpDefinition.Boost.ConstantValue.Value = 160f;

        var maxDmgPerStrengthAndAgility = this.Context.CreateNew<AttributeRelationship>();
        maxDmgPerStrengthAndAgility.InputAttribute = Stats.TotalStrengthAndAgility.GetPersistent(this.GameConfiguration);
        maxDmgPerStrengthAndAgility.InputOperator = InputOperator.Multiply;
        maxDmgPerStrengthAndAgility.InputOperand = 1.0f / 30;
        maxPhysPowerUpDefinition.Boost.RelatedValues.Add(maxDmgPerStrengthAndAgility);

        // Placeholder for the Berserker Strengthener master skill
        var strengthenerCurseDmgMultiplier = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(strengthenerCurseDmgMultiplier);
        strengthenerCurseDmgMultiplier.TargetAttribute = Stats.BerserkerCurseMultiplier.GetPersistent(this.GameConfiguration);
        strengthenerCurseDmgMultiplier.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        // Placeholder for the Berserker Proficiency master skill
        var proficiencyDmgMultiplier = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(proficiencyDmgMultiplier);
        proficiencyDmgMultiplier.TargetAttribute = Stats.BerserkerProficiencyMultiplier.GetPersistent(this.GameConfiguration);
        proficiencyDmgMultiplier.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
    }
}