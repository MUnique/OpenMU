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
        /// <inheritdoc/>
        protected override byte MapNumber => 6;

        /// <inheritdoc/>
        protected override string MapName => "Arena";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IRepositoryManager repositoryManager, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, gameConfiguration.Monsters.First(m => m.Number == 240), 1, 1, SpawnTrigger.Automatic, 58, 58, 140, 140);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, gameConfiguration.Monsters.First(m => m.Number == 200), 1, 1, SpawnTrigger.Automatic, 63, 63, 160, 160);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, gameConfiguration.Monsters.First(m => m.Number == 239), 1, 1, SpawnTrigger.Automatic, 67, 67, 140, 140);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
        {
            // no monsters to create
        }
    }
}
