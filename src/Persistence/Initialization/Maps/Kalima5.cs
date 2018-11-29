// <copyright file="Kalima5.cs" company="MUnique">
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
    /// The initialization for the Kalima 5 map.
    /// </summary>
    internal class Kalima5 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kalima5"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Kalima5(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 28;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 5";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[259], 007, 019, Direction.South); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[260], 120, 050); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 105, 054); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 119, 057); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 110, 065); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 121, 067); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 111, 072); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 105, 086); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 118, 095); // Death Angel 5
            yield return this.CreateMonsterSpawn(npcDictionary[260], 120, 075); // Death Angel 5

            yield return this.CreateMonsterSpawn(npcDictionary[261], 087, 090); // Death Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[261], 068, 077); // Death Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[261], 063, 072); // Death Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[261], 058, 078); // Death Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[261], 057, 071); // Death Centurion 5

            yield return this.CreateMonsterSpawn(npcDictionary[262], 110, 009); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 118, 017); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 110, 035); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 121, 027); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 119, 035); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 114, 044); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(npcDictionary[262], 108, 028); // Blood Soldier 5

            yield return this.CreateMonsterSpawn(npcDictionary[263], 030, 075); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 035, 021); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 028, 017); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 036, 011); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 051, 011); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 042, 012); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 045, 022); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 052, 024); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 053, 017); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 060, 009); // Aegis 5
            yield return this.CreateMonsterSpawn(npcDictionary[263], 060, 022); // Aegis 5

            yield return this.CreateMonsterSpawn(npcDictionary[264], 067, 022); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 069, 009); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 074, 014); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 082, 008); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 081, 019); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 086, 013); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 092, 006); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 096, 016); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 099, 009); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(npcDictionary[264], 109, 019); // Rogue Centurion 5

            yield return this.CreateMonsterSpawn(npcDictionary[265], 118, 084); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 104, 101); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 115, 106); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 093, 096); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 093, 084); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 082, 085); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 082, 077); // Necron 5
            yield return this.CreateMonsterSpawn(npcDictionary[265], 074, 076); // Necron 5

            yield return this.CreateMonsterSpawn(npcDictionary[266], 032, 050); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 042, 051); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 038, 058); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 029, 065); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 046, 066); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 042, 097); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 037, 109); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 047, 107); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 053, 093); // Schriker 5
            yield return this.CreateMonsterSpawn(npcDictionary[266], 035, 087); // Schriker 5

            yield return this.CreateMonsterSpawn(npcDictionary[267], 026, 076); // Illusion of Kundun 5
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 260;
                monster.Designation = "Death Angel 5";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 88 },
                    { Stats.MaximumHealth, 31000 },
                    { Stats.MinimumPhysBaseDmg, 408 },
                    { Stats.MaximumPhysBaseDmg, 443 },
                    { Stats.DefenseBase, 315 },
                    { Stats.AttackRatePvm, 587 },
                    { Stats.DefenseRatePvm, 195 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.LightningResistance, 23 },
                    { Stats.FireResistance, 23 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 261;
                monster.Designation = "Death Centurion 5";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 48000 },
                    { Stats.MinimumPhysBaseDmg, 546 },
                    { Stats.MaximumPhysBaseDmg, 581 },
                    { Stats.DefenseBase, 460 },
                    { Stats.AttackRatePvm, 715 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.LightningResistance, 25 },
                    { Stats.FireResistance, 25 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 262;
                monster.Designation = "Blood Soldier 5";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 85 },
                    { Stats.MaximumHealth, 26000 },
                    { Stats.MinimumPhysBaseDmg, 365 },
                    { Stats.MaximumPhysBaseDmg, 395 },
                    { Stats.DefenseBase, 280 },
                    { Stats.AttackRatePvm, 540 },
                    { Stats.DefenseRatePvm, 177 },
                    { Stats.PoisonResistance, 22 },
                    { Stats.IceResistance, 22 },
                    { Stats.LightningResistance, 22 },
                    { Stats.FireResistance, 22 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 263;
                monster.Designation = "Aegis 5";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 79 },
                    { Stats.MaximumHealth, 18000 },
                    { Stats.MinimumPhysBaseDmg, 310 },
                    { Stats.MaximumPhysBaseDmg, 340 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 460 },
                    { Stats.DefenseRatePvm, 163 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 264;
                monster.Designation = "Rogue Centurion 5";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 21000 },
                    { Stats.MinimumPhysBaseDmg, 335 },
                    { Stats.MaximumPhysBaseDmg, 365 },
                    { Stats.DefenseBase, 250 },
                    { Stats.AttackRatePvm, 490 },
                    { Stats.DefenseRatePvm, 168 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.LightningResistance, 21 },
                    { Stats.FireResistance, 21 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 265;
                monster.Designation = "Necron 5";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 93 },
                    { Stats.MaximumHealth, 38500 },
                    { Stats.MinimumPhysBaseDmg, 470 },
                    { Stats.MaximumPhysBaseDmg, 505 },
                    { Stats.DefenseBase, 370 },
                    { Stats.AttackRatePvm, 642 },
                    { Stats.DefenseRatePvm, 220 },
                    { Stats.PoisonResistance, 24 },
                    { Stats.IceResistance, 24 },
                    { Stats.LightningResistance, 24 },
                    { Stats.FireResistance, 24 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 266;
                monster.Designation = "Schriker 5";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 60000 },
                    { Stats.MinimumPhysBaseDmg, 640 },
                    { Stats.MaximumPhysBaseDmg, 675 },
                    { Stats.DefenseBase, 515 },
                    { Stats.AttackRatePvm, 810 },
                    { Stats.DefenseRatePvm, 290 },
                    { Stats.PoisonResistance, 26 },
                    { Stats.IceResistance, 26 },
                    { Stats.LightningResistance, 26 },
                    { Stats.FireResistance, 26 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 267;
                monster.Designation = "Illusion of Kundun 5";
                monster.MoveRange = 3;
                monster.AttackRange = 10;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10800 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 100000 },
                    { Stats.MinimumPhysBaseDmg, 835 },
                    { Stats.MaximumPhysBaseDmg, 870 },
                    { Stats.DefenseBase, 680 },
                    { Stats.AttackRatePvm, 1000 },
                    { Stats.DefenseRatePvm, 360 },
                    { Stats.PoisonResistance, 50 },
                    { Stats.IceResistance, 50 },
                    { Stats.LightningResistance, 50 },
                    { Stats.FireResistance, 50 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
