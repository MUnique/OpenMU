// <copyright file="ChaosCastle6.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 6.
    /// </summary>
    internal class ChaosCastle6 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle6"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle6(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 23;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 6";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11
            yield return this.CreateMonsterSpawn(this.NpcDictionary[172], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 11

            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
            yield return this.CreateMonsterSpawn(this.NpcDictionary[173], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 12
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 172;
                monster.Designation = "Chaos Castle 11";
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
                    { Stats.Level, 76 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 255 },
                    { Stats.MaximumPhysBaseDmg, 280 },
                    { Stats.DefenseBase, 215 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 140 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.FireResistance, 6f / 255 },
                    { Stats.LightningResistance, 6f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 172 Chaos Castle 11

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 173;
                monster.Designation = "Chaos Castle 12";
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
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 18000 },
                    { Stats.MinimumPhysBaseDmg, 280 },
                    { Stats.MaximumPhysBaseDmg, 300 },
                    { Stats.DefenseBase, 240 },
                    { Stats.AttackRatePvm, 470 },
                    { Stats.DefenseRatePvm, 150 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.FireResistance, 6f / 255 },
                    { Stats.LightningResistance, 6f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 173 Chaos Castle 12
        }
    }
}
