// <copyright file="Kalima3.cs" company="MUnique">
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
    /// The initialization for the Kalima 3 map.
    /// </summary>
    internal class Kalima3 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kalima 3 map.
        /// </summary>
        public static readonly byte Number = 26;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 3";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, Direction.South, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[182], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[183], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[183], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[183], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[183], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[183], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[184], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[185], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[186], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[188], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[187], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 3

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[189], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 3
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 182;
                monster.Designation = "Death Angel 3";
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
                    { Stats.Level, 53 },
                    { Stats.MaximumHealth, 4400 },
                    { Stats.MinimumPhysBaseDmg, 175 },
                    { Stats.MaximumPhysBaseDmg, 185 },
                    { Stats.DefenseBase, 105 },
                    { Stats.AttackRatePvm, 273 },
                    { Stats.DefenseRatePvm, 67 },
                    { Stats.PoisonResistance, 17 },
                    { Stats.IceResistance, 17 },
                    { Stats.LightningResistance, 17 },
                    { Stats.FireResistance, 17 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 183;
                monster.Designation = "Death Centurion 3";
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
                    { Stats.Level, 63 },
                    { Stats.MaximumHealth, 6600 },
                    { Stats.MinimumPhysBaseDmg, 225 },
                    { Stats.MaximumPhysBaseDmg, 235 },
                    { Stats.DefenseBase, 150 },
                    { Stats.AttackRatePvm, 325 },
                    { Stats.DefenseRatePvm, 92 },
                    { Stats.PoisonResistance, 19 },
                    { Stats.IceResistance, 19 },
                    { Stats.LightningResistance, 19 },
                    { Stats.FireResistance, 19 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 184;
                monster.Designation = "Blood Soldier 3";
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
                    { Stats.Level, 50 },
                    { Stats.MaximumHealth, 3650 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 170 },
                    { Stats.DefenseBase, 90 },
                    { Stats.AttackRatePvm, 250 },
                    { Stats.DefenseRatePvm, 600 },
                    { Stats.PoisonResistance, 16 },
                    { Stats.IceResistance, 16 },
                    { Stats.LightningResistance, 16 },
                    { Stats.FireResistance, 16 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 185;
                monster.Designation = "Aegis 3";
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
                    { Stats.Level, 46 },
                    { Stats.MaximumHealth, 2700 },
                    { Stats.MinimumPhysBaseDmg, 145 },
                    { Stats.MaximumPhysBaseDmg, 155 },
                    { Stats.DefenseBase, 73 },
                    { Stats.AttackRatePvm, 220 },
                    { Stats.DefenseRatePvm, 54 },
                    { Stats.PoisonResistance, 14 },
                    { Stats.IceResistance, 14 },
                    { Stats.LightningResistance, 14 },
                    { Stats.FireResistance, 14 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 186;
                monster.Designation = "Rogue Centurion 3";
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
                    { Stats.Level, 48 },
                    { Stats.MaximumHealth, 3100 },
                    { Stats.MinimumPhysBaseDmg, 152 },
                    { Stats.MaximumPhysBaseDmg, 162 },
                    { Stats.DefenseBase, 80 },
                    { Stats.AttackRatePvm, 232 },
                    { Stats.DefenseRatePvm, 56 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.LightningResistance, 15 },
                    { Stats.FireResistance, 15 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 187;
                monster.Designation = "Necron 3";
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
                    { Stats.Level, 58 },
                    { Stats.MaximumHealth, 5300 },
                    { Stats.MinimumPhysBaseDmg, 195 },
                    { Stats.MaximumPhysBaseDmg, 205 },
                    { Stats.DefenseBase, 126 },
                    { Stats.AttackRatePvm, 296 },
                    { Stats.DefenseRatePvm, 77 },
                    { Stats.PoisonResistance, 18 },
                    { Stats.IceResistance, 18 },
                    { Stats.LightningResistance, 18 },
                    { Stats.FireResistance, 18 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 188;
                monster.Designation = "Schriker 3";
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
                    { Stats.Level, 69 },
                    { Stats.MaximumHealth, 8400 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 270 },
                    { Stats.DefenseBase, 180 },
                    { Stats.AttackRatePvm, 365 },
                    { Stats.DefenseRatePvm, 110 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 189;
                monster.Designation = "Illusion of Kundun 3";
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
                    { Stats.Level, 81 },
                    { Stats.MaximumHealth, 14000 },
                    { Stats.MinimumPhysBaseDmg, 350 },
                    { Stats.MaximumPhysBaseDmg, 360 },
                    { Stats.DefenseBase, 255 },
                    { Stats.AttackRatePvm, 460 },
                    { Stats.DefenseRatePvm, 160 },
                    { Stats.PoisonResistance, 40 },
                    { Stats.IceResistance, 40 },
                    { Stats.LightningResistance, 40 },
                    { Stats.FireResistance, 40 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
