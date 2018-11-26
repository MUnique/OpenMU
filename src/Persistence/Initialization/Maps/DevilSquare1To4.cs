// <copyright file="DevilSquare1To4.cs" company="MUnique">
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
    /// Initialization for the devil square map which hosts devil square 1 to 4.
    /// </summary>
    internal class DevilSquare1To4 : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square (1-4)";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 119, 150, 80, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 119, 150, 80, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 121, 151, 152, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 121, 151, 152, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 49, 79, 138, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 49, 79, 138, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[64], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 53, 83, 74, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[65], 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 53, 83, 74, 109);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 64;
                monster.Designation = "Orc Archer";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 10000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 250 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 350 },
                    { Stats.DefenseRatePvm, 115 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
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
                monster.Number = 65;
                monster.Designation = "Elite Orc";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 14000 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 8 },
                    { Stats.FireResistance, 8 },
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
