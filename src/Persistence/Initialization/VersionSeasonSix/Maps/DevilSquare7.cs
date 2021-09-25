// <copyright file="DevilSquare7.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 7.
    /// </summary>
    internal class DevilSquare7 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare7"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare7(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 32;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 7";

        /// <inheritdoc/>
        protected override int Discriminator => 7;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[573], 50, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Crypta
            yield return this.CreateMonsterSpawn(this.NpcDictionary[574], 50, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Crypos
        }
    }
}