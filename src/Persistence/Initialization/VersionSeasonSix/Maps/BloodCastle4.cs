﻿// <copyright file="BloodCastle4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 4.
/// </summary>
internal class BloodCastle4 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle4"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle4(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 14;

    /// <inheritdoc/>
    protected override string MapName => "Blood Castle 4";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[113], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 4

        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[114], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 4

        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[115], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 4

        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[116], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 4

        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[117], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 4

        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
        yield return this.CreateMonsterSpawn(this.NpcDictionary[118], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 4
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 113;
            monster.Designation = "Chief Skeleton Warrior 4";
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
                { Stats.Level, 76 },
                { Stats.MaximumHealth, 17000 },
                { Stats.MinimumPhysBaseDmg, 260 },
                { Stats.MaximumPhysBaseDmg, 290 },
                { Stats.DefenseBase, 210 },
                { Stats.AttackRatePvm, 460 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 113 Chief Skeleton Warrior 4

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 114;
            monster.Designation = "Chief Skeleton Archer 4";
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
                { Stats.Level, 81 },
                { Stats.MaximumHealth, 20000 },
                { Stats.MinimumPhysBaseDmg, 320 },
                { Stats.MaximumPhysBaseDmg, 350 },
                { Stats.DefenseBase, 270 },
                { Stats.AttackRatePvm, 520 },
                { Stats.DefenseRatePvm, 160 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 114 Chief Skeleton Archer 4

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 115;
            monster.Designation = "Dark Skull Soldier 4";
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
                { Stats.Level, 86 },
                { Stats.MaximumHealth, 25000 },
                { Stats.MinimumPhysBaseDmg, 390 },
                { Stats.MaximumPhysBaseDmg, 420 },
                { Stats.DefenseBase, 360 },
                { Stats.AttackRatePvm, 580 },
                { Stats.DefenseRatePvm, 195 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 115 Dark Skull Soldier 4

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 116;
            monster.Designation = "Giant Ogre 4";
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
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 32000 },
                { Stats.MinimumPhysBaseDmg, 440 },
                { Stats.MaximumPhysBaseDmg, 500 },
                { Stats.DefenseBase, 400 },
                { Stats.AttackRatePvm, 660 },
                { Stats.DefenseRatePvm, 230 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 116 Giant Ogre 4

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 117;
            monster.Designation = "Red Skeleton Knight 4";
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
                { Stats.Level, 94 },
                { Stats.MaximumHealth, 38000 },
                { Stats.MinimumPhysBaseDmg, 480 },
                { Stats.MaximumPhysBaseDmg, 540 },
                { Stats.DefenseBase, 450 },
                { Stats.AttackRatePvm, 720 },
                { Stats.DefenseRatePvm, 260 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 117 Red Skeleton Knight 4

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 118;
            monster.Designation = "Magic Skeleton 4";
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
                { Stats.Level, 99 },
                { Stats.MaximumHealth, 47000 },
                { Stats.MinimumPhysBaseDmg, 530 },
                { Stats.MaximumPhysBaseDmg, 580 },
                { Stats.DefenseBase, 500 },
                { Stats.AttackRatePvm, 840 },
                { Stats.DefenseRatePvm, 280 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
                { Stats.LightningResistance, 9f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 118 Magic Skeleton 4
    }
}