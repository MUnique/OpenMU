// <copyright file="LandOfTrials.cs" company="MUnique">
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
    /// The initialization for the Land of Trials map.
    /// </summary>
    internal class LandOfTrials : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Land of Trials map.
        /// </summary>
        public static readonly byte Number = 31;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Land_of_Trials";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 103, 103); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 077, 077, 102, 102); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 085, 085); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 075, 075); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 070, 070, 065, 065); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 036, 036); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 109, 109); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 022, 022); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 061, 061); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 067, 067); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 065, 065); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 075, 075); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 066, 066); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 055, 055); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 095, 095); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 087, 087); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 063, 063); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 053, 053); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 039, 039); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 021, 021); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 060, 060); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 054, 054, 065, 065); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 059, 059); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 074, 074); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 083, 083); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 094, 094, 092, 092); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 139, 139, 050, 050); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 157, 157, 034, 034); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 055, 055); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 172, 172, 062, 062); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 142, 142, 066, 066); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 153, 153, 059, 059); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 070, 070); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 162, 162, 061, 061); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 070, 070); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 074, 074); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 082, 082); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 089, 089); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 077, 077); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 077, 077); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 081, 081); // Lizard Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[290], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 091, 091); // Lizard Warrior

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 190, 190); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 088, 088, 183, 183); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 141, 141); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 213, 213); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 048, 048, 207, 207); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 018, 018, 173, 173); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 172, 172); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 185, 185); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 031, 031, 209, 209); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 040, 040, 231, 231); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 056, 056, 227, 227); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 221, 221); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 080, 080, 234, 234); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 173, 173); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 229, 229); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 227, 227); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 145, 145, 209, 209); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 198, 198); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 131, 131, 215, 215); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 225, 225); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 188, 188); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 221, 221); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 211, 211); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 217, 217); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 199, 199); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 204, 204); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 192, 192); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 175, 175); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 183, 183); // Fire Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[291], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 167, 167); // Fire Golem

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 098, 098, 175, 175); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 147, 147); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 222, 222); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 048, 048, 221, 221); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 232, 232); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 075, 075, 204, 204); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 174, 174); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 185, 185); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 194, 194); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 225, 225); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 200, 200); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 159, 159); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 227, 227); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 177, 177); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 182, 182); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 173, 173); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 151, 151); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 140, 140); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 127, 127); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 120, 120); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 112, 112); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 146, 146, 126, 126); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 159, 159, 121, 121); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 132, 132); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 168, 168); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 182, 182); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 169, 169); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 224, 224); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 221, 221); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 225, 225); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 218, 218); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 146, 146); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 145, 145); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 136, 136); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 133, 133); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 095, 095, 138, 138); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 204, 204); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 208, 208); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 180, 180); // Queen Bee
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[292], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 191, 191); // Queen Bee

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 132, 132); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 124, 124); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 110, 110); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 119, 119); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 116, 116); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 102, 102); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 098, 098); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 086, 086); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 074, 074); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 077, 077); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 134, 134); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 076, 076, 140, 140); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 070, 070, 149, 149); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 150, 150); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 124, 124); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 066, 066, 120, 120); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 113, 113); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 080, 080, 111, 111); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 088, 088); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 088, 088); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 131, 131, 079, 079); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 083, 083); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 079, 079); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 089, 089); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 086, 086); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 105, 105); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 099, 099); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 186, 186, 092, 092); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 129, 129); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 138, 138); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 166, 166, 122, 122); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 121, 121); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 095, 095); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 087, 087, 140, 140); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 133, 133); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 138, 138); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 143, 143); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 098, 098); // Poison Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[293], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 106, 106); // Poison Golem

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 042, 042); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 031, 031); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 015, 015); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 012, 012); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 026, 026); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 012, 012); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 014, 014); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 020, 020); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 028, 028); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 221, 221, 017, 017); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 025, 025); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 031, 031); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 031, 031); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 039, 039); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 045, 045); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 054, 054); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 012, 012); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 011, 011); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 019, 019); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 017, 017); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 035, 035); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 040, 040); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 041, 041); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 037, 037); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 043, 043); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 039, 039); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 078, 078, 045, 045); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 050, 050); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 053, 053); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 035, 035); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 039, 039); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 036, 036); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 028, 028); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 052, 052); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 043, 043, 062, 062); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 036, 036); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 029, 029); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 029, 029); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 037, 037); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 046, 046); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 037, 037); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 033, 033); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 037, 037); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 045, 045); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 056, 056); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 071, 071); // Ax Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[294], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 069, 069); // Ax Warrior

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[295], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 211, 211); // Erohim
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 290;
                monster.Designation = "Lizard Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 78 },
                    { Stats.MaximumHealth, 15000 },
                    { Stats.MinimumPhysBaseDmg, 270 },
                    { Stats.MaximumPhysBaseDmg, 320 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 140 },
                    { Stats.PoisonResistance, 26 },
                    { Stats.IceResistance, 26 },
                    { Stats.LightningResistance, 26 },
                    { Stats.FireResistance, 26 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 291;
                monster.Designation = "Fire Golem";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 102 },
                    { Stats.MaximumHealth, 55000 },
                    { Stats.MinimumPhysBaseDmg, 560 },
                    { Stats.MaximumPhysBaseDmg, 600 },
                    { Stats.DefenseBase, 550 },
                    { Stats.AttackRatePvm, 870 },
                    { Stats.DefenseRatePvm, 310 },
                    { Stats.PoisonResistance, 29 },
                    { Stats.IceResistance, 29 },
                    { Stats.LightningResistance, 29 },
                    { Stats.FireResistance, 29 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 292;
                monster.Designation = "Queen Bee";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 92 },
                    { Stats.MaximumHealth, 34500 },
                    { Stats.MinimumPhysBaseDmg, 489 },
                    { Stats.MaximumPhysBaseDmg, 540 },
                    { Stats.DefenseBase, 360 },
                    { Stats.AttackRatePvm, 620 },
                    { Stats.DefenseRatePvm, 240 },
                    { Stats.PoisonResistance, 28 },
                    { Stats.IceResistance, 28 },
                    { Stats.LightningResistance, 28 },
                    { Stats.FireResistance, 28 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 293;
                monster.Designation = "Poison Golem";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 84 },
                    { Stats.MaximumHealth, 25000 },
                    { Stats.MinimumPhysBaseDmg, 375 },
                    { Stats.MaximumPhysBaseDmg, 425 },
                    { Stats.DefenseBase, 275 },
                    { Stats.AttackRatePvm, 530 },
                    { Stats.DefenseRatePvm, 190 },
                    { Stats.PoisonResistance, 27 },
                    { Stats.IceResistance, 27 },
                    { Stats.LightningResistance, 27 },
                    { Stats.FireResistance, 27 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 294;
                monster.Designation = "Ax Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 75 },
                    { Stats.MaximumHealth, 11500 },
                    { Stats.MinimumPhysBaseDmg, 255 },
                    { Stats.MaximumPhysBaseDmg, 290 },
                    { Stats.DefenseBase, 195 },
                    { Stats.AttackRatePvm, 385 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.LightningResistance, 25 },
                    { Stats.FireResistance, 25 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 295;
                monster.Designation = "Erohim";
                monster.MoveRange = 3;
                monster.AttackRange = 6;
                monster.ViewRange = 8;
                monster.MoveDelay = new TimeSpan(650 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(43200 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 128 },
                    { Stats.MaximumHealth, 3000000 },
                    { Stats.MinimumPhysBaseDmg, 1500 },
                    { Stats.MaximumPhysBaseDmg, 2000 },
                    { Stats.DefenseBase, 1000 },
                    { Stats.AttackRatePvm, 1500 },
                    { Stats.DefenseRatePvm, 800 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
                    { Stats.LightningResistance, 100 },
                    { Stats.FireResistance, 100 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }
        }
    }
}
