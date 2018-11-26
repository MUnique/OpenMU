// <copyright file="Kalima6.cs" company="MUnique">
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
    /// The initialization for the Kalima 6 map.
    /// </summary>
    internal class Kalima6 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kalima 6 map.
        /// </summary>
        public static readonly byte Number = 29;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 6";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, Direction.South, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[268], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[269], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[269], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[269], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[269], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[269], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[270], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[271], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[272], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[273], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[274], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[338], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 6
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 268;
                monster.Designation = "Death Angel 6";
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
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 65000 },
                    { Stats.MinimumPhysBaseDmg, 610 },
                    { Stats.MaximumPhysBaseDmg, 645 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 760 },
                    { Stats.DefenseRatePvm, 275 },
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
                monster.Number = 269;
                monster.Designation = "Death Centurion 6";
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
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 87500 },
                    { Stats.MinimumPhysBaseDmg, 753 },
                    { Stats.MaximumPhysBaseDmg, 788 },
                    { Stats.DefenseBase, 620 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 335 },
                    { Stats.PoisonResistance, 28 },
                    { Stats.IceResistance, 28 },
                    { Stats.LightningResistance, 28 },
                    { Stats.FireResistance, 28 },
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
                monster.Number = 270;
                monster.Designation = "Blood Soldier 6";
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
                    { Stats.Level, 101 },
                    { Stats.MaximumHealth, 56000 },
                    { Stats.MinimumPhysBaseDmg, 570 },
                    { Stats.MaximumPhysBaseDmg, 600 },
                    { Stats.DefenseBase, 460 },
                    { Stats.AttackRatePvm, 713 },
                    { Stats.DefenseRatePvm, 255 },
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
                monster.Number = 271;
                monster.Designation = "Aegis 6";
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
                    { Stats.Level, 95 },
                    { Stats.MaximumHealth, 42000 },
                    { Stats.MinimumPhysBaseDmg, 520 },
                    { Stats.MaximumPhysBaseDmg, 550 },
                    { Stats.DefenseBase, 410 },
                    { Stats.AttackRatePvm, 655 },
                    { Stats.DefenseRatePvm, 230 },
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
                monster.Number = 272;
                monster.Designation = "Rogue Centurion 6";
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
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 48500 },
                    { Stats.MinimumPhysBaseDmg, 540 },
                    { Stats.MaximumPhysBaseDmg, 570 },
                    { Stats.DefenseBase, 432 },
                    { Stats.AttackRatePvm, 680 },
                    { Stats.DefenseRatePvm, 240 },
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
                monster.Number = 273;
                monster.Designation = "Necron 6";
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
                    { Stats.Level, 109 },
                    { Stats.MaximumHealth, 75500 },
                    { Stats.MinimumPhysBaseDmg, 672 },
                    { Stats.MaximumPhysBaseDmg, 707 },
                    { Stats.DefenseBase, 550 },
                    { Stats.AttackRatePvm, 820 },
                    { Stats.DefenseRatePvm, 303 },
                    { Stats.PoisonResistance, 27 },
                    { Stats.IceResistance, 27 },
                    { Stats.LightningResistance, 27 },
                    { Stats.FireResistance, 27 },
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
                monster.Number = 274;
                monster.Designation = "Schriker 6";
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
                    { Stats.Level, 119 },
                    { Stats.MaximumHealth, 105000 },
                    { Stats.MinimumPhysBaseDmg, 850 },
                    { Stats.MaximumPhysBaseDmg, 885 },
                    { Stats.DefenseBase, 710 },
                    { Stats.AttackRatePvm, 1000 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.PoisonResistance, 29 },
                    { Stats.IceResistance, 29 },
                    { Stats.LightningResistance, 29 },
                    { Stats.FireResistance, 29 },
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
                monster.Number = 338;
                monster.Designation = "Illusion of Kundun 6";
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
                    { Stats.Level, 140 },
                    { Stats.MaximumHealth, 140000 },
                    { Stats.MinimumPhysBaseDmg, 1070 },
                    { Stats.MaximumPhysBaseDmg, 1105 },
                    { Stats.DefenseBase, 892 },
                    { Stats.AttackRatePvm, 1200 },
                    { Stats.DefenseRatePvm, 450 },
                    { Stats.PoisonResistance, 60 },
                    { Stats.IceResistance, 60 },
                    { Stats.LightningResistance, 60 },
                    { Stats.FireResistance, 60 },
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
