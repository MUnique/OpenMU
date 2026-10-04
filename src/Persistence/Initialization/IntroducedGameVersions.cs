// <copyright file="IntroducedGameVersions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The versions of the original game which introduced the maps, character classes, monsters, items and mini games,
/// see <see cref="GameMapDefinition.IntroducedIn"/>, <see cref="CharacterClass.IntroducedIn"/>, <see cref="MonsterDefinition.IntroducedIn"/>,
/// <see cref="ItemDefinition.IntroducedIn"/> and <see cref="MiniGameDefinition.IntroducedIn"/>.
/// </summary>
/// <remarks>
/// The versions are informational and compiled on a best effort basis from:
/// <list type="bullet">
///   <item>the history of the original patch notes, compiled at https://github.com/Khdoop/mu-online-history,</item>
///   <item>the content of the data initializations of the versions 0.75 and 0.95d, for content which is not mentioned in the history,</item>
///   <item>and, for monsters and NPCs, the maps on which they are spawned.</item>
/// </list>
/// Entries for which the version isn't known reliably are not listed, so they stay <see cref="GameVersion.Unknown"/>.
/// The keys are the numbers of the Season 6 data; the data of the older versions uses the same numbers.
/// Where the history contradicts the data of the older versions, the data wins: according to the history,
/// the Magic Gladiator and its Storm Crow set were introduced before 0.75, but the 0.75 client is not capable of
/// showing them, so they are listed as introduced in <see cref="GameVersion.Version095d"/>.
/// </remarks>
public static class IntroducedGameVersions
{
    /// <summary>
    /// Gets the versions of the maps by their number and discriminator.
    /// </summary>
    public static IReadOnlyDictionary<(short Number, int Discriminator), GameVersion> Maps { get; } = new Dictionary<(short Number, int Discriminator), GameVersion>
    {
        [(0, 0)] = GameVersion.Version029, // Lorencia
        [(1, 0)] = GameVersion.Version029, // Dungeon
        [(2, 0)] = GameVersion.Version029, // Devias
        [(3, 0)] = GameVersion.Version034, // Noria
        [(4, 0)] = GameVersion.Version045, // Lost Tower
        [(5, 0)] = GameVersion.Version075, // Exile
        [(6, 0)] = GameVersion.Version066, // Arena
        [(7, 0)] = GameVersion.Version064, // Atlans
        [(8, 0)] = GameVersion.Version084, // Tarkan
        [(9, 1)] = GameVersion.Version089c, // Devil Square 1
        [(9, 2)] = GameVersion.Version089c, // Devil Square 2
        [(9, 3)] = GameVersion.Version089c, // Devil Square 3
        [(9, 4)] = GameVersion.Version089c, // Devil Square 4
        [(10, 0)] = GameVersion.Version094b, // Icarus
        [(11, 0)] = GameVersion.Version096y, // Blood Castle 1
        [(12, 0)] = GameVersion.Version096y, // Blood Castle 2
        [(13, 0)] = GameVersion.Version096y, // Blood Castle 3
        [(14, 0)] = GameVersion.Version096y, // Blood Castle 4
        [(15, 0)] = GameVersion.Version096y, // Blood Castle 5
        [(16, 0)] = GameVersion.Version096y, // Blood Castle 6
        [(17, 0)] = GameVersion.Version098r, // Blood Castle 7
        [(18, 0)] = GameVersion.Version099, // Chaos Castle 1
        [(19, 0)] = GameVersion.Version099, // Chaos Castle 2
        [(20, 0)] = GameVersion.Version099, // Chaos Castle 3
        [(21, 0)] = GameVersion.Version099, // Chaos Castle 4
        [(22, 0)] = GameVersion.Version099, // Chaos Castle 5
        [(23, 0)] = GameVersion.Version099, // Chaos Castle 6
        [(24, 0)] = GameVersion.Version099GPlus, // Kalima 1
        [(25, 0)] = GameVersion.Version099GPlus, // Kalima 2
        [(26, 0)] = GameVersion.Version099GPlus, // Kalima 3
        [(27, 0)] = GameVersion.Version099GPlus, // Kalima 4
        [(28, 0)] = GameVersion.Version099GPlus, // Kalima 5
        [(29, 0)] = GameVersion.Version099GPlus, // Kalima 6
        [(30, 0)] = GameVersion.Version100s, // Valley of Loren
        [(31, 0)] = GameVersion.Version100s, // Land_of_Trials
        [(32, 5)] = GameVersion.Season1, // Devil Square 5
        [(32, 6)] = GameVersion.Season1, // Devil Square 6
        [(32, 7)] = GameVersion.Season3Episode1, // Devil Square 7
        [(33, 0)] = GameVersion.Season1, // Aida
        [(34, 0)] = GameVersion.Season1, // Crywolf Fortress
        [(36, 0)] = GameVersion.Season1, // Kalima 7
        [(37, 0)] = GameVersion.Season2, // Kanturu_I
        [(38, 0)] = GameVersion.Season2, // Kanturu_III
        [(39, 0)] = GameVersion.Season2, // Kanturu Event
        [(40, 0)] = GameVersion.Season2, // Silent Map?
        [(41, 0)] = GameVersion.Season2, // Barracks of Balgass
        [(42, 0)] = GameVersion.Season2, // Balgass Refuge
        [(45, 0)] = GameVersion.Season2, // Illusion Temple 1
        [(46, 0)] = GameVersion.Season2, // Illusion Temple 2
        [(47, 0)] = GameVersion.Season2, // Illusion Temple 3
        [(48, 0)] = GameVersion.Season2, // Illusion Temple 4
        [(49, 0)] = GameVersion.Season2, // Illusion Temple 5
        [(50, 0)] = GameVersion.Season3Episode1, // Illusion Temple 6
        [(51, 0)] = GameVersion.Season3Episode1, // Elvenland
        [(52, 0)] = GameVersion.Season3Episode1, // Blood Castle 8
        [(53, 0)] = GameVersion.Season3Episode1, // Chaos Castle 7
        [(56, 0)] = GameVersion.Season3Episode2, // Swamp Of Calmness
        [(57, 0)] = GameVersion.Season4Episode1, // LaCleon
        [(58, 0)] = GameVersion.Season4Episode1, // LaCleon Boss
        [(62, 0)] = GameVersion.Season4Episode1, // Santa Village
        [(63, 0)] = GameVersion.Season4Episode2, // Vulcanus
        [(64, 0)] = GameVersion.Season4Episode2, // Duel Arena
        [(65, 0)] = GameVersion.Season5Episode1, // Doppelgaenger 1
        [(66, 0)] = GameVersion.Season5Episode1, // Doppelgaenger 2
        [(67, 0)] = GameVersion.Season5Episode1, // Doppelgaenger 3
        [(68, 0)] = GameVersion.Season5Episode1, // Doppelgaenger 4
        [(69, 0)] = GameVersion.Season5Episode1, // Fortress of Imperial Guardian 1
        [(70, 0)] = GameVersion.Season5Episode1, // Fortress of Imperial Guardian 2
        [(71, 0)] = GameVersion.Season5Episode1, // Fortress of Imperial Guardian 3
        [(72, 0)] = GameVersion.Season5Episode1, // Fortress of Imperial Guardian 4
        [(79, 0)] = GameVersion.Season5Episode3, // LorenMarket
        [(80, 0)] = GameVersion.Season6Episode1, // Karutan 1
        [(81, 0)] = GameVersion.Season6Episode1, // Karutan 2
    };

