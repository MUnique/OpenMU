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
        /// Initializes a new instance of the <see cref="IllusionTemple2"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public IllusionTemple2(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 46;

        /// <inheritdoc/>
        protected override string MapName => "Illusion Temple 2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[658], 169, 085, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Cursed Statue
            yield return this.CreateMonsterSpawn(npcDictionary[659], 136, 101, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (1)
            yield return this.CreateMonsterSpawn(npcDictionary[660], 151, 119, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (2)
            yield return this.CreateMonsterSpawn(npcDictionary[661], 150, 088, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (3)
            yield return this.CreateMonsterSpawn(npcDictionary[662], 165, 102, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (4)
            yield return this.CreateMonsterSpawn(npcDictionary[663], 173, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (5)
            yield return this.CreateMonsterSpawn(npcDictionary[664], 187, 081, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (6)
            yield return this.CreateMonsterSpawn(npcDictionary[665], 187, 051, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (7)
            yield return this.CreateMonsterSpawn(npcDictionary[666], 203, 067, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (8)
            yield return this.CreateMonsterSpawn(npcDictionary[667], 133, 121, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (9)
            yield return this.CreateMonsterSpawn(npcDictionary[668], 206, 048, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Captured Stone Statue (10)
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            // no monsters here
        }
    }
}
