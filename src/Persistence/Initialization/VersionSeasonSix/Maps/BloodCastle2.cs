// <copyright file="BloodCastle2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 2.
/// </summary>
internal class BloodCastle2 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle2"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle2(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 12;

    /// <inheritdoc/>
    protected override int CastleLevel => 2;

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(100, this.NpcDictionary[090], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(101, this.NpcDictionary[090], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(102, this.NpcDictionary[090], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(103, this.NpcDictionary[090], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(104, this.NpcDictionary[090], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(105, this.NpcDictionary[090], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(106, this.NpcDictionary[090], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(107, this.NpcDictionary[090], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(108, this.NpcDictionary[090], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(109, this.NpcDictionary[090], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(110, this.NpcDictionary[090], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(111, this.NpcDictionary[090], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(112, this.NpcDictionary[090], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(113, this.NpcDictionary[090], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(114, this.NpcDictionary[090], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(115, this.NpcDictionary[090], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(116, this.NpcDictionary[090], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(117, this.NpcDictionary[090], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(118, this.NpcDictionary[090], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2

        yield return this.CreateMonsterSpawn(200, this.NpcDictionary[091], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(201, this.NpcDictionary[091], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(202, this.NpcDictionary[091], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(203, this.NpcDictionary[091], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(204, this.NpcDictionary[091], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(205, this.NpcDictionary[091], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(206, this.NpcDictionary[091], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(207, this.NpcDictionary[091], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(208, this.NpcDictionary[091], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(209, this.NpcDictionary[091], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(210, this.NpcDictionary[091], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(211, this.NpcDictionary[091], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(212, this.NpcDictionary[091], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(213, this.NpcDictionary[091], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(214, this.NpcDictionary[091], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(215, this.NpcDictionary[091], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(216, this.NpcDictionary[091], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2

        yield return this.CreateMonsterSpawn(300, this.NpcDictionary[092], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(301, this.NpcDictionary[092], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(302, this.NpcDictionary[092], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(303, this.NpcDictionary[092], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(304, this.NpcDictionary[092], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(305, this.NpcDictionary[092], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(306, this.NpcDictionary[092], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(307, this.NpcDictionary[092], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(308, this.NpcDictionary[092], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(309, this.NpcDictionary[092], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(310, this.NpcDictionary[092], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(311, this.NpcDictionary[092], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(312, this.NpcDictionary[092], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(313, this.NpcDictionary[092], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(314, this.NpcDictionary[092], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(315, this.NpcDictionary[092], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(316, this.NpcDictionary[092], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(317, this.NpcDictionary[092], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(318, this.NpcDictionary[092], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(319, this.NpcDictionary[092], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(320, this.NpcDictionary[092], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(321, this.NpcDictionary[092], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(322, this.NpcDictionary[092], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(323, this.NpcDictionary[092], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2

        yield return this.CreateMonsterSpawn(400, this.NpcDictionary[093], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(401, this.NpcDictionary[093], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(402, this.NpcDictionary[093], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(403, this.NpcDictionary[093], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(404, this.NpcDictionary[093], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(405, this.NpcDictionary[093], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(406, this.NpcDictionary[093], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(407, this.NpcDictionary[093], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(408, this.NpcDictionary[093], 014, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(409, this.NpcDictionary[093], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(410, this.NpcDictionary[093], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2

        yield return this.CreateMonsterSpawn(500, this.NpcDictionary[094], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(501, this.NpcDictionary[094], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(502, this.NpcDictionary[094], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(503, this.NpcDictionary[094], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(504, this.NpcDictionary[094], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(505, this.NpcDictionary[094], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(506, this.NpcDictionary[094], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(507, this.NpcDictionary[094], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(508, this.NpcDictionary[094], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(509, this.NpcDictionary[094], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(510, this.NpcDictionary[094], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(511, this.NpcDictionary[094], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(512, this.NpcDictionary[094], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(513, this.NpcDictionary[094], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(514, this.NpcDictionary[094], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(515, this.NpcDictionary[094], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2

        yield return this.CreateMonsterSpawn(600, this.NpcDictionary[095], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(601, this.NpcDictionary[095], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(602, this.NpcDictionary[095], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(603, this.NpcDictionary[095], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(604, this.NpcDictionary[095], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(605, this.NpcDictionary[095], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(606, this.NpcDictionary[095], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(607, this.NpcDictionary[095], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 90;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.ChiefSkeletonWarrior2);
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
                { Stats.Level, 49 },
                { Stats.MaximumHealth, 3500 },
                { Stats.MinimumPhysBaseDmg, 140 },
                { Stats.MaximumPhysBaseDmg, 170 },
                { Stats.DefenseBase, 90 },
                { Stats.AttackRatePvm, 250 },
                { Stats.DefenseRatePvm, 76 },
                { Stats.IceResistance, 1 },
                { Stats.PoisonResistance, 1 },
                { Stats.LightningResistance, 1 },
                { Stats.FireResistance, 1 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 090 Chief Skeleton Warrior 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 91;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.ChiefSkeletonArcher2);
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
                { Stats.Level, 53 },
                { Stats.MaximumHealth, 4100 },
                { Stats.MinimumPhysBaseDmg, 150 },
                { Stats.MaximumPhysBaseDmg, 180 },
                { Stats.DefenseBase, 100 },
                { Stats.AttackRatePvm, 275 },
                { Stats.DefenseRatePvm, 80 },
                { Stats.IceResistance, 1 },
                { Stats.PoisonResistance, 1 },
                { Stats.LightningResistance, 1 },
                { Stats.FireResistance, 1 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 091 Chief Skeleton Archer 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 92;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.DarkSkullSoldier2);
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
                { Stats.MaximumPhysBaseDmg, 190 },
                { Stats.DefenseBase, 115 },
                { Stats.AttackRatePvm, 300 },
                { Stats.DefenseRatePvm, 88 },
                { Stats.IceResistance, 1 },
                { Stats.PoisonResistance, 1 },
                { Stats.LightningResistance, 1 },
                { Stats.FireResistance, 1 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 092 Dark Skull Soldier 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 93;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.GiantOgre2);
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
                { Stats.Level, 60 },
                { Stats.MaximumHealth, 6000 },
                { Stats.MinimumPhysBaseDmg, 175 },
                { Stats.MaximumPhysBaseDmg, 210 },
                { Stats.DefenseBase, 130 },
                { Stats.AttackRatePvm, 320 },
                { Stats.DefenseRatePvm, 95 },
                { Stats.IceResistance, 1 },
                { Stats.PoisonResistance, 1 },
                { Stats.LightningResistance, 1 },
                { Stats.FireResistance, 1 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 093 Giant Ogre 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 94;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.RedSkeletonKnight2);
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
                { Stats.Level, 64 },
                { Stats.MaximumHealth, 7000 },
                { Stats.MinimumPhysBaseDmg, 185 },
                { Stats.MaximumPhysBaseDmg, 220 },
                { Stats.DefenseBase, 170 },
                { Stats.AttackRatePvm, 360 },
                { Stats.DefenseRatePvm, 110 },
                { Stats.IceResistance, 1 },
                { Stats.PoisonResistance, 1 },
                { Stats.LightningResistance, 1 },
                { Stats.FireResistance, 1 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 094 Red Skeleton Knight 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 95;
            monster.Designation = LocalizedString.FromResource(() => MonsterNames.MagicSkeleton2);
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
                { Stats.Level, 69 },
                { Stats.MaximumHealth, 8000 },
                { Stats.MinimumPhysBaseDmg, 200 },
                { Stats.MaximumPhysBaseDmg, 240 },
                { Stats.DefenseBase, 190 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 130 },
                { Stats.IceResistance, 4 },
                { Stats.PoisonResistance, 4 },
                { Stats.LightningResistance, 4 },
                { Stats.FireResistance, 4 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        } // 095 Magic Skeleton 2
    }
}