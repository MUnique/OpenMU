// <copyright file="Jewellery.cs" company="MUnique">
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
    /// Initializer for jewellery (rings and pendants).
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
    internal class Jewellery : InitializerBase
    {
        private ItemOptionDefinition healthRecoverOptionDefinition;

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

            this.CreateRing(8, "Ring of Ice", 20, 50, Stats.HealthRecoveryMultiplier, Stats.IceResistance);
            this.CreateRing(9, "Ring of Poison", 17, 50, Stats.HealthRecoveryMultiplier, Stats.PoisonResistance);
            this.CreateRing(21, "Ring of Fire", 30, 50, Stats.HealthRecoveryMultiplier, Stats.FireResistance);
            this.CreateRing(22, "Ring of Earth", 38, 50, Stats.HealthRecoveryMultiplier, Stats.EarthResistance);
            this.CreateRing(23, "Ring of Wind", 44, 50, Stats.HealthRecoveryMultiplier, Stats.WindResistance);
            this.CreateRing(24, "Ring of Magic", 47, 50, Stats.MaximumMana, null);

            this.CreatePendant(12, "Pendant of Lighting", 21, 50, DamageType.Wizardry, Stats.HealthRecoveryMultiplier, Stats.LightningResistance);
            this.CreatePendant(13, "Pendant of Fire", 13, 50, DamageType.Physical, Stats.HealthRecoveryMultiplier, Stats.FireResistance);
            this.CreatePendant(25, "Pendant of Ice", 34, 50, DamageType.Wizardry, Stats.HealthRecoveryMultiplier, Stats.IceResistance);
            this.CreatePendant(26, "Pendant of Wind", 42, 50, DamageType.Physical, Stats.HealthRecoveryMultiplier, Stats.WindResistance);
            this.CreatePendant(27, "Pendant of Water", 46, 50, DamageType.Wizardry, Stats.HealthRecoveryMultiplier, Stats.WaterResistance);
            this.CreatePendant(28, "Pendant of Ability", 50, 50, DamageType.Physical, Stats.MaximumAbility, null);

            // Requirement for Kanturu Event:
            var moonStonePendant = this.CreateJewelery(38, 10, false, "Moonstone Pendant", 21, 120, null, null, null);
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.MoonstonePendantEquipped.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = 1;
                moonStonePendant.BasePowerUpAttributes.Add(powerUp);
            }

            this.CreateWizardsRing();

            /* Cash shop rings
            this.CreateJewelery(107, 10, false, "Lethal Wizard's Ring", 0, 100, null, null, null);
            this.CreateJewelery(109, 10, false, "Sapphire Ring", 0, 0, null, null, null);
            this.CreateJewelery(110, 10, false, "Ruby Ring", 0, 0, null, null, null);
            this.CreateJewelery(111, 10, false, "Topaz Ring", 0, 0, null, null, null);
            this.CreateJewelery(112, 10, false, "Amethyst Ring", 0, 0, null, null, null);
            this.CreateJewelery(113, 9, false, "Ruby Necklace", 0, 0, null, null, null);
            this.CreateJewelery(114, 9, false, "Emerald Necklace", 0, 0, null, null, null);
            this.CreateJewelery(115, 9, false, "Sapphire Necklace", 0, 0, null, null, null);
            */

            /* TODO: Transformation feature
             * Idea: There could be an attribute "IsTransformed" which we could handle in the WorldView. These rings here could just add a value to this attribute.
            this.CreateJewelery(10, 10, false, "Transformation Ring", 0, 200, null, null, null);
            this.CreateJewelery(39, 10, false, "Elite Transfer Skeleton Ring", 10, 255, null, null, null);
            this.CreateJewelery(40, 10, false, "Jack Olantern Ring", 10, 100, null, null, null);
            this.CreateJewelery(41, 10, false, "Transfer Christmas Ring", 1, 100, null, null, null);
            this.CreateJewelery(68, 10, false, "Snowman Transformation Ring", 10, 100, null, null, null);
            this.CreateJewelery(122, 10, false, "Skeleton Transformation Ring", 1, 255, null, null, null);
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
        /// Creates the wizard ring which is dropped by the white wizard.
        /// </summary>
        /// <remarks>
        /// Options:
        /// Increase Damage 10%
        /// Increase Attacking(Wizardry) Speed+10.
        /// </remarks>
        private void CreateWizardsRing()
        {
            var ring = this.CreateJewelery(20, 10, false, "Wizards Ring", 0, 250, null, null, null);
            var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(optionDefinition);
            ring.PossibleItemOptions.Add(optionDefinition);
            ring.MaximumItemLevel = 0;
            optionDefinition.Name = "Wizard Ring Options";

            var increaseDamage = this.Context.CreateNew<IncreasableItemOption>();
            increaseDamage.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            increaseDamage.PowerUpDefinition.TargetAttribute = Stats.AttackDamageIncrease.GetPersistent(this.GameConfiguration);
            increaseDamage.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            increaseDamage.PowerUpDefinition.Boost.ConstantValue.Value = 0.1f;
            optionDefinition.PossibleOptions.Add(increaseDamage);

            var increaseSpeed = this.Context.CreateNew<IncreasableItemOption>();
            increaseSpeed.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            increaseSpeed.PowerUpDefinition.TargetAttribute = Stats.AttackSpeed.GetPersistent(this.GameConfiguration);
            increaseSpeed.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            increaseSpeed.PowerUpDefinition.Boost.ConstantValue.Value = 10f;
            optionDefinition.PossibleOptions.Add(increaseSpeed);

            // Always add both options "randomly" when it drops ;)
            optionDefinition.AddChance = 1.0f;
            optionDefinition.AddsRandomly = true;
            optionDefinition.MaximumOptionsPerItem = 2;
        }

        /// <summary>
        /// Creates a ring.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="name">The name.</param>
        /// <param name="level">The level.</param>
        /// <param name="durability">The durability.</param>
        /// <param name="optionTargetAttribute">The option target attribute.</param>
        /// <param name="resistanceAttribute">The resistance attribute.</param>
        /// <remarks>
        /// Rings always have defensive excellent options.
        /// </remarks>
        private void CreateRing(byte number, string name, byte level, byte durability, AttributeDefinition optionTargetAttribute, AttributeDefinition resistanceAttribute)
        {
            this.CreateJewelery(number, 10, true, name, level, durability, this.GameConfiguration.ExcellentDefenseOptions(), optionTargetAttribute, resistanceAttribute);
        }

        /// <summary>
        /// Creates a pendant.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="name">The name.</param>
        /// <param name="level">The level.</param>
        /// <param name="durability">The durability.</param>
        /// <param name="excellentOptionDamageType">Type of the excellent option damage.</param>
        /// <param name="optionTargetAttribute">The option target attribute.</param>
        /// <param name="resistanceAttribute">The resistance attribute.</param>
        /// <remarks>
        /// Pendants always have offensive excellent options. If it's wizardry or physical depends on the specific item. I didn't find a pattern yet.
        /// </remarks>
        private void CreatePendant(byte number, string name, byte level, byte durability, DamageType excellentOptionDamageType, AttributeDefinition optionTargetAttribute, AttributeDefinition resistanceAttribute)
        {
            var excellentOption = excellentOptionDamageType == DamageType.Physical
                ? this.GameConfiguration.ExcellentPhysicalAttackOptions()
                : this.GameConfiguration.ExcellentWizardryAttackOptions();
            this.CreateJewelery(number, 9, true, name, level, durability, excellentOption, optionTargetAttribute, resistanceAttribute);
        }

        private ItemDefinition CreateJewelery(byte number, int slot, bool dropsFromMonsters, string name, byte level, byte durability, ItemOptionDefinition excellentOptionDefinition, AttributeDefinition optionTargetAttribute, AttributeDefinition resistanceAttribute)
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
            if (excellentOptionDefinition != null)
            {
                item.PossibleItemOptions.Add(excellentOptionDefinition);
            }

            if (optionTargetAttribute == Stats.HealthRecoveryMultiplier)
            {
                item.PossibleItemOptions.Add(this.healthRecoverOptionDefinition);
            }
            else if (optionTargetAttribute != null)
            {
                // Then it's either maximum mana or ability increase by 1% for each option level
                var option = this.CreateOption("Jewellery option " + optionTargetAttribute.Designation, optionTargetAttribute, 1.01f, AggregateType.Multiplicate);

                item.PossibleItemOptions.Add(option);
            }
            else
            {
                // we add no option.
            }

            if (resistanceAttribute != null)
            {
                // TODO: Implement elemental attacks and resistancies.
                // Not sure how these resistancies work. If I remember correctly, it worked at the original server this way:
                //   - it only considers the maximum resistance of all equipped items
                //   - officially there were only rings/pendant up to level 4 and they worked pretty well
                //     -> one item level means about 20% less chance to get affected by the element (iced, poisoned etc.).
                //   - I'm not sure if they prevent hits or lower the damage. For example, you could miss an attack but still apply ice or poison to an opponent.
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                item.BasePowerUpAttributes.Add(powerUp);
                powerUp.BaseValue = 0.2f;
                powerUp.TargetAttribute = resistanceAttribute.GetPersistent(this.GameConfiguration);
                for (int i = 1; i <= 4; i++)
                {
                    var levelBonus = this.Context.CreateNew<LevelBonus>();
                    levelBonus.Level = i;
                    levelBonus.AdditionalValue = (1 + i) * 0.2f;
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