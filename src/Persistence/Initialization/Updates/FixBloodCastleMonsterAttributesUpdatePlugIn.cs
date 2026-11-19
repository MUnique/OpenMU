// <copyright file="FixBloodCastleMonsterAttributesUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sets the attributes of the monsters of Blood Castle 1 to 8 to the Season 6 data, so that the difficulty
/// rises from level to level.
/// </summary>
/// <remarks>
/// Version 1 swapped the attribute values between BC7 (monsters 138-143) and BC8 (monsters 428-433).
/// Version 2 sets the values on which the Season 6 server data of several sources agree for all levels: the former
/// levels 1 to 5 had the values of the Season 6 levels 3 to 7, and level 6 was stronger than level 7.
/// The resistances of BC7 and BC8 are whole numbers like those of the characters. Databases on which this update stored them
/// divided by 255, as before the elemental resistances became whole numbers, are corrected by <see cref="MonsterResistancesUpdatePlugInBase"/>.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixBloodCastleMonsterAttributesUpdatePlugIn_Name), Description = nameof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources.FixBloodCastleMonsterAttributesUpdatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.Persistence.Initialization.Properties.PlugInResources))]
[Guid("D4E5F6A0-1B2C-3D4E-5F6A-7B8C9D0E1F2A")]
public class FixBloodCastleMonsterAttributesUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// Gets the plugin name.
    /// </summary>
    internal const string PlugInName = "Season 6 Blood Castle monster attributes";

    /// <summary>
    /// Gets the plugin description.
    /// </summary>
    internal const string PlugInDescription = "Sets level, health, damage, defense, attack and defense rate, ranges and delays of the monsters of Blood Castle 1 to 8 to the Season 6 data, so the difficulty rises from level to level. Customized values of these stats are overwritten.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override int Version => 2;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 05, 17, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the corrected stats of the monsters of Blood Castle.
    /// </summary>
    internal static IReadOnlyList<MonsterStatsCorrection> Corrections { get; } =
    [
        new(84) { Level = 33, MaximumHealth = 1100, MinimumPhysicalDamage = 100, MaximumPhysicalDamage = 120, Defense = 55, AttackRate = 190, DefenseRate = 45 }, // Chief Skeleton Warrior 1
        new(85) { Level = 37, MaximumHealth = 1400, MinimumPhysicalDamage = 110, MaximumPhysicalDamage = 130, Defense = 65, AttackRate = 210, DefenseRate = 49 }, // Chief Skeleton Archer 1
        new(86) { Level = 41, MaximumHealth = 1700, MinimumPhysicalDamage = 120, MaximumPhysicalDamage = 140, Defense = 75, AttackRate = 230, DefenseRate = 61, AttackRange = 1 }, // Dark Skull Soldier 1
        new(87) { Level = 44, MaximumHealth = 2100, MinimumPhysicalDamage = 130, MaximumPhysicalDamage = 150, Defense = 85, AttackRate = 250, DefenseRate = 73 }, // Giant Ogre 1
        new(88) { Level = 47, MaximumHealth = 2800, MinimumPhysicalDamage = 140, MaximumPhysicalDamage = 160, Defense = 95, AttackRate = 270, DefenseRate = 77 }, // Red Skeleton Knight 1
        new(89) { Level = 51, MaximumHealth = 3700, MinimumPhysicalDamage = 150, MaximumPhysicalDamage = 180, Defense = 115, AttackRate = 300, DefenseRate = 88, AttackDelayMilliseconds = 1600 }, // Magic Skeleton 1
        new(90) { Level = 49, MaximumHealth = 3500, MinimumPhysicalDamage = 140, MaximumPhysicalDamage = 170, Defense = 90, AttackRate = 250, DefenseRate = 76 }, // Chief Skeleton Warrior 2
        new(91) { Level = 53, MaximumHealth = 4100, MinimumPhysicalDamage = 150, MaximumPhysicalDamage = 180, Defense = 100, AttackRate = 275, DefenseRate = 80 }, // Chief Skeleton Archer 2
        new(92) { Level = 56, MaximumHealth = 5000, MinimumPhysicalDamage = 160, MaximumPhysicalDamage = 190, Defense = 115, AttackRate = 300, DefenseRate = 88 }, // Dark Skull Soldier 2
        new(93) { Level = 60, MaximumHealth = 6000, MinimumPhysicalDamage = 175, MaximumPhysicalDamage = 210, Defense = 130, AttackRate = 320, DefenseRate = 95 }, // Giant Ogre 2
        new(94) { Level = 64, MaximumHealth = 7000, MinimumPhysicalDamage = 185, MaximumPhysicalDamage = 220, Defense = 170, AttackRate = 360, DefenseRate = 110 }, // Red Skeleton Knight 2
        new(95) { Level = 69, MaximumHealth = 8000, MinimumPhysicalDamage = 200, MaximumPhysicalDamage = 240, Defense = 190, AttackRate = 400, DefenseRate = 130 }, // Magic Skeleton 2
        new(96) { Level = 56, MaximumHealth = 5000, MinimumPhysicalDamage = 160, MaximumPhysicalDamage = 180, Defense = 110, AttackRate = 300, DefenseRate = 85 }, // Chief Skeleton Warrior 3
        new(97) { Level = 61, MaximumHealth = 6500, MinimumPhysicalDamage = 180, MaximumPhysicalDamage = 200, Defense = 120, AttackRate = 330, DefenseRate = 93 }, // Chief Skeleton Archer 3
        new(98) { Level = 66, MaximumHealth = 8000, MinimumPhysicalDamage = 190, MaximumPhysicalDamage = 220, Defense = 150, AttackRate = 360, DefenseRate = 98 }, // Dark Skull Soldier 3
        new(99) { Level = 70, MaximumHealth = 9500, MinimumPhysicalDamage = 210, MaximumPhysicalDamage = 240, Defense = 180, AttackRate = 400, DefenseRate = 115 }, // Giant Ogre 3
        new(111) { Level = 74, MaximumHealth = 12000, MinimumPhysicalDamage = 220, MaximumPhysicalDamage = 260, Defense = 190, AttackRate = 440, DefenseRate = 130 }, // Red Skeleton Knight 3
        new(112) { Level = 79, MaximumHealth = 15000, MinimumPhysicalDamage = 230, MaximumPhysicalDamage = 280, Defense = 240, AttackRate = 500, DefenseRate = 180 }, // Magic Skeleton 3
        new(113) { Level = 64, MaximumHealth = 7500, MinimumPhysicalDamage = 180, MaximumPhysicalDamage = 220, Defense = 150, AttackRate = 340, DefenseRate = 98 }, // Chief Skeleton Warrior 4
        new(114) { Level = 69, MaximumHealth = 9500, MinimumPhysicalDamage = 200, MaximumPhysicalDamage = 240, Defense = 170, AttackRate = 380, DefenseRate = 110 }, // Chief Skeleton Archer 4
        new(115) { Level = 75, MaximumHealth = 12000, MinimumPhysicalDamage = 220, MaximumPhysicalDamage = 260, Defense = 190, AttackRate = 440, DefenseRate = 130 }, // Dark Skull Soldier 4
        new(116) { Level = 79, MaximumHealth = 15000, MinimumPhysicalDamage = 250, MaximumPhysicalDamage = 290, Defense = 210, AttackRate = 500, DefenseRate = 150 }, // Giant Ogre 4
        new(117) { Level = 82, MaximumHealth = 18000, MinimumPhysicalDamage = 270, MaximumPhysicalDamage = 320, Defense = 290, AttackRate = 560, DefenseRate = 170 }, // Red Skeleton Knight 4
        new(118) { Level = 84, MaximumHealth = 24000, MinimumPhysicalDamage = 300, MaximumPhysicalDamage = 350, Defense = 360, AttackRate = 640, DefenseRate = 200 }, // Magic Skeleton 4
        new(119) { Level = 70, MaximumHealth = 12000, MinimumPhysicalDamage = 210, MaximumPhysicalDamage = 270, Defense = 185, AttackRate = 380, DefenseRate = 125 }, // Chief Skeleton Warrior 5
        new(120) { Level = 75, MaximumHealth = 15000, MinimumPhysicalDamage = 240, MaximumPhysicalDamage = 300, Defense = 190, AttackRate = 420, DefenseRate = 130 }, // Chief Skeleton Archer 5
        new(121) { Level = 80, MaximumHealth = 19000, MinimumPhysicalDamage = 280, MaximumPhysicalDamage = 340, Defense = 220, AttackRate = 470, DefenseRate = 175 }, // Dark Skull Soldier 5
        new(122) { Level = 85, MaximumHealth = 23000, MinimumPhysicalDamage = 340, MaximumPhysicalDamage = 390, Defense = 300, AttackRate = 520, DefenseRate = 190 }, // Giant Ogre 5
        new(123) { Level = 87, MaximumHealth = 26000, MinimumPhysicalDamage = 370, MaximumPhysicalDamage = 410, Defense = 400, AttackRate = 650, DefenseRate = 200 }, // Red Skeleton Knight 5
        new(124) { Level = 90, MaximumHealth = 32000, MinimumPhysicalDamage = 400, MaximumPhysicalDamage = 460, Defense = 450, AttackRate = 720, DefenseRate = 260 }, // Magic Skeleton 5
        new(125) { Level = 76, MaximumHealth = 17000, MinimumPhysicalDamage = 260, MaximumPhysicalDamage = 290, AttackRate = 460, DefenseRate = 150 }, // Chief Skeleton Warrior 6
        new(126) { Level = 81, MaximumHealth = 20000, MinimumPhysicalDamage = 320, MaximumPhysicalDamage = 350, Defense = 270, AttackRate = 520, DefenseRate = 160 }, // Chief Skeleton Archer 6
        new(127) { Level = 86, MaximumHealth = 25000, MinimumPhysicalDamage = 390, MaximumPhysicalDamage = 420, Defense = 360, AttackRate = 580, DefenseRate = 195 }, // Dark Skull Soldier 6
        new(128) { Level = 90, MaximumHealth = 32000, MinimumPhysicalDamage = 440, MaximumPhysicalDamage = 500, Defense = 400, AttackRate = 660, DefenseRate = 230 }, // Giant Ogre 6
        new(129) { Level = 94, MaximumHealth = 38000, MinimumPhysicalDamage = 480, MaximumPhysicalDamage = 540, Defense = 450, AttackRate = 720, DefenseRate = 260 }, // Red Skeleton Knight 6
        new(130) { Level = 99, MaximumHealth = 47000, MinimumPhysicalDamage = 530, MaximumPhysicalDamage = 580, Defense = 500, AttackRate = 840, DefenseRate = 280 }, // Magic Skeleton 6
        new(138) { Level = 85, MaximumHealth = 23000, MinimumPhysicalDamage = 340, MaximumPhysicalDamage = 390, Defense = 340, AttackRate = 520, DefenseRate = 190 }, // Chief Skeleton Warrior 7
        new(139) { Level = 87, MaximumHealth = 26000, MinimumPhysicalDamage = 390, MaximumPhysicalDamage = 430, Defense = 380, AttackRate = 590, DefenseRate = 200 }, // Chief Skeleton Archer 7
        new(140) { Level = 90, MaximumHealth = 32000, MinimumPhysicalDamage = 430, MaximumPhysicalDamage = 480, Defense = 400, AttackRate = 660, DefenseRate = 230 }, // Dark Skull Soldier 7
        new(141) { Level = 93, MaximumHealth = 37000, MinimumPhysicalDamage = 470, MaximumPhysicalDamage = 520, Defense = 440, AttackRate = 730, DefenseRate = 250 }, // Giant Ogre 7
        new(142) { Level = 98, MaximumHealth = 45000, MinimumPhysicalDamage = 510, MaximumPhysicalDamage = 560, Defense = 490, AttackRate = 800, DefenseRate = 270 }, // Red Skeleton Knight 7
        new(143) { Level = 102, MaximumHealth = 55000, MinimumPhysicalDamage = 560, MaximumPhysicalDamage = 600, Defense = 550, AttackRate = 870, DefenseRate = 310 }, // Magic Skeleton 7
        new(428) { Level = 105, MaximumHealth = 29000, MinimumPhysicalDamage = 475, MaximumPhysicalDamage = 510, Defense = 440, AttackRate = 570, DefenseRate = 250 }, // Chief Skeleton Warrior 8
        new(429) { Level = 106, MaximumHealth = 32000, MinimumPhysicalDamage = 510, MaximumPhysicalDamage = 555, Defense = 480, AttackRate = 640, DefenseRate = 260 }, // Chief Skeleton Archer 8
        new(430) { Level = 110, MaximumHealth = 37000, MinimumPhysicalDamage = 600, MaximumPhysicalDamage = 650, Defense = 500, AttackRate = 710, DefenseRate = 300 }, // Dark Skull Soldier 8
        new(431) { Level = 112, MaximumHealth = 45000, MinimumPhysicalDamage = 645, MaximumPhysicalDamage = 690, Defense = 540, AttackRate = 780, DefenseRate = 310 }, // Giant Ogre 8
        new(432) { Level = 119, MaximumHealth = 55000, MinimumPhysicalDamage = 780, MaximumPhysicalDamage = 820, Defense = 600, AttackRate = 850, DefenseRate = 360 }, // Red Skeleton Knight 8
        new(433) { Level = 125, MaximumHealth = 65000, MinimumPhysicalDamage = 830, MaximumPhysicalDamage = 865, Defense = 680, AttackRate = 920, DefenseRate = 370 }, // Magic Skeleton 8
    ];

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var correction in Corrections)
        {
            correction.ApplyTo(context, gameConfiguration);
        }

        // The resistances which version 1 swapped together with the other attributes.
        foreach (var (number, resistance) in new (short Number, float Resistance)[] { (138, 7), (139, 7), (140, 7), (141, 7), (142, 7), (143, 10), (428, 8), (429, 8), (430, 8), (431, 8), (432, 8), (433, 11) })
        {
            if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == number) is { } monster)
            {
                SetAttribute(monster, Stats.PoisonResistance, resistance);
                SetAttribute(monster, Stats.IceResistance, resistance);
                SetAttribute(monster, Stats.FireResistance, resistance);
                SetAttribute(monster, Stats.LightningResistance, resistance);
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void SetAttribute(MonsterDefinition monster, AttributeDefinition attributeDefinition, float value)
    {
        if (monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeDefinition.Id) is { } attribute)
        {
            attribute.Value = value;
        }
    }
}
