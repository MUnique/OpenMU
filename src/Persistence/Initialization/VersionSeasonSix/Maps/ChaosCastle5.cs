// <copyright file="ChaosCastle5.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 5.
    /// </summary>
    internal class ChaosCastle5 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle5"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle5(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 22;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 5";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9
            yield return this.CreateMonsterSpawn(this.NpcDictionary[170], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 9

            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
            yield return this.CreateMonsterSpawn(this.NpcDictionary[171], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 10
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 170;
                monster.Designation = "Chaos Castle 9";
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
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 10000 },
                    { Stats.MinimumPhysBaseDmg, 215 },
                    { Stats.MaximumPhysBaseDmg, 240 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 370 },
                    { Stats.DefenseRatePvm, 120 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 170 Chaos Castle 9

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 171;
                monster.Designation = "Chaos Castle 10";
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
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 12500 },
                    { Stats.MinimumPhysBaseDmg, 235 },
                    { Stats.MaximumPhysBaseDmg, 260 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 130 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                    { Stats.LightningResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 171 Chaos Castle 10
        }
    }
}
