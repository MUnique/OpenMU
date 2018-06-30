// <copyright file="ValleyOfLoren.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The initialization for the Valley of Loren map.
    /// </summary>
    internal class ValleyOfLoren : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Valley of Loren map.
        /// </summary>
        public const byte Number = 30;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Valley of Loren";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[216], 1, 1, SpawnTrigger.Automatic, 176, 176, 212, 212); // Crown
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[216], 1, 1, SpawnTrigger.Automatic, 176, 176, 212, 212); // Crown
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[217], 1, 7, SpawnTrigger.Automatic, 167, 167, 194, 194); // Crown Switch1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[218], 1, 7, SpawnTrigger.Automatic, 184, 184, 195, 195); // Crown Switch2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[220], 1, 3, SpawnTrigger.Automatic, 139, 139, 101, 101); // Guard
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[223], 1, 1, SpawnTrigger.Automatic, 179, 179, 214, 214); // Sinior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[224], 1, 1, SpawnTrigger.Automatic, 086, 086, 061, 061); // Guardsman
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[224], 1, 1, SpawnTrigger.Automatic, 099, 099, 061, 061); // Guardsman
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[376], 1, 1, SpawnTrigger.Automatic, 090, 090, 043, 043); // Pamela
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[377], 1, 3, SpawnTrigger.Automatic, 090, 090, 218, 218); // Angela
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}
