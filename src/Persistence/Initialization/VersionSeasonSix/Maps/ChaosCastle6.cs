// <copyright file="ChaosCastle6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Chaos Castle 6.
/// </summary>
internal class ChaosCastle6 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 23;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Chaos Castle 6";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastle6"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ChaosCastle6(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte SafezoneMapNumber => Devias.Number;

    /// <inheritdoc/>
    protected override byte MapNumber => Number;

    /// <inheritdoc/>
    protected override string MapName => Name;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        // Monsters:
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[172], 026, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[172], 028, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[172], 028, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[172], 034, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[172], 039, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[172], 038, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[172], 038, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[172], 041, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[172], 032, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[172], 042, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[172], 030, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[172], 040, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[172], 033, 103, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[172], 042, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[172], 035, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[172], 028, 097, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[172], 028, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[172], 025, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[172], 033, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[172], 031, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[172], 038, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[172], 039, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[172], 042, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[172], 034, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[172], 030, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[172], 028, 100, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[172], 035, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[172], 043, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[172], 025, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[172], 030, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[172], 041, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[172], 039, 095, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[172], 029, 103, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[172], 025, 097, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[172], 027, 088, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[172], 032, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[172], 037, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[172], 038, 099, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[172], 043, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[172], 042, 075, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[172], 024, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[172], 024, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[172], 024, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[172], 041, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[172], 041, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[172], 038, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[172], 032, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[172], 027, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[172], 044, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[172], 039, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 11

        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[173], 026, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[173], 027, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[173], 026, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[173], 033, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[173], 026, 077, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[173], 043, 077, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[173], 043, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[173], 027, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[173], 029, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[173], 044, 087, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[173], 038, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[173], 029, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[173], 038, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[173], 042, 099, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[173], 029, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[173], 026, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[173], 029, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[173], 035, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[173], 041, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[173], 036, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[173], 041, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[173], 035, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(172, this.NpcDictionary[173], 038, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(173, this.NpcDictionary[173], 026, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(174, this.NpcDictionary[173], 038, 106, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(175, this.NpcDictionary[173], 042, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(176, this.NpcDictionary[173], 030, 088, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(177, this.NpcDictionary[173], 029, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(178, this.NpcDictionary[173], 041, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(179, this.NpcDictionary[173], 043, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(180, this.NpcDictionary[173], 032, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(181, this.NpcDictionary[173], 033, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(182, this.NpcDictionary[173], 024, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(183, this.NpcDictionary[173], 032, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(184, this.NpcDictionary[173], 036, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(185, this.NpcDictionary[173], 040, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(186, this.NpcDictionary[173], 040, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(187, this.NpcDictionary[173], 036, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(188, this.NpcDictionary[173], 026, 075, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(189, this.NpcDictionary[173], 030, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(190, this.NpcDictionary[173], 024, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(191, this.NpcDictionary[173], 037, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(192, this.NpcDictionary[173], 039, 087, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(193, this.NpcDictionary[173], 038, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(194, this.NpcDictionary[173], 030, 095, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(195, this.NpcDictionary[173], 029, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(196, this.NpcDictionary[173], 040, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(197, this.NpcDictionary[173], 043, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
        yield return this.CreateMonsterSpawn(198, this.NpcDictionary[173], 041, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 12
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 172;
            monster.Designation = "Chaos Castle 11";
            monster.MoveRange = 50;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 76 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 255 },
                { Stats.MaximumPhysBaseDmg, 280 },
                { Stats.DefenseBase, 215 },
                { Stats.AttackRatePvm, 430 },
                { Stats.DefenseRatePvm, 140 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 172 Chaos Castle 11

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 173;
            monster.Designation = "Chaos Castle 12";
            monster.MoveRange = 50;
            monster.AttackRange = 6;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 18000 },
                { Stats.MinimumPhysBaseDmg, 280 },
                { Stats.MaximumPhysBaseDmg, 300 },
                { Stats.DefenseBase, 240 },
                { Stats.AttackRatePvm, 470 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 173 Chaos Castle 12
    }
}