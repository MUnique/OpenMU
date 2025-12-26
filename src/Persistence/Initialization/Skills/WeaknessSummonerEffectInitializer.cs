// <copyright file="WeaknessSummonerEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the weakness effect which results from Weakness (Summoner) skill.
/// </summary>
public class WeaknessSummonerEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeaknessSummonerEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public WeaknessSummonerEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.WeaknessSummoner;
        magicEffect.Name = "Weakness Effect (Summoner)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.DurationDependsOnTargetLevel = true;
        magicEffect.MonsterTargetLevelDivisor = 20;
        magicEffect.PlayerTargetLevelDivisor = 150;

        // Chance % = 32 + (Energy / 50) + (Book Rise / 6)
        magicEffect.Chance = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.32f; // 32%

        var chancePerEnergy = this.Context.CreateNew<AttributeRelationship>();
        chancePerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        chancePerEnergy.InputOperator = InputOperator.Multiply;
        chancePerEnergy.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerEnergy);

        var chancePerBookRise = this.Context.CreateNew<AttributeRelationship>();
        chancePerBookRise.InputAttribute = Stats.BookRise.GetPersistent(this.GameConfiguration);
        chancePerBookRise.InputOperator = InputOperator.Multiply;
        chancePerBookRise.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerBookRise);

        // Chance % = 17 + (Energy / 50) + (Book Rise / 6)
        magicEffect.ChancePvp = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.ChancePvp.ConstantValue.Value = 0.17f; // 17%

        var chancePerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        chancePerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        chancePerEnergyPvp.InputOperator = InputOperator.Multiply;
        chancePerEnergyPvp.InputOperand = 1f / 5000f; // 50 energy adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerEnergyPvp);

        var chancePerBookRisePvp = this.Context.CreateNew<AttributeRelationship>();
        chancePerBookRisePvp.InputAttribute = Stats.BookRise.GetPersistent(this.GameConfiguration);
        chancePerBookRisePvp.InputOperator = InputOperator.Multiply;
        chancePerBookRisePvp.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerBookRisePvp);

        // Duration = 4 + (Energy / 100)
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 4; // 4 Seconds
        magicEffect.Duration.MaximumValue = 100; // 100 Seconds

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Duration = 5 + (Energy / 300) + ((Level - Target's Level) / 150)
        magicEffect.DurationPvp = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.DurationPvp.MaximumValue = 20; // 20 Seconds

        var durationPerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergyPvp.InputOperator = InputOperator.Multiply;
        durationPerEnergyPvp.InputOperand = 1f / 300f; // 300 energy adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerEnergyPvp);

        var durationPerLevelPvp = this.Context.CreateNew<AttributeRelationship>();
        durationPerLevelPvp.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
        durationPerLevelPvp.InputOperator = InputOperator.Multiply;
        durationPerLevelPvp.InputOperand = 1f / 150f; // 150 levels adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerLevelPvp);

        // Phys damage decrease % = 4 + (Energy / 58)
        var decDmgPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDmgPowerUpDefinition);
        decDmgPowerUpDefinition.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(this.GameConfiguration);
        decDmgPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinition.Boost.ConstantValue.Value = 0.04f; // 4% decrease
        decDmgPowerUpDefinition.Boost.MaximumValue = 0.73f; // based on 4k total energy cap -- to-do: check zTeam

        var decDmgPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        decDmgPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        decDmgPerEnergy.InputOperator = InputOperator.Multiply;
        decDmgPerEnergy.InputOperand = 1f / 5800f; // 58 energy further decreases 0.01
        decDmgPowerUpDefinition.Boost.RelatedValues.Add(decDmgPerEnergy);

        // Phys damage decrease PvP % = 3 + (Energy / 93)
        var decDmgPowerUpDefinitionPvp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDmgPowerUpDefinitionPvp);
        decDmgPowerUpDefinitionPvp.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(this.GameConfiguration);
        decDmgPowerUpDefinitionPvp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinitionPvp.Boost.ConstantValue.Value = 0.03f; // 3% decrease
        decDmgPowerUpDefinitionPvp.Boost.MaximumValue = 1f; // -- to-do: check zTeam

        var decDmgPerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        decDmgPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        decDmgPerEnergyPvp.InputOperator = InputOperator.Multiply;
        decDmgPerEnergyPvp.InputOperand = 1f / 9300f; // 93 energy further decreases 0.01
        decDmgPowerUpDefinitionPvp.Boost.RelatedValues.Add(decDmgPerEnergyPvp);
    }
}