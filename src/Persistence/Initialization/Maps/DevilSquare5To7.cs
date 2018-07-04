// <copyright file="DevilSquare5To7.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization for the devil square map which hosts devil square 5 to 7.
    /// </summary>
    internal class DevilSquare5To7 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the devil square 5 to 7 map.
        /// </summary>
        public const byte Number = 32;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Devil Square (5-7)";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 35, 0, SpawnTrigger.AutomaticDuringEvent, 120, 150, 80, 115); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 35, 0, SpawnTrigger.AutomaticDuringEvent, 120, 150, 80, 115); // Crypos

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 35, 0, SpawnTrigger.AutomaticDuringEvent, 122, 151, 152, 184); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 35, 0, SpawnTrigger.AutomaticDuringEvent, 122, 151, 152, 184); // Condra

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 35, 0, SpawnTrigger.AutomaticDuringEvent, 50, 79, 138, 173); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 35, 0, SpawnTrigger.AutomaticDuringEvent, 50, 79, 138, 173); // Crypos
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}
