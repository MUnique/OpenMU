// <copyright file="LetterReadRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using Microsoft.Extensions.Logging;
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
        public void ReadRequest(Player player, ushort letterIndex)
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
                var letterBody = player.PersistenceContext.GetLetterBodyByHeaderId(letter.Id);
                if (letterBody != null)
                {
                    letter.ReadFlag = true;
                    player.ViewPlugIns.GetPlugIn<IShowLetterPlugIn>()?.ShowLetter(letterBody);
                }
            }
            else
            {
                player.Logger.LogWarning("Letter not found. Id={0} Player={1}", letterIndex, player.SelectedCharacter?.Name);
            }
        }
    }
}
