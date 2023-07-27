// <copyright file="BoxOfLuck.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for box of luck items.
/// </summary>
internal class BoxOfLuck : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoxOfLuck"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BoxOfLuck(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateBoxOfLuck();
        this.CreateWizardsRings();
        this.CreatePinkChocolateBox();
        this.CreateRedChocolateBox();
        this.CreateBlueChocolateBox();
        this.CreatePumpkinOfLuck();
        this.CreateRedRibbonBox();
        this.CreateGreenRibbonBox();
        this.CreateBlueRibbonBox();
        this.CreateChristmasStar();
        this.CreateChristmasFirecracker();
        this.CreateFireCracker();
        this.CreateCherryBlossomBox();
        this.CreateGameMasterPresentBox();
    }

    /// <summary>
    /// Creates the Box of Luck and its higher level boxes.
    /// +0: Box of Luck
    /// +1: Star of the Sacred Birth
    /// +2: Firecracker
    /// +3: Heart of Love
    /// +4: Olive of Love
    /// +5: Silver Medal
    /// +6: Gold Medal
    /// +7: Box of Heaven
    /// +8: Box of Kundun+1
    /// +9: Box of Kundun+2
    /// +10: Box of Kundun+3
    /// +11: Box of Kundun+4
    /// +12: Box of Kundun+5
    /// +13: Heart of Dark Lord
    /// +14: Lucky Blue Pouch
    /// +15: Lucky Red Pouch.
    /// </summary>
    /// <remarks>
    /// Regex for ExTeam Files:
    /// Search: ^(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\/\/(.+)$
    /// Replace: this.AddDropItem(boxOfLuck, $1, $2); // $9
    /// And for another format:
    /// Search: ^(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\/\/(.+)$
    /// Replace: this.AddDropItem(heart, $1, $2); // $7.
    /// </remarks>
    private void CreateBoxOfLuck()
    {
        var box = this.CreateBox("Box of Luck", 14, 11);
        box.MaximumItemLevel = 15;
        var boxOfLuck = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfLuck.ItemType = SpecialItemType.RandomItem;
        boxOfLuck.SourceItemLevel = 0;
        boxOfLuck.Chance = 0.5;
        boxOfLuck.MinimumLevel = 6;
        boxOfLuck.MaximumLevel = 6;
        boxOfLuck.Description = "Box of Luck";
        box.DropItems.Add(boxOfLuck);
        this.AddDropItem(boxOfLuck, 0, 3); // Katana
        this.AddDropItem(boxOfLuck, 0, 5); // Blade
        this.AddDropItem(boxOfLuck, 0, 9); // Sword of Salamander
        this.AddDropItem(boxOfLuck, 0, 10); // Light Saber
        this.AddDropItem(boxOfLuck, 0, 13); // Double Blade
        this.AddDropItem(boxOfLuck, 4, 4); // Tiger Bow
        this.AddDropItem(boxOfLuck, 4, 5); // Silver Bow
        this.AddDropItem(boxOfLuck, 4, 9); // Golden Crossbow
        this.AddDropItem(boxOfLuck, 4, 11); // Light Crossbow
        this.AddDropItem(boxOfLuck, 4, 12); // Serpent Crossbow
        this.AddDropItem(boxOfLuck, 5, 0); // Skull Staff
        this.AddDropItem(boxOfLuck, 5, 2); // Serpent Staff
        this.AddDropItem(boxOfLuck, 5, 3); // Lightning Staff
        this.AddDropItem(boxOfLuck, 5, 4); // Gorgon Staff
        this.AddDropItem(boxOfLuck, 12, 15); // Jewel of Chaos
        this.AddDropItem(boxOfLuck, 14, 13); // Jewel of Bless
        this.AddDropItem(boxOfLuck, 14, 14); // Jewel of Soul
        this.AddArmorSet(boxOfLuck, 0); // Bronze Set
        this.AddArmorSet(boxOfLuck, 2); // Pad Set
        this.AddArmorSet(boxOfLuck, 4); // Bone Set
        this.AddArmorSet(boxOfLuck, 5); // Leather Set
        this.AddArmorSet(boxOfLuck, 6); // Scale Set
        this.AddArmorSet(boxOfLuck, 7); // Sphinx Set
        this.AddArmorSet(boxOfLuck, 8); // Brass Set
        this.AddArmorSet(boxOfLuck, 10); // Vine Set
        this.AddArmorSet(boxOfLuck, 11); // Silk Set
        this.AddArmorSet(boxOfLuck, 12); // Wind Set
        this.AddMoneyDropFallback(box, 10000, boxOfLuck);

        var star = this.Context.CreateNew<ItemDropItemGroup>();
        star.ItemType = SpecialItemType.RandomItem;
        star.SourceItemLevel = 1;
        star.Chance = 1.0;
        star.Description = "Star of the Sacred Birth";
        star.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(star);
        this.AddDropItem(star, 0, 17); // Dark Breaker
        this.AddDropItem(star, 0, 20); // Knight Blade
        this.AddDropItem(star, 0, 21); // Dark Reign Blade
        this.AddDropItem(star, 2, 12); // Great Lord Scepter
        this.AddDropItem(star, 3, 10); // Dragon Spear
        this.AddDropItem(star, 4, 19); // Great Reign Crossbow
        this.AddDropItem(star, 4, 20); // Arrow Viper Bow
        this.AddDropItem(star, 5, 11); // Staff of Kundun
        this.AddDropItem(star, 6, 13); // Dragon Shield
        this.AddDropItem(star, 6, 15); // Grand Soul Shield
        this.AddArmorSet(star, 21); // Great Dragon Set
        this.AddArmorSet(star, 22); // Dark Soul Set
        this.AddArmorSet(star, 23); // Hurricane Set
        this.AddArmorSet(star, 24); // Red Spirit Set
        this.AddArmorSet(star, 28); // Dark Master Set

        var firecrackerJewels = this.Context.CreateNew<ItemDropItemGroup>();
        firecrackerJewels.ItemType = SpecialItemType.RandomItem;
        firecrackerJewels.SourceItemLevel = 2;
        firecrackerJewels.Chance = 0.2;
        firecrackerJewels.Description = "Firecracker (Jewels etc.)";
        firecrackerJewels.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(firecrackerJewels);
        this.AddDropItem(firecrackerJewels, 12, 15); // Jewel of Chaos
        this.AddDropItem(firecrackerJewels, 13, 2); // Horn of Uniria
        this.AddDropItem(firecrackerJewels, 14, 14); // Jewel of Soul
        this.AddDropItem(firecrackerJewels, 14, 13); // Jewel of Bless

        var firecracker = this.Context.CreateNew<ItemDropItemGroup>();
        firecracker.ItemType = SpecialItemType.RandomItem;
        firecracker.SourceItemLevel = 2;
        firecracker.Chance = 0.3;
        firecracker.Description = "Firecracker (Items +7 to +9)";
        firecracker.MinimumLevel = 7;
        firecracker.MaximumLevel = 9;
        firecracker.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(firecracker);
        this.AddDropItem(firecracker, 0, 4); // Sword Of Assassin
        this.AddDropItem(firecracker, 0, 5); // Blade
        this.AddDropItem(firecracker, 0, 6); // Gladius
        this.AddDropItem(firecracker, 0, 8); // Serpant Sword
        this.AddDropItem(firecracker, 0, 11); // Legendary Sword
        this.AddDropItem(firecracker, 0, 12); // Heliacal Sword
        this.AddDropItem(firecracker, 0, 14); // Lightning Sword
        this.AddDropItem(firecracker, 0, 15); // Giant Sword
        this.AddDropItem(firecracker, 1, 3); // Tomahawk
        this.AddDropItem(firecracker, 1, 4); // Fairy Axe
        this.AddDropItem(firecracker, 1, 5); // Battle Axe
        this.AddDropItem(firecracker, 1, 6); // Nikkea Axe
        this.AddDropItem(firecracker, 2, 2); // Flail
        this.AddDropItem(firecracker, 2, 3); // Great Warhammer
        this.AddDropItem(firecracker, 2, 8); // Battle Scepter
        this.AddDropItem(firecracker, 2, 9); // Master Scepter
        this.AddDropItem(firecracker, 3, 0); // Light Spear
        this.AddDropItem(firecracker, 3, 3); // Great Trident
        this.AddDropItem(firecracker, 3, 8); // Great Scythe
        this.AddDropItem(firecracker, 4, 2); // Elven Bow
        this.AddDropItem(firecracker, 4, 3); // Battle Bow
        this.AddDropItem(firecracker, 4, 4); // Tiger Bow
        this.AddDropItem(firecracker, 4, 10); // Arquebus
        this.AddDropItem(firecracker, 4, 11); // Light Crossbow
        this.AddDropItem(firecracker, 4, 12); // Serpent Crossbow
        this.AddDropItem(firecracker, 5, 0); // Skull Staff
        this.AddDropItem(firecracker, 5, 1); // Angelic Staff
        this.AddDropItem(firecracker, 5, 2); // Serpent Staff
        this.AddDropItem(firecracker, 5, 3); // Lightning Staff
        this.AddDropItem(firecracker, 5, 4); // Gorgon Staff
        this.AddDropItem(firecracker, 6, 2); // Kite Shield
        this.AddDropItem(firecracker, 6, 3); // Elven Shield
        this.AddDropItem(firecracker, 6, 4); // Buckler
        this.AddDropItem(firecracker, 6, 6); // Skull Shield
        this.AddDropItem(firecracker, 6, 7); // Spike Shield
        this.AddDropItem(firecracker, 6, 8); // Tower Shield
        this.AddDropItem(firecracker, 6, 9); // Plate Shield
        this.AddDropItem(firecracker, 6, 10); // Large Round Shield
        this.AddDropItem(firecracker, 6, 11); // Serpent Shield
        this.AddDropItem(firecracker, 6, 12); // Bronze Shield
        this.AddArmorSet(firecracker, 5); // Leather Set
        this.AddArmorSet(firecracker, 0); // Bronze Set
        this.AddArmorSet(firecracker, 6); // Scale Set
        this.AddArmorSet(firecracker, 2); // Pad Set
        this.AddArmorSet(firecracker, 4); // Bone Set
        this.AddArmorSet(firecracker, 7); // Sphinx Set
        this.AddArmorSet(firecracker, 10); // Vine Set
        this.AddArmorSet(firecracker, 11); // Silk Set
        this.AddArmorSet(firecracker, 12); // Wind Set
        this.AddMoneyDropFallback(box, 2004, firecracker);

        var heartJewels = this.Context.CreateNew<ItemDropItemGroup>();
        heartJewels.ItemType = SpecialItemType.RandomItem;
        heartJewels.SourceItemLevel = 3;
        heartJewels.Chance = 0.2;
        heartJewels.Description = "Heart of Love (Jewels etc.)";
        heartJewels.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(heartJewels);
        this.AddDropItem(heartJewels, 12, 15); // Jewel of Chaos
        this.AddDropItem(heartJewels, 13, 2); // Horn of Uniria
        this.AddDropItem(heartJewels, 14, 14); // Jewel of Soul
        this.AddDropItem(heartJewels, 14, 13); // Jewel of Bless

        var heart = this.Context.CreateNew<ItemDropItemGroup>();
        heart.ItemType = SpecialItemType.RandomItem;
        heart.SourceItemLevel = 3;
        heart.Chance = 0.3;
        heart.Description = "Heart of Love (Items +7 to +9)";
        heart.MinimumLevel = 7;
        heart.MaximumLevel = 9;
        heart.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(heart);
        this.AddDropItem(heart, 0, 4); // Sword Of Assassin
        this.AddDropItem(heart, 0, 5); // Blade
        this.AddDropItem(heart, 0, 6); // Gladius
        this.AddDropItem(heart, 0, 8); // Serpant Sword
        this.AddDropItem(heart, 0, 11); // Legendary Sword
        this.AddDropItem(heart, 0, 12); // Heliacal Sword
        this.AddDropItem(heart, 0, 14); // Lightning Sword
        this.AddDropItem(heart, 0, 15); // Giant Sword
        this.AddDropItem(heart, 1, 3); // Tomahawk
        this.AddDropItem(heart, 1, 4); // Fairy Axe
        this.AddDropItem(heart, 1, 5); // Battle Axe
        this.AddDropItem(heart, 1, 6); // Nikkea Axe
        this.AddDropItem(heart, 2, 2); // Flail
        this.AddDropItem(heart, 2, 3); // Great Warhammer
        this.AddDropItem(heart, 2, 8); // Battle Scepter
        this.AddDropItem(heart, 2, 9); // Master Scepter
        this.AddDropItem(heart, 3, 0); // Light Spear
        this.AddDropItem(heart, 3, 3); // Great Trident
        this.AddDropItem(heart, 3, 8); // Great Scythe
        this.AddDropItem(heart, 4, 2); // Elven Bow
        this.AddDropItem(heart, 4, 3); // Battle Bow
        this.AddDropItem(heart, 4, 4); // Tiger Bow
        this.AddDropItem(heart, 4, 10); // Arquebus
        this.AddDropItem(heart, 4, 11); // Light Crossbow
        this.AddDropItem(heart, 4, 12); // Serpent Crossbow
        this.AddDropItem(heart, 5, 0); // Skull Staff
        this.AddDropItem(heart, 5, 1); // Angelic Staff
        this.AddDropItem(heart, 5, 2); // Serpent Staff
        this.AddDropItem(heart, 5, 3); // Lightning Staff
        this.AddDropItem(heart, 5, 4); // Gorgon Staff
        this.AddDropItem(heart, 6, 2); // Kite Shield
        this.AddDropItem(heart, 6, 3); // Elven Shield
        this.AddDropItem(heart, 6, 4); // Buckler
        this.AddDropItem(heart, 6, 6); // Skull Shield
        this.AddDropItem(heart, 6, 7); // Spike Shield
        this.AddDropItem(heart, 6, 8); // Tower Shield
        this.AddDropItem(heart, 6, 9); // Plate Shield
        this.AddDropItem(heart, 6, 10); // Large Round Shield
        this.AddDropItem(heart, 6, 11); // Serpent Shield
        this.AddDropItem(heart, 6, 12); // Bronze Shield
        this.AddArmorSet(heart, 5); // Leather Set
        this.AddArmorSet(heart, 0); // Bronze Set
        this.AddArmorSet(heart, 6); // Scale Set
        this.AddArmorSet(heart, 2); // Pad Set
        this.AddArmorSet(heart, 4); // Bone Set
        this.AddArmorSet(heart, 7); // Sphinx Set
        this.AddArmorSet(heart, 10); // Vine Set
        this.AddArmorSet(heart, 11); // Silk Set
        this.AddArmorSet(heart, 12); // Wind Set
        this.AddMoneyDropFallback(box, 1004, heart);

        var silverMedal = this.Context.CreateNew<ItemDropItemGroup>();
        silverMedal.SourceItemLevel = 5;
        silverMedal.ItemType = SpecialItemType.RandomItem;
        silverMedal.Chance = 0.5;
        silverMedal.Description = "Silver Medal";
        silverMedal.MinimumLevel = 6;
        silverMedal.MaximumLevel = 6;
        silverMedal.DropEffect = ItemDropEffect.FanfareSound;
        box.DropItems.Add(silverMedal);
        this.AddDropItem(silverMedal, 12, 15); // Jewel of Chaos
        this.AddDropItem(silverMedal, 13, 0); // Guardian Angel
        this.AddDropItem(silverMedal, 13, 1); // Imp
        this.AddDropItem(silverMedal, 13, 2); // Horn of Uniria
        this.AddDropItem(silverMedal, 14, 13); // Jewel of Bless
        this.AddDropItem(silverMedal, 14, 14); // Jewel of Soul
        this.AddDropItem(silverMedal, 0, 10); // Light Saber
        this.AddDropItem(silverMedal, 0, 11); // Legendary Sword
        this.AddDropItem(silverMedal, 0, 13); // Double Blade
        this.AddDropItem(silverMedal, 0, 32); // Sacred Glove
        this.AddDropItem(silverMedal, 1, 7); // Larkan Axe
        this.AddDropItem(silverMedal, 2, 3); // Great Warhammer
        this.AddDropItem(silverMedal, 3, 0); // Light Spear
        this.AddDropItem(silverMedal, 3, 4); // Serpent Spear
        this.AddDropItem(silverMedal, 4, 3); // Battle Bow
        this.AddDropItem(silverMedal, 4, 11); // Light Crossbow
        this.AddDropItem(silverMedal, 5, 2); // Serpent Staff
        this.AddDropItem(silverMedal, 5, 3); // Lightning Staff
        this.AddDropItem(silverMedal, 6, 5); // Dragon Slayer Shield
        this.AddDropItem(silverMedal, 6, 6); // Skull Shield
        this.AddArmorSet(silverMedal, 6); // Scale Set
        this.AddArmorSet(silverMedal, 8); // Brass Set
        this.AddArmorSet(silverMedal, 4); // Bone Set
        this.AddArmorSet(silverMedal, 41); // Ancient Set
        this.AddArmorSet(silverMedal, 59); // Sacred Fire Set
        this.AddMoneyDropFallback(box, 100000, silverMedal);

        var goldMedal = this.Context.CreateNew<ItemDropItemGroup>();
        goldMedal.SourceItemLevel = 6;
        goldMedal.ItemType = SpecialItemType.RandomItem;
        goldMedal.Chance = 0.5;
        goldMedal.Description = "Gold Medal";
        goldMedal.MinimumLevel = 7;
        goldMedal.MaximumLevel = 7;
        goldMedal.DropEffect = ItemDropEffect.FanfareSound;
        box.DropItems.Add(goldMedal);
        this.AddDropItem(goldMedal, 12, 15); // Jewel of Chaos
        this.AddDropItem(goldMedal, 13, 0); // Guardian Angel
        this.AddDropItem(goldMedal, 13, 1); // Imp
        this.AddDropItem(goldMedal, 13, 2); // Horn of Uniria
        this.AddDropItem(goldMedal, 14, 13); // Jewel of Bless
        this.AddDropItem(goldMedal, 14, 14); // Jewel of Soul
        this.AddDropItem(goldMedal, 0, 12); // Heliacal Sword
        this.AddDropItem(goldMedal, 0, 15); // Giant Sword
        this.AddDropItem(goldMedal, 0, 32); // Sacred Fire RF
        this.AddDropItem(goldMedal, 1, 8); // Crescent Axe
        this.AddDropItem(goldMedal, 3, 8); // Great Scythe
        this.AddDropItem(goldMedal, 4, 4); // Tiger Bow
        this.AddDropItem(goldMedal, 4, 12); // Serpent Crossbow
        this.AddDropItem(goldMedal, 5, 4); // Gorgon Staff
        this.AddDropItem(goldMedal, 6, 8); // Tower Shield
        this.AddDropItem(goldMedal, 6, 11); // Serpent Shield
        this.AddArmorSet(goldMedal, 9); // Plate Armor
        this.AddArmorSet(goldMedal, 7); // Sphinx Armor
        this.AddArmorSet(goldMedal, 12); // Wind Armor
        this.AddArmorSet(goldMedal, 13); // Spirit Armor
        this.AddArmorSet(goldMedal, 41); // Ancient Set
        this.AddArmorSet(goldMedal, 59); // Sacred Fire Set
        this.AddMoneyDropFallback(box, 100000, goldMedal);

        var boxOfHeaven = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfHeaven.ItemType = SpecialItemType.RandomItem;
        boxOfHeaven.SourceItemLevel = 7;
        boxOfHeaven.Chance = 0.5;
        boxOfHeaven.Description = "Box of Heaven";
        boxOfHeaven.DropEffect = ItemDropEffect.FanfareSound;
        box.DropItems.Add(boxOfHeaven);
        this.AddDropItem(boxOfHeaven, 12, 15); // Jewel of Chaos
        this.AddDropItem(boxOfHeaven, 14, 13); // Jewel of Bless
        this.AddDropItem(boxOfHeaven, 14, 14); // Jewel of Soul
        this.AddMoneyDropFallback(box, 1000, boxOfHeaven);

        var boxOfKundun1 = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfKundun1.SourceItemLevel = 8;
        boxOfKundun1.ItemType = SpecialItemType.Excellent;
        boxOfKundun1.Chance = 0.2;
        boxOfKundun1.Description = "Box of Kundun+1";
        box.DropItems.Add(boxOfKundun1);
        this.AddDropItem(boxOfKundun1, 0, 0); // Kris
        this.AddDropItem(boxOfKundun1, 0, 1); // Short Sword
        this.AddDropItem(boxOfKundun1, 0, 2); // Rapier
        this.AddDropItem(boxOfKundun1, 0, 3); // Katana
        this.AddDropItem(boxOfKundun1, 0, 4); // Sword of Assassin
        this.AddDropItem(boxOfKundun1, 1, 0); // Small Axe
        this.AddDropItem(boxOfKundun1, 1, 1); // Hand Axe
        this.AddDropItem(boxOfKundun1, 1, 2); // Double Axe
        this.AddDropItem(boxOfKundun1, 1, 3); // Tomahawk
        this.AddDropItem(boxOfKundun1, 2, 0); // Mace
        this.AddDropItem(boxOfKundun1, 2, 1); // Morning Star
        this.AddDropItem(boxOfKundun1, 2, 2); // Flail
        this.AddDropItem(boxOfKundun1, 3, 1); // Spear
        this.AddDropItem(boxOfKundun1, 3, 2); // Dragon Lance
        this.AddDropItem(boxOfKundun1, 3, 5); // Double Poleaxe
        this.AddDropItem(boxOfKundun1, 3, 6); // Halberd
        this.AddDropItem(boxOfKundun1, 3, 7); // Berdysh
        this.AddDropItem(boxOfKundun1, 4, 0); // Short Bow
        this.AddDropItem(boxOfKundun1, 4, 1); // Bow
        this.AddDropItem(boxOfKundun1, 4, 2); // Elven Bow
        this.AddDropItem(boxOfKundun1, 4, 8); // Crossbow
        this.AddDropItem(boxOfKundun1, 4, 9); // Golden Crossbow
        this.AddDropItem(boxOfKundun1, 5, 0); // Skull Staff
        this.AddDropItem(boxOfKundun1, 5, 1); // Angelic Staff
        this.AddDropItem(boxOfKundun1, 5, 2); // Serpent Staff
        this.AddDropItem(boxOfKundun1, 5, 14); // Mystery Stick
        this.AddDropItem(boxOfKundun1, 5, 15); // Violent Wind Stick
        this.AddDropItem(boxOfKundun1, 6, 0); // Small Shield
        this.AddDropItem(boxOfKundun1, 6, 1); // Horn Shield
        this.AddDropItem(boxOfKundun1, 6, 2); // Kite Shield
        this.AddDropItem(boxOfKundun1, 6, 3); // Elven Shield
        this.AddDropItem(boxOfKundun1, 6, 4); // Buckler
        this.AddArmorSet(boxOfKundun1, 5); // Leather Set
        this.AddArmorSet(boxOfKundun1, 2); // Pad Set
        this.AddArmorSet(boxOfKundun1, 10); // Vine Set
        this.AddArmorSet(boxOfKundun1, 0); // Bronze Set
        this.AddArmorSet(boxOfKundun1, 11); // Silk Set
        this.AddArmorSet(boxOfKundun1, 39); // Violent Wind Set
        this.AddArmorSet(boxOfKundun1, 40); // Red Wing Set
        this.AddMoneyDropFallback(box, 50000, boxOfKundun1);

        var boxOfKundun2 = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfKundun2.SourceItemLevel = 9;
        boxOfKundun2.ItemType = SpecialItemType.Excellent;
        boxOfKundun2.Chance = 0.2;
        boxOfKundun2.Description = "Box of Kundun+2";
        box.DropItems.Add(boxOfKundun2);
        this.AddDropItem(boxOfKundun2, 0, 5); // Blade
        this.AddDropItem(boxOfKundun2, 0, 6); // Gladius
        this.AddDropItem(boxOfKundun2, 0, 7); // Falchion
        this.AddDropItem(boxOfKundun2, 0, 8); // Serpent Sword
        this.AddDropItem(boxOfKundun2, 0, 9); // Sword of Salamander
        this.AddDropItem(boxOfKundun2, 0, 10); // Light Saber
        this.AddDropItem(boxOfKundun2, 1, 4); // Elven Axe
        this.AddDropItem(boxOfKundun2, 1, 5); // Battle Axe
        this.AddDropItem(boxOfKundun2, 1, 6); // Nikea Axe
        this.AddDropItem(boxOfKundun2, 1, 7); // Larkan Axe
        this.AddDropItem(boxOfKundun2, 2, 3); // Great Hammer
        this.AddDropItem(boxOfKundun2, 2, 4); // Crystal Morning Star
        this.AddDropItem(boxOfKundun2, 2, 8); // Battle Scepter
        this.AddDropItem(boxOfKundun2, 2, 9); // Master Scepter
        this.AddDropItem(boxOfKundun2, 3, 0); // Light Spear
        this.AddDropItem(boxOfKundun2, 3, 3); // Giant Trident
        this.AddDropItem(boxOfKundun2, 3, 4); // Serpent Spear
        this.AddDropItem(boxOfKundun2, 3, 8); // Great Scythe
        this.AddDropItem(boxOfKundun2, 4, 3); // Battle Bow
        this.AddDropItem(boxOfKundun2, 4, 4); // Tiger Bow
        this.AddDropItem(boxOfKundun2, 4, 10); // Arquebus
        this.AddDropItem(boxOfKundun2, 4, 11); // Light Crossbow
        this.AddDropItem(boxOfKundun2, 4, 12); // Serpent Crossbow
        this.AddDropItem(boxOfKundun2, 5, 3); // Thunder Staff
        this.AddDropItem(boxOfKundun2, 5, 4); // Gorgon Staff
        this.AddDropItem(boxOfKundun2, 5, 16); // Red Wing Stick
        this.AddDropItem(boxOfKundun2, 6, 5); // Dragon Slayer Shield
        this.AddDropItem(boxOfKundun2, 6, 6); // Skull Shield
        this.AddDropItem(boxOfKundun2, 6, 7); // Spiked Shield
        this.AddDropItem(boxOfKundun2, 6, 8); // Tower Shield
        this.AddDropItem(boxOfKundun2, 13, 8); // Ring of Ice
        this.AddDropItem(boxOfKundun2, 13, 9); // Ring of Poison
        this.AddDropItem(boxOfKundun2, 13, 12); // Pendant of Lighting
        this.AddDropItem(boxOfKundun2, 13, 13); // Pendant of Fire
        this.AddArmorSet(boxOfKundun2, 6); // Scale Set
        this.AddArmorSet(boxOfKundun2, 8); // Brass Set
        this.AddArmorSet(boxOfKundun2, 4); // Bone Set
        this.AddArmorSet(boxOfKundun2, 7); // Sphinx Set
        this.AddArmorSet(boxOfKundun2, 12); // Wind Set
        this.AddArmorSet(boxOfKundun2, 13); // Spirit Set
        this.AddArmorSet(boxOfKundun2, 25); // Light Plate Set
        this.AddArmorSet(boxOfKundun2, 41); // Ancient Set
        this.AddMoneyDropFallback(box, 100000, boxOfKundun2);

        var boxOfKundun3 = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfKundun3.SourceItemLevel = 10;
        boxOfKundun3.ItemType = SpecialItemType.Excellent;
        boxOfKundun3.Chance = 0.2;
        boxOfKundun3.Description = "Box of Kundun+3";
        box.DropItems.Add(boxOfKundun3);
        this.AddDropItem(boxOfKundun3, 0, 11); // Legendary Sword
        this.AddDropItem(boxOfKundun3, 0, 12); // Heliacal Sword
        this.AddDropItem(boxOfKundun3, 0, 13); // Double Blade
        this.AddDropItem(boxOfKundun3, 0, 14); // Lightning Sword
        this.AddDropItem(boxOfKundun3, 0, 15); // Giant Sword
        this.AddDropItem(boxOfKundun3, 0, 32); // Sacred Glove
        this.AddDropItem(boxOfKundun3, 1, 8); // Crescent Axe
        this.AddDropItem(boxOfKundun3, 2, 5); // Crystal Sword
        this.AddDropItem(boxOfKundun3, 2, 6); // Chaos Dragon Axe
        this.AddDropItem(boxOfKundun3, 2, 7); // Elemental Mace
        this.AddDropItem(boxOfKundun3, 2, 10); // Great Scepter
        this.AddDropItem(boxOfKundun3, 3, 9); // Bill of Balrog
        this.AddDropItem(boxOfKundun3, 4, 5); // Silver Bow
        this.AddDropItem(boxOfKundun3, 4, 6); // Chaos Nature Bow
        this.AddDropItem(boxOfKundun3, 4, 13); // Bluewing Crossbow
        this.AddDropItem(boxOfKundun3, 4, 14); // Aquagold Crossbow
        this.AddDropItem(boxOfKundun3, 5, 3); // Staff of Resurrection
        this.AddDropItem(boxOfKundun3, 5, 5); // Legendary Staff
        this.AddDropItem(boxOfKundun3, 5, 7); // Chaos Lightning Staff
        this.AddDropItem(boxOfKundun3, 5, 17); // Ancient Stick
        this.AddDropItem(boxOfKundun3, 5, 18); // Black Rose Stick
        this.AddDropItem(boxOfKundun3, 6, 9); // Plate Shield
        this.AddDropItem(boxOfKundun3, 6, 10); // Large Round Shield
        this.AddDropItem(boxOfKundun3, 6, 11); // Serpent Shield
        this.AddDropItem(boxOfKundun3, 6, 12); // Bronze Shield
        this.AddDropItem(boxOfKundun3, 6, 14); // Legendary Shield
        this.AddDropItem(boxOfKundun3, 13, 21); // Ring of Fire
        this.AddDropItem(boxOfKundun3, 13, 22); // Ring of Earth
        this.AddDropItem(boxOfKundun3, 13, 23); // Ring of Wind
        this.AddDropItem(boxOfKundun3, 13, 24); // Ring of Magic
        this.AddDropItem(boxOfKundun3, 13, 25); // Pendant of Ice
        this.AddDropItem(boxOfKundun3, 13, 26); // Pendant of Wind
        this.AddDropItem(boxOfKundun3, 13, 27); // Pendant of Water
        this.AddDropItem(boxOfKundun3, 13, 28); // Pendant of Ability
        this.AddArmorSet(boxOfKundun3, 9); // Plate Set
        this.AddArmorSet(boxOfKundun3, 1); // Dragon Set
        this.AddArmorSet(boxOfKundun3, 3); // Legendary Set
        this.AddArmorSet(boxOfKundun3, 14); // Guardian Set
        this.AddArmorSet(boxOfKundun3, 15); // Storm Crow Set
        this.AddArmorSet(boxOfKundun3, 26); // Adamantine Set
        this.AddArmorSet(boxOfKundun3, 42); // Bloody Amethyst Set
        this.AddArmorSet(boxOfKundun3, 59); // Sacred Fire Set
        this.AddMoneyDropFallback(box, 150000, boxOfKundun3);

        var boxOfKundun4 = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfKundun4.SourceItemLevel = 11;
        boxOfKundun4.ItemType = SpecialItemType.Excellent;
        boxOfKundun4.Chance = 0.2;
        boxOfKundun4.Description = "Box of Kundun+4";
        box.DropItems.Add(boxOfKundun4);
        this.AddDropItem(boxOfKundun4, 0, 16); // Sword of Destruction
        this.AddDropItem(boxOfKundun4, 0, 17); // Dark Breaker
        this.AddDropItem(boxOfKundun4, 0, 18); // Thunder Blade
        this.AddDropItem(boxOfKundun4, 0, 19); // Divine Sword of Archangel
        this.AddDropItem(boxOfKundun4, 0, 33); // Storm Jahad Glove
        this.AddDropItem(boxOfKundun4, 2, 11); // Lord Scepter
        this.AddDropItem(boxOfKundun4, 2, 12); // Great Lord Scepter
        this.AddDropItem(boxOfKundun4, 2, 13); // Divine Scepter of Archangel
        this.AddDropItem(boxOfKundun4, 3, 10); // Dragon Spear
        this.AddDropItem(boxOfKundun4, 4, 16); // Saint Crossbow
        this.AddDropItem(boxOfKundun4, 4, 17); // Celestial Bow
        this.AddDropItem(boxOfKundun4, 4, 18); // Divine Crossbow of Archangel
        this.AddDropItem(boxOfKundun4, 4, 19); // Great Reign Crossbow
        this.AddDropItem(boxOfKundun4, 5, 8); // Staff of Destruction
        this.AddDropItem(boxOfKundun4, 5, 9); // Dragon Soul Staff
        this.AddDropItem(boxOfKundun4, 5, 10); // Divine Staff of Archangel
        this.AddDropItem(boxOfKundun4, 5, 19); // Aura Stick
        this.AddDropItem(boxOfKundun4, 6, 13); // Chaos Dragon Shield
        this.AddDropItem(boxOfKundun4, 6, 15); // Grand Soul Shield
        this.AddDropItem(boxOfKundun4, 6, 16); // Elemental Shield
        this.AddArmorSet(boxOfKundun4, 16); // Black Dragon Set
        this.AddArmorSet(boxOfKundun4, 17); // Dark Phoenix Set
        this.AddArmorSet(boxOfKundun4, 18); // Grand Soul Set
        this.AddArmorSet(boxOfKundun4, 19); // Divine Set
        this.AddArmorSet(boxOfKundun4, 20); // Thunder Hawk Set
        this.AddArmorSet(boxOfKundun4, 27); // Dark Steel Set
        this.AddArmorSet(boxOfKundun4, 43); // Rhodon Quartz Set
        this.AddArmorSet(boxOfKundun4, 60); // Storm Jahad Fire Set
        this.AddMoneyDropFallback(box, 200000, boxOfKundun4);

        var boxOfKundun5 = this.Context.CreateNew<ItemDropItemGroup>();
        boxOfKundun5.SourceItemLevel = 12;
        boxOfKundun5.ItemType = SpecialItemType.Excellent;
        boxOfKundun5.Chance = 0.2;
        boxOfKundun5.Description = "Box of Kundun+5";
        box.DropItems.Add(boxOfKundun5);
        this.AddDropItem(boxOfKundun5, 0, 20); // Knight Blade
        this.AddDropItem(boxOfKundun5, 0, 21); // Dark Reign Blade
        this.AddDropItem(boxOfKundun5, 0, 31); // Rune Blade
        this.AddDropItem(boxOfKundun5, 0, 34); // Piercing Groove Glove
        this.AddDropItem(boxOfKundun5, 2, 15); // Shining Scepter
        this.AddDropItem(boxOfKundun5, 4, 20); // Arrow Viper Bow
        this.AddDropItem(boxOfKundun5, 5, 11); // Staff of Kundun
        this.AddDropItem(boxOfKundun5, 5, 13); // Platina Staff
        this.AddArmorSet(boxOfKundun5, 21); // Great Dragon Set
        this.AddArmorSet(boxOfKundun5, 22); // Dark Soul Set
        this.AddArmorSet(boxOfKundun5, 23); // Hurricane Set
        this.AddArmorSet(boxOfKundun5, 24); // Red Spirit Set
        this.AddArmorSet(boxOfKundun5, 28); // Dark Master Set
        this.AddArmorSet(boxOfKundun5, 34); // Ashcrow Set
        this.AddArmorSet(boxOfKundun5, 35); // Eclipse Set
        this.AddArmorSet(boxOfKundun5, 36); // Iris Set
        this.AddArmorSet(boxOfKundun5, 37); // Valiant Set
        this.AddArmorSet(boxOfKundun5, 38); // Glorious Set
        this.AddArmorSet(boxOfKundun5, 61); // Piercing Groove Fire Set
        this.AddMoneyDropFallback(box, 250000, boxOfKundun5);

        var heartOfDarkLord = this.Context.CreateNew<ItemDropItemGroup>();
        heartOfDarkLord.ItemType = SpecialItemType.RandomItem;
        heartOfDarkLord.SourceItemLevel = 13;
        heartOfDarkLord.Chance = 0.5;
        heartOfDarkLord.Description = "Heart of Dark Lord";
        heartOfDarkLord.MinimumLevel = 7;
        heartOfDarkLord.MaximumLevel = 8;
        heartOfDarkLord.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(heartOfDarkLord);
        this.AddDropItem(heartOfDarkLord, 0, 3); // Katana
        this.AddDropItem(heartOfDarkLord, 0, 5); // Blade
        this.AddDropItem(heartOfDarkLord, 0, 15); // Giant Sword
        this.AddDropItem(heartOfDarkLord, 2, 8); // Battle Scepter
        this.AddDropItem(heartOfDarkLord, 4, 2); // Elven Bow
        this.AddDropItem(heartOfDarkLord, 4, 3); // Battle Bow
        this.AddDropItem(heartOfDarkLord, 4, 4); // Tiger Bow
        this.AddDropItem(heartOfDarkLord, 4, 5); // Silver Bow
        this.AddDropItem(heartOfDarkLord, 5, 0); // Skull Staff
        this.AddDropItem(heartOfDarkLord, 5, 1); // Angelic Staff
        this.AddDropItem(heartOfDarkLord, 5, 2); // Serpent Staff
        this.AddDropItem(heartOfDarkLord, 5, 3); // Lightning Staff
        this.AddArmorSet(heartOfDarkLord, 5); // Leather Set
        this.AddArmorSet(heartOfDarkLord, 0); // Bronze Set
        this.AddArmorSet(heartOfDarkLord, 6); // Scale Set
        this.AddArmorSet(heartOfDarkLord, 8); // Brass Set
        this.AddArmorSet(heartOfDarkLord, 2); // Pad Set
        this.AddArmorSet(heartOfDarkLord, 4); // Bone Set
        this.AddArmorSet(heartOfDarkLord, 7); // Sphinx Set
        this.AddArmorSet(heartOfDarkLord, 10); // Vine Set
        this.AddArmorSet(heartOfDarkLord, 11); // Silk Set
        this.AddArmorSet(heartOfDarkLord, 12); // Wind Set
        this.AddArmorSet(heartOfDarkLord, 25); // Light Plate Set
        this.AddMoneyDropFallback(box, 10000, heartOfDarkLord);
    }

    private void CreatePinkChocolateBox()
    {
        var box = this.CreateBox("Pink Chocolate Box", 14, 32);
        var pinkBox = this.Context.CreateNew<ItemDropItemGroup>();
        pinkBox.ItemType = SpecialItemType.RandomItem;
        pinkBox.SourceItemLevel = 0;
        pinkBox.Chance = 0.4;
        pinkBox.Description = "Pink Chocolate Box - Normal Items";
        pinkBox.MinimumLevel = 6;
        pinkBox.MaximumLevel = 9;
        pinkBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(pinkBox);
        this.AddDropItem(pinkBox, 0, 3);
        this.AddDropItem(pinkBox, 0, 5);
        this.AddDropItem(pinkBox, 0, 7);
        this.AddDropItem(pinkBox, 0, 8);
        this.AddDropItem(pinkBox, 0, 10);
        this.AddDropItem(pinkBox, 0, 11);
        this.AddDropItem(pinkBox, 1, 3);
        this.AddDropItem(pinkBox, 1, 5);
        this.AddDropItem(pinkBox, 1, 6);
        this.AddDropItem(pinkBox, 2, 1);
        this.AddDropItem(pinkBox, 2, 3);
        this.AddDropItem(pinkBox, 2, 8);
        this.AddDropItem(pinkBox, 3, 2);
        this.AddDropItem(pinkBox, 3, 0);
        this.AddDropItem(pinkBox, 4, 2);
        this.AddDropItem(pinkBox, 4, 4);
        this.AddDropItem(pinkBox, 4, 9);
        this.AddDropItem(pinkBox, 4, 10);
        this.AddDropItem(pinkBox, 4, 12);
        this.AddDropItem(pinkBox, 5, 0);
        this.AddDropItem(pinkBox, 5, 2);
        this.AddDropItem(pinkBox, 5, 3);
        this.AddDropItem(pinkBox, 6, 3);
        this.AddDropItem(pinkBox, 6, 5);
        this.AddDropItem(pinkBox, 6, 6);
        this.AddDropItem(pinkBox, 6, 8);
        this.AddDropItem(pinkBox, 6, 9);

        this.AddArmorSet(pinkBox, 5); // Leather Set
        this.AddArmorSet(pinkBox, 2); // Pad Set
        this.AddArmorSet(pinkBox, 10); // Vine Set
        this.AddArmorSet(pinkBox, 6); // Scale Set
        this.AddArmorSet(pinkBox, 8); // Brass Set
        this.AddArmorSet(pinkBox, 4); // Bone Set
        this.AddArmorSet(pinkBox, 7); // Sphinx Set
        this.AddArmorSet(pinkBox, 11); // Silk Set
        this.AddArmorSet(pinkBox, 13); // Spirit Set
        this.AddArmorSet(pinkBox, 9); // Plate Set
        this.AddDropItem(pinkBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(pinkBox, 14, 13); // Jewel of Bless
        this.AddDropItem(pinkBox, 14, 14); // Jewel of Soul

        var pinkBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        pinkBoxExc.ItemType = SpecialItemType.Excellent;
        pinkBoxExc.SourceItemLevel = 0;
        pinkBoxExc.Chance = 0.1;
        pinkBoxExc.Description = "Pink Chocolate Box - Excellent Items";
        pinkBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(pinkBoxExc);
        this.AddDropItem(pinkBoxExc, 0, 5); // Blade
        this.AddDropItem(pinkBoxExc, 0, 10); // Light Saber
        this.AddDropItem(pinkBoxExc, 1, 6); // Nikea Axe
        this.AddDropItem(pinkBoxExc, 3, 0); // Light Spear
        this.AddDropItem(pinkBoxExc, 4, 3); // Battle Bow
        this.AddDropItem(pinkBoxExc, 4, 11); // Light Crossbow
        this.AddDropItem(pinkBoxExc, 5, 2); // Serpent Staff
        this.AddDropItem(pinkBoxExc, 6, 8); // Tower Shield
        this.AddDropItem(pinkBoxExc, 6, 9); // Plate Shield
        this.AddArmorSet(pinkBoxExc, 0); // Bronze Set
        this.AddArmorSet(pinkBoxExc, 11); // Silk Set
        this.AddArmorSet(pinkBoxExc, 4); // Bone Set

        this.AddMoneyDropFallback(box, 100000, pinkBox, "Pink Chocolate Box");

        var lightPurpleCandyBox = this.Context.CreateNew<ItemDropItemGroup>();
        lightPurpleCandyBox.ItemType = SpecialItemType.RandomItem;
        lightPurpleCandyBox.SourceItemLevel = 1;
        lightPurpleCandyBox.Chance = 0.4;
        lightPurpleCandyBox.Description = "Light Purple Candy Box - Normal Items";
        lightPurpleCandyBox.MinimumLevel = 6;
        lightPurpleCandyBox.MaximumLevel = 9;
        lightPurpleCandyBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(lightPurpleCandyBox);
        this.AddDropItem(lightPurpleCandyBox, 0, 3);
        this.AddDropItem(lightPurpleCandyBox, 0, 5);
        this.AddDropItem(lightPurpleCandyBox, 0, 7);
        this.AddDropItem(lightPurpleCandyBox, 0, 8);
        this.AddDropItem(lightPurpleCandyBox, 0, 10);
        this.AddDropItem(lightPurpleCandyBox, 0, 11);
        this.AddDropItem(lightPurpleCandyBox, 1, 3);
        this.AddDropItem(lightPurpleCandyBox, 1, 5);
        this.AddDropItem(lightPurpleCandyBox, 1, 6);
        this.AddDropItem(lightPurpleCandyBox, 2, 1);
        this.AddDropItem(lightPurpleCandyBox, 2, 3);
        this.AddDropItem(lightPurpleCandyBox, 2, 8);
        this.AddDropItem(lightPurpleCandyBox, 3, 0);
        this.AddDropItem(lightPurpleCandyBox, 3, 2);
        this.AddDropItem(lightPurpleCandyBox, 4, 2);
        this.AddDropItem(lightPurpleCandyBox, 4, 4);
        this.AddDropItem(lightPurpleCandyBox, 4, 9);
        this.AddDropItem(lightPurpleCandyBox, 4, 10);
        this.AddDropItem(lightPurpleCandyBox, 4, 12);
        this.AddDropItem(lightPurpleCandyBox, 5, 0);
        this.AddDropItem(lightPurpleCandyBox, 5, 2);
        this.AddDropItem(lightPurpleCandyBox, 5, 3);
        this.AddDropItem(lightPurpleCandyBox, 6, 3);
        this.AddDropItem(lightPurpleCandyBox, 6, 5);
        this.AddDropItem(lightPurpleCandyBox, 6, 6);
        this.AddDropItem(lightPurpleCandyBox, 6, 8);
        this.AddDropItem(lightPurpleCandyBox, 6, 9);
        this.AddArmorSet(lightPurpleCandyBox, 5); // Leather Set
        this.AddArmorSet(lightPurpleCandyBox, 2); // Pad Set
        this.AddArmorSet(lightPurpleCandyBox, 10); // Vine Set
        this.AddArmorSet(lightPurpleCandyBox, 6); // Scale Set
        this.AddArmorSet(lightPurpleCandyBox, 8); // Brass Set
        this.AddArmorSet(lightPurpleCandyBox, 4); // Bone Set
        this.AddArmorSet(lightPurpleCandyBox, 7); // Sphinx Set
        this.AddArmorSet(lightPurpleCandyBox, 11); // Silk Set
        this.AddArmorSet(lightPurpleCandyBox, 13); // Spirit Set
        this.AddArmorSet(lightPurpleCandyBox, 9); // Plate Set
        this.AddDropItem(lightPurpleCandyBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(lightPurpleCandyBox, 14, 13); // Jewel of Bless
        this.AddDropItem(lightPurpleCandyBox, 14, 14); // Jewel of Soul

        var lightPurpleCandyBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        lightPurpleCandyBoxExc.ItemType = SpecialItemType.Excellent;
        lightPurpleCandyBoxExc.SourceItemLevel = 1;
        lightPurpleCandyBoxExc.Chance = 0.1;
        lightPurpleCandyBoxExc.Description = "Light Purple Candy Box - Excellent Items";
        lightPurpleCandyBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(lightPurpleCandyBoxExc);
        this.AddDropItem(lightPurpleCandyBoxExc, 0, 5); // Blade
        this.AddDropItem(lightPurpleCandyBoxExc, 0, 10); // Light Saber
        this.AddDropItem(lightPurpleCandyBoxExc, 1, 6); // Nikea Axe
        this.AddDropItem(lightPurpleCandyBoxExc, 3, 0); // Light Spear
        this.AddDropItem(lightPurpleCandyBoxExc, 4, 3); // Battle Bow
        this.AddDropItem(lightPurpleCandyBoxExc, 4, 11); // Light Crossbow
        this.AddDropItem(lightPurpleCandyBoxExc, 5, 2); // Serpent Staff
        this.AddDropItem(lightPurpleCandyBoxExc, 6, 8); // Tower Shield
        this.AddDropItem(lightPurpleCandyBoxExc, 6, 9); // Plate Shield
        this.AddArmorSet(lightPurpleCandyBoxExc, 0); // Bronze Set
        this.AddArmorSet(lightPurpleCandyBoxExc, 11); // Silk Set
        this.AddArmorSet(lightPurpleCandyBoxExc, 4); // Bone Set

        this.AddMoneyDropFallback(box, 100000, lightPurpleCandyBox, "Light Purple Candy Box");
    }

    private void CreateRedChocolateBox()
    {
        var box = this.CreateBox("Red Chocolate Box", 14, 33);
        var redBox = this.Context.CreateNew<ItemDropItemGroup>();
        redBox.ItemType = SpecialItemType.RandomItem;
        redBox.SourceItemLevel = 0;
        redBox.Chance = 0.4;
        redBox.Description = "Red Chocolate Box - Normal Items";
        redBox.MinimumLevel = 6;
        redBox.MaximumLevel = 9;
        redBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(redBox);
        this.AddDropItem(redBox, 0, 12);
        this.AddDropItem(redBox, 0, 13);
        this.AddDropItem(redBox, 0, 14);
        this.AddDropItem(redBox, 1, 8);
        this.AddDropItem(redBox, 2, 4);
        this.AddDropItem(redBox, 2, 9);
        this.AddDropItem(redBox, 2, 5);
        this.AddDropItem(redBox, 2, 10);
        this.AddDropItem(redBox, 3, 9);
        this.AddDropItem(redBox, 4, 5);
        this.AddDropItem(redBox, 4, 13);
        this.AddDropItem(redBox, 4, 14);
        this.AddDropItem(redBox, 5, 5);
        this.AddDropItem(redBox, 5, 6);
        this.AddDropItem(redBox, 6, 11);
        this.AddDropItem(redBox, 6, 12);
        this.AddDropItem(redBox, 6, 13);
        this.AddDropItem(redBox, 6, 14);
        this.AddDropItem(redBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(redBox, 14, 13); // Jewel of Bless
        this.AddDropItem(redBox, 14, 14); // Jewel of Soul
        this.AddDropItem(redBox, 14, 16); // Jewel of Life
        this.AddArmorSet(redBox, 34);
        this.AddArmorSet(redBox, 35);
        this.AddArmorSet(redBox, 36);
        this.AddArmorSet(redBox, 26);
        this.AddArmorSet(redBox, 15);
        this.AddArmorSet(redBox, 16);
        this.AddArmorSet(redBox, 18);
        this.AddArmorSet(redBox, 19);
        this.AddArmorSet(redBox, 20);
        this.AddArmorSet(redBox, 1);
        this.AddArmorSet(redBox, 14);
        this.AddArmorSet(redBox, 3);

        var redBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        redBoxExc.ItemType = SpecialItemType.Excellent;
        redBoxExc.SourceItemLevel = 0;
        redBoxExc.Chance = 0.1;
        redBoxExc.Description = "Red Chocolate Box - Excellent Items";
        redBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(redBoxExc);
        this.AddDropItem(redBoxExc, 0, 13);
        this.AddDropItem(redBoxExc, 0, 14);
        this.AddDropItem(redBoxExc, 1, 8);
        this.AddDropItem(redBoxExc, 2, 3);
        this.AddDropItem(redBoxExc, 2, 4);
        this.AddDropItem(redBoxExc, 2, 8);
        this.AddDropItem(redBoxExc, 2, 9);
        this.AddDropItem(redBoxExc, 3, 8);
        this.AddDropItem(redBoxExc, 4, 4);
        this.AddDropItem(redBoxExc, 4, 12);
        this.AddDropItem(redBoxExc, 4, 13);
        this.AddDropItem(redBoxExc, 5, 2);
        this.AddDropItem(redBoxExc, 5, 3);
        this.AddDropItem(redBoxExc, 5, 6);
        this.AddDropItem(redBoxExc, 6, 12);
        this.AddDropItem(redBoxExc, 6, 11);
        this.AddDropItem(redBoxExc, 6, 13);
        this.AddDropItem(redBoxExc, 6, 14);
        this.AddDropItem(redBoxExc, 13, 8); // Ring of Ice
        this.AddDropItem(redBoxExc, 13, 9); // Ring of Poison
        this.AddDropItem(redBoxExc, 13, 12); // Pendant of Lightning
        this.AddDropItem(redBoxExc, 13, 13); // Pendant of Fire
        this.AddArmorSet(redBoxExc, 1);
        this.AddArmorSet(redBoxExc, 14);
        this.AddArmorSet(redBoxExc, 3);
        this.AddArmorSet(redBoxExc, 25);
        this.AddArmorSet(redBoxExc, 9);
        this.AddArmorSet(redBoxExc, 26);

        this.AddMoneyDropFallback(box, 500000, redBox, "Red Chocolate Box");

        var vermilionBox = this.Context.CreateNew<ItemDropItemGroup>();
        vermilionBox.ItemType = SpecialItemType.RandomItem;
        vermilionBox.SourceItemLevel = 1;
        vermilionBox.Chance = 0.4;
        vermilionBox.Description = "Vermilion Candy Box - Normal Items";
        vermilionBox.MinimumLevel = 6;
        vermilionBox.MaximumLevel = 9;
        vermilionBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(vermilionBox);
        this.AddDropItem(vermilionBox, 0, 12);
        this.AddDropItem(vermilionBox, 0, 13);
        this.AddDropItem(vermilionBox, 0, 14);
        this.AddDropItem(vermilionBox, 1, 8);
        this.AddDropItem(vermilionBox, 2, 4);
        this.AddDropItem(vermilionBox, 2, 5);
        this.AddDropItem(vermilionBox, 2, 9);
        this.AddDropItem(vermilionBox, 2, 10);
        this.AddDropItem(vermilionBox, 3, 9);
        this.AddDropItem(vermilionBox, 4, 5);
        this.AddDropItem(vermilionBox, 4, 13);
        this.AddDropItem(vermilionBox, 4, 14);
        this.AddDropItem(vermilionBox, 5, 5);
        this.AddDropItem(vermilionBox, 5, 6);
        this.AddDropItem(vermilionBox, 6, 11);
        this.AddDropItem(vermilionBox, 6, 12);
        this.AddDropItem(vermilionBox, 6, 13);
        this.AddDropItem(vermilionBox, 6, 14);
        this.AddDropItem(vermilionBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(vermilionBox, 14, 13); // Jewel of Bless
        this.AddDropItem(vermilionBox, 14, 14); // Jewel of Soul
        this.AddDropItem(vermilionBox, 14, 16); // Jewel of Life
        this.AddArmorSet(vermilionBox, 34);
        this.AddArmorSet(vermilionBox, 35);
        this.AddArmorSet(vermilionBox, 36);
        this.AddArmorSet(vermilionBox, 26);
        this.AddArmorSet(vermilionBox, 15);
        this.AddArmorSet(vermilionBox, 16);
        this.AddArmorSet(vermilionBox, 18);
        this.AddArmorSet(vermilionBox, 19);
        this.AddArmorSet(vermilionBox, 20);
        this.AddArmorSet(vermilionBox, 1);
        this.AddArmorSet(vermilionBox, 14);
        this.AddArmorSet(vermilionBox, 3);

        var vermilionBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        vermilionBoxExc.ItemType = SpecialItemType.Excellent;
        vermilionBoxExc.SourceItemLevel = 1;
        vermilionBoxExc.Chance = 0.1;
        vermilionBoxExc.Description = "Vermilion Candy Box - Excellent Items";
        vermilionBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(vermilionBoxExc);
        this.AddDropItem(vermilionBoxExc, 0, 13);
        this.AddDropItem(vermilionBoxExc, 0, 14);
        this.AddDropItem(vermilionBoxExc, 1, 8);
        this.AddDropItem(vermilionBoxExc, 2, 3);
        this.AddDropItem(vermilionBoxExc, 2, 4);
        this.AddDropItem(vermilionBoxExc, 2, 8);
        this.AddDropItem(vermilionBoxExc, 2, 9);
        this.AddDropItem(vermilionBoxExc, 3, 8);
        this.AddDropItem(vermilionBoxExc, 4, 4);
        this.AddDropItem(vermilionBoxExc, 4, 12);
        this.AddDropItem(vermilionBoxExc, 4, 13);
        this.AddDropItem(vermilionBoxExc, 5, 2);
        this.AddDropItem(vermilionBoxExc, 5, 3);
        this.AddDropItem(vermilionBoxExc, 5, 6);
        this.AddDropItem(vermilionBoxExc, 6, 11);
        this.AddDropItem(vermilionBoxExc, 6, 12);
        this.AddDropItem(vermilionBoxExc, 6, 13);
        this.AddDropItem(vermilionBoxExc, 6, 14);
        this.AddDropItem(vermilionBoxExc, 13, 8); // Ring of Ice
        this.AddDropItem(vermilionBoxExc, 13, 9); // Ring of Poison
        this.AddDropItem(vermilionBoxExc, 13, 12); // Pendant of Lightning
        this.AddDropItem(vermilionBoxExc, 13, 13); // Pendant of Fire
        this.AddArmorSet(vermilionBoxExc, 1);
        this.AddArmorSet(vermilionBoxExc, 14);
        this.AddArmorSet(vermilionBoxExc, 3);
        this.AddArmorSet(vermilionBoxExc, 25);
        this.AddArmorSet(vermilionBoxExc, 9);
        this.AddArmorSet(vermilionBoxExc, 26);

        this.AddMoneyDropFallback(box, 400000, vermilionBox, "Vermilion Candy Box");
    }

    private void CreateBlueChocolateBox()
    {
        var box = this.CreateBox("Blue Chocolate Box", 14, 34);
        var blueBox = this.Context.CreateNew<ItemDropItemGroup>();
        blueBox.ItemType = SpecialItemType.RandomItem;
        blueBox.SourceItemLevel = 0;
        blueBox.Chance = 0.4;
        blueBox.Description = "Blue Chocolate Box - Normal Items";
        blueBox.MinimumLevel = 6;
        blueBox.MaximumLevel = 9;
        blueBox.DropEffect = ItemDropEffect.Fireworks;
        this.AddDropItem(blueBox, 0, 20);
        this.AddDropItem(blueBox, 0, 25);
        this.AddDropItem(blueBox, 0, 31);
        this.AddDropItem(blueBox, 2, 11);
        this.AddDropItem(blueBox, 2, 15);
        this.AddDropItem(blueBox, 4, 17);
        this.AddDropItem(blueBox, 4, 22);
        this.AddDropItem(blueBox, 5, 9);
        this.AddDropItem(blueBox, 5, 13);
        this.AddDropItem(blueBox, 10, 21); // Great Dragon Gloves
        this.AddDropItem(blueBox, 11, 21); // Great Dragon Boots
        this.AddDropItem(blueBox, 10, 22); // Dark Soul Gloves
        this.AddDropItem(blueBox, 11, 22); // Dark Soul Boots
        this.AddArmorSet(blueBox, 17); // Dark Phoenix Set
        this.AddArmorSet(blueBox, 24); // Red Spirit Set
        this.AddArmorSet(blueBox, 37); // Valiant Set
        this.AddArmorSet(blueBox, 38); // Glorious Set
        this.AddDropItem(blueBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(blueBox, 14, 13); // Jewel of Bless
        this.AddDropItem(blueBox, 14, 14); // Jewel of Soul
        this.AddDropItem(blueBox, 14, 16); // Jewel of Life
        this.AddDropItem(blueBox, 14, 22); // Jewel of Creation
        this.AddDropItem(blueBox, 14, 42); // Jewel of Harmony

        var blueBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        blueBoxExc.ItemType = SpecialItemType.Excellent;
        blueBoxExc.SourceItemLevel = 0;
        blueBoxExc.Chance = 0.1;
        blueBoxExc.Description = "Blue Chocolate Box - Excellent Items";
        blueBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(blueBoxExc);
        this.AddDropItem(blueBoxExc, 0, 16);
        this.AddDropItem(blueBoxExc, 0, 31);
        this.AddDropItem(blueBoxExc, 2, 5);
        this.AddDropItem(blueBoxExc, 2, 10);
        this.AddDropItem(blueBoxExc, 4, 14);
        this.AddDropItem(blueBoxExc, 4, 16);
        this.AddDropItem(blueBoxExc, 5, 8);
        this.AddDropItem(blueBoxExc, 6, 13);
        this.AddDropItem(blueBoxExc, 6, 14);
        this.AddDropItem(blueBoxExc, 6, 15);
        this.AddDropItem(blueBoxExc, 6, 16);
        this.AddDropItem(blueBoxExc, 13, 8); // Ring of Ice
        this.AddDropItem(blueBoxExc, 13, 9); // Ring of Poison
        this.AddDropItem(blueBoxExc, 13, 21); // Ring of Fire
        this.AddDropItem(blueBoxExc, 13, 12); // Pendant of Lightning
        this.AddDropItem(blueBoxExc, 13, 13); // Pendant of Fire
        this.AddDropItem(blueBoxExc, 13, 25); // Pendant of Ice
        this.AddArmorSet(blueBoxExc, 16); // Black Dragon Set
        this.AddArmorSet(blueBoxExc, 18); // Grand Soul Set
        this.AddArmorSet(blueBoxExc, 19); // Divine Set
        this.AddArmorSet(blueBoxExc, 27); // Dark Steel Set

        this.AddMoneyDropFallback(box, 500000, blueBox, "Blue Chocolate Box");

        var deepBlueBox = this.Context.CreateNew<ItemDropItemGroup>();
        deepBlueBox.ItemType = SpecialItemType.RandomItem;
        deepBlueBox.SourceItemLevel = 1;
        deepBlueBox.Chance = 0.4;
        deepBlueBox.Description = "Deep Blue Candy Box - Normal Items";
        deepBlueBox.MinimumLevel = 6;
        deepBlueBox.MaximumLevel = 9;
        deepBlueBox.DropEffect = ItemDropEffect.Fireworks;
        this.AddDropItem(deepBlueBox, 0, 20);
        this.AddDropItem(deepBlueBox, 0, 25);
        this.AddDropItem(deepBlueBox, 0, 31);
        this.AddDropItem(deepBlueBox, 2, 11);
        this.AddDropItem(deepBlueBox, 2, 15);
        this.AddDropItem(deepBlueBox, 4, 17);
        this.AddDropItem(deepBlueBox, 4, 22);
        this.AddDropItem(deepBlueBox, 5, 9);
        this.AddDropItem(deepBlueBox, 5, 13);
        this.AddDropItem(deepBlueBox, 10, 21); // Great Dragon Gloves
        this.AddDropItem(deepBlueBox, 11, 21); // Great Dragon Boots
        this.AddDropItem(deepBlueBox, 10, 22); // Dark Soul Gloves
        this.AddDropItem(deepBlueBox, 11, 22); // Dark Soul Boots
        this.AddArmorSet(deepBlueBox, 17); // Dark Phoenix Set
        this.AddArmorSet(deepBlueBox, 24); // Red Spirit Set
        this.AddArmorSet(deepBlueBox, 37); // Valiant Set
        this.AddArmorSet(deepBlueBox, 38); // Glorious Set
        this.AddDropItem(deepBlueBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(deepBlueBox, 14, 13); // Jewel of Bless
        this.AddDropItem(deepBlueBox, 14, 14); // Jewel of Soul
        this.AddDropItem(deepBlueBox, 14, 16); // Jewel of Life
        this.AddDropItem(deepBlueBox, 14, 22); // Jewel of Creation
        this.AddDropItem(deepBlueBox, 14, 42); // Jewel of Harmony

        var deepBlueBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        deepBlueBoxExc.ItemType = SpecialItemType.Excellent;
        deepBlueBoxExc.SourceItemLevel = 1;
        deepBlueBoxExc.Chance = 0.1;
        deepBlueBoxExc.Description = "Blue Chocolate Box - Excellent Items";
        deepBlueBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(deepBlueBoxExc);
        this.AddDropItem(deepBlueBoxExc, 0, 16);
        this.AddDropItem(deepBlueBoxExc, 0, 31);
        this.AddDropItem(deepBlueBoxExc, 2, 5);
        this.AddDropItem(deepBlueBoxExc, 2, 10);
        this.AddDropItem(deepBlueBoxExc, 4, 14);
        this.AddDropItem(deepBlueBoxExc, 4, 16);
        this.AddDropItem(deepBlueBoxExc, 5, 8);
        this.AddDropItem(deepBlueBoxExc, 6, 13);
        this.AddDropItem(deepBlueBoxExc, 6, 14);
        this.AddDropItem(deepBlueBoxExc, 6, 15);
        this.AddDropItem(deepBlueBoxExc, 6, 16);
        this.AddDropItem(deepBlueBoxExc, 13, 8); // Ring of Ice
        this.AddDropItem(deepBlueBoxExc, 13, 9); // Ring of Poison
        this.AddDropItem(deepBlueBoxExc, 13, 21); // Ring of Fire
        this.AddDropItem(deepBlueBoxExc, 13, 12); // Pendant of Lightning
        this.AddDropItem(deepBlueBoxExc, 13, 13); // Pendant of Fire
        this.AddDropItem(deepBlueBoxExc, 13, 25); // Pendant of Ice
        this.AddArmorSet(deepBlueBoxExc, 16); // Black Dragon Set
        this.AddArmorSet(deepBlueBoxExc, 18); // Grand Soul Set
        this.AddArmorSet(deepBlueBoxExc, 19); // Divine Set
        this.AddArmorSet(deepBlueBoxExc, 27); // Dark Steel Set

        this.AddMoneyDropFallback(box, 800000, deepBlueBox, "Deep Blue Candy Box");
    }

    private void CreatePumpkinOfLuck()
    {
        var box = this.CreateBox("Pumpkin of Luck", 14, 45);
        box.Value = 1;

        var pumpkinBox = this.Context.CreateNew<ItemDropItemGroup>();
        pumpkinBox.ItemType = SpecialItemType.RandomItem;
        pumpkinBox.Chance = 1.0;
        pumpkinBox.Description = "Pumpkin of Luck";
        box.DropItems.Add(pumpkinBox);

        this.AddDropItem(pumpkinBox, 14, 46); // Jack O'Lantern Bless Scroll
        this.AddDropItem(pumpkinBox, 14, 47); // Jack O'Lantern Rage Scroll
        this.AddDropItem(pumpkinBox, 14, 48); // Jack O'Lantern Scream Scroll
        this.AddDropItem(pumpkinBox, 14, 49); // Jack O'Lantern Food Scroll
        this.AddDropItem(pumpkinBox, 14, 50); // Jack O'Lantern Drink Scroll
    }

    private void CreateRedRibbonBox()
    {
        var box = this.CreateBox("Red Ribbon Box", 12, 32);
        var redBox = this.Context.CreateNew<ItemDropItemGroup>();
        redBox.ItemType = SpecialItemType.RandomItem;
        redBox.SourceItemLevel = 0;
        redBox.Chance = 0.5;
        redBox.Description = "Red Ribbon Box";
        redBox.MinimumLevel = 7;
        redBox.MaximumLevel = 9;
        redBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(redBox);

        this.AddDropItem(redBox, 0, 3);
        this.AddDropItem(redBox, 0, 5);
        this.AddDropItem(redBox, 0, 11);
        this.AddDropItem(redBox, 1, 3);
        this.AddDropItem(redBox, 2, 1);
        this.AddDropItem(redBox, 4, 9);
        this.AddDropItem(redBox, 4, 11);
        this.AddDropItem(redBox, 5, 0);
        this.AddDropItem(redBox, 5, 2);
        this.AddDropItem(redBox, 6, 9);
        this.AddDropItem(redBox, 6, 3);
        this.AddDropItem(redBox, 6, 6);

        this.AddArmorSet(redBox, 5);
        this.AddArmorSet(redBox, 10);
        this.AddArmorSet(redBox, 2);
        this.AddArmorSet(redBox, 6);
        this.AddArmorSet(redBox, 11);
        this.AddArmorSet(redBox, 4);
        this.AddArmorSet(redBox, 25);

        this.AddMoneyDropFallback(box, 10000, redBox);
    }

    private void CreateGreenRibbonBox()
    {
        var box = this.CreateBox("Green Ribbon Box", 12, 33);
        var greenBox = this.Context.CreateNew<ItemDropItemGroup>();
        greenBox.ItemType = SpecialItemType.RandomItem;
        greenBox.SourceItemLevel = 0;
        greenBox.Chance = 0.4;
        greenBox.Description = "Green Ribbon Box - Normal Items";
        greenBox.MinimumLevel = 7;
        greenBox.MaximumLevel = 9;
        greenBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(greenBox);
        this.AddDropItem(greenBox, 0, 14);
        this.AddDropItem(greenBox, 4, 13);
        this.AddDropItem(greenBox, 5, 5);
        this.AddDropItem(greenBox, 6, 13);
        this.AddDropItem(greenBox, 6, 14);
        this.AddDropItem(greenBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(greenBox, 14, 13); // Jewel of Bless
        this.AddDropItem(greenBox, 14, 14); // Jewel of Soul
        this.AddDropItem(greenBox, 14, 16); // Jewel of Life

        this.AddArmorSet(greenBox, 1);
        this.AddArmorSet(greenBox, 14);
        this.AddArmorSet(greenBox, 3);
        this.AddArmorSet(greenBox, 34);
        this.AddArmorSet(greenBox, 35);
        this.AddArmorSet(greenBox, 36);
        this.AddArmorSet(greenBox, 15);
        this.AddArmorSet(greenBox, 26);

        var greenBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        greenBoxExc.ItemType = SpecialItemType.Excellent;
        greenBoxExc.SourceItemLevel = 0;
        greenBoxExc.Chance = 0.1;
        greenBoxExc.Description = "Green Ribbon Box - Excellent Items";
        greenBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(greenBoxExc);
        this.AddDropItem(greenBoxExc, 0, 5);
        this.AddDropItem(greenBoxExc, 2, 3);
        this.AddDropItem(greenBoxExc, 2, 8);
        this.AddDropItem(greenBoxExc, 4, 4);
        this.AddDropItem(greenBoxExc, 4, 12);
        this.AddDropItem(greenBoxExc, 5, 2);
        this.AddDropItem(greenBoxExc, 6, 11);
        this.AddArmorSet(greenBoxExc, 5);
        this.AddArmorSet(greenBoxExc, 7);
        this.AddArmorSet(greenBoxExc, 12);

        this.AddMoneyDropFallback(box, 40000, greenBox, "Green Ribbon Box");
    }

    private void CreateBlueRibbonBox()
    {
        var box = this.CreateBox("Blue Ribbon Box", 12, 34);
        var blueBox = this.Context.CreateNew<ItemDropItemGroup>();
        blueBox.ItemType = SpecialItemType.RandomItem;
        blueBox.SourceItemLevel = 0;
        blueBox.Chance = 0.4;
        blueBox.Description = "Blue Ribbon Box - Normal Items";
        blueBox.MinimumLevel = 7;
        blueBox.MaximumLevel = 9;
        blueBox.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(blueBox);
        this.AddDropItem(blueBox, 0, 16);
        this.AddDropItem(blueBox, 2, 10);
        this.AddDropItem(blueBox, 4, 16);
        this.AddDropItem(blueBox, 5, 6);
        this.AddDropItem(blueBox, 5, 8);
        this.AddDropItem(blueBox, 12, 15); // Jewel of Chaos
        this.AddDropItem(blueBox, 14, 13); // Jewel of Bless
        this.AddDropItem(blueBox, 14, 14); // Jewel of Soul
        this.AddDropItem(blueBox, 14, 16); // Jewel of Life
        this.AddDropItem(blueBox, 14, 22); // Jewel of Creation

        this.AddArmorSet(blueBox, 17);
        this.AddArmorSet(blueBox, 19);
        this.AddArmorSet(blueBox, 18);
        this.AddArmorSet(blueBox, 20);
        this.AddArmorSet(blueBox, 27);

        var blueBoxExc = this.Context.CreateNew<ItemDropItemGroup>();
        blueBoxExc.ItemType = SpecialItemType.Excellent;
        blueBoxExc.SourceItemLevel = 0;
        blueBoxExc.Chance = 0.1;
        blueBoxExc.Description = "Blue Ribbon Box - Excellent Items";
        blueBoxExc.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(blueBoxExc);
        this.AddDropItem(blueBoxExc, 0, 14);
        this.AddDropItem(blueBoxExc, 2, 5);
        this.AddDropItem(blueBoxExc, 2, 9);
        this.AddDropItem(blueBoxExc, 4, 14);
        this.AddDropItem(blueBoxExc, 5, 5);
        this.AddDropItem(blueBoxExc, 4, 16);
        this.AddDropItem(blueBoxExc, 6, 13);
        this.AddDropItem(blueBoxExc, 6, 14);
        this.AddDropItem(blueBoxExc, 13, 8);
        this.AddDropItem(blueBoxExc, 13, 9);
        this.AddDropItem(blueBoxExc, 13, 12);
        this.AddDropItem(blueBoxExc, 13, 13);
        this.AddArmorSet(blueBoxExc, 1);
        this.AddArmorSet(blueBoxExc, 14);
        this.AddArmorSet(blueBoxExc, 3);
        this.AddArmorSet(blueBoxExc, 26);

        this.AddMoneyDropFallback(box, 80000, blueBox, "Blue Ribbon Box");
    }

    // season 2.5
    private void CreateChristmasStar()
    {
        var box = this.CreateBox("Christmas Star", 14, 51);
        var starEffect = this.Context.CreateNew<ItemDropItemGroup>();
        starEffect.ItemType = SpecialItemType.None;
        starEffect.SourceItemLevel = 0;
        starEffect.Chance = 1.0;
        starEffect.Description = "Christmas Star - Drop Effect";
        starEffect.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(starEffect);
    }

    // from Natasha NPC, drops same as christmas star, season 2.5
    private void CreateFireCracker()
    {
        var box = this.CreateBox("Firecracker", 14, 63);
        var crackerEffect = this.Context.CreateNew<ItemDropItemGroup>();
        crackerEffect.ItemType = SpecialItemType.None;
        crackerEffect.SourceItemLevel = 0;
        crackerEffect.Chance = 1.0;
        crackerEffect.Description = "Firecracker - Drop Effect";
        crackerEffect.DropEffect = ItemDropEffect.Fireworks;
        box.DropItems.Add(crackerEffect);
    }

    // Season 3
    private void CreateCherryBlossomBox()
    {
        var box = this.CreateBox("Cherry Blossom Play-Box", 14, 84);
        var playBox = this.Context.CreateNew<ItemDropItemGroup>();
        playBox.ItemType = SpecialItemType.RandomItem;
        playBox.SourceItemLevel = 0;
        playBox.Chance = 1.0;
        playBox.Description = "Cherry Blossom Play-Box";
        playBox.DropEffect = ItemDropEffect.Swirl;
        box.DropItems.Add(playBox);

        this.AddDropItem(playBox, 14, 85); // Cherry Blossom Wine
        this.AddDropItem(playBox, 14, 86); // Cherry Blossom Rice Cake
        this.AddDropItem(playBox, 14, 87); // Cherry Blossom Flower Petal
        this.AddDropItem(playBox, 14, 90); // Golden Cherry Blossom Branch
    }

    // season 4
    private void CreateChristmasFirecracker()
    {
        var box = this.CreateBox("Christmas Firecracker", 14, 99);
        var crackerEffect = this.Context.CreateNew<ItemDropItemGroup>();
        crackerEffect.ItemType = SpecialItemType.None;
        crackerEffect.SourceItemLevel = 0;
        crackerEffect.Chance = 1.0;
        crackerEffect.Description = "Christmas Firecracker - Drop Effect";
        crackerEffect.DropEffect = ItemDropEffect.ChristmasFireworks;
        box.DropItems.Add(crackerEffect);
    }

    private void CreateGameMasterPresentBox()
    {
        var box = this.CreateBox("GM Gift", 14, 52);
    }

    private void CreateWizardsRings()
    {
        // The ring already exists. At item level 0 it drops from the white wizard and can be equipped.
        // At item level 1 it acts like a box of luck, but the character must be at least of level 40.
        // At item level 2 it acts like a box of luck, but the character must be at least of level 80.
        var box = this.GameConfiguration.Items.First(i => i.Group == 13 && i.Number == 20);
        box.MaximumItemLevel = 2;

        var level40Ring = this.Context.CreateNew<ItemDropItemGroup>();
        level40Ring.ItemType = SpecialItemType.RandomItem;
        level40Ring.SourceItemLevel = 1;
        level40Ring.Chance = 1.0;
        level40Ring.Description = "Ring of Warrior 40";
        level40Ring.MinimumLevel = 6;
        level40Ring.MaximumLevel = 9;
        level40Ring.RequiredCharacterLevel = 40;
        box.DropItems.Add(level40Ring);
        this.AddDropItem(level40Ring, 0, 3); // Katana
        this.AddDropItem(level40Ring, 0, 5); // Blade
        this.AddDropItem(level40Ring, 0, 9); // Sword of Salamander
        this.AddDropItem(level40Ring, 0, 10); // Light Saber
        this.AddDropItem(level40Ring, 0, 13); // Double Blade
        this.AddDropItem(level40Ring, 3, 8); // Great Scythe
        this.AddDropItem(level40Ring, 4, 4); // Tiger Bow
        this.AddDropItem(level40Ring, 4, 5); // Silver Bow
        this.AddDropItem(level40Ring, 4, 9); // Golden Crossbow
        this.AddDropItem(level40Ring, 4, 11); // Light Crossbow
        this.AddDropItem(level40Ring, 4, 12); // Serpent Crossbow
        this.AddDropItem(level40Ring, 5, 0); // Skull Staff
        this.AddDropItem(level40Ring, 5, 2); // Serpent Staff
        this.AddDropItem(level40Ring, 5, 3); // Lightning Staff
        this.AddDropItem(level40Ring, 5, 4); // Gorgon Staff
        this.AddArmorSet(level40Ring, 0); // Bronze Set
        this.AddArmorSet(level40Ring, 2); // Pad Set
        this.AddArmorSet(level40Ring, 4); // Bone Set
        this.AddArmorSet(level40Ring, 5); // Leather Set
        this.AddArmorSet(level40Ring, 6); // Scale Set
        this.AddArmorSet(level40Ring, 7); // Sphinx Set
        this.AddArmorSet(level40Ring, 8); // Brass Set
        this.AddArmorSet(level40Ring, 10); // Vine Set
        this.AddArmorSet(level40Ring, 11); // Silk Set
        this.AddArmorSet(level40Ring, 12); // Wind Set
        this.AddMoneyDropFallback(box, 100000, level40Ring);

        var level80Ring = this.Context.CreateNew<ItemDropItemGroup>();
        level80Ring.ItemType = SpecialItemType.RandomItem;
        level80Ring.SourceItemLevel = 2;
        level80Ring.Chance = 1.0;
        level80Ring.Description = "Ring of Warrior 80";
        level80Ring.MinimumLevel = 6;
        level80Ring.MaximumLevel = 9;
        level80Ring.RequiredCharacterLevel = 80;
        box.DropItems.Add(level80Ring);
        this.AddDropItem(level80Ring, 0, 3); // Katana
        this.AddDropItem(level80Ring, 0, 5); // Blade
        this.AddDropItem(level80Ring, 0, 9); // Sword of Salamander
        this.AddDropItem(level80Ring, 0, 10); // Light Saber
        this.AddDropItem(level80Ring, 0, 13); // Double Blade
        this.AddDropItem(level80Ring, 3, 8); // Great Scythe
        this.AddDropItem(level80Ring, 4, 4); // Tiger Bow
        this.AddDropItem(level80Ring, 4, 5); // Silver Bow
        this.AddDropItem(level80Ring, 4, 9); // Golden Crossbow
        this.AddDropItem(level80Ring, 4, 11); // Light Crossbow
        this.AddDropItem(level80Ring, 4, 12); // Serpent Crossbow
        this.AddDropItem(level80Ring, 5, 0); // Skull Staff
        this.AddDropItem(level80Ring, 5, 2); // Serpent Staff
        this.AddDropItem(level80Ring, 5, 3); // Lightning Staff
        this.AddDropItem(level80Ring, 5, 4); // Gorgon Staff
        this.AddArmorSet(level80Ring, 0); // Bronze Set
        this.AddArmorSet(level80Ring, 2); // Pad Set
        this.AddArmorSet(level80Ring, 4); // Bone Set
        this.AddArmorSet(level80Ring, 5); // Leather Set
        this.AddArmorSet(level80Ring, 6); // Scale Set
        this.AddArmorSet(level80Ring, 7); // Sphinx Set
        this.AddArmorSet(level80Ring, 8); // Brass Set
        this.AddArmorSet(level80Ring, 10); // Vine Set
        this.AddArmorSet(level80Ring, 11); // Silk Set
        this.AddArmorSet(level80Ring, 12); // Wind Set
        this.AddMoneyDropFallback(box, 100000, level80Ring);
    }

    private ItemDefinition CreateBox(string name, byte group, byte number, byte width = 1, byte height = 1, byte maximumItemLevel = 0)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = width;
        item.Height = height;
        item.Durability = 1;
        item.DropsFromMonsters = false;
        item.MaximumItemLevel = maximumItemLevel;
        item.SetGuid(item.Group, item.Number);
        return item;
    }

    private void AddMoneyDropFallback(ItemDefinition item, int moneyAmount, ItemDropItemGroup baseGroup, string itemName = "")
    {
        var zenDrop = this.Context.CreateNew<ItemDropItemGroup>();
        zenDrop.ItemType = SpecialItemType.Money;
        zenDrop.MoneyAmount = moneyAmount;
        zenDrop.SourceItemLevel = baseGroup.SourceItemLevel;
        zenDrop.Chance = 1.0;
        zenDrop.Description = string.IsNullOrWhiteSpace(itemName) ? $"{baseGroup.Description} - Money" : $"{itemName} - Money";
        item.DropItems.Add(zenDrop);
    }

    private void AddArmorSet(ItemDropItemGroup dropGroup, short number)
    {
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Helm, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Armor, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Pants, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Gloves, number);
        this.TryAddDropItem(dropGroup, (byte)ItemGroups.Boots, number);
    }

    private void AddDropItem(ItemDropItemGroup dropGroup, byte group, short number)
    {
        var item = this.GameConfiguration.Items.First(i => i.Group == group && i.Number == number);
        dropGroup.PossibleItems.Add(item);
    }

    private void TryAddDropItem(ItemDropItemGroup dropGroup, byte group, short number)
    {
        if (this.GameConfiguration.Items.FirstOrDefault(i => i.Group == group && i.Number == number) is { } item)
        {
            dropGroup.PossibleItems.Add(item);
        }
    }
}
