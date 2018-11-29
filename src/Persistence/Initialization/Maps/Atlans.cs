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
        /// <summary>
        /// Initializes a new instance of the <see cref="Atlans"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Atlans(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 7;

        /// <inheritdoc/>
        protected override string MapName => "Atlans";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns()
        {
            var npcDictionary = this.GameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Guard:
            yield return this.CreateMonsterSpawn(npcDictionary[240], 23, 17, Direction.SouthWest);

            // Monsters:
            yield return this.CreateMonsterSpawn(npcDictionary[48], 236, 177);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 212, 96);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 21, 41);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 23, 33);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 31, 31);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 39, 23);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 41, 11);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 21, 48);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 47, 41);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 39, 63);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 72, 42);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 23, 56);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 23, 64);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 35, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 29, 75);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 62, 21);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 78, 19);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 67, 36);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 24, 86);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 48, 57);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 45, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 19, 125);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 49, 8);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 46, 20);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 39, 35);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 37, 50);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 20, 75);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 63, 44);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 68, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 49, 64);
            yield return this.CreateMonsterSpawn(npcDictionary[45], 29, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 46, 21);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 48, 9);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 50, 65);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 51, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 80, 33);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 62, 17);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 61, 11);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 69, 19);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 75, 15);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 83, 15);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 84, 21);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 67, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 59, 34);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 86, 37);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 78, 44);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 71, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 65, 59);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 65, 51);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 55, 44);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 40, 48);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 31, 53);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 24, 90);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 21, 100);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 25, 105);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 30, 110);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 21, 118);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 38, 117);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 141, 11);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 127, 17);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 114, 26);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 107, 18);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 104, 13);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 115, 12);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 89, 62);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 75, 76);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 69, 87);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 50, 93);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 23, 72);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 78, 23);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 70, 38);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 55, 25);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 30, 122);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 17, 126);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 122, 29);
            yield return this.CreateMonsterSpawn(npcDictionary[46], 108, 40);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 66, 222);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 63, 218);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 57, 225);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 171, 162);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 175, 169);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 178, 174);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 173, 179);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 171, 187);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 165, 193);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 159, 198);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 152, 201);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 144, 201);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 137, 205);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 124, 206);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 115, 210);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 104, 211);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 94, 210);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 87, 214);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 76, 215);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 47, 224);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 46, 217);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 29, 226);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 22, 213);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 24, 204);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 34, 200);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 35, 209);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 40, 210);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 45, 205);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 47, 211);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 37, 232);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 226, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 22, 230);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 41, 224);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 82, 215);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 167, 156);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 177, 146);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 156, 156);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 162, 149);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 152, 169);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 72, 128);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 70, 138);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 61, 147);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 49, 158);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 33, 169);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 22, 173);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 55, 181);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 67, 186);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 73, 180);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 89, 184);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 101, 183);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 111, 183);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 122, 178);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 129, 171);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 138, 167);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 147, 164);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 170, 145);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 163, 138);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 53, 220);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 41, 222);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 29, 231);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 22, 232);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 27, 209);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 36, 205);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 109, 211);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 131, 207);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 155, 196);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 167, 154);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 177, 19);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 118, 167);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 111, 175);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 63, 178);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 162, 141);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 138, 176);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 41, 109);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 49, 100);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 54, 89);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 62, 87);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 71, 82);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 79, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 85, 64);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 94, 60);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 101, 50);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 106, 48);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 112, 45);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 109, 38);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 118, 37);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 122, 29);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 119, 20);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 124, 14);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 133, 14);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 150, 14);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 161, 11);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 166, 16);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 171, 18);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 182, 14);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 190, 24);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 196, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 200, 17);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 208, 9);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 218, 47);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 222, 14);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 117, 74);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 139, 61);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 152, 61);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 172, 55);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 189, 56);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 226, 46);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 240, 11);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 227, 16);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 152, 22);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 142, 15);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 128, 22);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 116, 9);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 113, 27);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 33, 108);
            yield return this.CreateMonsterSpawn(npcDictionary[47], 35, 118);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 228, 16);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 237, 17);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 237, 26);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 238, 40);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 235, 48);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 251, 51);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 206, 55);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 200, 49);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 193, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 184, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 178, 58);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 170, 61);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 156, 59);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 144, 63);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 133, 61);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 133, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 125, 78);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 118, 85);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 124, 90);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 116, 98);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 106, 101);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 99, 108);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 91, 115);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 81, 123);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 126, 167);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 111, 174);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 90, 177);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 76, 180);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 62, 178);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 42, 178);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 27, 161);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 42, 152);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 19, 159);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 62, 140);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 70, 133);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 168, 54);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 7, 52);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 225, 45);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 207, 9);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 154, 69);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 111, 82);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 81, 116);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 72, 125);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 122, 66);
            yield return this.CreateMonsterSpawn(npcDictionary[49], 19, 230);
            yield return this.CreateMonsterSpawn(npcDictionary[49], 24, 227);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 219, 56);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 215, 65);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 232, 55);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 231, 64);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 238, 71);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 229, 74);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 217, 75);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 225, 81);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 215, 86);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 223, 89);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 218, 101);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 232, 95);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 223, 70);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 211, 59);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 232, 86);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 208, 104);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 235, 106);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 224, 108);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 219, 124);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 225, 117);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 227, 123);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 219, 133);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 226, 134);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 224, 142);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 226, 154);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 221, 150);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 220, 168);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 232, 166);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 231, 176);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 217, 197);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 215, 182);
            yield return this.CreateMonsterSpawn(npcDictionary[49], 211, 192);
            yield return this.CreateMonsterSpawn(npcDictionary[49], 229, 203);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 208, 183);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 215, 174);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 222, 190);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 229, 190);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 230, 181);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 226, 161);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 208, 198);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 232, 206);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 231, 196);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 223, 202);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 224, 184);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 230, 171);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 221, 158);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 233, 124);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 232, 102);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 200, 103);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 192, 101);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 179, 92);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 187, 94);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 177, 101);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 166, 117);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 159, 112);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 156, 122);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 173, 109);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 167, 107);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 150, 120);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 142, 125);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 136, 129);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 129, 130);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 117, 130);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 121, 135);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 144, 109);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 122, 144);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 112, 137);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 101, 147);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 97, 142);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 144, 132);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 93, 136);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 90, 148);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 124, 125);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 87, 132);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 86, 140);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 78, 145);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 79, 136);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 67, 147);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 71, 140);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 105, 137);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 83, 155);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 81, 164);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 70, 168);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 76, 174);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 85, 185);
            yield return this.CreateMonsterSpawn(npcDictionary[52], 87, 175);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 65, 187);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 64, 159);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 186, 99);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 185, 90);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 147, 114);
            yield return this.CreateMonsterSpawn(npcDictionary[51], 84, 170);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 15, 234);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 20, 237);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 34, 237);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 47, 231);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 54, 232);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 20, 218);
            yield return this.CreateMonsterSpawn(npcDictionary[48], 34, 176);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 43 },
                    { Stats.MaximumHealth, 2400 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 140 },
                    { Stats.DefenseBase, 65 },
                    { Stats.AttackRatePvm, 215 },
                    { Stats.DefenseRatePvm, 52 },
                    { Stats.PoisonResistance, 1 },
                    { Stats.IceResistance, 1 },
                    { Stats.WaterResistance, 1 },
                    { Stats.FireResistance, 1 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 45 },
                    { Stats.MaximumHealth, 2800 },
                    { Stats.MinimumPhysBaseDmg, 135 },
                    { Stats.MaximumPhysBaseDmg, 145 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 225 },
                    { Stats.DefenseRatePvm, 58 },
                    { Stats.PoisonResistance, 2 },
                    { Stats.IceResistance, 2 },
                    { Stats.WaterResistance, 3 },
                    { Stats.FireResistance, 2 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 46 },
                    { Stats.MaximumHealth, 3200 },
                    { Stats.MinimumPhysBaseDmg, 140 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.DefenseBase, 75 },
                    { Stats.AttackRatePvm, 230 },
                    { Stats.DefenseRatePvm, 64 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 2 },
                    { Stats.WaterResistance, 2 },
                    { Stats.FireResistance, 2 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 70 },
                    { Stats.MaximumHealth, 9000 },
                    { Stats.MinimumPhysBaseDmg, 240 },
                    { Stats.MaximumPhysBaseDmg, 270 },
                    { Stats.DefenseBase, 180 },
                    { Stats.AttackRatePvm, 350 },
                    { Stats.DefenseRatePvm, 115 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 310 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 12 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 74 },
                    { Stats.MaximumHealth, 19000 },
                    { Stats.MinimumPhysBaseDmg, 250 },
                    { Stats.MaximumPhysBaseDmg, 310 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 430 },
                    { Stats.DefenseRatePvm, 125 },
                    { Stats.PoisonResistance, 12 },
                    { Stats.IceResistance, 12 },
                    { Stats.WaterResistance, 12 },
                    { Stats.FireResistance, 12 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 66 },
                    { Stats.MaximumHealth, 7000 },
                    { Stats.MinimumPhysBaseDmg, 210 },
                    { Stats.MaximumPhysBaseDmg, 230 },
                    { Stats.DefenseBase, 150 },
                    { Stats.AttackRatePvm, 330 },
                    { Stats.DefenseRatePvm, 98 },
                    { Stats.PoisonResistance, 6 },
                    { Stats.IceResistance, 6 },
                    { Stats.WaterResistance, 6 },
                    { Stats.FireResistance, 6 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 68 },
                    { Stats.MaximumHealth, 8000 },
                    { Stats.MinimumPhysBaseDmg, 230 },
                    { Stats.MaximumPhysBaseDmg, 260 },
                    { Stats.DefenseBase, 170 },
                    { Stats.AttackRatePvm, 340 },
                    { Stats.DefenseRatePvm, 110 },
                    { Stats.PoisonResistance, 7 },
                    { Stats.IceResistance, 7 },
                    { Stats.WaterResistance, 7 },
                    { Stats.FireResistance, 7 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
