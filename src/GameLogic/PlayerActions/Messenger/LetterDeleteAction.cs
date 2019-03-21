// <copyright file="LetterDeleteAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using log4net;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to delete a letter.
    /// </summary>
    public class LetterDeleteAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LetterDeleteAction));

        /// <summary>
        /// Deletes the letter.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="letter">The letter.</param>
        public void DeleteLetter(Player player, LetterHeader letter)
        {
            if (letter == null)
            {
                Log.WarnFormat("letter is null, player {0}", player.SelectedCharacter.Name);
            }

            var letterIndex = player.SelectedCharacter.Letters.IndexOf(letter);
            player.SelectedCharacter.Letters.RemoveAt(letterIndex);
            player.ViewPlugIns.GetPlugIn<ILetterDeletedPlugIn>()?.LetterDeleted((ushort)letterIndex);
        }
    }
}
