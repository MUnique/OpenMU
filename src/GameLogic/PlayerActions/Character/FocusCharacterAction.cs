// <copyright file="FocusCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Action to focus a character in the character selection screen.
/// </summary>
public class FocusCharacterAction
{
    /// <summary>
    /// Focuses the character.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="characterName">Name of the character.</param>
    public async ValueTask FocusCharacterAsync(Player player, string characterName)
    {
        if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
        {
            return;
        }

        var character = player.Account?.Characters.FirstOrDefault(c => c.Name == characterName);
        if (character is not null)
        {
            await player.InvokeViewPlugInAsync<ICharacterFocusedPlugIn>(p => p.CharacterFocusedAsync(character)).ConfigureAwait(false);
        }
    }
}