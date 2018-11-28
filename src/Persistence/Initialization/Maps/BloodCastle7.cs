// <copyright file="BloodCastle7.cs" company="MUnique">
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
    /// Initialization for the Blood Castle 7.
    /// </summary>
    internal class BloodCastle7 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Blood Castle 7 map.
        /// </summary>
        public static readonly byte Number = 17;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Blood Castle 7";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[131], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 075, 075); // Castle Gate
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[132], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 095, 095); // Statue of Saint
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[232], 1, Direction.SouthWest, SpawnTrigger.AutomaticDuringEvent, 010, 010, 009, 009); // Archangel

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 034, 034); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 038, 038); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 035, 035); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 028, 028); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 035, 035); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 033, 033); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 041, 041); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 042, 042); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 025, 025); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 023, 023); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 026, 026); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 037, 037); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 040, 040); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 039, 039); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 027, 027); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 024, 024); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 032, 032); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 045, 045); // Chief Skeleton Warrior 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[138], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 031, 031); // Chief Skeleton Warrior 7

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 045, 045); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 023, 023); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 043, 043); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 039, 039); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 037, 037); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 029, 029); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 028, 028); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 025, 025); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 021, 021); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 022, 022); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 021, 021); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 031, 031); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 030, 030); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 033, 033); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 021, 021); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 043, 043); // Chief Skeleton Archer 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[139], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 026, 026); // Chief Skeleton Archer 7

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 046, 046); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 063, 063); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 053, 053); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 065, 065); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 067, 067); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 059, 059); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 059, 059); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 055, 055); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 062, 062); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 041, 041); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 069, 069); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 050, 050); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 048, 048); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 046, 046); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 044, 044); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 051, 051); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 049, 049); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 057, 057); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 064, 064); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 067, 067); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 053, 053); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 065, 065); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 060, 060); // Dark Skull Soldier 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[140], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 057, 057); // Dark Skull Soldier 7

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 069, 069); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 061, 061); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 055, 055); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 058, 058); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 052, 052); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 050, 050); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 066, 066); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 056, 056); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 070, 070); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 048, 048); // Giant Ogre 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[141], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 063, 063); // Giant Ogre 7

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 078, 078); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 083, 083); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 078, 078); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 078, 078); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 080, 080); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 062, 062); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 017, 017, 083, 083); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 090, 090); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 011, 011, 083, 083); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 094, 094); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 082, 082); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 020, 020, 088, 088); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 054, 054); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 036, 036); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 015, 015, 068, 068); // Red Skeleton Knight 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[142], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 047, 047); // Red Skeleton Knight 7

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 016, 016, 087, 087); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 012, 012, 081, 081); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 088, 088); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 085, 085); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 019, 019, 085, 085); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 013, 013, 093, 093); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 014, 014, 080, 080); // Magic Skeleton 7
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[143], 1, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent, 018, 018, 081, 081); // Magic Skeleton 7
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 138;
                monster.Designation = "Chief Skeleton Warrior 7";
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
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 173500 },
                    { Stats.MinimumPhysBaseDmg, 745 },
                    { Stats.MaximumPhysBaseDmg, 800 },
                    { Stats.DefenseBase, 600 },
                    { Stats.AttackRatePvm, 640 },
                    { Stats.DefenseRatePvm, 426 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 138 Chief Skeleton Warrior 7

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 139;
                monster.Designation = "Chief Skeleton Archer 7";
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
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 175000 },
                    { Stats.MinimumPhysBaseDmg, 825 },
                    { Stats.MaximumPhysBaseDmg, 872 },
                    { Stats.DefenseBase, 615 },
                    { Stats.AttackRatePvm, 690 },
                    { Stats.DefenseRatePvm, 440 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 139 Chief Skeleton Archer 7

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 140;
                monster.Designation = "Dark Skull Soldier 7";
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
                    { Stats.Level, 125 },
                    { Stats.MaximumHealth, 184000 },
                    { Stats.MinimumPhysBaseDmg, 890 },
                    { Stats.MaximumPhysBaseDmg, 915 },
                    { Stats.DefenseBase, 622 },
                    { Stats.AttackRatePvm, 760 },
                    { Stats.DefenseRatePvm, 465 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 140 Dark Skull Soldier 7

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 141;
                monster.Designation = "Giant Ogre 7";
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
                    { Stats.Level, 129 },
                    { Stats.MaximumHealth, 208000 },
                    { Stats.MinimumPhysBaseDmg, 920 },
                    { Stats.MaximumPhysBaseDmg, 946 },
                    { Stats.DefenseBase, 635 },
                    { Stats.AttackRatePvm, 830 },
                    { Stats.DefenseRatePvm, 510 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 141 Giant Ogre 7

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 142;
                monster.Designation = "Red Skeleton Knight 7";
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
                    { Stats.Level, 132 },
                    { Stats.MaximumHealth, 208700 },
                    { Stats.MinimumPhysBaseDmg, 995 },
                    { Stats.MaximumPhysBaseDmg, 1120 },
                    { Stats.DefenseBase, 648 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 585 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.FireResistance, 7 },
                    { Stats.LightningResistance, 7 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 142 Red Skeleton Knight 7

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 143;
                monster.Designation = "Magic Skeleton 7";
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
                    { Stats.Level, 140 },
                    { Stats.MaximumHealth, 215000 },
                    { Stats.MinimumPhysBaseDmg, 1500 },
                    { Stats.MaximumPhysBaseDmg, 1780 },
                    { Stats.DefenseBase, 670 },
                    { Stats.AttackRatePvm, 950 },
                    { Stats.DefenseRatePvm, 750 },
                    { Stats.PoisonResistance, 10 },
                    { Stats.IceResistance, 10 },
                    { Stats.FireResistance, 10 },
                    { Stats.LightningResistance, 10 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 143 Magic Skeleton 7
        }
    }
}
