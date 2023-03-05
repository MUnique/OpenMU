// <copyright file="MerchantStores.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal partial class NpcInitialization
{
    /// <summary>
    /// Creates the merchant store of 'Potion Girl'.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>
    /// The created store.
    /// </returns>
    protected virtual ItemStorage CreatePotionGirlItemStorage(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
            this.ItemHelper.CreatePotion(8, 0, 3, 0),    // Apple +0 x3

            this.ItemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
            this.ItemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3

            this.ItemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
            this.ItemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3

            this.ItemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
            this.ItemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3

            this.ItemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
            this.ItemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

            this.ItemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
            this.ItemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

            this.ItemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
            this.ItemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

            this.ItemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
            this.ItemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

            this.ItemHelper.CreateWeapon(16, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
            this.ItemHelper.CreateWeapon(17, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow
            this.ItemHelper.CreateItem(18, 10, 14, 1, 0), // Town Portal Scroll
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the wandering merchant store.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <returns>
    /// The created store.
    /// </returns>
    protected virtual ItemStorage CreateWanderingMerchant(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreateSetItem(0, 5, ItemGroups.Helm, null, 0, 1, true),     // Leather Helm     +0+4+L
            this.ItemHelper.CreateSetItem(16, 5, ItemGroups.Armor, null, 0, 1, true),    // Leather Armor    +0+4+L
            this.ItemHelper.CreateSetItem(40, 5, ItemGroups.Pants, null, 0, 1, true),    // Leather Pants    +0+4+L
            this.ItemHelper.CreateSetItem(56, 5, ItemGroups.Boots, null, 0, 1, true),   // Leather Boots    +0+4+L
            this.ItemHelper.CreateSetItem(72, 5, ItemGroups.Gloves, null, 0, 1, true),   // Leather Gloves   +0+4+L

            this.ItemHelper.CreateSetItem(2, 0, ItemGroups.Helm, null, 2, 1, true),     // Bronze Helm    +2+4+L
            this.ItemHelper.CreateSetItem(18, 0, ItemGroups.Armor, null, 2, 1, true),    // Bronze Armor   +2+4+L
            this.ItemHelper.CreateSetItem(34, 0, ItemGroups.Pants, null, 2, 1, true),    // Bronze Pants   +2+4+L
            this.ItemHelper.CreateSetItem(50, 0, ItemGroups.Boots, null, 2, 1, true),   // Bronze Boots   +2+4+L
            this.ItemHelper.CreateSetItem(66, 0, ItemGroups.Gloves, null, 2, 1, true),   // Bronze Gloves  +2+4+L

            this.ItemHelper.CreateSetItem(4, 6, ItemGroups.Helm, null, 3, 1, true),     // Scale Helm      +3+4+L
            this.ItemHelper.CreateSetItem(20, 6, ItemGroups.Armor, null, 3, 1, true),    // Scale Armor     +3+4+L
            this.ItemHelper.CreateSetItem(36, 6, ItemGroups.Pants, null, 3, 1, true),    // Scale Pants     +3+4+L
            this.ItemHelper.CreateSetItem(52, 6, ItemGroups.Boots, null, 3, 1, true),   // Scale Boots     +3+4+L
            this.ItemHelper.CreateSetItem(68, 6, ItemGroups.Gloves, null, 3, 1, true),   // Scale Gloves    +3+4+L

            this.ItemHelper.CreateSetItem(6, 8, ItemGroups.Helm, null, 3, 1, true),     // Brass Helm      +3+4+L
            this.ItemHelper.CreateSetItem(22, 8, ItemGroups.Armor, null, 3, 1, true),    // Brass Armor     +3+4+L
            this.ItemHelper.CreateSetItem(38, 8, ItemGroups.Pants, null, 3, 1, true),    // Brass Pants     +3+4+L
            this.ItemHelper.CreateSetItem(54, 8, ItemGroups.Boots, null, 3, 1, true),   // Brass Boots     +3+4+L
            this.ItemHelper.CreateSetItem(70, 8, ItemGroups.Gloves, null, 3, 1, true),   // Brass Gloves    +3+4+L

            this.ItemHelper.CreateSetItem(88, 9, ItemGroups.Helm, null, 3, 1, true),    // Plate Helm      +3+4+L
            this.ItemHelper.CreateSetItem(104, 9, ItemGroups.Armor, null, 3, 1, true),   // Plate Armor     +3+4+L
            this.ItemHelper.CreateSetItem(82, 9, ItemGroups.Pants, null, 3, 1, true),    // Plate Pants     +3+4+L
            this.ItemHelper.CreateSetItem(98, 9, ItemGroups.Boots, null, 3, 1, true),   // Plate Boots     +3+4+L
            this.ItemHelper.CreateSetItem(84, 9, ItemGroups.Gloves, null, 3, 1, true),   // Plate Gloves    +3+4+L
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Hanzo the Blacksmith'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateHanzoTheBlacksmith(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreateShield(0, 0, false, null, 0, 1, true),  // Small Shield     +0+4+L
            this.ItemHelper.CreateShield(2, 4, true, null, 1, 1, true),   // Buckler          +1+4+L+S
            this.ItemHelper.CreateShield(4, 1, false, null, 2, 1, true),  // Horn Shield      +2+4+L
            this.ItemHelper.CreateShield(6, 2, false, null, 3, 1, true),  // Kite Shield      +3+4+L

            this.ItemHelper.CreateShield(16, 6, true, null, 3, 1, true),  // Skull Shield     +3+4+L+S
            this.ItemHelper.CreateShield(18, 10, true, null, 3, 1, true), // Big Round Shield +3+4+L+S
            this.ItemHelper.CreateShield(20, 9, true, null, 3, 1, true),  // Plate Shield     +3+4+L+S
            this.ItemHelper.CreateShield(22, 7, true, null, 3, 1, true),  // Spiked Shield    +3+4+L+S

            this.ItemHelper.CreateShield(32, 5, true, null, 3, 1, true),  // Dragon Slayer    +3+4+L+S
            this.ItemHelper.CreateShield(34, 8, true, null, 3, 1, true),  // Tower Shield     +3+4+L+S
            this.ItemHelper.CreateShield(36, 11, true, null, 3, 1, true), // Serpent Shield   +3+4+L+S
            this.ItemHelper.CreateShield(38, 12, true, null, 3, 1, true), // Bronze Shield    +3+4+L+S

            this.ItemHelper.CreateWeapon(48, ItemGroups.Swords, 1, 0, 1, true, false, null),  // Short Sword  +0+4+L
            this.ItemHelper.CreateWeapon(49, ItemGroups.Axes, 1, 1, 1, true, false, null),  // Hand Axe     +1+4+L
            this.ItemHelper.CreateWeapon(50, ItemGroups.Scepters, 1, 2, 1, true, false, null),  // Mace         +2+4+L
            this.ItemHelper.CreateWeapon(51, ItemGroups.Swords, 2, 2, 1, true, false, null),  // Rapier       +2+4+L
            this.ItemHelper.CreateWeapon(52, ItemGroups.Axes, 2, 2, 1, true, false, null),  // Double Axe   +2+4+L
            this.ItemHelper.CreateWeapon(53, ItemGroups.Swords, 4, 3, 1, true, true, null),   // Assassin     +3+4+L
            this.ItemHelper.CreateWeapon(54, ItemGroups.Scepters, 1, 3, 1, true, true, null),   // Morning Star +3+4+L
            this.ItemHelper.CreateWeapon(55, ItemGroups.Axes, 3, 3, 1, true, true, null),   // Tomahawk     +3+4+L

            this.ItemHelper.CreateWeapon(72, ItemGroups.Swords, 0, 2, 1, true, false, null),  // Kris             +2+4+L
            this.ItemHelper.CreateWeapon(73, ItemGroups.Swords, 6, 3, 1, true, true, null),   // Gladius          +3+4+L+S
            this.ItemHelper.CreateWeapon(73, ItemGroups.Swords, 7, 3, 1, true, true, null),   // Falchion         +3+4+L+S
            this.ItemHelper.CreateWeapon(74, ItemGroups.Swords, 8, 3, 1, true, false, null),  // Serpent Sword    +3+4+L
            this.ItemHelper.CreateWeapon(75, ItemGroups.Swords, 5, 3, 1, true, true, null),   // Blade            +3+4+L+S
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Pasi the Mage'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreatePasiTheMageStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreateScroll(0, 3),   // Scroll of Fire Ball
            this.ItemHelper.CreateScroll(1, 10),  // Scroll of Power Wave
            this.ItemHelper.CreateScroll(2, 2),   // Scroll of Lighting
            this.ItemHelper.CreateScroll(3, 1),   // Scroll of Meteorite
            this.ItemHelper.CreateScroll(4, 5),   // Scroll of Teleport
            this.ItemHelper.CreateScroll(5, 6),   // Scroll of Ice
            this.ItemHelper.CreateScroll(6, 0),   // Scroll of Poison

            this.ItemHelper.CreateSetItem(16, 2, ItemGroups.Helm, null, 0, 1, true),    // Pad Helm     +0+4+L
            this.ItemHelper.CreateSetItem(32, 2, ItemGroups.Armor, null, 0, 1, true),    // Pad Armor    +0+4+L
            this.ItemHelper.CreateSetItem(48, 2, ItemGroups.Pants, null, 0, 1, true),    // Pad Pants    +0+4+L
            this.ItemHelper.CreateSetItem(64, 2, ItemGroups.Boots, null, 0, 1, true),   // Pad Boots    +0+4+L
            this.ItemHelper.CreateSetItem(80, 2, ItemGroups.Gloves, null, 0, 1, true),   // Pad Gloves   +0+4+L

            this.ItemHelper.CreateSetItem(18, 4, ItemGroups.Helm, null, 2, 1, true),    // Bone Helm    +2+4+L
            this.ItemHelper.CreateSetItem(34, 4, ItemGroups.Armor, null, 2, 1, true),    // Bone Armor   +2+4+L
            this.ItemHelper.CreateSetItem(50, 4, ItemGroups.Pants, null, 2, 1, true),    // Bone Pants   +2+4+L
            this.ItemHelper.CreateSetItem(66, 4, ItemGroups.Boots, null, 2, 1, true),   // Bone Boots   +2+4+L
            this.ItemHelper.CreateSetItem(82, 4, ItemGroups.Gloves, null, 2, 1, true),   // Bone Gloves  +2+4+L

            this.ItemHelper.CreateSetItem(20, 7, ItemGroups.Helm, null, 3, 1, true),    // Sphinx Helm      +3+4+L
            this.ItemHelper.CreateSetItem(36, 7, ItemGroups.Armor, null, 3, 1, true),    // Sphinx Armor     +3+4+L
            this.ItemHelper.CreateSetItem(60, 7, ItemGroups.Pants, null, 3, 1, true),    // Sphinx Pants     +3+4+L
            this.ItemHelper.CreateSetItem(76, 7, ItemGroups.Boots, null, 3, 1, true),   // Sphinx Boots     +3+4+L
            this.ItemHelper.CreateSetItem(92, 7, ItemGroups.Gloves, null, 3, 1, true),   // Sphinx Gloves    +3+4+L

            this.ItemHelper.CreateWeapon(22, ItemGroups.Staff, 0, 0, 1, true, false, null),  // Skull Staff      +0+4+L
            this.ItemHelper.CreateWeapon(46, ItemGroups.Staff, 1, 2, 1, true, false, null),  // Angelic Staff    +2+4+L
            this.ItemHelper.CreateWeapon(70, ItemGroups.Staff, 2, 3, 1, true, false, null),  // Serpent Staff    +3+4+L
            this.ItemHelper.CreateWeapon(94, ItemGroups.Staff, 3, 3, 1, true, false, null),  // Thunder Staff    +3+4+L
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Elf Lala'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateElfLalaStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
            this.ItemHelper.CreatePotion(8, 0, 3, 0),     // Apple +0 x3

            this.ItemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
            this.ItemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3

            this.ItemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
            this.ItemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3

            this.ItemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
            this.ItemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3

            this.ItemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
            this.ItemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

            this.ItemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
            this.ItemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

            this.ItemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
            this.ItemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

            this.ItemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
            this.ItemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

            this.ItemHelper.CreateOrb(16, 8),
            this.ItemHelper.CreateOrb(17, 9),
            this.ItemHelper.CreateOrb(18, 10),

            this.ItemHelper.CreateItem(21, 10, 14, 1, 0), // Town Portal Scroll
            this.ItemHelper.CreateWeapon(22, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
            this.ItemHelper.CreateWeapon(23, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow

            this.ItemHelper.CreateSummonOrb(24, 0), // Orb of Summon "Goblin"
            this.ItemHelper.CreateSummonOrb(25, 1), // Orb of Summon + 1 "Stone Golem"
            this.ItemHelper.CreateSummonOrb(26, 2), // Orb of Summon + 2 "Assassin"
            this.ItemHelper.CreateSummonOrb(27, 3), // Orb of Summon + 3 "Elite Yeti"
            this.ItemHelper.CreateSummonOrb(28, 4), // Orb of Summon + 4 "Dark Knight"

            this.ItemHelper.CreateSetItem(32, 10, ItemGroups.Helm, null, 0, 1, true),    // Vine Helme +0+4+L
            this.ItemHelper.CreateSetItem(34, 10, ItemGroups.Armor, null, 0, 1, true),    // Vine Armor +0+4+L
            this.ItemHelper.CreateSetItem(36, 10, ItemGroups.Pants, null, 0, 1, true),    // Vine Pants +0+4+L
            this.ItemHelper.CreateSetItem(38, 10, ItemGroups.Gloves, null, 3, 1, true),    // Vine Gloves +3+4+L
            this.ItemHelper.CreateSetItem(48, 10, ItemGroups.Boots, null, 3, 1, true),    // Vine Boots +3+4+L

            this.ItemHelper.CreateSetItem(50, 11, ItemGroups.Helm, null, 2, 1, true),    // Silk Helme +2+4+L
            this.ItemHelper.CreateSetItem(52, 11, ItemGroups.Armor, null, 2, 1, true),    // Silk Armor +2+4+L
            this.ItemHelper.CreateSetItem(54, 11, ItemGroups.Pants, null, 2, 1, true),    // Silk Pants +2+4+L
            this.ItemHelper.CreateSetItem(64, 11, ItemGroups.Gloves, null, 2, 1, true),    // Silk Gloves +2+4+L
            this.ItemHelper.CreateSetItem(66, 11, ItemGroups.Boots, null, 2, 1, true),    // Silk Boots +2+4+L

            this.ItemHelper.CreateSetItem(68, 12, ItemGroups.Helm, null, 3, 1, true),    // Wind Helme +3+4+L
            this.ItemHelper.CreateSetItem(70, 12, ItemGroups.Armor, null, 3, 1, true),    // Wind Armor +3+4+L
            this.ItemHelper.CreateSetItem(80, 12, ItemGroups.Pants, null, 3, 1, true),    // Wind Pants +3+4+L
            this.ItemHelper.CreateSetItem(82, 12, ItemGroups.Gloves, null, 3, 1, true),    // Wind Gloves +3+4+L
            this.ItemHelper.CreateSetItem(84, 12, ItemGroups.Boots, null, 3, 1, true),    // Wind Boots +3+4+L

        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Izabel the Wizard'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateIzabelTheWizardStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreatePotion(0, 0, 1, 0),     // Apple +0 x1
            this.ItemHelper.CreatePotion(8, 0, 3, 0),     // Apple +0 x3

            this.ItemHelper.CreatePotion(1, 1, 1, 0),     // Small Healing Potion +0 x1
            this.ItemHelper.CreatePotion(9, 1, 3, 0),     // Small Healing Potion +0 x3

            this.ItemHelper.CreatePotion(2, 2, 1, 0),     // Medium Healing Potion +0 x1
            this.ItemHelper.CreatePotion(10, 2, 3, 0),    // Medium Healing Potion +0 x3

            this.ItemHelper.CreatePotion(3, 3, 1, 0),     // Large Healing Potion +0 x1
            this.ItemHelper.CreatePotion(11, 3, 3, 0),    // Large Healing Potion +0 x3

            this.ItemHelper.CreatePotion(4, 4, 1, 0),     // Small Mana Potion +0 x1
            this.ItemHelper.CreatePotion(12, 4, 3, 0),    // Small Mana Potion +0 x3

            this.ItemHelper.CreatePotion(5, 5, 1, 0),     // Medium Mana Potion +0 x1
            this.ItemHelper.CreatePotion(13, 5, 3, 0),    // Medium Mana Potion +0 x3

            this.ItemHelper.CreatePotion(6, 6, 1, 0),     // Large Mana Potion +0 x1
            this.ItemHelper.CreatePotion(14, 6, 3, 0),    // Large Mana Potion +0 x3

            this.ItemHelper.CreatePotion(7, 8, 1, 0),    // Antidote +0 x1
            this.ItemHelper.CreatePotion(15, 8, 3, 0),    // Antidote +0 x3

            this.ItemHelper.CreateSetItem(16, 3, ItemGroups.Helm, null, 3, 1, true), // Legendary Helm +3+Luck+4
            this.ItemHelper.CreateSetItem(18, 3, ItemGroups.Armor, null, 3, 1, true), // Legendary Armor +3+Luck+4
            this.ItemHelper.CreateSetItem(20, 3, ItemGroups.Pants, null, 3, 1, true), // Legendary Pants +3+Luck+4
            this.ItemHelper.CreateSetItem(22, 3, ItemGroups.Gloves, null, 3, 1, true), // Legendary Glover +3+Luck+4
            this.ItemHelper.CreateSetItem(32, 3, ItemGroups.Boots, null, 3, 1, true), // Legendary Boots +3+Luck+4
            this.ItemHelper.CreateItem(34, 10, 14, 1, 0), // Town Portal Scroll
            this.ItemHelper.CreateWeapon(35, ItemGroups.Bows, 7, 0, 0, false, false, null), // Bolt
            this.ItemHelper.CreateWeapon(36, ItemGroups.Bows, 15, 0, 0, false, false, null), // Arrow
            this.ItemHelper.CreateScroll(37, 4), // Scroll of Flame
            this.ItemHelper.CreateScroll(38, 7), // Scroll of Twister

            this.ItemHelper.CreateWeapon(48, ItemGroups.Staff, 4, 3, 1, true, false, null), // Gordon Staff +3+Luck+4
            this.ItemHelper.CreateWeapon(50, ItemGroups.Staff, 5, 3, 1, true, false, null), // Legendary Staff +3+Luck+4
            this.ItemHelper.CreateWeapon(52, ItemGroups.Shields, 14, 3, 2, true, false, null), // Legendary Shield +3+Luck+5
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Eo the Craftsman'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateEoTheCraftsmanStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreateSetItem(0, 13, ItemGroups.Helm, null, 3, 1, true), // Spirit Helm +3+Luck+4
            this.ItemHelper.CreateSetItem(2, 13, ItemGroups.Armor, null, 3, 1, true), // Spirit Armor +3+Luck+4
            this.ItemHelper.CreateSetItem(4, 13, ItemGroups.Pants, null, 3, 1, true), // Spirit Pants +3+Luck+4
            this.ItemHelper.CreateSetItem(6, 13, ItemGroups.Gloves, null, 3, 1, true), // Spirit Gloves +3+Luck+4
            this.ItemHelper.CreateSetItem(16, 13, ItemGroups.Boots, null, 3, 1, true), // Spirit Boots +3+Luck+4

            this.ItemHelper.CreateSetItem(18, 14, ItemGroups.Helm, null, 3, 1, true), // Guardian Helm +3+Luck+4
            this.ItemHelper.CreateSetItem(20, 14, ItemGroups.Armor, null, 3, 1, true), // Guardian Armor +3+Luck+4
            this.ItemHelper.CreateSetItem(22, 14, ItemGroups.Pants, null, 3, 1, true), // Guardian Pants +3+Luck+4
            this.ItemHelper.CreateSetItem(32, 14, ItemGroups.Gloves, null, 3, 1, true), // Guardian Gloves +3+Luck+4
            this.ItemHelper.CreateSetItem(34, 14, ItemGroups.Boots, null, 3, 1, true), // Guardian Boots +3+Luck+4

            this.ItemHelper.CreateWeapon(36, ItemGroups.Bows, 8, 1, 1, true, true, null), // Crossbow +1+Skill+Luck+4
            this.ItemHelper.CreateWeapon(38, ItemGroups.Bows, 9, 3, 1, true, true, null), // Golden Crossbow +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(48, ItemGroups.Bows, 0, 0, 1, true, true, null), // Short Bow +0+Skill+Luck+4
            this.ItemHelper.CreateWeapon(50, ItemGroups.Bows, 1, 0, 1, true, true, null), // bow +0+Skill+Luck+4
            this.ItemHelper.CreateWeapon(52, ItemGroups.Bows, 2, 2, 1, true, true, null), // Elven Bow +2+Skill+Luck+4
            this.ItemHelper.CreateWeapon(54, ItemGroups.Bows, 3, 3, 1, true, true, null), // Battle Bow +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(72, ItemGroups.Bows, 11, 3, 1, true, true, null), // Light Crossbow +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(74, ItemGroups.Bows, 4, 3, 1, true, true, null), // Tiger Bow +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(76, ItemGroups.Bows, 10, 3, 1, true, true, null), // Arquebus +3+Skill+Luck+4

            this.ItemHelper.CreateShield(78, 3, false, null, 3, 2, true), // Elven Shield +3+Luck+5
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Zienna'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateZiennaStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreateSetItem(0, 1, ItemGroups.Helm, null, 3, 1, true), // Dragon Helm +3+Luck+4
            this.ItemHelper.CreateSetItem(2, 1, ItemGroups.Armor, null, 3, 1, true), // Dragon Armor +3+Luck+4
            this.ItemHelper.CreateSetItem(4, 1, ItemGroups.Pants, null, 3, 1, true), // Dragon Pants +3+Luck+4
            this.ItemHelper.CreateSetItem(6, 1, ItemGroups.Gloves, null, 3, 1, true), // Dragon Gloves +3+Luck+4
            this.ItemHelper.CreateSetItem(16, 1, ItemGroups.Boots, null, 3, 1, true), // Dragon Boots +3+Luck+4

            this.ItemHelper.CreateWeapon(20, ItemGroups.Swords, 9, 3, 1, true, true, null), // Sword of Salamander +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(22, ItemGroups.Swords, 11, 3, 1, true, true, null), // Legendary Sword +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(32, ItemGroups.Swords, 13, 3, 1, true, true, null), // Double Blade +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(33, ItemGroups.Swords, 14, 3, 1, true, true, null), // Lighting Sword +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(26, ItemGroups.Swords, 15, 3, 1, true, true, null), // Giant Sword +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(44, ItemGroups.Swords, 12, 3, 1, true, true, null), // Heliacal Sword +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(46, ItemGroups.Bows, 12, 3, 1, true, true, null), // Serpent Crossbow +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(56, ItemGroups.Spears, 9, 3, 1, true, true, null), // Bill of Balrog +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(50, ItemGroups.Spears, 8, 3, 1, true, true, null), // Great Scythe +3+Skill+Luck+4

            this.ItemHelper.CreateWeapon(68, ItemGroups.Bows, 5, 3, 1, true, true, null), // Silver Bow +3+Skill+Luck+4
            this.ItemHelper.CreateWeapon(70, ItemGroups.Bows, 13, 3, 1, true, true, null), // Bluewing Crossbow +3+Skill+Luck+4
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Lumen the Barmaid'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateLumenTheBarmaidStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreatePotion(0, 9, 1, 0), // Ale
            this.ItemHelper.CreateItem(1, 10, 14, 1, 0), // Town Portal Scroll
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store of 'Carmen the Barmaid'.
    /// </summary>
    /// <returns>The created store.</returns>
    protected virtual ItemStorage CreateCarenTheBarmaidStore(short number)
    {
        List<Item> itemList = new ()
        {
            this.ItemHelper.CreatePotion(0, 9, 1, 0), // Ale
            this.ItemHelper.CreateItem(1, 10, 14, 1, 0), // Town Portal Scroll
        };

        var storage = this.CreateMerchantStore(itemList);
        storage.SetGuid(number);
        return storage;
    }

    /// <summary>
    /// Creates the merchant store with the specified item list.
    /// </summary>
    /// <param name="itemList">The item list.</param>
    /// <returns>The created store.</returns>
    protected ItemStorage CreateMerchantStore(IEnumerable<Item> itemList)
    {
        var merchantStore = this.Context.CreateNew<ItemStorage>();

        foreach (var item in itemList)
        {
            merchantStore.Items.Add(item);
        }

        return merchantStore;
    }
}