// <copyright file="GreaterDamageEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for the greater damage buff effect.
    /// </summary>
    public class GreaterDamageEffectInitializer : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterDamageEffectInitializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public GreaterDamageEffectInitializer(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
            this.GameConfiguration.MagicEffects.Add(magicEffect);
            magicEffect.Number = (short)MagicEffectNumber.GreaterDamage;
            magicEffect.Name = "Greater Damage Buff Skill Effect";
            magicEffect.InformObservers = true;
            magicEffect.SendDuration = false;
            magicEffect.StopByDeath = true;
            magicEffect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
            magicEffect.PowerUpDefinition.TargetAttribute = Stats.BaseDamageBonus.GetPersistent(this.GameConfiguration);
            magicEffect.PowerUpDefinition.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Duration.ConstantValue.Value = 60; // 60 Seconds

            // The buff gives 3 + (energy / 7) damage
            magicEffect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Boost.ConstantValue.Value = 3f;
            magicEffect.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

            var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
            boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
            boostPerEnergy.InputOperator = InputOperator.Multiply;
            boostPerEnergy.InputOperand = 1f / 7f; // one damage per 7 energy
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
        }
    }
}
