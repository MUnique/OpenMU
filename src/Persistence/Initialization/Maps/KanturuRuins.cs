// <copyright file="KanturuRuins.cs" company="MUnique">
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
    /// The initialization for the Kanturu Ruins map.
    /// </summary>
    internal class KanturuRuins : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the kanturu ruins map.
        /// </summary>
        public const byte Number = 37;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Kanturu_I"; // Kanturu Ruins (1, 2), Kanturu Ruins (3) Island

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 188, 188, 011, 011); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 151, 151, 011, 011); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 154, 154, 041, 041); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 150, 150, 056, 056); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 158, 158, 057, 057); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 165, 165, 050, 050); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 092, 092, 034, 034); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 085, 085, 061, 061); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 137, 137, 047, 047); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 061, 061, 031, 031); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 085, 085, 021, 021); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 071, 071, 020, 020); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 046, 046, 034, 034); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 064, 064, 097, 097); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 066, 066, 099, 099); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 048, 048, 103, 103); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 031, 031, 087, 087); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 050, 050, 049, 049); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 055, 055, 104, 104); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 038, 038, 051, 051); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 056, 056, 044, 044); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 065, 065, 035, 035); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 101, 101, 068, 068); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 122, 122, 022, 022); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 129, 129, 019, 019); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 137, 137, 026, 026); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 117, 117, 040, 040); // Berserker
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[350], 1, 0, SpawnTrigger.Automatic, 145, 145, 047, 047); // Berserker

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 188, 188, 218, 218); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 159, 159, 232, 232); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 036, 036, 234, 234); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 042, 042, 235, 235); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 118, 118, 232, 232); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 055, 055, 228, 228); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 075, 075, 231, 231); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 069, 069, 238, 238); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 174, 174, 196, 196); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 094, 094, 234, 234); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 100, 100, 240, 240); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 114, 114, 239, 239); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 135, 135, 223, 223); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 143, 143, 239, 239); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 151, 151, 206, 206); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 155, 155, 196, 196); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 180, 180, 225, 225); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 172, 172, 215, 215); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 042, 042, 227, 227); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 170, 170, 224, 224); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 157, 157, 204, 204); // Splinter Wolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[351], 1, 0, SpawnTrigger.Automatic, 189, 189, 211, 211); // Splinter Wolf

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 207, 207, 187, 187); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 187, 187, 205, 205); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 172, 172, 205, 205); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 207, 207, 195, 195); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 198, 198, 179, 179); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 180, 180, 164, 164); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 174, 174, 170, 170); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 185, 185, 148, 148); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 165, 165, 157, 157); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 165, 165, 165, 165); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 175, 175, 150, 150); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 214, 214, 143, 143); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 217, 217, 145, 145); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 204, 204, 137, 137); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 195, 195, 156, 156); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 195, 195, 148, 148); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 236, 236, 135, 135); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 223, 223, 132, 132); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 212, 212, 125, 125); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 222, 222, 113, 113); // Iron Rider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[352], 1, 0, SpawnTrigger.Automatic, 225, 225, 141, 141); // Iron Rider

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 217, 217, 141, 141); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 226, 226, 154, 154); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 218, 218, 164, 164); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 213, 213, 165, 165); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 195, 195, 152, 152); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 193, 193, 181, 181); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 166, 166, 162, 162); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 182, 182, 150, 150); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 233, 233, 123, 123); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 234, 234, 132, 132); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 236, 236, 091, 091); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 231, 231, 096, 096); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 234, 234, 101, 101); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 220, 220, 155, 155); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 178, 178, 168, 168); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 225, 225, 086, 086); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 224, 224, 082, 082); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 230, 230, 082, 082); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 229, 229, 058, 058); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 226, 226, 068, 068); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 206, 206, 038, 038); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 213, 213, 044, 044); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 214, 214, 035, 035); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 215, 215, 037, 037); // Satyros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[353], 1, 0, SpawnTrigger.Automatic, 224, 224, 043, 043); // Satyros

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 204, 204, 035, 035); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 221, 221, 047, 047); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 207, 207, 040, 040); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 189, 189, 014, 014); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 187, 187, 007, 007); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 175, 175, 010, 010); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 148, 148, 019, 019); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 155, 155, 018, 018); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 164, 164, 022, 022); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 165, 165, 040, 040); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 167, 167, 043, 043); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 152, 152, 059, 059); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 156, 156, 059, 059); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 148, 148, 051, 051); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 134, 134, 039, 039); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 138, 138, 039, 039); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 142, 142, 041, 041); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 120, 120, 028, 028); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 126, 126, 018, 018); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 124, 124, 025, 025); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 116, 116, 043, 043); // Blade Hunter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[354], 1, 0, SpawnTrigger.Automatic, 122, 122, 044, 044); // Blade Hunter

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 176, 176, 041, 041); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 181, 181, 015, 015); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 153, 153, 015, 015); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 163, 163, 033, 033); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 181, 181, 027, 027); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 137, 137, 042, 042); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 136, 136, 051, 051); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 117, 117, 026, 026); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 119, 119, 055, 055); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 115, 115, 065, 065); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 076, 076, 053, 053); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 072, 072, 062, 062); // Kentauros
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[355], 1, 0, SpawnTrigger.Automatic, 087, 087, 071, 071); // Kentauros

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 087, 087, 036, 036); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 077, 077, 028, 028); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 066, 066, 021, 021); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 059, 059, 040, 040); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 090, 090, 039, 039); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 041, 041, 037, 037); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 051, 051, 032, 032); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 077, 077, 019, 019); // Gigantis
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[356], 1, 0, SpawnTrigger.Automatic, 068, 068, 028, 028); // Gigantis

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 037, 037, 059, 059); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 045, 045, 070, 070); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 035, 035, 069, 069); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 063, 063, 101, 101); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 035, 035, 055, 055); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 032, 032, 076, 076); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 058, 058, 104, 104); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 058, 058, 092, 092); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 046, 046, 102, 102); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 047, 047, 054, 054); // Genocider
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[357], 1, 0, SpawnTrigger.Automatic, 044, 044, 044, 044); // Genocider

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 068, 068, 164, 164); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 077, 077, 157, 157); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 079, 079, 166, 166); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 060, 060, 156, 156); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 058, 058, 135, 135); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 104, 104, 157, 157); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 124, 124, 165, 165); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 093, 093, 118, 118); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 131, 131, 119, 119); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 110, 110, 140, 140); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 098, 098, 137, 137); // Berserker Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[553], 1, 0, SpawnTrigger.Automatic, 069, 069, 158, 158); // Berserker Warrior

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 074, 074, 162, 162); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 096, 096, 156, 156); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 102, 102, 150, 150); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 136, 136, 161, 161); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 133, 133, 127, 127); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 089, 089, 124, 124); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 061, 061, 131, 131); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 052, 052, 153, 153); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 094, 094, 128, 128); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 115, 115, 162, 162); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 140, 140, 152, 152); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 086, 086, 119, 119); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 091, 091, 164, 164); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 049, 049, 140, 140); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 132, 132, 111, 111); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 171, 171, 111, 111); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 145, 145, 087, 087); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[554], 1, 0, SpawnTrigger.Automatic, 141, 141, 133, 133); // Kentauros Warrior

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 120, 120, 160, 160); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 141, 141, 159, 159); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 128, 128, 130, 130); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 081, 081, 125, 125); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 107, 107, 103, 103); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 115, 115, 104, 104); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 126, 126, 109, 109); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 134, 134, 107, 107); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 142, 142, 127, 127); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 145, 145, 091, 091); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 167, 167, 090, 090); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 175, 175, 113, 113); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 129, 129, 114, 114); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 112, 112, 098, 098); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 145, 145, 153, 153); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[555], 1, 0, SpawnTrigger.Automatic, 110, 110, 106, 106); // Gigantis Warrior

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 145, 145, 133, 133); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 148, 148, 129, 129); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 167, 167, 114, 114); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 171, 171, 106, 106); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 191, 191, 096, 096); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 187, 187, 087, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 131, 131, 081, 081); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 119, 119, 087, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 181, 181, 109, 109); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 177, 177, 089, 089); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 141, 141, 089, 089); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 151, 151, 087, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[556], 1, 0, SpawnTrigger.Automatic, 115, 115, 098, 098); // Genocider Warrior
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 350;
                monster.Designation = "Berserker";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1550 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 44370 },
                    { Stats.MinimumPhysBaseDmg, 555 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 443 },
                    { Stats.AttackRatePvm, 728 },
                    { Stats.DefenseRatePvm, 255 },
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
                monster.Number = 351;
                monster.Designation = "Splinter Wolf";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 16000 },
                    { Stats.MinimumPhysBaseDmg, 310 },
                    { Stats.MaximumPhysBaseDmg, 340 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 460 },
                    { Stats.DefenseRatePvm, 163 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 20 },
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
                monster.Number = 352;
                monster.Designation = "Iron Rider";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 18000 },
                    { Stats.MinimumPhysBaseDmg, 335 },
                    { Stats.MaximumPhysBaseDmg, 365 },
                    { Stats.DefenseBase, 250 },
                    { Stats.AttackRatePvm, 490 },
                    { Stats.DefenseRatePvm, 168 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.LightningResistance, 21 },
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
                monster.Number = 353;
                monster.Designation = "Satyros";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 85 },
                    { Stats.MaximumHealth, 22000 },
                    { Stats.MinimumPhysBaseDmg, 365 },
                    { Stats.MaximumPhysBaseDmg, 395 },
                    { Stats.DefenseBase, 280 },
                    { Stats.AttackRatePvm, 540 },
                    { Stats.DefenseRatePvm, 177 },
                    { Stats.PoisonResistance, 22 },
                    { Stats.IceResistance, 22 },
                    { Stats.LightningResistance, 22 },
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
                monster.Number = 354;
                monster.Designation = "Blade Hunter";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 88 },
                    { Stats.MaximumHealth, 32000 },
                    { Stats.MinimumPhysBaseDmg, 408 },
                    { Stats.MaximumPhysBaseDmg, 443 },
                    { Stats.DefenseBase, 315 },
                    { Stats.AttackRatePvm, 587 },
                    { Stats.DefenseRatePvm, 195 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.LightningResistance, 23 },
                    { Stats.FireResistance, 23 },
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
                monster.Number = 355;
                monster.Designation = "Kentauros";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 93 },
                    { Stats.MaximumHealth, 38500 },
                    { Stats.MinimumPhysBaseDmg, 470 },
                    { Stats.MaximumPhysBaseDmg, 505 },
                    { Stats.DefenseBase, 370 },
                    { Stats.AttackRatePvm, 645 },
                    { Stats.DefenseRatePvm, 220 },
                    { Stats.PoisonResistance, 24 },
                    { Stats.IceResistance, 24 },
                    { Stats.LightningResistance, 24 },
                    { Stats.FireResistance, 24 },
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
                monster.Number = 356;
                monster.Designation = "Gigantis";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 43000 },
                    { Stats.MinimumPhysBaseDmg, 546 },
                    { Stats.MaximumPhysBaseDmg, 581 },
                    { Stats.DefenseBase, 430 },
                    { Stats.AttackRatePvm, 715 },
                    { Stats.DefenseRatePvm, 250 },
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
                monster.Number = 357;
                monster.Designation = "Genocider";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 48500 },
                    { Stats.MinimumPhysBaseDmg, 640 },
                    { Stats.MaximumPhysBaseDmg, 675 },
                    { Stats.DefenseBase, 515 },
                    { Stats.AttackRatePvm, 810 },
                    { Stats.DefenseRatePvm, 290 },
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
                monster.Number = 553;
                monster.Designation = "Berserker Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 123 },
                    { Stats.MaximumHealth, 184370 },
                    { Stats.MinimumPhysBaseDmg, 915 },
                    { Stats.MaximumPhysBaseDmg, 990 },
                    { Stats.DefenseBase, 543 },
                    { Stats.AttackRatePvm, 1557 },
                    { Stats.DefenseRatePvm, 755 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 150 },
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
                monster.Number = 554;
                monster.Designation = "Kentauros Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 126 },
                    { Stats.MaximumHealth, 198500 },
                    { Stats.MinimumPhysBaseDmg, 970 },
                    { Stats.MaximumPhysBaseDmg, 1005 },
                    { Stats.DefenseBase, 570 },
                    { Stats.AttackRatePvm, 1258 },
                    { Stats.DefenseRatePvm, 920 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 150 },
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
                monster.Number = 555;
                monster.Designation = "Gigantis Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 128 },
                    { Stats.MaximumHealth, 203000 },
                    { Stats.MinimumPhysBaseDmg, 1046 },
                    { Stats.MaximumPhysBaseDmg, 1181 },
                    { Stats.DefenseBase, 630 },
                    { Stats.AttackRatePvm, 1575 },
                    { Stats.DefenseRatePvm, 750 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 150 },
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
                monster.Number = 556;
                monster.Designation = "Genocider Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 129 },
                    { Stats.MaximumHealth, 218500 },
                    { Stats.MinimumPhysBaseDmg, 1240 },
                    { Stats.MaximumPhysBaseDmg, 1375 },
                    { Stats.DefenseBase, 715 },
                    { Stats.AttackRatePvm, 1251 },
                    { Stats.DefenseRatePvm, 990 },
                    { Stats.PoisonResistance, 20 },
                    { Stats.IceResistance, 20 },
                    { Stats.LightningResistance, 150 },
                    { Stats.FireResistance, 20 },
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
