// <copyright file="MiniGameDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions regarding the <see cref="MiniGameDefinition"/>s.
/// </summary>
public static class MiniGameDefinitionExtensions
{
    /// <summary>
    /// Gets the suitable mini game definition for the player and mini game type.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="miniGameType">Type of the mini game.</param>
    /// <param name="eventLevel">The event level.</param>
    /// <returns>The suitable mini game definition.</returns>
    public static MiniGameDefinition? GetSuitableMiniGameDefinition(this Player player, MiniGameType miniGameType, byte eventLevel)
    {
        var definitions = player.GameContext.Configuration.MiniGameDefinitions
            .Where(def => def.Type == miniGameType)
            .ToList();
        return definitions
                   .OrderByDescending(def => def.GameLevel)
                   .FirstOrDefault(def => def.IsInLevelRange(player))
               ?? definitions.FirstOrDefault(def => def.GameLevel == eventLevel);
    }

    /// <summary>
    /// Determines whether the specified player is in level range of the mini game.
    /// </summary>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="player">The player.</param>
    /// <returns><c>true</c> if the specified player is in level range; otherwise, <c>false</c>.</returns>
    public static bool IsInLevelRange(this MiniGameDefinition definition, Player player)
    {
        if (definition.RequiresMasterClass && player.SelectedCharacter?.CharacterClass?.IsMasterClass is not true)
        {
            return false;
        }

        var isSpecialCharacter = player.SelectedCharacter!.IsSpecialCharacter();
        if (isSpecialCharacter)
        {
            return player.Level >= definition.MinimumSpecialCharacterLevel
                   && player.Level <= definition.MaximumSpecialCharacterLevel;
        }

        return player.Level >= definition.MinimumCharacterLevel
               && player.Level <= definition.MaximumCharacterLevel;
    }
}