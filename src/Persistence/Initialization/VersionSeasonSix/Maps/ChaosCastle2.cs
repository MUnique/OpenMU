// <copyright file="ChaosCastle2.cs" company="MUnique">
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
    /// Initialization for the Chaos Castle 2.
    /// </summary>
    internal class ChaosCastle2 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosCastle2"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public ChaosCastle2(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 19;

        /// <inheritdoc/>
        protected override string MapName => "Chaos Castle 2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            // Monsters:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 026, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 028, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 028, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 034, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 039, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 038, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 038, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 041, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 032, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 042, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 030, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 040, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 033, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 042, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 035, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 028, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 028, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 025, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 033, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 031, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 038, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 039, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 042, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 034, 090, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 030, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 028, 100, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 035, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 043, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 025, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 030, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 041, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 039, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 029, 103, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 025, 097, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 027, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 032, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 037, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 038, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 043, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 042, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 024, 080, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 024, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 024, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 041, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 041, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 038, 096, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 032, 078, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 027, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 044, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3
            yield return this.CreateMonsterSpawn(this.NpcDictionary[164], 039, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 3

            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 027, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 033, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 043, 077, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 043, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 027, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 044, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 038, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 038, 089, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 042, 099, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 035, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 041, 079, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 036, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 041, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 035, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 038, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 038, 106, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 042, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 030, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 041, 093, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 043, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 032, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 033, 105, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 024, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 032, 094, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 036, 092, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 040, 091, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 040, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 036, 076, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 026, 075, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 030, 082, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 024, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 037, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 039, 087, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 038, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 030, 095, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 029, 086, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 040, 104, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 043, 098, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
            yield return this.CreateMonsterSpawn(this.NpcDictionary[165], 041, 084, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Chaos Castle 4
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 164;
                monster.Designation = "Chaos Castle 3";
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
                    { Stats.Level, 40 },
                    { Stats.MaximumHealth, 1600 },
                    { Stats.MinimumPhysBaseDmg, 132 },
                    { Stats.MaximumPhysBaseDmg, 135 },
                    { Stats.DefenseBase, 75 },
                    { Stats.AttackRatePvm, 225 },
                    { Stats.DefenseRatePvm, 60 },
                    { Stats.PoisonResistance, 2f / 255 },
                    { Stats.IceResistance, 2f / 255 },
                    { Stats.FireResistance, 2f / 255 },
                    { Stats.LightningResistance, 2f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 164 Chaos Castle 3

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 165;
                monster.Designation = "Chaos Castle 4";
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
                    { Stats.Level, 45 },
                    { Stats.MaximumHealth, 2700 },
                    { Stats.MinimumPhysBaseDmg, 150 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.DefenseBase, 90 },
                    { Stats.AttackRatePvm, 250 },
                    { Stats.DefenseRatePvm, 70 },
                    { Stats.PoisonResistance, 2f / 255 },
                    { Stats.IceResistance, 2f / 255 },
                    { Stats.FireResistance, 2f / 255 },
                    { Stats.LightningResistance, 2f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            } // 165 Chaos Castle 4
        }
    }
}
