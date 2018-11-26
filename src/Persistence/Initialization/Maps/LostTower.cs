// <copyright file="LostTower.cs" company="MUnique">
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
    /// The initialization for the Lost Tower map.
    /// </summary>
    internal class LostTower : BaseMapInitializer
    {
        /// <inheritdoc/>
        protected override byte MapNumber => 4;

        /// <inheritdoc/>
        protected override string MapName => "Lost Tower";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[253], 1, Direction.SouthWest, SpawnTrigger.Automatic, 207, 207, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.SouthEast, SpawnTrigger.Automatic, 201, 201, 76, 76);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 6, 6, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 10, 10, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 5, 5, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 10, 10, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 12, 12, 100, 100);

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 239, 239);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 131, 131);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 63, 63);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 74, 74);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 59, 59);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 197, 197, 64, 64);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 72, 72);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 117, 117);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 117, 117);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 113, 113);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 128, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 136, 136);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 136, 136);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 242, 242, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 14, 14);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 118, 118);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 124, 124);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 200, 200, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 68, 68);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 64, 64);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 52, 52);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 44, 44);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 91, 91);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 73, 73);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 94, 94);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 206, 206);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 6, 6);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 243, 243, 65, 65);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 82, 82);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 244, 244, 92, 92);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 23, 23);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 22, 22);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 67, 67);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 81, 81);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 220, 220, 79, 79);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 60, 60);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 45, 45);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 165, 165);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 165, 165);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 197, 197);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 192, 192, 203, 203);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 200, 200, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 200, 200, 228, 228);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 190, 190);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 172, 172, 177, 177);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 216, 216);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 238, 238);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 210, 210);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 177, 177, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 126, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 95, 95, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 89, 89, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 190, 190);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 89, 89, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 194, 194);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 202, 202);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 219, 219);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 89, 89);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 116, 116, 113, 113);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 115, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 136, 136);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 136, 136);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 96, 96, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 94, 94, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 89, 89, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 106, 106, 112, 112);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 95, 95, 106, 106);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 87, 87, 7, 7);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 6, 6);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 37, 37);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 93, 93, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 99, 99, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 29, 29);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 39, 39);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 28, 28);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 13, 13);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 11, 11);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 39, 39);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 10, 10, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 52, 52);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 41, 41, 57, 57);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 123, 123, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 28, 28);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 23, 23, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 30, 30);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 6, 6, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 33, 33, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 87, 87);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 35, 35, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 98, 98);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 41, 41, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 191, 191);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 51, 51, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 211, 211);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 231, 231);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 39, 39, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 11, 11, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 31, 31, 127, 127);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[38], 1, Direction.Undefined, SpawnTrigger.Automatic, 31, 31, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 233, 233);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 21, 21, 216, 216);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 24, 24, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 41, 41, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 234, 234);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 13, 13, 191, 191);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 214, 214);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 223, 223);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 239, 239);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 125, 125, 180, 180);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 113, 113, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 98, 98, 231, 231);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 89, 89, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 126, 126, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 26, 26);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 13, 13, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 16, 16, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 53, 53);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 51, 51);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 52, 52, 92, 92);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 102, 102);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 34, 34, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 53, 53, 111, 111);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 45, 45, 137, 137);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 11, 11, 96, 96);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 43, 43, 170, 170);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 27, 27, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 24, 24, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 166, 166);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 227, 227, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 239, 239, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 179, 179);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 189, 189);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 246, 246, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 245, 245, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 226, 226);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 238, 238, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 238, 238, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 224, 224, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 176, 176);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 192, 192);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 209, 209);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 172, 172, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 83, 83, 220, 220);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 90, 90, 239, 239);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 215, 215);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 88, 88, 206, 206);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 96, 96, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 86, 86, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 124, 124);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 128, 128, 6, 6);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 34, 34);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 130, 130, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 105, 105, 55, 55);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 84, 84, 16, 16);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 124, 124, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 15, 15);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 13, 13, 33, 33);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 39, 39);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 55, 55, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 44, 44, 41, 41);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 25, 25);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 37, 37, 21, 21);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 29, 29, 24, 24);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 19, 19, 97, 97);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 42, 42, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 22, 22, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 5, 5, 122, 122);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 5, 5, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 137, 137);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 38, 38, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 36, 36, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 23, 23, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 8, 8, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 19, 19, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 50, 50, 241, 241);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 46, 46, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 18, 18, 183, 183);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 14, 14, 224, 224);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 7, 7, 240, 240);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 215, 215);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 49, 49, 191, 191);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 48, 48, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[38], 1, Direction.Undefined, SpawnTrigger.Automatic, 28, 28, 212, 212);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 23, 23, 239, 239);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 23, 23, 195, 195);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 31, 31, 191, 191);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 25, 25, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 11, 11, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 4, 4, 177, 177);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 47, 47, 247, 247);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 232, 232);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 217, 217);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 205, 205);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 103, 103, 197, 197);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 97, 97, 196, 196);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 188, 188);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 206, 206);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 199, 199);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 122, 122, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[41], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 168, 168);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 96, 96, 167, 167);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[37], 1, Direction.Undefined, SpawnTrigger.Automatic, 26, 26, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 19, 19, 86, 86);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 15, 15, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 34, 34, 137, 137);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 54, 54, 121, 121);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[35], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 10, 10);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 24, 24, 9, 9);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 30, 30, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 134, 134, 8, 8);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 135, 135, 17, 17);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 111, 111, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 99, 99, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 85, 85, 49, 49);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[40], 1, Direction.Undefined, SpawnTrigger.Automatic, 91, 91, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 32, 32);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 38, 38);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[36], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 40, 40);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 54, 54);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 229, 229);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 238, 238, 213, 213);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[34], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 233, 233);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[39], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 237, 237);

            // Traps:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 5, 5, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 6, 6, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 4, 4, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 15, 15, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 15, 15, 173, 173);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 15, 15, 174, 174);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 14, 14, 207, 207);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 4, 4, 194, 194);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 5, 5, 194, 194);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 6, 6, 194, 194);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 14, 14, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 5, 5, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 6, 6, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 7, 7, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 26, 26, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 26, 26, 103, 103);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 26, 26, 102, 102);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 26, 26, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 30, 30, 101, 101);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 30, 30, 100, 100);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 30, 30, 99, 99);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 29, 29, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 28, 28, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 26, 26, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 128, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 129, 129);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 27, 27, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 21, 21, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 22, 22, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 23, 23, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 45, 45, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 45, 45, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 45, 45, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 45, 45, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 45, 45, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 38, 38, 110, 110);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 38, 38, 109, 109);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.NorthEast, SpawnTrigger.Automatic, 38, 38, 108, 108);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 35, 35, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 34, 34, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 33, 33, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 36, 36, 125, 125);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 37, 37, 119, 119);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 37, 37, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 34, 34, 128, 128);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 34, 34, 129, 129);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 34, 34, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 47, 47, 131, 131);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 47, 47, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 41, 41, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 41, 41, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 36, 36, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 36, 36, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 46, 46, 169, 169);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 46, 46, 170, 170);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 46, 46, 171, 171);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 46, 46, 172, 172);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 42, 42, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 42, 42, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 52, 52, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 53, 53, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 54, 54, 114, 114);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 53, 53, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 54, 54, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 52, 52, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 51, 51, 198, 198);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 51, 51, 199, 199);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 51, 51, 200, 200);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 51, 51, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 85, 85, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 86, 86, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 84, 84, 120, 120);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 131, 131);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 130, 130);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 82, 82, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 83, 83, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 84, 84, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 85, 85, 175, 175);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 82, 82, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 83, 83, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 84, 84, 184, 184);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 82, 82, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 83, 83, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 84, 84, 201, 201);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 245, 245);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 93, 93, 246, 246);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 100, 100, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 101, 101, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 102, 102, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 103, 103, 185, 185);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 98, 98, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 99, 99, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 100, 100, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 101, 101, 225, 225);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 111, 111, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 110, 110, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 107, 107, 242, 242);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 107, 107, 243, 243);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 107, 107, 244, 244);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 126, 126, 104, 104);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 126, 126, 105, 105);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 126, 126, 106, 106);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 126, 126, 107, 107);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 126, 126, 115, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 127, 127, 115, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 125, 125, 115, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 123, 123, 135, 135);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 123, 123, 134, 134);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 123, 123, 133, 133);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 123, 123, 132, 132);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 121, 121, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 122, 122, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 120, 120, 208, 208);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 112, 112, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 113, 113, 227, 227);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 132, 132, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 133, 133, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 134, 134, 126, 126);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 132, 132, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 133, 133, 178, 178);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 128, 128, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 129, 129, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 130, 130, 221, 221);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 202, 202, 45, 45);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 202, 202, 46, 46);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 202, 202, 47, 47);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 196, 196, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 198, 198, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 197, 197, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 199, 199, 62, 62);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 196, 196, 58, 58);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 196, 196, 57, 57);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 196, 196, 56, 56);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 202, 202, 48, 48);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 219, 219, 234, 234);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 219, 219, 235, 235);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 219, 219, 236, 236);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 219, 219, 237, 237);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 226, 226, 115, 115);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 226, 226, 116, 116);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthEast, SpawnTrigger.Automatic, 226, 226, 117, 117);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 226, 226, 233, 233);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 227, 227, 233, 233);
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[103], 1, Direction.SouthWest, SpawnTrigger.Automatic, 228, 228, 233, 233);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 34;
                monster.Designation = "Cursed Wizard";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 54 },
                    { Stats.MaximumHealth, 4000 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 170 },
                    { Stats.DefenseBase, 95 },
                    { Stats.AttackRatePvm, 270 },
                    { Stats.DefenseRatePvm, 80 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 5 },
                    { Stats.IceResistance, 5 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
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
                monster.Number = 35;
                monster.Designation = "Death Gorgon";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 64 },
                    { Stats.MaximumHealth, 6000 },
                    { Stats.MinimumPhysBaseDmg, 200 },
                    { Stats.MaximumPhysBaseDmg, 210 },
                    { Stats.DefenseBase, 130 },
                    { Stats.AttackRatePvm, 320 },
                    { Stats.DefenseRatePvm, 94 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 6 },
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
                monster.Number = 36;
                monster.Designation = "Shadow";
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
                    { Stats.Level, 47 },
                    { Stats.MaximumHealth, 2800 },
                    { Stats.MinimumPhysBaseDmg, 148 },
                    { Stats.MaximumPhysBaseDmg, 153 },
                    { Stats.DefenseBase, 78 },
                    { Stats.AttackRatePvm, 235 },
                    { Stats.DefenseRatePvm, 67 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 3 },
                    { Stats.IceResistance, 3 },
                    { Stats.WaterResistance, 3 },
                    { Stats.FireResistance, 5 },
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
                monster.Number = 37;
                monster.Designation = "Devil";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 60 },
                    { Stats.MaximumHealth, 5000 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 195 },
                    { Stats.DefenseBase, 115 },
                    { Stats.AttackRatePvm, 300 },
                    { Stats.DefenseRatePvm, 88 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 5 },
                    { Stats.IceResistance, 5 },
                    { Stats.WaterResistance, 5 },
                    { Stats.FireResistance, 7 },
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
                monster.Number = 38;
                monster.Designation = "Balrog";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 9000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 240 },
                    { Stats.DefenseBase, 160 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 99 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 10 },
                    { Stats.IceResistance, 10 },
                    { Stats.WaterResistance, 10 },
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
                monster.Number = 39;
                monster.Designation = "Poison Shadow";
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
                    { Stats.Level, 50 },
                    { Stats.MaximumHealth, 3500 },
                    { Stats.MinimumPhysBaseDmg, 155 },
                    { Stats.MaximumPhysBaseDmg, 160 },
                    { Stats.DefenseBase, 85 },
                    { Stats.AttackRatePvm, 250 },
                    { Stats.DefenseRatePvm, 73 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 4 },
                    { Stats.WaterResistance, 4 },
                    { Stats.FireResistance, 6 },
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
                monster.Number = 40;
                monster.Designation = "Death Knight";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 62 },
                    { Stats.MaximumHealth, 5500 },
                    { Stats.MinimumPhysBaseDmg, 190 },
                    { Stats.MaximumPhysBaseDmg, 200 },
                    { Stats.DefenseBase, 120 },
                    { Stats.AttackRatePvm, 310 },
                    { Stats.DefenseRatePvm, 91 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 6 },
                    { Stats.FireResistance, 7 },
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
                monster.Number = 41;
                monster.Designation = "Death Cow";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 57 },
                    { Stats.MaximumHealth, 4500 },
                    { Stats.MinimumPhysBaseDmg, 170 },
                    { Stats.MaximumPhysBaseDmg, 180 },
                    { Stats.DefenseBase, 110 },
                    { Stats.AttackRatePvm, 285 },
                    { Stats.DefenseRatePvm, 85 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 5 },
                    { Stats.IceResistance, 5 },
                    { Stats.WaterResistance, 5 },
                    { Stats.FireResistance, 7 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var trap = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(trap);
                trap.Number = 103;
                trap.Designation = "Meteorite Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 0;
                trap.ViewRange = 1;
                trap.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                trap.Attribute = 1;
                trap.NumberOfMaximumItemDrops = 0;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 190 },
                    { Stats.DefenseBase, 0 },
                    { Stats.AttackRatePvm, 450 },
                    { Stats.DefenseRatePvm, 500 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(trap.Attributes.Add);
            }
        }
    }
}
