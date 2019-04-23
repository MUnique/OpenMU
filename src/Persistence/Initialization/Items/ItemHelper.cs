// <copyright file="ItemHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A helper class which contains some convenient methods to create items.
    /// </summary>
    internal class ItemHelper
    {
        private readonly IContext context;

        private readonly GameConfiguration gameConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ItemHelper(IContext context, GameConfiguration gameConfiguration)
        {
            this.context = context;
            this.gameConfiguration = gameConfiguration;
        }

        /// <summary>
        /// Creates an orb.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="learnableId">The learnable identifier.</param>
        /// <returns>The orb.</returns>
        public Item CreateOrb(byte itemSlot, byte learnableId)
        {
            return this.CreateLearnable(itemSlot, learnableId, ItemGroups.Orbs);
        }

        /// <summary>
        /// Creates an summon orb.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="level">The level of the orb - indirectly defines the skill which will be learned.</param>
        /// <returns>The orb.</returns>
        public Item CreateSummonOrb(byte itemSlot, byte level)
        {
            var orb = this.CreateLearnable(itemSlot, 11, ItemGroups.Orbs);
            orb.Level = level;
            return orb;
        }

        /// <summary>
        /// Creates a scroll.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="learnableId">The learnable identifier.</param>
        /// <returns>The scroll.</returns>
        public Item CreateScroll(byte itemSlot, byte learnableId)
        {
            return this.CreateLearnable(itemSlot, learnableId, ItemGroups.Scrolls);
        }

        /// <summary>
        /// Creates the learnable.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="learnableId">The learnable identifier.</param>
        /// <param name="group">The group.</param>
        /// <returns>The learnable.</returns>
        public Item CreateLearnable(byte itemSlot, byte learnableId, ItemGroups group)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == (byte)group && def.Number == learnableId);
            item.ItemSlot = itemSlot;

            return item;
        }

        /// <summary>
        /// Creates a potion.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="potionId">The potion identifier.</param>
        /// <param name="stackSize">Size of the stack.</param>
        /// <param name="level">The level.</param>
        /// <returns>The potion.</returns>
        public Item CreatePotion(byte itemSlot, byte potionId, byte stackSize, byte level)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == 14 && def.Number == potionId);
            item.ItemSlot = itemSlot;
            item.Durability = stackSize;
            item.Level = level;

            return item;
        }

        /// <summary>
        /// Creates an item.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="itemGroup">The item group.</param>
        /// <param name="stackSize">Size of the stak.</param>
        /// <param name="level">The level.</param>
        /// <returns>The created item.</returns>
        public Item CreateItem(byte itemSlot, byte itemId, byte itemGroup, byte stackSize, byte level)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == itemGroup && def.Number == itemId);
            item.ItemSlot = itemSlot;
            item.Durability = stackSize;
            item.Level = level;

            return item;
        }

        /// <summary>
        /// Creates a new item of an armor set.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="setNumber">The set number.</param>
        /// <param name="group">The group.</param>
        /// <param name="targetExcellentOption">The target excellent option.</param>
        /// <param name="level">The level.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="luck">if set to <c>true</c>, the item has luck option.</param>
        /// <returns>The created armor set item.</returns>
        public Item CreateSetItem(byte itemSlot, byte setNumber, ItemGroups group, AttributeDefinition targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
        {
            return this.CreateEquippableItem(itemSlot, group, setNumber, level, optionLevel, luck, targetExcellentOption);
        }

        /// <summary>
        /// Creates a shield.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="setNumber">The set number.</param>
        /// <param name="skill">if set to <c>true</c>, the item has skill.</param>
        /// <param name="targetExcellentOption">The target excellent option.</param>
        /// <param name="level">The level.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="luck">if set to <c>true</c>, the item has luck option.</param>
        /// <returns>The created shield.</returns>
        public Item CreateShield(byte itemSlot, byte setNumber, bool skill, AttributeDefinition targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
        {
            var item = this.CreateEquippableItem(itemSlot, ItemGroups.Shields, setNumber, level, optionLevel, luck, targetExcellentOption);
            item.HasSkill = skill;
            return item;
        }

        /// <summary>
        /// Creates an equippable item.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="group">The group.</param>
        /// <param name="number">The number.</param>
        /// <param name="level">The level.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="luck">if set to <c>true</c>, the item has luck option.</param>
        /// <param name="targetExcellentOption">The target excellent option.</param>
        /// <returns>The created item.</returns>
        public Item CreateEquippableItem(byte itemSlot, ItemGroups group, byte number, byte level, byte optionLevel, bool luck, AttributeDefinition targetExcellentOption)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == (byte)group && def.Number == number);
            item.Durability = item.Definition.Durability;
            item.ItemSlot = itemSlot;
            item.Level = level;
            var excellentOption = this.GetExcellentOption(targetExcellentOption, item);
            if (excellentOption != null)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = excellentOption;
                item.ItemOptions.Add(optionLink);
            }

            if (optionLevel > 0)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Option);
                optionLink.Level = optionLevel;
                item.ItemOptions.Add(optionLink);
            }

            if (luck)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Luck);
                item.ItemOptions.Add(optionLink);
            }

            return item;
        }

        /// <summary>
        /// Creates the weapon.
        /// </summary>
        /// <param name="itemSlot">The item slot.</param>
        /// <param name="group">The group.</param>
        /// <param name="number">The number.</param>
        /// <param name="level">The level.</param>
        /// <param name="optionLevel">The option level.</param>
        /// <param name="luck">if set to <c>true</c>, the item has luck option.</param>
        /// <param name="skill">if set to <c>true</c>, the item has skill.</param>
        /// <param name="targetExcellentOption">The target excellent option.</param>
        /// <returns>The created weapon.</returns>
        public Item CreateWeapon(byte itemSlot, ItemGroups group, byte number, byte level, byte optionLevel, bool luck, bool skill, AttributeDefinition targetExcellentOption)
        {
            var weapon = this.CreateEquippableItem(itemSlot, group, number, level, optionLevel, luck, targetExcellentOption);
            weapon.HasSkill = skill;
            return weapon;
        }

        private IncreasableItemOption GetExcellentOption(AttributeDefinition targetExcellentOption, Item item)
        {
            if (targetExcellentOption == null)
            {
                return null;
            }

            return item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .First(o => o.PowerUpDefinition.TargetAttribute == targetExcellentOption);
        }
    }
}