// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GameServer.MessageHandler;
    using MUnique.OpenMU.GameServer.MessageHandler.Friends;
    using MUnique.OpenMU.GameServer.MessageHandler.Guild;
    using MUnique.OpenMU.GameServer.MessageHandler.Items;
    using MUnique.OpenMU.GameServer.MessageHandler.Party;
    using MUnique.OpenMU.GameServer.MessageHandler.Trade;
    using MUnique.OpenMU.Persistence.Initialization.Items;
    using MUnique.OpenMU.Persistence.Initialization.Maps;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// Class to manage data initialization.
    /// </summary>
    public class DataInitialization
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private GameConfiguration gameConfiguration;
        private IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitialization"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public DataInitialization(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Creates the initial data for a server.
        /// </summary>
        public void CreateInitialData()
        {
            using (var temporaryContext = this.persistenceContextProvider.CreateNewContext())
            {
                this.gameConfiguration = temporaryContext.CreateNew<GameConfiguration>();
            }

            using (this.context = this.persistenceContextProvider.CreateNewContext(this.gameConfiguration))
            {
                this.InitializeGameConfiguration();
                var gameServerConfiguration = this.CreateGameServerConfiguration(this.gameConfiguration.Maps);
                this.CreateGameServerDefinitions(gameServerConfiguration, 3);
                this.context.SaveChanges();

                var lorencia = this.gameConfiguration.Maps.First(map => map.Number == 0);
                foreach (var map in this.gameConfiguration.Maps)
                {
                    // set safezone to lorencia for now...
                    map.SafezoneMap = lorencia;
                }

                this.CreateTestAccounts(10);
                this.CreateTest300();
                this.CreateTest400();

                this.context.SaveChanges();
            }
        }

        private void CreateTestAccounts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.CreateTestAccount(i);
            }
        }

        private long CalcNeededMasterExp(long lvl)
        {
            // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
            return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
        }

        private long CalculateNeededExperience(long level)
        {
            if (level == 0)
            {
                return 0;
            }

            if (level < 256)
            {
                return 10 * (level + 8) * (level - 1) * (level - 1);
            }

            return (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256));
        }

        private void CreateTestAccount(int index)
        {
            var loginName = "test" + index.ToString();

            var account = this.context.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginName);
            account.Vault = this.context.CreateNew<ItemStorage>();

            var level = (index * 10) + 1;
            account.Characters.Add(this.CreateDarkKnight(loginName + "Dk", level));
            account.Characters.Add(this.CreateElf(loginName + "Elf", level));
            account.Characters.Add(this.CreateWizard(loginName + "Wiz", level));
        }

        private Character CreateWizard(string name, int level)
        {
            var character = this.CreateCharacter(name, CharacterClassNumber.DarkWizard, level, 1);
            this.AddTestJewelsAndPotions(character.Inventory);
            character.Inventory.Items.Add(this.CreateSkullStaff(0));
            character.Inventory.Items.Add(this.CreateSetItem(52, 2, 8)); // Pad Armor
            character.Inventory.Items.Add(this.CreateSetItem(47, 2, 7)); // Pad Helm
            character.Inventory.Items.Add(this.CreateSetItem(49, 2, 9)); // Pad Pants
            character.Inventory.Items.Add(this.CreateSetItem(63, 2, 10)); // Pad Gloves
            character.Inventory.Items.Add(this.CreateSetItem(65, 2, 11)); // Pad Boots

            return character;
        }

        private Character CreateElf(string name, int level)
        {
            var character = this.CreateCharacter(name, CharacterClassNumber.FairyElf, level, 2);
            this.AddTestJewelsAndPotions(character.Inventory);
            character.Inventory.Items.Add(this.CreateShortBow(1));
            character.Inventory.Items.Add(this.CreateArrows(0));
            character.Inventory.Items.Add(this.CreateSetItem(52, 10, 8)); // Vine Armor
            character.Inventory.Items.Add(this.CreateSetItem(47, 10, 7)); // Vine Helm
            character.Inventory.Items.Add(this.CreateSetItem(49, 10, 9)); // Vine Pants
            character.Inventory.Items.Add(this.CreateSetItem(63, 10, 10)); // Vine Gloves
            character.Inventory.Items.Add(this.CreateSetItem(65, 10, 11)); // Vine Boots
            character.Inventory.Items.Add(this.CreateOrb(67, 8)); // Healing Orb
            character.Inventory.Items.Add(this.CreateOrb(75, 9)); // Defense Orb
            character.Inventory.Items.Add(this.CreateOrb(68, 10)); // Damage Orb
            return character;
        }

        private Character CreateDarkKnight(string name, int level)
        {
            var character = this.CreateCharacter(name, CharacterClassNumber.DarkKnight, level, 0);
            this.AddTestJewelsAndPotions(character.Inventory);
            character.Inventory.Items.Add(this.CreateSmallAxe(0));
            character.Inventory.Items.Add(this.CreateSetItem(52, 5, 8)); // Leather Armor
            character.Inventory.Items.Add(this.CreateSetItem(47, 5, 7)); // Leather Helm
            character.Inventory.Items.Add(this.CreateSetItem(49, 5, 9)); // Leather Pants
            character.Inventory.Items.Add(this.CreateSetItem(63, 5, 10, Stats.DamageReflection)); // Leather Gloves
            character.Inventory.Items.Add(this.CreateSetItem(65, 5, 11, Stats.DamageReflection)); // Leather Boots

            return character;
        }

        private Character CreateCharacter(string name, CharacterClassNumber characterClass, int level, byte slot)
        {
            var character = this.context.CreateNew<Character>();
            character.CharacterClass = this.gameConfiguration.CharacterClasses.First(c => c.Number == (byte)characterClass);
            character.Name = name;
            character.CharacterSlot = slot;
            character.CreateDate = DateTime.Now;
            character.KeyConfiguration = new byte[30];
            foreach (
                var attribute in
                character.CharacterClass.StatAttributes.Select(
                    a => this.context.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
            {
                character.Attributes.Add(attribute);
            }

            character.CurrentMap = character.CharacterClass.HomeMap;
            var spawnGate = character.CurrentMap.ExitGates.Where(m => m.IsSpawnGate).SelectRandom();
            character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
            character.Attributes.First(a => a.Definition == Stats.Level).Value = level;
            character.Experience = this.CalculateNeededExperience(level);
            character.LevelUpPoints = (int)(character.Attributes.First(a => a.Definition == Stats.Level).Value - 1) * character.CharacterClass.PointsPerLevelUp;
            character.Inventory = this.context.CreateNew<ItemStorage>();
            character.Inventory.Money = 1000000;
            return character;
        }

        private void AddTestJewelsAndPotions(ItemStorage inventory)
        {
            inventory.Items.Add(this.CreateJewelOfBless(12));
            inventory.Items.Add(this.CreateJewelOfBless(13));
            inventory.Items.Add(this.CreateJewelOfBless(14));
            inventory.Items.Add(this.CreateJewelOfBless(15));
            inventory.Items.Add(this.CreateJewelOfBless(16));
            inventory.Items.Add(this.CreateJewelOfBless(17));
            inventory.Items.Add(this.CreateJewelOfBless(18));
            inventory.Items.Add(this.CreateJewelOfBless(19));
            inventory.Items.Add(this.CreateJewelOfSoul(20));
            inventory.Items.Add(this.CreateJewelOfSoul(21));
            inventory.Items.Add(this.CreateJewelOfSoul(22));
            inventory.Items.Add(this.CreateJewelOfSoul(23));
            inventory.Items.Add(this.CreateJewelOfSoul(24));
            inventory.Items.Add(this.CreateJewelOfSoul(25));
            inventory.Items.Add(this.CreateJewelOfSoul(26));
            inventory.Items.Add(this.CreateJewelOfSoul(27));
            inventory.Items.Add(this.CreateJewelOfLife(28));
            inventory.Items.Add(this.CreateJewelOfLife(29));
            inventory.Items.Add(this.CreateJewelOfLife(30));
            inventory.Items.Add(this.CreateJewelOfLife(31));
            inventory.Items.Add(this.CreateJewelOfLife(32));
            inventory.Items.Add(this.CreateJewelOfLife(33));
            inventory.Items.Add(this.CreateJewelOfLife(34));
            inventory.Items.Add(this.CreateJewelOfLife(35));
            inventory.Items.Add(this.CreateHealthPotion(36, 0));
            inventory.Items.Add(this.CreateHealthPotion(37, 1));
            inventory.Items.Add(this.CreateHealthPotion(38, 2));
            inventory.Items.Add(this.CreateHealthPotion(39, 3));
            inventory.Items.Add(this.CreateManaPotion(40, 0));
            inventory.Items.Add(this.CreateManaPotion(41, 1));
            inventory.Items.Add(this.CreateManaPotion(42, 2));
            inventory.Items.Add(this.CreateAlcohol(43));
            inventory.Items.Add(this.CreateShieldPotion(44, 0));
            inventory.Items.Add(this.CreateShieldPotion(45, 1));
            inventory.Items.Add(this.CreateShieldPotion(46, 2));
        }

        private void CreateTest400()
        {
            var loginName = "test400";

            var account = this.context.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginName);
            account.Vault = this.context.CreateNew<ItemStorage>();

            var character = this.CreateCharacter(loginName, CharacterClassNumber.BattleMaster, 400, 0);
            account.Characters.Add(character);
            character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 1200;
            character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 400;
            character.LevelUpPoints -= 1600; // for the added strength and agility
            character.LevelUpPoints -= 220; // Before level 220, it's a point less per level
            character.MasterLevelUpPoints = 100; // To test master skill tree

            character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 19, 15, 4, true, true, Stats.ExcellentDamageChance)); // Exc AA Sword+15+16+L+ExcDmg
            character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 0, 22, 15, 4, true, true)); // Bone Blade+15+16+L

            // Dragon Knight Set+15+16+L:
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.ArmorSlot, 29, 8, null, 15, 4, true));
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.HelmSlot, 29, 7, null, 15, 4, true));
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.PantsSlot, 29, 9, null, 15, 4, true));
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.GlovesSlot, 29, 10, null, 15, 4, true));
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.BootsSlot, 29, 11, null, 15, 4, true));
            character.Inventory.Items.Add(this.CreateTestWing(InventoryConstants.WingsSlot, 36, 15)); // Wing of Storm +15
            this.AddTestJewelsAndPotions(character.Inventory);
            character.Inventory.Items.Add(this.CreateOrb(47, 12));
            character.Inventory.Items.Add(this.CreateOrb(48, 14));
            character.Inventory.Items.Add(this.CreateOrb(49, 19));
            character.Inventory.Items.Add(this.CreateOrb(50, 44));
            character.Inventory.Items.Add(this.CreateOrb(56, 7));
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(52, 20)); // Wizards Ring
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(53, 8)); // Ring of Ice
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(54, 9)); // Ring of Poison
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(55, 12)); // Pendant of Lightning
        }

        private void CreateTest300()
        {
            var loginName = "test300";

            var account = this.context.CreateNew<Account>();
            account.LoginName = loginName;
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginName);
            account.Vault = this.context.CreateNew<ItemStorage>();

            var character = this.CreateCharacter(loginName, CharacterClassNumber.BladeKnight, 300, 0);
            account.Characters.Add(character);

            character.Attributes.First(a => a.Definition == Stats.BaseStrength).Value += 400;
            character.Attributes.First(a => a.Definition == Stats.BaseAgility).Value += 300;
            character.LevelUpPoints -= 700; // for the added strength and agility
            character.LevelUpPoints -= 220; // Before level 220, it's a point less per level

            character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.LeftHandSlot, 0, 0, 13, 4, true, false, Stats.ExcellentDamageChance)); // Exc Kris+13+16+L+ExcDmg
            character.Inventory.Items.Add(this.CreateWeapon(InventoryConstants.RightHandSlot, 0, 5, 13, 4, true, true, Stats.ExcellentDamageChance)); // Exc Blade+13+16+L+ExcDmg
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.ArmorSlot, 6, 8, Stats.DamageReceiveDecrement, 13, 4, true)); // Exc Scale Armor+13+16+L
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.HelmSlot, 6, 7, Stats.MaximumHealth, 13, 4, true)); // Exc Scale Helm+13+16+L
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.PantsSlot, 6, 9, Stats.MoneyAmountRate, 13, 4, true)); // Exc Scale Pants+13+16+L
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.GlovesSlot, 6, 10, Stats.MaximumMana, 13, 4, true)); // Exc Scale Gloves+13+16+L
            character.Inventory.Items.Add(this.CreateSetItem(InventoryConstants.BootsSlot, 6, 11, Stats.DamageReflection, 13, 4, true)); // Exc Scale Boots+13+16+L
            character.Inventory.Items.Add(this.CreateTestWing(InventoryConstants.WingsSlot, 5, 13)); // Dragon Wings +13
            this.AddTestJewelsAndPotions(character.Inventory);
            character.Inventory.Items.Add(this.CreateOrb(47, 12));
            character.Inventory.Items.Add(this.CreateOrb(48, 14));
            character.Inventory.Items.Add(this.CreateOrb(49, 19));
            character.Inventory.Items.Add(this.CreateOrb(50, 44));
            character.Inventory.Items.Add(this.CreateOrb(56, 7));
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(52, 20)); // Wizards Ring
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(53, 8)); // Ring of Ice
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(54, 9)); // Ring of Poison
            character.Inventory.Items.Add(this.CreateFullOptionJewellery(55, 12)); // Pendant of Lightning
        }

        private Item CreateFullOptionJewellery(byte itemSlot, int number)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == 13 && def.Number == number);
            item.Durability = item.Definition.Durability;
            item.ItemSlot = itemSlot;
            foreach (var possibleOption in item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions))
            {
                var optionLink = this.context.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = possibleOption;
                item.ItemOptions.Add(optionLink);
            }

            return item;
        }

        private Item CreateTestWing(byte itemSlot, byte number, byte level)
        {
            var item = this.context.CreateNew<Item>();
            item.Definition = this.gameConfiguration.Items.First(def => def.Group == 12 && def.Number == number);
            item.Durability = item.Definition.Durability;
            item.ItemSlot = itemSlot;
            item.Level = level;
            var option = item.Definition.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Wing));
            var optionLink = this.context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option.PossibleOptions.SelectRandom();
            item.ItemOptions.Add(optionLink);
            return item;
        }

        private Item CreateSetItem(byte itemSlot, byte setNumber, byte group, AttributeDefinition targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
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

        private Item CreateWeapon(byte itemSlot, byte group, byte number, byte level, byte optionLevel, bool luck, bool skill, AttributeDefinition targetExcellentOption = null)
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

        private Item CreateAlcohol(byte itemSlot)
        {
            var potion = this.context.CreateNew<Item>();
            potion.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == 9);
            potion.Durability = 1;
            potion.ItemSlot = itemSlot;
            return potion;
        }

        private Item CreateManaPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, (byte)(size + 4));
        }

        private Item CreateHealthPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, size);
        }

        private Item CreateShieldPotion(byte itemSlot, byte size)
        {
            return this.CreatePotion(itemSlot, (byte)(size + 35));
        }

        private Item CreateOrb(byte itemSlot, byte itemNumber)
        {
            var potion = this.context.CreateNew<Item>();
            potion.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 12 && def.Number == itemNumber);
            potion.ItemSlot = itemSlot;
            return potion;
        }

        private Item CreatePotion(byte itemSlot, byte itemNumber)
        {
            var potion = this.context.CreateNew<Item>();
            potion.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == itemNumber);
            potion.Durability = 3; // Stack of 3 Potions
            potion.ItemSlot = itemSlot;
            return potion;
        }

        private Item CreateJewelOfBless(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 13);
        }

        private Item CreateJewelOfSoul(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 14);
        }

        private Item CreateJewelOfLife(byte itemSlot)
        {
            return this.CreateJewel(itemSlot, 16);
        }

        private Item CreateJewel(byte itemSlot, byte itemNumber)
        {
            var jewel = this.context.CreateNew<Item>();
            jewel.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == itemNumber);
            jewel.Durability = 1;
            jewel.ItemSlot = itemSlot;
            return jewel;
        }

        private Item CreateSmallAxe(byte itemSlot)
        {
            var smallAxe = this.context.CreateNew<Item>();
            smallAxe.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 1 && def.Number == 0); // small axe
            smallAxe.Durability = smallAxe.Definition?.Durability ?? 0;
            smallAxe.ItemSlot = itemSlot;
            return smallAxe;
        }

        private Item CreateShortBow(byte itemSlot)
        {
            var shortBow = this.context.CreateNew<Item>();
            shortBow.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 4 && def.Number == 0); // short bow
            shortBow.Durability = shortBow.Definition?.Durability ?? 0;
            shortBow.ItemSlot = itemSlot;
            return shortBow;
        }

        private Item CreateArrows(byte itemSlot)
        {
            var arrows = this.context.CreateNew<Item>();
            arrows.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 4 && def.Number == 15); // short bow
            arrows.Durability = 255;
            arrows.ItemSlot = itemSlot;
            return arrows;
        }

        private Item CreateSkullStaff(byte itemSlot)
        {
            var skullStaff = this.context.CreateNew<Item>();
            skullStaff.Definition = this.gameConfiguration.Items.FirstOrDefault(def => def.Group == 5 && def.Number == 0); // skull staff
            skullStaff.Durability = skullStaff.Definition?.Durability ?? 0;
            skullStaff.ItemSlot = itemSlot;
            return skullStaff;
        }

        private void CreateNpcs()
        {
            var init = new NpcInitialization(this.context, this.gameConfiguration);
            init.CreateNpcs();
        }

        private PacketHandlerConfiguration CreatePacketConfig<THandler>(PacketType packetType, bool needsEncryption = false)
            where THandler : IPacketHandler
        {
            var config = this.context.CreateNew<PacketHandlerConfiguration>();
            config.PacketIdentifier = (byte)packetType;
            config.PacketHandlerClassName = typeof(THandler).AssemblyQualifiedName;
            config.NeedsToBeEncrypted = needsEncryption;
            return config;
        }

        private ItemOptionDefinition CreateLuckOptionDefinition()
        {
            var definition = this.context.CreateNew<ItemOptionDefinition>();

            definition.Name = "Luck";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Luck);
            itemOption.PowerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.FirstOrDefault(a => a == Stats.CriticalDamageChance);
            itemOption.PowerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private ItemOptionDefinition CreateOptionDefinition(AttributeDefinition attributeDefinition)
        {
            var definition = this.context.CreateNew<ItemOptionDefinition>();

            definition.Name = attributeDefinition.Designation + " Option";
            definition.AddChance = 0.25f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            var itemOption = this.context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.gameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
            itemOption.PowerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.gameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = 4;
            for (int level = 2; level <= 4; level++)
            {
                var levelDependentOption = this.context.CreateNew<ItemOptionOfLevel>();
                levelDependentOption.Level = level;
                var powerUpDefinition = this.context.CreateNew<PowerUpDefinition>();
                powerUpDefinition.TargetAttribute = itemOption.PowerUpDefinition.TargetAttribute;
                powerUpDefinition.Boost = this.context.CreateNew<PowerUpDefinitionValue>();
                powerUpDefinition.Boost.ConstantValue.Value = level * 4;
                levelDependentOption.PowerUpDefinition = powerUpDefinition;
                itemOption.LevelDependentOptions.Add(levelDependentOption);
            }

            definition.PossibleOptions.Add(itemOption);

            return definition;
        }

        private void CreateItemOptionTypes()
        {
            var optionTypes = typeof(ItemOptionTypes)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == typeof(ItemOptionType))
                .Select(p => p.GetValue(typeof(ItemOptionType)))
                .OfType<ItemOptionType>()
                .ToList();

            foreach (var optionType in optionTypes)
            {
                var persistentOptionType = this.context.CreateNew<ItemOptionType>();
                persistentOptionType.Description = optionType.Description;
                persistentOptionType.Id = optionType.Id;
                persistentOptionType.Name = optionType.Name;
                this.gameConfiguration.ItemOptionTypes.Add(persistentOptionType);
            }
        }

        /// <summary>
        /// Creates the stat attributes.
        /// </summary>
        private void CreateStatAttributes()
        {
            var attributes = typeof(Stats)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == typeof(AttributeDefinition))
                .Select(p => p.GetValue(typeof(Stats)))
                .OfType<AttributeDefinition>()
                .ToList();

            foreach (var attribute in attributes)
            {
                var persistentAttribute = this.context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
                this.gameConfiguration.Attributes.Add(persistentAttribute);
            }
        }

        private void CreateItemSlotTypes()
        {
            var leftHand = this.context.CreateNew<ItemSlotType>();
            leftHand.Description = "Left Hand";
            leftHand.ItemSlots.Add(0);
            this.gameConfiguration.ItemSlotTypes.Add(leftHand);

            var rightHand = this.context.CreateNew<ItemSlotType>();
            rightHand.Description = "Right Hand";
            rightHand.ItemSlots.Add(1);
            this.gameConfiguration.ItemSlotTypes.Add(rightHand);

            var leftOrRightHand = this.context.CreateNew<ItemSlotType>();
            leftOrRightHand.Description = "Left or Right Hand";
            leftOrRightHand.ItemSlots.Add(0);
            leftOrRightHand.ItemSlots.Add(1);
            this.gameConfiguration.ItemSlotTypes.Add(leftOrRightHand);

            var helm = this.context.CreateNew<ItemSlotType>();
            helm.Description = "Helm";
            helm.ItemSlots.Add(2);
            this.gameConfiguration.ItemSlotTypes.Add(helm);

            var armor = this.context.CreateNew<ItemSlotType>();
            armor.Description = "Armor";
            armor.ItemSlots.Add(3);
            this.gameConfiguration.ItemSlotTypes.Add(armor);

            var pants = this.context.CreateNew<ItemSlotType>();
            pants.Description = "Pants";
            pants.ItemSlots.Add(4);
            this.gameConfiguration.ItemSlotTypes.Add(pants);

            var gloves = this.context.CreateNew<ItemSlotType>();
            gloves.Description = "Gloves";
            gloves.ItemSlots.Add(5);
            this.gameConfiguration.ItemSlotTypes.Add(gloves);

            var boots = this.context.CreateNew<ItemSlotType>();
            boots.Description = "Boots";
            boots.ItemSlots.Add(6);
            this.gameConfiguration.ItemSlotTypes.Add(boots);

            var wings = this.context.CreateNew<ItemSlotType>();
            wings.Description = "Wings";
            wings.ItemSlots.Add(7);
            this.gameConfiguration.ItemSlotTypes.Add(wings);

            var pet = this.context.CreateNew<ItemSlotType>();
            pet.Description = "Pet";
            pet.ItemSlots.Add(8);
            this.gameConfiguration.ItemSlotTypes.Add(pet);

            var pendant = this.context.CreateNew<ItemSlotType>();
            pendant.Description = "Pendant";
            pendant.ItemSlots.Add(9);
            this.gameConfiguration.ItemSlotTypes.Add(pendant);

            var ring = this.context.CreateNew<ItemSlotType>();
            ring.Description = "Ring";
            ring.ItemSlots.Add(10);
            ring.ItemSlots.Add(11);
            this.gameConfiguration.ItemSlotTypes.Add(ring);
        }

        private void CreateGameServerDefinitions(GameServerConfiguration gameServerConfiguration, int numberOfServers)
        {
            for (int i = 0; i < numberOfServers; i++)
            {
                var server = this.context.CreateNew<GameServerDefinition>();
                server.ServerID = (byte)i;
                server.Description = $"Server {i}";
                server.NetworkPort = 55901 + i;
                server.GameConfiguration = this.gameConfiguration;
                server.ServerConfiguration = gameServerConfiguration;
            }
        }

        private void CreateGameMapDefinitions()
        {
            var mapInitializers = new[]
            {
                typeof(Lorencia),
                typeof(Dungeon),
                typeof(Devias),
                typeof(Noria),
                typeof(LostTower),
                typeof(Exile),
                typeof(Arena),
                typeof(Atlans),
                typeof(Tarkan),
                typeof(DevilSquare1To4),
                typeof(Icarus),
                typeof(Elvenland),
                typeof(Karutan1),
                typeof(Karutan2),
                typeof(Aida),
                typeof(Vulcanus),
                typeof(CrywolfFortress),
                typeof(LandOfTrials),
                typeof(LorenMarket),
                typeof(SantaVillage),
                typeof(SilentMap),
                typeof(ValleyOfLoren),
                typeof(BarracksOfBalgass),
                typeof(BalgassRefuge),
                typeof(Kalima1),
                typeof(Kalima2),
                typeof(Kalima3),
                typeof(Kalima4),
                typeof(Kalima5),
                typeof(Kalima6),
                typeof(Kalima7),
                typeof(KanturuRelics),
                typeof(KanturuRuins),
                typeof(KanturuEvent),
                typeof(Raklion),
                typeof(RaklionBoss),
                typeof(SwampOfCalmness),
                typeof(DuelArena),
                typeof(BloodCastle1),
                typeof(BloodCastle2),
                typeof(BloodCastle3),
                typeof(BloodCastle4),
                typeof(BloodCastle5),
                typeof(BloodCastle6),
                typeof(BloodCastle7),
                typeof(BloodCastle8),
                typeof(ChaosCastle1),
                typeof(ChaosCastle2),
                typeof(ChaosCastle3),
                typeof(ChaosCastle4),
                typeof(ChaosCastle5),
                typeof(ChaosCastle6),
                typeof(ChaosCastle7),
                typeof(IllusionTemple1),
                typeof(IllusionTemple2),
                typeof(IllusionTemple3),
                typeof(IllusionTemple4),
                typeof(IllusionTemple5),
                typeof(IllusionTemple6),
                typeof(DevilSquare5To7),
                typeof(Doppelgaenger1),
                typeof(Doppelgaenger2),
                typeof(Doppelgaenger3),
                typeof(Doppelgaenger4),
                typeof(FortressOfImperialGuardian1),
                typeof(FortressOfImperialGuardian2),
                typeof(FortressOfImperialGuardian3),
                typeof(FortressOfImperialGuardian4),
            };

            var parameters = new object[] { this.context, this.gameConfiguration };
            foreach (var map in mapInitializers)
            {
                var mapInitializer = map.GetConstructors().First().Invoke(parameters) as IInitializer;
                mapInitializer?.Initialize();
            }
        }

        private GameServerConfiguration CreateGameServerConfiguration(ICollection<GameMapDefinition> maps)
        {
            var gameServerConfiguration = this.context.CreateNew<GameServerConfiguration>();
            gameServerConfiguration.MaximumNPCs = 20000;
            gameServerConfiguration.MaximumPlayers = 1000;

            var mainPacketHandlerConfig = this.context.CreateNew<MainPacketHandlerConfiguration>();
            mainPacketHandlerConfig.ClientVersion = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
            mainPacketHandlerConfig.ClientSerial = Encoding.UTF8.GetBytes("k1Pk2jcET48mxL3b");

            this.CreatePacketHandlerConfiguration().ToList().ForEach(mainPacketHandlerConfig.PacketHandlers.Add);
            gameServerConfiguration.SupportedPacketHandlers.Add(mainPacketHandlerConfig);

            // by default we add every map to a server configuration
            foreach (var map in maps)
            {
                gameServerConfiguration.Maps.Add(map);
            }

            return gameServerConfiguration;
        }

        private IEnumerable<PacketHandlerConfiguration> CreatePacketHandlerConfiguration()
        {
            yield return this.CreatePacketConfig<ChatMessageHandler>(PacketType.Speak);
            yield return this.CreatePacketConfig<ChatMessageHandler>(PacketType.Whisper);
            yield return this.CreatePacketConfig<LoginHandler>(PacketType.LoginLogoutGroup);
            yield return this.CreatePacketConfig<StoreHandler>(PacketType.PersonalShopGroup);
            yield return this.CreatePacketConfig<PickupItemHandler>(PacketType.PickupItem);
            yield return this.CreatePacketConfig<DropItemHandler>(PacketType.DropItem);
            yield return this.CreatePacketConfig<ItemMoveHandler>(PacketType.InventoryMove);
            yield return this.CreatePacketConfig<ConsumeItemHandler>(PacketType.ConsumeItem);
            yield return this.CreatePacketConfig<TalkNpcHandler>(PacketType.TalkNPC);
            yield return this.CreatePacketConfig<CloseNPCHandler>(PacketType.CloseNPC);
            yield return this.CreatePacketConfig<BuyNPCItemHandler>(PacketType.BuyNPCItem);
            yield return this.CreatePacketConfig<SellItemToNPCHandler>(PacketType.SellNPCItem);
            yield return this.CreatePacketConfig<WarpHandler>(PacketType.WarpCommand);
            yield return this.CreatePacketConfig<WarpGateHandler>(PacketType.WarpGate);
            yield return this.CreatePacketConfig<WarehouseCloseHandler>(PacketType.VaultClose);
            yield return this.CreatePacketConfig<JewelMixHandler>(PacketType.JewelMix);

            yield return this.CreatePacketConfig<PartyListRequestHandler>(PacketType.RequestPartyList);
            yield return this.CreatePacketConfig<PartyKickHandler>(PacketType.PartyKick);
            yield return this.CreatePacketConfig<PartyRequestHandler>(PacketType.PartyRequest);
            yield return this.CreatePacketConfig<PartyResponseHandler>(PacketType.PartyRequestAnswer);

            yield return this.CreatePacketConfig<CharacterMoveHandler>(PacketType.Walk);
            yield return this.CreatePacketConfig<CharacterMoveHandler>(PacketType.Teleport);
            yield return this.CreatePacketConfig<AnimationHandler>(PacketType.Animation);
            yield return this.CreatePacketConfig<CharacterGroupHandler>(PacketType.CharacterGroup);

            yield return this.CreatePacketConfig<HitHandler>(PacketType.Hit);
            yield return this.CreatePacketConfig<TargetedSkillHandler>(PacketType.SkillAttack);
            yield return this.CreatePacketConfig<AreaSkillAttackHandler>(PacketType.AreaSkill);
            yield return this.CreatePacketConfig<AreaSkillHitHandler>(PacketType.AreaSkillHit);

            yield return this.CreatePacketConfig<TradeCancelHandler>(PacketType.TradeCancel);
            yield return this.CreatePacketConfig<TradeButtonHandler>(PacketType.TradeButton);
            yield return this.CreatePacketConfig<TradeRequestHandler>(PacketType.TradeRequest);
            yield return this.CreatePacketConfig<TradeAcceptHandler>(PacketType.TradeAccept);
            yield return this.CreatePacketConfig<TradeMoneyHandler>(PacketType.TradeMoney);
            yield return this.CreatePacketConfig<LetterDeleteHandler>(PacketType.FriendMemoDelete);
            yield return this.CreatePacketConfig<LetterSendHandler>(PacketType.FriendMemoSend);
            yield return this.CreatePacketConfig<LetterReadRequestHandler>(PacketType.FriendMemoReadRequest);
            yield return this.CreatePacketConfig<GuildKickPlayerHandler>(PacketType.GuildKickPlayer);
            yield return this.CreatePacketConfig<GuildRequestHandler>(PacketType.GuildJoinRequest);
            yield return this.CreatePacketConfig<GuildRequestAnswerHandler>(PacketType.GuildJoinAnswer);
            yield return this.CreatePacketConfig<GuildListRequestHandler>(PacketType.RequestGuildList);
            yield return this.CreatePacketConfig<GuildCreateHandler>(PacketType.GuildMasterInfoSave);
            yield return this.CreatePacketConfig<GuildMasterAnswerHandler>(PacketType.GuildMasterAnswer);
            yield return this.CreatePacketConfig<GuildInfoRequestHandler>(PacketType.GuildInfoRequest);

            yield return this.CreatePacketConfig<ItemRepairHandler>(PacketType.ItemRepair);
            yield return this.CreatePacketConfig<ChaosMixHandler>(PacketType.ChaosMachineMix);
            yield return this.CreatePacketConfig<AddFriendHandler>(PacketType.FriendAdd);
            yield return this.CreatePacketConfig<DeleteFriendHandler>(PacketType.FriendDelete);
            yield return this.CreatePacketConfig<ChatRequestHandler>(PacketType.ChatRoomCreate);
            yield return this.CreatePacketConfig<ChatRoomInvitationRequest>(PacketType.ChatRoomInvitationReq);
            yield return this.CreatePacketConfig<FriendAddResponseHandler>(PacketType.FriendAddReponse);
            yield return this.CreatePacketConfig<ChangeOnlineStateHandler>(PacketType.FriendStateClient);
        }

        private void InitializeGameConfiguration()
        {
            this.gameConfiguration.MaximumLevel = 400;
            this.gameConfiguration.InfoRange = 12;
            this.gameConfiguration.AreaSkillHitsPlayer = false;
            this.gameConfiguration.MaximumInventoryMoney = int.MaxValue;
            this.gameConfiguration.RecoveryInterval = 3000;
            this.gameConfiguration.MaximumLetters = 50;
            this.gameConfiguration.MaximumCharactersPerAccount = 5;
            this.gameConfiguration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";
            this.gameConfiguration.MaximumPasswordLength = 20;
            this.gameConfiguration.MaximumPartySize = 5;
            this.gameConfiguration.ExperienceTable =
                Enumerable.Range(0, this.gameConfiguration.MaximumLevel + 2)
                    .Select(level => this.CalculateNeededExperience(level))
                    .ToArray();
            this.gameConfiguration.MasterExperienceTable =
                Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();
            var moneyDropItemGroup = this.context.CreateNew<DropItemGroup>();
            moneyDropItemGroup.Chance = 0.5;
            moneyDropItemGroup.ItemType = SpecialItemType.Money;
            this.gameConfiguration.BaseDropItemGroups.Add(moneyDropItemGroup);
            this.CreateStatAttributes();

            this.CreateItemSlotTypes();
            this.CreateItemOptionTypes();
            this.gameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition());
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.DefenseBase));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumPhysBaseDmg));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumWizBaseDmg));
            this.gameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumCurseBaseDmg));

            new CharacterClassInitialization(this.context, this.gameConfiguration).CreateCharacterClasses();
            new SkillsInitializer(this.context, this.gameConfiguration).Initialize();
            new Orbs(this.context, this.gameConfiguration).Initialize();
            new Scrolls(this.context, this.gameConfiguration).Initialize();
            new Wings(this.context, this.gameConfiguration).Initialize();
            new ExcellentOptions(this.context, this.gameConfiguration).Initialize();
            new Armors(this.context, this.gameConfiguration).Initialize();
            new Weapons(this.context, this.gameConfiguration).Initialize();
            new Potions(this.context, this.gameConfiguration).Initialize();
            new Jewels(this.context, this.gameConfiguration).Initialize();
            new Jewellery(this.context, this.gameConfiguration).Initialize();
            this.CreateNpcs();
            this.CreateGameMapDefinitions();
            this.AssignCharacterClassHomeMaps();
            new Gates().Initialize(this.context, this.gameConfiguration);
            //// TODO: ItemSetGroups
            //// TODO: MagicEffects
            //// TODO: MasterSkillRoots
        }

        private void AssignCharacterClassHomeMaps()
        {
            foreach (var characterClass in this.gameConfiguration.CharacterClasses)
            {
                byte mapNumber;
                switch ((CharacterClassNumber)characterClass.Number)
                {
                    case CharacterClassNumber.FairyElf:
                    case CharacterClassNumber.HighElf:
                    case CharacterClassNumber.MuseElf:
                        mapNumber = Noria.Number;
                        break;
                    case CharacterClassNumber.BloodySummoner:
                    case CharacterClassNumber.Summoner:
                        mapNumber = Elvenland.Number;
                        break;
                    default:
                        mapNumber = Lorencia.Number;
                        break;
                }

                characterClass.HomeMap = this.gameConfiguration.Maps.First(map => map.Number == mapNumber);
            }
        }
    }
}
