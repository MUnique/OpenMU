﻿// <copyright file="BloodCastle2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
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
    protected override string MapName => "Blood Castle 2";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[090], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 2

        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[091], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 2

        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[092], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 2

        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[093], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 2

        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[094], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 2

        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
        yield return this.CreateMonsterSpawn(this.NpcDictionary[095], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 2
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 90;
            monster.Designation = "Chief Skeleton Warrior 2";
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
                { Stats.MaximumHealth, 7500 },
                { Stats.MinimumPhysBaseDmg, 180 },
                { Stats.MaximumPhysBaseDmg, 220 },
                { Stats.DefenseBase, 150 },
                { Stats.AttackRatePvm, 340 },
                { Stats.DefenseRatePvm, 98 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
                { Stats.LightningResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 090 Chief Skeleton Warrior 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 91;
            monster.Designation = "Chief Skeleton Archer 2";
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
                { Stats.Level, 69 },
                { Stats.MaximumHealth, 9500 },
                { Stats.MinimumPhysBaseDmg, 200 },
                { Stats.MaximumPhysBaseDmg, 240 },
                { Stats.DefenseBase, 170 },
                { Stats.AttackRatePvm, 380 },
                { Stats.DefenseRatePvm, 110 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
                { Stats.LightningResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 091 Chief Skeleton Archer 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 92;
            monster.Designation = "Dark Skull Soldier 2";
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
                { Stats.Level, 75 },
                { Stats.MaximumHealth, 12000 },
                { Stats.MinimumPhysBaseDmg, 220 },
                { Stats.MaximumPhysBaseDmg, 260 },
                { Stats.DefenseBase, 190 },
                { Stats.AttackRatePvm, 440 },
                { Stats.DefenseRatePvm, 130 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
                { Stats.LightningResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 092 Dark Skull Soldier 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 93;
            monster.Designation = "Giant Ogre 2";
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
                { Stats.Level, 79 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 250 },
                { Stats.MaximumPhysBaseDmg, 290 },
                { Stats.DefenseBase, 210 },
                { Stats.AttackRatePvm, 500 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
                { Stats.LightningResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 093 Giant Ogre 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 94;
            monster.Designation = "Red Skeleton Knight 2";
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
                { Stats.Level, 82 },
                { Stats.MaximumHealth, 18000 },
                { Stats.MinimumPhysBaseDmg, 270 },
                { Stats.MaximumPhysBaseDmg, 320 },
                { Stats.DefenseBase, 290 },
                { Stats.AttackRatePvm, 560 },
                { Stats.DefenseRatePvm, 170 },
                { Stats.PoisonResistance, 4f / 255 },
                { Stats.IceResistance, 4f / 255 },
                { Stats.FireResistance, 4f / 255 },
                { Stats.LightningResistance, 4f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 094 Red Skeleton Knight 2

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 95;
            monster.Designation = "Magic Skeleton 2";
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
                { Stats.Level, 88 },
                { Stats.MaximumHealth, 24000 },
                { Stats.MinimumPhysBaseDmg, 300 },
                { Stats.MaximumPhysBaseDmg, 350 },
                { Stats.DefenseBase, 360 },
                { Stats.AttackRatePvm, 640 },
                { Stats.DefenseRatePvm, 200 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 095 Magic Skeleton 2
    }
}