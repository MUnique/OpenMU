// <copyright file="FortressOfImperialGuardian3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Map initialization for the Empire fortress 3 event map.
    /// </summary>
    internal class FortressOfImperialGuardian3 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FortressOfImperialGuardian3"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public FortressOfImperialGuardian3(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        protected override byte MapNumber => 71;

        /// <inheritdoc />
        protected override string MapName => "Fortress of Imperial Guardian 3";

        /// <inheritdoc />
        protected override void CreateMonsters()
        {
            // All Monsters and NPCs are defined in the first map.
        }

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(npcDictionary[520], 132, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[520], 131, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[520], 132, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 126, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 124, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 126, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[516], 127, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[518], 217, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[518], 228, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[518], 222, 142, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 218, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 222, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 227, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[517], 222, 147, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[521], 162, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[521], 166, 228, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[521], 171, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 161, 231, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 164, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[519], 171, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[510], 166, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[525], 146, 191, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[525], 89, 195, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[524], 119, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 111, 197, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 111, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[525], 222, 134, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[525], 223, 193, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[524], 222, 160, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 220, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 224, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[525], 167, 217, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 158, 236, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[526], 175, 238, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 4
            yield return this.CreateMonsterSpawn(npcDictionary[520], 132, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[520], 131, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[520], 132, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 126, 185, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 124, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 126, 199, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[516], 127, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[518], 217, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[518], 228, 140, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[518], 222, 142, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 218, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 222, 152, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 227, 148, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[517], 222, 147, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[521], 162, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[521], 166, 228, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[521], 171, 226, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 161, 231, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 164, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[519], 171, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[506], 166, 233, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[525], 146, 191, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[525], 89, 195, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[524], 119, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 111, 197, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 111, 192, (Direction)3, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[525], 222, 134, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[525], 223, 193, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[524], 222, 160, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 220, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 224, 173, (Direction)1, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[525], 167, 217, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 158, 236, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7
            yield return this.CreateMonsterSpawn(npcDictionary[526], 175, 238, (Direction)1, SpawnTrigger.OnceAtEventStart); // 2 7

            // Traps:
            yield return this.CreateMonsterSpawn(npcDictionary[523], 113, 193, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 107, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 102, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 97, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 192, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 97, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 102, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 107, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 165, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 170, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 175, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 180, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 185, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 190, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 4
            yield return this.CreateMonsterSpawn(npcDictionary[523], 113, 193, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 107, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 102, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 97, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 203, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 198, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 192, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 93, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 97, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 102, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 107, 186, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 112, 187, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 0 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 165, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 170, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 175, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 180, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 185, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
            yield return this.CreateMonsterSpawn(npcDictionary[523], 222, 190, Direction.Undefined, SpawnTrigger.OnceAtEventStart); // 1 7
        }
    }
}