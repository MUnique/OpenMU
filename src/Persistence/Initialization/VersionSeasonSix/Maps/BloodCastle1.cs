// <copyright file="BloodCastle1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 1.
/// </summary>
internal class BloodCastle1 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle1"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle1(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 11;

    /// <inheritdoc/>
    protected override int CastleLevel => 1;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[084], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[084], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[084], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[084], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[084], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[084], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[084], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[084], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[084], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[084], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[084], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[084], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[084], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[084], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[084], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[084], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[084], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[084], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[084], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 1

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[085], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[085], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[085], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[085], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[085], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[085], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[085], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[085], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[085], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[085], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[085], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[085], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[085], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[085], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[085], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[085], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[085], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 1

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[086], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[086], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[086], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[086], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[086], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[086], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[086], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[086], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[086], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[086], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[086], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[086], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[086], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[086], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[086], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[086], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[086], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[086], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[086], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[086], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[086], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[086], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[086], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[086], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 1

        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[087], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[087], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[087], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[087], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[087], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[087], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[087], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[087], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[087], 014, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[087], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[087], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 1

        yield return this.CreateMonsterSpawn(500, this.NpcDictionary[088], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[088], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[088], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[088], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[088], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[088], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[088], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[088], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[088], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[088], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(510, this.NpcDictionary[088], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(511, this.NpcDictionary[088], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(512, this.NpcDictionary[088], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(513, this.NpcDictionary[088], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(514, this.NpcDictionary[088], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1
        yield return this.CreateMonsterSpawn(515, this.NpcDictionary[088], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 1

        yield return this.CreateMonsterSpawn(600, this.NpcDictionary[089], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(601, this.NpcDictionary[089], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(602, this.NpcDictionary[089], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(603, this.NpcDictionary[089], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(604, this.NpcDictionary[089], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(605, this.NpcDictionary[089], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(606, this.NpcDictionary[089], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
        yield return this.CreateMonsterSpawn(607, this.NpcDictionary[089], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 1
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 84;
            monster.Designation = "Chief Skeleton Warrior 1";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 3;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 56 },
                { Stats.MaximumHealth, 5000 },
                { Stats.MinimumPhysBaseDmg, 160 },
                { Stats.MaximumPhysBaseDmg, 180 },
                { Stats.DefenseBase, 110 },
                { Stats.AttackRatePvm, 300 },
                { Stats.DefenseRatePvm, 85 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
                { Stats.LightningResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 084 Chief Skeleton Warrior 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 85;
            monster.Designation = "Chief Skeleton Archer 1";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 61 },
                { Stats.MaximumHealth, 6500 },
                { Stats.MinimumPhysBaseDmg, 180 },
                { Stats.MaximumPhysBaseDmg, 200 },
                { Stats.DefenseBase, 120 },
                { Stats.AttackRatePvm, 330 },
                { Stats.DefenseRatePvm, 93 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
                { Stats.LightningResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 085 Chief Skeleton Archer 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 86;
            monster.Designation = "Dark Skull Soldier 1";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 3;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 66 },
                { Stats.MaximumHealth, 8000 },
                { Stats.MinimumPhysBaseDmg, 190 },
                { Stats.MaximumPhysBaseDmg, 220 },
                { Stats.DefenseBase, 160 },
                { Stats.AttackRatePvm, 360 },
                { Stats.DefenseRatePvm, 98 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
                { Stats.LightningResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 086 Dark Skull Soldier 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 87;
            monster.Designation = "Giant Ogre 1";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 70 },
                { Stats.MaximumHealth, 9500 },
                { Stats.MinimumPhysBaseDmg, 210 },
                { Stats.MaximumPhysBaseDmg, 240 },
                { Stats.DefenseBase, 180 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 115 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
                { Stats.LightningResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 087 Giant Ogre 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 88;
            monster.Designation = "Red Skeleton Knight 1";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 3;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 74 },
                { Stats.MaximumHealth, 12000 },
                { Stats.MinimumPhysBaseDmg, 220 },
                { Stats.MaximumPhysBaseDmg, 260 },
                { Stats.DefenseBase, 190 },
                { Stats.AttackRatePvm, 440 },
                { Stats.DefenseRatePvm, 130 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
                { Stats.LightningResistance, 2f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 088 Red Skeleton Knight 1

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 89;
            monster.Designation = "Magic Skeleton 1";
            monster.MoveRange = 4;
            monster.AttackRange = 4;
            monster.ViewRange = 6;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 79 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 230 },
                { Stats.MaximumPhysBaseDmg, 280 },
                { Stats.DefenseBase, 240 },
                { Stats.AttackRatePvm, 500 },
                { Stats.DefenseRatePvm, 180 },
                { Stats.PoisonResistance, 5f / 255 },
                { Stats.IceResistance, 5f / 255 },
                { Stats.FireResistance, 5f / 255 },
                { Stats.LightningResistance, 5f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 089 Magic Skeleton 1
    }
}