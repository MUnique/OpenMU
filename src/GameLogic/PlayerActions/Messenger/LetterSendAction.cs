// <copyright file="LetterSendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger
{
    using System;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to send a letter to another player.
    /// </summary>
    public class LetterSendAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LetterSendAction));

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
        /// <param name="letterId">The client side letter id.</param>
        public void SendLetter(Player player, string receiver, string message, string title, byte rotation, byte animation, uint letterId)
        {
            var sendPrice = this.gameContext.Configuration.LetterSendPrice;
            if (player.Money < sendPrice)
            {
                player.PlayerView.ShowMessage("Not enough Zen to send a letter.", MessageType.BlueNormal);
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.NotEnoughMoney, letterId);
                return;
            }

            LetterHeader letter = null;
            try
            {
                using (var context = this.gameContext.PersistenceContextProvider.CreateNewPlayerContext(this.gameContext.Configuration))
                {
                    letter = this.CreateLetter(context, player, receiver, message, title, rotation, animation);
                    if (!context.CanSaveLetter(letter))
                    {
                        player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.ReceiverNotExists, letterId);
                        return;
                    }

                    context.SaveChanges();
                }

                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.Success, letterId);
                player.TryAddMoney(-sendPrice);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error when trying to send a letter", ex);
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
                player.PlayerView.ShowMessage("Oops, some error happened during sending the Letter.", MessageType.BlueNormal);
            }

            // Try to forward it to the player, if he is online
            var receiverPlayer = this.gameContext.GetPlayerByCharacterName(receiver);
            if (receiverPlayer != null)
            {
                receiverPlayer.PersistenceContext.Attach(letter);
                receiverPlayer.SelectedCharacter.Letters.Add(letter);
                receiverPlayer.PlayerView.MessengerView.AddToLetterList(letter, (ushort)(receiverPlayer.SelectedCharacter.Letters.Count - 1), true);
            }
            else
            {
                this.friendServer.ForwardLetter(letter);
            }
        }

        private LetterHeader CreateLetter(IContext context, Player player, string receiver, string message, string title, byte rotation, byte animation)
        {
            var letterHeader = context.CreateNew<LetterHeader>();
            letterHeader.LetterDate = DateTime.Now;
            letterHeader.SenderName = player.SelectedCharacter.Name;
            letterHeader.ReceiverName = receiver;
            letterHeader.Subject = title;

            var letterBody = context.CreateNew<LetterBody>();
            letterBody.Header = letterHeader;
            letterBody.Message = message;
            letterBody.SenderAppearance = context.CreateNew<AppearanceData>();
            letterBody.SenderAppearance.CharacterClass = player.AppearanceData.CharacterClass;
            player.AppearanceData.EquippedItems.Select(i => i.MakePersistent(context)).ForEach(letterBody.SenderAppearance.EquippedItems.Add);
            letterBody.Rotation = rotation;
            letterBody.Animation = animation;
            return letterHeader;
        }
    }
}
