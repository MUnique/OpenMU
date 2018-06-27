// <copyright file="Karutan1.cs" company="MUnique">
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
    /// The initialization for the Karutan1 map.
    /// </summary>
    internal class Karutan1 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the karutan 1 map.
        /// </summary>
        public const byte Number = 80;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Karutan1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[577], 1, 3, SpawnTrigger.Automatic, 121, 121, 102, 102); // Leina the General Goods Merchant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[578], 1, 3, SpawnTrigger.Automatic, 117, 117, 126, 126); // Weapons Merchant Bolo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 2, SpawnTrigger.Automatic, 123, 123, 132, 132); // Safety Guardian
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 3, SpawnTrigger.Automatic, 122, 122, 096, 096); // Safety Guardian
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 1, SpawnTrigger.Automatic, 158, 158, 126, 126); // Safety Guardian

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 35, 0, SpawnTrigger.Automatic, 000, 255, 000, 255); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 47, 0, SpawnTrigger.Automatic, 000, 255, 000, 255); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 10, 0, SpawnTrigger.Automatic, 000, 255, 000, 255); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 18, 0, SpawnTrigger.Automatic, 000, 255, 000, 255); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 13, 0, SpawnTrigger.Automatic, 000, 255, 000, 255); // Crypos
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 569;
                monster.Designation = "Venomous Chain Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 99 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 555 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 445 },
                    { Stats.AttackRatePvm, 845 },
                    { Stats.DefenseRatePvm, 248 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 200 },
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
                monster.Number = 570;
                monster.Designation = "Bone Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 103 },
                    { Stats.MaximumHealth, 60000 },
                    { Stats.MinimumPhysBaseDmg, 595 },
                    { Stats.MaximumPhysBaseDmg, 635 },
                    { Stats.DefenseBase, 283 },
                    { Stats.AttackRatePvm, 915 },
                    { Stats.DefenseRatePvm, 363 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 200 },
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
                monster.Number = 571;
                monster.Designation = "Orcus";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 65000 },
                    { Stats.MinimumPhysBaseDmg, 618 },
                    { Stats.MaximumPhysBaseDmg, 655 },
                    { Stats.DefenseBase, 518 },
                    { Stats.AttackRatePvm, 965 },
                    { Stats.DefenseRatePvm, 293 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 100 },
                    { Stats.IceResistance, 100 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 240 },
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
                monster.Number = 573;
                monster.Designation = "Crypta";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 111 },
                    { Stats.MaximumHealth, 78000 },
                    { Stats.MinimumPhysBaseDmg, 705 },
                    { Stats.MaximumPhysBaseDmg, 755 },
                    { Stats.DefenseBase, 560 },
                    { Stats.AttackRatePvm, 1080 },
                    { Stats.DefenseRatePvm, 340 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
                monster.Number = 574;
                monster.Designation = "Crypos";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 83000 },
                    { Stats.MinimumPhysBaseDmg, 720 },
                    { Stats.MaximumPhysBaseDmg, 770 },
                    { Stats.DefenseBase, 575 },
                    { Stats.AttackRatePvm, 11400 },
                    { Stats.DefenseRatePvm, 375 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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