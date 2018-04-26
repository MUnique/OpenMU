// <copyright file="Tarkan.cs" company="MUnique">
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
    /// The initialization for the Tarkan map.
    /// </summary>
    internal class Tarkan : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 8;

        /// <inheritdoc/>
        protected override string MapName => "Tarkan";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 7, 7, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 5, 5, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 8, 8, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 6, 6, 228, 228);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[59], 1, 0, SpawnTrigger.Automatic, 11, 11, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 18, 18, 238, 238);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 146, 146, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 72, 72, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 83, 83, 176, 176);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 63, 63, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 148, 148, 43, 43);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 155, 155, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 134, 134, 57, 57);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 151, 151, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 149, 149, 28, 28);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 141, 141, 74, 74);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 142, 142, 68, 68);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 11, 11, 234, 234);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 141, 141, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 137, 137, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 132, 132, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 148, 148, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 134, 134, 82, 82);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 135, 135, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 136, 136, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 148, 148, 91, 91);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 157, 157, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 158, 158, 80, 80);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 154, 154, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 162, 162, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 161, 161, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 171, 171, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 180, 180, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 190, 190, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 197, 197, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 205, 205, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 186, 186, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 181, 181, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 173, 173, 129, 129);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 169, 169, 136, 136);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 162, 162, 141, 141);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 159, 159, 148, 148);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 151, 151, 155, 155);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 152, 152, 147, 147);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 152, 152, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 162, 162, 129, 129);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 152, 152, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 152, 152, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 171, 171, 106, 106);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 151, 151, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 142, 142, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 140, 140, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[62], 1, 0, SpawnTrigger.Automatic, 133, 133, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 134, 134, 72, 72);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 128, 128, 74, 74);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 123, 123, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 115, 115, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 121, 121, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 128, 128, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 121, 121, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 108, 108, 95, 95);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 104, 104, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 107, 107, 80, 80);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 109, 109, 64, 64);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 108, 108, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 118, 118, 42, 42);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 126, 126, 42, 42);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 133, 133, 44, 44);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 125, 125, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 113, 113, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 105, 105, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 108, 108, 43, 43);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 103, 103, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 97, 97, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 88, 88, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 95, 95, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 96, 96, 36, 36);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 96, 96, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 102, 102, 61, 61);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 104, 104, 70, 70);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 96, 96, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 97, 97, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 81, 81, 63, 63);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 70, 70, 57, 57);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 65, 65, 64, 64);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 64, 64, 71, 71);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 56, 56, 75, 75);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 46, 46, 71, 71);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 38, 38, 71, 71);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 35, 35, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 27, 27, 75, 75);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 34, 34, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 25, 25, 83, 83);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 21, 21, 92, 92);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 38, 38, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 38, 38, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 48, 48, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 55, 55, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 62, 62, 91, 91);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 70, 70, 92, 92);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 79, 79, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 85, 85, 91, 91);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 91, 91, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 89, 89, 71, 71);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 90, 90, 50, 50);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 69, 69, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 61, 61, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 54, 54, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 43, 43, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 35, 35, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 24, 24, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 18, 18, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 25, 25, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 32, 32, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 29, 29, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 42, 42, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 35, 35, 141, 141);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 26, 26, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 26, 26, 160, 160);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 33, 33, 157, 157);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 40, 40, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 52, 52, 138, 138);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 61, 61, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[60], 1, 0, SpawnTrigger.Automatic, 74, 74, 81, 81);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 75, 75, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 81, 81, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 87, 87, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 77, 77, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 96, 96, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 107, 107, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 112, 112, 124, 124);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 119, 119, 139, 139);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 107, 107, 139, 139);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 105, 105, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 107, 107, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 86, 86, 131, 131);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 73, 73, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 76, 76, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 69, 69, 139, 139);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 60, 60, 143, 143);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 52, 52, 157, 157);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 52, 52, 165, 165);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 45, 45, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 33, 33, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 35, 35, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 42, 42, 164, 164);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 64, 64, 164, 164);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 131, 131, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 79, 79, 156, 156);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 93, 93, 137, 137);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 115, 115, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 124, 124, 152, 152);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 129, 129, 141, 141);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 125, 125, 129, 129);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 124, 124, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 115, 115, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 120, 120, 177, 177);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 127, 127, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 127, 127, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 121, 121, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 123, 123, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 121, 121, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 106, 106, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 109, 109, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 102, 102, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 98, 98, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 88, 88, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 80, 80, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 78, 78, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 88, 88, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 83, 83, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 98, 98, 193, 193);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 73, 73, 179, 179);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 71, 71, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 161, 161, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 114, 114, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 153, 153, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 166, 166, 190, 190);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 159, 159, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 172, 172, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 179, 179, 194, 194);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 183, 183, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 177, 177, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 176, 176, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 173, 173, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 160, 160, 203, 203);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[63], 1, 0, SpawnTrigger.Automatic, 161, 161, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 161, 161, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 157, 157, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 153, 153, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 150, 150, 203, 203);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 167, 167, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 93, 93, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 169, 169, 204, 204);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 44, 44, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 39, 39, 210, 210);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 31, 31, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 24, 24, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 27, 27, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 31, 31, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 29, 29, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 36, 36, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 42, 42, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 50, 50, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 50, 50, 230, 230);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 43, 43, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 29, 29, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 15, 15, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 19, 19, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 11, 11, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 6, 6, 240, 240);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 157, 157, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[61], 1, 0, SpawnTrigger.Automatic, 85, 85, 141, 141);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 22, 22, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 52, 52, 204, 204);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[58], 1, 0, SpawnTrigger.Automatic, 165, 165, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[57], 1, 0, SpawnTrigger.Automatic, 114, 114, 78, 78);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 57;
                monster.Designation = "Iron Wheel";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 17000 },
                    { Stats.MinimumPhysBaseDmg, 280 },
                    { Stats.MaximumPhysBaseDmg, 330 },
                    { Stats.DefenseBase, 215 },
                    { Stats.AttackRatePvm, 446 },
                    { Stats.DefenseRatePvm, 150 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 9 },
                    { Stats.FireResistance, 9 },
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
                monster.Number = 58;
                monster.Designation = "Tantallos";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(20 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 83 },
                    { Stats.MaximumHealth, 22000 },
                    { Stats.MinimumPhysBaseDmg, 335 },
                    { Stats.MaximumPhysBaseDmg, 385 },
                    { Stats.DefenseBase, 250 },
                    { Stats.AttackRatePvm, 500 },
                    { Stats.DefenseRatePvm, 175 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 9 },
                    { Stats.IceResistance, 9 },
                    { Stats.WaterResistance, 9 },
                    { Stats.FireResistance, 9 },
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
                monster.Number = 59;
                monster.Designation = "Zaikan";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 34000 },
                    { Stats.MinimumPhysBaseDmg, 510 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 400 },
                    { Stats.AttackRatePvm, 550 },
                    { Stats.DefenseRatePvm, 185 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.WaterResistance, 13 },
                    { Stats.FireResistance, 15 },
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
                monster.Number = 60;
                monster.Designation = "Bloody Wolf";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 76 },
                    { Stats.MaximumHealth, 13500 },
                    { Stats.MinimumPhysBaseDmg, 260 },
                    { Stats.MaximumPhysBaseDmg, 300 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 410 },
                    { Stats.DefenseRatePvm, 130 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 8 },
                    { Stats.FireResistance, 8 },
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
                monster.Number = 61;
                monster.Designation = "Beam Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(20 * TimeSpan.TicksPerSecond);
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
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 10 },
                    { Stats.IceResistance, 10 },
                    { Stats.WaterResistance, 10 },
                    { Stats.FireResistance, 10 },
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
                monster.Number = 62;
                monster.Designation = "Mutant";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 72 },
                    { Stats.MaximumHealth, 10000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 280 },
                    { Stats.DefenseBase, 190 },
                    { Stats.AttackRatePvm, 365 },
                    { Stats.DefenseRatePvm, 120 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 8 },
                    { Stats.IceResistance, 8 },
                    { Stats.WaterResistance, 8 },
                    { Stats.FireResistance, 8 },
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
                monster.Number = 63;
                monster.Designation = "Death Beam Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 93 },
                    { Stats.MaximumHealth, 40000 },
                    { Stats.MinimumPhysBaseDmg, 590 },
                    { Stats.MaximumPhysBaseDmg, 650 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 575 },
                    { Stats.DefenseRatePvm, 220 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 13 },
                    { Stats.IceResistance, 13 },
                    { Stats.WaterResistance, 13 },
                    { Stats.FireResistance, 17 },
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
