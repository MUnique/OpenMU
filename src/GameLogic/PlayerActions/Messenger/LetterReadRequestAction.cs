// <copyright file="LetterReadRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

using MUnique.OpenMU.GameLogic.Views.Messenger;

/// <summary>
/// Action to read a letter.
/// </summary>
public class LetterReadRequestAction
{
    /// <summary>
    /// Requests the letter which should be shown to the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="letterIndex">Index of the letter.</param>
    public async ValueTask ReadRequestAsync(Player player, ushort letterIndex)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.SelectedCharacter?.Letters.Count < letterIndex)
        {
            player.Logger.LogWarning("Player {0} requested non-existing letter, id {1}", player.SelectedCharacter.Name, letterIndex);
            return;
        }

        var letter = player.SelectedCharacter?.Letters[letterIndex];
        if (letter != null)
        {
            var letterBody = await player.PersistenceContext.GetLetterBodyByHeaderIdAsync(letter.Id).ConfigureAwait(false);
            if (letterBody is not null)
            {
                letter.ReadFlag = true;
                await player.InvokeViewPlugInAsync<IShowLetterPlugIn>(p => p.ShowLetterAsync(letterBody)).ConfigureAwait(false);
            }
        }
        else
        {
            player.Logger.LogWarning("Letter not found. Id={0} Player={1}", letterIndex, player.SelectedCharacter?.Name);
        }
    }
}