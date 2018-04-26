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
        /// <inheritdoc/>
        protected override byte MapNumber => 10;

        /// <inheritdoc/>
        protected override string MapName => "Icarus";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 66, 66, 68, 68);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 85, 85, 88, 88);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 80, 80, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 70, 70, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 45, 45, 72, 72);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 60, 60, 84, 84);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 41, 41, 113, 113);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 42, 42, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 29, 29, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 38, 38, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 35, 35, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 32, 32, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 35, 35, 69, 69);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 26, 26, 74, 74);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 36, 36, 79, 79);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 39, 39, 85, 85);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 51, 51, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 88, 88, 70, 70);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 53, 53, 138, 138);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[73], 1, 0, SpawnTrigger.Automatic, 55, 55, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 44, 44, 148, 148);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 33, 33, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[74], 1, 0, SpawnTrigger.Automatic, 33, 33, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 84, 84, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 79, 79, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 58, 58, 42, 42);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 89, 89, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 70, 70, 43, 43);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 64, 64, 30, 30);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 53, 53, 27, 27);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[70], 1, 0, SpawnTrigger.Automatic, 91, 91, 59, 59);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 41, 41, 157, 157);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 21, 21, 39, 39);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 44, 44, 27, 27);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 12, 12, 27, 27);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 13, 13, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 48, 48, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 42, 42, 122, 122);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 28, 28, 28, 28);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 93, 93, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 53, 53, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 50, 50, 162, 162);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 52, 52, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 55, 55, 181, 181);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 44, 44, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 38, 38, 216, 216);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 47, 47, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 59, 59, 190, 190);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 48, 48, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 57, 57, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 27, 27, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 66, 66, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 80, 80, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 43, 43, 238, 238);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 25, 25, 231, 231);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 44, 44, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 40, 40, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 31, 31, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[69], 1, 0, SpawnTrigger.Automatic, 18, 18, 31, 31);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 57, 57, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[71], 1, 0, SpawnTrigger.Automatic, 71, 71, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[72], 1, 0, SpawnTrigger.Automatic, 36, 36, 226, 226);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[75], 1, 0, SpawnTrigger.Automatic, 48, 48, 228, 228);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[77], 1, 0, SpawnTrigger.Automatic, 34, 34, 238, 238);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 11500 },
                    { Stats.MinimumPhysBaseDmg, 255 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 195 },
                    { Stats.AttackRatePvm, 385 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 11 },
                    { Stats.FireResistance, 9 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 305 },
                    { Stats.MaximumPhysBaseDmg, 350 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 475 },
                    { Stats.DefenseRatePvm, 160 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 11 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 9 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 78 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 270 },
                    { Stats.MaximumPhysBaseDmg, 320 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 140 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 9 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 96 },
                    { Stats.MaximumHealth, 41000 },
                    { Stats.MinimumPhysBaseDmg, 560 },
                    { Stats.MaximumPhysBaseDmg, 610 },
                    { Stats.DefenseBase, 425 },
                    { Stats.AttackRatePvm, 690 },
                    { Stats.DefenseRatePvm, 270 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 14 },
                    { Stats.IceResistance, 14 },
                    { Stats.WaterResistance, 16 },
                    { Stats.FireResistance, 14 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 86 },
                    { Stats.MaximumHealth, 29000 },
                    { Stats.MinimumPhysBaseDmg, 425 },
                    { Stats.MaximumPhysBaseDmg, 480 },
                    { Stats.DefenseBase, 305 },
                    { Stats.AttackRatePvm, 570 },
                    { Stats.DefenseRatePvm, 210 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 13 },
                    { Stats.FireResistance, 12 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 92 },
                    { Stats.MaximumHealth, 34500 },
                    { Stats.MinimumPhysBaseDmg, 489 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 360 },
                    { Stats.AttackRatePvm, 620 },
                    { Stats.DefenseRatePvm, 240 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.WaterResistance, 14 },
                    { Stats.FireResistance, 13 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 650 },
                    { Stats.MaximumPhysBaseDmg, 700 },
                    { Stats.DefenseBase, 495 },
                    { Stats.AttackRatePvm, 800 },
                    { Stats.DefenseRatePvm, 305 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.WaterResistance, 17 },
                    { Stats.FireResistance, 18 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 106 },
                    { Stats.MaximumHealth, 73000 },
                    { Stats.MinimumPhysBaseDmg, 850 },
                    { Stats.MaximumPhysBaseDmg, 960 },
                    { Stats.DefenseBase, 580 },
                    { Stats.AttackRatePvm, 840 },
                    { Stats.DefenseRatePvm, 305 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                    { Stats.WaterResistance, 35 },
                    { Stats.FireResistance, 30 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 108 },
                    { Stats.MaximumHealth, 95000 },
                    { Stats.MinimumPhysBaseDmg, 950 },
                    { Stats.MaximumPhysBaseDmg, 1000 },
                    { Stats.DefenseBase, 600 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 350 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                    { Stats.WaterResistance, 35 },
                    { Stats.FireResistance, 30 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }
        }
    }
}
