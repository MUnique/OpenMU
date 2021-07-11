// <copyright file="CharacterClasses.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.CharacterClasses
{
    using System;

    /// <summary>
    /// Defines the character classes as flags enum.
    /// </summary>
    [Flags]
    public enum CharacterClasses : long
    {
        /// <summary>
        /// None of the classes.
        /// </summary>
        None = 0,

        /// <summary>
        /// All of the classes.
        /// </summary>
        All = 0xFFFFFFFF,

        /// <summary>
        /// The dark wizard
        /// </summary>
        DarkWizard = 0b0000_0001,

        /// <summary>
        /// The soul master
        /// </summary>
        SoulMaster = 0b0000_0010,

        /// <summary>
        /// The grand master
        /// </summary>
        GrandMaster = 0b0000_0100,

        /// <summary>
        /// The dark knight
        /// </summary>
        DarkKnight = 0b0001_0000,

        /// <summary>
        /// The blade knight
        /// </summary>
        BladeKnight = 0b0010_0000,

        /// <summary>
        /// The blade master
        /// </summary>
        BladeMaster = 0b0100_0000,

        /// <summary>
        /// The fairy elf
        /// </summary>
        FairyElf = 0b0001_000_0000,

        /// <summary>
        /// The muse elf
        /// </summary>
        MuseElf = 0b0010_0000_0000,

        /// <summary>
        /// The high elf
        /// </summary>
        HighElf = 0b0100_0000_0000,

        /// <summary>
        /// The magic gladiator
        /// </summary>
        MagicGladiator = 0b0001_0000_0000_0000,

        /// <summary>
        /// The duel master
        /// </summary>
        DuelMaster = 0b0010_0000_0000_0000,

        /// <summary>
        /// The dark lord
        /// </summary>
        DarkLord = 0b0001_0000_0000_0000_0000,

        /// <summary>
        /// The lord emperor
        /// </summary>
        LordEmperor = 0b0010_0000_0000_0000_0000,

        /// <summary>
        /// The summoner
        /// </summary>
        Summoner = 0b0001_0000_0000_0000_0000_0000,

        /// <summary>
        /// The bloody summoner
        /// </summary>
        BloodySummoner = 0b0010_0000_0000_0000_0000_0000,

        /// <summary>
        /// The dimension master
        /// </summary>
        DimensionMaster = 0b0100_0000_0000_0000_0000_0000,

        /// <summary>
        /// The rage fighter.
        /// </summary>
        RageFighter = 0b0001_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// The fist master.
        /// </summary>
        FistMaster = 0b0010_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// All wizards.
        /// </summary>
        AllWizards = DarkWizard | SoulMaster | GrandMaster,

        /// <summary>
        /// All knights.
        /// </summary>
        AllKnights = DarkKnight | BladeKnight | BladeMaster,

        /// <summary>
        /// All elfs.
        /// </summary>
        AllElfs = FairyElf | MuseElf | HighElf,

        /// <summary>
        /// All summoners.
        /// </summary>
        AllSummoners = Summoner | BloodySummoner | DimensionMaster,

        /// <summary>
        /// All magic gladiators.
        /// </summary>
        AllMGs = MagicGladiator | DuelMaster,

        /// <summary>
        /// All lords.
        /// </summary>
        AllLords = DarkLord | LordEmperor,

        /// <summary>
        /// All fighters.
        /// </summary>
        AllFighters = RageFighter | FistMaster,

        /// <summary>
        /// All magicians (wizards and magic gladiators).
        /// </summary>
        AllMagicians = AllWizards | AllMGs,

        /// <summary>
        /// All knights, lords and magic gladiators.
        /// </summary>
        AllKnightsLordsAndMGs = AllKnights | AllLords | AllMGs,

        /// <summary>
        /// The soul master and grand master.
        /// </summary>
        SoulMasterAndGrandMaster = SoulMaster | GrandMaster,

        /// <summary>
        /// The blade knight and blade master.
        /// </summary>
        BladeKnightAndBladeMaster = BladeKnight | BladeMaster,

        /// <summary>
        /// The muse elf and high elf.
        /// </summary>
        MuseElfAndHighElf = MuseElf | HighElf,

        /// <summary>
        /// The bloody summoner and dimension master.
        /// </summary>
        BloodySummonerAndDimensionMaster = BloodySummoner | DimensionMaster,

        /// <summary>
        /// All masters.
        /// </summary>
        AllMasters = GrandMaster | BladeMaster | HighElf | DimensionMaster | DuelMaster | LordEmperor | FistMaster,

        /// <summary>
        /// All masters except fist master.
        /// </summary>
        AllMastersExceptFistMaster = GrandMaster | BladeMaster | HighElf | DimensionMaster | DuelMaster | LordEmperor,

        /// <summary>
        /// All second classes.
        /// </summary>
        AllSecondClass = SoulMaster | BladeKnight | MuseElf | BloodySummoner | MagicGladiator | DarkLord | RageFighter,

        /// <summary>
        /// All masters and second classes.
        /// </summary>
        AllMastersAndSecondClass = AllMasters | AllSecondClass,
    }
}