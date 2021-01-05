// <copyright file="LetterDeleteAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to delete a letter.
    /// </summary>
    public class LetterDeleteAction
    {
        /// <summary>
        /// Deletes the letter.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="letter">The letter.</param>
        public void DeleteLetter(Player player, LetterHeader letter)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            if (player.SelectedCharacter is not { } character)
            {
                player.Logger.LogWarning("no character selected, player {0}", player);
                return;
            }

            var letterIndex = player.SelectedCharacter.Letters.IndexOf(letter);
            player.SelectedCharacter.Letters.RemoveAt(letterIndex);
            player.ViewPlugIns.GetPlugIn<ILetterDeletedPlugIn>()?.LetterDeleted((ushort)letterIndex);
        }
    }
}
