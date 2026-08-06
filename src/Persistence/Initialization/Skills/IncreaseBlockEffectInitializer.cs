// <copyright file="IncreaseBlockEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the increase block effect.
/// </summary>
public class IncreaseBlockEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseBlockEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseBlockEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlock;
        magicEffect.Name = "Increase Block Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SubType = 74;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60f;
        magicEffect.Duration.MaximumValue = 180f;

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 5f; // 5 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // The buff gives 2 + (energy / 10) defense rate (PvM)
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(this.GameConfiguration);
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 2f;   // The parchment requires 80 energy => base value = 10
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
        powerUpDefinition.Boost.MaximumValue = 100f;

        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 10f; // one defense rate per 10 energy
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);

        // Only increases with DefSuccessRateIncPowUp master skill, but we need to nullify the AggregateType.AddRaw values until then
        var powerUpDefinition2 = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition2);
        powerUpDefinition2.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(this.GameConfiguration);
        powerUpDefinition2.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition2.Boost.ConstantValue.Value = 0f;
        powerUpDefinition2.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}