    /// <summary>
    /// Gets the versions of the character classes by their number.
    /// </summary>
    public static IReadOnlyDictionary<byte, GameVersion> CharacterClasses { get; } = new Dictionary<byte, GameVersion>
    {
        [0] = GameVersion.Version029, // Dark Wizard
        [2] = GameVersion.Version095k, // Soul Master
        [3] = GameVersion.Season2, // Grand Master
        [4] = GameVersion.Version029, // Dark Knight
        [6] = GameVersion.Version095k, // Blade Knight
        [7] = GameVersion.Season2, // Blade Master
        [8] = GameVersion.Version034, // Fairy Elf
        [10] = GameVersion.Version095k, // Muse Elf
        [11] = GameVersion.Season2, // High Elf
        [12] = GameVersion.Version095d, // Magic Gladiator
        [13] = GameVersion.Season2, // Duel Master
        [16] = GameVersion.Version099GPlus, // Dark Lord
        [17] = GameVersion.Season2, // Lord Emperor
        [20] = GameVersion.Season3Episode1, // Summoner
        [22] = GameVersion.Season3Episode1, // Bloody Summoner
        [23] = GameVersion.Season3Episode2, // Dimension Master
        [24] = GameVersion.Season6Episode1, // Rage Fighter
        [25] = GameVersion.Season6Episode1, // Fist Master
    };

