// <copyright file="Lorencia.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The initialization for the Lorencia map.
    /// </summary>
    internal class Lorencia : Version075.Maps.Lorencia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lorencia"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Lorencia(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override string TerrainVersionPrefix => string.Empty;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
        {
            foreach (var npc in base.CreateNpcSpawns())
            {
                yield return npc;
            }

            yield return this.CreateMonsterSpawn(this.NpcDictionary[230], 62, 130, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[226], 122, 110, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[236], 175, 120, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[257], 96, 129, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[257], 174, 129, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[257], 130, 128, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[257], 132, 165, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[229], 136, 88, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[375], 132, 161, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[543], 141, 143, Direction.South);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[371], 130, 126, Direction.SouthEast);
        }
    }
}
