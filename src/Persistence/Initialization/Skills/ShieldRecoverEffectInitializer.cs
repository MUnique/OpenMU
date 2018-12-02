// <copyright file="ShieldRecoverEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
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
            magicEffect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
            magicEffect.PowerUpDefinition.TargetAttribute = Stats.CurrentHealth.GetPersistent(this.GameConfiguration);

            // The effect gives: level + (energy / 4) shield
            magicEffect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Boost.ConstantValue.Value = 0f;
            magicEffect.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

            var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
            boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
            boostPerEnergy.InputOperator = InputOperator.Multiply;
            boostPerEnergy.InputOperand = 1f / 4f; // one shield per 4 energy
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);

            var boostPerLevel = this.Context.CreateNew<AttributeRelationship>();
            boostPerLevel.InputAttribute = Stats.Level.GetPersistent(this.GameConfiguration);
            boostPerLevel.InputOperator = InputOperator.Multiply;
            boostPerLevel.InputOperand = 1f; // one shield per level
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerLevel);
        }
    }
}