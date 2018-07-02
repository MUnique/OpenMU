// <copyright file="Kalima2.cs" company="MUnique">
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
    /// The initialization for the Kalima 2 map.
    /// </summary>
    internal class Kalima2 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kalima 2 map.
        /// </summary>
        public const byte Number = 25;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, 2, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[174], 1, 0, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[175], 1, 0, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[175], 1, 0, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[175], 1, 0, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[175], 1, 0, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[175], 1, 0, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[176], 1, 0, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[177], 1, 0, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[178], 1, 0, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[179], 1, 0, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[180], 1, 0, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[181], 1, 0, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 2
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 174;
                monster.Designation = "Death Angel 2";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 40 },
                    { Stats.MaximumHealth, 1800 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 137 },
                    { Stats.DefenseBase, 57 },
                    { Stats.AttackRatePvm, 182 },
                    { Stats.DefenseRatePvm, 45 },
                    { Stats.PoisonResistance, 14 },
                    { Stats.IceResistance, 14 },
                    { Stats.LightningResistance, 14 },
                    { Stats.FireResistance, 14 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 175;
                monster.Designation = "Death Centurion 2";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 48 },
                    { Stats.MaximumHealth, 3000 },
                    { Stats.MinimumPhysBaseDmg, 155 },
                    { Stats.MaximumPhysBaseDmg, 162 },
                    { Stats.DefenseBase, 81 },
                    { Stats.AttackRatePvm, 235 },
                    { Stats.DefenseRatePvm, 58 },
                    { Stats.PoisonResistance, 16 },
                    { Stats.IceResistance, 16 },
                    { Stats.LightningResistance, 16 },
                    { Stats.FireResistance, 16 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 176;
                monster.Designation = "Blood Soldier 2";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 37 },
                    { Stats.MaximumHealth, 1400 },
                    { Stats.MinimumPhysBaseDmg, 120 },
                    { Stats.MaximumPhysBaseDmg, 127 },
                    { Stats.DefenseBase, 50 },
                    { Stats.AttackRatePvm, 163 },
                    { Stats.DefenseRatePvm, 40 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.LightningResistance, 13 },
                    { Stats.FireResistance, 13 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 177;
                monster.Designation = "Aegis 2";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 32 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 105 },
                    { Stats.MaximumPhysBaseDmg, 112 },
                    { Stats.DefenseBase, 42 },
                    { Stats.AttackRatePvm, 135 },
                    { Stats.DefenseRatePvm, 33 },
                    { Stats.PoisonResistance, 11 },
                    { Stats.IceResistance, 11 },
                    { Stats.LightningResistance, 11 },
                    { Stats.FireResistance, 11 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 178;
                monster.Designation = "Rogue Centurion 2";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 34 },
                    { Stats.MaximumHealth, 1200 },
                    { Stats.MinimumPhysBaseDmg, 112 },
                    { Stats.MaximumPhysBaseDmg, 119 },
                    { Stats.DefenseBase, 45 },
                    { Stats.AttackRatePvm, 147 },
                    { Stats.DefenseRatePvm, 36 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.LightningResistance, 12 },
                    { Stats.FireResistance, 12 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 179;
                monster.Designation = "Necron 2";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 44 },
                    { Stats.MaximumHealth, 2300 },
                    { Stats.MinimumPhysBaseDmg, 140 },
                    { Stats.MaximumPhysBaseDmg, 147 },
                    { Stats.DefenseBase, 68 },
                    { Stats.AttackRatePvm, 205 },
                    { Stats.DefenseRatePvm, 50 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.LightningResistance, 15 },
                    { Stats.FireResistance, 15 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 180;
                monster.Designation = "Schriker 2";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 53 },
                    { Stats.MaximumHealth, 3900 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 187 },
                    { Stats.DefenseBase, 100 },
                    { Stats.AttackRatePvm, 275 },
                    { Stats.DefenseRatePvm, 67 },
                    { Stats.PoisonResistance, 17 },
                    { Stats.IceResistance, 17 },
                    { Stats.LightningResistance, 17 },
                    { Stats.FireResistance, 17 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 181;
                monster.Designation = "Illusion of Kundun 2";
                monster.MoveRange = 3;
                monster.AttackRange = 10;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(800 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10800 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 65 },
                    { Stats.MaximumHealth, 6000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 227 },
                    { Stats.DefenseBase, 140 },
                    { Stats.AttackRatePvm, 355 },
                    { Stats.DefenseRatePvm, 100 },
                    { Stats.PoisonResistance, 35 },
                    { Stats.IceResistance, 35 },
                    { Stats.LightningResistance, 35 },
                    { Stats.FireResistance, 35 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }
        }
    }
}
