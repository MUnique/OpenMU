// <copyright file="ChaosCastle1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Chaos Castle 1.
/// </summary>
internal class ChaosCastle1 : BaseMapInitializer
{
    /// <summary>
    /// The Number of the Map.
    /// </summary>
    internal const byte Number = 18;

    /// <summary>
    /// The Name of the Map.
    /// </summary>
    internal const string Name = "Chaos Castle 1";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaosCastle1"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ChaosCastle1(IContext context, GameConfiguration gameConfiguration)
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
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[162], 026, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[162], 028, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[162], 028, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[162], 034, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[162], 039, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[162], 038, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[162], 038, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[162], 041, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[162], 032, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[162], 042, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[162], 030, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[162], 040, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[162], 033, 103, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[162], 042, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[162], 035, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[162], 028, 097, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[162], 028, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[162], 025, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[162], 033, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(119, this.NpcDictionary[162], 031, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(120, this.NpcDictionary[162], 038, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(121, this.NpcDictionary[162], 039, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(122, this.NpcDictionary[162], 042, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(123, this.NpcDictionary[162], 034, 090, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(124, this.NpcDictionary[162], 030, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(125, this.NpcDictionary[162], 028, 100, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(126, this.NpcDictionary[162], 035, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(127, this.NpcDictionary[162], 043, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(128, this.NpcDictionary[162], 025, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(129, this.NpcDictionary[162], 030, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(130, this.NpcDictionary[162], 041, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(131, this.NpcDictionary[162], 039, 095, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(132, this.NpcDictionary[162], 029, 103, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(133, this.NpcDictionary[162], 025, 097, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(134, this.NpcDictionary[162], 027, 088, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(135, this.NpcDictionary[162], 032, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(136, this.NpcDictionary[162], 037, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(137, this.NpcDictionary[162], 038, 099, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(138, this.NpcDictionary[162], 043, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(139, this.NpcDictionary[162], 042, 075, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(140, this.NpcDictionary[162], 024, 080, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(141, this.NpcDictionary[162], 024, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(142, this.NpcDictionary[162], 024, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(143, this.NpcDictionary[162], 041, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(144, this.NpcDictionary[162], 041, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(145, this.NpcDictionary[162], 038, 096, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(146, this.NpcDictionary[162], 032, 078, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(147, this.NpcDictionary[162], 027, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(148, this.NpcDictionary[162], 044, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1
        yield return this.CreateMonsterSpawn(149, this.NpcDictionary[162], 039, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 1

        yield return this.CreateMonsterSpawn(150, this.NpcDictionary[163], 026, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(151, this.NpcDictionary[163], 027, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(152, this.NpcDictionary[163], 026, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(153, this.NpcDictionary[163], 033, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(154, this.NpcDictionary[163], 026, 077, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(155, this.NpcDictionary[163], 043, 077, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(156, this.NpcDictionary[163], 043, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(157, this.NpcDictionary[163], 027, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(158, this.NpcDictionary[163], 029, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(159, this.NpcDictionary[163], 044, 087, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(160, this.NpcDictionary[163], 038, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(161, this.NpcDictionary[163], 029, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(162, this.NpcDictionary[163], 038, 089, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(163, this.NpcDictionary[163], 042, 099, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(164, this.NpcDictionary[163], 029, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(165, this.NpcDictionary[163], 026, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(166, this.NpcDictionary[163], 029, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(167, this.NpcDictionary[163], 035, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(168, this.NpcDictionary[163], 041, 079, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(169, this.NpcDictionary[163], 036, 081, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(170, this.NpcDictionary[163], 041, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(171, this.NpcDictionary[163], 035, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(172, this.NpcDictionary[163], 038, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(173, this.NpcDictionary[163], 026, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(174, this.NpcDictionary[163], 038, 106, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(175, this.NpcDictionary[163], 042, 102, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(176, this.NpcDictionary[163], 030, 088, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(177, this.NpcDictionary[163], 029, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(178, this.NpcDictionary[163], 041, 093, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(179, this.NpcDictionary[163], 043, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(180, this.NpcDictionary[163], 032, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(181, this.NpcDictionary[163], 033, 105, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(182, this.NpcDictionary[163], 024, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(183, this.NpcDictionary[163], 032, 094, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(184, this.NpcDictionary[163], 036, 092, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(185, this.NpcDictionary[163], 040, 091, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(186, this.NpcDictionary[163], 040, 101, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(187, this.NpcDictionary[163], 036, 076, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(188, this.NpcDictionary[163], 026, 075, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(189, this.NpcDictionary[163], 030, 082, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(190, this.NpcDictionary[163], 024, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(191, this.NpcDictionary[163], 037, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(192, this.NpcDictionary[163], 039, 087, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(193, this.NpcDictionary[163], 038, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(194, this.NpcDictionary[163], 030, 095, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(195, this.NpcDictionary[163], 029, 086, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(196, this.NpcDictionary[163], 040, 104, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(197, this.NpcDictionary[163], 043, 098, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
        yield return this.CreateMonsterSpawn(198, this.NpcDictionary[163], 041, 084, Direction.Undefined, SpawnTrigger.ManuallyForEvent); // Chaos Castle 2
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 162;
            monster.Designation = "Chaos Castle 1";
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
                { Stats.Level, 30 },
                { Stats.MaximumHealth, 700 },
                { Stats.MinimumPhysBaseDmg, 70 },
                { Stats.MaximumPhysBaseDmg, 90 },
                { Stats.DefenseBase, 30 },
                { Stats.AttackRatePvm, 160 },
                { Stats.DefenseRatePvm, 20 },
                { Stats.PoisonResistance, 1f / 255 },
                { Stats.IceResistance, 1f / 255 },
                { Stats.FireResistance, 1f / 255 },
                { Stats.LightningResistance, 1f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 162 Chaos Castle 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 163;
            monster.Designation = "Chaos Castle 2";
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
                { Stats.Level, 32 },
                { Stats.MaximumHealth, 800 },
                { Stats.MinimumPhysBaseDmg, 80 },
                { Stats.MaximumPhysBaseDmg, 100 },
                { Stats.DefenseBase, 40 },
                { Stats.AttackRatePvm, 180 },
                { Stats.DefenseRatePvm, 30 },
                { Stats.PoisonResistance, 1f / 255 },
                { Stats.IceResistance, 1f / 255 },
                { Stats.FireResistance, 1f / 255 },
                { Stats.LightningResistance, 1f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 163 Chaos Castle 2
    }
}