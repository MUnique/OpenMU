// <copyright file="BloodCastle3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// Initialization for the Blood Castle 3.
    /// </summary>
    internal class BloodCastle3 : BloodCastleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BloodCastle3"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public BloodCastle3(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 13;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 3";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 034, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 038, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 013, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 013, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 035, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 042, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 013, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 013, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 040, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 013, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 027, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 024, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 014, 032, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[096], 015, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Warrior 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 045, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 023, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 039, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 037, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 029, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 028, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 025, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 014, 022, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 031, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 030, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 033, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 014, 021, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 013, 043, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[097], 015, 026, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chief Skeleton Archer 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 059, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 041, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 046, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 014, 044, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 014, 049, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 014, 064, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 014, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 053, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 065, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 015, 060, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[098], 013, 057, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Dark Skull Soldier 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 013, 069, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 014, 061, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 013, 055, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 014, 058, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 014, 052, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 015, 050, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 015, 066, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 014, 056, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 014, 070, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 013, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[099], 013, 063, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Giant Ogre 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 013, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 015, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 016, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 015, 062, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 017, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 011, 083, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 015, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 020, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 020, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 054, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 036, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 015, 068, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[111], 014, 047, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Red Skeleton Knight 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 016, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 012, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 013, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 013, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 019, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 013, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 014, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[112], 018, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Magic Skeleton 3
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 96;
                monster.Designation = "Chief Skeleton Warrior 3";
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
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 12000 },
                    { Stats.MinimumPhysBaseDmg, 210 },
                    { Stats.MaximumPhysBaseDmg, 270 },
                    { Stats.DefenseBase, 185 },
                    { Stats.AttackRatePvm, 380 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 096 Chief Skeleton Warrior 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 97;
                monster.Designation = "Chief Skeleton Archer 3";
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
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 240 },
                    { Stats.MaximumPhysBaseDmg, 300 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 420 },
                    { Stats.DefenseRatePvm, 130 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 097 Chief Skeleton Archer 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 98;
                monster.Designation = "Dark Skull Soldier 3";
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
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 280 },
                    { Stats.MaximumPhysBaseDmg, 340 },
                    { Stats.DefenseBase, 220 },
                    { Stats.AttackRatePvm, 470 },
                    { Stats.DefenseRatePvm, 175 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 098 Dark Skull Soldier 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 99;
                monster.Designation = "Giant Ogre 3";
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
                    { Stats.Level, 85 },
                    { Stats.MaximumHealth, 23000 },
                    { Stats.MinimumPhysBaseDmg, 340 },
                    { Stats.MaximumPhysBaseDmg, 390 },
                    { Stats.DefenseBase, 300 },
                    { Stats.AttackRatePvm, 520 },
                    { Stats.DefenseRatePvm, 190 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 099 Giant Ogre 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 111;
                monster.Designation = "Red Skeleton Knight 3";
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
                    { Stats.Level, 87 },
                    { Stats.MaximumHealth, 26000 },
                    { Stats.MinimumPhysBaseDmg, 370 },
                    { Stats.MaximumPhysBaseDmg, 410 },
                    { Stats.DefenseBase, 400 },
                    { Stats.AttackRatePvm, 650 },
                    { Stats.DefenseRatePvm, 200 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 111 Red Skeleton Knight 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 112;
                monster.Designation = "Magic Skeleton 3";
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
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 32000 },
                    { Stats.MinimumPhysBaseDmg, 400 },
                    { Stats.MaximumPhysBaseDmg, 460 },
                    { Stats.DefenseBase, 450 },
                    { Stats.AttackRatePvm, 720 },
                    { Stats.DefenseRatePvm, 260 },
                    { Stats.PoisonResistance, 8f / 255 },
                    { Stats.IceResistance, 8f / 255 },
                    { Stats.FireResistance, 8f / 255 },
                    { Stats.LightningResistance, 8f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 112 Magic Skeleton 3
        }
    }
}
