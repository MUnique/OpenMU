// <copyright file="FortressOfImperialGuardian4.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Map initialization for the Empire fortress 4 event map.
    /// </summary>
    internal class FortressOfImperialGuardian4 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FortressOfImperialGuardian4"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public FortressOfImperialGuardian4(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        protected override byte MapNumber => 72;

        /// <inheritdoc />
        protected override string MapName => "Fortress of Imperial Guardian 4";

        /// <inheritdoc />
        protected override void CreateMonsters()
        {
            // All Monsters and NPCs are defined in the first map.
        }

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(npcDictionary[518], 64, 65, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 64, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 64, 73, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 61, 64, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 61, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 61, 74, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 55, 71, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 55, 66, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 54, 68, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[507], 57, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 29, 186, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 34, 187, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 39, 186, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 34, 194, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 29, 192, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 39, 192, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 26, 191, (Direction)2, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 32, 195, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 39, 196, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[506], 36, 191, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 173, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 170, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 173, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 181, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 181, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 181, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 184, 129, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 184, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 184, 137, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[505], 177, 133, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 183, 29, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 183, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 187, 32, (Direction)1, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[521], 187, 20, (Direction)5, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 192, 31, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 192, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[518], 194, 26, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 189, 21, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 189, 26, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[519], 189, 31, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[504], 183, 24, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 81, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 32, 90, (Direction)1, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[527], 50, 69, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 52, 77, (Direction)2, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 53, 61, (Direction)4, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 34, 176, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 69, 166, (Direction)5, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[527], 52, 191, (Direction)7, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 21, 198, (Direction)2, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 47, 199, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 156, 132, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 224, 159, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[527], 197, 132, (Direction)7, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 161, 139, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 161, 127, (Direction)5, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[528], 214, 24, (Direction)3, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 207, 32, (Direction)1, SpawnTrigger.OnceAtEventStart); // 3 1
            yield return this.CreateMonsterSpawn(npcDictionary[526], 207, 20, (Direction)5, SpawnTrigger.OnceAtEventStart); // 3 1

            // Traps
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 45, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 41, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 37, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 37, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 33, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 33, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 29, 67, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 29, 74, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 31, 78, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 31, 83, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 31, 87, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 57, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 61, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 65, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 69, 191, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 68, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 68, 181, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 68, 176, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 68, 171, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 202, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 206, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 210, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 214, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 218, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 226, 133, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 224, 137, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 224, 142, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 224, 147, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 224, 152, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
            yield return this.CreateMonsterSpawn(npcDictionary[523], 224, 155, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 2 1
        }
    }
}