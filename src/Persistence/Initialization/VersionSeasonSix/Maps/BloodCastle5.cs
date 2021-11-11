﻿// <copyright file="BloodCastle5.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Initialization for the Blood Castle 5.
/// </summary>
internal class BloodCastle5 : BloodCastleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BloodCastle5"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BloodCastle5(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 15;

    /// <inheritdoc/>
    protected override string MapName => "Blood Castle 5";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[119], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 5

        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[120], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 5

        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[121], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 5

        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[122], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 5

        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[123], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 5

        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
        yield return this.CreateMonsterSpawn(this.NpcDictionary[124], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 5
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 119;
            monster.Designation = "Chief Skeleton Warrior 5";
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
                { Stats.Level, 85 },
                { Stats.MaximumHealth, 23000 },
                { Stats.MinimumPhysBaseDmg, 340 },
                { Stats.MaximumPhysBaseDmg, 390 },
                { Stats.DefenseBase, 340 },
                { Stats.AttackRatePvm, 520 },
                { Stats.DefenseRatePvm, 190 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 119 Chief Skeleton Warrior 5

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 120;
            monster.Designation = "Chief Skeleton Archer 5";
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
                { Stats.Level, 87 },
                { Stats.MaximumHealth, 26000 },
                { Stats.MinimumPhysBaseDmg, 390 },
                { Stats.MaximumPhysBaseDmg, 430 },
                { Stats.DefenseBase, 380 },
                { Stats.AttackRatePvm, 590 },
                { Stats.DefenseRatePvm, 200 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 120 Chief Skeleton Archer 5

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 121;
            monster.Designation = "Dark Skull Soldier 5";
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
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 32000 },
                { Stats.MinimumPhysBaseDmg, 430 },
                { Stats.MaximumPhysBaseDmg, 480 },
                { Stats.DefenseBase, 400 },
                { Stats.AttackRatePvm, 660 },
                { Stats.DefenseRatePvm, 230 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 121 Dark Skull Soldier 5

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 122;
            monster.Designation = "Giant Ogre 5";
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
                { Stats.Level, 93 },
                { Stats.MaximumHealth, 37000 },
                { Stats.MinimumPhysBaseDmg, 470 },
                { Stats.MaximumPhysBaseDmg, 520 },
                { Stats.DefenseBase, 440 },
                { Stats.AttackRatePvm, 730 },
                { Stats.DefenseRatePvm, 250 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 122 Giant Ogre 5

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 123;
            monster.Designation = "Red Skeleton Knight 5";
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
                { Stats.Level, 98 },
                { Stats.MaximumHealth, 45000 },
                { Stats.MinimumPhysBaseDmg, 510 },
                { Stats.MaximumPhysBaseDmg, 560 },
                { Stats.DefenseBase, 490 },
                { Stats.AttackRatePvm, 800 },
                { Stats.DefenseRatePvm, 270 },
                { Stats.PoisonResistance, 7f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.FireResistance, 7f / 255 },
                { Stats.LightningResistance, 7f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 123 Red Skeleton Knight 5

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 124;
            monster.Designation = "Magic Skeleton 5";
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
                { Stats.Level, 102 },
                { Stats.MaximumHealth, 55000 },
                { Stats.MinimumPhysBaseDmg, 560 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 550 },
                { Stats.AttackRatePvm, 870 },
                { Stats.DefenseRatePvm, 310 },
                { Stats.PoisonResistance, 10f / 255 },
                { Stats.IceResistance, 10f / 255 },
                { Stats.FireResistance, 10f / 255 },
                { Stats.LightningResistance, 10f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        } // 124 Magic Skeleton 5
    }
}