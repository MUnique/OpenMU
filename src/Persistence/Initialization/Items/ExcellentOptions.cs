// <copyright file="ExcellentOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for excellent options.
    /// </summary>
    public class ExcellentOptions : InitializerBase
    {
        /// <summary>
        /// The name of the <see cref="ItemOptionDefinition"/> of excellent defense options.
        /// </summary>
        public const string DefenseOptionsName = "Excellent Defense Options";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcellentOptions"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ExcellentOptions(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            this.CreateDefenseOptions();

            // TODO: Attack options
        }

        private void CreateDefenseOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = DefenseOptionsName;
            definition.AddChance = 0.001f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 2;

            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(1, Stats.MoneyAmountRate, 1.4f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(2, Stats.DefenseRatePvm, 1.1f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(3, Stats.DamageReflection, 0.4f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(4, Stats.DamageReceiveDecrement, 0.96f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(5, Stats.MaximumMana, 1.04f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(6, Stats.MaximumHealth, 1.04f, AggregateType.Multiplicate));
        }

        private IncreasableItemOption CreateExcellentDefenseOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Excellent);
            itemOption.Number = number;
            itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
            itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            return itemOption;
        }
    }
}
