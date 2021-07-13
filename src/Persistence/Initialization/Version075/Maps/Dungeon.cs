// <copyright file="Dungeon.cs" company="MUnique">
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
    /// The initialization for the Dungeon map.
    /// </summary>
    internal class Dungeon : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dungeon"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Dungeon(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 1;

        /// <inheritdoc/>
        protected override string MapName => "Dungeon";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 50, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 42, 122);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 4, 73);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 12, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 2, 78);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 108, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 120, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 98, 221);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 100, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 100, 225);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 108, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 107, 230);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 65, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 84, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 61, 169);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 94, 242);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 108, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 94, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 69, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 87, 177);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 136, 149);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 70, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 62, 181);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 61, 213);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 133, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 69, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 57, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 49, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 31, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 29, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 83, 198);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 18, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 33, 209);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 135, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 86, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 151, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 168, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 46, 210);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 45, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 46, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 46, 146);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 49, 182);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 44, 162);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 180);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 164, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 137, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 130, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 92, 157);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 102, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 113, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 217, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 222, 223);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 163, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 164, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 230, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 236, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 150, 193);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 142, 218);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 134, 240);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 241, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 199, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 170, 126);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 120, 243);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 240, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 242, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 241, 226);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 246, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 243, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 163, 114);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 244, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 213, 248);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 173, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 148, 149);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 163, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 141, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 145, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 112, 93);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 98, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 99, 49);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 104, 65);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 183, 27);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 187, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 158, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 159, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 115, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 123, 120);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 114, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 68, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 76, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 124, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 90, 6);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 114, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 138, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 212, 83);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 211, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 226, 58);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 239, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 235, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 247, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 247, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 240, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 241, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 28, 18);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 22, 4);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 18, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 18, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 34, 4);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 119, 47);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 22, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 33, 78);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 25, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 29, 80);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 42, 117);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 14, 107);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 21, 82);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 22, 65);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 39, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 29, 66);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 16, 55);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 35, 55);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 116, 47);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 122, 46);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 193, 30);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 120, 75);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 149, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 81, 58);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 126, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 71, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 69, 88);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 86, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 88, 104);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 98, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 156, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 165, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 208, 162);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 220, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 176, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 168, 150);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 156, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 176, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 185, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 181, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 198, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 190, 172);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 227, 248);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 181, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 152, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 128, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 156, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 193, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 194, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 194, 221);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 86, 150);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 98, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 79, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 78, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 97, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 105, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 85, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 84, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 85, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 100, 198);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 95, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 90, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 68, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 61, 238);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 73, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 73, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 219);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 7, 219);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 17, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 13, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 27, 236);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 53, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 52, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 62, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 70, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 77, 152);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 110, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 119, 146);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 120, 159);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 111, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 125, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 135, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 144, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 125, 218);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 143, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 50, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 197, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 247, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 231, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 223, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 213, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 209, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 193, 148);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 188, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 206, 152);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 212, 151);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 220, 172);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 247, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 247, 176);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 52, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 50, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 38, 234);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 16, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 25, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 235, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 228, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 214, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 183, 128);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 175, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 175, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 183, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 87, 114);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 65, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 68, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 80, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 70, 76);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 168, 96);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 196, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 240, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 247, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 248, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 244, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 229, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 205, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 155, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 137, 33);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 122, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 118, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 48, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 38, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 160, 10);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 194, 18);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 213, 12);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 221, 14);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 243, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 233, 66);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 39, 112);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 26, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 15, 113);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 12, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 44, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 41, 88);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 19, 120);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 37, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 30, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 27, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 10, 68);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 33, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 12, 121);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 43, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 8, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 7, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 26, 67);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 50, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 42, 122);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 4, 73);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 12, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 2, 78);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 108, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 120, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 107, 230);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 65, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 84, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 61, 169);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 94, 242);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 108, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 94, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 69, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 87, 177);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 136, 149);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 70, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 62, 181);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 61, 213);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 133, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 69, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 57, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 49, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 31, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 29, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 83, 198);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 18, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 33, 209);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 135, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 86, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 151, 212);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 168, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 46, 210);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 45, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 46, 205);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 46, 146);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 49, 182);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 44, 162);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 180);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 183);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 164, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 137, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 130, 244);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 92, 157);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 102, 174);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 113, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 217, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 222, 223);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 163, 184);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 164, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 230, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 236, 167);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 150, 193);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 142, 218);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 134, 240);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 241, 173);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 199, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 170, 126);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 120, 243);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 240, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 242, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 241, 226);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 246, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 243, 201);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 163, 114);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 244, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 213, 248);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 173, 200);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 148, 149);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 163, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 141, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 145, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 112, 93);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 98, 54);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 99, 49);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 104, 65);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 183, 27);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 187, 15);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 158, 17);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 159, 41);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 115, 8);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 123, 120);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 114, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 68, 110);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 76, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 124, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 90, 6);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 114, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 138, 86);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 212, 83);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 211, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 226, 58);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 239, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 235, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 247, 87);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 247, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 240, 7);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 241, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 28, 18);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 22, 4);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 18, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 18, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 34, 4);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 119, 47);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 22, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 33, 78);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 25, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 29, 80);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 42, 117);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 14, 107);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 21, 82);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 22, 65);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 39, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 29, 66);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 16, 55);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 35, 55);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 116, 47);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 122, 46);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 193, 30);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 120, 75);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 149, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 81, 58);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 126, 94);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 71, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 69, 88);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 86, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 88, 104);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 98, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 156, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 165, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 208, 162);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 220, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 176, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 168, 150);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 156, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 176, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 185, 161);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 181, 171);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 198, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 190, 172);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 227, 248);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 181, 246);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 152, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 128, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 156, 247);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 193, 224);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 194, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 194, 221);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 86, 150);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 98, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 79, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 78, 189);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 97, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 105, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 85, 208);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 84, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 85, 229);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 100, 198);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 95, 192);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 90, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 68, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 61, 238);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 73, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[12], 73, 241);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 3, 219);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 7, 219);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[14], 17, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 13, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 27, 236);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 45, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 53, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 52, 166);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 62, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 70, 154);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 77, 152);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 110, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 119, 146);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 120, 159);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 111, 188);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 125, 195);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 135, 196);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 144, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 125, 218);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 143, 232);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 50, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 197, 245);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 247, 211);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 231, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 223, 187);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 213, 186);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 209, 175);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 193, 148);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 188, 145);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 206, 152);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 212, 151);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 220, 172);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 247, 168);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 247, 176);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 52, 222);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 50, 235);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 38, 234);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 16, 227);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 25, 220);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 235, 108);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 228, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[17], 214, 116);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[11], 183, 128);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 175, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 175, 118);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 183, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 87, 114);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 65, 127);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 68, 125);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[15], 80, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[5], 70, 76);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 168, 96);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[13], 196, 100);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 240, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 247, 77);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 248, 20);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 244, 9);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 229, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 205, 24);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 155, 32);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 137, 33);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 122, 29);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 118, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[16], 48, 11);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 38, 23);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 160, 10);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 194, 18);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 213, 12);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 221, 14);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 243, 16);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 233, 66);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 39, 112);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 26, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 15, 113);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 12, 97);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[8], 44, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 41, 88);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 19, 120);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[10], 37, 98);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 30, 105);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 27, 53);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 10, 68);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 33, 90);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[9], 12, 121);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 43, 111);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 8, 123);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 7, 56);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[18], 26, 67);

            // Traps:
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 10, 26, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 11, 26, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 27, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 24, 5, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 24, 4, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 27, 11, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 23, 24, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 27, 21, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 19, 19, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 22, 24, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 23, 29, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 23, 28, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 33, 9, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 35, 9, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 39, 18, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 39, 17, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 39, 16, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[102], 45, 224, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 48, 193, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 49, 193, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[102], 66, 71, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[102], 80, 61, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 90, 164, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 92, 164, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 91, 164, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 126, 99, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 123, 99, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 120, 99, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 117, 99, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 136, 95, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 139, 95, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 128, 212, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 130, 213, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 143, 214, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[101], 155, 230, Direction.NorthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 172, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 166, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[102], 169, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[102], 175, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 178, 12, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 177, 103, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 180, 103, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 183, 103, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 186, 103, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 189, 103, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 186, 151, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 196, 33, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 193, 33, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 202, 93, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 205, 93, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 198, 130, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 202, 150, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 232, 46, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 232, 40, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 232, 37, Direction.SouthEast);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 227, 61, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 229, 93, Direction.SouthWest);
            yield return this.CreateMonsterSpawn(this.NpcDictionary[100], 232, 93, Direction.SouthWest);
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                    { Stats.FireResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 46 },
                    { Stats.MaximumHealth, 2500 },
                    { Stats.MinimumPhysBaseDmg, 145 },
                    { Stats.MaximumPhysBaseDmg, 150 },
                    { Stats.DefenseBase, 75 },
                    { Stats.AttackRatePvm, 230 },
                    { Stats.DefenseRatePvm, 61 },
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.PoisonDamageMultiplier, 0.03f },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lightning);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 44 },
                    { Stats.MaximumHealth, 2000 },
                    { Stats.MinimumPhysBaseDmg, 140 },
                    { Stats.MaximumPhysBaseDmg, 145 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 220 },
                    { Stats.DefenseRatePvm, 55 },
                    { Stats.WaterResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 3f / 255 },
                    { Stats.IceResistance, 3f / 255 },
                    { Stats.WaterResistance, 3f / 255 },
                    { Stats.FireResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 2f / 255 },
                    { Stats.IceResistance, 2f / 255 },
                    { Stats.WaterResistance, 2f / 255 },
                    { Stats.FireResistance, 2f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Poison);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 25 },
                    { Stats.MaximumHealth, 750 },
                    { Stats.MinimumPhysBaseDmg, 90 },
                    { Stats.MaximumPhysBaseDmg, 95 },
                    { Stats.DefenseBase, 31 },
                    { Stats.AttackRatePvm, 125 },
                    { Stats.DefenseRatePvm, 31 },
                    { Stats.PoisonDamageMultiplier, 0.03f },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PowerWave);
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 40 },
                    { Stats.MaximumHealth, 1600 },
                    { Stats.MinimumPhysBaseDmg, 130 },
                    { Stats.MaximumPhysBaseDmg, 135 },
                    { Stats.DefenseBase, 60 },
                    { Stats.AttackRatePvm, 200 },
                    { Stats.DefenseRatePvm, 47 },
                    { Stats.PoisonResistance, 3f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 2f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
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
                    { Stats.PoisonResistance, 6f / 255 },
                    { Stats.IceResistance, 6f / 255 },
                    { Stats.WaterResistance, 6f / 255 },
                    { Stats.FireResistance, 6f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var trap = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(trap);
                trap.Number = 100;
                trap.Designation = "Lance Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 4;
                trap.ViewRange = 4;
                trap.ObjectKind = NpcObjectKind.Trap;
                trap.IntelligenceTypeName = typeof(AttackSingleWhenPressedTrapIntelligence).FullName;
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

                trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var trap = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(trap);
                trap.Number = 101;
                trap.Designation = "Iron Stick Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 0;
                trap.ObjectKind = NpcObjectKind.Trap;
                trap.IntelligenceTypeName = typeof(AttackSingleWhenPressedTrapIntelligence).FullName;
                trap.ViewRange = 1;
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

                trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var trap = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(trap);
                trap.Number = 102;
                trap.Designation = "Fire Trap";
                trap.MoveRange = 0;
                trap.AttackRange = 2;
                trap.ObjectKind = NpcObjectKind.Trap;
                trap.IntelligenceTypeName = typeof(AttackAreaTargetInDirectionTrapIntelligence).FullName;
                trap.ViewRange = 1;
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

                trap.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
