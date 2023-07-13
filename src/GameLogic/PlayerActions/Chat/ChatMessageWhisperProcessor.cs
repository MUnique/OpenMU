// <copyright file="ChatMessageWhisperProcessor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// A chat message processor which sends the message to the whisper receiver.
/// </summary>
public class ChatMessageWhisperProcessor : BannableChatMessageBaseProcessor
{
    /// <inheritdoc />
    public override async ValueTask SubclassProcessMessageAsync(Player sender, (string Message, string PlayerName) content)
    {
        var whisperReceiver = sender.GameContext.GetPlayerByCharacterName(content.PlayerName);
        if (whisperReceiver != null)
        {
            var eventArgs = new CancelEventArgs();
            sender.GameContext.PlugInManager.GetPlugInPoint<IWhisperMessageReceivedPlugIn>()?.WhisperMessageReceived(sender, whisperReceiver, content.Message, eventArgs);
            if (!eventArgs.Cancel)
            {
                await whisperReceiver.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(content.Message, sender.SelectedCharacter!.Name, ChatMessageType.Whisper)).ConfigureAwait(false);
            }
        }
    }
}