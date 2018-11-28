// <copyright file="Raklion.cs" company="MUnique">
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
    /// The initialization for the Raklion map.
    /// </summary>
    internal class Raklion : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the raklion map.
        /// </summary>
        public static readonly byte Number = 57;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "LaCleon"; // Raklion

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 203, 203, 204, 204); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 203, 203); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 203, 203); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 204, 204); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 104, 104); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 154, 154, 129, 129); // Ice Walker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[454], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 113, 113); // Ice Walker

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[455], 1, Direction.Undefined, SpawnTrigger.Automatic, 171, 171, 207, 207); // Giant Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[455], 1, Direction.Undefined, SpawnTrigger.Automatic, 174, 174, 214, 214); // Giant Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[455], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 212, 212); // Giant Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[455], 1, Direction.Undefined, SpawnTrigger.Automatic, 142, 142, 126, 126); // Giant Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[455], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 150, 150); // Giant Mammoth

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 217, 217); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 157, 157, 217, 217); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 153, 153, 210, 210); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 172, 172, 188, 188); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 166, 166, 187, 187); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 162, 162, 182, 182); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 192, 192, 110, 110); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 132, 132); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 149, 149); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 136, 136); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 137, 137); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 106, 106); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 148, 148); // Ice Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[456], 1, Direction.Undefined, SpawnTrigger.Automatic, 095, 095, 195, 195); // Ice Giant

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 149, 149, 197, 197); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 153, 153, 194, 194); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 214, 214); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 026, 026); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 039, 039); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 089, 089); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 095, 095); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 120, 120); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 140, 140); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 099, 099); // Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[457], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 135, 135); // Coolutin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 181, 181); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 184, 184); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 219, 219); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 217, 217); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 090, 090, 213, 213); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 098, 098, 178, 178); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 142, 142); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 135, 135); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 107, 107); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 099, 099); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 161, 161, 135, 135); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 144, 144); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 102, 102); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 104, 104); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 092, 092); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 113, 113); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 140, 140); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 081, 081); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 203, 203, 028, 028); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 033, 033); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 042, 042); // Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[458], 1, Direction.Undefined, SpawnTrigger.Automatic, 203, 203, 049, 049); // Iron Knight

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 101, 101); // Dark Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 107, 107); // Dark Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 109, 109); // Dark Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 101, 101); // Dark Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 117, 117); // Dark Mammoth
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[562], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 116, 116); // Dark Mammoth

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 095, 095); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 079, 079); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 088, 088); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 156, 156); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 065, 065, 143, 143); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 145, 145); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 070, 070, 101, 101); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 088, 088); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 087, 087); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 220, 220); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 195, 195); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 098, 098, 031, 031); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 045, 045); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 023, 023); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 039, 148, 148); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 139, 139); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 159, 159); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 089, 089); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 227, 227); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 216, 216); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 049, 049, 219, 219); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 195, 195); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 120, 120); // Dark Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[563], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 133, 133); // Dark Giant

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 040, 040); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 042, 042); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 222, 222); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 199, 199); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 146, 146); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 159, 159); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 135, 135); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 093, 093); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 115, 115); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 036, 036); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 032, 032); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 068, 068); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 047, 047); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 037, 037); // Dark Coolutin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[564], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 199, 199); // Dark Coolutin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 043, 043); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 032, 032); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 029, 029); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 023, 023); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 026, 026); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 039, 039); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 091, 091, 031, 031); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 070, 070, 095, 095); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 121, 121); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 119, 119); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 224, 224); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 191, 191); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 195, 195); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 031, 031, 051, 051); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 046, 046); // Dark Iron Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[565], 1, Direction.Undefined, SpawnTrigger.Automatic, 066, 066, 137, 137); // Dark Iron Knight
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 454;
                monster.Designation = "Ice Walker";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 102 },
                    { Stats.MaximumHealth, 68000 },
                    { Stats.MinimumPhysBaseDmg, 1310 },
                    { Stats.MaximumPhysBaseDmg, 1965 },
                    { Stats.DefenseBase, 615 },
                    { Stats.AttackRatePvm, 1190 },
                    { Stats.DefenseRatePvm, 800 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 455;
                monster.Designation = "Giant Mammoth";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 112 },
                    { Stats.MaximumHealth, 77000 },
                    { Stats.MinimumPhysBaseDmg, 1441 },
                    { Stats.MaximumPhysBaseDmg, 2017 },
                    { Stats.DefenseBase, 585 },
                    { Stats.AttackRatePvm, 1350 },
                    { Stats.DefenseRatePvm, 840 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 456;
                monster.Designation = "Ice Giant";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 122 },
                    { Stats.MaximumHealth, 84000 },
                    { Stats.MinimumPhysBaseDmg, 1585 },
                    { Stats.MaximumPhysBaseDmg, 2060 },
                    { Stats.DefenseBase, 620 },
                    { Stats.AttackRatePvm, 1570 },
                    { Stats.DefenseRatePvm, 770 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 457;
                monster.Designation = "Coolutin";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 132 },
                    { Stats.MaximumHealth, 88000 },
                    { Stats.MinimumPhysBaseDmg, 1743 },
                    { Stats.MaximumPhysBaseDmg, 2092 },
                    { Stats.DefenseBase, 650 },
                    { Stats.AttackRatePvm, 1940 },
                    { Stats.DefenseRatePvm, 840 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 458;
                monster.Designation = "Iron Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 142 },
                    { Stats.MaximumHealth, 95000 },
                    { Stats.MinimumPhysBaseDmg, 1917 },
                    { Stats.MaximumPhysBaseDmg, 2301 },
                    { Stats.DefenseBase, 660 },
                    { Stats.AttackRatePvm, 2000 },
                    { Stats.DefenseRatePvm, 800 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 562;
                monster.Designation = "Dark Mammoth";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 140 },
                    { Stats.MaximumHealth, 237000 },
                    { Stats.MinimumPhysBaseDmg, 1741 },
                    { Stats.MaximumPhysBaseDmg, 2317 },
                    { Stats.DefenseBase, 785 },
                    { Stats.AttackRatePvm, 2240 },
                    { Stats.DefenseRatePvm, 1440 },
                    { Stats.PoisonResistance, 150 },
                    { Stats.IceResistance, 30 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 563;
                monster.Designation = "Dark Giant";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 8;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 143 },
                    { Stats.MaximumHealth, 254000 },
                    { Stats.MinimumPhysBaseDmg, 1885 },
                    { Stats.MaximumPhysBaseDmg, 2360 },
                    { Stats.DefenseBase, 820 },
                    { Stats.AttackRatePvm, 2647 },
                    { Stats.DefenseRatePvm, 970 },
                    { Stats.PoisonResistance, 150 },
                    { Stats.IceResistance, 30 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 564;
                monster.Designation = "Dark Coolutin";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 8;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 145 },
                    { Stats.MaximumHealth, 248000 },
                    { Stats.MinimumPhysBaseDmg, 2043 },
                    { Stats.MaximumPhysBaseDmg, 2392 },
                    { Stats.DefenseBase, 850 },
                    { Stats.AttackRatePvm, 2142 },
                    { Stats.DefenseRatePvm, 1440 },
                    { Stats.PoisonResistance, 150 },
                    { Stats.IceResistance, 30 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 565;
                monster.Designation = "Dark Iron Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 8;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 148 },
                    { Stats.MaximumHealth, 265000 },
                    { Stats.MinimumPhysBaseDmg, 2217 },
                    { Stats.MaximumPhysBaseDmg, 2601 },
                    { Stats.DefenseBase, 860 },
                    { Stats.AttackRatePvm, 2441 },
                    { Stats.DefenseRatePvm, 1430 },
                    { Stats.PoisonResistance, 150 },
                    { Stats.IceResistance, 30 },
                    { Stats.LightningResistance, 30 },
                    { Stats.FireResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
