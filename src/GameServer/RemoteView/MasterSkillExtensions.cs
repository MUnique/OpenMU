// <copyright file="MasterSkillExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Extension methods regarding master skills.
    /// </summary>
    public static class MasterSkillExtensions
    {
        private const short GrandMasterNumber = 3;
        private const short BattleMaster = 7;
        private const short HighElf = 11;
        private const short DuelMaster = 13;
        private const short LordEmperor = 17;
        private const short DimensionMaster = 0xB8 >> 3;
        private const short FistMaster = 0xC8 >> 3;

        private static readonly IDictionary<short, IDictionary<short, byte>> SkillNumberToIndex = new SortedDictionary<short, IDictionary<short, byte>>
        {
            {
                GrandMasterNumber, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 334, 51 },
                    { 338, 55 },
                    { 347, 73 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 362, 92 },
                    { 378, 41 },
                    { 379, 42 },
                    { 380, 43 },
                    { 381, 45 },
                    { 382, 46 },
                    { 383, 47 },
                    { 384, 48 },
                    { 385, 49 },
                    { 386, 50 },
                    { 387, 52 },
                    { 388, 54 },
                    { 389, 56 },
                    { 397, 77 },
                    { 398, 78 },
                    { 399, 79 },
                    { 400, 81 },
                    { 401, 82 },
                    { 402, 83 },
                    { 403, 85 },
                    { 404, 89 },
                    { 405, 90 },
                }
            },
            {
                BattleMaster, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 326, 41 },
                    { 327, 42 },
                    { 328, 43 },
                    { 329, 44 },
                    { 330, 45 },
                    { 331, 46 },
                    { 332, 49 },
                    { 333, 50 },
                    { 334, 51 },
                    { 335, 52 },
                    { 336, 53 },
                    { 337, 54 },
                    { 338, 55 },
                    { 347, 73 },
                    { 348, 77 },
                    { 349, 78 },
                    { 350, 79 },
                    { 351, 80 },
                    { 352, 81 },
                    { 353, 82 },
                    { 354, 83 },
                    { 355, 84 },
                    { 356, 85 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 360, 89 },
                    { 361, 90 },
                    { 362, 92 },
                }
            },
            {
                HighElf, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 334, 52 },
                    { 338, 56 },
                    { 347, 73 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 362, 92 },
                    { 413, 41 },
                    { 414, 43 },
                    { 415, 44 },
                    { 416, 45 },
                    { 417, 46 },
                    { 418, 47 },
                    { 419, 48 },
                    { 420, 49 },
                    { 421, 51 },
                    { 422, 53 },
                    { 423, 54 },
                    { 424, 55 },
                    { 435, 77 },
                    { 436, 78 },
                    { 437, 79 },
                    { 438, 81 },
                    { 439, 82 },
                    { 440, 83 },
                    { 441, 89 },
                    { 442, 90 },
                }
            },
            {
                DuelMaster, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 334, 52 },
                    { 338, 56 },
                    { 347, 73 },
                    { 348, 77 },
                    { 349, 78 },
                    { 352, 81 },
                    { 353, 82 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 361, 90 },
                    { 362, 92 },
                    { 397, 79 },
                    { 398, 80 },
                    { 400, 83 },
                    { 401, 84 },
                    { 405, 89 },
                    { 479, 41 },
                    { 480, 42 },
                    { 481, 43 },
                    { 482, 44 },
                    { 483, 45 },
                    { 484, 46 },
                    { 485, 48 },
                    { 486, 49 },
                    { 487, 50 },
                    { 488, 51 },
                    { 489, 53 },
                    { 490, 54 },
                }
            },
            {
                LordEmperor, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 334, 52 },
                    { 338, 56 },
                    { 347, 73 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 361, 91 },
                    { 362, 92 },
                    { 508, 41 },
                    { 509, 42 },
                    { 510, 43 },
                    { 511, 46 },
                    { 512, 47 },
                    { 513, 48 },
                    { 514, 49 },
                    { 515, 50 },
                    { 516, 51 },
                    { 517, 54 },
                    { 518, 55 },
                    { 526, 77 },
                    { 527, 78 },
                    { 528, 79 },
                    { 529, 80 },
                    { 530, 81 },
                    { 531, 82 },
                    { 532, 83 },
                    { 533, 84 },
                    { 534, 89 },
                    { 535, 90 },
                }
            },
            {
                DimensionMaster, new SortedDictionary<short, byte>
                {
                    { 300, 1 },
                    { 301, 4 },
                    { 302, 6 },
                    { 303, 7 },
                    { 304, 8 },
                    { 305, 9 },
                    { 306, 10 },
                    { 307, 11 },
                    { 308, 12 },
                    { 309, 14 },
                    { 310, 15 },
                    { 311, 16 },
                    { 312, 17 },
                    { 313, 18 },
                    { 325, 37 },
                    { 334, 52 },
                    { 338, 56 },
                    { 347, 73 },
                    { 357, 86 },
                    { 358, 87 },
                    { 359, 88 },
                    { 362, 92 },
                    { 448, 41 },
                    { 449, 42 },
                    { 450, 43 },
                    { 451, 45 },
                    { 452, 46 },
                    { 453, 47 },
                    { 454, 48 },
                    { 455, 49 },
                    { 456, 50 },
                    { 457, 53 },
                    { 458, 55 },
                    { 465, 77 },
                    { 466, 78 },
                    { 467, 81 },
                    { 468, 82 },
                    { 469, 85 },
                    { 470, 89 },
                    { 471, 90 },
                }
            },
            {
                FistMaster, new SortedDictionary<short, byte>
                {
                    { 325, 37 },
                    { 578, 1 },
                    { 579, 4 },
                    { 580, 6 },
                    { 581, 7 },
                    { 582, 8 },
                    { 583, 9 },
                    { 584, 10 },
                    { 585, 11 },
                    { 586, 12 },
                    { 587, 14 },
                    { 588, 15 },
                    { 589, 16 },
                    { 590, 17 },
                    { 591, 18 },
                    { 599, 37 },
                    { 551, 41 },
                    { 552, 42 },
                    { 554, 45 },
                    { 555, 46 },
                    { 600, 51 },
                    { 557, 52 },
                    { 558, 53 },
                    { 559, 54 },
                    { 601, 55 },
                    { 560, 56 },
                    { 603, 73 },
                    { 568, 77 },
                    { 569, 78 },
                    { 571, 81 },
                    { 572, 82 },
                    { 573, 85 },
                    { 604, 86 },
                    { 605, 87 },
                    { 606, 88 },
                    { 607, 90 },
                    { 608, 92 },
                }
            },
        };

        /// <summary>
        /// Gets the index of the master skill on the clients master skill tree for the given character class.
        /// </summary>
        /// <param name="skill">The skill.</param>
        /// <param name="characterClass">The character class.</param>
        /// <returns>The index of the master skill on the clients master skill tree for the given character class.</returns>
        /// <remarks>
        /// Each possible visible slot of a master skill in the master skill tree is identified
        /// by an index. Each root has 36 slots (4 * 9), and roots are indexed one after another,
        /// that means left root starts at index 1, middle root at 37, right root at 73.
        /// As this display can differ between character classes, we have to pass it in here.
        /// To me, it's a mystery why Webzen can't work with the skill number alone and determine
        /// this index on client side.
        /// I decided to put this indexes into code since it would pollute the data model too much.
        /// </remarks>
        public static byte GetMasterSkillIndex(this Skill skill, CharacterClass characterClass)
        {
            if (skill.MasterDefinition == null)
            {
                return 0;
            }

            if (SkillNumberToIndex.TryGetValue(characterClass.Number, out var skillToIndexOfClass)
                && skillToIndexOfClass.TryGetValue(skill.Number, out var index))
            {
                return index;
            }

            return 0;
        }
    }
}
