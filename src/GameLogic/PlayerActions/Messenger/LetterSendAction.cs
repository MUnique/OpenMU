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
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to send a letter to another player.
    /// </summary>
    public class LetterSendAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LetterSendAction));

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
            var sendPrice = player.GameContext.Configuration.LetterSendPrice;
            if (player.Money < sendPrice)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Not enough Zen to send a letter.", MessageType.BlueNormal);
                player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.NotEnoughMoney, letterId);
                return;
            }

            LetterHeader letter = null;
            try
            {
                using (var context = player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration))
                {
                    letter = this.CreateLetter(context, player, receiver, message, title, rotation, animation);
                    if (!context.CanSaveLetter(letter))
                    {
                        player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.ReceiverNotExists, letterId);
                        return;
                    }

                    context.SaveChanges();
                }

                player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.Success, letterId);
                player.TryAddMoney(-sendPrice);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error when trying to send a letter", ex);
                player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Oops, some error happened during sending the Letter.", MessageType.BlueNormal);
            }

            // Try to forward it to the player, if he is online
            var receiverPlayer = player.GameContext.GetPlayerByCharacterName(receiver);
            if (receiverPlayer != null)
            {
                receiverPlayer.PersistenceContext.Attach(letter);
                receiverPlayer.SelectedCharacter.Letters.Add(letter);
                receiverPlayer.ViewPlugIns.GetPlugIn<IAddToLetterListPlugIn>()?.AddToLetterList(letter, (ushort)(receiverPlayer.SelectedCharacter.Letters.Count - 1), true);
            }
            else
            {
                (player.GameContext as IGameServerContext)?.FriendServer?.ForwardLetter(letter);
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
