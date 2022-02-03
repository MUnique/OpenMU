// <copyright file="LetterSendAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Action to send a letter to another player.
/// </summary>
public class LetterSendAction
{
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
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        var sendPrice = player.GameContext.Configuration.LetterSendPrice;
        if (player.Money < sendPrice)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Not enough Zen to send a letter.", MessageType.BlueNormal);
            player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.NotEnoughMoney, letterId);
            return;
        }

        if (player.SelectedCharacter is null)
        {
            player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
            return;
        }

        LetterHeader? letter;
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
            player.Logger.LogError(ex, "Unexpected error when trying to send a letter");
            player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Oops, some error happened during sending the Letter.", MessageType.BlueNormal);
            return;
        }

        // Try to forward it to the player, if he is online
        (player.GameContext as IGameServerContext)?.FriendServer.ForwardLetter(letter);
    }

    private LetterHeader CreateLetter(IContext context, Player player, string receiver, string message, string title, byte rotation, byte animation)
    {
        var letterHeader = context.CreateNew<LetterHeader>();
        letterHeader.LetterDate = DateTime.UtcNow;
        letterHeader.SenderName = player.SelectedCharacter!.Name;
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