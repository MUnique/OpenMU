// <copyright file="LetterReadRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using MUnique.OpenMU.GameLogic.Views.Messenger;

    /// <summary>
    /// Action to read a letter.
    /// </summary>
    public class LetterReadRequestAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(LetterReadRequestAction));

        /// <summary>
        /// Requests the letter which should be shown to the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="letterIndex">Index of the letter.</param>
        public void ReadRequest(Player player, ushort letterIndex)
        {
            if (player.SelectedCharacter.Letters.Count < letterIndex)
            {
                Log.WarnFormat("Player {0} requested non-existing letter, id {1}", player.SelectedCharacter.Name, letterIndex);
                return;
            }

            var letter = player.SelectedCharacter.Letters[letterIndex];
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
                Log.WarnFormat("Letter not found. Id={0} Player={1}", letterIndex, player.SelectedCharacter.Name);
            }
        }
    }
}
