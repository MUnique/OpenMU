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
        /// Initializes a new instance of the <see cref="SantaVillage"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public SantaVillage(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 62;

        /// <inheritdoc/>
        protected override string MapName => "Santa Village";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(npcDictionary[467], 202, 041, Direction.SouthWest); // Snowman
            yield return this.CreateMonsterSpawn(npcDictionary[468], 222, 024, Direction.SouthWest); // Little Santa Yellow
            yield return this.CreateMonsterSpawn(npcDictionary[469], 202, 033, Direction.SouthWest); // Little Santa Green
            yield return this.CreateMonsterSpawn(npcDictionary[470], 192, 024, Direction.SouthWest); // Little Santa Red
            yield return this.CreateMonsterSpawn(npcDictionary[471], 207, 009, Direction.SouthWest); // Little Santa Blue
            yield return this.CreateMonsterSpawn(npcDictionary[472], 225, 011, Direction.SouthWest); // Little Santa White
            yield return this.CreateMonsterSpawn(npcDictionary[473], 232, 013, Direction.SouthWest); // Little Santa Black
            yield return this.CreateMonsterSpawn(npcDictionary[474], 216, 019, Direction.SouthWest); // Little Santa Orange
            yield return this.CreateMonsterSpawn(npcDictionary[475], 193, 027, Direction.SouthWest); // Little Santa Pink
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            // no monsters here
        }
    }
}
