// <copyright file="Noria.cs" company="MUnique">
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
    /// The initialization for the Noria map.
    /// </summary>
    internal class Noria : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the noria map.
        /// </summary>
        public static readonly byte Number = 3;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Noria";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[253], 1, 3, SpawnTrigger.Automatic, 169, 169, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[253], 1, 2, SpawnTrigger.Automatic, 193, 193, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[544], 1, 1, SpawnTrigger.Automatic, 187, 187, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[237], 1, 3, SpawnTrigger.Automatic, 171, 171, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[242], 1, 1, SpawnTrigger.Automatic, 173, 173, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[243], 1, 2, SpawnTrigger.Automatic, 195, 195, 124, 124);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 3, SpawnTrigger.Automatic, 172, 172, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[238], 1, 1, SpawnTrigger.Automatic, 180, 180, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[257], 1, 1, SpawnTrigger.Automatic, 167, 167, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[450], 1, 1, SpawnTrigger.Automatic, 179, 179, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[451], 1, 3, SpawnTrigger.Automatic, 179, 179, 129, 129);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[26], 155, 0, SpawnTrigger.Automatic, 128, 251, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[27], 125, 0, SpawnTrigger.Automatic, 128, 251, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[28], 125, 0, SpawnTrigger.Automatic, 0, 128, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[29], 125, 0, SpawnTrigger.Automatic, 0, 128, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[30], 125, 0, SpawnTrigger.Automatic, 0, 251, 128, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[31], 125, 0, SpawnTrigger.Automatic, 0, 251, 128, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[32], 100, 0, SpawnTrigger.Automatic, 128, 251, 128, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[33], 125, 0, SpawnTrigger.Automatic, 0, 128, 128, 245);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 26;
                monster.Designation = "Goblin";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 3 },
                    { Stats.MaximumHealth, 45 },
                    { Stats.MinimumPhysBaseDmg, 7 },
                    { Stats.MaximumPhysBaseDmg, 10 },
                    { Stats.DefenseBase, 2 },
                    { Stats.AttackRatePvm, 13 },
                    { Stats.DefenseRatePvm, 2 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 27;
                monster.Designation = "Chain Scorpion";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 5 },
                    { Stats.MaximumHealth, 80 },
                    { Stats.MinimumPhysBaseDmg, 13 },
                    { Stats.MaximumPhysBaseDmg, 17 },
                    { Stats.DefenseBase, 4 },
                    { Stats.AttackRatePvm, 23 },
                    { Stats.DefenseRatePvm, 4 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 28;
                monster.Designation = "Beetle Monster";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 10 },
                    { Stats.MaximumHealth, 165 },
                    { Stats.MinimumPhysBaseDmg, 26 },
                    { Stats.MaximumPhysBaseDmg, 31 },
                    { Stats.DefenseBase, 10 },
                    { Stats.AttackRatePvm, 44 },
                    { Stats.DefenseRatePvm, 10 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 29;
                monster.Designation = "Hunter";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 13 },
                    { Stats.MaximumHealth, 220 },
                    { Stats.MinimumPhysBaseDmg, 36 },
                    { Stats.MaximumPhysBaseDmg, 41 },
                    { Stats.DefenseBase, 13 },
                    { Stats.AttackRatePvm, 56 },
                    { Stats.DefenseRatePvm, 13 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 30;
                monster.Designation = "Forest Monster";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 15 },
                    { Stats.MaximumHealth, 295 },
                    { Stats.MinimumPhysBaseDmg, 46 },
                    { Stats.MaximumPhysBaseDmg, 51 },
                    { Stats.DefenseBase, 15 },
                    { Stats.AttackRatePvm, 68 },
                    { Stats.DefenseRatePvm, 15 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 31;
                monster.Designation = "Agon";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 16 },
                    { Stats.MaximumHealth, 340 },
                    { Stats.MinimumPhysBaseDmg, 51 },
                    { Stats.MaximumPhysBaseDmg, 57 },
                    { Stats.DefenseBase, 16 },
                    { Stats.AttackRatePvm, 74 },
                    { Stats.DefenseRatePvm, 16 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 32;
                monster.Designation = "Stone Golem";
                monster.MoveRange = 2;
                monster.AttackRange = 2;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 18 },
                    { Stats.MaximumHealth, 465 },
                    { Stats.MinimumPhysBaseDmg, 62 },
                    { Stats.MaximumPhysBaseDmg, 68 },
                    { Stats.DefenseBase, 20 },
                    { Stats.AttackRatePvm, 86 },
                    { Stats.DefenseRatePvm, 20 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
                monster.Number = 33;
                monster.Designation = "Elite Goblin";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 8 },
                    { Stats.MaximumHealth, 120 },
                    { Stats.MinimumPhysBaseDmg, 19 },
                    { Stats.MaximumPhysBaseDmg, 23 },
                    { Stats.DefenseBase, 8 },
                    { Stats.AttackRatePvm, 33 },
                    { Stats.DefenseRatePvm, 8 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
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
