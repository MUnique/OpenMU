// <copyright file="RaklionBoss.cs" company="MUnique">
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

    /// <summary>
    /// Map initialization for the raklion boss map.
    /// </summary>
    /// <seealso cref="BaseMapInitializer" />
    internal class RaklionBoss : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RaklionBoss"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public RaklionBoss(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 58;

        /// <inheritdoc />
        protected override string MapName => "LaCleon Boss";

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.GameConfiguration.Monsters.First(m => m.Number == 459), 145, 31); // Selupan (the boss)

            var spiderEggs1 = this.GameConfiguration.Monsters.First(m => m.Number == 460);
            var spiderEggs2 = this.GameConfiguration.Monsters.First(m => m.Number == 461);
            var spiderEggs3 = this.GameConfiguration.Monsters.First(m => m.Number == 462);
            yield return this.CreateMonsterSpawn(spiderEggs1, 141, 27);
            yield return this.CreateMonsterSpawn(spiderEggs2, 141, 28);
            yield return this.CreateMonsterSpawn(spiderEggs3, 142, 28);
            yield return this.CreateMonsterSpawn(spiderEggs1, 146, 34);
            yield return this.CreateMonsterSpawn(spiderEggs3, 147, 34);
            yield return this.CreateMonsterSpawn(spiderEggs2, 146, 33);
            yield return this.CreateMonsterSpawn(spiderEggs1, 152, 31);
            yield return this.CreateMonsterSpawn(spiderEggs1, 152, 30);
            yield return this.CreateMonsterSpawn(spiderEggs2, 151, 30);
            yield return this.CreateMonsterSpawn(spiderEggs3, 146, 29);
            yield return this.CreateMonsterSpawn(spiderEggs3, 146, 30);
            yield return this.CreateMonsterSpawn(spiderEggs2, 147, 29);
            yield return this.CreateMonsterSpawn(spiderEggs1, 145, 24);
            yield return this.CreateMonsterSpawn(spiderEggs1, 144, 24);
            yield return this.CreateMonsterSpawn(spiderEggs2, 144, 25);

            var coolutin = this.GameConfiguration.Monsters.First(m => m.Number == 457);
            yield return this.CreateMonsterSpawn(coolutin, 144, 25);
            yield return this.CreateMonsterSpawn(coolutin, 144, 29);
            yield return this.CreateMonsterSpawn(coolutin, 144, 32);
            yield return this.CreateMonsterSpawn(coolutin, 144, 35);
            yield return this.CreateMonsterSpawn(coolutin, 144, 38);
            yield return this.CreateMonsterSpawn(coolutin, 153, 24);
            yield return this.CreateMonsterSpawn(coolutin, 153, 28);
            yield return this.CreateMonsterSpawn(coolutin, 153, 32);
            yield return this.CreateMonsterSpawn(coolutin, 153, 35);
            yield return this.CreateMonsterSpawn(coolutin, 153, 38);
        }

        /// <inheritdoc />
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 459;
                monster.Designation = "Selupan";
                monster.MoveRange = 3;
                monster.AttackRange = 10;
                monster.ViewRange = 8;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 145 },
                    { Stats.MaximumHealth, 4800000 },
                    { Stats.MinimumPhysBaseDmg, 2500 },
                    { Stats.MaximumPhysBaseDmg, 3000 },
                    { Stats.DefenseBase, 1250 },
                    { Stats.AttackRatePvm, 2200 },
                    { Stats.DefenseRatePvm, 1800 },
                    { Stats.PoisonResistance, 254f / 255 },
                    { Stats.IceResistance, 254f / 255 },
                    { Stats.WaterResistance, 150f / 255 },
                    { Stats.FireResistance, 150f / 255 },
                };
                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 460;
                monster.Designation = "Spider Eggs 1";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 0;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 10 },
                    { Stats.MaximumHealth, 40000 },
                    { Stats.MinimumPhysBaseDmg, 500 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 410 },
                    { Stats.AttackRatePvm, 750 },
                    { Stats.DefenseRatePvm, 280 },
                    { Stats.PoisonResistance, 50f / 255 },
                    { Stats.IceResistance, 50f / 255 },
                    { Stats.WaterResistance, 50f / 255 },
                    { Stats.FireResistance, 50f / 255 },
                };
                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 461;
                monster.Designation = "Spider Eggs 2";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 0;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 10 },
                    { Stats.MaximumHealth, 40000 },
                    { Stats.MinimumPhysBaseDmg, 500 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 410 },
                    { Stats.AttackRatePvm, 750 },
                    { Stats.DefenseRatePvm, 280 },
                    { Stats.PoisonResistance, 50f / 255 },
                    { Stats.IceResistance, 50f / 255 },
                    { Stats.WaterResistance, 50f / 255 },
                    { Stats.FireResistance, 50f / 255 },
                };
                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 462;
                monster.Designation = "Spider Eggs 3";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 0;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 10 },
                    { Stats.MaximumHealth, 40000 },
                    { Stats.MinimumPhysBaseDmg, 500 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 410 },
                    { Stats.AttackRatePvm, 750 },
                    { Stats.DefenseRatePvm, 280 },
                    { Stats.PoisonResistance, 50f / 255 },
                    { Stats.IceResistance, 50f / 255 },
                    { Stats.WaterResistance, 50f / 255 },
                    { Stats.FireResistance, 50f / 255 },
                };
                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
