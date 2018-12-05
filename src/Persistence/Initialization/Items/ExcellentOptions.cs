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
        public static readonly string DefenseOptionsName = "Excellent Defense Options";

        /// <summary>
        /// The name of the <see cref="ItemOptionDefinition"/> of excellent physical attack options.
        /// </summary>
        public static readonly string PhysicalAttackOptionsName = "Excellent Physical Attack Options";

        /// <summary>
        /// The name of the <see cref="ItemOptionDefinition"/> of excellent wizardry attack options.
        /// </summary>
        public static readonly string WizardryAttackOptionsName = "Excellent Wizardry Attack Options";

        /// <summary>
        /// The name of the <see cref="ItemOptionDefinition"/> of excellent curse attack options.
        /// </summary>
        public static readonly string CurseAttackOptionsName = "Excellent Curse Attack Options";

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
            this.CreatePhysicalAttackOptions();
            this.CreateWizardryAttackOptions();
            this.CreateCurseAttackOptions();
        }

        private void CreateCurseAttackOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = CurseAttackOptionsName;
            definition.AddChance = 0.001f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 2;

            definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeed, 7, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumCurseBaseDmg, 1.02f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(5, Stats.MaximumCurseBaseDmgPer20LevelItemCount, 1, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw));
        }

        private void CreateWizardryAttackOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = WizardryAttackOptionsName;
            definition.AddChance = 0.001f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 2;

            definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeed, 7, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumWizBaseDmg, 1.02f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(5, Stats.MaximumWizBaseDmgPer20LevelItemCount, 1, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw));
        }

        private void CreatePhysicalAttackOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = PhysicalAttackOptionsName;
            definition.AddChance = 0.001f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 2;

            definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.ManaAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.HealthAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.AttackSpeed, 7, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.MaximumPhysBaseDmg, 1.02f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(5, Stats.MaximumPhysBaseDmgPer20LevelItemCount, 1, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw));
        }

        private void CreateDefenseOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = DefenseOptionsName;
            definition.AddChance = 0.001f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 2;

            definition.PossibleOptions.Add(this.CreateExcellentOption(1, Stats.MoneyAmountRate, 1.4f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(2, Stats.DefenseRatePvm, 1.1f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(3, Stats.DamageReflection, 0.4f, AggregateType.AddRaw));
            definition.PossibleOptions.Add(this.CreateExcellentOption(4, Stats.DamageReceiveDecrement, 0.96f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(5, Stats.MaximumMana, 1.04f, AggregateType.Multiplicate));
            definition.PossibleOptions.Add(this.CreateExcellentOption(6, Stats.MaximumHealth, 1.04f, AggregateType.Multiplicate));
        }

        private IncreasableItemOption CreateExcellentOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
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
