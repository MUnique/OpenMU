// <copyright file="Icarus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The initialization for the Icarus map.
    /// </summary>
    internal class Icarus : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Icarus"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Icarus(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 10;

        /// <inheritdoc/>
        protected override string MapName => "Icarus";

        /// <inheritdoc/>
        protected override void CreateMapAttributeRequirements()
        {
            this.CreateRequirement(Stats.CanFly, 1);
        }

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 66, 68);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 85, 88);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 80, 76);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 70, 78);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 45, 72);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 60, 84);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 41, 113);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 42, 135);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 29, 104);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 38, 103);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 35, 94);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 32, 87);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 35, 69);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 26, 74);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 36, 79);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 39, 85);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 51, 126);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 88, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 53, 138);
            yield return this.CreateMonsterSpawn(npcDictionary[73], 55, 73);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 44, 148);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 33, 119);
            yield return this.CreateMonsterSpawn(npcDictionary[74], 33, 111);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 84, 47);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 79, 33);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 58, 42);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 89, 38);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 70, 43);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 64, 30);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 53, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[70], 91, 59);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 41, 157);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 21, 39);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 44, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 12, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 13, 38);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 48, 40);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 42, 122);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 28, 28);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 93, 47);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 53, 150);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 50, 162);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 52, 174);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 55, 181);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 44, 212);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 38, 216);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 47, 189);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 59, 190);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 48, 200);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 57, 200);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 27, 220);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 66, 37);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 80, 41);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 43, 238);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 25, 231);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 44, 221);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 40, 34);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 31, 37);
            yield return this.CreateMonsterSpawn(npcDictionary[69], 18, 31);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 57, 34);
            yield return this.CreateMonsterSpawn(npcDictionary[71], 71, 33);
            yield return this.CreateMonsterSpawn(npcDictionary[72], 36, 226);
            yield return this.CreateMonsterSpawn(npcDictionary[75], 48, 228);
            yield return this.CreateMonsterSpawn(npcDictionary[77], 34, 238);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 69;
                monster.Designation = "Alquamos";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 11500 },
                    { Stats.MinimumPhysBaseDmg, 255 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 195 },
                    { Stats.AttackRatePvm, 385 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 11 },
                    { Stats.FireResistance, 9 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 70;
                monster.Designation = "Queen Rainer";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 305 },
                    { Stats.MaximumPhysBaseDmg, 350 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 475 },
                    { Stats.DefenseRatePvm, 160 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 11 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 9 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 71;
                monster.Designation = "Mega Crust";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 78 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 270 },
                    { Stats.MaximumPhysBaseDmg, 320 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 140 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 9 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 72;
                monster.Designation = "Phantom Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 96 },
                    { Stats.MaximumHealth, 41000 },
                    { Stats.MinimumPhysBaseDmg, 560 },
                    { Stats.MaximumPhysBaseDmg, 610 },
                    { Stats.DefenseBase, 425 },
                    { Stats.AttackRatePvm, 690 },
                    { Stats.DefenseRatePvm, 270 },
                    { Stats.PoisonResistance, 14 },
                    { Stats.IceResistance, 14 },
                    { Stats.WaterResistance, 16 },
                    { Stats.FireResistance, 14 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 73;
                monster.Designation = "Drakan";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(20 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 86 },
                    { Stats.MaximumHealth, 29000 },
                    { Stats.MinimumPhysBaseDmg, 425 },
                    { Stats.MaximumPhysBaseDmg, 480 },
                    { Stats.DefenseBase, 305 },
                    { Stats.AttackRatePvm, 570 },
                    { Stats.DefenseRatePvm, 210 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 13 },
                    { Stats.FireResistance, 12 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 74;
                monster.Designation = "Alpha Crust";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 92 },
                    { Stats.MaximumHealth, 34500 },
                    { Stats.MinimumPhysBaseDmg, 489 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 360 },
                    { Stats.AttackRatePvm, 620 },
                    { Stats.DefenseRatePvm, 240 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.WaterResistance, 14 },
                    { Stats.FireResistance, 13 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 75;
                monster.Designation = "Great Drakan";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 650 },
                    { Stats.MaximumPhysBaseDmg, 700 },
                    { Stats.DefenseBase, 495 },
                    { Stats.AttackRatePvm, 800 },
                    { Stats.DefenseRatePvm, 305 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.WaterResistance, 17 },
                    { Stats.FireResistance, 18 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 76;
                monster.Designation = "Dark Phoenix Shield";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 106 },
                    { Stats.MaximumHealth, 73000 },
                    { Stats.MinimumPhysBaseDmg, 850 },
                    { Stats.MaximumPhysBaseDmg, 960 },
                    { Stats.DefenseBase, 580 },
                    { Stats.AttackRatePvm, 840 },
                    { Stats.DefenseRatePvm, 305 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                    { Stats.WaterResistance, 35 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 77;
                monster.Designation = "Dark Phoenix";
                monster.MoveRange = 1;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(15 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 108 },
                    { Stats.MaximumHealth, 95000 },
                    { Stats.MinimumPhysBaseDmg, 950 },
                    { Stats.MaximumPhysBaseDmg, 1000 },
                    { Stats.DefenseBase, 600 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 350 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                    { Stats.WaterResistance, 35 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
