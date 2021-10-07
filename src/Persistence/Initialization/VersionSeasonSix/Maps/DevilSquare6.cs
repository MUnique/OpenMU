// <copyright file="DevilSquare6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 5.
    /// </summary>
    internal class DevilSquare6 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare6"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare6(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 32;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 6";

        /// <inheritdoc/>
        protected override int Discriminator => 6;

        /// <inheritdoc/>
        protected override byte SafezoneMapNumber => Noria.Number;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[449], 122, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Blaze Napin
            yield return this.CreateMonsterSpawn(this.NpcDictionary[575], 122, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent); // Condra
        }
    }
}