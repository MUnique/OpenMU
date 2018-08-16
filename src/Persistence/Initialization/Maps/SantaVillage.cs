// <copyright file="SantaVillage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The initialization for the Santa Village map.
    /// </summary>
    internal class SantaVillage : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the santa village map.
        /// </summary>
        public static readonly byte Number = 62;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Santa Village";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[467], 1, 1, SpawnTrigger.Automatic, 202, 202, 041, 041); // Snowman
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[468], 1, 1, SpawnTrigger.Automatic, 222, 222, 024, 024); // Little Santa Yellow
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[469], 1, 1, SpawnTrigger.Automatic, 202, 202, 033, 033); // Little Santa Green
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[470], 1, 1, SpawnTrigger.Automatic, 192, 192, 024, 024); // Little Santa Red
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[471], 1, 1, SpawnTrigger.Automatic, 207, 207, 009, 009); // Little Santa Blue
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[472], 1, 1, SpawnTrigger.Automatic, 225, 225, 011, 011); // Little Santa White
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[473], 1, 1, SpawnTrigger.Automatic, 232, 232, 013, 013); // Little Santa Black
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[474], 1, 1, SpawnTrigger.Automatic, 216, 216, 019, 019); // Little Santa Orange
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[475], 1, 1, SpawnTrigger.Automatic, 193, 193, 027, 027); // Little Santa Pink
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}
