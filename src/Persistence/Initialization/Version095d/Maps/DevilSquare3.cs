// <copyright file="DevilSquare3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 3.
    /// </summary>
    internal class DevilSquare3 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevilSquare3"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public DevilSquare3(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 9;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square 3";

        /// <inheritdoc/>
        protected override int Discriminator => 3;

        /// <inheritdoc/>
        protected override byte SafezoneMapNumber => Noria.Number;

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 49, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 49, 79, 138, 173, 35, Direction.Undefined, SpawnTrigger.AutomaticDuringEvent);
        }
    }
}