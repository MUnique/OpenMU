namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;


    public class ItemHelper
    {
        private readonly IContext context;

        private readonly GameConfiguration gameConfiguration;

        public ItemHelper(IContext context, GameConfiguration gameConfiguration)
        {
            this.context = context;
            this.gameConfiguration = gameConfiguration;
        }

        public Item CreateOrb(byte itemSlot, byte learnableId)
        {
            return this.CreateLearnable(itemSlot, learnableId, 12); // 12 - Orb group
        }

        public Item CreateScroll(byte itemSlot, byte learnableId)
        {
            return this.CreateLearnable(itemSlot, learnableId, 15); // 15 - Scroll group
        }

        public Item CreateLearnable(byte itemSlot, byte learnableId, byte group)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == group && def.Number == learnableId);
            item.ItemSlot = itemSlot;

            return item;
        }

        public Item CreatePotion(byte itemSlot, byte potionId, byte stakSize, byte level)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == 14 && def.Number == potionId);
            item.ItemSlot = itemSlot;
            item.Durability = stakSize;
            item.Level = level;

            return item;
        }

        public Item CreateItem(byte itemSlot, byte itemId, byte itemGroup, byte stakSize, byte level)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == itemGroup && def.Number == itemId);
            item.ItemSlot = itemSlot;
            item.Durability = stakSize;
            item.Level = level;

            return item;
        } 

        public Item CreateSetItem(byte itemSlot, byte setNumber, byte group, AttributeDefinition targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
        {

            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == group && def.Number == setNumber);
            item.Level = level;
            item.Durability = item.Definition.Durability;
            item.ItemSlot = itemSlot;
            if (targetExcellentOption != null)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                    .First(o => o.PowerUpDefinition.TargetAttribute == targetExcellentOption);
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

        public Item CreateShield(byte itemSlot, byte setNumber, bool skill, AttributeDefinition targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
        {

            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == 6 && def.Number == setNumber);
            item.Level = level;
            item.HasSkill = skill;
            item.Durability = item.Definition.Durability;
            item.ItemSlot = itemSlot;
            if (targetExcellentOption != null)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                    .First(o => o.PowerUpDefinition.TargetAttribute == targetExcellentOption);
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

        public Item CreateWeapon(byte itemSlot, byte group, byte number, byte level, byte optionLevel, bool luck, bool skill, AttributeDefinition targetExcellentOption = null)
        {
            var weapon = this.context.CreateNew<Item>();
            weapon.Definition = this.gameConfiguration.Items.First(def => def.Group == group && def.Number == number);
            weapon.Durability = weapon.Definition?.Durability ?? 0;
            weapon.ItemSlot = itemSlot;
            weapon.Level = level;
            weapon.HasSkill = skill;
            if (targetExcellentOption != null)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                    .First(o => o.PowerUpDefinition.TargetAttribute == targetExcellentOption);
                weapon.ItemOptions.Add(optionLink);
            }

            if (optionLevel > 0)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Option);
                optionLink.Level = optionLevel;
                weapon.ItemOptions.Add(optionLink);
            }

            if (luck)
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Luck);
                weapon.ItemOptions.Add(optionLink);
            }

            return weapon;
        }
    }
}
