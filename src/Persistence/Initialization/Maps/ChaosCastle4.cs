// <copyright file="ChaosCastle4.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 4.
    /// </summary>
    internal class ChaosCastle4 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle4"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle4(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 21;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 4";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[168], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7
            yield return this.CreateMonsterSpawn(npcDictionary[168], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 7

            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
            yield return this.CreateMonsterSpawn(npcDictionary[169], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 8
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 168;
                monster.Designation = "Chaos Castle 7";
                monster.MoveRange = 50;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 65 },
                    { Stats.MaximumHealth, 65000 },
                    { Stats.MinimumPhysBaseDmg, 185 },
                    { Stats.MaximumPhysBaseDmg, 210 },
                    { Stats.DefenseBase, 145 },
                    { Stats.AttackRatePvm, 320 },
                    { Stats.DefenseRatePvm, 110 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 168 Chaos Castle 7

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 169;
                monster.Designation = "Chaos Castle 8";
                monster.MoveRange = 50;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 68 },
                    { Stats.MaximumHealth, 8000 },
                    { Stats.MinimumPhysBaseDmg, 205 },
                    { Stats.MaximumPhysBaseDmg, 230 },
                    { Stats.DefenseBase, 160 },
                    { Stats.AttackRatePvm, 350 },
                    { Stats.DefenseRatePvm, 120 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 169 Chaos Castle 8
        }
    }
}
