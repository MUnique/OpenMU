// <copyright file="ChatMessageGlobalNotificationProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// A chat message processor which sends a global notification.
/// </summary>
public class ChatMessageGlobalNotificationProcessor : IChatMessageProcessor
{
    /// <inheritdoc />
    public async ValueTask ProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var eventArgs = new CancelEventArgs();
        sender.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()
            ?.ChatMessageReceived(sender, content.Message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        if (sender.SelectedCharacter!.CharacterStatus < CharacterStatus.GameMaster)
        {
            return;
        }

        await sender.GameContext.SendGlobalNotificationAsync(content.Message).ConfigureAwait(false);
    }
}