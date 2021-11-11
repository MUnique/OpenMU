﻿// <copyright file="BloodCastle6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 6.
/// </summary>
internal class BloodCastle6 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle6"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle6(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 16;

    /// <inheritdoc/>
    protected override string MapName => "Blood Castle 6";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[125], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 6

        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[126], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 6

        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[127], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 6

        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[128], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 6

        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[129], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 6

        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
        yield return this.CreateMonsterSpawn(this.NpcDictionary[130], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 6
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 125;
            monster.Designation = "Chief Skeleton Warrior 6";
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
                { Stats.Level, 110 },
                { Stats.MaximumHealth, 50000 },
                { Stats.MinimumPhysBaseDmg, 470 },
                { Stats.MaximumPhysBaseDmg, 510 },
                { Stats.DefenseBase, 210 },
                { Stats.AttackRatePvm, 570 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 125 Chief Skeleton Warrior 6

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 126;
            monster.Designation = "Chief Skeleton Archer 6";
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
                { Stats.Level, 112 },
                { Stats.MaximumHealth, 53000 },
                { Stats.MinimumPhysBaseDmg, 530 },
                { Stats.MaximumPhysBaseDmg, 556 },
                { Stats.DefenseBase, 590 },
                { Stats.AttackRatePvm, 640 },
                { Stats.DefenseRatePvm, 380 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 126 Chief Skeleton Archer 6

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 127;
            monster.Designation = "Dark Skull Soldier 6";
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
                { Stats.Level, 118 },
                { Stats.MaximumHealth, 57000 },
                { Stats.MinimumPhysBaseDmg, 560 },
                { Stats.MaximumPhysBaseDmg, 581 },
                { Stats.DefenseBase, 610 },
                { Stats.AttackRatePvm, 710 },
                { Stats.DefenseRatePvm, 300 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 127 Dark Skull Soldier 6

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 128;
            monster.Designation = "Giant Ogre 6";
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
                { Stats.Level, 120 },
                { Stats.MaximumHealth, 62000 },
                { Stats.MinimumPhysBaseDmg, 580 },
                { Stats.MaximumPhysBaseDmg, 599 },
                { Stats.DefenseBase, 615 },
                { Stats.AttackRatePvm, 780 },
                { Stats.DefenseRatePvm, 310 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 128 Giant Ogre 6

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 129;
            monster.Designation = "Red Skeleton Knight 6";
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
                { Stats.Level, 127 },
                { Stats.MaximumHealth, 70000 },
                { Stats.MinimumPhysBaseDmg, 600 },
                { Stats.MaximumPhysBaseDmg, 618 },
                { Stats.DefenseBase, 635 },
                { Stats.AttackRatePvm, 850 },
                { Stats.DefenseRatePvm, 360 },
                { Stats.PoisonResistance, 6f / 255 },
                { Stats.IceResistance, 6f / 255 },
                { Stats.FireResistance, 6f / 255 },
                { Stats.LightningResistance, 6f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 129 Red Skeleton Knight 6

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 130;
            monster.Designation = "Magic Skeleton 6";
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
                { Stats.Level, 135 },
                { Stats.MaximumHealth, 80000 },
                { Stats.MinimumPhysBaseDmg, 634 },
                { Stats.MaximumPhysBaseDmg, 651 },
                { Stats.DefenseBase, 680 },
                { Stats.AttackRatePvm, 920 },
                { Stats.DefenseRatePvm, 370 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
                { Stats.LightningResistance, 9f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 130 Magic Skeleton 6
    }
}