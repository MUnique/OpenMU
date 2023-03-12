// <copyright file="ChatNormalProcessorMessage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// A chat processor message for normal chat.
/// </summary>
public class ChatNormalProcessorMessage : IChatProcessorMessage
{
    /// <inheritdoc />
    public async ValueTask ConsumeAsync(Player sender, (string Message, string PlayerName) content)
    {
        sender.Logger.LogDebug("Sending Chat Message to Observers, Count: {0}", sender.Observers.Count);
        await sender.ForEachWorldObserverAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(content.Message, sender.SelectedCharacter!.Name, ChatMessageType.Normal), true).ConfigureAwait(false);
    }
}