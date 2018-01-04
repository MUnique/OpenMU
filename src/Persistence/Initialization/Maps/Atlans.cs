// <copyright file="Atlans.cs" company="MUnique">
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
    /// Initialization for the Atlans map.
    /// </summary>
    internal class Atlans : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 7;

        /// <inheritdoc/>
        protected override string MapName => "Atlans";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IRepositoryManager repositoryManager, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Guard:
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[240], 1, 1, SpawnTrigger.Automatic, 23, 23, 17, 17);

            // Monsters:
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 236, 236, 177, 177);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 212, 212, 96, 96);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 21, 21, 41, 41);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 23, 23, 33, 33);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 31, 31, 31, 31);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 39, 39, 23, 23);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 41, 41, 11, 11);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 21, 21, 48, 48);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 47, 47, 41, 41);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 39, 39, 63, 63);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 72, 72, 42, 42);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 23, 23, 56, 56);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 23, 23, 64, 64);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 35, 35, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 29, 29, 75, 75);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 62, 62, 21, 21);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 78, 78, 19, 19);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 67, 67, 36, 36);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 24, 24, 86, 86);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 48, 48, 57, 57);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 45, 45, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 19, 19, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 49, 49, 8, 8);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 46, 46, 20, 20);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 39, 39, 35, 35);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 37, 37, 50, 50);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 20, 20, 75, 75);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 63, 63, 44, 44);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 68, 68, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 49, 49, 64, 64);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[45], 1, 0, SpawnTrigger.Automatic, 29, 29, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 46, 46, 21, 21);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 48, 48, 9, 9);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 50, 50, 65, 65);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 51, 51, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 80, 80, 33, 33);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 62, 62, 17, 17);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 61, 61, 11, 11);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 69, 69, 19, 19);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 75, 75, 15, 15);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 83, 83, 15, 15);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 84, 84, 21, 21);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 67, 67, 27, 27);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 59, 59, 34, 34);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 86, 86, 37, 37);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 78, 78, 44, 44);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 71, 71, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 65, 65, 59, 59);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 65, 65, 51, 51);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 55, 55, 44, 44);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 40, 40, 48, 48);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 31, 31, 53, 53);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 24, 24, 90, 90);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 21, 21, 100, 100);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 25, 25, 105, 105);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 30, 30, 110, 110);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 21, 21, 118, 118);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 38, 38, 117, 117);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 141, 141, 11, 11);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 127, 127, 17, 17);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 114, 114, 26, 26);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 107, 107, 18, 18);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 104, 104, 13, 13);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 115, 115, 12, 12);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 89, 89, 62, 62);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 75, 75, 76, 76);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 69, 69, 87, 87);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 50, 50, 93, 93);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 23, 23, 72, 72);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 78, 78, 23, 23);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 70, 70, 38, 38);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 55, 55, 25, 25);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 30, 30, 122, 122);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 17, 17, 126, 126);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 122, 122, 29, 29);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[46], 1, 0, SpawnTrigger.Automatic, 108, 108, 40, 40);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 66, 66, 222, 222);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 63, 63, 218, 218);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 57, 57, 225, 225);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 171, 171, 162, 162);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 175, 175, 169, 169);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 178, 178, 174, 174);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 173, 173, 179, 179);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 171, 171, 187, 187);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 165, 165, 193, 193);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 159, 159, 198, 198);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 152, 152, 201, 201);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 144, 144, 201, 201);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 137, 137, 205, 205);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 124, 124, 206, 206);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 115, 115, 210, 210);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 104, 104, 211, 211);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 94, 94, 210, 210);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 87, 87, 214, 214);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 76, 76, 215, 215);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 47, 47, 224, 224);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 46, 46, 217, 217);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 29, 29, 226, 226);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 22, 22, 213, 213);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 24, 24, 204, 204);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 34, 34, 200, 200);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 35, 35, 209, 209);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 40, 40, 210, 210);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 45, 45, 205, 205);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 47, 47, 211, 211);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 37, 37, 232, 232);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 226, 226, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 22, 22, 230, 230);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 41, 41, 224, 224);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 82, 82, 215, 215);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 167, 167, 156, 156);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 177, 177, 146, 146);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 156, 156, 156, 156);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 162, 162, 149, 149);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 152, 152, 169, 169);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 72, 72, 128, 128);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 70, 70, 138, 138);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 61, 61, 147, 147);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 49, 49, 158, 158);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 33, 33, 169, 169);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 22, 22, 173, 173);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 55, 55, 181, 181);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 67, 67, 186, 186);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 73, 73, 180, 180);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 89, 89, 184, 184);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 101, 101, 183, 183);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 111, 111, 183, 183);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 122, 122, 178, 178);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 129, 129, 171, 171);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 138, 138, 167, 167);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 147, 147, 164, 164);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 170, 170, 145, 145);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 163, 163, 138, 138);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 53, 53, 220, 220);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 41, 41, 222, 222);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 29, 29, 231, 231);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 22, 22, 232, 232);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 27, 27, 209, 209);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 36, 36, 205, 205);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 109, 109, 211, 211);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 131, 131, 207, 207);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 155, 155, 196, 196);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 167, 167, 154, 154);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 177, 177, 19, 19);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 118, 118, 167, 167);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 111, 111, 175, 175);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 63, 63, 178, 178);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 162, 162, 141, 141);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 138, 138, 176, 176);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 41, 41, 109, 109);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 49, 49, 100, 100);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 54, 54, 89, 89);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 62, 62, 87, 87);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 71, 71, 82, 82);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 79, 79, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 85, 85, 64, 64);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 94, 94, 60, 60);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 101, 101, 50, 50);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 106, 106, 48, 48);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 112, 112, 45, 45);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 109, 109, 38, 38);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 118, 118, 37, 37);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 122, 122, 29, 29);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 119, 119, 20, 20);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 124, 124, 14, 14);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 133, 133, 14, 14);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 150, 150, 14, 14);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 161, 161, 11, 11);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 166, 166, 16, 16);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 171, 171, 18, 18);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 182, 182, 14, 14);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 190, 190, 24, 24);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 196, 196, 27, 27);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 200, 200, 17, 17);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 208, 208, 9, 9);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 218, 218, 47, 47);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 222, 222, 14, 14);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 117, 117, 74, 74);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 139, 139, 61, 61);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 152, 152, 61, 61);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 172, 172, 55, 55);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 189, 189, 56, 56);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 226, 226, 46, 46);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 240, 240, 11, 11);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 227, 227, 16, 16);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 152, 152, 22, 22);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 142, 142, 15, 15);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 128, 128, 22, 22);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 116, 116, 9, 9);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 113, 113, 27, 27);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 33, 33, 108, 108);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[47], 1, 0, SpawnTrigger.Automatic, 35, 35, 118, 118);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 228, 228, 16, 16);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 237, 237, 17, 17);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 237, 237, 26, 26);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 238, 238, 40, 40);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 235, 235, 48, 48);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 251, 251, 51, 51);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 206, 206, 55, 55);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 200, 200, 49, 49);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 193, 193, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 184, 184, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 178, 178, 58, 58);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 170, 170, 61, 61);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 156, 156, 59, 59);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 144, 144, 63, 63);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 133, 133, 61, 61);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 133, 133, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 125, 125, 78, 78);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 118, 118, 85, 85);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 124, 124, 90, 90);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 116, 116, 98, 98);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 106, 106, 101, 101);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 99, 99, 108, 108);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 91, 91, 115, 115);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 81, 81, 123, 123);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 126, 126, 167, 167);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 111, 111, 174, 174);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 90, 90, 177, 177);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 76, 76, 180, 180);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 62, 62, 178, 178);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 42, 42, 178, 178);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 27, 27, 161, 161);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 42, 42, 152, 152);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 19, 19, 159, 159);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 62, 62, 140, 140);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 70, 70, 133, 133);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 168, 168, 54, 54);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 7, 7, 52, 52);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 225, 225, 45, 45);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 207, 207, 9, 9);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 154, 154, 69, 69);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 111, 111, 82, 82);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 81, 81, 116, 116);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 72, 72, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 122, 122, 66, 66);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[49], 1, 0, SpawnTrigger.Automatic, 19, 19, 230, 230);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[49], 1, 0, SpawnTrigger.Automatic, 24, 24, 227, 227);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 219, 219, 56, 56);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 215, 215, 65, 65);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 232, 232, 55, 55);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 231, 231, 64, 64);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 238, 238, 71, 71);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 229, 229, 74, 74);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 217, 217, 75, 75);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 225, 225, 81, 81);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 215, 215, 86, 86);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 223, 223, 89, 89);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 218, 218, 101, 101);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 232, 232, 95, 95);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 223, 223, 70, 70);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 211, 211, 59, 59);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 232, 232, 86, 86);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 208, 208, 104, 104);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 235, 235, 106, 106);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 224, 224, 108, 108);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 219, 219, 124, 124);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 225, 225, 117, 117);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 227, 227, 123, 123);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 219, 219, 133, 133);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 226, 226, 134, 134);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 224, 224, 142, 142);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 226, 226, 154, 154);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 221, 221, 150, 150);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 220, 220, 168, 168);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 232, 232, 166, 166);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 231, 231, 176, 176);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 217, 217, 197, 197);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 215, 215, 182, 182);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[49], 1, 0, SpawnTrigger.Automatic, 211, 211, 192, 192);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[49], 1, 0, SpawnTrigger.Automatic, 229, 229, 203, 203);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 208, 208, 183, 183);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 215, 215, 174, 174);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 222, 222, 190, 190);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 229, 229, 190, 190);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 230, 230, 181, 181);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 226, 226, 161, 161);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 208, 208, 198, 198);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 232, 232, 206, 206);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 231, 231, 196, 196);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 223, 223, 202, 202);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 224, 224, 184, 184);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 230, 230, 171, 171);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 221, 221, 158, 158);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 233, 233, 124, 124);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 232, 232, 102, 102);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 200, 200, 103, 103);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 192, 192, 101, 101);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 179, 179, 92, 92);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 187, 187, 94, 94);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 177, 177, 101, 101);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 166, 166, 117, 117);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 159, 159, 112, 112);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 156, 156, 122, 122);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 173, 173, 109, 109);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 167, 167, 107, 107);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 150, 150, 120, 120);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 142, 142, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 136, 136, 129, 129);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 129, 129, 130, 130);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 117, 117, 130, 130);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 121, 121, 135, 135);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 144, 144, 109, 109);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 122, 122, 144, 144);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 112, 112, 137, 137);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 101, 101, 147, 147);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 97, 97, 142, 142);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 144, 144, 132, 132);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 93, 93, 136, 136);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 90, 90, 148, 148);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 124, 124, 125, 125);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 87, 87, 132, 132);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 86, 86, 140, 140);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 78, 78, 145, 145);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 79, 79, 136, 136);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 67, 67, 147, 147);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 71, 71, 140, 140);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 105, 105, 137, 137);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 83, 83, 155, 155);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 81, 81, 164, 164);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 70, 70, 168, 168);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 76, 76, 174, 174);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 85, 85, 185, 185);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[52], 1, 0, SpawnTrigger.Automatic, 87, 87, 175, 175);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 65, 65, 187, 187);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 64, 64, 159, 159);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 186, 186, 99, 99);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 185, 185, 90, 90);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 147, 147, 114, 114);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[51], 1, 0, SpawnTrigger.Automatic, 84, 84, 170, 170);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 15, 15, 234, 234);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 20, 20, 237, 237);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 34, 34, 237, 237);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 47, 47, 231, 231);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 54, 54, 232, 232);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 20, 20, 218, 218);
            yield return this.CreateMonsterSpawn(repositoryManager, mapDefinition, npcDictionary[48], 1, 0, SpawnTrigger.Automatic, 34, 34, 176, 176);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
        {
            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 45;
                monster.Designation = "Bahamut";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 43 },
                    { Stats.MaximumHealth, 2400 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 140 },
                    { Stats.DefenseBase, 65 },
                    { Stats.AttackRatePvm, 215 },
                    { Stats.DefenseRatePvm, 52 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 1 },
                    { Stats.IceResistance, 1 },
                    { Stats.WaterResistance, 1 },
                    { Stats.FireResistance, 1 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 46;
                monster.Designation = "Vepar";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 45 },
                    { Stats.MaximumHealth, 2800 },
                    { Stats.MinimumPhysBaseDmg, 135 },
                    { Stats.MaximumPhysBaseDmg, 145 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 225 },
                    { Stats.DefenseRatePvm, 58 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.WaterResistance, 3 },
                    { Stats.FireResistance, 2 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 47;
                monster.Designation = "Valkyrie";
                monster.MoveRange = 3;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 46 },
                    { Stats.MaximumHealth, 3200 },
                    { Stats.MinimumPhysBaseDmg, 140 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.DefenseBase, 75 },
                    { Stats.AttackRatePvm, 230 },
                    { Stats.DefenseRatePvm, 64 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 2 },
                    { Stats.WaterResistance, 2 },
                    { Stats.FireResistance, 2 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 48;
                monster.Designation = "Lizard King";
                monster.MoveRange = 3;
                monster.AttackRange = 3;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(15 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 9000 },
                    { Stats.MinimumPhysBaseDmg, 240 },
                    { Stats.MaximumPhysBaseDmg, 270 },
                    { Stats.DefenseBase, 180 },
                    { Stats.AttackRatePvm, 350 },
                    { Stats.DefenseRatePvm, 115 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 49;
                monster.Designation = "Hydra";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 310 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 12 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 50;
                monster.Designation = "Sea Worm";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 310 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 12 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 51;
                monster.Designation = "Great Bahamut";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(15 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 7000 },
                    { Stats.MinimumPhysBaseDmg, 210 },
                    { Stats.MaximumPhysBaseDmg, 230 },
                    { Stats.DefenseBase, 150 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 98 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 6 },
                    { Stats.FireResistance, 6 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = repositoryManager.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 52;
                monster.Designation = "Silver Valkyrie";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(15 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 68 },
                    { Stats.MaximumHealth, 8000 },
                    { Stats.MinimumPhysBaseDmg, 230 },
                    { Stats.MaximumPhysBaseDmg, 260 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 340 },
                    { Stats.DefenseRatePvm, 110 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
                }.Select(kvp =>
                {
                    var attribute = repositoryManager.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }
        }
    }
}
