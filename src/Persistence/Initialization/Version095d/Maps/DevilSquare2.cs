// <copyright file="DevilSquare2.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 1 to 4.
    /// </summary>
    internal class DevilSquare2 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare2"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare2(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 2";

        /// <inheritdoc/>
        protected override int Discriminator => 2;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 121, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 121, 151, 152, 184, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
        }
    }
}