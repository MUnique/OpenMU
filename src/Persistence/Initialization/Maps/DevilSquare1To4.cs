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
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare1To4"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare1To4(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square (1-4)";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            yield return this.CreateMonsterSpawn(npcDictionary[17], 119, 150, 80, 115, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[15], 119, 150, 80, 115, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[10], 121, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[39], 121, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[41], 49, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[37], 49, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[64], 53, 83, 74, 109, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(npcDictionary[65], 53, 83, 74, 109, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 10000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 250 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 350 },
                    { Stats.DefenseRatePvm, 115 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 14000 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 8 },
                    { Stats.FireResistance, 8 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
