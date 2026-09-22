// <copyright file="BotClassNumbers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Single owner of mirrored character class numbers.
/// </summary>
/// <remarks>
/// Mirrors CharacterClassNumber. GameLogic has no reference
/// to the initialization assembly, so the values live here.
/// </remarks>
internal static class BotClassNumbers
{
    /// <summary>The Dark Wizard class number.</summary>
    internal const byte DarkWizardNumber = 0;

    /// <summary>The Soul Master class number.</summary>
    internal const byte SoulMasterNumber = 2;

    /// <summary>The Grand Master class number.</summary>
    internal const byte GrandMasterNumber = 3;

    /// <summary>The Dark Knight class number.</summary>
    internal const byte DarkKnightNumber = 4;

    /// <summary>The Blade Knight class number.</summary>
    internal const byte BladeKnightNumber = 6;

    /// <summary>The Blade Master class number.</summary>
    internal const byte BladeMasterNumber = 7;

    /// <summary>The Fairy Elf class number.</summary>
    internal const byte FairyElfNumber = 8;

    /// <summary>The Muse Elf class number.</summary>
    internal const byte MuseElfNumber = 10;

    /// <summary>The High Elf class number.</summary>
    internal const byte HighElfNumber = 11;

    /// <summary>The Magic Gladiator class number.</summary>
    internal const byte MagicGladiatorNumber = 12;

    /// <summary>The Duel Master class number.</summary>
    internal const byte DuelMasterNumber = 13;

    /// <summary>The Dark Lord class number.</summary>
    internal const byte DarkLordNumber = 16;

    /// <summary>The Lord Emperor class number.</summary>
    internal const byte LordEmperorNumber = 17;

    /// <summary>The Summoner class number.</summary>
    internal const byte SummonerNumber = 20;

    /// <summary>The Bloody Summoner class number.</summary>
    internal const byte BloodySummonerNumber = 22;

    /// <summary>The Dimension Master class number.</summary>
    internal const byte DimensionMasterNumber = 23;

    /// <summary>The Rage Fighter class number.</summary>
    internal const byte RageFighterNumber = 24;

    /// <summary>The Fist Master class number.</summary>
    internal const byte FistMasterNumber = 25;
}
