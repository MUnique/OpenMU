// <copyright file="FortressOfImperialGuardian2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Map initialization for the Empire fortress 2 event map.
    /// </summary>
    internal class FortressOfImperialGuardian2 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FortressOfImperialGuardian2"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public FortressOfImperialGuardian2(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        protected override byte MapNumber => 70;

        /// <inheritdoc />
        protected override string MapName => "Fortress of Imperial Guardian 2";

        /// <inheritdoc />
        protected override void CreateMonsters()
        {
            // All Monsters and NPCs are defined in the first map.
        }

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(npcDictionary[520], 66, 59, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[520], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[520], 66, 70, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 58, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[514], 59, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[518], 32, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[518], 42, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[518], 38, 101, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 33, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 44, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 39, 110, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[515], 38, 106, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[521], 98, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[521], 96, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[521], 98, 117, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 93, 105, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 92, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[519], 93, 118, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[509], 89, 111, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[525], 75, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[525], 19, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[524], 50, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 24, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 24, 62, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[525], 37, 93, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[525], 55, 154, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[524], 41, 117, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 53, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 56, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[525], 107, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 85, 113, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[526], 85, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 3
            yield return this.CreateMonsterSpawn(npcDictionary[520], 66, 59, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[520], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[520], 66, 70, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 58, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 56, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[514], 59, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[518], 32, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[518], 42, 99, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[518], 38, 101, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 33, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 44, 107, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 39, 110, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[515], 38, 106, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[521], 98, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[521], 96, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[521], 98, 117, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 93, 105, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 92, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[519], 93, 118, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[507], 89, 111, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[525], 75, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[525], 19, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[524], 50, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 24, 67, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 24, 62, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[525], 37, 93, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[525], 55, 154, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[524], 41, 117, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 53, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 56, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[525], 107, 112, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 85, 113, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6
            yield return this.CreateMonsterSpawn(npcDictionary[526], 85, 107, (Direction)3, SpawnTrigger.OnceAtEventStart); // 2 6

            // Traps:
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 42, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 39, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 35, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 32, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 29, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 26, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 24, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 22, 66, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 122, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 126, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 130, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 50, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 129, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 134, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 139, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 144, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 149, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 3
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 42, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 39, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 35, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 32, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 29, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 26, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 24, 62, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 22, 66, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 122, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 126, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 130, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 50, 128, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 129, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 134, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 139, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 144, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
            yield return this.CreateMonsterSpawn(npcDictionary[523], 55, 149, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 6
        }
    }
}