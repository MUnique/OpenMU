// <copyright file="MerchantStores.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence.Initialization.Items;

    /// <summary>
    /// The initialization of all NPCs, which are no monsters.
    /// </summary>
    internal partial class NpcInitialization
    {
        private ItemStorage CreatePotionGirl()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
                this.itemHelper.CreatePotion(8, 0, 3, 0),     // Apple +0 x3
                this.itemHelper.CreatePotion(16, 0, 1, 1),    // Apple +1 x1
                this.itemHelper.CreatePotion(20, 0, 3, 1),    // Apple +1 x3

                this.itemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
                this.itemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3
                this.itemHelper.CreatePotion(17, 1, 1, 1),    // Small Healing Potion +1 x1
                this.itemHelper.CreatePotion(21, 1, 3, 1),    // Small Healing Potion +1 x3

                this.itemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
                this.itemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3
                this.itemHelper.CreatePotion(18, 2, 1, 1),    // Medium Healing Potion +1 x1
                this.itemHelper.CreatePotion(22, 2, 3, 1),    // Medium Healing Potion +1 x3

                this.itemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
                this.itemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3
                this.itemHelper.CreatePotion(19, 3, 1, 1),    // Large Healing Potion +1 x1
                this.itemHelper.CreatePotion(23, 3, 3, 1),    // Large Healing Potion +1 x3

                this.itemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
                this.itemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

                this.itemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
                this.itemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

                this.itemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
                this.itemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

                this.itemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
                this.itemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

                this.itemHelper.CreateWeapon(24, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
                this.itemHelper.CreateWeapon(25, ItemGroups.Bows, 7, 1, 0, false, false, null), // Bolt +1
                this.itemHelper.CreateWeapon(26, ItemGroups.Bows, 7, 2, 0, false, false, null), // Bolt +2

                this.itemHelper.CreateWeapon(27, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow
                this.itemHelper.CreateWeapon(28, ItemGroups.Bows, 15, 1, 0, false, false, null), // Arrow +1
                this.itemHelper.CreateWeapon(29, ItemGroups.Bows, 15, 2, 0, false, false, null), // Arrow +2

                //// TODO: insert Archer and Spearman

                this.itemHelper.CreateItem(32, 10, 14, 1, 0), // Town Portal Scroll
                this.itemHelper.CreateItem(33, 29, 13, 1, 0), // Armor of Guardsman
            };
            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateWanderingMerchant()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateSetItem(0, 5, ItemGroups.Helm, null, 0, 1, true),     // Leather Helm     +0+4+L
                this.itemHelper.CreateSetItem(16, 5, ItemGroups.Armor, null, 0, 1, true),    // Leather Armor    +0+4+L
                this.itemHelper.CreateSetItem(40, 5, ItemGroups.Pants, null, 0, 1, true),    // Leather Pants    +0+4+L
                this.itemHelper.CreateSetItem(56, 5, ItemGroups.Boots, null, 0, 1, true),   // Leather Boots    +0+4+L
                this.itemHelper.CreateSetItem(72, 5, ItemGroups.Gloves, null, 0, 1, true),   // Leather Gloves   +0+4+L

                this.itemHelper.CreateSetItem(2, 0, ItemGroups.Helm, null, 2, 1, true),     // Bronze Helm    +2+4+L
                this.itemHelper.CreateSetItem(18, 0, ItemGroups.Armor, null, 2, 1, true),    // Bronze Armor   +2+4+L
                this.itemHelper.CreateSetItem(34, 0, ItemGroups.Pants, null, 2, 1, true),    // Bronze Pants   +2+4+L
                this.itemHelper.CreateSetItem(50, 0, ItemGroups.Boots, null, 2, 1, true),   // Bronze Boots   +2+4+L
                this.itemHelper.CreateSetItem(66, 0, ItemGroups.Gloves, null, 2, 1, true),   // Bronze Gloves  +2+4+L

                this.itemHelper.CreateSetItem(4, 6, ItemGroups.Helm, null, 3, 1, true),     // Scale Helm      +3+4+L
                this.itemHelper.CreateSetItem(20, 6, ItemGroups.Armor, null, 3, 1, true),    // Scale Armor     +3+4+L
                this.itemHelper.CreateSetItem(36, 6, ItemGroups.Pants, null, 3, 1, true),    // Scale Pants     +3+4+L
                this.itemHelper.CreateSetItem(52, 6, ItemGroups.Boots, null, 3, 1, true),   // Scale Boots     +3+4+L
                this.itemHelper.CreateSetItem(68, 6, ItemGroups.Gloves, null, 3, 1, true),   // Scale Gloves    +3+4+L

                this.itemHelper.CreateSetItem(6, 8, ItemGroups.Helm, null, 3, 1, true),     // Brass Helm      +3+4+L
                this.itemHelper.CreateSetItem(22, 8, ItemGroups.Armor, null, 3, 1, true),    // Brass Armor     +3+4+L
                this.itemHelper.CreateSetItem(38, 8, ItemGroups.Pants, null, 3, 1, true),    // Brass Pants     +3+4+L
                this.itemHelper.CreateSetItem(54, 8, ItemGroups.Boots, null, 3, 1, true),   // Brass Boots     +3+4+L
                this.itemHelper.CreateSetItem(70, 8, ItemGroups.Gloves, null, 3, 1, true),   // Brass Gloves    +3+4+L

                this.itemHelper.CreateSetItem(88, 9, ItemGroups.Helm, null, 3, 1, true),    // Plate Helm      +3+4+L
                this.itemHelper.CreateSetItem(104, 9, ItemGroups.Armor, null, 3, 1, true),   // Plate Armor     +3+4+L
                this.itemHelper.CreateSetItem(82, 9, ItemGroups.Pants, null, 3, 1, true),    // Plate Pants     +3+4+L
                this.itemHelper.CreateSetItem(98, 9, ItemGroups.Boots, null, 3, 1, true),   // Plate Boots     +3+4+L
                this.itemHelper.CreateSetItem(84, 9, ItemGroups.Gloves, null, 3, 1, true),   // Plate Gloves    +3+4+L
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateHanzoTheBlacksmith()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateShield(0, 0, false, null, 0, 1, true),  // Small Shield     +0+4+L
                this.itemHelper.CreateShield(2, 4, true, null, 1, 1, true),   // Buckler          +1+4+L+S
                this.itemHelper.CreateShield(4, 1, false, null, 2, 1, true),  // Horn Shield      +2+4+L
                this.itemHelper.CreateShield(6, 2, false, null, 3, 1, true),  // Kite Shield      +3+4+L

                this.itemHelper.CreateShield(16, 6, true, null, 3, 1, true),  // Skull Shield     +3+4+L+S
                this.itemHelper.CreateShield(18, 10, true, null, 3, 1, true), // Big Round Shield +3+4+L+S
                this.itemHelper.CreateShield(20, 9, true, null, 3, 1, true),  // Plate Shield     +3+4+L+S
                this.itemHelper.CreateShield(22, 7, true, null, 3, 1, true),  // Spiked Shield    +3+4+L+S

                this.itemHelper.CreateShield(32, 5, true, null, 3, 1, true),  // Dragon Slayer    +3+4+L+S
                this.itemHelper.CreateShield(34, 8, true, null, 3, 1, true),  // Tower Shield     +3+4+L+S
                this.itemHelper.CreateShield(36, 11, true, null, 3, 1, true), // Serpent Shield   +3+4+L+S
                this.itemHelper.CreateShield(38, 12, true, null, 3, 1, true), // Bronze Shield    +3+4+L+S

                this.itemHelper.CreateWeapon(48, ItemGroups.Swords, 1, 0, 1, true, false, null),  // Short Sword  +0+4+L
                this.itemHelper.CreateWeapon(49, ItemGroups.Axes, 1, 1, 1, true, false, null),  // Hand Axe     +1+4+L
                this.itemHelper.CreateWeapon(50, ItemGroups.Scepters, 1, 2, 1, true, false, null),  // Mace         +2+4+L
                this.itemHelper.CreateWeapon(51, ItemGroups.Swords, 2, 2, 1, true, false, null),  // Rapier       +2+4+L
                this.itemHelper.CreateWeapon(52, ItemGroups.Axes, 2, 2, 1, true, false, null),  // Double Axe   +2+4+L
                this.itemHelper.CreateWeapon(53, ItemGroups.Swords, 4, 3, 1, true, true, null),   // Assassin     +3+4+L
                this.itemHelper.CreateWeapon(54, ItemGroups.Scepters, 1, 3, 1, true, true, null),   // Morning Star +3+4+L
                this.itemHelper.CreateWeapon(55, ItemGroups.Axes, 3, 3, 1, true, true, null),   // Tomahawk     +3+4+L

                this.itemHelper.CreateWeapon(72, ItemGroups.Swords, 0, 2, 1, true, false, null),  // Kris             +2+4+L
                this.itemHelper.CreateWeapon(73, ItemGroups.Swords, 6, 3, 1, true, true, null),   // Gladius          +3+4+L+S
                this.itemHelper.CreateWeapon(73, ItemGroups.Swords, 7, 3, 1, true, true, null),   // Falchion         +3+4+L+S
                this.itemHelper.CreateWeapon(74, ItemGroups.Swords, 8, 3, 1, true, false, null),  // Serpent Sword    +3+4+L
                this.itemHelper.CreateWeapon(75, ItemGroups.Swords, 5, 3, 1, true, true, null),   // Blade            +3+4+L+S
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreatePasiTheMageStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateScroll(0, 3),   // Scroll of Fire Ball
                this.itemHelper.CreateScroll(1, 10),  // Scroll of Power Wave
                this.itemHelper.CreateScroll(2, 2),   // Scroll of Lighting
                this.itemHelper.CreateScroll(3, 1),   // Scroll of Meteorite
                this.itemHelper.CreateScroll(4, 5),   // Scroll of Teleport
                this.itemHelper.CreateScroll(5, 6),   // Scroll of Ice
                this.itemHelper.CreateScroll(6, 0),   // Scroll of Poison

                this.itemHelper.CreateOrb(7, 13),     // Orb of Impale
                this.itemHelper.CreateOrb(15, 7),     // Orb of Twisting Slash

                this.itemHelper.CreateSetItem(16, 2, ItemGroups.Helm, null, 0, 1, true),    // Pad Helm     +0+4+L
                this.itemHelper.CreateSetItem(32, 2, ItemGroups.Armor, null, 0, 1, true),    // Pad Armor    +0+4+L
                this.itemHelper.CreateSetItem(48, 2, ItemGroups.Pants, null, 0, 1, true),    // Pad Pants    +0+4+L
                this.itemHelper.CreateSetItem(64, 2, ItemGroups.Boots, null, 0, 1, true),   // Pad Boots    +0+4+L
                this.itemHelper.CreateSetItem(80, 2, ItemGroups.Gloves, null, 0, 1, true),   // Pad Gloves   +0+4+L

                this.itemHelper.CreateSetItem(18, 4, ItemGroups.Helm, null, 2, 1, true),    // Bone Helm    +2+4+L
                this.itemHelper.CreateSetItem(34, 4, ItemGroups.Armor, null, 2, 1, true),    // Bone Armor   +2+4+L
                this.itemHelper.CreateSetItem(50, 4, ItemGroups.Pants, null, 2, 1, true),    // Bone Pants   +2+4+L
                this.itemHelper.CreateSetItem(66, 4, ItemGroups.Boots, null, 2, 1, true),   // Bone Boots   +2+4+L
                this.itemHelper.CreateSetItem(82, 4, ItemGroups.Gloves, null, 2, 1, true),   // Bone Gloves  +2+4+L

                this.itemHelper.CreateSetItem(20, 7, ItemGroups.Helm, null, 3, 1, true),    // Sphinx Helm      +3+4+L
                this.itemHelper.CreateSetItem(36, 7, ItemGroups.Armor, null, 3, 1, true),    // Sphinx Armor     +3+4+L
                this.itemHelper.CreateSetItem(60, 7, ItemGroups.Pants, null, 3, 1, true),    // Sphinx Pants     +3+4+L
                this.itemHelper.CreateSetItem(76, 7, ItemGroups.Boots, null, 3, 1, true),   // Sphinx Boots     +3+4+L
                this.itemHelper.CreateSetItem(92, 7, ItemGroups.Gloves, null, 3, 1, true),   // Sphinx Gloves    +3+4+L

                this.itemHelper.CreateWeapon(22, ItemGroups.Staff, 0, 0, 1, true, false, null),  // Skull Staff      +0+4+L
                this.itemHelper.CreateWeapon(46, ItemGroups.Staff, 1, 2, 1, true, false, null),  // Angelic Staff    +2+4+L
                this.itemHelper.CreateWeapon(70, ItemGroups.Staff, 2, 3, 1, true, false, null),  // Serpent Staff    +3+4+L
                this.itemHelper.CreateWeapon(94, ItemGroups.Staff, 3, 3, 1, true, false, null),  // Thunder Staff    +3+4+L
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateMarceStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateScroll(0, 3),   // Scroll of Fire Ball
                this.itemHelper.CreateScroll(1, 10),  // Scroll of Power Wave
                this.itemHelper.CreateScroll(2, 1),   // Scroll of Meteorite
                this.itemHelper.CreateScroll(3, 6),   // Scroll of Ice
                this.itemHelper.CreateScroll(4, 20),  // Drain Life Parchment
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateElfLalaStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
                this.itemHelper.CreatePotion(8, 0, 3, 0),     // Apple +0 x3
                this.itemHelper.CreatePotion(16, 0, 1, 1),    // Apple +1 x1
                this.itemHelper.CreatePotion(20, 0, 3, 1),    // Apple +1 x3

                this.itemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
                this.itemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3
                this.itemHelper.CreatePotion(17, 1, 1, 1),    // Small Healing Potion +1 x1
                this.itemHelper.CreatePotion(21, 1, 3, 1),    // Small Healing Potion +1 x3

                this.itemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
                this.itemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3
                this.itemHelper.CreatePotion(18, 2, 1, 1),    // Medium Healing Potion +1 x1
                this.itemHelper.CreatePotion(22, 2, 3, 1),    // Medium Healing Potion +1 x3

                this.itemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
                this.itemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3
                this.itemHelper.CreatePotion(19, 3, 1, 1),    // Large Healing Potion +1 x1
                this.itemHelper.CreatePotion(23, 3, 3, 1),    // Large Healing Potion +1 x3

                this.itemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
                this.itemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

                this.itemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
                this.itemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

                this.itemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
                this.itemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

                this.itemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
                this.itemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

                this.itemHelper.CreateOrb(24, 8),
                this.itemHelper.CreateOrb(25, 9),
                this.itemHelper.CreateOrb(26, 10),

                this.itemHelper.CreateSummonOrb(27, 0), // Orb of Summon "Goblin"
                this.itemHelper.CreateSummonOrb(28, 1), // Orb of Summon + 1 "Stone Golem"
                this.itemHelper.CreateSummonOrb(29, 2), // Orb of Summon + 2 "Assassin"
                this.itemHelper.CreateSummonOrb(30, 3), // Orb of Summon + 3 "Elite Yeti"
                this.itemHelper.CreateSummonOrb(31, 4), // Orb of Summon + 4 "Dark Knight"

                this.itemHelper.CreateSetItem(32, 10, ItemGroups.Helm, null, 0, 1, true),    // Vine Helme +0+4+L
                this.itemHelper.CreateSetItem(34, 10, ItemGroups.Armor, null, 0, 1, true),    // Vine Armor +0+4+L
                this.itemHelper.CreateSetItem(36, 10, ItemGroups.Pants, null, 0, 1, true),    // Vine Pants +0+4+L
                this.itemHelper.CreateSetItem(38, 10, ItemGroups.Gloves, null, 3, 1, true),    // Vine Gloves +3+4+L
                this.itemHelper.CreateSetItem(48, 10, ItemGroups.Boots, null, 3, 1, true),    // Vine Boots +3+4+L

                this.itemHelper.CreateSetItem(50, 11, ItemGroups.Helm, null, 2, 1, true),    // Silk Helme +2+4+L
                this.itemHelper.CreateSetItem(52, 11, ItemGroups.Armor, null, 2, 1, true),    // Silk Armor +2+4+L
                this.itemHelper.CreateSetItem(54, 11, ItemGroups.Pants, null, 2, 1, true),    // Silk Pants +2+4+L
                this.itemHelper.CreateSetItem(64, 11, ItemGroups.Gloves, null, 2, 1, true),    // Silk Gloves +2+4+L
                this.itemHelper.CreateSetItem(66, 11, ItemGroups.Boots, null, 2, 1, true),    // Silk Boots +2+4+L

                this.itemHelper.CreateSetItem(68, 12, ItemGroups.Helm, null, 3, 1, true),    // Wind Helme +3+4+L
                this.itemHelper.CreateSetItem(70, 12, ItemGroups.Armor, null, 3, 1, true),    // Wind Armor +3+4+L
                this.itemHelper.CreateSetItem(80, 12, ItemGroups.Pants, null, 3, 1, true),    // Wind Pants +3+4+L
                this.itemHelper.CreateSetItem(82, 12, ItemGroups.Gloves, null, 3, 1, true),    // Wind Gloves +3+4+L
                this.itemHelper.CreateSetItem(84, 12, ItemGroups.Boots, null, 3, 1, true),    // Wind Boots +3+4+L

                this.itemHelper.CreateItem(86, 29, 13, 1, 0), // Armor of Guardsman
                this.itemHelper.CreateItem(96, 10, 14, 1, 0), // Town Portal Scroll

                this.itemHelper.CreateWeapon(97, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
                this.itemHelper.CreateWeapon(98, ItemGroups.Bows, 7, 1, 0, false, false, null), // Bolt +1
                this.itemHelper.CreateWeapon(99, ItemGroups.Bows, 7, 2, 0, false, false, null), // Bolt +2

                this.itemHelper.CreateWeapon(100, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow
                this.itemHelper.CreateWeapon(101, ItemGroups.Bows, 15, 1, 0, false, false, null), // Arrow +1
                this.itemHelper.CreateWeapon(102, ItemGroups.Bows, 15, 2, 0, false, false, null), // Arrow +2
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateAlexStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateWeapon(0, ItemGroups.Spears, 5, 3, 1, true, false, null), // Double Poleaxe +3+Luck+4
                this.itemHelper.CreateWeapon(2, ItemGroups.Spears, 1, 3, 1, true, false, null), // Spear +3+Luck+4
                this.itemHelper.CreateWeapon(4, ItemGroups.Spears, 3, 3, 1, true, false, null), // Giant Trident +3 + Luck + 4
                this.itemHelper.CreateWeapon(6, ItemGroups.Spears, 7, 3, 1, true, true, null), // Berdysh +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(32, ItemGroups.Spears, 4, 3, 1, true, true, null), // Serpent Spear +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(34, ItemGroups.Spears, 0, 3, 1, true, true, null), // Light Spear +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(36, ItemGroups.Swords, 10, 3, 1, true, true, null), // Light Saber +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(38, ItemGroups.Scepters, 3, 3, 1, true, true, null), // Great Hammer +3+Skill+Luck+4
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateIzabelTheWizardStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
                this.itemHelper.CreatePotion(8, 0, 3, 0),     // Apple +0 x3
                this.itemHelper.CreatePotion(16, 0, 1, 1),    // Apple +1 x1
                this.itemHelper.CreatePotion(20, 0, 3, 1),    // Apple +1 x3

                this.itemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
                this.itemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3
                this.itemHelper.CreatePotion(17, 1, 1, 1),    // Small Healing Potion +1 x1
                this.itemHelper.CreatePotion(21, 1, 3, 1),    // Small Healing Potion +1 x3

                this.itemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
                this.itemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3
                this.itemHelper.CreatePotion(18, 2, 1, 1),    // Medium Healing Potion +1 x1
                this.itemHelper.CreatePotion(22, 2, 3, 1),    // Medium Healing Potion +1 x3

                this.itemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
                this.itemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3
                this.itemHelper.CreatePotion(19, 3, 1, 1),    // Large Healing Potion +1 x1
                this.itemHelper.CreatePotion(23, 3, 3, 1),    // Large Healing Potion +1 x3

                this.itemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
                this.itemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

                this.itemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
                this.itemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

                this.itemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
                this.itemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

                this.itemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
                this.itemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

                this.itemHelper.CreateSetItem(24, 3, ItemGroups.Helm, null, 3, 1, true), // Legendary Helm +3+Luck+4
                this.itemHelper.CreateSetItem(26, 3, ItemGroups.Armor, null, 3, 1, true), // Legendary Armor +3+Luck+4
                this.itemHelper.CreateSetItem(28, 3, ItemGroups.Pants, null, 3, 1, true), // Legendary Pants +3+Luck+4
                this.itemHelper.CreateSetItem(30, 3, ItemGroups.Gloves, null, 3, 1, true), // Legendary Glover +3+Luck+4
                this.itemHelper.CreateSetItem(40, 3, ItemGroups.Boots, null, 3, 1, true), // Legendary Boots +3+Luck+4
                this.itemHelper.CreateItem(42, 29, 13, 1, 0), // Armor of Guardsman
                this.itemHelper.CreateItem(44, 10, 14, 1, 0), // Town Portal Scroll

                this.itemHelper.CreateWeapon(45, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
                this.itemHelper.CreateWeapon(46, ItemGroups.Bows, 7, 1, 0, false, false, null), // Bolt +1
                this.itemHelper.CreateWeapon(47, ItemGroups.Bows, 7, 2, 0, false, false, null), // Bolt +2

                this.itemHelper.CreateWeapon(53, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow
                this.itemHelper.CreateWeapon(54, ItemGroups.Bows, 15, 1, 0, false, false, null), // Arrow +1
                this.itemHelper.CreateWeapon(55, ItemGroups.Bows, 15, 2, 0, false, false, null), // Arrow +2

                this.itemHelper.CreateWeapon(56, ItemGroups.Staff, 4, 3, 1, true, false, null), // Gordon Staff +3+Luck+4
                this.itemHelper.CreateWeapon(58, ItemGroups.Staff, 5, 3, 1, true, false, null), // Legendary Staff +3+Luck+4
                this.itemHelper.CreateWeapon(59, ItemGroups.Shields, 14, 3, 2, true, false, null), // Legendary Shield +3+Luck+5

                this.itemHelper.CreateScroll(61, 4), // Scroll of Flame
                this.itemHelper.CreateScroll(62, 7), // Scroll of Twister
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateEoTheCraftsmanStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateSetItem(0, 13, ItemGroups.Helm, null, 3, 1, true), // Spirit Helm +3+Luck+4
                this.itemHelper.CreateSetItem(2, 13, ItemGroups.Armor, null, 3, 1, true), // Spirit Armor +3+Luck+4
                this.itemHelper.CreateSetItem(4, 13, ItemGroups.Pants, null, 3, 1, true), // Spirit Pants +3+Luck+4
                this.itemHelper.CreateSetItem(6, 13, ItemGroups.Gloves, null, 3, 1, true), // Spirit Gloves +3+Luck+4
                this.itemHelper.CreateSetItem(16, 13, ItemGroups.Boots, null, 3, 1, true), // Spirit Boots +3+Luck+4

                this.itemHelper.CreateSetItem(18, 14, ItemGroups.Helm, null, 3, 1, true), // Guardian Helm +3+Luck+4
                this.itemHelper.CreateSetItem(20, 14, ItemGroups.Armor, null, 3, 1, true), // Guardian Armor +3+Luck+4
                this.itemHelper.CreateSetItem(22, 14, ItemGroups.Pants, null, 3, 1, true), // Guardian Pants +3+Luck+4
                this.itemHelper.CreateSetItem(32, 14, ItemGroups.Gloves, null, 3, 1, true), // Guardian Gloves +3+Luck+4
                this.itemHelper.CreateSetItem(34, 14, ItemGroups.Boots, null, 3, 1, true), // Guardian Boots +3+Luck+4

                this.itemHelper.CreateWeapon(36, ItemGroups.Bows, 8, 1, 1, true, true, null), // Crossbow +1+Skill+Luck+4
                this.itemHelper.CreateWeapon(38, ItemGroups.Bows, 9, 3, 1, true, true, null), // Golden Crossbow +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(48, ItemGroups.Bows, 0, 0, 1, true, true, null), // Short Bow +0+Skill+Luck+4
                this.itemHelper.CreateWeapon(50, ItemGroups.Bows, 1, 0, 1, true, true, null), // bow +0+Skill+Luck+4
                this.itemHelper.CreateWeapon(52, ItemGroups.Bows, 2, 2, 1, true, true, null), // Elven Bow +2+Skill+Luck+4
                this.itemHelper.CreateWeapon(54, ItemGroups.Bows, 3, 3, 1, true, true, null), // Battle Bow +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(72, ItemGroups.Bows, 11, 3, 1, true, true, null), // Light Crossbow +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(74, ItemGroups.Bows, 4, 3, 1, true, true, null), // Tiger Bow +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(76, ItemGroups.Bows, 10, 3, 1, true, true, null), // Arquebus +3+Skill+Luck+4

                this.itemHelper.CreateShield(78, 3, false, null, 3, 2, true), // Elven Shield +3+Luck+5
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateRheaStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateSetItem(0, 39, ItemGroups.Helm, null, 2, 1, true), // Violent Wind Helm +2+Luck+4
                this.itemHelper.CreateSetItem(2, 39, ItemGroups.Armor, null, 2, 1, true), // Violent Wind Armor +2+Luck+4
                this.itemHelper.CreateSetItem(4, 39, ItemGroups.Pants, null, 2, 1, true), // Violent Wind Pants +2+Luck+4
                this.itemHelper.CreateSetItem(6, 39, ItemGroups.Gloves, null, 2, 1, true), // Violent Wind Gloves +2+Luck+4
                this.itemHelper.CreateSetItem(16, 39, ItemGroups.Boots, null, 2, 1, true), // Violent Wind Boots +2+Luck+4

                this.itemHelper.CreateSetItem(18, 40, ItemGroups.Helm, null, 3, 1, true), // Red Wing Helm +3+Luck+4
                this.itemHelper.CreateSetItem(20, 40, ItemGroups.Armor, null, 3, 1, true), // Red Wing Armor +3+Luck+4
                this.itemHelper.CreateSetItem(22, 40, ItemGroups.Pants, null, 3, 1, true), // Red Wing Pants +3+Luck+4
                this.itemHelper.CreateSetItem(32, 40, ItemGroups.Gloves, null, 3, 1, true), // Red Wing Gloves +3+Luck+4
                this.itemHelper.CreateSetItem(34, 40, ItemGroups.Boots, null, 3, 1, true), // Red Wing Pants +3+Luck+4

                this.itemHelper.CreateWeapon(36, ItemGroups.Axes, 0, 1, 1, true, false, null), // Small Axe +1+Luck+4
                this.itemHelper.CreateWeapon(37, ItemGroups.Swords, 1, 0, 1, true, false, null), // Short Sword +0+Luck+4
                this.itemHelper.CreateWeapon(38, ItemGroups.Axes, 1, 1, 1, true, false, null), // Hand Axe +1+Luck+4
                this.itemHelper.CreateWeapon(39, ItemGroups.Swords, 2, 2, 1, true, false, null), // Rapier +2+Luck+4

                this.itemHelper.CreateWeapon(48, ItemGroups.Axes, 4, 1, 1, true, false, null), // Elven Axe +1+Luck+4
                this.itemHelper.CreateWeapon(49, ItemGroups.Swords, 0, 1, 1, true, false, null), // Kris +1+Luck+4
                this.itemHelper.CreateWeapon(50, ItemGroups.Staff, 0, 1, 1, true, false, null), // Skull Staff +1+Luck+4
                this.itemHelper.CreateWeapon(51, ItemGroups.Staff, 14, 1, 1, true, false, null), // Mystery Stick +1+Luck+4

                this.itemHelper.CreateWeapon(60, ItemGroups.Staff, 15, 2, 1, true, false, null), // Violent Wind Stick +2+Luck+4
                this.itemHelper.CreateWeapon(61, ItemGroups.Staff, 16, 3, 1, true, false, null), // Red Wing Stick +3+Luck+4
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateZiennaStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateSetItem(0, 1, ItemGroups.Helm, null, 3, 1, true), // Dragon Helm +3+Luck+4
                this.itemHelper.CreateSetItem(2, 1, ItemGroups.Armor, null, 3, 1, true), // Dragon Armor +3+Luck+4
                this.itemHelper.CreateSetItem(4, 1, ItemGroups.Pants, null, 3, 1, true), // Dragon Pants +3+Luck+4
                this.itemHelper.CreateSetItem(6, 1, ItemGroups.Gloves, null, 3, 1, true), // Dragon Gloves +3+Luck+4
                this.itemHelper.CreateSetItem(16, 1, ItemGroups.Boots, null, 3, 1, true), // Dragon Boots +3+Luck+4

                this.itemHelper.CreateWeapon(20, ItemGroups.Swords, 9, 3, 1, true, true, null), // Sword of Salamander +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(22, ItemGroups.Swords, 11, 3, 1, true, true, null), // Legendary Sword +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(32, ItemGroups.Swords, 13, 3, 1, true, true, null), // Double Blade +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(33, ItemGroups.Swords, 14, 3, 1, true, true, null), // Lighting Sword +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(26, ItemGroups.Swords, 15, 3, 1, true, true, null), // Giant Sword +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(44, ItemGroups.Swords, 12, 3, 1, true, true, null), // Heliacal Sword +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(46, ItemGroups.Bows, 12, 3, 1, true, true, null), // Serpent Crossbow +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(56, ItemGroups.Spears, 9, 3, 1, true, true, null), // Bill of Balrog +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(50, ItemGroups.Spears, 8, 3, 1, true, true, null), // Great Scythe +3+Skill+Luck+4

                this.itemHelper.CreateWeapon(68, ItemGroups.Bows, 5, 3, 1, true, true, null), // Silver Bow +3+Skill+Luck+4
                this.itemHelper.CreateWeapon(70, ItemGroups.Bows, 13, 3, 1, true, true, null), // Bluewing Crossbow +3+Skill+Luck+4
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateBoloStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateWeapon(0, ItemGroups.Swords, 16, 3, 3, true, false, null), // Sword of Destruction +3+Luck+12
                this.itemHelper.CreateWeapon(1, ItemGroups.Scepters, 8, 3, 3, true, true, null), // Battle Scepter +3+Skill+Luck+12
                this.itemHelper.CreateWeapon(2, ItemGroups.Scepters, 9, 3, 3, true, true, null), // Master Scepter +3+Skill+Luck+12
                this.itemHelper.CreateWeapon(3, ItemGroups.Bows, 14, 3, 3, true, true, null), // Aquagold Crossbow +3+Skill+Luck+12
                this.itemHelper.CreateWeapon(5, ItemGroups.Bows, 16, 3, 3, true, true, null), // Saint's Crossbow +3+Skill+Luck+12
                this.itemHelper.CreateWeapon(7, ItemGroups.Staff, 6, 3, 3, true, false, null), // Staff of Resurrection +3+Luck+12
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateLumenTheBarmaidStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreateItem(0, 17, 13, 1, 1), // Blood Bone + 1
                this.itemHelper.CreateItem(1, 17, 13, 1, 2), // Blood Bone + 2
                this.itemHelper.CreateItem(2, 17, 13, 1, 3), // Blood Bone + 3
                this.itemHelper.CreateItem(3, 17, 13, 1, 4), // Blood Bone + 4
                this.itemHelper.CreateItem(4, 17, 13, 1, 5), // Blood Bone + 5
                this.itemHelper.CreateItem(5, 17, 13, 1, 6), // Blood Bone+  6
                this.itemHelper.CreateItem(6, 17, 13, 1, 7), // Blood Bone + 7
                this.itemHelper.CreateItem(7, 17, 13, 1, 8), // Blood Bone + 8
                this.itemHelper.CreateItem(16, 16, 13, 1, 1), // Scroll of Archangel + 1
                this.itemHelper.CreateItem(17, 16, 13, 1, 2), // Scroll of Archangel + 2
                this.itemHelper.CreateItem(18, 16, 13, 1, 3), // Scroll of Archangel + 3
                this.itemHelper.CreateItem(19, 16, 13, 1, 4), // Scroll of Archangel + 4
                this.itemHelper.CreateItem(20, 16, 13, 1, 5), // Scroll of Archangel + 5
                this.itemHelper.CreateItem(21, 16, 13, 1, 6), // Scroll of Archangel + 6
                this.itemHelper.CreateItem(22, 16, 13, 1, 7), // Scroll of Archangel + 7
                this.itemHelper.CreateItem(23, 16, 13, 1, 8), // Scroll of Archangel + 8

                this.itemHelper.CreateItem(32, 17, 14, 1, 1), // Devils Eye + 1
                this.itemHelper.CreateItem(33, 17, 14, 1, 2), // Devils Eye + 2
                this.itemHelper.CreateItem(34, 17, 14, 1, 3), // Devils Eye + 3
                this.itemHelper.CreateItem(35, 17, 14, 1, 4), // Devils Eye + 4
                this.itemHelper.CreateItem(36, 17, 14, 1, 5), // Devils Eye + 5
                this.itemHelper.CreateItem(37, 17, 14, 1, 6), // Devils Eye + 6
                this.itemHelper.CreateItem(38, 17, 14, 1, 7), // Devils Eye + 7
                this.itemHelper.CreateItem(39, 10, 14, 1, 0), // Town Portal Scroll
                this.itemHelper.CreateItem(40, 18, 14, 1, 1), // Devils key + 1
                this.itemHelper.CreateItem(41, 18, 14, 1, 2), // Devils key + 2
                this.itemHelper.CreateItem(42, 18, 14, 1, 3), // Devils key + 3
                this.itemHelper.CreateItem(43, 18, 14, 1, 4), // Devils key + 4
                this.itemHelper.CreateItem(44, 18, 14, 1, 5), // Devils key + 5
                this.itemHelper.CreateItem(45, 18, 14, 1, 6), // Devils key + 6
                this.itemHelper.CreateItem(46, 18, 14, 1, 7), // Devils key + 7

                this.itemHelper.CreateItem(48, 50, 13, 1, 1), // Illusion Sorcerer Covenant
                this.itemHelper.CreateItem(49, 50, 13, 1, 2), // Illusion Sorcerer Covenant
                this.itemHelper.CreateItem(50, 50, 13, 1, 3), // Illusion Sorcerer Covenant
                this.itemHelper.CreateItem(51, 50, 13, 1, 4), // Illusion Sorcerer Covenant
                this.itemHelper.CreateItem(52, 50, 13, 1, 5), // Illusion Sorcerer Covenant

                this.itemHelper.CreateItem(53, 49, 13, 1, 1), // Old Scroll
                this.itemHelper.CreateItem(54, 49, 13, 1, 2), // Old Scroll
                this.itemHelper.CreateItem(55, 49, 13, 1, 3), // Old Scroll
                this.itemHelper.CreateItem(61, 49, 13, 1, 4), // Old Scroll
                this.itemHelper.CreateItem(62, 49, 13, 1, 5), // Old Scroll
                this.itemHelper.CreatePotion(64, 9, 1, 0), // Ale
                this.itemHelper.CreateItem(65, 29, 13, 1, 0), // Armor of Guardsman
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateCarenTheBarmaidStore()
        {
            List<Item> itemList = new List<Item>
            {
                this.itemHelper.CreatePotion(0, 9, 1, 0), // Ale
                this.itemHelper.CreateItem(1, 10, 14, 1, 0), // Town Portal Scroll
                this.itemHelper.CreateItem(2, 16, 13, 1, 1), // Scroll of Archangel + 1
                this.itemHelper.CreateItem(3, 16, 13, 1, 2), // Scroll of Archangel + 2
                this.itemHelper.CreateItem(4, 17, 13, 1, 1), // Blood Bone + 1
                this.itemHelper.CreateItem(5, 17, 13, 1, 2), // Blood Bone + 2
                this.itemHelper.CreateItem(6, 17, 14, 1, 1), // Devils Eye + 1
                this.itemHelper.CreateItem(7, 17, 14, 1, 2), // Devils Eye + 2
                this.itemHelper.CreateItem(14, 18, 14, 1, 1), // Devils key + 1
                this.itemHelper.CreateItem(15, 18, 14, 1, 2), // Devils key + 2
                this.itemHelper.CreateItem(16, 29, 13, 1, 0), // Armor of Guardsman
            };

            return this.CreateMerchantStore(itemList);
        }

        private ItemStorage CreateMerchantStore(List<Item> itemList)
        {
            var merchantStore = this.context.CreateNew<ItemStorage>();

            foreach (var item in itemList)
            {
                merchantStore.Items.Add(item);
            }

            return merchantStore;
        }
    }
}