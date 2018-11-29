// <copyright file="Arena.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the Arena map.
    /// </summary>
    internal class Arena : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Arena"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Arena(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 6;

        /// <inheritdoc/>
        protected override string MapName => "Arena";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            yield return this.CreateMonsterSpawn(this.GameConfiguration.Monsters.First(m => m.Number == 240), 58, 58, 140, 140, 1, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.GameConfiguration.Monsters.First(m => m.Number == 200), 63, 63, 160, 160, 1, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.GameConfiguration.Monsters.First(m => m.Number == 239), 67, 67, 140, 140, 1, Direction.SouthWest);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            // no monsters to create
        }
    }
}
