// <copyright file="MonsterStatsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Sets the stats of the monsters to the Season 6 data, which all versions use.
/// </summary>
/// <remarks>
/// The values are the ones on which the Season 6 server data of several sources agree. They correct wrong levels,
/// health, damage, defense, attack and defense rates, ranges, delays and respawn times of field monsters, of the
/// invasion monsters, of the guards, of Chaos Castle 4 and 7 and of Devil Square 7; some of them were typos,
/// e.g. ten times the health.
/// The older versions use the same values, because no better data of these versions is known and it keeps the monster
/// levels consistent across the versions. Monsters which the game configuration doesn't have are skipped.
/// The monsters of Blood Castle are corrected by <see cref="FixBloodCastleMonsterAttributesUpdatePlugIn"/>.
/// </remarks>
public abstract class MonsterStatsUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Season 6 monster stats";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Sets level, health, damage, defense, attack and defense rate, ranges, delays and respawn times of the monsters to the Season 6 data, which this version uses as well. It corrects field monsters, invasion monsters and guards and, in Season 6, Chaos Castle 4 and 7 and Devil Square 7. Customized values of these stats are overwritten.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the corrected stats of the monsters.
    /// </summary>
    internal static IReadOnlyList<MonsterStatsCorrection> Corrections { get; } =
    [
        new(0) { RespawnDelaySeconds = 10 }, // Bull Fighter
        new(2) { RespawnDelaySeconds = 10 }, // Budge Dragon
        new(18) { RespawnDelaySeconds = 150 }, // Gorgon
        new(19) { RespawnDelaySeconds = 10 }, // Yeti
        new(20) { RespawnDelaySeconds = 10 }, // Elite Yeti
        new(43) { MaximumHealth = 2200 }, // Golden Budge Dragon
        new(44) { Level = 60, AttackRate = 300 }, // Red Dragon
        new(53) { MaximumHealth = 8000, RespawnDelaySeconds = 20 }, // Golden Titan
        new(54) { MaximumHealth = 3500, RespawnDelaySeconds = 20 }, // Golden Soldier
        new(71) { RespawnDelaySeconds = 10 }, // Mega Crust
        new(72) { RespawnDelaySeconds = 30 }, // Phantom Knight
        new(74) { RespawnDelaySeconds = 10 }, // Alpha Crust
        new(75) { RespawnDelaySeconds = 30 }, // Great Drakan
        new(77) { RespawnDelaySeconds = 150 }, // Dark Phoenix
        new(78) { MaximumHealth = 3100, DefenseRate = 30 }, // Golden Goblin
        new(79) { MaximumHealth = 23000, RespawnDelaySeconds = 5 }, // Golden Dragon
        new(80) { RespawnDelaySeconds = 15 }, // Golden Lizard King
        new(81) { MaximumHealth = 6300, RespawnDelaySeconds = 8 }, // Golden Vepar
        new(82) { MaximumHealth = 30000, RespawnDelaySeconds = 70 }, // Golden Tantallos
        new(83) { RespawnDelaySeconds = 70 }, // Golden Wheel
        new(135) { AttackDelayMilliseconds = 2200 }, // White Wizard
        new(136) { AttackDelayMilliseconds = 1500 }, // Destructive Ogre Soldier
        new(137) { AttackDelayMilliseconds = 1800 }, // Destructive Ogre Archer
        new(144) { MoveRange = 3 }, // Death Angel 1
        new(146) { AttackRange = 6 }, // Blood Soldier 1
        new(168) { MaximumHealth = 6500 }, // Chaos Castle 7
        new(184) { DefenseRate = 60 }, // Blood Soldier 3
        new(196) { DefenseRate = 215 }, // Schriker 4
        new(197) { AttackRate = 780 }, // Illusion Of Kundun 4
        new(247) { MoveRange = 5, AttackRange = 2, ViewRange = 2, RespawnDelaySeconds = 10 }, // Crossbow Guard
        new(249) { MoveRange = 5, AttackRange = 5, ViewRange = 5, RespawnDelaySeconds = 10 }, // Berdysh Guard
        new(261) { Defense = 430 }, // Death Centurion 5
        new(265) { AttackRate = 645 }, // Necron 5
        new(275) { Level = 147, MaximumHealth = 5000000, MinimumPhysicalDamage = 2000, MaximumPhysicalDamage = 2500, Defense = 1500, AttackRate = 2000, DefenseRate = 1000, AttackRange = 15, ViewRange = 10, AttackDelayMilliseconds = 2000, RespawnDelaySeconds = 43200 }, // Illusion Of Kundun 7
        new(294) { MoveDelayMilliseconds = 400, AttackDelayMilliseconds = 1400 }, // Axe Warrior
        new(355) { AttackRange = 4, ViewRange = 5 }, // Kentauros
        new(360) { DefenseRate = 905 }, // Dreadfear
        new(409) { AttackDelayMilliseconds = 1500 }, // Balram Trainee Soldier
        new(410) { Level = 118, AttackDelayMilliseconds = 1500 }, // Death Spirit Trainee Soldier
        new(411) { AttackDelayMilliseconds = 1500 }, // Soram Trainee Soldier
        new(418) { AttackRate = 18, ViewRange = 4, AttackDelayMilliseconds = 2000 }, // Strange Rabbit
        new(419) { Level = 19, MaximumHealth = 520, MinimumPhysicalDamage = 68, MaximumPhysicalDamage = 72, Defense = 22, AttackRate = 93, DefenseRate = 22, ViewRange = 4, AttackDelayMilliseconds = 1400 }, // Polluted Butterfly
        new(420) { Level = 24, MaximumHealth = 720, MinimumPhysicalDamage = 85, MaximumPhysicalDamage = 90, Defense = 30, AttackRate = 120, DefenseRate = 30, AttackDelayMilliseconds = 1800 }, // Hideous Rabbit
        new(421) { Level = 13, MaximumHealth = 230, MinimumPhysicalDamage = 37, MaximumPhysicalDamage = 42, Defense = 13, AttackRate = 58, DefenseRate = 13, MoveRange = 3, ViewRange = 4, AttackDelayMilliseconds = 1600 }, // Werewolf 421
        new(422) { AttackRate = 150, DefenseRate = 36, AttackRange = 4, ViewRange = 7, AttackDelayMilliseconds = 2000 }, // Cursed Lich
        new(423) { AttackRate = 180, DefenseRate = 43, ViewRange = 6, AttackDelayMilliseconds = 1400 }, // Totem Golem
        new(424) { MaximumPhysicalDamage = 140, AttackRate = 215, DefenseRate = 52, AttackRange = 2, ViewRange = 6, AttackDelayMilliseconds = 1600, RespawnDelaySeconds = 8 }, // Grizzly
        new(425) { AttackRate = 240, MoveRange = 3, AttackDelayMilliseconds = 1400 }, // Captain Grizzly
        new(426) { DefenseRate = 190 }, // Chaos Castle 13
        new(427) { AttackRate = 520, DefenseRate = 200 }, // Chaos Castle 14
        new(434) { Level = 105, MaximumHealth = 30000, MinimumPhysicalDamage = 475, MaximumPhysicalDamage = 510, Defense = 360, DefenseRate = 270 }, // Gigantis 434
        new(435) { Level = 106, MaximumHealth = 33000, MinimumPhysicalDamage = 500, MaximumPhysicalDamage = 540, Defense = 410, AttackRate = 750, DefenseRate = 280 }, // Berserk
        new(436) { Level = 117, MaximumHealth = 37000, MinimumPhysicalDamage = 560, MaximumPhysicalDamage = 600, Defense = 460, AttackRate = 800, DefenseRate = 320, AttackRange = 4, MoveDelayMilliseconds = 500, AttackDelayMilliseconds = 1500 }, // Balram Trainee
        new(437) { Level = 119, MaximumHealth = 40000, MinimumPhysicalDamage = 610, MaximumPhysicalDamage = 650, Defense = 510, AttackRate = 835, DefenseRate = 330, AttackRange = 2, MoveDelayMilliseconds = 500, AttackDelayMilliseconds = 1500 }, // Soram Trainee
        new(438) { Level = 121, MaximumHealth = 50000, MinimumPhysicalDamage = 640, MaximumPhysicalDamage = 680, Defense = 560, AttackRate = 935, DefenseRate = 370 }, // Persona 438
        new(439) { Level = 123, MaximumHealth = 55000, MinimumPhysicalDamage = 700, MaximumPhysicalDamage = 740, Defense = 610, DefenseRate = 380 }, // Dreadfear 439
        new(440) { MaximumHealth = 75000, MinimumPhysicalDamage = 850, MaximumPhysicalDamage = 900, Defense = 690, AttackRate = 1300, DefenseRate = 430 }, // Dark Elf
        new(448) { AttackRate = 980 }, // Ghost Napin
        new(481) { MaximumHealth = 500000 }, // Zombie Fighter 481
        new(558) { AttackDelayMilliseconds = 2500 }, // Ice Napin
        new(559) { AttackDelayMilliseconds = 2500 }, // Shadow Master
        new(569) { MoveDelayMilliseconds = 900 }, // Venomous Chain Scorpion
        new(570) { Defense = 485, DefenseRate = 283, MoveDelayMilliseconds = 900 }, // Bone Scorpion
        new(571) { MoveDelayMilliseconds = 700, AttackDelayMilliseconds = 1800 }, // Orcus
        new(573) { MoveDelayMilliseconds = 800, AttackDelayMilliseconds = 1700 }, // Crypta
        new(574) { AttackRate = 1140, MoveDelayMilliseconds = 800, AttackDelayMilliseconds = 1700 }, // Crypos
        new(575) { DefenseRate = 405 }, // Condra
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var correction in Corrections)
        {
            correction.ApplyTo(context, gameConfiguration);
        }

        return ValueTask.CompletedTask;
    }
}
