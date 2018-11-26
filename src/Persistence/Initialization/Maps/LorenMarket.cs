// <copyright file="LorenMarket.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The initialization for the LorenMarket map.
    /// </summary>
    internal class LorenMarket : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the LorenMarket map.
        /// </summary>
        public static readonly byte Number = 79;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "LorenMarket";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            // yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 045, 045, 075, 075); // Safety Guardian
            // yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 077, 077, 045, 045); // Safety Guardian
            // yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[385], 1, Direction.SouthWest, SpawnTrigger.Automatic, 014, 014, 067, 067); // Mirage
            // yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[540], 1, Direction.SouthEast, SpawnTrigger.Automatic, 086, 086, 129, 129); // Lugard
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[545], 1, Direction.SouthEast, SpawnTrigger.Automatic, 113, 113, 116, 116); // Christine the General Goods Merchant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[546], 1, Direction.SouthEast, SpawnTrigger.Automatic, 131, 131, 109, 109); // Jeweler Raul
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[547], 1, Direction.SouthEast, SpawnTrigger.Automatic, 123, 123, 140, 140); // Market Union Member Julia
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            // no monsters here
        }
    }
}