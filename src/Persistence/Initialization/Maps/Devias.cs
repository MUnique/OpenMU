// <copyright file="Devias.cs" company="MUnique">
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
    /// The initialization for the Devias map.
    /// </summary>
    internal class Devias : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 2;

        /// <inheritdoc/>
        protected override string MapName => "Devias";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[540], 1, Direction.SouthWest, SpawnTrigger.Automatic, 233, 233, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[406], 1, Direction.SouthEast, SpawnTrigger.Automatic, 181, 181, 35, 35);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[385], 1, Direction.SouthWest, SpawnTrigger.Automatic, 197, 197, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[478], 1, Direction.SouthEast, SpawnTrigger.Automatic, 191, 191, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[256], 1, Direction.SouthEast, SpawnTrigger.Automatic, 193, 193, 13, 13);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[522], 1, Direction.SouthWest, SpawnTrigger.Automatic, 229, 229, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[229], 1, Direction.SouthEast, SpawnTrigger.Automatic, 183, 183, 30, 30);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[233], 1, Direction.SouthEast, SpawnTrigger.Automatic, 217, 217, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[233], 1, Direction.SouthEast, SpawnTrigger.Automatic, 217, 217, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[235], 1, Direction.SouthEast, SpawnTrigger.Automatic, 183, 183, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[244], 1, Direction.SouthEast, SpawnTrigger.Automatic, 226, 226, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[245], 1, Direction.SouthEast, SpawnTrigger.Automatic, 225, 225, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[246], 1, Direction.SouthEast, SpawnTrigger.Automatic, 186, 186, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[247], 1, Direction.NorthEast, SpawnTrigger.Automatic, 224, 224, 79, 79);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[247], 1, Direction.NorthEast, SpawnTrigger.Automatic, 219, 219, 79, 79);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[247], 1, Direction.NorthWest, SpawnTrigger.Automatic, 169, 169, 45, 45);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[247], 1, Direction.NorthWest, SpawnTrigger.Automatic, 169, 169, 39, 39);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[241], 1, Direction.SouthWest, SpawnTrigger.Automatic, 215, 215, 45, 45);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.South, SpawnTrigger.Automatic, 218, 218, 63, 63);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[257], 1, Direction.SouthEast, SpawnTrigger.Automatic, 219, 219, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[566], 1, Direction.SouthEast, SpawnTrigger.Automatic, 204, 204, 61, 61);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[379], 1, Direction.SouthEast, SpawnTrigger.Automatic, 13, 13, 28, 28);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[20], 10, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 165, 165);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[20], 10, Direction.Undefined, SpawnTrigger.Automatic, 36, 36, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[20], 15, Direction.Undefined, SpawnTrigger.Automatic, 210, 242, 210, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[20], 200, Direction.Undefined, SpawnTrigger.Automatic, 0, 251, 128, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[25], 75, Direction.Undefined, SpawnTrigger.Automatic, 0, 128, 128, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[23], 75, Direction.Undefined, SpawnTrigger.Automatic, 0, 128, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[22], 75, Direction.Undefined, SpawnTrigger.Automatic, 0, 128, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[24], 65, Direction.Undefined, SpawnTrigger.Automatic, 128, 251, 0, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[21], 35, Direction.Undefined, SpawnTrigger.Automatic, 128, 251, 0, 128);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 19;
                monster.Designation = "Yeti";
                monster.MoveRange = 2;
                monster.AttackRange = 4;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 30 },
                    { Stats.MaximumHealth, 900 },
                    { Stats.MinimumPhysBaseDmg, 105 },
                    { Stats.MaximumPhysBaseDmg, 110 },
                    { Stats.DefenseBase, 37 },
                    { Stats.AttackRatePvm, 150 },
                    { Stats.DefenseRatePvm, 37 },
                    { Stats.IceResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 20;
                monster.Designation = "Elite Yeti";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 36 },
                    { Stats.MaximumHealth, 1200 },
                    { Stats.MinimumPhysBaseDmg, 120 },
                    { Stats.MaximumPhysBaseDmg, 125 },
                    { Stats.DefenseBase, 50 },
                    { Stats.AttackRatePvm, 180 },
                    { Stats.DefenseRatePvm, 43 },
                    { Stats.PoisonResistance, 1 },
                    { Stats.IceResistance, 4 },
                    { Stats.WaterResistance, 1 },
                    { Stats.FireResistance, 1 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 21;
                monster.Designation = "Assassin";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 26 },
                    { Stats.MaximumHealth, 800 },
                    { Stats.MinimumPhysBaseDmg, 95 },
                    { Stats.MaximumPhysBaseDmg, 100 },
                    { Stats.DefenseBase, 33 },
                    { Stats.AttackRatePvm, 130 },
                    { Stats.DefenseRatePvm, 33 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 22;
                monster.Designation = "Ice Monster";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 22 },
                    { Stats.MaximumHealth, 650 },
                    { Stats.MinimumPhysBaseDmg, 80 },
                    { Stats.MaximumPhysBaseDmg, 85 },
                    { Stats.DefenseBase, 27 },
                    { Stats.AttackRatePvm, 110 },
                    { Stats.DefenseRatePvm, 27 },
                    { Stats.IceResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 23;
                monster.Designation = "Hommerd";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 24 },
                    { Stats.MaximumHealth, 700 },
                    { Stats.MinimumPhysBaseDmg, 85 },
                    { Stats.MaximumPhysBaseDmg, 90 },
                    { Stats.DefenseBase, 29 },
                    { Stats.AttackRatePvm, 120 },
                    { Stats.DefenseRatePvm, 29 },
                    { Stats.IceResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 24;
                monster.Designation = "Worm";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 20 },
                    { Stats.MaximumHealth, 600 },
                    { Stats.MinimumPhysBaseDmg, 75 },
                    { Stats.MaximumPhysBaseDmg, 80 },
                    { Stats.DefenseBase, 25 },
                    { Stats.AttackRatePvm, 100 },
                    { Stats.DefenseRatePvm, 25 },
                    { Stats.IceResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 25;
                monster.Designation = "Ice Queen";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(50 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 52 },
                    { Stats.MaximumHealth, 4000 },
                    { Stats.MinimumPhysBaseDmg, 155 },
                    { Stats.MaximumPhysBaseDmg, 165 },
                    { Stats.DefenseBase, 90 },
                    { Stats.AttackRatePvm, 260 },
                    { Stats.DefenseRatePvm, 76 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 5 },
                    { Stats.WaterResistance, 4 },
                    { Stats.FireResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
