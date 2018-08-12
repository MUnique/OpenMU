namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    // TODO: Add ability to make items with AttributeDefinition.
    // TODO: Add ability to create any kind of items e.g. jewellery, jewels, tickets, maps, etc.
    // TODO: Change syntax for item creation... Create AtSlot() method to be first and required in flow.
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;

    public class ItemBuilder : IBuilderWeaponEnter, IBuilderShieldEnter, IBuilderSetItemEnter, IBuilderOrb, IBuilderScroll, IBuilderPotion, IBuilderEquipableChained, IBuilderPotionDurabilityChained, IBuilderDurabilityChained, IBuilderLevelChained, IBuilderPotionLevelChained, IBuilderOptionLevelChained, IBuilderWithSkillChained, IBuilderWithLuckChained, IBuilderSetItemPartChained, IBuilderOrbAndScrollChained, IBuilderPotionChained
    {
        private static IContext context;
        private static GameConfiguration gameConfiguration;

        private byte itemSlot;
        private byte itemNumber;
        private byte groupNumber;
        private byte durability;
        private byte level;
        private byte optionLevel;
        private bool skill;
        private bool luck;
        private AttributeDefinition targetExcellentOption;

        private ItemBuilder(byte slotNumber, byte groupNumber = 0, byte durability = 0)
        {
            this.itemSlot = slotNumber;
            this.itemNumber = 0;
            this.groupNumber = groupNumber;
            this.durability = durability;
            this.level = 0;
            this.optionLevel = 0;
            this.skill = false;
            this.luck = false;
        }

        public static void Initialize(IContext context, GameConfiguration gameConfiguration)
        {
            ItemBuilder.context = context;
            ItemBuilder.gameConfiguration = gameConfiguration;
        }

        public static IBuilderWeaponEnter WeaponAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber);
        }

        public static IBuilderShieldEnter ShieldAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber, (byte)ItemConst.Groups.Shields);
        }

        public static IBuilderSetItemEnter SetItemAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber);
        }

        public static IBuilderOrb OrbAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber, (byte)ItemConst.Groups.Orbs);
        }

        public static IBuilderScroll ScrollAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber, (byte)ItemConst.Groups.Scrolls);
        }

        public static IBuilderPotion PotionAtSlot(byte slotNumber)
        {
            return new ItemBuilder(slotNumber, (byte)ItemConst.Groups.Potions, 1);
        }

        public IBuilderEquipableChained Sword(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Swords;
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderEquipableChained Axe(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Axes;
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderEquipableChained MaceOrBlunt(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Mace;
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderEquipableChained Spear(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Spears;
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderEquipableChained BowOrCrossbow(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Bows;
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderEquipableChained StaffOrBook(byte itemNumber)
        {
            this.groupNumber = (byte)ItemConst.Groups.Staffs;
            this.itemNumber = itemNumber;
            return this;
        }


        public IBuilderDurabilityChained Durability(byte durability)
        {
            this.durability = durability;
            return this;
        }

        public IBuilderPotionDurabilityChained StackSize(byte durability)
        {
            this.durability = durability;
            return this;
        }

        public IBuilderLevelChained Level(byte level)
        {
            this.level = level;
            return this;
        }

        public IBuilderPotionLevelChained PotionLevel(byte level)
        {
            this.level = level;
            return this;
        }

        public IBuilderOptionLevelChained OptionLevel(byte optionLevel)
        {
            this.optionLevel = optionLevel;
            return this;
        }

        public IBuilderWithSkillChained WithSkill()
        {
            this.skill = true;
            return this;
        }

        public IBuilderWithLuckChained WithLuck()
        {
            this.luck = true;
            return this;
        }

        public IBuilderEquipableChained Shield(byte itemNumber)
        {
            this.itemNumber = itemNumber;
            return this;
        }

        // TODO: fix naming. It is itemNumber not group. Group refers to type like helm, armor etc.
        public IBuilderSetItemPartChained SetNumber(byte groupNumber)
        {
            this.itemNumber = groupNumber;
            return this;
        }

        public IBuilderEquipableChained Helm()
        {
            this.groupNumber = (byte)ItemConst.Groups.Helms;
            return this;
        }

        public IBuilderEquipableChained Armor()
        {
            this.groupNumber = (byte)ItemConst.Groups.Armors;
            return this;
        }

        public IBuilderEquipableChained Pants()
        {
            this.groupNumber = (byte)ItemConst.Groups.Pants;
            return this;
        }

        public IBuilderEquipableChained Gloves()
        {
            this.groupNumber = (byte)ItemConst.Groups.Gloves;
            return this;
        }

        public IBuilderEquipableChained Boots()
        {
            this.groupNumber = (byte)ItemConst.Groups.Boots;
            return this;
        }

        public IBuilderOrbAndScrollChained Orb(byte itemNumber)
        {
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderOrbAndScrollChained Scroll(byte itemNumber)
        {
            this.itemNumber = itemNumber;
            return this;
        }

        public IBuilderPotionChained Potion(byte itemNumber)
        {
            this.itemNumber = itemNumber;
            return this;
        }

        public Item Make()
        {
            var item = ItemBuilder.context.CreateNew<Item>();

            item.Definition = ItemBuilder.gameConfiguration.Items.First(def => def.Group == this.groupNumber && def.Number == this.itemNumber);
            item.Level = this.level;

            item.Durability = item.Definition.Durability;
            if (this.durability != null)
            {
                item.Durability = (byte)this.durability;
            }

            item.ItemSlot = this.itemSlot;

            if (this.skill)
            {
                item.HasSkill = this.skill;
            }

            if (this.optionLevel > 0)
            {
                var optionLink = ItemBuilder.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Option);
                optionLink.Level = this.optionLevel;
                item.ItemOptions.Add(optionLink);
            }

            if (this.luck)
            {
                var optionLink = ItemBuilder.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Luck);
                item.ItemOptions.Add(optionLink);
            }

            if (this.targetExcellentOption != null)
            {
                var optionLink = ItemBuilder.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                    .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                    .First(o => o.PowerUpDefinition.TargetAttribute == this.targetExcellentOption);
                item.ItemOptions.Add(optionLink);
            }

            return item;
        }
    }
}