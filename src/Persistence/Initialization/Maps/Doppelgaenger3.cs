// <copyright file="Doppelgaenger3.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Map initialization for the Doppelgaenger 3 (a.k.a. "Double Gear", "Double Goer", etc.) event map.
    /// </summary>
    internal class Doppelgaenger3 : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Doppelgaenger3"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Doppelgaenger3(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        protected override byte MapNumber => 67;

        /// <inheritdoc />
        protected override string MapName => "Doppelgaenger 3";

        /// <inheritdoc />
        protected override void CreateMonsters()
        {
            // no special monsters required
        }

        /// <inheritdoc />
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            // event map, spawns controlled by event
            yield break;
        }
    }
}