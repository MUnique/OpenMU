// <copyright file="ChaosCastle1.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 1.
    /// </summary>
    internal class ChaosCastle1 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle1"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle1(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 18;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1
            yield return this.CreateMonsterSpawn(this.NpcDictionary[162], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 1

            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
            yield return this.CreateMonsterSpawn(this.NpcDictionary[163], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 2
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 162;
                monster.Designation = "Chaos Castle 1";
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
                    { Stats.Level, 30 },
                    { Stats.MaximumHealth, 700 },
                    { Stats.MinimumPhysBaseDmg, 70 },
                    { Stats.MaximumPhysBaseDmg, 90 },
                    { Stats.DefenseBase, 30 },
                    { Stats.AttackRatePvm, 160 },
                    { Stats.DefenseRatePvm, 20 },
                    { Stats.PoisonResistance, 1f / 255 },
                    { Stats.IceResistance, 1f / 255 },
                    { Stats.FireResistance, 1f / 255 },
                    { Stats.LightningResistance, 1f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 162 Chaos Castle 1

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 163;
                monster.Designation = "Chaos Castle 2";
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
                    { Stats.Level, 32 },
                    { Stats.MaximumHealth, 800 },
                    { Stats.MinimumPhysBaseDmg, 80 },
                    { Stats.MaximumPhysBaseDmg, 100 },
                    { Stats.DefenseBase, 40 },
                    { Stats.AttackRatePvm, 180 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.PoisonResistance, 1f / 255 },
                    { Stats.IceResistance, 1f / 255 },
                    { Stats.FireResistance, 1f / 255 },
                    { Stats.LightningResistance, 1f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 163 Chaos Castle 2
        }
    }
}