    /// <summary>
    /// Gets the versions of the mini games by their type and level.
    /// </summary>
    public static IReadOnlyDictionary<(MiniGameType Type, byte Level), GameVersion> MiniGames { get; } = new Dictionary<(MiniGameType Type, byte Level), GameVersion>
    {
        [(MiniGameType.DevilSquare, 1)] = GameVersion.Version089c,
        [(MiniGameType.DevilSquare, 2)] = GameVersion.Version089c,
        [(MiniGameType.DevilSquare, 3)] = GameVersion.Version089c,
        [(MiniGameType.DevilSquare, 4)] = GameVersion.Version089c,
        [(MiniGameType.DevilSquare, 5)] = GameVersion.Season1,
        [(MiniGameType.DevilSquare, 6)] = GameVersion.Season1,
        [(MiniGameType.DevilSquare, 7)] = GameVersion.Season3Episode1,
        [(MiniGameType.BloodCastle, 1)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 2)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 3)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 4)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 5)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 6)] = GameVersion.Version096y,
        [(MiniGameType.BloodCastle, 7)] = GameVersion.Version098r,
        [(MiniGameType.BloodCastle, 8)] = GameVersion.Season3Episode1,
        [(MiniGameType.ChaosCastle, 1)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 2)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 3)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 4)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 5)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 6)] = GameVersion.Version099,
        [(MiniGameType.ChaosCastle, 7)] = GameVersion.Season3Episode1,
        [(MiniGameType.Kanturu, 1)] = GameVersion.Season2,
        [(MiniGameType.Doppelganger, 1)] = GameVersion.Season5Episode1,
        [(MiniGameType.Doppelganger, 2)] = GameVersion.Season5Episode1,
        [(MiniGameType.Doppelganger, 3)] = GameVersion.Season5Episode1,
        [(MiniGameType.Doppelganger, 4)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 1)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 2)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 3)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 4)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 5)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 6)] = GameVersion.Season5Episode1,
        [(MiniGameType.ImperialGuardian, 7)] = GameVersion.Season5Episode1,
    };

    /// <summary>
    /// Gets the versions of the monsters and NPCs, as ranges of their numbers.
    /// </summary>
    public static IReadOnlyList<(short From, short To, GameVersion Version)> Monsters { get; } =
    [
        (0, 18, GameVersion.Version029), // Bull Fighter - Gorgon
        (19, 19, GameVersion.Version075), // Yeti
        (20, 25, GameVersion.Version029), // Elite Yeti - Ice Queen
        (26, 33, GameVersion.Version034), // Goblin - Elite Goblin
        (34, 41, GameVersion.Version045), // Cursed Wizard - Death Cow
        (43, 43, GameVersion.Version095d), // Golden Budge Dragon
        (44, 44, GameVersion.Version048), // Red Dragon
        (45, 49, GameVersion.Version064), // Bahamut - Hydra
        (50, 50, GameVersion.Version075), // Sea Worm
        (51, 52, GameVersion.Version064), // Great Bahamut - Silver Valkyrie
        (53, 54, GameVersion.Version095d), // Golden Titan - Golden Soldier
        (57, 63, GameVersion.Version084), // Iron Wheel - Death Beam Knight
        (64, 67, GameVersion.Version089c), // Orc Archer - Metal Balrog
        (69, 69, GameVersion.Version094b), // Alquamos
        (70, 70, GameVersion.Version089c), // Queen Rainer
        (71, 75, GameVersion.Version094b), // Mega Crust - Great Drakan
        (76, 76, GameVersion.Version095d), // Dark Phoenix Shield
        (77, 77, GameVersion.Version094b), // Dark Phoenix
        (79, 79, GameVersion.Version095k), // Golden Dragon
        (84, 99, GameVersion.Version096y), // Chief Skeleton Warrior 1 - Giant Ogre 3
        (100, 102, GameVersion.Version029), // Lance Trap - Fire Trap
        (103, 103, GameVersion.Version045), // Meteorite Trap
        (105, 106, GameVersion.Season2), // Canon Trap - Laser Trap
        (111, 134, GameVersion.Version096y), // Red Skeleton Knight 3 - Statue of Saint
        (135, 137, GameVersion.Version097r), // White Wizard - Destructive Ogre Archer
        (138, 143, GameVersion.Version098r), // Chief Skeleton Warrior 7 - Magic Skeleton 7
        (144, 149, GameVersion.Version099GPlus), // Death Angel 1 - Necron 1
        (150, 150, GameVersion.Version075), // Bali
        (151, 151, GameVersion.Version084), // Soldier
        (152, 157, GameVersion.Version099GPlus), // Gate to Kalima 1 of {0} - Gate to Kalima 6 of {0}
        (158, 158, GameVersion.Season1), // Gate to Kalima 7 of {0}
        (160, 161, GameVersion.Version099GPlus), // Schriker 1 - Illusion of Kundun 1
        (162, 173, GameVersion.Version099), // Chaos Castle 1 - Chaos Castle 12
        (174, 197, GameVersion.Version099GPlus), // Death Angel 2 - Illusion of Kundun 4
        (200, 200, GameVersion.Version066), // Soccerball
        (204, 209, GameVersion.Season1), // Wolf Status - Wolf Altar5
        (215, 222, GameVersion.Version100s), // Shield - Slingshot Defense
        (223, 223, GameVersion.Season1), // Senior
        (224, 224, GameVersion.Version100s), // Guardsman
        (226, 226, GameVersion.Version099GPlus), // Pet Trainer
        (229, 229, GameVersion.Version097p), // Marlon
        (232, 233, GameVersion.Version096y), // Archangel - Messenger of Arch.
        (235, 235, GameVersion.Version095d), // Sevina the Priestess
        (236, 236, GameVersion.Version094b), // Golden Archer
        (237, 237, GameVersion.Version089c), // Charon
        (238, 238, GameVersion.Version068), // Chaos Goblin
        (239, 239, GameVersion.Version066), // Arena Guard
        (240, 240, GameVersion.Version060), // Baz The Vault Keeper
        (241, 241, GameVersion.Version043), // Guild Master
        (242, 243, GameVersion.Version034), // Elf Lala - Eo the Craftsman
        (244, 247, GameVersion.Version029), // Caren the Barmaid - Crossbow Guard
        (248, 248, GameVersion.Version075), // Wandering Merchant Martin
        (249, 249, GameVersion.Version029), // Berdysh Guard
        (250, 250, GameVersion.Version075), // Wandering Merchant Harold
        (251, 251, GameVersion.Version029), // Hanzo The Blacksmith
        (253, 255, GameVersion.Version029), // Potion Girl Amy - Lumen the Barmaid
        (256, 257, GameVersion.Season1), // Lahap - Elf Soldier
        (259, 274, GameVersion.Version099GPlus), // Oracle Layla - Schriker 6
        (275, 275, GameVersion.Season1), // Illusion of Kundun 7
        (277, 278, GameVersion.Version100s), // Castle Gate1 - Life Stone
        (283, 283, GameVersion.Version100s), // Guardian Statue
        (285, 288, GameVersion.Version100s), // Guardian - Canon Tower
        (290, 295, GameVersion.Version100s), // Lizard Warrior - Erohim
        (304, 317, GameVersion.Season1), // Witch Queen - Soram
        (331, 337, GameVersion.Season1), // Aegis 7 - Schriker 7
        (338, 338, GameVersion.Version099GPlus), // Illusion of Kundun 6
        (350, 364, GameVersion.Season2), // Berserker - Maya
        (367, 368, GameVersion.Season2), // Gateway Machine - Elphis
        (369, 370, GameVersion.Season1), // Osbourne - Jerridon
        (376, 378, GameVersion.Season2), // Pamela the Supplier - GameMaster
        (380, 385, GameVersion.Season2), // Stone Statue - Mirage
        (404, 412, GameVersion.Season2), // MU Allies - Dark Elf (Trainee Soldier)
        (415, 440, GameVersion.Season3Episode1), // Silvia - Dark_Elf
        (441, 449, GameVersion.Season3Episode2), // Sapi-Unus - Blaze Napin
        (452, 462, GameVersion.Season4Episode1), // Seed Master - Spider Eggs 3
        (467, 477, GameVersion.Season4Episode1), // Snowman - Transformed Snowman
        (479, 492, GameVersion.Season4Episode2), // Gatekeeper Titus - Moss The Merchant
        (504, 542, GameVersion.Season5Episode1), // Gayion The Gladiator - Golden Compensation Box
        (543, 544, GameVersion.Season5Episode2), // Gens Duprian - Gens Vanert
        (545, 547, GameVersion.Season5Episode3), // Christine the General Goods Merchant - Market Union Member Julia
        (549, 559, GameVersion.Season5Episode4), // Bloody Orc - Shadow Master
        (562, 565, GameVersion.Season5Episode4), // Dark Mammoth - Dark Iron Knight
        (569, 578, GameVersion.Season6Episode1), // Venomous Chain Scorpion - Weapons Merchant Bolo
        (579, 579, GameVersion.Season3Episode1), // David
        (658, 668, GameVersion.Season2), // Cursed Statue - Captured Stone Statue (10)
    ];

    /// <summary>
    /// Gets the versions of the items, as ranges of their numbers within their group.
    /// </summary>
    public static IReadOnlyList<(byte Group, short From, short To, GameVersion Version)> Items { get; } =
    [
        (0, 0, 11, GameVersion.Version075), // Kris - Legendary Sword
        (0, 12, 12, GameVersion.Version045), // Heliacal Sword
        (0, 13, 15, GameVersion.Version075), // Double Blade - Giant Sword
        (0, 16, 16, GameVersion.Version084), // Sword of Destruction
        (0, 17, 18, GameVersion.Version095d), // Dark Breaker - Thunder Blade
        (0, 19, 19, GameVersion.Version096y), // Divine Sword of Archangel
        (0, 20, 21, GameVersion.Version099GPlus), // Knight Blade - Dark Reign Blade
        (0, 22, 23, GameVersion.Season1), // Bone Blade - Explosion Blade
        (0, 24, 25, GameVersion.Season2), // Daybreak - Sword Dancer
        (0, 26, 28, GameVersion.Season4Episode1), // Flamberge - Imperial Sword
        (0, 31, 31, GameVersion.Version096y), // Rune Blade
        (0, 32, 34, GameVersion.Season6Episode1), // Sacred Glove - Piercing Blade Glove
        (0, 35, 35, GameVersion.Season6Episode2), // Phoenix Soul Star
        (1, 0, 7, GameVersion.Version075), // Small Axe - Larkan Axe
        (1, 8, 8, GameVersion.Version045), // Crescent Axe
        (2, 0, 3, GameVersion.Version075), // Mace - Great Hammer
        (2, 4, 5, GameVersion.Version064), // Crystal Morning Star - Crystal Sword
        (2, 6, 6, GameVersion.Version068), // Chaos Dragon Axe
        (2, 7, 7, GameVersion.Version096y), // Elemental Mace
        (2, 8, 12, GameVersion.Version099GPlus), // Battle Scepter - Great Lord Scepter
        (2, 13, 13, GameVersion.Version098r), // Divine Scepter of Archangel
        (2, 14, 14, GameVersion.Season1), // Soleil Scepter
        (2, 15, 15, GameVersion.Season2), // Shining Scepter
        (2, 16, 17, GameVersion.Season4Episode1), // Frost Mace - Absolute Scepter
        (2, 18, 18, GameVersion.Season4Episode2), // Stryker Scepter
        (3, 0, 7, GameVersion.Version075), // Light Spear - Berdysh
        (3, 8, 9, GameVersion.Version045), // Great Scythe - Bill of Balrog
        (3, 10, 10, GameVersion.Version096y), // Dragon Spear
        (3, 11, 11, GameVersion.Season4Episode2), // Beuroba
        (4, 0, 0, GameVersion.Version034), // Short Bow
        (4, 1, 1, GameVersion.Version075), // Bow
        (4, 2, 4, GameVersion.Version034), // Elven Bow - Tiger Bow
        (4, 5, 5, GameVersion.Version045), // Silver Bow
        (4, 6, 6, GameVersion.Version068), // Chaos Nature Bow
        (4, 7, 12, GameVersion.Version034), // Bolt - Serpent Crossbow
        (4, 13, 14, GameVersion.Version064), // Bluewing Crossbow - Aquagold Crossbow
        (4, 15, 15, GameVersion.Version034), // Arrows
        (4, 16, 16, GameVersion.Version084), // Saint Crossbow
        (4, 17, 17, GameVersion.Version095k), // Celestial Bow
        (4, 18, 18, GameVersion.Version096y), // Divine Crossbow of Archangel
        (4, 19, 19, GameVersion.Version097p), // Great Reign Crossbow
        (4, 20, 20, GameVersion.Version099GPlus), // Arrow Viper Bow
        (4, 21, 21, GameVersion.Season1), // Sylph Wind Bow
        (4, 22, 22, GameVersion.Season2), // Albatross Bow
        (4, 23, 23, GameVersion.Season4Episode1), // Stinger Bow
        (4, 24, 24, GameVersion.Season4Episode2), // Air Lyn Bow
        (5, 0, 4, GameVersion.Version075), // Skull Staff - Gorgon Staff
        (5, 5, 5, GameVersion.Version045), // Legendary Staff
        (5, 6, 6, GameVersion.Version064), // Staff of Resurrection
        (5, 7, 7, GameVersion.Version068), // Chaos Lightning Staff
        (5, 8, 8, GameVersion.Version084), // Staff of Destruction
        (5, 9, 9, GameVersion.Version095k), // Dragon Soul Staff
        (5, 10, 10, GameVersion.Version096y), // Divine Staff of Archangel
        (5, 11, 11, GameVersion.Version099GPlus), // Staff of Kundun
        (5, 12, 12, GameVersion.Season1), // Grand Viper Staff
        (5, 13, 13, GameVersion.Season2), // Platina Staff
        (5, 14, 17, GameVersion.Season3Episode1), // Mistery Stick - Ancient Stick
        (5, 18, 19, GameVersion.Season4Episode1), // Demonic Stick - Storm Blitz Stick
        (5, 20, 20, GameVersion.Season6Episode2), // Eternal Wing Stick
        (5, 21, 22, GameVersion.Season3Episode1), // Book of Sahamutt - Book of Neil
        (5, 23, 23, GameVersion.Season4Episode1), // Book of Lagle
        (5, 30, 31, GameVersion.Season4Episode1), // Deadly Staff - Imperial Staff
        (5, 33, 34, GameVersion.Season4Episode2), // Chromatic Staff - Raven Stick
        (5, 36, 36, GameVersion.Season6Episode2), // Divine Stick of Archangel
        (6, 0, 12, GameVersion.Version075), // Small Shield - Bronze Shield
        (6, 13, 14, GameVersion.Version045), // Dragon Shield - Legendary Shield
        (6, 15, 16, GameVersion.Version096y), // Grand Soul Shield - Elemental Shield
        (6, 17, 20, GameVersion.Season4Episode1), // Crimson Glory - Guardian Shield
        (6, 21, 21, GameVersion.Version099GPlus), // Cross Shield
        (7, 0, 9, GameVersion.Version075), // Bronze Helm - Plate Helm
        (7, 10, 13, GameVersion.Version034), // Vine Helm - Spirit Helm
        (7, 14, 14, GameVersion.Version045), // Guardian Helm
        (7, 16, 16, GameVersion.Version084), // Black Dragon Helm
        (7, 17, 19, GameVersion.Version095k), // Dark Phoenix Helm - Divine Helm
        (7, 21, 22, GameVersion.Version099GPlus), // Great Dragon Helm - Dark Soul Helm
        (7, 24, 28, GameVersion.Version099GPlus), // Red Spirit Helm - Dark Master Mask
        (7, 29, 31, GameVersion.Season1), // Dragon Knight Helm - Sylphid Ray Helm
        (7, 33, 33, GameVersion.Season1), // Sunlight Mask
        (7, 34, 36, GameVersion.Season2), // Ashcrow Helm - Iris Helm
        (7, 38, 38, GameVersion.Season2), // Glorious Mask
        (7, 39, 41, GameVersion.Season3Episode1), // Mistery Helm - Ancient Helm
        (7, 42, 43, GameVersion.Season4Episode1), // Black Rose Helm - Aura Helm
        (7, 44, 44, GameVersion.Season6Episode2), // Lilium Helm
        (7, 45, 46, GameVersion.Season4Episode1), // Titan Helm - Brave Helm
        (7, 49, 52, GameVersion.Season4Episode1), // Seraphim Helm - Hades Helm
        (7, 59, 61, GameVersion.Season6Episode1), // Sacred Helm - Piercing Helm
        (7, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Helmet
        (8, 0, 0, GameVersion.Version075), // Bronze Armor
        (8, 1, 1, GameVersion.Version045), // Dragon Armor
        (8, 2, 2, GameVersion.Version075), // Pad Armor
        (8, 3, 3, GameVersion.Version045), // Legendary Armor
        (8, 4, 9, GameVersion.Version075), // Bone Armor - Plate Armor
        (8, 10, 13, GameVersion.Version034), // Vine Armor - Spirit Armor
        (8, 14, 14, GameVersion.Version045), // Guardian Armor
        (8, 15, 15, GameVersion.Version095d), // Storm Crow Armor
        (8, 16, 16, GameVersion.Version084), // Black Dragon Armor
        (8, 17, 20, GameVersion.Version095k), // Dark Phoenix Armor - Thunder Hawk Armor
        (8, 21, 28, GameVersion.Version099GPlus), // Great Dragon Armor - Dark Master Armor
        (8, 29, 33, GameVersion.Season1), // Dragon Knight Armor - Sunlight Armor
        (8, 34, 38, GameVersion.Season2), // Ashcrow Armor - Glorious Armor
        (8, 39, 41, GameVersion.Season3Episode1), // Mistery Armor - Ancient Armor
        (8, 42, 43, GameVersion.Season4Episode1), // Black Rose Armor - Aura Armor
        (8, 44, 44, GameVersion.Season6Episode2), // Lilium Armor
        (8, 45, 52, GameVersion.Season4Episode1), // Titan Armor - Hades Armor
        (8, 59, 61, GameVersion.Season6Episode1), // Sacred Armor - Piercing Armor
        (8, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Armor
        (9, 0, 0, GameVersion.Version075), // Bronze Pants
        (9, 1, 1, GameVersion.Version045), // Dragon Pants
        (9, 2, 9, GameVersion.Version075), // Pad Pants - Plate Pants
        (9, 10, 13, GameVersion.Version034), // Vine Pants - Spirit Pants
        (9, 14, 14, GameVersion.Version045), // Guardian Pants
        (9, 15, 15, GameVersion.Version095d), // Storm Crow Pants
        (9, 16, 16, GameVersion.Version084), // Black Dragon Pants
        (9, 17, 20, GameVersion.Version095k), // Dark Phoenix Pants - Thunder Hawk Pants
        (9, 21, 28, GameVersion.Version099GPlus), // Great Dragon Pants - Dark Master Pants
        (9, 29, 33, GameVersion.Season1), // Dragon Knight Pants - Sunlight Pants
        (9, 34, 38, GameVersion.Season2), // Ashcrow Pants - Glorious Pants
        (9, 39, 41, GameVersion.Season3Episode1), // Mistery Pants - Ancient Pants
        (9, 42, 43, GameVersion.Season4Episode1), // Black Rose Pants - Aura Pants
        (9, 44, 44, GameVersion.Season6Episode2), // Lilium Pants
        (9, 45, 52, GameVersion.Season4Episode1), // Titan Pants - Hades Pants
        (9, 59, 61, GameVersion.Season6Episode1), // Sacred Pants - Piercing Pants
        (9, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Pants
        (10, 0, 9, GameVersion.Version075), // Bronze Gloves - Plate Gloves
        (10, 10, 13, GameVersion.Version034), // Vine Gloves - Spirit Gloves
        (10, 14, 14, GameVersion.Version045), // Guardian Gloves
        (10, 15, 15, GameVersion.Version095d), // Storm Crow Gloves
        (10, 16, 16, GameVersion.Version084), // Black Dragon Gloves
        (10, 17, 20, GameVersion.Version095k), // Dark Phoenix Gloves - Thunder Hawk Gloves
        (10, 21, 28, GameVersion.Version099GPlus), // Great Dragon Gloves - Dark Master Gloves
        (10, 29, 33, GameVersion.Season1), // Dragon Knight Gloves - Sunlight Gloves
        (10, 34, 38, GameVersion.Season2), // Ashcrow Gloves - Glorious Gloves
        (10, 39, 41, GameVersion.Season3Episode1), // Mistery Gloves - Ancient Gloves
        (10, 42, 43, GameVersion.Season4Episode1), // Black Rose Gloves - Aura Gloves
        (10, 44, 44, GameVersion.Season6Episode2), // Lilium Gloves
        (10, 45, 52, GameVersion.Season4Episode1), // Titan Gloves - Hades Gloves
        (11, 0, 9, GameVersion.Version075), // Bronze Boots - Plate Boots
        (11, 10, 13, GameVersion.Version034), // Vine Boots - Spirit Boots
        (11, 14, 14, GameVersion.Version045), // Guardian Boots
        (11, 15, 15, GameVersion.Version095d), // Storm Crow Boots
        (11, 16, 16, GameVersion.Version084), // Black Dragon Boots
        (11, 17, 20, GameVersion.Version095k), // Dark Phoenix Boots - Thunder Hawk Boots
        (11, 21, 28, GameVersion.Version099GPlus), // Great Dragon Boots - Dark Master Boots
        (11, 29, 33, GameVersion.Season1), // Dragon Knight Boots - Sunlight Boots
        (11, 34, 38, GameVersion.Season2), // Ashcrow Boots - Glorious Boots
        (11, 39, 41, GameVersion.Season3Episode1), // Mistery Boots - Ancient Boots
        (11, 42, 43, GameVersion.Season4Episode1), // Black Rose Boots - Aura Boots
        (11, 44, 44, GameVersion.Season6Episode2), // Lilium Boots
        (11, 45, 52, GameVersion.Season4Episode1), // Titan Boots - Hades Boots
        (11, 59, 61, GameVersion.Season6Episode1), // Sacred Boots - Piercing Boots
        (11, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Boots
        (12, 0, 2, GameVersion.Version075), // Wings of Elf - Wings of Satan
        (12, 3, 6, GameVersion.Version095k), // Wings of Spirits - Wings of Darkness
        (12, 7, 7, GameVersion.Version084), // Orb of Twisting Slash
        (12, 8, 8, GameVersion.Version075), // Orb of Healing
        (12, 9, 10, GameVersion.Version034), // Orb of Greater Defense - Orb of Greater Damage
        (12, 11, 11, GameVersion.Version075), // Orb of Summoning
        (12, 12, 14, GameVersion.Version095k), // Orb of Rageful Blow - Orb of Greater Fortitude
        (12, 15, 15, GameVersion.Version068), // Jewel of Chaos
        (12, 16, 19, GameVersion.Version095k), // Orb of Fire Slash - Orb of Death Stab
        (12, 21, 24, GameVersion.Version099GPlus), // Scroll of FireBurst - Scroll of Electric Spark
        (12, 30, 31, GameVersion.Season1), // Packed Jewel of Bless - Packed Jewel of Soul
        (12, 32, 40, GameVersion.Season2), // Red Ribbon Box - Cape of Emperor
        (12, 41, 42, GameVersion.Season3Episode1), // Wings of Curse - Wings of Despair
        (12, 43, 43, GameVersion.Season3Episode2), // Wing of Dimension
        (12, 44, 48, GameVersion.Season4Episode1), // Crystal of Destruction - Scroll of Chaotic Diseier
        (12, 49, 50, GameVersion.Season6Episode1), // Cape of Fighter - Cape of Overrule
        (12, 60, 64, GameVersion.Season4Episode1), // Seed (Fire) - Seed (Lightning)
        (12, 65, 65, GameVersion.Season6Episode1), // Seed (Earth)
        (12, 70, 70, GameVersion.Season4Episode1), // Sphere (Mono)
        (12, 71, 72, GameVersion.Season5Episode4), // Sphere (Di) - Sphere (Tri)
        (12, 73, 74, GameVersion.Season6Episode1), // Sphere (4) - Sphere (5)
        (12, 100, 104, GameVersion.Season4Episode1), // Seed Sphere (Fire) (1) - Seed Sphere (Lightning) (1)
        (12, 105, 105, GameVersion.Season6Episode1), // Seed Sphere (Earth) (1)
        (12, 106, 110, GameVersion.Season5Episode4), // Seed Sphere (Fire) (2) - Seed Sphere (Lightning) (2)
        (12, 111, 111, GameVersion.Season6Episode1), // Seed Sphere (Earth) (2)
        (12, 112, 116, GameVersion.Season5Episode4), // Seed Sphere (Fire) (3) - Seed Sphere (Lightning) (3)
        (12, 117, 129, GameVersion.Season6Episode1), // Seed Sphere (Earth) (3) - Seed Sphere (Earth) (5)
        (12, 130, 134, GameVersion.Season5Episode2), // Small Cape of Lord - Small Wings of Satan
        (12, 135, 135, GameVersion.Season6Episode1), // Little Warrior's Cloak
        (13, 0, 2, GameVersion.Version075), // Guardian Angel - Horn of Uniria
        (13, 3, 3, GameVersion.Version094b), // Horn of Dinorant
        (13, 4, 5, GameVersion.Version099GPlus), // Dark Horse - Dark Raven
        (13, 8, 9, GameVersion.Version075), // Ring of Ice - Ring of Poison
        (13, 10, 10, GameVersion.Version064), // Transformation Ring
        (13, 11, 11, GameVersion.Version100s), // Life Stone
        (13, 12, 13, GameVersion.Version075), // Pendant of Lighting - Pendant of Fire
        (13, 14, 15, GameVersion.Version095k), // Loch's Feather - Fruits
        (13, 16, 19, GameVersion.Version096y), // Scroll of Archangel - Weapon of Archangel
        (13, 29, 29, GameVersion.Version100s), // Armor of Guardsman
        (13, 30, 31, GameVersion.Version099GPlus), // Cape of Lord - Spirit
        (13, 32, 37, GameVersion.Season1), // Splinter of Armor - Horn of Fenrir
        (13, 38, 38, GameVersion.Season2), // Moonstone Pendant
        (13, 40, 40, GameVersion.Season2), // Jack O'lantern Transformation Ring
        (13, 42, 42, GameVersion.Season2), // Game Master Transformation Ring
        (13, 49, 53, GameVersion.Season2), // Old Scroll - Feather of Condor
        (13, 64, 65, GameVersion.Season5Episode2), // Demon - Spirit of Guardian
        (13, 68, 68, GameVersion.Season4Episode1), // Snowman Transformation Ring
        (13, 125, 125, GameVersion.Season5Episode1), // Doppelganger Free Ticket
        (14, 0, 6, GameVersion.Version075), // Apple - Large Mana Potion
        (14, 7, 7, GameVersion.Version100s), // Potion of Bless,Potion of Soul
        (14, 8, 10, GameVersion.Version075), // Antidote - Town Portal Scroll
        (14, 11, 11, GameVersion.Version048), // Box of Luck
        (14, 13, 14, GameVersion.Version075), // Jewel of Bless - Jewel of Soul
        (14, 16, 16, GameVersion.Version084), // Jewel of Life
        (14, 17, 19, GameVersion.Version095d), // Devil's Eye - Devil's Invitation
        (14, 21, 22, GameVersion.Version094b), // Rena - Jewel of Creation
        (14, 23, 26, GameVersion.Version095k), // Scroll of Emperor,Ring of Honor - Soul Shard of Wizard
        (14, 28, 29, GameVersion.Version099GPlus), // Lost Map - Symbol of Kundun
        (14, 31, 31, GameVersion.Season1), // Jewel of Guardian
        (14, 35, 40, GameVersion.Season1), // Small Shield Potion - Large Complex Potion
        (14, 41, 50, GameVersion.Season2), // Gemstone - Jack O'Lantern Drink
        (14, 52, 52, GameVersion.Season2), // GM Gift
        (14, 63, 63, GameVersion.Version089c), // Firecracker
        (14, 65, 67, GameVersion.Season2), // Flame of Death Beam Knight - Feather of Dark Phoenix
        (14, 68, 68, GameVersion.Season3Episode1), // Eye of Abyssal
        (14, 101, 111, GameVersion.Season5Episode1), // Suspicious Scrap of Paper - Mirror of Dimensions
        (15, 0, 8, GameVersion.Version075), // Scroll of Poison - Scroll of Evil Spirit
        (15, 9, 9, GameVersion.Version045), // Scroll of Hellfire
        (15, 10, 10, GameVersion.Version075), // Scroll of Power Wave
        (15, 11, 11, GameVersion.Version064), // Scroll of Aqua Beam
        (15, 12, 12, GameVersion.Version095d), // Scroll of Cometfall
        (15, 13, 13, GameVersion.Version084), // Scroll of Inferno
        (15, 14, 15, GameVersion.Version095k), // Scroll of Teleport Ally - Scroll of Soul Barrier
        (15, 16, 18, GameVersion.Version097p), // Scroll of Decay - Scroll of Nova
        (15, 19, 20, GameVersion.Season3Episode1), // Chain Lightning Parchment - Drain Life Parchment
        (15, 21, 21, GameVersion.Season4Episode1), // Lightning Shock Parchment
        (15, 22, 22, GameVersion.Season3Episode1), // Damage Reflection Parchment
        (15, 23, 23, GameVersion.Season4Episode1), // Berserker Parchment
        (15, 24, 24, GameVersion.Season3Episode1), // Sleep Parchment
        (15, 26, 27, GameVersion.Season3Episode2), // Weakness Parchment - Innovation Parchment
        (15, 28, 29, GameVersion.Season4Episode1), // Scroll of Wizardry Enhance - Scroll of Gigantic Storm
        (15, 30, 36, GameVersion.Season6Episode1), // Chain Drive Parchment - Increase Block Parchment
    ];

    /// <summary>
    /// Sets the versions (<c>IntroducedIn</c>) of the content of the game configuration.
    /// Only versions which are still <see cref="GameVersion.Unknown"/> are set, so that values
    /// which were set by the server administrator are kept.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void Apply(GameConfiguration gameConfiguration)
    {
        foreach (var map in gameConfiguration.Maps.Where(m => m.IntroducedIn == GameVersion.Unknown))
        {
            map.IntroducedIn = Maps.GetValueOrDefault((map.Number, map.Discriminator));
        }

        foreach (var characterClass in gameConfiguration.CharacterClasses.Where(c => c.IntroducedIn == GameVersion.Unknown))
        {
            characterClass.IntroducedIn = CharacterClasses.GetValueOrDefault(characterClass.Number);
        }

        foreach (var miniGame in gameConfiguration.MiniGameDefinitions.Where(m => m.IntroducedIn == GameVersion.Unknown))
        {
            miniGame.IntroducedIn = MiniGames.GetValueOrDefault((miniGame.Type, miniGame.GameLevel));
        }

        foreach (var monster in gameConfiguration.Monsters.Where(m => m.IntroducedIn == GameVersion.Unknown))
        {
            monster.IntroducedIn = Monsters
                .FirstOrDefault(range => range.From <= monster.Number && monster.Number <= range.To)
                .Version;
        }

        foreach (var item in gameConfiguration.Items.Where(i => i.IntroducedIn == GameVersion.Unknown))
        {
            item.IntroducedIn = Items
                .FirstOrDefault(range => range.Group == item.Group && range.From <= item.Number && item.Number <= range.To)
                .Version;
        }
    }
}
