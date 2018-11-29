// <copyright file="Kalima1.cs" company="MUnique">
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
    /// The initialization for the Kalima 1 map.
    /// </summary>
    internal class Kalima1 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Kalima1"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Kalima1(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 24;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[259], 007, 019, Direction.South); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[144], 120, 050); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 105, 054); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 119, 057); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 110, 065); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 121, 067); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 111, 072); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 105, 086); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 118, 095); // Death Angel 1
            yield return this.CreateMonsterSpawn(npcDictionary[144], 120, 075); // Death Angel 1

            yield return this.CreateMonsterSpawn(npcDictionary[145], 087, 090); // Death Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[145], 068, 077); // Death Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[145], 063, 072); // Death Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[145], 058, 078); // Death Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[145], 057, 071); // Death Centurion 1

            yield return this.CreateMonsterSpawn(npcDictionary[146], 110, 009); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 118, 017); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 110, 035); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 121, 027); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 119, 035); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 114, 044); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(npcDictionary[146], 108, 028); // Blood Soldier 1

            yield return this.CreateMonsterSpawn(npcDictionary[147], 030, 075); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 035, 021); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 028, 017); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 036, 011); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 051, 011); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 042, 012); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 045, 022); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 052, 024); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 053, 017); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 060, 009); // Aegis 1
            yield return this.CreateMonsterSpawn(npcDictionary[147], 060, 022); // Aegis 1

            yield return this.CreateMonsterSpawn(npcDictionary[148], 067, 022); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 069, 009); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 074, 014); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 082, 008); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 081, 019); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 086, 013); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 092, 006); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 096, 016); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 099, 009); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(npcDictionary[148], 109, 019); // Rogue Centurion 1

            yield return this.CreateMonsterSpawn(npcDictionary[149], 118, 084); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 104, 101); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 115, 106); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 093, 096); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 093, 084); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 082, 085); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 082, 077); // Necron 1
            yield return this.CreateMonsterSpawn(npcDictionary[149], 074, 076); // Necron 1

            yield return this.CreateMonsterSpawn(npcDictionary[160], 032, 050); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 042, 051); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 038, 058); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 029, 065); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 046, 066); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 042, 097); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 037, 109); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 047, 107); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 053, 093); // Schriker 1
            yield return this.CreateMonsterSpawn(npcDictionary[160], 035, 087); // Schriker 1

            yield return this.CreateMonsterSpawn(npcDictionary[161], 026, 076); // Illusion of Kundun 1
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 144;
                monster.Designation = "Death Angel 1";
                monster.MoveRange = 2;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 24 },
                    { Stats.MaximumHealth, 740 },
                    { Stats.MinimumPhysBaseDmg, 80 },
                    { Stats.MaximumPhysBaseDmg, 87 },
                    { Stats.DefenseBase, 30 },
                    { Stats.AttackRatePvm, 100 },
                    { Stats.DefenseRatePvm, 24 },
                    { Stats.PoisonResistance, 11 },
                    { Stats.IceResistance, 11 },
                    { Stats.LightningResistance, 11 },
                    { Stats.FireResistance, 11 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 145;
                monster.Designation = "Death Centurion 1";
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
                    { Stats.Level, 33 },
                    { Stats.MaximumHealth, 1250 },
                    { Stats.MinimumPhysBaseDmg, 110 },
                    { Stats.MaximumPhysBaseDmg, 117 },
                    { Stats.DefenseBase, 50 },
                    { Stats.AttackRatePvm, 145 },
                    { Stats.DefenseRatePvm, 39 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.LightningResistance, 13 },
                    { Stats.FireResistance, 13 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 146;
                monster.Designation = "Blood Soldier 1";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 21 },
                    { Stats.MaximumHealth, 600 },
                    { Stats.MinimumPhysBaseDmg, 70 },
                    { Stats.MaximumPhysBaseDmg, 77 },
                    { Stats.DefenseBase, 25 },
                    { Stats.AttackRatePvm, 88 },
                    { Stats.DefenseRatePvm, 20 },
                    { Stats.PoisonResistance, 10 },
                    { Stats.IceResistance, 10 },
                    { Stats.LightningResistance, 10 },
                    { Stats.FireResistance, 10 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 147;
                monster.Designation = "Aegis 1";
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
                    { Stats.Level, 17 },
                    { Stats.MaximumHealth, 440 },
                    { Stats.MinimumPhysBaseDmg, 55 },
                    { Stats.MaximumPhysBaseDmg, 62 },
                    { Stats.DefenseBase, 17 },
                    { Stats.AttackRatePvm, 75 },
                    { Stats.DefenseRatePvm, 17 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.LightningResistance, 8 },
                    { Stats.FireResistance, 8 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 148;
                monster.Designation = "Rogue Centurion 1";
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
                    { Stats.Level, 19 },
                    { Stats.MaximumHealth, 500 },
                    { Stats.MinimumPhysBaseDmg, 60 },
                    { Stats.MaximumPhysBaseDmg, 67 },
                    { Stats.DefenseBase, 20 },
                    { Stats.AttackRatePvm, 80 },
                    { Stats.DefenseRatePvm, 18 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.LightningResistance, 9 },
                    { Stats.FireResistance, 9 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 149;
                monster.Designation = "Necron 1";
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
                    { Stats.Level, 28 },
                    { Stats.MaximumHealth, 900 },
                    { Stats.MinimumPhysBaseDmg, 95 },
                    { Stats.MaximumPhysBaseDmg, 102 },
                    { Stats.DefenseBase, 38 },
                    { Stats.AttackRatePvm, 118 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.LightningResistance, 12 },
                    { Stats.FireResistance, 12 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 160;
                monster.Designation = "Schriker 1";
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
                    { Stats.Level, 40 },
                    { Stats.MaximumHealth, 1800 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 137 },
                    { Stats.DefenseBase, 66 },
                    { Stats.AttackRatePvm, 180 },
                    { Stats.DefenseRatePvm, 50 },
                    { Stats.PoisonResistance, 14 },
                    { Stats.IceResistance, 14 },
                    { Stats.LightningResistance, 14 },
                    { Stats.FireResistance, 14 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 161;
                monster.Designation = "Illusion of Kundun 1";
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
                    { Stats.Level, 52 },
                    { Stats.MaximumHealth, 3000 },
                    { Stats.MinimumPhysBaseDmg, 165 },
                    { Stats.MaximumPhysBaseDmg, 172 },
                    { Stats.DefenseBase, 90 },
                    { Stats.AttackRatePvm, 255 },
                    { Stats.DefenseRatePvm, 70 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
