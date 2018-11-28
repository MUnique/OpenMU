// <copyright file="BarracksOfBalgass.cs" company="MUnique">
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
    internal class BarracksOfBalgass : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the barracks of balgass map.
        /// </summary>
        public static readonly byte Number = 41;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Barracks of Balgass";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[408], 1, Direction.South, SpawnTrigger.Automatic, 119, 119, 168, 168); // Gatekeeper

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 039, 101, 101); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 110, 110); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 134, 134); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 112, 112); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 090, 090); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 090, 090, 067, 067); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 090, 090, 078, 078); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 101, 101); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 132, 132); // Balram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[409], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 101, 101); // Balram (Trainee Soldier)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 098, 098, 096, 096); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 090, 090); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 099, 099); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 140, 140); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 150, 150); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 127, 127); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 123, 123); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 079, 079, 086, 086); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 069, 069); // Death Spirit (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[410], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 118, 118); // Death Spirit (Trainee Soldier)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 127, 127); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 166, 164, 164); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 160, 160); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 156, 156); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 136, 136); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 103, 103); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 146, 146); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 098, 098); // Soram (Trainee Soldier)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[411], 1, Direction.Undefined, SpawnTrigger.Automatic, 113, 113, 109, 109); // Soram (Trainee Soldier)
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 409;
                monster.Designation = "Balram (Trainee Soldier)";
                monster.MoveRange = 6;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 75000 },
                    { Stats.MinimumPhysBaseDmg, 550 },
                    { Stats.MaximumPhysBaseDmg, 650 },
                    { Stats.DefenseBase, 480 },
                    { Stats.AttackRatePvm, 1200 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.LightningResistance, 23 },
                    { Stats.FireResistance, 23 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 410;
                monster.Designation = "Death Spirit (Trainee Soldier)";
                monster.MoveRange = 6;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 119 },
                    { Stats.MaximumHealth, 85000 },
                    { Stats.MinimumPhysBaseDmg, 580 },
                    { Stats.MaximumPhysBaseDmg, 680 },
                    { Stats.DefenseBase, 490 },
                    { Stats.AttackRatePvm, 1200 },
                    { Stats.DefenseRatePvm, 390 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.LightningResistance, 25 },
                    { Stats.FireResistance, 25 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 411;
                monster.Designation = "Soram (Trainee Soldier)";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 119 },
                    { Stats.MaximumHealth, 90000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 700 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 1200 },
                    { Stats.DefenseRatePvm, 390 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.LightningResistance, 25 },
                    { Stats.FireResistance, 25 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
