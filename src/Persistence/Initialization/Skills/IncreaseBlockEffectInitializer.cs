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
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;

        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60f;

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 10f; // 10 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        var powerUpDefinitionPvm = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinitionPvm);
        powerUpDefinitionPvm.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);

        // one per 10 energy
        var boostPerEnergyPvm = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergyPvm.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergyPvm.InputOperator = InputOperator.Multiply;
        boostPerEnergyPvm.InputOperand = 1f / 10f;

        powerUpDefinitionPvm.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinitionPvm.Boost.ConstantValue.Value = 16f;
        powerUpDefinitionPvm.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
        powerUpDefinitionPvm.Boost.RelatedValues.Add(boostPerEnergyPvm);

        var powerUpDefinitionPvp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinitionPvp);
        powerUpDefinitionPvp.TargetAttribute = Stats.DefenseRatePvp.GetPersistent(this.GameConfiguration);

        // one per 10 energy
        var boostPerEnergyPvp = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergyPvp.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergyPvp.InputOperator = InputOperator.Multiply;
        boostPerEnergyPvp.InputOperand = 1f / 10f;

        powerUpDefinitionPvp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinitionPvp.Boost.ConstantValue.Value = 16f;
        powerUpDefinitionPvp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
        powerUpDefinitionPvp.Boost.RelatedValues.Add(boostPerEnergyPvp);
    }
}