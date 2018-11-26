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
        /// The default number of the kalima 1 map.
        /// </summary>
        public static readonly byte Number = 24;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kalima 1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[259], 1, Direction.South, SpawnTrigger.Automatic, 007, 007, 019, 019); // Oracle Layla

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 050, 050); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 054, 054); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 057, 057); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 065, 065); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 067, 067); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 072, 072); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 086, 086); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 095, 095); // Death Angel 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[144], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 075, 075); // Death Angel 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[145], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 090, 090); // Death Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[145], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 077, 077); // Death Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[145], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 072, 072); // Death Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[145], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 078, 078); // Death Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[145], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 071, 071); // Death Centurion 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 009, 009); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 017, 017); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 035, 035); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 027, 027); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 035, 035); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 044, 044); // Blood Soldier 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[146], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 028, 028); // Blood Soldier 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 075, 075); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 021, 021); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 017, 017); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 011, 011); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 011, 011); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 012, 012); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 022, 022); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 024, 024); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 017, 017); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 009, 009); // Aegis 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[147], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 022, 022); // Aegis 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 022, 022); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 009, 009); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 014, 014); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 008, 008); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 019, 019); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 013, 013); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 006, 006); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 096, 096, 016, 016); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 009, 009); // Rogue Centurion 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[148], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 019, 019); // Rogue Centurion 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 084, 084); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 101, 101); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 106, 106); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 096, 096); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 084, 084); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 085, 085); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 077, 077); // Necron 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[149], 1, Direction.Undefined, SpawnTrigger.Automatic, 074, 074, 076, 076); // Necron 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 050, 050); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 051, 051); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 058, 058); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 065, 065); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 066, 066); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 097, 097); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 109, 109); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 107, 107); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 093, 093); // Schriker 1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[160], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 087, 087); // Schriker 1

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[161], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 076, 076); // Illusion of Kundun 1
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
                new Dictionary<AttributeDefinition, float>
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
