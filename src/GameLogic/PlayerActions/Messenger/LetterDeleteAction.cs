// <copyright file="LetterDeleteAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to delete a letter.
    /// </summary>
    public class LetterDeleteAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LetterDeleteAction));

        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterDeleteAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public LetterDeleteAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

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
            player.PlayerView.MessengerView.LetterDeleted((ushort)letterIndex);

            // TODO: Deleting it from the repository should not be required.
            using (this.gameContext.RepositoryManager.UseContext(player.PersistenceContext))
            {
                if (!this.gameContext.RepositoryManager.GetRepository<LetterHeader>().Delete(letter))
                {
                    Log.WarnFormat("Player {0} tried to delete a letter, no success. LetterID: {1}", player.SelectedCharacter.Name, letterIndex);
                }
            }
        }
    }
}
