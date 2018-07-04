// <copyright file="BloodCastle8.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 8.
    /// </summary>
    internal class BloodCastle8 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 8 map.
        /// </summary>
        public const byte Number = 52;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 8";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, 1, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[428], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 8

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[429], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 8

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[430], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 8

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[431], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 8

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[432], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 8

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 8
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[433], 1, 0, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 8
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 428;
                monster.Designation = "Chief Skeleton Warrior 8";
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
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 29000 },
                    { Stats.MinimumPhysBaseDmg, 475 },
                    { Stats.MaximumPhysBaseDmg, 510 },
                    { Stats.DefenseBase, 440 },
                    { Stats.AttackRatePvm, 570 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 8 },
                    { Stats.LightningResistance, 8 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 428 Chief Skeleton Warrior 8

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 429;
                monster.Designation = "Chief Skeleton Archer 8";
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
                    { Stats.Level, 106 },
                    { Stats.MaximumHealth, 32000 },
                    { Stats.MinimumPhysBaseDmg, 510 },
                    { Stats.MaximumPhysBaseDmg, 555 },
                    { Stats.DefenseBase, 480 },
                    { Stats.AttackRatePvm, 640 },
                    { Stats.DefenseRatePvm, 260 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 8 },
                    { Stats.LightningResistance, 8 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 429 Chief Skeleton Archer 8

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 430;
                monster.Designation = "Dark Skull Soldier 8";
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
                    { Stats.MaximumHealth, 37000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 650 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 710 },
                    { Stats.DefenseRatePvm, 300 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 8 },
                    { Stats.LightningResistance, 8 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 430 Dark Skull Soldier 8

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 431;
                monster.Designation = "Giant Ogre 8";
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
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 645 },
                    { Stats.MaximumPhysBaseDmg, 690 },
                    { Stats.DefenseBase, 540 },
                    { Stats.AttackRatePvm, 780 },
                    { Stats.DefenseRatePvm, 310 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 8 },
                    { Stats.LightningResistance, 8 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 431 Giant Ogre 8

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 432;
                monster.Designation = "Red Skeleton Knight 8";
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
                    { Stats.Level, 119 },
                    { Stats.MaximumHealth, 55000 },
                    { Stats.MinimumPhysBaseDmg, 780 },
                    { Stats.MaximumPhysBaseDmg, 820 },
                    { Stats.DefenseBase, 600 },
                    { Stats.AttackRatePvm, 850 },
                    { Stats.DefenseRatePvm, 360 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 8 },
                    { Stats.LightningResistance, 8 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 432 Red Skeleton Knight 8

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 433;
                monster.Designation = "Magic Skeleton 8";
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
                    { Stats.Level, 125 },
                    { Stats.MaximumHealth, 60000 },
                    { Stats.MinimumPhysBaseDmg, 830 },
                    { Stats.MaximumPhysBaseDmg, 865 },
                    { Stats.DefenseBase, 680 },
                    { Stats.AttackRatePvm, 920 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 11 },
                    { Stats.IceResistance, 11 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 11 },
                    { Stats.LightningResistance, 11 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            } // 433 Magic Skeleton 8
        }
    }
}
