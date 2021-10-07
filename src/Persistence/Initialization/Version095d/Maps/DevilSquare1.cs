// <copyright file="DevilSquare1.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 1.
    /// </summary>
    internal class DevilSquare1 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare1"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare1(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 1";

        /// <inheritdoc/>
        protected override int Discriminator => 1;

        /// <inheritdoc/>
        protected override byte SafezoneMapNumber => Noria.Number;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 119, 150, 80, 115, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 119, 150, 80, 115, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
        }
    }
}
