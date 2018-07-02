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
        /// The default number of the kalima 5 map.
        /// </summary>
        public const byte Number = 28;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 5";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, 2, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[260], 1, 0, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[261], 1, 0, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[261], 1, 0, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[261], 1, 0, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[261], 1, 0, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[261], 1, 0, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[262], 1, 0, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[263], 1, 0, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[264], 1, 0, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[265], 1, 0, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 5
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[266], 1, 0, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 5

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[267], 1, 0, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 5
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
