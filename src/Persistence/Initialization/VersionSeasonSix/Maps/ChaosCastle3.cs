// <copyright file="ChaosCastle3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// Initialization for the Chaos Castle 3.
    /// </summary>
    internal class ChaosCastle3 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle3"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle3(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 20;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 3";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5
            yield return this.CreateMonsterSpawn(this.NpcDictionary[166], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 5

            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
            yield return this.CreateMonsterSpawn(this.NpcDictionary[167], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 6
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 166;
                monster.Designation = "Chaos Castle 5";
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
                    { Stats.Level, 54 },
                    { Stats.MaximumHealth, 4000 },
                    { Stats.MinimumPhysBaseDmg, 165 },
                    { Stats.MaximumPhysBaseDmg, 168 },
                    { Stats.DefenseBase, 105 },
                    { Stats.AttackRatePvm, 270 },
                    { Stats.DefenseRatePvm, 80 },
                    { Stats.PoisonResistance, 3f / 255 },
                    { Stats.IceResistance, 3f / 255 },
                    { Stats.FireResistance, 3f / 255 },
                    { Stats.LightningResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 166 Chaos Castle 5

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 167;
                monster.Designation = "Chaos Castle 6";
                monster.MoveRange = 50;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 60 },
                    { Stats.MaximumHealth, 5000 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 180 },
                    { Stats.DefenseBase, 120 },
                    { Stats.AttackRatePvm, 290 },
                    { Stats.DefenseRatePvm, 90 },
                    { Stats.PoisonResistance, 3f / 255 },
                    { Stats.IceResistance, 3f / 255 },
                    { Stats.FireResistance, 3f / 255 },
                    { Stats.LightningResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 167 Chaos Castle 6
        }
    }
}
