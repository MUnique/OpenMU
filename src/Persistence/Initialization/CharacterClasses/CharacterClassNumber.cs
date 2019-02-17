// <copyright file="CharacterClassNumber.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    /// <summary>
    /// The default character class numbers.
    /// </summary>
    public enum CharacterClassNumber : byte
    {
        /// <summary>
        /// The dark wizard
        /// </summary>
        DarkWizard = 0,

        /// <summary>
        /// The soul master
        /// </summary>
        SoulMaster = 0x10 >> 3,

        /// <summary>
        /// The grand master
        /// </summary>
        GrandMaster = 0x18 >> 3,

        /// <summary>
        /// The dark knight
        /// </summary>
        DarkKnight = 0x20 >> 3,

        /// <summary>
        /// The blade knight
        /// </summary>
        BladeKnight = 0x30 >> 3,

        /// <summary>
        /// The blade master
        /// </summary>
        BladeMaster = 0x38 >> 3,

        /// <summary>
        /// The fairy elf
        /// </summary>
        FairyElf = 0x40 >> 3,

        /// <summary>
        /// The muse elf
        /// </summary>
        MuseElf = 0x50 >> 3,

        /// <summary>
        /// The high elf
        /// </summary>
        HighElf = 0x58 >> 3,

        /// <summary>
        /// The magic gladiator
        /// </summary>
        MagicGladiator = 0x60 >> 3,

        /// <summary>
        /// The duel master
        /// </summary>
        DuelMaster = 0x68 >> 3,

        /// <summary>
        /// The dark lord
        /// </summary>
        DarkLord = 0x80 >> 3,

        /// <summary>
        /// The lord emperor
        /// </summary>
        LordEmperor = 0x88 >> 3,

        /// <summary>
        /// The summoner
        /// </summary>
        Summoner = 0xA0 >> 3,

        /// <summary>
        /// The bloody summoner
        /// </summary>
        BloodySummoner = 0xB0 >> 3,

        /// <summary>
        /// The dimension master
        /// </summary>
        DimensionMaster = 0xB8 >> 3,

        /// <summary>
        /// The rage fighter.
        /// </summary>
        RageFighter = 0xC0 >> 3,

        /// <summary>
        /// The fist master.
        /// </summary>
        FistMaster = 0xC8 >> 3,
    }
}