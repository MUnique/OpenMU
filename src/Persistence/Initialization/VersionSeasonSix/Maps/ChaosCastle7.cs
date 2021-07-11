// <copyright file="ChaosCastle7.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 7.
    /// </summary>
    internal class ChaosCastle7 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle7"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle7(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 53;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 7";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13
            yield return this.CreateMonsterSpawn(this.NpcDictionary[426], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 13

            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
            yield return this.CreateMonsterSpawn(this.NpcDictionary[427], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 14
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 426;
                monster.Designation = "Chaos Castle 13";
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
                    { Stats.Level, 111 },
                    { Stats.MaximumHealth, 20000 },
                    { Stats.MinimumPhysBaseDmg, 300 },
                    { Stats.MaximumPhysBaseDmg, 345 },
                    { Stats.DefenseBase, 245 },
                    { Stats.AttackRatePvm, 480 },
                    { Stats.DefenseRatePvm, 1900 },
                    { Stats.PoisonResistance, 7f / 255 },
                    { Stats.IceResistance, 7f / 255 },
                    { Stats.FireResistance, 7f / 255 },
                    { Stats.LightningResistance, 7f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 426 Chaos Castle 13

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 427;
                monster.Designation = "Chaos Castle 14";
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
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 23000 },
                    { Stats.MinimumPhysBaseDmg, 330 },
                    { Stats.MaximumPhysBaseDmg, 380 },
                    { Stats.DefenseBase, 280 },
                    { Stats.AttackRatePvm, 480 },
                    { Stats.DefenseRatePvm, 190 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.FireResistance, 6f / 255 },
                    { Stats.LightningResistance, 6f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 427 Chaos Castle 14
        }
    }
}
