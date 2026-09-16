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
    internal const byte DarkWizardNumber = 0;
    internal const byte SoulMasterNumber = 2;
    internal const byte GrandMasterNumber = 3;
    internal const byte DarkKnightNumber = 4;
    internal const byte BladeKnightNumber = 6;
    internal const byte BladeMasterNumber = 7;
    internal const byte FairyElfNumber = 8;
    internal const byte MuseElfNumber = 10;
    internal const byte HighElfNumber = 11;
    internal const byte MagicGladiatorNumber = 12;
    internal const byte DuelMasterNumber = 13;
    internal const byte DarkLordNumber = 16;
    internal const byte LordEmperorNumber = 17;
    internal const byte SummonerNumber = 20;
    internal const byte BloodySummonerNumber = 22;
    internal const byte DimensionMasterNumber = 23;
    internal const byte RageFighterNumber = 24;
    internal const byte FistMasterNumber = 25;
}
