// <copyright file="BalgassRefuge.cs" company="MUnique">
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
    /// The initialization for the Barracks of Balgass map.
    /// </summary>
    internal class BalgassRefuge : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the barracks of balgass map.
        /// </summary>
        public const byte Number = 42;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Balgass Refuge";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, 0, SpawnTrigger.Automatic, 104, 104, 179, 179); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, 0, SpawnTrigger.Automatic, 104, 104, 200, 200); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, 0, SpawnTrigger.Automatic, 085, 085, 179, 179); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, 0, SpawnTrigger.Automatic, 086, 086, 199, 199); // Death Spirit (Trainee Soldier)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, 0, SpawnTrigger.Automatic, 092, 092, 174, 174); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, 0, SpawnTrigger.Automatic, 111, 111, 190, 190); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, 0, SpawnTrigger.Automatic, 094, 094, 202, 202); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, 0, SpawnTrigger.Automatic, 082, 082, 190, 190); // Soram (Trainee Soldier)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[412], 1, 0, SpawnTrigger.Automatic, 097, 097, 187, 187); // Dark Elf (Trainee Soldier)
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 412;
                monster.Designation = "Dark Elf (Trainee Soldier)";
                monster.MoveRange = 6;
                monster.AttackRange = 6;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 128 },
                    { Stats.MaximumHealth, 1500000 },
                    { Stats.MinimumPhysBaseDmg, 800 },
                    { Stats.MaximumPhysBaseDmg, 900 },
                    { Stats.DefenseBase, 900 },
                    { Stats.AttackRatePvm, 1500 },
                    { Stats.DefenseRatePvm, 400 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 23 },
                    { Stats.LightningResistance, 23 },
                    { Stats.FireResistance, 23 },
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
