// <copyright file="LostTower.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.Persistence.Initialization.Skills;

    /// <summary>
    /// The initialization for the Lost Tower map.
    /// </summary>
    internal class LostTower : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LostTower"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public LostTower(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 4;

        /// <inheritdoc/>
        protected override string MapName => "Lost Tower";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[253], 207, 76, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[240], 201, 76, Direction.SouthEast);
        }

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 6, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 10, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 5, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 15, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 12, 99);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 10, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 12, 100);

            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 193, 239);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 191, 131);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 191, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 37);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 63);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 74);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 195, 40);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 190, 59);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 197, 64);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 244, 72);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 244, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 245, 117);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 219, 117);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 209, 107);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 201, 113);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 212, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 112);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 126);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 175, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 184, 128);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 199, 134);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 206, 136);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 227, 136);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 242, 135);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 229, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 217, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 244, 89);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 207, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 38);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 236, 26);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 226, 14);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 211, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 210, 101);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 210, 114);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 244, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 195, 99);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 226, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 199, 124);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 164, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 200, 104);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 205, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 219, 68);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 211, 64);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 209, 52);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 210, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 210, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 194, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 204, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 236, 44);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 222, 109);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 218, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 228, 91);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 244, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 228, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 202, 25);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 193, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 209, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 235, 73);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 196, 89);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 219, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 190, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 205, 62);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 29, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 30, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 33, 206);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 54, 213);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 243, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 109);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 219, 6);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 244, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 239, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 244, 25);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 243, 40);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 243, 65);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 244, 82);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 244, 92);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 210, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 226, 22);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 227, 33);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 227, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 228, 67);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 228, 81);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 228, 104);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 191, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 220, 79);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 219, 60);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 219, 45);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 219, 34);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 202, 38);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 208, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 196, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 188, 165);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 179, 165);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 165, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 167, 197);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 167, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 192, 203);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 165, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 184, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 191, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 198, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 214, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 228, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 235, 237);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 224, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 206, 237);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 200, 228);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 209, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 225, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 209, 190);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 198, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 187, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 193, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 172, 177);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 198, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 217, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 227, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 217, 217);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 201, 216);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 191, 238);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 193, 217);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 178, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 178, 210);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 177, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 126, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 133, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 133, 225);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 133, 185);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 133, 209);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 132, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 125, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 128, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 134, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 129, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 97, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 107, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 95, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 89, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 83, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 86, 190);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 83, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 83, 213);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 89, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 84, 237);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 86, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 98, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 108, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 116, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 118, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 116, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 111, 194);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 115, 178);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 97, 202);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 97, 214);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 102, 225);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 98, 236);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 111, 219);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 123, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 116, 134);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 97, 133);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 85, 132);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 85, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 85, 96);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 101, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 118, 89);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 133, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 116, 113);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 133, 115);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 104, 136);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 124, 136);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 96, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 94, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 89, 135);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 133, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 106, 112);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 95, 106);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 112, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 133, 109);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 121, 119);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 123, 130);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 87, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 97, 6);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 117, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 124, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 133, 26);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 134, 37);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 134, 49);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 123, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 115, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 98, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 84, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 93, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 99, 40);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 101, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 98, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 112, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 124, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 111, 39);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 86, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 98, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 115, 28);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 55, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 46, 13);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 39, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 22, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 14, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 53, 39);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 10, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 7, 52);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 15, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 25, 49);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 38, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 48, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 41, 57);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 123, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 55, 28);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 46, 25);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 30, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 23, 40);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 22, 30);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 39, 38);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 6, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 33, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 46, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 55, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 53, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 42, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 35, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 25, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 15, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 8, 107);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 53, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 41, 121);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 115, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 25, 135);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 42, 135);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 52, 135);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 30, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 38, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 191);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 51, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 231);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 15, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 18, 107);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 39, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 11, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 54, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 31, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 55, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 105, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 55, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[38], 31, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 30, 233);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 21, 216);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 38, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 29, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 24, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 48, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 41, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 38, 234);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 38, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 18, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 13, 191);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 38, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 215, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 245, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 235, 214);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 165, 223);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 127, 239);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 125, 180);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 113, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 98, 231);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 89, 119);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 86, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 108, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 126, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 86, 26);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 111, 26);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 48, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 13, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 16, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 38, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 54, 51);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 45, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 25, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 52, 92);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 30, 102);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 34, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 53, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 45, 137);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 11, 96);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 43, 170);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 27, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 54, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 24, 225);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 226, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 239, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 227, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 239, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 179);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 246, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 245, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 237, 226);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 236, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 238, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 237, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 232, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 238, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 224, 185);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 216, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 207, 178);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 196, 176);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 187, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 185, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 187, 209);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 185, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 175, 217);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 176, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 206, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 215, 209);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 184, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 172, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 173, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 206, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 218, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 112, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 83, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 90, 239);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 133, 215);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 88, 206);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 121, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 104, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 86, 109);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 96, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 128, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 122, 101);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 101, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 86, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 132, 124);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 110, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 103, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 128, 6);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 127, 34);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 130, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 105, 55);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 84, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 124, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 14, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 13, 33);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 30, 39);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 55, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 44, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 25);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 37, 21);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 29, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 19, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 42, 109);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 22, 121);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 5, 122);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 5, 132);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 14, 137);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 38, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 36, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 23, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 18, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 7, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 8, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 19, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 50, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 54, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 46, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 18, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 14, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 7, 240);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 48, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 48, 215);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 49, 191);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 48, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[38], 28, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 23, 239);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 23, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 31, 191);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 25, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 11, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 4, 177);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 47, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 104, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 102, 217);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 103, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 103, 197);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 97, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 101, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 110, 206);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 115, 199);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 135, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 122, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[41], 101, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 96, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[37], 26, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 19, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 15, 119);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 34, 137);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 54, 121);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[35], 30, 10);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 24, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 30, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 110, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 134, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 135, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 111, 46);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 99, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 85, 49);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[40], 91, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 195, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 189, 38);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[36], 195, 40);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 189, 48);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 196, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 196, 207);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 195, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 230, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 238, 213);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 210, 225);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[34], 215, 236);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 173, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 183, 233);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[39], 173, 237);

            // Traps:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 5, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 6, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 4, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 15, 172, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 15, 173, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 15, 174, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 14, 207, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 4, 194, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 5, 194, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 6, 194, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 14, 208, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 5, 237, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 6, 237, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 7, 237, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 26, 104, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 26, 103, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 26, 102, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 26, 101, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 30, 101, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 30, 100, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 30, 99, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 29, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 28, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 27, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 26, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 27, 128, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 27, 129, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 27, 130, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 21, 243, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 22, 243, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 23, 243, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 45, 110, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 45, 110, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 45, 110, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 45, 109, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 45, 108, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 38, 110, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 38, 109, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 38, 108, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 35, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 34, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 33, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 36, 125, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 37, 119, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 37, 120, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 34, 128, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 34, 129, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 34, 130, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 47, 131, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 47, 132, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 41, 132, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 41, 133, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 36, 133, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 36, 134, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 46, 169, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 46, 170, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 46, 171, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 46, 172, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 42, 245, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 42, 246, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 52, 114, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 53, 114, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 54, 114, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 53, 178, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 54, 178, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 52, 178, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 51, 198, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 51, 199, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 51, 200, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 51, 201, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 85, 120, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 86, 120, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 84, 120, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 131, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 130, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 132, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 82, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 83, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 84, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 85, 175, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 82, 184, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 83, 184, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 84, 184, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 82, 201, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 83, 201, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 84, 201, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 243, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 244, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 245, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 93, 246, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 100, 185, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 101, 185, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 102, 185, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 103, 185, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 98, 225, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 99, 225, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 100, 225, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 101, 225, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 111, 227, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 110, 227, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 107, 242, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 107, 243, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 107, 244, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 126, 104, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 126, 105, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 126, 106, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 126, 107, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 126, 115, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 127, 115, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 125, 115, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 123, 135, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 123, 134, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 123, 133, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 123, 132, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 121, 208, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 122, 208, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 120, 208, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 112, 227, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 113, 227, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 132, 126, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 133, 126, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 134, 126, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 132, 178, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 133, 178, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 128, 221, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 129, 221, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 130, 221, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 202, 45, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 202, 46, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 202, 47, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 196, 62, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 198, 62, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 197, 62, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 199, 62, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 196, 58, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 196, 57, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 196, 56, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 202, 48, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 219, 234, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 219, 235, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 219, 236, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 219, 237, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 226, 115, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 226, 116, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 226, 117, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 226, 233, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 227, 233, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[103], 228, 233, Direction.SouthWest);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Meteorite);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 54 },
                    { Stats.MaximumHealth, 4000 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 170 },
                    { Stats.DefenseBase, 95 },
                    { Stats.AttackRatePvm, 270 },
                    { Stats.DefenseRatePvm, 80 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.WaterResistance, 7f / 255 },
                    { Stats.FireResistance, 7f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 64 },
                    { Stats.MaximumHealth, 6000 },
                    { Stats.MinimumPhysBaseDmg, 200 },
                    { Stats.MaximumPhysBaseDmg, 210 },
                    { Stats.DefenseBase, 130 },
                    { Stats.AttackRatePvm, 320 },
                    { Stats.DefenseRatePvm, 94 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.WaterResistance, 6f / 255 },
                    { Stats.FireResistance, 8f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 47 },
                    { Stats.MaximumHealth, 2800 },
                    { Stats.MinimumPhysBaseDmg, 148 },
                    { Stats.MaximumPhysBaseDmg, 153 },
                    { Stats.DefenseBase, 78 },
                    { Stats.AttackRatePvm, 235 },
                    { Stats.DefenseRatePvm, 67 },
                    { Stats.PoisonResistance, 3f / 255 },
                    { Stats.IceResistance, 3f / 255 },
                    { Stats.WaterResistance, 3f / 255 },
                    { Stats.FireResistance, 5f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lightning);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 60 },
                    { Stats.MaximumHealth, 5000 },
                    { Stats.MinimumPhysBaseDmg, 180 },
                    { Stats.MaximumPhysBaseDmg, 195 },
                    { Stats.DefenseBase, 115 },
                    { Stats.AttackRatePvm, 300 },
                    { Stats.DefenseRatePvm, 88 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.WaterResistance, 5f / 255 },
                    { Stats.FireResistance, 7f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 9000 },
                    { Stats.MinimumPhysBaseDmg, 220 },
                    { Stats.MaximumPhysBaseDmg, 240 },
                    { Stats.DefenseBase, 160 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 99 },
                    { Stats.PoisonResistance, 10f / 255 },
                    { Stats.IceResistance, 10f / 255 },
                    { Stats.WaterResistance, 10f / 255 },
                    { Stats.FireResistance, 15f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 50 },
                    { Stats.MaximumHealth, 3500 },
                    { Stats.MinimumPhysBaseDmg, 155 },
                    { Stats.MaximumPhysBaseDmg, 160 },
                    { Stats.DefenseBase, 85 },
                    { Stats.AttackRatePvm, 250 },
                    { Stats.DefenseRatePvm, 73 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 4f / 255 },
                    { Stats.WaterResistance, 4f / 255 },
                    { Stats.FireResistance, 6f / 255 },
                    { Stats.PoisonDamageMultiplier, 0.03f },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 62 },
                    { Stats.MaximumHealth, 5500 },
                    { Stats.MinimumPhysBaseDmg, 190 },
                    { Stats.MaximumPhysBaseDmg, 200 },
                    { Stats.DefenseBase, 120 },
                    { Stats.AttackRatePvm, 310 },
                    { Stats.DefenseRatePvm, 91 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.WaterResistance, 6f / 255 },
                    { Stats.FireResistance, 7f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 57 },
                    { Stats.MaximumHealth, 4500 },
                    { Stats.MinimumPhysBaseDmg, 170 },
                    { Stats.MaximumPhysBaseDmg, 180 },
                    { Stats.DefenseBase, 110 },
                    { Stats.AttackRatePvm, 285 },
                    { Stats.DefenseRatePvm, 85 },
                    { Stats.PoisonResistance, 5f / 255 },
                    { Stats.IceResistance, 5f / 255 },
                    { Stats.WaterResistance, 5f / 255 },
                    { Stats.FireResistance, 7f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var trap = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(trap);
                trap.Number = 103;
                trap.Designation = "Meteorite Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 3;
                trap.ViewRange = 1;
                trap.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                trap.AttackDelay = new TimeSpan(1000 * TimeSpan.TicksPerMillisecond);
                trap.RespawnDelay = new TimeSpan(3 * TimeSpan.TicksPerSecond);
                trap.ObjectKind = NpcObjectKind.Trap;
                trap.IntelligenceTypeName = typeof(AttackAreaWhenPressedTrapIntelligence).FullName;
                trap.Attribute = 1;
                trap.NumberOfMaximumItemDrops = 0;
                trap.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.FlameofEvil);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 1000 },
                    { Stats.MinimumPhysBaseDmg, 160 },
                    { Stats.MaximumPhysBaseDmg, 190 },
                    { Stats.AttackRatePvm, 450 },
                    { Stats.DefenseRatePvm, 500 },
                };

                trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
