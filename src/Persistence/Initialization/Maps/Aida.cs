// <copyright file="Aida.cs" company="MUnique">
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
    /// The initialization for the Aida map.
    /// </summary>
    internal class Aida : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the aida map.
        /// </summary>
        public static readonly byte Number = 33;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Aida";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[369], 1, 3, SpawnTrigger.Automatic, 078, 078, 013, 013); // Osbourne
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[370], 1, 1, SpawnTrigger.Automatic, 086, 086, 014, 014); // Jerridon

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 088, 088, 073, 073); // Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 106, 106, 049, 049); // Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 120, 120, 097, 097); // Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 125, 125, 082, 082); // Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 137, 137, 125, 125); // Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[304], 1, 0, SpawnTrigger.Automatic, 141, 141, 076, 076); // Witch Queen

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 081, 081, 080, 080); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 083, 083, 121, 121); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 088, 088, 062, 062); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 092, 092, 045, 045); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 092, 092, 086, 086); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 094, 094, 158, 158); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 095, 095, 150, 150); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 102, 102, 175, 175); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 104, 104, 042, 042); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 104, 104, 160, 160); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 111, 111, 094, 094); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 113, 113, 155, 155); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 114, 114, 067, 067); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 125, 125, 112, 112); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 126, 126, 073, 073); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 131, 131, 094, 094); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 133, 133, 079, 079); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 133, 133, 133, 133); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 134, 134, 106, 106); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 136, 136, 066, 066); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 146, 146, 072, 072); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 148, 148, 093, 093); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 157, 157, 166, 166); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 167, 167, 127, 127); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 168, 168, 140, 140); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 168, 168, 168, 168); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 169, 169, 174, 174); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 180, 180, 040, 040); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 185, 185, 147, 147); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 194, 194, 019, 019); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 197, 197, 148, 148); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 208, 208, 168, 168); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 235, 235, 101, 101); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 195, 195, 153, 153); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 117, 117, 073, 073); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 114, 114, 101, 101); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 086, 086, 155, 155); // Blue Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[305], 1, 0, SpawnTrigger.Automatic, 230, 230, 132, 132); // Blue Golem

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 215, 215, 032, 032); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 217, 217, 036, 036); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 085, 085, 145, 145); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 088, 088, 126, 126); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 089, 089, 109, 109); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 093, 093, 173, 173); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 100, 100, 058, 058); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 119, 119, 056, 056); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 126, 126, 156, 156); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 136, 136, 144, 144); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 145, 145, 087, 087); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 149, 149, 112, 112); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 161, 161, 117, 117); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 167, 167, 152, 152); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 176, 176, 124, 124); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 186, 186, 089, 089); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 189, 189, 179, 179); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 190, 190, 144, 144); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 191, 191, 138, 138); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 207, 207, 054, 054); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 209, 209, 075, 075); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 221, 221, 060, 060); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 223, 223, 158, 158); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 227, 227, 052, 052); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 230, 230, 012, 012); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 233, 233, 061, 061); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 234, 234, 071, 071); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 234, 234, 146, 146); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 235, 235, 090, 090); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 214, 214, 035, 035); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 228, 228, 017, 017); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 236, 236, 033, 033); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 237, 237, 038, 038); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 169, 169, 085, 085); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 193, 193, 150, 150); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 204, 204, 172, 172); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 090, 090, 203, 203); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 188, 188, 027, 027); // Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[306], 1, 0, SpawnTrigger.Automatic, 228, 228, 060, 060); // Death Rider

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 168, 168, 082, 082); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 166, 166, 073, 073); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 234, 234, 036, 036); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 086, 086, 171, 171); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 126, 126, 020, 020); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 126, 126, 026, 026); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 148, 148, 010, 010); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 148, 148, 019, 019); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 149, 149, 035, 035); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 157, 157, 016, 016); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 158, 158, 046, 046); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 166, 166, 057, 057); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 172, 172, 011, 011); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 176, 176, 060, 060); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 183, 183, 096, 096); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 187, 187, 017, 017); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 189, 189, 055, 055); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 190, 190, 040, 040); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 190, 190, 079, 079); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 194, 194, 033, 033); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 196, 196, 045, 045); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 199, 199, 108, 108); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 210, 210, 122, 122); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 212, 212, 062, 062); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 213, 213, 082, 082); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 213, 213, 137, 137); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 214, 214, 010, 010); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 225, 225, 149, 149); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 226, 226, 067, 067); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 226, 226, 140, 140); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 233, 233, 125, 125); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 234, 234, 111, 111); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 234, 234, 155, 155); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 236, 236, 135, 135); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 232, 232, 038, 038); // Forest Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[307], 1, 0, SpawnTrigger.Automatic, 171, 171, 082, 082); // Forest Orc

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 213, 213, 038, 038); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 216, 216, 038, 038); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 103, 103, 016, 016); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 113, 113, 008, 008); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 118, 118, 016, 016); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 136, 136, 023, 023); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 137, 137, 013, 013); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 150, 150, 046, 046); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 157, 157, 023, 023); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 158, 158, 010, 010); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 162, 162, 070, 070); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 164, 164, 018, 018); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 166, 166, 048, 048); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 174, 174, 023, 023); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 177, 177, 016, 016); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 189, 189, 038, 038); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 189, 189, 095, 095); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 195, 195, 026, 026); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 196, 196, 099, 099); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 200, 200, 071, 071); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 204, 204, 012, 012); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 205, 205, 031, 031); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 206, 206, 091, 091); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 214, 214, 114, 114); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 219, 219, 091, 091); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 222, 222, 075, 075); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 223, 223, 130, 130); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 228, 228, 111, 111); // Death Tree
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[308], 1, 0, SpawnTrigger.Automatic, 226, 226, 011, 011); // Death Tree

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[309], 1, 0, SpawnTrigger.Automatic, 108, 108, 147, 147); // Hell Maine
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[309], 1, 0, SpawnTrigger.Automatic, 112, 112, 047, 047); // Hell Maine
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[309], 1, 0, SpawnTrigger.Automatic, 123, 123, 103, 103); // Hell Maine

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 211, 211, 204, 204); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 199, 199, 200, 200); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 208, 208, 210, 210); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 192, 192, 207, 207); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 221, 221, 236, 236); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 215, 215, 238, 238); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 228, 228, 219, 219); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 218, 218, 224, 224); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 232, 232, 217, 217); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 240, 240, 230, 230); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 237, 237, 236, 236); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 186, 186, 207, 207); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 171, 171, 212, 212); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 158, 158, 216, 216); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 162, 162, 194, 194); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 148, 148, 198, 198); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 158, 158, 235, 235); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 163, 163, 237, 237); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 067, 067, 227, 227); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 073, 073, 222, 222); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 082, 082, 228, 228); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 040, 040, 224, 224); // Bloody Orc
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[549], 1, 0, SpawnTrigger.Automatic, 038, 038, 206, 206); // Bloody Orc

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 203, 203, 213, 213); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 154, 154, 193, 193); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 152, 152, 211, 211); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 170, 170, 203, 203); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 165, 165, 215, 215); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 155, 155, 231, 231); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 159, 159, 228, 228); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 138, 138, 200, 200); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 126, 126, 184, 184); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 131, 131, 227, 227); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 127, 127, 224, 224); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 137, 137, 218, 218); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 122, 122, 200, 200); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 082, 082, 216, 216); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 033, 033, 222, 222); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 054, 054, 155, 155); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 042, 042, 179, 179); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 026, 026, 134, 134); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 026, 026, 118, 118); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 047, 047, 125, 125); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 046, 046, 212, 212); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 020, 020, 181, 181); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 206, 206, 197, 197); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 026, 026, 054, 054); // Bloody Death Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[550], 1, 0, SpawnTrigger.Automatic, 053, 053, 053, 053); // Bloody Death Rider

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 128, 128, 190, 190); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 143, 143, 198, 198); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 135, 135, 196, 196); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 127, 127, 230, 230); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 130, 130, 218, 218); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 123, 123, 195, 195); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 138, 138, 188, 188); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 136, 136, 184, 184); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 087, 087, 209, 209); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 077, 077, 212, 212); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 066, 066, 209, 209); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 022, 022, 190, 190); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 028, 028, 191, 191); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 040, 040, 184, 184); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 044, 044, 176, 176); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 039, 039, 178, 178); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 055, 055, 120, 120); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 022, 022, 127, 127); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 035, 035, 116, 116); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 025, 025, 061, 061); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 025, 025, 049, 049); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 051, 051, 048, 048); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 051, 051, 060, 060); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 199, 199, 207, 207); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 218, 218, 232, 232); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 236, 236, 232, 232); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 160, 160, 232, 232); // Bloody Golem
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[551], 1, 0, SpawnTrigger.Automatic, 039, 039, 222, 222); // Bloody Golem

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 045, 045, 227, 227); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 044, 044, 219, 219); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 032, 032, 205, 205); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 026, 026, 182, 182); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 050, 050, 158, 158); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 056, 056, 160, 160); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 024, 024, 186, 186); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 043, 043, 134, 134); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 054, 054, 117, 117); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 039, 039, 119, 119); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 020, 020, 133, 133); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 026, 026, 146, 146); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 039, 039, 148, 148); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 051, 051, 123, 123); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 035, 035, 107, 107); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 036, 036, 093, 093); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 037, 037, 077, 077); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 046, 046, 054, 054); // Bloody Witch Queen
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[552], 1, 0, SpawnTrigger.Automatic, 031, 031, 053, 053); // Bloody Witch Queen
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 304;
                monster.Designation = "Witch Queen";
                monster.MoveRange = 4;
                monster.AttackRange = 3;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 94 },
                    { Stats.MaximumHealth, 38500 },
                    { Stats.MinimumPhysBaseDmg, 408 },
                    { Stats.MaximumPhysBaseDmg, 442 },
                    { Stats.DefenseBase, 357 },
                    { Stats.AttackRatePvm, 637 },
                    { Stats.DefenseRatePvm, 230 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 22 },
                    { Stats.IceResistance, 22 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 22 },
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
                monster.Number = 305;
                monster.Designation = "Blue Golem";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 84 },
                    { Stats.MaximumHealth, 25000 },
                    { Stats.MinimumPhysBaseDmg, 288 },
                    { Stats.MaximumPhysBaseDmg, 322 },
                    { Stats.DefenseBase, 277 },
                    { Stats.AttackRatePvm, 507 },
                    { Stats.DefenseRatePvm, 170 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 21 },
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
                monster.Number = 306;
                monster.Designation = "Death Rider";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 78 },
                    { Stats.MaximumHealth, 14000 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 294 },
                    { Stats.DefenseBase, 222 },
                    { Stats.AttackRatePvm, 402 },
                    { Stats.DefenseRatePvm, 144 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 20 },
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
                monster.Number = 307;
                monster.Designation = "Forest Orc";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 11000 },
                    { Stats.MinimumPhysBaseDmg, 258 },
                    { Stats.MaximumPhysBaseDmg, 282 },
                    { Stats.DefenseBase, 197 },
                    { Stats.AttackRatePvm, 390 },
                    { Stats.DefenseRatePvm, 127 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 19 },
                    { Stats.IceResistance, 19 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 19 },
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
                monster.Number = 308;
                monster.Designation = "Death Tree";
                monster.MoveRange = 4;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 72 },
                    { Stats.MaximumHealth, 10000 },
                    { Stats.MinimumPhysBaseDmg, 248 },
                    { Stats.MaximumPhysBaseDmg, 272 },
                    { Stats.DefenseBase, 187 },
                    { Stats.AttackRatePvm, 362 },
                    { Stats.DefenseRatePvm, 120 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 18 },
                    { Stats.IceResistance, 18 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 18 },
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
                monster.Number = 309;
                monster.Designation = "Hell Maine";
                monster.MoveRange = 6;
                monster.AttackRange = 5;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 550 },
                    { Stats.MaximumPhysBaseDmg, 600 },
                    { Stats.DefenseBase, 520 },
                    { Stats.AttackRatePvm, 850 },
                    { Stats.DefenseRatePvm, 300 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 50 },
                    { Stats.IceResistance, 50 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 50 },
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
                monster.Number = 549;
                monster.Designation = "Bloody Orc";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 117000 },
                    { Stats.MinimumPhysBaseDmg, 675 },
                    { Stats.MaximumPhysBaseDmg, 710 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 820 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
                monster.Number = 550;
                monster.Designation = "Bloody Death Rider";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 115 },
                    { Stats.MaximumHealth, 88000 },
                    { Stats.MinimumPhysBaseDmg, 795 },
                    { Stats.MaximumPhysBaseDmg, 830 },
                    { Stats.DefenseBase, 460 },
                    { Stats.AttackRatePvm, 1100 },
                    { Stats.DefenseRatePvm, 430 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
                monster.Number = 551;
                monster.Designation = "Bloody Golem";
                monster.MoveRange = 4;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 129700 },
                    { Stats.MinimumPhysBaseDmg, 715 },
                    { Stats.MaximumPhysBaseDmg, 750 },
                    { Stats.DefenseBase, 470 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 960 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
                monster.Number = 552;
                monster.Designation = "Bloody Witch Queen";
                monster.MoveRange = 4;
                monster.AttackRange = 3;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 120 },
                    { Stats.MaximumHealth, 85700 },
                    { Stats.MinimumPhysBaseDmg, 835 },
                    { Stats.MaximumPhysBaseDmg, 870 },
                    { Stats.DefenseBase, 480 },
                    { Stats.AttackRatePvm, 1100 },
                    { Stats.DefenseRatePvm, 430 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
