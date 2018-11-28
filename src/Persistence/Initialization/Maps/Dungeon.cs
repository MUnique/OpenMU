// <copyright file="Dungeon.cs" company="MUnique">
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
    /// The initialization for the Dungeon map.
    /// </summary>
    internal class Dungeon : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 1;

        /// <inheritdoc/>
        protected override string MapName => "Dungeon";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 122, 122);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 4, 4, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 2, 2, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 230, 230);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 65, 65, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 169, 169);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 94, 94, 242, 242);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 94, 94, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 87, 87, 177, 177);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 62, 62, 181, 181);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 57, 57, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 49, 49, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 31, 31, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 210, 210);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 146, 146);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 49, 49, 182, 182);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 44, 44, 162, 162);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 180, 180);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 92, 92, 157, 157);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 113, 113, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 223, 223);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 193, 193);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 142, 142, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 240, 240);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 242, 242, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 226, 226);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 246, 246, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 248, 248);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 148, 148, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 145, 145, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 93, 93);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 99, 99, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 27, 27);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 159, 159, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 76, 76, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 90, 90, 6, 6);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 83, 83);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 58, 58);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 28, 28, 18, 18);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 4, 4);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 34, 34, 4, 4);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 80, 80);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 117, 117);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 21, 21, 82, 82);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 16, 16, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 35, 35, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 30, 30);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 75, 75);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 149, 149, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 81, 81, 58, 58);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 126, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 71, 71, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 88, 88);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 88, 88, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 162, 162);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 248, 248);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 79, 79, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 78, 78, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 95, 95, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 90, 90, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 238, 238);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 73, 73, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 73, 73, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 17, 17, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 13, 13, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 27, 27, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 62, 62, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 77, 77, 152, 152);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 146, 146);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 159, 159);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 148, 148);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 152, 152);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 151, 151);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 176, 176);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 234, 234);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 16, 16, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 128, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 87, 87, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 65, 65, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 80, 80, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 248, 248, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 160, 160, 10, 10);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 18, 18);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 221, 221, 14, 14);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 26, 26, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 113, 113);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 44, 44, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 41, 41, 88, 88);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 19, 19, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 37, 37, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 27, 27, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 10, 10, 68, 68);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 43, 43, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 26, 26, 67, 67);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 122, 122);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 4, 4, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 2, 2, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 230, 230);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 65, 65, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 169, 169);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 94, 94, 242, 242);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 94, 94, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 87, 87, 177, 177);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 62, 62, 181, 181);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 57, 57, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 49, 49, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 31, 31, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 210, 210);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 146, 146);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 49, 49, 182, 182);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 44, 44, 162, 162);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 180, 180);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 92, 92, 157, 157);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 113, 113, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 223, 223);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 193, 193);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 142, 142, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 240, 240);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 242, 242, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 226, 226);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 246, 246, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 248, 248);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 148, 148, 149, 149);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 141, 141, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 145, 145, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 93, 93);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 99, 99, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 27, 27);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 159, 159, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 76, 76, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 90, 90, 6, 6);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 114, 114, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 83, 83);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 58, 58);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 28, 28, 18, 18);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 4, 4);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 34, 34, 4, 4);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 78, 78);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 80, 80);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 117, 117);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 21, 21, 82, 82);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 16, 16, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 35, 35, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 30, 30);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 75, 75);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 149, 149, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 81, 81, 58, 58);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 126, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 71, 71, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 69, 69, 88, 88);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 88, 88, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 162, 162);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 161, 161);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 248, 248);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 79, 79, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 78, 78, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 95, 95, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 90, 90, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 61, 61, 238, 238);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 73, 73, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[12], 1, Direction.Undefined, SpawnTrigger.Automatic, 73, 73, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 3, 3, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[14], 1, Direction.Undefined, SpawnTrigger.Automatic, 17, 17, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 13, 13, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 27, 27, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 62, 62, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 154, 154);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 77, 77, 152, 152);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 119, 119, 146, 146);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 159, 159);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 218, 218);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 231, 231, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 187, 187);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 186, 186);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 148, 148);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 145, 145);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 152, 152);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 151, 151);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 176, 176);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 222, 222);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 234, 234);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 16, 16, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[17], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[11], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 128, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 87, 87, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 65, 65, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 68, 68, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[15], 1, Direction.Undefined, SpawnTrigger.Automatic, 80, 80, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[5], 1, Direction.Undefined, SpawnTrigger.Automatic, 70, 70, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 168, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[13], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 247, 247, 77, 77);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 248, 248, 20, 20);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 155, 155, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[16], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 160, 160, 10, 10);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 18, 18);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 221, 221, 14, 14);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 66, 66);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 26, 26, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 113, 113);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[8], 1, Direction.Undefined, SpawnTrigger.Automatic, 44, 44, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 41, 41, 88, 88);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 19, 19, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[10], 1, Direction.Undefined, SpawnTrigger.Automatic, 37, 37, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 27, 27, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 10, 10, 68, 68);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 90, 90);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[9], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 43, 43, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 123, 123);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[18], 1, Direction.Undefined, SpawnTrigger.Automatic, 26, 26, 67, 67);

            // Traps:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 10, 10, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 11, 11, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 24, 24, 5, 5);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 24, 24, 4, 4);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 23, 23, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 21, 21);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 19, 19, 19, 19);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 22, 22, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 23, 23, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 23, 23, 28, 28);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 33, 33, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 35, 35, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 39, 39, 18, 18);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 39, 39, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 39, 39, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[102], 1, Direction.SouthEast, SpawnTrigger.Automatic, 45, 45, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 48, 48, 193, 193);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 49, 49, 193, 193);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[102], 1, Direction.SouthEast, SpawnTrigger.Automatic, 66, 66, 71, 71);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[102], 1, Direction.SouthWest, SpawnTrigger.Automatic, 80, 80, 61, 61);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 90, 90, 164, 164);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 92, 92, 164, 164);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 91, 91, 164, 164);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 126, 126, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 123, 123, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 120, 120, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 117, 117, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 136, 136, 95, 95);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 139, 139, 95, 95);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 128, 128, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 130, 130, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.SouthWest, SpawnTrigger.Automatic, 143, 143, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[101], 1, Direction.NorthEast, SpawnTrigger.Automatic, 155, 155, 230, 230);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 172, 172, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 166, 166, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[102], 1, Direction.SouthWest, SpawnTrigger.Automatic, 169, 169, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[102], 1, Direction.SouthWest, SpawnTrigger.Automatic, 175, 175, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 178, 178, 12, 12);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 177, 177, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 180, 180, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 183, 183, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 186, 186, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 189, 189, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthEast, SpawnTrigger.Automatic, 186, 186, 151, 151);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 196, 196, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 193, 193, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 202, 202, 93, 93);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 205, 205, 93, 93);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 198, 198, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 202, 202, 150, 150);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthEast, SpawnTrigger.Automatic, 232, 232, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthEast, SpawnTrigger.Automatic, 232, 232, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthEast, SpawnTrigger.Automatic, 232, 232, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 227, 227, 61, 61);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 229, 229, 93, 93);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[100], 1, Direction.SouthWest, SpawnTrigger.Automatic, 232, 232, 93, 93);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 5;
                monster.Designation = "Hell Hound";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 38 },
                    { Stats.MaximumHealth, 1400 },
                    { Stats.MinimumPhysBaseDmg, 125 },
                    { Stats.MaximumPhysBaseDmg, 130 },
                    { Stats.DefenseBase, 55 },
                    { Stats.AttackRatePvm, 190 },
                    { Stats.DefenseRatePvm, 45 },
                    { Stats.FireResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 8;
                monster.Designation = "Poison Bull";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 46 },
                    { Stats.MaximumHealth, 2500 },
                    { Stats.MinimumPhysBaseDmg, 145 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.DefenseBase, 75 },
                    { Stats.AttackRatePvm, 230 },
                    { Stats.DefenseRatePvm, 61 },
                    { Stats.PoisonResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 9;
                monster.Designation = "Thunder Lich";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 44 },
                    { Stats.MaximumHealth, 2000 },
                    { Stats.MinimumPhysBaseDmg, 140 },
                    { Stats.MaximumPhysBaseDmg, 145 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 220 },
                    { Stats.DefenseRatePvm, 55 },
                    { Stats.WaterResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 10;
                monster.Designation = "Dark Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 48 },
                    { Stats.MaximumHealth, 3000 },
                    { Stats.MinimumPhysBaseDmg, 150 },
                    { Stats.MaximumPhysBaseDmg, 155 },
                    { Stats.DefenseBase, 80 },
                    { Stats.AttackRatePvm, 240 },
                    { Stats.DefenseRatePvm, 70 },
                    { Stats.PoisonResistance, 3 },
                    { Stats.IceResistance, 3 },
                    { Stats.WaterResistance, 3 },
                    { Stats.FireResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 11;
                monster.Designation = "Ghost";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 32 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 110 },
                    { Stats.MaximumPhysBaseDmg, 115 },
                    { Stats.DefenseBase, 40 },
                    { Stats.AttackRatePvm, 160 },
                    { Stats.DefenseRatePvm, 39 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.WaterResistance, 2 },
                    { Stats.FireResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 12;
                monster.Designation = "Larva";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 25 },
                    { Stats.MaximumHealth, 750 },
                    { Stats.MinimumPhysBaseDmg, 90 },
                    { Stats.MaximumPhysBaseDmg, 95 },
                    { Stats.DefenseBase, 31 },
                    { Stats.AttackRatePvm, 125 },
                    { Stats.DefenseRatePvm, 31 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 13;
                monster.Designation = "Hell Spider";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 40 },
                    { Stats.MaximumHealth, 1600 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 135 },
                    { Stats.DefenseBase, 60 },
                    { Stats.AttackRatePvm, 200 },
                    { Stats.DefenseRatePvm, 47 },
                    { Stats.PoisonResistance, 3 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 15;
                monster.Designation = "Skeleton Archer";
                monster.MoveRange = 2;
                monster.AttackRange = 5;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 34 },
                    { Stats.MaximumHealth, 1100 },
                    { Stats.MinimumPhysBaseDmg, 115 },
                    { Stats.MaximumPhysBaseDmg, 120 },
                    { Stats.DefenseBase, 45 },
                    { Stats.AttackRatePvm, 170 },
                    { Stats.DefenseRatePvm, 41 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 16;
                monster.Designation = "Elite Skeleton";
                monster.MoveRange = 2;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 42 },
                    { Stats.MaximumHealth, 1800 },
                    { Stats.MinimumPhysBaseDmg, 135 },
                    { Stats.MaximumPhysBaseDmg, 140 },
                    { Stats.DefenseBase, 65 },
                    { Stats.AttackRatePvm, 210 },
                    { Stats.DefenseRatePvm, 49 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 17;
                monster.Designation = "Cyclops";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 4;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 28 },
                    { Stats.MaximumHealth, 850 },
                    { Stats.MinimumPhysBaseDmg, 100 },
                    { Stats.MaximumPhysBaseDmg, 105 },
                    { Stats.DefenseBase, 35 },
                    { Stats.AttackRatePvm, 140 },
                    { Stats.DefenseRatePvm, 35 },
                    { Stats.PoisonResistance, 2 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 18;
                monster.Designation = "Gorgon";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 55 },
                    { Stats.MaximumHealth, 6000 },
                    { Stats.MinimumPhysBaseDmg, 165 },
                    { Stats.MaximumPhysBaseDmg, 175 },
                    { Stats.DefenseBase, 100 },
                    { Stats.AttackRatePvm, 275 },
                    { Stats.DefenseRatePvm, 82 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 6 },
                    { Stats.FireResistance, 6 },
                };

                monster.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var trap = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(trap);
                trap.Number = 100;
                trap.Designation = "Lance Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 4;
                trap.ViewRange = 1;
                trap.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                trap.Attribute = 1;
                trap.NumberOfMaximumItemDrops = 0;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 100 },
                    { Stats.MaximumPhysBaseDmg, 110 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 500 },
                };

                trap.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var trap = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(trap);
                trap.Number = 101;
                trap.Designation = "Iron Stick Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 0;
                trap.ViewRange = 1;
                trap.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                trap.Attribute = 1;
                trap.NumberOfMaximumItemDrops = 0;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 110 },
                    { Stats.MaximumPhysBaseDmg, 130 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 500 },
                };

                trap.AddAttributes(attributes, context, gameConfiguration);
            }

            {
                var trap = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(trap);
                trap.Number = 102;
                trap.Designation = "Fire Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 2;
                trap.ViewRange = 1;
                trap.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                trap.Attribute = 1;
                trap.NumberOfMaximumItemDrops = 0;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.AttackRatePvm, 400 },
                    { Stats.DefenseRatePvm, 500 },
                };

                trap.AddAttributes(attributes, context, gameConfiguration);
            }
        }
    }
}
