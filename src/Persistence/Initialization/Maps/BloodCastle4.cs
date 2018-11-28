// <copyright file="BloodCastle4.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 4.
    /// </summary>
    internal class BloodCastle4 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 4 map.
        /// </summary>
        public static readonly byte Number = 14;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 4";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, Direction.SouthWest, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[113], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[114], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[115], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[116], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[117], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 4

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[118], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 4
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 113;
                monster.Designation = "Chief Skeleton Warrior 4";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 76 },
                    { Stats.MaximumHealth, 17000 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 460 },
                    { Stats.DefenseRatePvm, 150 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 113 Chief Skeleton Warrior 4

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 114;
                monster.Designation = "Chief Skeleton Archer 4";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 81 },
                    { Stats.MaximumHealth, 20000 },
                    { Stats.MinimumPhysBaseDmg, 320 },
                    { Stats.MaximumPhysBaseDmg, 350 },
                    { Stats.DefenseBase, 270 },
                    { Stats.AttackRatePvm, 520 },
                    { Stats.DefenseRatePvm, 160 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 114 Chief Skeleton Archer 4

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 115;
                monster.Designation = "Dark Skull Soldier 4";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 86 },
                    { Stats.MaximumHealth, 25000 },
                    { Stats.MinimumPhysBaseDmg, 390 },
                    { Stats.MaximumPhysBaseDmg, 420 },
                    { Stats.DefenseBase, 360 },
                    { Stats.AttackRatePvm, 580 },
                    { Stats.DefenseRatePvm, 195 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 115 Dark Skull Soldier 4

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 116;
                monster.Designation = "Giant Ogre 4";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 32000 },
                    { Stats.MinimumPhysBaseDmg, 440 },
                    { Stats.MaximumPhysBaseDmg, 500 },
                    { Stats.DefenseBase, 400 },
                    { Stats.AttackRatePvm, 660 },
                    { Stats.DefenseRatePvm, 230 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 116 Giant Ogre 4

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 117;
                monster.Designation = "Red Skeleton Knight 4";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 3;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 94 },
                    { Stats.MaximumHealth, 38000 },
                    { Stats.MinimumPhysBaseDmg, 480 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 450 },
                    { Stats.AttackRatePvm, 720 },
                    { Stats.DefenseRatePvm, 260 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.FireResistance, 6 },
                    { Stats.LightningResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 117 Red Skeleton Knight 4

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 118;
                monster.Designation = "Magic Skeleton 4";
                monster.MoveRange = 4;
                monster.AttackRange = 4;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 99 },
                    { Stats.MaximumHealth, 47000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 580 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 840 },
                    { Stats.DefenseRatePvm, 280 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.FireResistance, 9 },
                    { Stats.LightningResistance, 9 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 118 Magic Skeleton 4
        }
    }
}
