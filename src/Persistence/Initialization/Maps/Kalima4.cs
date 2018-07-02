// <copyright file="Kalima4.cs" company="MUnique">
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
    /// The initialization for the Kalima 4 map.
    /// </summary>
    internal class Kalima4 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kalima 4 map.
        /// </summary>
        public const byte Number = 27;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 4";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, 2, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[190], 1, 0, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[191], 1, 0, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[191], 1, 0, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[191], 1, 0, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[191], 1, 0, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[191], 1, 0, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[192], 1, 0, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[193], 1, 0, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[194], 1, 0, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[195], 1, 0, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[196], 1, 0, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[197], 1, 0, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 4
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 190;
                monster.Designation = "Death Angel 4";
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
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 12000 },
                    { Stats.MinimumPhysBaseDmg, 270 },
                    { Stats.MaximumPhysBaseDmg, 295 },
                    { Stats.DefenseBase, 185 },
                    { Stats.AttackRatePvm, 397 },
                    { Stats.DefenseRatePvm, 120 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
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
                monster.Number = 191;
                monster.Designation = "Death Centurion 4";
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
                    { Stats.Level, 83 },
                    { Stats.MaximumHealth, 22500 },
                    { Stats.MinimumPhysBaseDmg, 350 },
                    { Stats.MaximumPhysBaseDmg, 375 },
                    { Stats.DefenseBase, 255 },
                    { Stats.AttackRatePvm, 500 },
                    { Stats.DefenseRatePvm, 175 },
                    { Stats.PoisonResistance, 22 },
                    { Stats.IceResistance, 22 },
                    { Stats.LightningResistance, 22 },
                    { Stats.FireResistance, 22 },
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
                monster.Number = 192;
                monster.Designation = "Blood Soldier 4";
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
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 8800 },
                    { Stats.MinimumPhysBaseDmg, 245 },
                    { Stats.MaximumPhysBaseDmg, 265 },
                    { Stats.DefenseBase, 165 },
                    { Stats.AttackRatePvm, 360 },
                    { Stats.DefenseRatePvm, 105 },
                    { Stats.PoisonResistance, 19 },
                    { Stats.IceResistance, 19 },
                    { Stats.LightningResistance, 19 },
                    { Stats.FireResistance, 19 },
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
                monster.Number = 193;
                monster.Designation = "Aegis 4";
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
                    { Stats.Level, 62 },
                    { Stats.MaximumHealth, 6000 },
                    { Stats.MinimumPhysBaseDmg, 215 },
                    { Stats.MaximumPhysBaseDmg, 235 },
                    { Stats.DefenseBase, 140 },
                    { Stats.AttackRatePvm, 310 },
                    { Stats.DefenseRatePvm, 87 },
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
                monster.Number = 194;
                monster.Designation = "Rogue Centurion 4";
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
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 6900 },
                    { Stats.MinimumPhysBaseDmg, 230 },
                    { Stats.MaximumPhysBaseDmg, 250 },
                    { Stats.DefenseBase, 150 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 95 },
                    { Stats.PoisonResistance, 18 },
                    { Stats.IceResistance, 18 },
                    { Stats.LightningResistance, 18 },
                    { Stats.FireResistance, 18 },
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
                monster.Number = 195;
                monster.Designation = "Necron 4";
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
                    { Stats.Level, 78 },
                    { Stats.MaximumHealth, 16000 },
                    { Stats.MinimumPhysBaseDmg, 305 },
                    { Stats.MaximumPhysBaseDmg, 330 },
                    { Stats.DefenseBase, 215 },
                    { Stats.AttackRatePvm, 440 },
                    { Stats.DefenseRatePvm, 145 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.LightningResistance, 21 },
                    { Stats.FireResistance, 21 },
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
                monster.Number = 196;
                monster.Designation = "Schriker 4";
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
                    { Stats.Level, 88 },
                    { Stats.MaximumHealth, 30000 },
                    { Stats.MinimumPhysBaseDmg, 410 },
                    { Stats.MaximumPhysBaseDmg, 435 },
                    { Stats.DefenseBase, 300 },
                    { Stats.AttackRatePvm, 575 },
                    { Stats.DefenseRatePvm, 245 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.LightningResistance, 23 },
                    { Stats.FireResistance, 23 },
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
                monster.Number = 197;
                monster.Designation = "Illusion of Kundun 4";
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
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 625 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 7800 },
                    { Stats.DefenseRatePvm, 295 },
                    { Stats.PoisonResistance, 45 },
                    { Stats.IceResistance, 45 },
                    { Stats.LightningResistance, 45 },
                    { Stats.FireResistance, 45 },
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
