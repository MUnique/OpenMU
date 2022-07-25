// <copyright file="ShieldRecoverEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the shield recover effect.
/// </summary>
public class ShieldRecoverEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShieldRecoverEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ShieldRecoverEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.ShieldRecover;
        magicEffect.Name = "Shield Recover Effect";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.CurrentShield.GetPersistent(this.GameConfiguration);

        // The effect gives: level + (energy / 4) shield
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 0f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 4f; // one shield per 4 energy
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);

        var boostPerLevel = this.Context.CreateNew<AttributeRelationship>();
        boostPerLevel.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
        boostPerLevel.InputOperator = InputOperator.Multiply;
        boostPerLevel.InputOperand = 1f; // one shield per level
        powerUpDefinition.Boost.RelatedValues.Add(boostPerLevel);
    }
}