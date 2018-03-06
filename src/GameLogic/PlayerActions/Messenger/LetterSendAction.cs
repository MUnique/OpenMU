// <copyright file="LetterSendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using System;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to send a letter to another player.
    /// </summary>
    public class LetterSendAction
    {
        /// <summary>
        /// The price of sending a letter. TODO: Letter price should be configurable.
        /// </summary>
        private const int LetterSendCost = 1000;

        private readonly IGameContext gameContext;

        private readonly IFriendServer friendServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterSendAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <param name="friendServer">The friend server.</param>
        /// <exception cref="System.ArgumentNullException">appearanceSerializer</exception>
        public LetterSendAction(IGameContext gameContext, IFriendServer friendServer)
        {
            this.gameContext = gameContext;
            this.friendServer = friendServer;
        }

        /// <summary>
        /// Sends the letter.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="animation">The animation.</param>
        public void SendLetter(Player player, string receiver, string message, string title, byte rotation, byte animation)
        {
            if (player.Money < LetterSendCost)
            {
                player.PlayerView.ShowMessage("Not enough Zen to send a letter.", MessageType.BlueNormal);
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.TryAgain);
                return;
            }

            if (player.Money >= LetterSendCost)
            {
                player.TryAddMoney(-LetterSendCost); // Checked before if enough money is there
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.Success);
            }
            else
            {
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.TryAgain);
                player.PlayerView.ShowMessage("Oops, some error happened during sending the Letter.", MessageType.BlueNormal);
            }

            // Try to forward it to the player, if he is online
            var receiverPlayer = this.gameContext.GetPlayerByCharacterName(receiver);
            if (receiverPlayer != null)
            {
                using (this.gameContext.RepositoryManager.UseContext(receiverPlayer.PersistenceContext))
                {
                    var letter = this.CreateLetter(player, receiver, message, title, rotation, animation);
                    var newLetterIndex = receiverPlayer.SelectedCharacter.Letters.Count;
                    receiverPlayer.SelectedCharacter.Letters.Add(letter);
                    receiverPlayer.PlayerView.MessengerView.AddToLetterList(letter, (ushort)newLetterIndex, true);
                }
            }
            else
            {
                using (var context = this.gameContext.RepositoryManager.UseTemporaryContext())
                {
                    var letter = this.CreateLetter(player, receiver, message, title, rotation, animation);

                    if (!context.SaveChanges())
                    {
                        player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.ReceiverNotExists);
                    }
                    else
                    {
                        this.friendServer.ForwardLetter(letter);
                    }
                }
            }
        }

        private LetterHeader CreateLetter(Player player, string receiver, string message, string title, byte rotation, byte animation)
        {
            var letterHeader = this.gameContext.RepositoryManager.CreateNew<LetterHeader>();
            letterHeader.LetterDate = DateTime.Now;
            letterHeader.Sender = player.SelectedCharacter.Name;
            letterHeader.Receiver = receiver;
            letterHeader.Subject = title;

            var letterBody = this.gameContext.RepositoryManager.CreateNew<LetterBody>();
            letterBody.Header = letterHeader;
            letterBody.Message = message;
            letterBody.SenderAppearance = this.gameContext.RepositoryManager.CreateNew<AppearanceData>();
            letterBody.SenderAppearance.CharacterClass = player.AppearanceData.CharacterClass;
            player.AppearanceData.EquippedItems.Select(i => i.MakePersistent(this.gameContext))
                .ForEach(letterBody.SenderAppearance.EquippedItems.Add);
            letterBody.Rotation = rotation;
            letterBody.Animation = animation;
            return letterHeader;
        }
    }
}
