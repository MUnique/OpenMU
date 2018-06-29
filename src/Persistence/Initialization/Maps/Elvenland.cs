// <copyright file="Elvenland.cs" company="MUnique">
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
    /// The initialization for the Elvenland map.
    /// </summary>
    internal class Elvenland : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the elvenland map.
        /// </summary>
        public const byte Number = 51;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Elvenland";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[256], 1, 1, SpawnTrigger.Automatic, 37, 37, 242, 242); // Lahap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[415], 1, 1, SpawnTrigger.Automatic, 44, 44, 229, 229); // Silvia
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[416], 1, 2, SpawnTrigger.Automatic, 29, 29, 237, 237); // Rhea
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[417], 1, 1, SpawnTrigger.Automatic, 37, 37, 218, 218); // Marce
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[257], 1, 3, SpawnTrigger.Automatic, 44, 44, 189, 189); // Shadow Phantom Soldier
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[257], 1, 1, SpawnTrigger.Automatic, 57, 57, 231, 231); // Shadow Phantom Soldier
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[257], 1, 1, SpawnTrigger.Automatic, 74, 74, 219, 220); // Shadow Phantom Soldier
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 1, SpawnTrigger.Automatic, 51, 51, 229, 229); // Safety Guardian
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[385], 1, 1, SpawnTrigger.Automatic, 55, 55, 243, 243); // Mirage
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[452], 1, 1, SpawnTrigger.Automatic, 45, 45, 243, 243); // Seed Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[453], 1, 1, SpawnTrigger.Automatic, 49, 49, 243, 243); // Seed Researcher
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[540], 1, 2, SpawnTrigger.Automatic, 49, 49, 216, 216); // Lugard
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[492], 1, 2, SpawnTrigger.Automatic, 22, 22, 225, 225); // Moss
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[579], 1, 3, SpawnTrigger.Automatic, 20, 20, 214, 214); // David
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[568], 1, 2, SpawnTrigger.Automatic, 55, 55, 199, 199); // Wandering Merchant Zyro

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[418], 80, 0, SpawnTrigger.Automatic, 0, 128, 128, 245); // Strange Rabbit
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[419], 45, 0, SpawnTrigger.Automatic, 0, 251, 128, 245); // Polluted Butterfly
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[420], 45, 0, SpawnTrigger.Automatic, 0, 128, 0, 128); // Hideous Rabbit
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[421], 30, 0, SpawnTrigger.Automatic, 0, 128, 0, 128); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[422], 30, 0, SpawnTrigger.Automatic, 128, 251, 0, 128); // Cursed Lich
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[423], 20, 0, SpawnTrigger.Automatic, 128, 251, 0, 128); // Totem Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[424], 20, 0, SpawnTrigger.Automatic, 128, 251, 0, 128); // Grizzly
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[425], 20, 0, SpawnTrigger.Automatic, 128, 251, 0, 128); // Captain Grizzly
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 418;
                monster.Designation = "Strange Rabbit";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 4 },
                    { Stats.MaximumHealth, 60 },
                    { Stats.MinimumPhysBaseDmg, 10 },
                    { Stats.MaximumPhysBaseDmg, 13 },
                    { Stats.DefenseBase, 3 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 3 },
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
                monster.Number = 419;
                monster.Designation = "Polluted Butterfly";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 13 },
                    { Stats.MaximumHealth, 230 },
                    { Stats.MinimumPhysBaseDmg, 37 },
                    { Stats.MaximumPhysBaseDmg, 42 },
                    { Stats.DefenseBase, 13 },
                    { Stats.AttackRatePvm, 10 },
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
                monster.Number = 420;
                monster.Designation = "Hideous Rabbit";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 19 },
                    { Stats.MaximumHealth, 520 },
                    { Stats.MinimumPhysBaseDmg, 68 },
                    { Stats.MaximumPhysBaseDmg, 72 },
                    { Stats.DefenseBase, 22 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 22 },
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
                monster.Number = 421;
                monster.Designation = "Werewolf";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 24 },
                    { Stats.MaximumHealth, 720 },
                    { Stats.MinimumPhysBaseDmg, 85 },
                    { Stats.MaximumPhysBaseDmg, 90 },
                    { Stats.DefenseBase, 30 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
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
                monster.Number = 422;
                monster.Designation = "Cursed Lich";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 30 },
                    { Stats.MaximumHealth, 900 },
                    { Stats.MinimumPhysBaseDmg, 105 },
                    { Stats.MaximumPhysBaseDmg, 110 },
                    { Stats.DefenseBase, 33 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 33 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.WaterResistance, 4 },
                    { Stats.FireResistance, 4 },
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
                monster.Number = 423;
                monster.Designation = "Totem Golem";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 36 },
                    { Stats.MaximumHealth, 1200 },
                    { Stats.MinimumPhysBaseDmg, 120 },
                    { Stats.MaximumPhysBaseDmg, 125 },
                    { Stats.DefenseBase, 50 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 50 },
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
                monster.Number = 424;
                monster.Designation = "Grizzly";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 43 },
                    { Stats.MaximumHealth, 2400 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 145 },
                    { Stats.DefenseBase, 65 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 65 },
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
                monster.Number = 425;
                monster.Designation = "Captain Grizzly";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 48 },
                    { Stats.MaximumHealth, 3000 },
                    { Stats.MinimumPhysBaseDmg, 150 },
                    { Stats.MaximumPhysBaseDmg, 155 },
                    { Stats.DefenseBase, 80 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 70 },
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