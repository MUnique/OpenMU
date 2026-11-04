// <copyright file="BotBuild.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Single place for bot build variants.
/// </summary>
/// <remarks>
/// Variant 1 of the elf is the energy buffer.
/// </remarks>
internal static class BotBuild
{
    private const int SupportElfVariant = 1;

    /// <summary>
    /// Gets the stable build variant for a character name.
    /// </summary>
    /// <param name="characterName">The character name.</param>
    internal static int GetVariant(string characterName)
    {
        return characterName.Aggregate(0, (acc, c) => acc + c) % 2;
    }

    /// <summary>
    /// Gets the variant a generated character name should hit.
    /// </summary>
    /// <param name="wantSupport">Whether the buffer build is wanted.</param>
    internal static int WantedVariant(bool wantSupport)
    {
        return wantSupport ? SupportElfVariant : 0;
    }

    /// <summary>
    /// Checks for an elf class in any generation.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    internal static bool IsElf(CharacterClass characterClass)
    {
        return characterClass.Number is BotClassNumbers.FairyElfNumber or BotClassNumbers.MuseElfNumber or BotClassNumbers.HighElfNumber;
    }

    /// <summary>
    /// Checks for the energy elf buffer build.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="characterName">The character name.</param>
    internal static bool IsSupportElf(CharacterClass? characterClass, string? characterName)
    {
        return characterClass is not null
            && characterName is not null
            && IsElf(characterClass)
            && GetVariant(characterName) == SupportElfVariant;
    }

    /// <summary>
    /// Checks for the energy elf buffer build of a player.
    /// </summary>
    /// <param name="player">The player.</param>
    internal static bool IsSupportElf(Player player)
    {
        return IsSupportElf(player.SelectedCharacter?.CharacterClass, player.Name);
    }

    /// <summary>
    /// Gets the build key of a bot. One entry per party at most.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="characterName">The character name.</param>
    internal static (byte Line, int Slot) GetBuildKey(CharacterClass? characterClass, string? characterName)
    {
        if (characterClass is null || characterName is null)
        {
            return (byte.MaxValue, 0);
        }

        var line = characterClass.Number switch
        {
            BotClassNumbers.DarkKnightNumber or BotClassNumbers.BladeKnightNumber or BotClassNumbers.BladeMasterNumber => BotClassNumbers.DarkKnightNumber,
            BotClassNumbers.FairyElfNumber or BotClassNumbers.MuseElfNumber or BotClassNumbers.HighElfNumber => BotClassNumbers.FairyElfNumber,
            BotClassNumbers.MagicGladiatorNumber or BotClassNumbers.DuelMasterNumber => BotClassNumbers.MagicGladiatorNumber,
            _ => characterClass.Number,
        };

        var slot = IsTwoBuildLine(line) ? GetVariant(characterName) : 0;

        return (line, slot);
    }

    /// <summary>
    /// Gets the build key of a player.
    /// </summary>
    /// <param name="player">The player.</param>
    internal static (byte Line, int Slot) GetBuildKey(Player player)
    {
        return GetBuildKey(player.SelectedCharacter?.CharacterClass, player.Name);
    }

    private static bool IsTwoBuildLine(byte line)
    {
        return line is BotClassNumbers.DarkKnightNumber or BotClassNumbers.FairyElfNumber or BotClassNumbers.MagicGladiatorNumber;
    }
}
