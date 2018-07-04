// <copyright file="IllusionTemple2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the Illusion Temple 2.
    /// </summary>
    internal class IllusionTemple2 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Illusion Temple 2 map.
        /// </summary>
        public const byte Number = 46;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Illusion Temple 2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[658], 1, 0, SpawnTrigger.AutomaticDuringEvent, 169, 169, 085, 085); // Cursed Statue
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[659], 1, 0, SpawnTrigger.AutomaticDuringEvent, 136, 136, 101, 101); // Captured Stone Statue (1)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[660], 1, 0, SpawnTrigger.AutomaticDuringEvent, 151, 151, 119, 119); // Captured Stone Statue (2)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[661], 1, 0, SpawnTrigger.AutomaticDuringEvent, 150, 150, 088, 088); // Captured Stone Statue (3)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[662], 1, 0, SpawnTrigger.AutomaticDuringEvent, 165, 165, 102, 102); // Captured Stone Statue (4)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[663], 1, 0, SpawnTrigger.AutomaticDuringEvent, 173, 173, 067, 067); // Captured Stone Statue (5)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[664], 1, 0, SpawnTrigger.AutomaticDuringEvent, 187, 187, 081, 081); // Captured Stone Statue (6)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[665], 1, 0, SpawnTrigger.AutomaticDuringEvent, 187, 187, 051, 051); // Captured Stone Statue (7)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[666], 1, 0, SpawnTrigger.AutomaticDuringEvent, 203, 203, 067, 067); // Captured Stone Statue (8)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[667], 1, 0, SpawnTrigger.AutomaticDuringEvent, 133, 133, 121, 121); // Captured Stone Statue (9)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[668], 1, 0, SpawnTrigger.AutomaticDuringEvent, 206, 206, 048, 048); // Captured Stone Statue (10)
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}
