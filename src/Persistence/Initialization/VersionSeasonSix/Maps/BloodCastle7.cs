﻿// <copyright file="BloodCastle7.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 7.
/// </summary>
internal class BloodCastle7 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle7"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle7(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 17;

    /// <inheritdoc/>
    protected override string MapName => "Blood Castle 7";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[138], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 7

        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[139], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 7

        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[140], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 7

        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[141], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 7

        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[142], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 7

        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
        yield return this.CreateMonsterSpawn(this.NpcDictionary[143], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 7
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 138;
            monster.Designation = "Chief Skeleton Warrior 7";
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
                { Stats.Level, 114 },
                { Stats.MaximumHealth, 173500 },
                { Stats.MinimumPhysBaseDmg, 745 },
                { Stats.MaximumPhysBaseDmg, 800 },
                { Stats.DefenseBase, 600 },
                { Stats.AttackRatePvm, 640 },
                { Stats.DefenseRatePvm, 426 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 138 Chief Skeleton Warrior 7

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 139;
            monster.Designation = "Chief Skeleton Archer 7";
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
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 175000 },
                { Stats.MinimumPhysBaseDmg, 825 },
                { Stats.MaximumPhysBaseDmg, 872 },
                { Stats.DefenseBase, 615 },
                { Stats.AttackRatePvm, 690 },
                { Stats.DefenseRatePvm, 440 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 139 Chief Skeleton Archer 7

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 140;
            monster.Designation = "Dark Skull Soldier 7";
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
                { Stats.Level, 125 },
                { Stats.MaximumHealth, 184000 },
                { Stats.MinimumPhysBaseDmg, 890 },
                { Stats.MaximumPhysBaseDmg, 915 },
                { Stats.DefenseBase, 622 },
                { Stats.AttackRatePvm, 760 },
                { Stats.DefenseRatePvm, 465 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 140 Dark Skull Soldier 7

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 141;
            monster.Designation = "Giant Ogre 7";
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
                { Stats.Level, 129 },
                { Stats.MaximumHealth, 208000 },
                { Stats.MinimumPhysBaseDmg, 920 },
                { Stats.MaximumPhysBaseDmg, 946 },
                { Stats.DefenseBase, 635 },
                { Stats.AttackRatePvm, 830 },
                { Stats.DefenseRatePvm, 510 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 141 Giant Ogre 7

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 142;
            monster.Designation = "Red Skeleton Knight 7";
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
                { Stats.Level, 132 },
                { Stats.MaximumHealth, 208700 },
                { Stats.MinimumPhysBaseDmg, 995 },
                { Stats.MaximumPhysBaseDmg, 1120 },
                { Stats.DefenseBase, 648 },
                { Stats.AttackRatePvm, 900 },
                { Stats.DefenseRatePvm, 585 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 142 Red Skeleton Knight 7

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 143;
            monster.Designation = "Magic Skeleton 7";
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
                { Stats.Level, 140 },
                { Stats.MaximumHealth, 215000 },
                { Stats.MinimumPhysBaseDmg, 1500 },
                { Stats.MaximumPhysBaseDmg, 1780 },
                { Stats.DefenseBase, 670 },
                { Stats.AttackRatePvm, 950 },
                { Stats.DefenseRatePvm, 750 },
                { Stats.PoisonResistance, 10f / 255 },
                { Stats.IceResistance, 10f / 255 },
                { Stats.FireResistance, 10f / 255 },
                { Stats.LightningResistance, 10f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 143 Magic Skeleton 7
    }
}