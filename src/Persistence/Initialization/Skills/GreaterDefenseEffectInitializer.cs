// <copyright file="GreaterDefenseEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the greater defense buff effect.
/// </summary>
public class GreaterDefenseEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterDefenseEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GreaterDefenseEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.GreaterDefense;
        magicEffect.Name = "Greater Defense Buff Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60; // 60 Seconds

        // The buff gives 2 + (energy / 8) defense
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.DefenseFinal.GetPersistent(this.GameConfiguration);
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 2f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;

        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 8f; // one defense per 8 energy
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
    }
}