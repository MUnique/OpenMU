// <copyright file="Jewellery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for jewellery (rings and pendants).
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
    internal class Jewellery : InitializerBase
    {
        private ItemOptionDefinition? healthRecoverOptionDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="Jewellery"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Jewellery(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            this.healthRecoverOptionDefinition = this.CreateOption("Health recover for jewellery", Stats.HealthRecoveryMultiplier, 0.01f);

            this.CreateRing(8, "Ring of Ice", 20, 50, Stats.IceResistance);
            this.CreateRing(9, "Ring of Poison", 17, 50, Stats.PoisonResistance);

            this.CreatePendant(12, "Pendant of Lighting", 21, 50, Stats.LightningResistance);
            this.CreatePendant(13, "Pendant of Fire", 13, 50, Stats.FireResistance);

            /* TODO: Transformation feature
             * Idea: There could be an attribute "IsTransformed" which we could handle in the WorldView. These rings here could just add a value to this attribute.
            this.CreateJewelery(10, 10, false, "Transformation Ring", 0, 200, null, null, null);
            */
        }

        private ItemOptionDefinition CreateOption(string name, AttributeDefinition targetAttribute, float boostPerLevel, AggregateType aggregateType = AggregateType.AddRaw)
        {
            var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
            optionDefinition.AddsRandomly = true;
            optionDefinition.Name = name;
            optionDefinition.AddChance = 0.25f;
            optionDefinition.MaximumOptionsPerItem = 1;

            var option = this.Context.CreateNew<IncreasableItemOption>();
            option.OptionType = ItemOptionTypes.Option;
            option.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            option.PowerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(this.GameConfiguration);
            for (int i = 1; i <= 4; i++)
            {
                var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = i;
                optionOfLevel.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
                optionOfLevel.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
                optionOfLevel.PowerUpDefinition.Boost.ConstantValue.Value = boostPerLevel * i;
                optionOfLevel.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
                optionOfLevel.PowerUpDefinition.TargetAttribute = option.PowerUpDefinition.TargetAttribute;
                option.LevelDependentOptions.Add(optionOfLevel);
            }

            optionDefinition.PossibleOptions.Add(option);
            this.GameConfiguration.ItemOptions.Add(optionDefinition);
            return optionDefinition;
        }

        /// <summary>
        /// Creates a ring.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="name">The name.</param>
        /// <param name="level">The level.</param>
        /// <param name="durability">The durability.</param>
        /// <param name="resistanceAttribute">The resistance attribute.</param>
        /// <remarks>
        /// Rings always have defensive excellent options.
        /// </remarks>
        private void CreateRing(byte number, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
        {
            this.CreateJewelery(number, 10, true, name, level, durability, resistanceAttribute);
        }

        /// <summary>
        /// Creates a pendant.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="name">The name.</param>
        /// <param name="level">The level.</param>
        /// <param name="durability">The durability.</param>
        /// <param name="resistanceAttribute">The resistance attribute.</param>
        /// <remarks>
        /// Pendants always have offensive excellent options. If it's wizardry or physical depends on the specific item. I didn't find a pattern yet.
        /// </remarks>
        private void CreatePendant(byte number, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
        {
            this.CreateJewelery(number, 9, true, name, level, durability, resistanceAttribute);
        }

        private ItemDefinition CreateJewelery(byte number, int slot, bool dropsFromMonsters, string name, byte level, byte durability, AttributeDefinition? resistanceAttribute)
        {
            var item = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(item);
            item.Group = 13;
            item.Number = number;
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.FirstOrDefault(slotType => slotType.ItemSlots.Contains(slot));
            item.DropsFromMonsters = dropsFromMonsters;
            item.Name = name;
            item.DropLevel = level;
            item.MaximumItemLevel = 4;
            item.Width = 1;
            item.Height = 1;

            //// TODO: Requirement increases with item level
            this.CreateItemRequirementIfNeeded(item, Stats.Level, level);

            item.Durability = durability;
            item.PossibleItemOptions.Add(this.healthRecoverOptionDefinition!);

            if (resistanceAttribute != null)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                item.BasePowerUpAttributes.Add(powerUp);
                powerUp.BaseValue = 0.1f;
                powerUp.TargetAttribute = resistanceAttribute.GetPersistent(this.GameConfiguration);
                for (int lvl = 1; lvl <= 4; lvl++)
                {
                    var levelBonus = this.Context.CreateNew<LevelBonus>();
                    levelBonus.Level = lvl;
                    levelBonus.AdditionalValue = lvl * 0.1f;
                    powerUp.BonusPerLevel.Add(levelBonus);
                }
            }

            foreach (var characterClass in this.GameConfiguration.CharacterClasses)
            {
                item.QualifiedCharacters.Add(characterClass);
            }

            return item;
        }
    }
}