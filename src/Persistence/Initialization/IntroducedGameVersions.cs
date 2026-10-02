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
///   <item>the content of the data initializations of the versions 0.75 and 0.95d,</item>
///   <item>the history of the original patch notes, compiled at https://github.com/Khdoop/mu-online-history,</item>
///   <item>and, for monsters, the maps on which they are spawned.</item>
/// </list>
/// Entries for which the version isn't known reliably are not listed, so they stay <see cref="GameVersion.Unknown"/>.
/// The keys are the numbers of the Season 6 data; the data of the older versions uses the same numbers.
/// The Magic Gladiator is not part of the 0.75 data, so it's listed as introduced in <see cref="GameVersion.Version095d"/>.
/// </remarks>
public static class IntroducedGameVersions
{
    /// <summary>
    /// Gets the versions of the maps by their number and discriminator.
    /// </summary>
    public static IReadOnlyDictionary<(short Number, int Discriminator), GameVersion> Maps { get; } = new Dictionary<(short Number, int Discriminator), GameVersion>
    {
        [(0, 0)] = GameVersion.Version075, // Lorencia
        [(1, 0)] = GameVersion.Version075, // Dungeon
        [(2, 0)] = GameVersion.Version075, // Devias
        [(3, 0)] = GameVersion.Version075, // Noria
        [(4, 0)] = GameVersion.Version075, // Lost Tower
        [(5, 0)] = GameVersion.Version075, // Exile
        [(6, 0)] = GameVersion.Version075, // Arena
        [(7, 0)] = GameVersion.Version075, // Atlans
        [(8, 0)] = GameVersion.Version095d, // Tarkan
        [(9, 1)] = GameVersion.Version095d, // Devil Square 1
        [(9, 2)] = GameVersion.Version095d, // Devil Square 2
        [(9, 3)] = GameVersion.Version095d, // Devil Square 3
        [(9, 4)] = GameVersion.Version095d, // Devil Square 4
        [(10, 0)] = GameVersion.Version095d, // Icarus
        [(11, 0)] = GameVersion.Version097d, // Blood Castle 1
        [(12, 0)] = GameVersion.Version097d, // Blood Castle 2
        [(13, 0)] = GameVersion.Version097d, // Blood Castle 3
        [(14, 0)] = GameVersion.Version097d, // Blood Castle 4
        [(15, 0)] = GameVersion.Version097d, // Blood Castle 5
        [(16, 0)] = GameVersion.Version097d, // Blood Castle 6
        [(17, 0)] = GameVersion.Version099, // Blood Castle 7
        [(18, 0)] = GameVersion.Version099, // Chaos Castle 1
        [(19, 0)] = GameVersion.Version099, // Chaos Castle 2
        [(20, 0)] = GameVersion.Version099, // Chaos Castle 3
        [(21, 0)] = GameVersion.Version099, // Chaos Castle 4
        [(22, 0)] = GameVersion.Version099, // Chaos Castle 5
        [(23, 0)] = GameVersion.Version099, // Chaos Castle 6
        [(24, 0)] = GameVersion.Version099, // Kalima 1
        [(25, 0)] = GameVersion.Version099, // Kalima 2
        [(26, 0)] = GameVersion.Version099, // Kalima 3
        [(27, 0)] = GameVersion.Version099, // Kalima 4
        [(28, 0)] = GameVersion.Version099, // Kalima 5
        [(29, 0)] = GameVersion.Version099, // Kalima 6
        [(30, 0)] = GameVersion.Version100, // Valley of Loren
        [(31, 0)] = GameVersion.Version100, // Land_of_Trials
        [(32, 5)] = GameVersion.Version100, // Devil Square 5
        [(32, 6)] = GameVersion.Version100, // Devil Square 6
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
        [0] = GameVersion.Version075, // Dark Wizard
        [2] = GameVersion.Version097d, // Soul Master
        [3] = GameVersion.Season2, // Grand Master
        [4] = GameVersion.Version075, // Dark Knight
        [6] = GameVersion.Version097d, // Blade Knight
        [7] = GameVersion.Season2, // Blade Master
        [8] = GameVersion.Version075, // Fairy Elf
        [10] = GameVersion.Version097d, // Muse Elf
        [11] = GameVersion.Season2, // High Elf
        [12] = GameVersion.Version095d, // Magic Gladiator
        [13] = GameVersion.Season2, // Duel Master
        [16] = GameVersion.Version099, // Dark Lord
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
        [(MiniGameType.DevilSquare, 1)] = GameVersion.Version095d,
        [(MiniGameType.DevilSquare, 2)] = GameVersion.Version095d,
        [(MiniGameType.DevilSquare, 3)] = GameVersion.Version095d,
        [(MiniGameType.DevilSquare, 4)] = GameVersion.Version095d,
        [(MiniGameType.DevilSquare, 5)] = GameVersion.Version100,
        [(MiniGameType.DevilSquare, 6)] = GameVersion.Version100,
        [(MiniGameType.DevilSquare, 7)] = GameVersion.Season3Episode1,
        [(MiniGameType.BloodCastle, 1)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 2)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 3)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 4)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 5)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 6)] = GameVersion.Version097d,
        [(MiniGameType.BloodCastle, 7)] = GameVersion.Version099,
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
        (0, 41, GameVersion.Version075), // Bull Fighter - Death Cow
        (43, 44, GameVersion.Version095d), // Golden Budge Dragon - Red Dragon
        (45, 52, GameVersion.Version075), // Bahamut - Silver Valkyrie
        (53, 54, GameVersion.Version095d), // Golden Titan - Golden Soldier
        (57, 67, GameVersion.Version095d), // Iron Wheel - Metal Balrog
        (69, 77, GameVersion.Version095d), // Alquamos - Dark Phoenix
        (79, 79, GameVersion.Version095d), // Golden Dragon
        (84, 99, GameVersion.Version097d), // Chief Skeleton Warrior 1 - Giant Ogre 3
        (100, 103, GameVersion.Version075), // Lance Trap - Meteorite Trap
        (105, 106, GameVersion.Season2), // Canon Trap - Laser Trap
        (111, 137, GameVersion.Version097d), // Red Skeleton Knight 3 - Destructive Ogre Archer
        (138, 149, GameVersion.Version099), // Chief Skeleton Warrior 7 - Necron 1
        (150, 150, GameVersion.Version075), // Bali
        (151, 151, GameVersion.Version095d), // Soldier
        (152, 157, GameVersion.Version099), // Gate to Kalima 1 of {0} - Gate to Kalima 6 of {0}
        (158, 158, GameVersion.Season1), // Gate to Kalima 7 of {0}
        (160, 197, GameVersion.Version099), // Schriker 1 - Illusion of Kundun 4
        (200, 200, GameVersion.Version075), // Soccerball
        (204, 209, GameVersion.Season1), // Wolf Status - Wolf Altar5
        (215, 224, GameVersion.Version100), // Shield - Guardsman
        (226, 226, GameVersion.Version099), // Pet Trainer
        (229, 229, GameVersion.Version097d), // Marlon
        (232, 233, GameVersion.Version097d), // Archangel - Messenger of Arch.
        (235, 237, GameVersion.Version095d), // Sevina the Priestess - Charon
        (238, 251, GameVersion.Version075), // Chaos Goblin - Hanzo The Blacksmith
        (253, 255, GameVersion.Version075), // Potion Girl Amy - Lumen the Barmaid
        (256, 257, GameVersion.Version100), // Lahap - Elf Soldier
        (259, 274, GameVersion.Version099), // Oracle Layla - Schriker 6
        (275, 275, GameVersion.Season1), // Illusion of Kundun 7
        (277, 278, GameVersion.Version100), // Castle Gate1 - Life Stone
        (283, 283, GameVersion.Version100), // Guardian Statue
        (285, 288, GameVersion.Version100), // Guardian - Canon Tower
        (290, 295, GameVersion.Version100), // Lizard Warrior - Erohim
        (304, 317, GameVersion.Season1), // Witch Queen - Soram
        (331, 337, GameVersion.Season1), // Aegis 7 - Schriker 7
        (338, 338, GameVersion.Version099), // Illusion of Kundun 6
        (350, 364, GameVersion.Season2), // Berserker - Maya
        (367, 368, GameVersion.Season2), // Gateway Machine - Elphis
        (369, 370, GameVersion.Season1), // Osbourne - Jerridon
        (376, 377, GameVersion.Version100), // Pamela the Supplier - Angela the Supplier
        (378, 378, GameVersion.Season2), // GameMaster
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
        (0, 0, 15, GameVersion.Version075), // Kris - Giant Sword
        (0, 16, 18, GameVersion.Version095d), // Sword of Destruction - Thunder Blade
        (0, 19, 19, GameVersion.Version097d), // Divine Sword of Archangel
        (0, 20, 21, GameVersion.Version099), // Knight Blade - Dark Reign Blade
        (0, 22, 23, GameVersion.Season1), // Bone Blade - Explosion Blade
        (0, 24, 25, GameVersion.Season2), // Daybreak - Sword Dancer
        (0, 26, 28, GameVersion.Season4Episode1), // Flamberge - Imperial Sword
        (0, 31, 31, GameVersion.Version097d), // Rune Blade
        (0, 32, 34, GameVersion.Season6Episode1), // Sacred Glove - Piercing Blade Glove
        (0, 35, 35, GameVersion.Season6Episode2), // Phoenix Soul Star
        (1, 0, 8, GameVersion.Version075), // Small Axe - Crescent Axe
        (2, 0, 6, GameVersion.Version075), // Mace - Chaos Dragon Axe
        (2, 7, 7, GameVersion.Version097d), // Elemental Mace
        (2, 8, 13, GameVersion.Version099), // Battle Scepter - Divine Scepter of Archangel
        (2, 14, 14, GameVersion.Season1), // Soleil Scepter
        (2, 15, 15, GameVersion.Season2), // Shining Scepter
        (2, 16, 17, GameVersion.Season4Episode1), // Frost Mace - Absolute Scepter
        (2, 18, 18, GameVersion.Season4Episode2), // Stryker Scepter
        (3, 0, 9, GameVersion.Version075), // Light Spear - Bill of Balrog
        (3, 10, 10, GameVersion.Version097d), // Dragon Spear
        (3, 11, 11, GameVersion.Season4Episode2), // Beuroba
        (4, 0, 15, GameVersion.Version075), // Short Bow - Arrows
        (4, 16, 16, GameVersion.Version095d), // Saint Crossbow
        (4, 17, 19, GameVersion.Version097d), // Celestial Bow - Great Reign Crossbow
        (4, 20, 20, GameVersion.Version099), // Arrow Viper Bow
        (4, 21, 21, GameVersion.Season1), // Sylph Wind Bow
        (4, 22, 22, GameVersion.Season2), // Albatross Bow
        (4, 23, 23, GameVersion.Season4Episode1), // Stinger Bow
        (4, 24, 24, GameVersion.Season4Episode2), // Air Lyn Bow
        (5, 0, 7, GameVersion.Version075), // Skull Staff - Chaos Lightning Staff
        (5, 8, 8, GameVersion.Version095d), // Staff of Destruction
        (5, 9, 10, GameVersion.Version097d), // Dragon Soul Staff - Divine Staff of Archangel
        (5, 11, 11, GameVersion.Version099), // Staff of Kundun
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
        (6, 0, 14, GameVersion.Version075), // Small Shield - Legendary Shield
        (6, 15, 16, GameVersion.Version097d), // Grand Soul Shield - Elemental Shield
        (6, 17, 20, GameVersion.Season4Episode1), // Crimson Glory - Guardian Shield
        (6, 21, 21, GameVersion.Version099), // Cross Shield
        (7, 0, 14, GameVersion.Version075), // Bronze Helm - Guardian Helm
        (7, 16, 16, GameVersion.Version095d), // Black Dragon Helm
        (7, 17, 19, GameVersion.Version097d), // Dark Phoenix Helm - Divine Helm
        (7, 21, 22, GameVersion.Version099), // Great Dragon Helm - Dark Soul Helm
        (7, 24, 28, GameVersion.Version099), // Red Spirit Helm - Dark Master Mask
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
        (8, 0, 15, GameVersion.Version075), // Bronze Armor - Storm Crow Armor
        (8, 16, 16, GameVersion.Version095d), // Black Dragon Armor
        (8, 17, 20, GameVersion.Version097d), // Dark Phoenix Armor - Thunder Hawk Armor
        (8, 21, 28, GameVersion.Version099), // Great Dragon Armor - Dark Master Armor
        (8, 29, 33, GameVersion.Season1), // Dragon Knight Armor - Sunlight Armor
        (8, 34, 38, GameVersion.Season2), // Ashcrow Armor - Glorious Armor
        (8, 39, 41, GameVersion.Season3Episode1), // Mistery Armor - Ancient Armor
        (8, 42, 43, GameVersion.Season4Episode1), // Black Rose Armor - Aura Armor
        (8, 44, 44, GameVersion.Season6Episode2), // Lilium Armor
        (8, 45, 52, GameVersion.Season4Episode1), // Titan Armor - Hades Armor
        (8, 59, 61, GameVersion.Season6Episode1), // Sacred Armor - Piercing Armor
        (8, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Armor
        (9, 0, 15, GameVersion.Version075), // Bronze Pants - Storm Crow Pants
        (9, 16, 16, GameVersion.Version095d), // Black Dragon Pants
        (9, 17, 20, GameVersion.Version097d), // Dark Phoenix Pants - Thunder Hawk Pants
        (9, 21, 28, GameVersion.Version099), // Great Dragon Pants - Dark Master Pants
        (9, 29, 33, GameVersion.Season1), // Dragon Knight Pants - Sunlight Pants
        (9, 34, 38, GameVersion.Season2), // Ashcrow Pants - Glorious Pants
        (9, 39, 41, GameVersion.Season3Episode1), // Mistery Pants - Ancient Pants
        (9, 42, 43, GameVersion.Season4Episode1), // Black Rose Pants - Aura Pants
        (9, 44, 44, GameVersion.Season6Episode2), // Lilium Pants
        (9, 45, 52, GameVersion.Season4Episode1), // Titan Pants - Hades Pants
        (9, 59, 61, GameVersion.Season6Episode1), // Sacred Pants - Piercing Pants
        (9, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Pants
        (10, 0, 15, GameVersion.Version075), // Bronze Gloves - Storm Crow Gloves
        (10, 16, 16, GameVersion.Version095d), // Black Dragon Gloves
        (10, 17, 20, GameVersion.Version097d), // Dark Phoenix Gloves - Thunder Hawk Gloves
        (10, 21, 28, GameVersion.Version099), // Great Dragon Gloves - Dark Master Gloves
        (10, 29, 33, GameVersion.Season1), // Dragon Knight Gloves - Sunlight Gloves
        (10, 34, 38, GameVersion.Season2), // Ashcrow Gloves - Glorious Gloves
        (10, 39, 41, GameVersion.Season3Episode1), // Mistery Gloves - Ancient Gloves
        (10, 42, 43, GameVersion.Season4Episode1), // Black Rose Gloves - Aura Gloves
        (10, 44, 44, GameVersion.Season6Episode2), // Lilium Gloves
        (10, 45, 52, GameVersion.Season4Episode1), // Titan Gloves - Hades Gloves
        (11, 0, 15, GameVersion.Version075), // Bronze Boots - Storm Crow Boots
        (11, 16, 16, GameVersion.Version095d), // Black Dragon Boots
        (11, 17, 20, GameVersion.Version097d), // Dark Phoenix Boots - Thunder Hawk Boots
        (11, 21, 28, GameVersion.Version099), // Great Dragon Boots - Dark Master Boots
        (11, 29, 33, GameVersion.Season1), // Dragon Knight Boots - Sunlight Boots
        (11, 34, 38, GameVersion.Season2), // Ashcrow Boots - Glorious Boots
        (11, 39, 41, GameVersion.Season3Episode1), // Mistery Boots - Ancient Boots
        (11, 42, 43, GameVersion.Season4Episode1), // Black Rose Boots - Aura Boots
        (11, 44, 44, GameVersion.Season6Episode2), // Lilium Boots
        (11, 45, 52, GameVersion.Season4Episode1), // Titan Boots - Hades Boots
        (11, 59, 61, GameVersion.Season6Episode1), // Sacred Boots - Piercing Boots
        (11, 73, 73, GameVersion.Season6Episode2), // Phoenix Soul Boots
        (12, 0, 2, GameVersion.Version075), // Wings of Elf - Wings of Satan
        (12, 3, 6, GameVersion.Version097d), // Wings of Spirits - Wings of Darkness
        (12, 7, 7, GameVersion.Version095d), // Orb of Twisting Slash
        (12, 8, 11, GameVersion.Version075), // Orb of Healing - Orb of Summoning
        (12, 12, 14, GameVersion.Version097d), // Orb of Rageful Blow - Orb of Greater Fortitude
        (12, 15, 15, GameVersion.Version075), // Jewel of Chaos
        (12, 16, 19, GameVersion.Version097d), // Orb of Fire Slash - Orb of Death Stab
        (12, 21, 24, GameVersion.Version099), // Scroll of FireBurst - Scroll of Electric Spark
        (12, 30, 31, GameVersion.Version100), // Packed Jewel of Bless - Packed Jewel of Soul
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
        (13, 3, 3, GameVersion.Version095d), // Horn of Dinorant
        (13, 4, 5, GameVersion.Version099), // Dark Horse - Dark Raven
        (13, 8, 10, GameVersion.Version075), // Ring of Ice - Transformation Ring
        (13, 11, 11, GameVersion.Version100), // Life Stone
        (13, 12, 13, GameVersion.Version075), // Pendant of Lighting - Pendant of Fire
        (13, 14, 14, GameVersion.Version099), // Loch's Feather
        (13, 15, 19, GameVersion.Version097d), // Fruits - Weapon of Archangel
        (13, 29, 29, GameVersion.Version100), // Armor of Guardsman
        (13, 30, 31, GameVersion.Version099), // Cape of Lord - Spirit
        (13, 32, 37, GameVersion.Season1), // Splinter of Armor - Horn of Fenrir
        (13, 38, 38, GameVersion.Season2), // Moonstone Pendant
        (13, 40, 40, GameVersion.Season2), // Jack O'lantern Transformation Ring
        (13, 42, 42, GameVersion.Season2), // Game Master Transformation Ring
        (13, 49, 53, GameVersion.Season2), // Old Scroll - Feather of Condor
        (13, 64, 65, GameVersion.Season5Episode2), // Demon - Spirit of Guardian
        (13, 68, 68, GameVersion.Season4Episode1), // Snowman Transformation Ring
        (13, 125, 125, GameVersion.Season5Episode1), // Doppelganger Free Ticket
        (14, 0, 6, GameVersion.Version075), // Apple - Large Mana Potion
        (14, 7, 7, GameVersion.Version100), // Potion of Bless,Potion of Soul
        (14, 8, 10, GameVersion.Version075), // Antidote - Town Portal Scroll
        (14, 11, 11, GameVersion.Version095d), // Box of Luck
        (14, 13, 14, GameVersion.Version075), // Jewel of Bless - Jewel of Soul
        (14, 16, 19, GameVersion.Version095d), // Jewel of Life - Devil's Invitation
        (14, 21, 22, GameVersion.Version095d), // Rena - Jewel of Creation
        (14, 23, 26, GameVersion.Version097d), // Scroll of Emperor,Ring of Honor - Soul Shard of Wizard
        (14, 28, 29, GameVersion.Version099), // Lost Map - Symbol of Kundun
        (14, 31, 31, GameVersion.Version100), // Jewel of Guardian
        (14, 35, 40, GameVersion.Season1), // Small Shield Potion - Large Complex Potion
        (14, 41, 50, GameVersion.Season2), // Gemstone - Jack O'Lantern Drink
        (14, 52, 52, GameVersion.Season2), // GM Gift
        (14, 63, 63, GameVersion.Version095d), // Firecracker
        (14, 65, 67, GameVersion.Season2), // Flame of Death Beam Knight - Feather of Dark Phoenix
        (14, 68, 68, GameVersion.Season3Episode1), // Eye of Abyssal
        (14, 101, 111, GameVersion.Season5Episode1), // Suspicious Scrap of Paper - Mirror of Dimensions
        (15, 0, 11, GameVersion.Version075), // Scroll of Poison - Scroll of Aqua Beam
        (15, 12, 13, GameVersion.Version095d), // Scroll of Cometfall - Scroll of Inferno
        (15, 14, 18, GameVersion.Version097d), // Scroll of Teleport Ally - Scroll of Nova
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
