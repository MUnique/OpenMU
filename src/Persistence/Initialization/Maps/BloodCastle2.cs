// <copyright file="BloodCastle2.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 2.
    /// </summary>
    internal class BloodCastle2 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 2 map.
        /// </summary>
        public static readonly byte Number = 12;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, Direction.SouthWest, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[090], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[091], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[092], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[093], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[094], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 2

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[095], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 2
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 90;
                monster.Designation = "Chief Skeleton Warrior 2";
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
                    { Stats.Level, 64 },
                    { Stats.MaximumHealth, 7500 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 220 },
                    { Stats.DefenseBase, 150 },
                    { Stats.AttackRatePvm, 340 },
                    { Stats.DefenseRatePvm, 98 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 090 Chief Skeleton Warrior 2

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 91;
                monster.Designation = "Chief Skeleton Archer 2";
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
                    { Stats.Level, 69 },
                    { Stats.MaximumHealth, 9500 },
                    { Stats.MinimumPhysBaseDmg, 200 },
                    { Stats.MaximumPhysBaseDmg, 240 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 380 },
                    { Stats.DefenseRatePvm, 110 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 091 Chief Skeleton Archer 2

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 92;
                monster.Designation = "Dark Skull Soldier 2";
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
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 12000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 260 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 440 },
                    { Stats.DefenseRatePvm, 130 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 092 Dark Skull Soldier 2

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 93;
                monster.Designation = "Giant Ogre 2";
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
                    { Stats.Level, 79 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 500 },
                    { Stats.DefenseRatePvm, 150 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 093 Giant Ogre 2

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 94;
                monster.Designation = "Red Skeleton Knight 2";
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
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 18000 },
                    { Stats.MinimumPhysBaseDmg, 270 },
                    { Stats.MaximumPhysBaseDmg, 320 },
                    { Stats.DefenseBase, 290 },
                    { Stats.AttackRatePvm, 560 },
                    { Stats.DefenseRatePvm, 170 },
                    { Stats.PoisonResistance, 4 },
                    { Stats.IceResistance, 4 },
                    { Stats.FireResistance, 4 },
                    { Stats.LightningResistance, 4 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 094 Red Skeleton Knight 2

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 95;
                monster.Designation = "Magic Skeleton 2";
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
                    { Stats.Level, 88 },
                    { Stats.MaximumHealth, 24000 },
                    { Stats.MinimumPhysBaseDmg, 300 },
                    { Stats.MaximumPhysBaseDmg, 350 },
                    { Stats.DefenseBase, 360 },
                    { Stats.AttackRatePvm, 640 },
                    { Stats.DefenseRatePvm, 200 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 095 Magic Skeleton 2
        }
    }
}
