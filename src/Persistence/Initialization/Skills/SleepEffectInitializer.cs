// <copyright file="SleepEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the sleep effect.
/// </summary>
public class SleepEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SleepEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SleepEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Sleep;
        magicEffect.Name = "Sleep Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.DurationDependsOnTargetLevel = true;
        magicEffect.MonsterTargetLevelDivisor = 20;
        magicEffect.PlayerTargetLevelDivisor = 100;

        // Chance % = 20 + (Energy / 30) + (Book Rise / 6)
        magicEffect.Chance = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.2f; // 20%

        var chancePerEnergy = this.Context.CreateNew<AttributeRelationship>();
        chancePerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        chancePerEnergy.InputOperator = InputOperator.Multiply;
        chancePerEnergy.InputOperand = 1f / 3000f; // 30 energy adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerEnergy);

        var chancePerBookRise = this.Context.CreateNew<AttributeRelationship>();
        chancePerBookRise.InputAttribute = Stats.BookRise.GetPersistent(this.GameConfiguration);
        chancePerBookRise.InputOperator = InputOperator.Multiply;
        chancePerBookRise.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.Chance.RelatedValues.Add(chancePerBookRise);

        // Chance PvP % = 15 + (Energy / 37) + (Book Rise / 6)
        magicEffect.ChancePvp = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.ChancePvp.ConstantValue.Value = 0.15f; // 15%

        var chancePerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        chancePerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        chancePerEnergyPvp.InputOperator = InputOperator.Multiply;
        chancePerEnergyPvp.InputOperand = 1f / 3700f; // 37 energy adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerEnergyPvp);

        var chancePerBookRisePvp = this.Context.CreateNew<AttributeRelationship>();
        chancePerBookRisePvp.InputAttribute = Stats.BookRise.GetPersistent(this.GameConfiguration);
        chancePerBookRisePvp.InputOperator = InputOperator.Multiply;
        chancePerBookRisePvp.InputOperand = 1f / 600f; // 6 book rise adds 1% chance
        magicEffect.ChancePvp.RelatedValues.Add(chancePerBookRisePvp);

        // Duration = 5 + (Energy / 100)
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 5; // 5 Seconds
        magicEffect.Duration.MaximumValue = 20; // 20 Seconds

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 100f; // 100 energy adds 1s
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Duration = 4 + (Energy / 250) + ((Level - Target's Level) / 100)
        magicEffect.DurationPvp = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.DurationPvp.ConstantValue.Value = 4; // 4 Seconds
        magicEffect.DurationPvp.MaximumValue = 10; // 10 Seconds

        var durationPerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergyPvp.InputOperator = InputOperator.Multiply;
        durationPerEnergyPvp.InputOperand = 1f / 250f; // 250 energy adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerEnergyPvp);

        var durationPerLevelPvp = this.Context.CreateNew<AttributeRelationship>();
        durationPerLevelPvp.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
        durationPerLevelPvp.InputOperator = InputOperator.Multiply;
        durationPerLevelPvp.InputOperand = 1f / 100f; // 100 levels adds 1s
        magicEffect.DurationPvp.RelatedValues.Add(durationPerLevelPvp);

        var isAsleep = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(isAsleep);
        isAsleep.TargetAttribute = Stats.IsAsleep.GetPersistent(this.GameConfiguration);
        isAsleep.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        isAsleep.Boost.ConstantValue.Value = 1;
    }
}