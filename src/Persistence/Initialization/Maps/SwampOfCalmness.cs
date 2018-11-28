// <copyright file="SwampOfCalmness.cs" company="MUnique">
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
    /// The initialization for the Swamp Of Calmness map.
    /// </summary>
    internal class SwampOfCalmness : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the swamp of calmness map.
        /// </summary>
        public static readonly byte Number = 56;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Swamp Of Calmness"; // Kanturu Refinery Tower

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters: 1 O'Clock Island
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 192, 192); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 202, 202); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 215, 215); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 235, 235); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 220, 220); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 203, 203); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 229, 229); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 232, 232); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 218, 218); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 224, 224); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 202, 202); // Sapi-Tres

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 203, 203); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 231, 231); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 048, 048, 227, 227); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 237, 237); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 230, 230); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 223, 223); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 200, 200); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 235, 235); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 226, 226); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 192, 192); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 083, 083, 219, 219); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 184, 184); // Ghost Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 012, 012, 182, 182); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 019, 019, 182, 182); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 010, 010, 192, 192); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 014, 014, 224, 224); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 020, 020, 233, 233); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 186, 186); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 025, 025, 209, 209); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 234, 234); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 020, 020, 211, 211); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 030, 030, 230, 230); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 085, 085, 223, 223); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 212, 212); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 205, 205); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 020, 020, 207, 207); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 188, 188); // Blaze Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[449], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 183, 183); // Blaze Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 210, 210); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 232, 232); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 225, 225); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 186, 186); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 164, 164); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 167, 167); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 170, 170); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 168, 168); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 231, 231); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 229, 229); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 106, 106, 191, 191); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 188, 188); // Sapi Queen

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 224, 224); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 219, 219); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 231, 231); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 230, 230); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 190, 190); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 185, 185); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 166, 166); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 167, 167); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 165, 165); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 189, 189); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 229, 229); // Ice Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 229, 229); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 223, 223); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 210, 210); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 185, 185); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 183, 183); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 169, 169); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 169, 169); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 170, 170); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 213, 213); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 213, 213); // Shadow Master

            // Monsters: 5 O'Clock Island
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 198, 198); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 202, 202); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 208, 208); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 214, 214); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 215, 215); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 222, 222); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 184, 184); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 184, 184); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 203, 203, 198, 198); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 204, 204); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 202, 202); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 201, 201); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 210, 210); // Sapi-Unus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[441], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 210, 210); // Sapi-Unus

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 196, 196); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 192, 192); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 217, 217); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 217, 217); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 177, 177); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 173, 173); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 171, 171); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 169, 169); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 197, 197); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 226, 226); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 228, 228); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 234, 234); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 194, 194); // Sapi-Duo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[442], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 194, 194); // Sapi-Duo

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 222, 222); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 218, 218); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 223, 223); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 233, 233); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 234, 234); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 234, 234); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 234, 234); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 235, 235); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 238, 238, 233, 233); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 238, 238); // Thunder Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[447], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 227, 227); // Thunder Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 150, 150); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 142, 142); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 152, 152); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 137, 137); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 131, 131); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 127, 127); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 150, 150); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 200, 200, 145, 145); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 153, 153); // Sapi Queen

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 136, 136); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 148, 148); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 151, 151); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 133, 133); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 142, 142); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 123, 123); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 130, 130); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 186, 186, 113, 113); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 108, 108); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 139, 139); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 127, 127); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 147, 147); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 148, 148); // Ice Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 113, 113); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 112, 112); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 110, 110); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 124, 124); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 129, 129); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 143, 143); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 136, 136); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 144, 144); // Shadow Master

            // Monsters: 7 O'Clock Island
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 010, 010); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 010, 010); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 014, 014); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 024, 024); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 027, 027); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 029, 029); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 039, 039); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 056, 056); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 068, 068); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 080, 080); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 088, 088); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 221, 221, 088, 088); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 082, 082); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 011, 011); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 030, 030); // Shadow Pawn
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[444], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 072, 072); // Shadow Pawn

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 009, 009); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 015, 015); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 014, 014); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 021, 021); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 028, 028); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 027, 027); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 033, 033); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 044, 044); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 063, 063); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 074, 074); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 086, 086); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 086, 086); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 074, 074); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 080, 080); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 089, 089); // Shadow Knight

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 049, 049); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 068, 068); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 082, 082); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 221, 221, 084, 084); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 090, 090); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 027, 027); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 034, 034); // Ghost Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[448], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 077, 077); // Ghost Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 041, 041); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 041, 041); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 162, 162, 041, 041); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 021, 021); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 019, 019); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 022, 022); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 028, 028); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 025, 025); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 026, 026); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 043, 043); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 148, 148, 026, 026); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 145, 145, 023, 023); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 017, 017); // Sapi Queen

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 153, 153, 039, 039); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 157, 157, 036, 036); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 157, 157, 038, 038); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 014, 014); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 028, 028); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 034, 034); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 029, 029); // Ice Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 045, 045); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 145, 145, 046, 046); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 014, 014); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 044, 044); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 028, 028); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 017, 017); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 131, 131, 031, 031); // Shadow Master

            // Monsters: 11 O'Clock Island
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 044, 044); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 044, 044); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 051, 051); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 040, 040, 049, 049); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 024, 024, 049, 049); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 010, 010, 044, 044); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 010, 010, 050, 050); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 013, 013, 047, 047); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 044, 044); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 011, 011); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 027, 027); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 020, 020); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 015, 015); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 023, 023); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 093, 020, 020); // Sapi-Tres
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[443], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 026, 026); // Sapi-Tres

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 053, 053); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 053, 053); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 043, 043); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 009, 009, 047, 047); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 008, 008, 034, 034); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 009, 009, 022, 022); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 012, 012, 011, 011); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 019, 019, 011, 011); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 023, 023); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 023, 023); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 054, 054, 015, 015); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 055, 055, 024, 024); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 014, 014); // Shadow Knight
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[445], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 016, 016); // Shadow Knight

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 016, 016, 015, 015); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 024, 024, 010, 010); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 011, 011); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 020, 020, 030, 030); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 013, 013, 032, 032); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 018, 018, 045, 045); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 027, 027); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 020, 020); // Shadow Look
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[446], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 023, 023); // Shadow Look

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 012, 012, 087, 087); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 014, 014, 085, 085); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 021, 021, 128, 128); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 126, 126); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 102, 102); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 097, 097); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 120, 120); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 040, 040, 117, 117); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 109, 109); // Sapi Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[557], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 075, 075); // Sapi Queen

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 015, 015, 089, 089); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 100, 100); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 012, 012, 112, 112); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 021, 021, 124, 124); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 024, 024, 121, 121); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 023, 023, 112, 112); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 123, 123); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 043, 043, 118, 118); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 114, 114); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 108, 108); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 104, 104); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 078, 078); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 015, 015, 127, 127); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 011, 011, 118, 118); // Ice Napin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[558], 1, Direction.Undefined, SpawnTrigger.Automatic, 018, 018, 112, 112); // Ice Napin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 054, 054, 113, 113); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 097, 097); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 082, 082); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 079, 079); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 056, 056, 077, 077); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 123, 123); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 111, 111); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 105, 105); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 026, 026, 116, 116); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 014, 014, 104, 104); // Shadow Master
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[559], 1, Direction.Undefined, SpawnTrigger.Automatic, 013, 013, 093, 093); // Shadow Master
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 441;
                monster.Designation = "Sapi-Unus";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 95 },
                    { Stats.MaximumHealth, 37000 },
                    { Stats.MinimumPhysBaseDmg, 475 },
                    { Stats.MaximumPhysBaseDmg, 510 },
                    { Stats.DefenseBase, 370 },
                    { Stats.AttackRatePvm, 700 },
                    { Stats.DefenseRatePvm, 220 },
                    { Stats.PoisonResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 441 Sapi-Unus

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 442;
                monster.Designation = "Sapi-Duo";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 96 },
                    { Stats.MaximumHealth, 39500 },
                    { Stats.MinimumPhysBaseDmg, 485 },
                    { Stats.MaximumPhysBaseDmg, 505 },
                    { Stats.DefenseBase, 375 },
                    { Stats.AttackRatePvm, 730 },
                    { Stats.DefenseRatePvm, 225 },
                    { Stats.PoisonResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 442 Sapi-Duo

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 443;
                monster.Designation = "Sapi-Tres";
                monster.MoveRange = 4;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 102 },
                    { Stats.MaximumHealth, 58000 },
                    { Stats.MinimumPhysBaseDmg, 590 },
                    { Stats.MaximumPhysBaseDmg, 625 },
                    { Stats.DefenseBase, 470 },
                    { Stats.AttackRatePvm, 880 },
                    { Stats.DefenseRatePvm, 275 },
                    { Stats.PoisonResistance, 40 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 443 Sapi-Tres

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 444;
                monster.Designation = "Shadow Pawn";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 48500 },
                    { Stats.MinimumPhysBaseDmg, 540 },
                    { Stats.MaximumPhysBaseDmg, 575 },
                    { Stats.DefenseBase, 430 },
                    { Stats.AttackRatePvm, 830 },
                    { Stats.DefenseRatePvm, 240 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 444 Shadow Pawn

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 445;
                monster.Designation = "Shadow Knight";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 52000 },
                    { Stats.MinimumPhysBaseDmg, 570 },
                    { Stats.MaximumPhysBaseDmg, 600 },
                    { Stats.DefenseBase, 455 },
                    { Stats.AttackRatePvm, 860 },
                    { Stats.DefenseRatePvm, 255 },
                    { Stats.PoisonResistance, 30 },
                    { Stats.IceResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 445 Shadow Knight

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 446;
                monster.Designation = "Shadow Look";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 104 },
                    { Stats.MaximumHealth, 62000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 645 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 950 },
                    { Stats.DefenseRatePvm, 290 },
                    { Stats.PoisonResistance, 40 },
                    { Stats.IceResistance, 40 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 446 Shadow Look

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 447;
                monster.Designation = "Thunder Napin";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 42000 },
                    { Stats.MinimumPhysBaseDmg, 520 },
                    { Stats.MaximumPhysBaseDmg, 555 },
                    { Stats.DefenseBase, 395 },
                    { Stats.AttackRatePvm, 750 },
                    { Stats.DefenseRatePvm, 235 },
                    { Stats.LightningResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 447 Thunder Napin

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 448;
                monster.Designation = "Ghost Napin";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 106 },
                    { Stats.MaximumHealth, 68000 },
                    { Stats.MinimumPhysBaseDmg, 635 },
                    { Stats.MaximumPhysBaseDmg, 665 },
                    { Stats.DefenseBase, 535 },
                    { Stats.AttackRatePvm, 983 },
                    { Stats.DefenseRatePvm, 295 },
                    { Stats.PoisonResistance, 30 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 448 Ghost Napin

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 449;
                monster.Designation = "Blaze Napin";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 107 },
                    { Stats.MaximumHealth, 70000 },
                    { Stats.MinimumPhysBaseDmg, 670 },
                    { Stats.MaximumPhysBaseDmg, 725 },
                    { Stats.DefenseBase, 530 },
                    { Stats.AttackRatePvm, 1000 },
                    { Stats.DefenseRatePvm, 303 },
                    { Stats.FireResistance, 40 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 449 Blaze Napin

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 557;
                monster.Designation = "Sapi Queen";
                monster.MoveRange = 4;
                monster.AttackRange = 3;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 131 },
                    { Stats.MaximumHealth, 218000 },
                    { Stats.MinimumPhysBaseDmg, 1441 },
                    { Stats.MaximumPhysBaseDmg, 2017 },
                    { Stats.DefenseBase, 670 },
                    { Stats.AttackRatePvm, 1847 },
                    { Stats.DefenseRatePvm, 875 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 557 Sapi Queen

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 558;
                monster.Designation = "Ice Napin";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 135 },
                    { Stats.MaximumHealth, 230000 },
                    { Stats.MinimumPhysBaseDmg, 1585 },
                    { Stats.MaximumPhysBaseDmg, 2060 },
                    { Stats.DefenseBase, 730 },
                    { Stats.AttackRatePvm, 1544 },
                    { Stats.DefenseRatePvm, 903 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 558 Ice Napin

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 559;
                monster.Designation = "Shadow Master";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2550 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 137 },
                    { Stats.MaximumHealth, 242000 },
                    { Stats.MinimumPhysBaseDmg, 1743 },
                    { Stats.MaximumPhysBaseDmg, 2092 },
                    { Stats.DefenseBase, 700 },
                    { Stats.AttackRatePvm, 1646 },
                    { Stats.DefenseRatePvm, 990 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 20 },
                    { Stats.FireResistance, 20 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            } // 559 Shadow Master
        }
    }
}
