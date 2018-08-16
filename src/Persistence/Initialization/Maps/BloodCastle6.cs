// <copyright file="BloodCastle6.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 6.
    /// </summary>
    internal class BloodCastle6 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 6 map.
        /// </summary>
        public static readonly byte Number = 16;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 6";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, 1, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[125], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[126], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[127], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[128], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[129], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 6

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 6
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[130], 1, 0, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 6
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 125;
                monster.Designation = "Chief Skeleton Warrior 6";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 110 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 470 },
                    { Stats.MaximumPhysBaseDmg, 510 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 570 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 125 Chief Skeleton Warrior 6

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 126;
                monster.Designation = "Chief Skeleton Archer 6";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 112 },
                    { Stats.MaximumHealth, 53000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 556 },
                    { Stats.DefenseBase, 590 },
                    { Stats.AttackRatePvm, 640 },
                    { Stats.DefenseRatePvm, 380 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 126 Chief Skeleton Archer 6

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 127;
                monster.Designation = "Dark Skull Soldier 6";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 118 },
                    { Stats.MaximumHealth, 57000 },
                    { Stats.MinimumPhysBaseDmg, 560 },
                    { Stats.MaximumPhysBaseDmg, 581 },
                    { Stats.DefenseBase, 610 },
                    { Stats.AttackRatePvm, 710 },
                    { Stats.DefenseRatePvm, 300 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 127 Dark Skull Soldier 6

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 128;
                monster.Designation = "Giant Ogre 6";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 120 },
                    { Stats.MaximumHealth, 62000 },
                    { Stats.MinimumPhysBaseDmg, 580 },
                    { Stats.MaximumPhysBaseDmg, 599 },
                    { Stats.DefenseBase, 615 },
                    { Stats.AttackRatePvm, 780 },
                    { Stats.DefenseRatePvm, 310 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 128 Giant Ogre 6

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 129;
                monster.Designation = "Red Skeleton Knight 6";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 127 },
                    { Stats.MaximumHealth, 70000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 618 },
                    { Stats.DefenseBase, 635 },
                    { Stats.AttackRatePvm, 850 },
                    { Stats.DefenseRatePvm, 360 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 129 Red Skeleton Knight 6

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 130;
                monster.Designation = "Magic Skeleton 6";
                monster.MoveRange = 4;
                monster.AttackRange = 4;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 135 },
                    { Stats.MaximumHealth, 80000 },
                    { Stats.MinimumPhysBaseDmg, 634 },
                    { Stats.MaximumPhysBaseDmg, 651 },
                    { Stats.DefenseBase, 680 },
                    { Stats.AttackRatePvm, 920 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 9 },
                    { Stats.LightningResistance, 9 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 130 Magic Skeleton 6
        }
    }
}
