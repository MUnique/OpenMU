// <copyright file="LifeSwellEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer which initializes the greater fortitude effect.
    /// </summary>
    public class LifeSwellEffectInitializer : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifeSwellEffectInitializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public LifeSwellEffectInitializer(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
            this.GameConfiguration.MagicEffects.Add(magicEffect);
            magicEffect.Number = (byte)MagicEffectNumber.GreaterFortitude;
            magicEffect.Name = "Life Swell Skill Effect";
            magicEffect.InformObservers = true;
            magicEffect.SendDuration = false;
            magicEffect.StopByDeath = true;
            magicEffect.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinitionWithDuration>();
            magicEffect.PowerUpDefinition.TargetAttribute = Stats.MaximumHealth.GetPersistent(this.GameConfiguration);
            var durationPerEnergy = this.Context.CreateNew<AttributeRelationship>();
            durationPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
            durationPerEnergy.InputOperator = InputOperator.Multiply;
            durationPerEnergy.InputOperand = 1f / 5f; // 5 energy adds 1 second duration
            magicEffect.PowerUpDefinition.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Duration.ConstantValue.Value = 60f;
            magicEffect.PowerUpDefinition.Duration.RelatedValues.Add(durationPerEnergy);

            // one percent per 20 energy
            var boostPerEnergy = this.Context.CreateNew<AttributeRelationship>();
            boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
            boostPerEnergy.InputOperator = InputOperator.Multiply;
            boostPerEnergy.InputOperand = 1f + (0.01f / 20f);

            // one percent per 100 vitality
            var boostPerVitality = this.Context.CreateNew<AttributeRelationship>();
            boostPerVitality.InputAttribute = Stats.TotalVitality.GetPersistent(this.GameConfiguration);
            boostPerVitality.InputOperator = InputOperator.Multiply;
            boostPerVitality.InputOperand = 1f + (0.01f / 100f);

            magicEffect.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            magicEffect.PowerUpDefinition.Boost.ConstantValue.Value = 1.12f;
            magicEffect.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);
            magicEffect.PowerUpDefinition.Boost.RelatedValues.Add(boostPerVitality);
        }
    }
}
