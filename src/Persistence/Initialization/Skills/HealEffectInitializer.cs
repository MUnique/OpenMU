// <copyright file="HealEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for the heal effect.
    /// </summary>
    public class HealEffectInitializer : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealEffectInitializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public HealEffectInitializer(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
            this.GameConfiguration.MagicEffects.Add(magicEffect);
            magicEffect.Number = (short)MagicEffectNumber.Heal;
            magicEffect.Name = "Heal Effect";
            magicEffect.InformObservers = false;
            magicEffect.SendDuration = false;
            magicEffect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
            magicEffect.PowerUpDefinition.TargetAttribute = Stats.CurrentHealth.GetPersistent(this.GameConfiguration);

            // The effect gives 5 + (energy / 5) health
            magicEffect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Boost.ConstantValue.Value = 5f;
            magicEffect.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

            var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
            boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
            boostPerEnergy.InputOperator = InputOperator.Multiply;
            boostPerEnergy.InputOperand = 1f / 5f; // one health per 5 energy
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
        }
    }
}
