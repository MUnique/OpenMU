// <copyright file="MonsterResistancesUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Converts the elemental resistances of the monsters to whole numbers and corrects the ones which were assigned to the
/// wrong elements.
/// </summary>
/// <remarks>
/// The resistance check treats a resistance as a whole number from 0 to 255, like the original server: an effect is
/// resisted with a chance of <c>r / (r + 1)</c>, and 255 means immunity. The resistances of the characters were converted
/// to this scale, but the monsters kept their values divided by 255, so they resisted elemental effects in only about
/// 1 to 2 % of the hits, whatever their values.
/// Additionally, the import of the monster data had assigned the columns to the wrong elements: ice and poison were
/// swapped, lightning was stored as water and the monster skill as wind. Monsters don't have a water or wind resistance
/// in the original data.
/// The update first converts every monster which still has a resistance below 1 (a value of the former scale) by
/// multiplying all its resistances by 255, so customized values keep their meaning. Then it sets the values on which the
/// Season 6 server data of several sources agree, but only for monsters whose resistances are still the ones of the
/// former initialization data, so customized resistances are kept.
/// </remarks>
public abstract class MonsterResistancesUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Monster elemental resistances";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Converts the elemental resistances of the monsters to whole numbers from 0 to 255, which the resistance check expects; until now, monsters resisted ice, poison and lightning effects in only about 1 to 2 % of the hits. Corrects the resistances which the import of the monster data assigned to the wrong elements (ice and poison swapped, lightning stored as water) and sets the Season 6 values. Customized resistances are converted to the new scale, but not changed otherwise.";

    /// <summary>
    /// The resistance attributes which a monster can have, including the ones which the import set by mistake.
    /// </summary>
    private static readonly AttributeDefinition[] ResistanceAttributes =
    [
        Stats.IceResistance,
        Stats.PoisonResistance,
        Stats.LightningResistance,
        Stats.FireResistance,
        Stats.WaterResistance,
        Stats.WindResistance,
        Stats.EarthResistance,
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 16, 22, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the corrected resistances of the monsters, in whole numbers.
    /// </summary>
    internal static IReadOnlyList<MonsterResistancesCorrection> Corrections { get; } =
    [
        new(5, (0, 0, 0, 3, 0, 0), (1, 1, 1, 3)), // Hell Hound
        new(8, (0, 6, 0, 0, 0, 0), (6, 2, 2, 2)), // Poison Bull Fighter
        new(9, (0, 0, 0, 0, 3, 0), (2, 2, 3, 2)), // Thunder Lich
        new(10, (3, 3, 0, 3, 3, 0), (3, 3, 3, 5)), // Dark Knight
        new(11, (2, 2, 0, 2, 2, 0), (2, 2, 2, 2)), // Ghost
        new(12, (0, 0, 0, 0, 0, 0), (2, 0, 0, 0)), // Larva
        new(13, (0, 3, 0, 0, 0, 0), (3, 1, 1, 1)), // Hell Spider
        new(15, (0, 0, 0, 0, 0, 0), (1, 1, 1, 1)), // Skeleton Archer
        new(16, (0, 0, 0, 0, 0, 0), (1, 1, 1, 1)), // Chief Skeleton Warrior
        new(17, (0, 2, 0, 0, 0, 0), (2, 0, 0, 0)), // Cyclops
        new(18, (6, 6, 0, 6, 6, 0), (6, 6, 6, 6)), // Gorgon
        new(19, (3, 0, 0, 0, 0, 0), (0, 3, 0, 0)), // Yeti
        new(20, (4, 1, 0, 1, 1, 0), (1, 4, 1, 1)), // Elite Yeti
        new(22, (3, 0, 0, 0, 0, 0), (0, 3, 0, 0)), // Ice Monster
        new(23, (3, 0, 0, 0, 0, 0), (0, 3, 0, 0)), // Hommerd
        new(24, (2, 0, 0, 0, 0, 0), (0, 2, 0, 0)), // Worm
        new(25, (5, 4, 0, 4, 4, 0), (4, 5, 4, 4)), // Ice Queen
        new(34, (5, 5, 0, 7, 7, 0), (5, 5, 7, 7)), // Cursed Wizard
        new(35, (6, 6, 0, 8, 6, 0), (6, 6, 6, 8)), // Death Gorgon
        new(36, (3, 3, 0, 5, 3, 0), (3, 3, 3, 5)), // Shadow
        new(37, (5, 5, 0, 7, 5, 0), (5, 5, 5, 7)), // Devil
        new(38, (10, 10, 0, 15, 10, 0), (10, 10, 10, 15)), // Balrog
        new(39, (4, 6, 0, 6, 4, 0), (6, 4, 4, 6)), // Poison Shadow
        new(40, (6, 6, 0, 7, 6, 0), (6, 6, 6, 7)), // Death Knight
        new(41, (5, 5, 0, 7, 5, 0), (5, 5, 5, 7)), // Death Cow
        new(43, (2, 2, 0, 2, 2, 0), (2, 2, 2, 2)), // Golden Budge Dragon
        new(44, (7, 9, 0, 9, 9, 0), (9, 7, 9, 9)), // Dragon
        new(45, (1, 1, 0, 1, 1, 0), (1, 1, 1, 1)), // Bahamut
        new(46, (2, 2, 0, 2, 3, 0), (2, 2, 3, 2)), // Vepar
        new(47, (2, 6, 0, 2, 2, 0), (6, 2, 2, 2)), // Valkyrie
        new(48, (7, 7, 0, 7, 7, 0), (7, 7, 7, 7)), // Lizard King
        new(49, (12, 12, 0, 12, 12, 0), (12, 12, 12, 12)), // Hydra
        new(50, (12, 12, 0, 12, 12, 0), (12, 12, 12, 12)), // Sea Worm
        new(51, (6, 6, 0, 6, 6, 0), (6, 6, 6, 6)), // Great Bahamut
        new(52, (7, 7, 0, 7, 7, 0), (7, 7, 7, 7)), // Silver Valkyrie
        new(53, (5, 5, 0, 5, 5, 0), (5, 5, 5, 5)), // Golden Titan
        new(54, (3, 3, 0, 3, 3, 0), (3, 3, 3, 3)), // Golden Soldier
        new(57, (9, 9, 0, 9, 9, 0), (9, 9, 9, 9)), // Iron Wheel
        new(58, (9, 9, 0, 9, 9, 0), (9, 9, 9, 9)), // Tantalos
        new(59, (13, 13, 0, 15, 13, 0), (13, 13, 13, 15)), // Zaikan
        new(60, (8, 8, 0, 8, 8, 0), (8, 8, 8, 8)), // Bloody wolf
        new(61, (10, 10, 0, 10, 10, 0), (10, 10, 10, 10)), // Beam Knight
        new(62, (8, 8, 0, 8, 8, 0), (8, 8, 8, 8)), // Mutant
        new(63, (13, 13, 0, 17, 13, 0), (13, 13, 13, 17)), // Death Beam Knight
        new(64, (7, 7, 0, 7, 7, 0), (7, 7, 7, 7)), // Ogre archer
        new(65, (8, 8, 0, 8, 8, 0), (8, 8, 8, 8)), // Elite Ogre
        new(66, (17, 17, 0, 17, 17, 0), (17, 17, 17, 17)), // Cursed king
        new(67, (9, 9, 0, 9, 9, 0), (9, 9, 9, 9)), // Metal Balrog
        new(69, (9, 9, 0, 9, 11, 0), (9, 9, 11, 9)), // Alquamos
        new(70, (11, 9, 0, 9, 12, 0), (9, 11, 12, 9)), // Queen Rainier
        new(71, (9, 9, 0, 9, 12, 0), (9, 9, 12, 9)), // Mega Crust
        new(72, (14, 14, 0, 14, 16, 0), (14, 14, 16, 14)), // Phantom Knight
        new(73, (12, 12, 0, 12, 13, 0), (12, 12, 13, 12)), // Drakan
        new(74, (13, 13, 0, 13, 14, 0), (13, 13, 14, 13)), // Alpha Crust
        new(75, (15, 15, 0, 18, 17, 0), (15, 15, 17, 18)), // Great Drakan
        new(76, (30, 30, 0, 30, 35, 0), (30, 30, 35, 30)), // Phoenix Darkness shield
        new(77, (30, 30, 0, 30, 35, 0), (30, 30, 35, 30)), // Phoenix of Darkness
        new(78, (2, 2, 0, 2, 2, 0), (2, 2, 2, 2)), // Golden Goblin
        new(79, (7, 9, 0, 9, 9, 0), (9, 7, 9, 9)), // Golden Derkon
        new(80, (7, 9, 0, 9, 9, 0), (9, 7, 9, 9)), // Golden Lizard King
        new(81, (0, 0, 0, 0, 3, 0), (0, 0, 3, 0)), // Golden Vepar
        new(82, (7, 9, 0, 9, 9, 0), (9, 7, 9, 9)), // Golden Tantalos
        new(83, (7, 9, 0, 9, 9, 0), (9, 7, 9, 9)), // Golden Wheel
        new(84, (2, 2, 2, 2, 0, 0), (0, 0, 0, 0)), // Chief Skeleton Warrior1
        new(85, (2, 2, 2, 2, 0, 0), (0, 0, 0, 0)), // Chief Skeleton Archer1
        new(86, (2, 2, 2, 2, 0, 0), (0, 0, 0, 0)), // Dark Skull Soldier1
        new(87, (2, 2, 2, 2, 0, 0), (0, 0, 0, 0)), // Giant Ogre1
        new(88, (2, 2, 2, 2, 0, 0), (0, 0, 0, 0)), // Red Skeleton Knight1
        new(89, (5, 5, 5, 5, 0, 0), (3, 3, 3, 3)), // Magic Skeleton1
        new(90, (4, 4, 4, 4, 0, 0), (1, 1, 1, 1)), // Chief Skeleton Warrior2
        new(91, (4, 4, 4, 4, 0, 0), (1, 1, 1, 1)), // Chief Skeleton Archer2
        new(92, (4, 4, 4, 4, 0, 0), (1, 1, 1, 1)), // Dark Skull Soldier2
        new(93, (4, 4, 4, 4, 0, 0), (1, 1, 1, 1)), // Giant Ogre2
        new(94, (4, 4, 4, 4, 0, 0), (1, 1, 1, 1)), // Red Skeleton Knight2
        new(95, (7, 7, 7, 7, 0, 0), (4, 4, 4, 4)), // Magic Skeleton2
        new(96, (5, 5, 5, 5, 0, 0), (2, 2, 2, 2)), // Chief Skeleton Warrior3
        new(97, (5, 5, 5, 5, 0, 0), (2, 2, 2, 2)), // Chief Skeleton Archer3
        new(98, (5, 5, 5, 5, 0, 0), (2, 2, 2, 2)), // Dark Skull Soldier3
        new(99, (5, 5, 5, 5, 0, 0), (2, 2, 2, 2)), // Giant Ogre3
        new(111, (5, 5, 5, 5, 0, 0), (2, 2, 2, 2)), // Red Skeleton Knight3
        new(112, (8, 8, 8, 8, 0, 0), (5, 5, 5, 5)), // Magic Skeleton3
        new(113, (6, 6, 6, 6, 0, 0), (4, 4, 4, 4)), // Chief Skeleton Warrior4
        new(114, (6, 6, 6, 6, 0, 0), (4, 4, 4, 4)), // Chief Skeleton Archer4
        new(115, (6, 6, 6, 6, 0, 0), (4, 4, 4, 4)), // Dark Skull Soldier4
        new(116, (6, 6, 6, 6, 0, 0), (4, 4, 4, 4)), // Giant Ogre4
        new(117, (6, 6, 6, 6, 0, 0), (4, 4, 4, 4)), // Red Skeleton Knight4
        new(118, (9, 9, 9, 9, 0, 0), (7, 7, 7, 7)), // Magic Skeleton4
        new(119, (7, 7, 7, 7, 0, 0), (5, 5, 5, 5)), // Chief Skeleton Warrior5
        new(120, (7, 7, 7, 7, 0, 0), (5, 5, 5, 5)), // Chief Skeleton Archer5
        new(121, (7, 7, 7, 7, 0, 0), (5, 5, 5, 5)), // Dark Skull Soldier5
        new(122, (7, 7, 7, 7, 0, 0), (5, 5, 5, 5)), // Giant Ogre5
        new(123, (7, 7, 7, 7, 0, 0), (5, 5, 5, 5)), // Red Skeleton Knight5
        new(124, (10, 10, 10, 10, 0, 0), (8, 8, 8, 8)), // Magic Skeleton5
        new(131, (0, 0, 0, 0, 0, 0), (100, 100, 100, 100)), // Castle Gate
        new(132, (0, 0, 0, 0, 0, 0), (100, 100, 100, 100)), // Statue of Saint
        new(133, (0, 0, 0, 0, 0, 0), (100, 100, 100, 100)), // Statue of Saint
        new(134, (0, 0, 0, 0, 0, 0), (100, 100, 100, 100)), // Statue of Saint
        new(150, (6, 6, 0, 6, 6, 0), (6, 6, 6, 6)), // Bali
        new(151, (6, 6, 0, 6, 6, 0), (6, 6, 6, 6)), // Soldier
        new(162, (1, 1, 1, 1, 0, 0), (0, 0, 0, 0)), // Chaos Castle 1
        new(163, (1, 1, 1, 1, 0, 0), (0, 0, 0, 0)), // Chaos Castle 1
        new(164, (2, 2, 2, 2, 0, 0), (1, 1, 1, 1)), // Chaos Castle 2
        new(165, (2, 2, 2, 2, 0, 0), (1, 1, 1, 1)), // Chaos Castle 2
        new(166, (3, 3, 3, 3, 0, 0), (2, 2, 2, 2)), // Chaos Castle 3
        new(167, (3, 3, 3, 3, 0, 0), (2, 2, 2, 2)), // Chaos Castle 3
        new(275, (60, 60, 60, 60, 0, 0), (100, 254, 100, 100)), // Kundun7
        new(304, (22, 22, 0, 22, 0, 0), (22, 22, 22, 22)), // Witch Queen
        new(305, (21, 21, 0, 21, 0, 0), (21, 21, 21, 21)), // Blue Golem
        new(306, (20, 20, 0, 20, 0, 0), (20, 20, 20, 20)), // Death Rider
        new(307, (19, 19, 0, 19, 0, 0), (19, 19, 19, 19)), // Forest Orc
        new(308, (18, 18, 0, 18, 0, 0), (18, 18, 18, 18)), // Death Tree
        new(309, (50, 50, 0, 50, 0, 0), (50, 50, 50, 50)), // Hell Maine
        new(310, (15, 15, 0, 15, 0, 0), (15, 15, 15, 15)), // Hammer Scout
        new(311, (17, 17, 0, 17, 0, 0), (17, 17, 17, 17)), // Lance Scout
        new(312, (19, 19, 0, 19, 0, 0), (19, 19, 19, 19)), // Bow Scout
        new(313, (21, 21, 0, 21, 0, 0), (21, 21, 21, 21)), // Werewolf
        new(314, (23, 23, 0, 23, 0, 0), (23, 23, 23, 23)), // Scout(Hero)
        new(315, (25, 25, 0, 25, 0, 0), (25, 25, 25, 25)), // Werewolf(Hero)
        new(316, (27, 27, 0, 27, 0, 0), (27, 27, 27, 27)), // Balram
        new(317, (29, 29, 0, 29, 0, 0), (29, 29, 29, 29)), // Soram
        new(361, (50, 50, 50, 50, 0, 0), (150, 255, 150, 150)), // Nightmare
        new(362, (40, 40, 40, 40, 0, 0), (100, 100, 100, 100)), // Maya Hand
        new(363, (40, 40, 40, 40, 0, 0), (100, 100, 100, 100)), // Maya Hand
        new(364, (50, 50, 50, 50, 0, 0), (100, 100, 100, 100)), // Maya
        new(422, (4, 4, 0, 4, 4, 0), (0, 2, 0, 0)), // Cursed Lich
        new(424, (0, 0, 0, 0, 0, 0), (0, 3, 0, 0)), // Grizzly
        new(425, (0, 0, 0, 0, 0, 0), (3, 3, 3, 5)), // Captain Grizzly
        new(427, (6, 6, 6, 6, 0, 0), (7, 7, 7, 7)), // Chaos Castle7
        new(434, (25, 25, 0, 25, 25, 0), (25, 25, 25, 25)), // Gigantis
        new(435, (50, 50, 0, 50, 50, 0), (50, 50, 50, 50)), // Berserker
        new(436, (23, 23, 0, 23, 23, 0), (23, 23, 23, 23)), // Balram (Trainee Soldier)
        new(437, (25, 25, 0, 25, 25, 0), (25, 25, 25, 25)), // Soram (Trainee Soldier)
        new(438, (29, 29, 0, 29, 29, 0), (29, 29, 29, 29)), // Persona
        new(439, (27, 27, 0, 27, 27, 0), (27, 27, 27, 27)), // Dreadfear
        new(440, (23, 23, 0, 23, 23, 23), (23, 23, 23, 23)), // Dark Elf
        new(459, (254, 254, 0, 150, 150, 0), (254, 254, 150, 150)), // Selupan
        new(460, (50, 50, 0, 50, 50, 0), (50, 50, 50, 50)), // Spider Eggs1
        new(461, (50, 50, 0, 50, 50, 0), (50, 50, 50, 50)), // Spider Eggs2
        new(462, (50, 50, 0, 50, 50, 0), (50, 50, 50, 50)), // Spider Eggs3
        new(480, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Zombie Fighter
        new(481, (200, 200, 0, 200, 0, 0), (200, 200, 200, 250)), // Zombie Fighter
        new(482, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Resurrected Gladiator
        new(483, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Resurrected Gladiator
        new(484, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Ash Slaughterer
        new(485, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Ash Slaughterer
        new(486, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Blood Assassin
        new(487, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Cruel Blood Assassin
        new(488, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Cruel Blood Assassin
        new(489, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Burning Lava Giant
        new(490, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Ruthless Lava Giant
        new(491, (200, 200, 0, 250, 0, 0), (200, 200, 200, 250)), // Ruthless Lava Giant
        new(504, (29, 29, 0, 29, 29, 0), (29, 29, 29, 29)), // Gaion Kharein
        new(505, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Jerint
        new(506, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Raymond
        new(507, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Erkanne
        new(508, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Destler
        new(509, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Vermont
        new(510, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Kato
        new(511, (255, 150, 0, 150, 150, 0), (150, 255, 150, 150)), // Galia
        new(512, (23, 23, 0, 23, 23, 0), (23, 23, 23, 23)), // Quartermaster
        new(513, (29, 29, 0, 29, 29, 0), (29, 29, 29, 29)), // Combat Instructor
        new(514, (23, 23, 0, 23, 23, 23), (23, 23, 23, 23)), // Knight Commander
        new(515, (25, 25, 0, 25, 25, 0), (25, 25, 25, 25)), // Grand Wizard
        new(516, (29, 29, 0, 29, 29, 0), (29, 29, 29, 29)), // Master Assassin
        new(517, (60, 60, 0, 60, 60, 0), (60, 60, 60, 60)), // Cavalry Captain
        new(518, (29, 29, 0, 29, 29, 0), (29, 29, 29, 29)), // Shield Bearer
        new(519, (50, 50, 0, 50, 50, 0), (50, 50, 50, 50)), // Medic
        new(520, (25, 25, 0, 25, 25, 0), (25, 25, 25, 25)), // Knights
        new(521, (26, 26, 0, 26, 26, 0), (26, 26, 26, 26)), // Bodyguard
        new(523, (5, 5, 0, 7, 5, 0), (5, 5, 5, 7)), // Trap
        new(549, (20, 20, 0, 150, 0, 0), (20, 20, 20, 150)), // Bloody Orc
        new(550, (20, 20, 0, 150, 0, 0), (20, 20, 20, 150)), // Bloody Death Rider
        new(551, (20, 20, 0, 150, 0, 0), (20, 20, 20, 150)), // Bloody Golem
        new(552, (20, 20, 0, 150, 0, 0), (20, 20, 20, 150)), // Bloody Witch Queen
        new(569, (100, 254, 0, 200, 0, 0), (100, 254, 100, 200)), // Venomous Chain Scorpion
        new(570, (100, 254, 0, 200, 0, 0), (100, 254, 100, 200)), // Bone Scorpion
        new(571, (100, 100, 0, 240, 0, 0), (100, 100, 100, 240)), // Orcus
        new(573, (150, 254, 0, 150, 0, 0), (150, 254, 100, 150)), // Crypta
        new(574, (150, 254, 0, 150, 0, 0), (150, 254, 100, 150)), // Crypos
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var monster in gameConfiguration.Monsters)
        {
            ConvertToWholeNumbers(monster);
        }

        foreach (var correction in Corrections)
        {
            correction.ApplyTo(context, gameConfiguration);
        }

        return ValueTask.CompletedTask;
    }

    private static void ConvertToWholeNumbers(MonsterDefinition monster)
    {
        var resistances = monster.Attributes
            .Where(a => ResistanceAttributes.Any(r => r.Id == a.AttributeDefinition?.Id))
            .ToList();

        // Only the former scale has values with a fraction. Whole numbers, including 1, were already converted,
        // so checking for fractions keeps the update idempotent.
        if (!resistances.Any(a => a.Value != MathF.Round(a.Value)))
        {
            return;
        }

        foreach (var resistance in resistances)
        {
            resistance.Value = MathF.Round(resistance.Value * 255);
        }
    }
}
