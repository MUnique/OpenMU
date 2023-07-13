// <copyright file="ChatMessagePartyProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// A chat message processor which sends the message to the party.
/// </summary>
public class ChatMessagePartyProcessor : BannableChatMessageBaseProcessor
{
    /// <inheritdoc />
    public override async ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var eventArgs = new CancelEventArgs();
        sender.GameContext.PlugInManager.GetPlugInPoint<IChatMessageReceivedPlugIn>()?.ChatMessageReceived(sender, content.Message, eventArgs);
        if (eventArgs.Cancel)
        {
            return;
        }

        sender.Party?.SendChatMessageAsync(content.Message, sender.SelectedCharacter!.Name);
    }
}