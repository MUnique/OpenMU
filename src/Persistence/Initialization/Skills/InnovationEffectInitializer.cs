// <copyright file="InnovationEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the innovation effect.
/// </summary>
public class InnovationEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnovationEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public InnovationEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Innovation;
        magicEffect.Name = "Innovation Effect";
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

        // Chance PvP % = 17 + (Energy / 50) + (Book Rise / 6)
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
        magicEffect.Duration.MaximumValue = 44; // 44 Seconds (based on 4k total energy cap)

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Duration PvP = 5 + (Energy / 300) + ((Level - Target's Level) / 150)
        magicEffect.DurationPvp = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.DurationPvp.MaximumValue = 18; // 18 Seconds (based on 4k total energy cap)

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

        // Defense decrease % (applies last) = 20 + (Energy / 90)
        var decDefPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDefPowerUpDefinition);
        decDefPowerUpDefinition.TargetAttribute = Stats.InnovationDefDecrement.GetPersistent(this.GameConfiguration);
        decDefPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDefPowerUpDefinition.Boost.ConstantValue.Value = 0.20f; // 20% decrease
        decDefPowerUpDefinition.Boost.MaximumValue = 0.64f; // 64% decrease (based on 4k total energy cap)

        var decDefPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        decDefPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        decDefPerEnergy.InputOperator = InputOperator.Multiply;
        decDefPerEnergy.InputOperand = 1f / 9000f; // 90 energy further decreases 0.01
        decDefPowerUpDefinition.Boost.RelatedValues.Add(decDefPerEnergy);

        // Defense decrease PvP % (applies last) = 12 + (Energy / 110)
        var decDefPowerUpDefinitionPvp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDefPowerUpDefinitionPvp);
        decDefPowerUpDefinitionPvp.TargetAttribute = Stats.InnovationDefDecrement.GetPersistent(this.GameConfiguration);
        decDefPowerUpDefinitionPvp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDefPowerUpDefinitionPvp.Boost.ConstantValue.Value = 0.12f; // 12% decrease
        decDefPowerUpDefinitionPvp.Boost.MaximumValue = 0.48f; // 48% decrease (based on 4k total energy cap)

        var decDefPerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        decDefPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        decDefPerEnergyPvp.InputOperator = InputOperator.Multiply;
        decDefPerEnergyPvp.InputOperand = 1f / 11000f; // 110 energy further decreases 0.01
        decDefPowerUpDefinitionPvp.Boost.RelatedValues.Add(decDefPerEnergyPvp);
    }
}