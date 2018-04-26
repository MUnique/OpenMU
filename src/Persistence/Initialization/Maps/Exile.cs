// <copyright file="Exile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// The initialization for the Exile map.
    /// </summary>
    internal class Exile : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 5;

        /// <inheritdoc/>
        protected override string MapName => "Exile";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            yield break;
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}
