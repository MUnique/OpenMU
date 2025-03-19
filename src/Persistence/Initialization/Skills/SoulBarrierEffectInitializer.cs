// <copyright file="SoulBarrierEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the soul barrier buff effect.
/// </summary>
public class SoulBarrierEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoulBarrierEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SoulBarrierEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.SoulBarrier;
        magicEffect.Name = "Soul Barrier";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60;

        var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        durationPerEnergy.InputOperator = InputOperator.Multiply;
        durationPerEnergy.InputOperand = 1f / 40f; // 40 energy adds 1 second duration
        magicEffect.Duration.RelatedValues.Add(durationPerEnergy);

        // Soul barrier dmg decrease % = 10 + (Agility/50) + (Energy/200)
        var damageDecrement = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(damageDecrement);
        damageDecrement.TargetAttribute = Stats.SoulBarrierReceiveDecrement.GetPersistent(this.GameConfiguration);
        damageDecrement.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        damageDecrement.Boost.ConstantValue.Value = 0.1f;

        var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 20000f;  // one percent per 200 energy
        damageDecrement.Boost.RelatedValues.Add(boostPerEnergy);

        var boostPerAgility = this.Context.CreateNew<AttributeRelationship>();
        boostPerAgility.InputAttribute = Stats.TotalAgility.GetPersistent(this.GameConfiguration);
        boostPerAgility.InputOperator = InputOperator.Multiply;
        boostPerAgility.InputOperand = 1f / 5000f; // one percent per 50 agility
        damageDecrement.Boost.RelatedValues.Add(boostPerAgility);

        // Mana toll = 2% + 0.1% per SB strengthener level (added in skill action plugin)
        var manaTollPerHit = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaTollPerHit);
        manaTollPerHit.TargetAttribute = Stats.SoulBarrierManaTollPerHit.GetPersistent(this.GameConfiguration);
        manaTollPerHit.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();

        var manaToll = this.Context.CreateNew<AttributeRelationship>();
        manaToll.InputAttribute = Stats.MaximumMana.GetPersistent(this.GameConfiguration);
        manaToll.InputOperator = InputOperator.Multiply;
        manaToll.InputOperand = 0.02f; // two percent of total mana
        manaTollPerHit.Boost.RelatedValues.Add(manaToll);
    }
}