// <copyright file="ChatMessageAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Chat;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Action to send chat messages.
/// </summary>
public class ChatMessageAction
{
    private readonly IDictionary<string, ChatMessageType> _messagePrefixes;
    private readonly IDictionary<ChatMessageType, IChatMessageProcessor> _chatProcessMessages;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageAction"/> class.
    /// </summary>
    public ChatMessageAction()
    {
        this._messagePrefixes = new SortedDictionary<string, ChatMessageType>(new ReverseComparer())
        {
            { "~", ChatMessageType.Party },
            { "@", ChatMessageType.Guild },
            { "@@", ChatMessageType.Alliance },
            { "$", ChatMessageType.Gens },
            { "!", ChatMessageType.GlobalNotification },
            { "/", ChatMessageType.Command },
        };

        this._chatProcessMessages = new Dictionary<ChatMessageType, IChatMessageProcessor>
        {
            { ChatMessageType.Command, new ChatMessageCommandProcessor() },
            { ChatMessageType.Whisper, new ChatMessageWhisperProcessor() },
            { ChatMessageType.Party, new ChatMessagePartyProcessor() },
            { ChatMessageType.Alliance, new ChatMessageAllianceProcessor() },
            { ChatMessageType.Guild, new ChatMessageGuildProcessor() },
            { ChatMessageType.GlobalNotification, new ChatMessageGlobalNotificationProcessor() },
            { ChatMessageType.Normal, new ChatMessageNormalProcessor() },
        };
    }

    /// <summary>
    /// Sends a chat message from the player to other players.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="playerName">Name of the <paramref name="sender"/>, except for <see cref="ChatMessageType.Whisper"/>, then its the receiver name.</param>
    /// <param name="message">The message which should be sent.</param>
    /// <param name="whisper">If set to <c>true</c> the message is whispered to the player with the <paramref name="playerName"/>; Otherwise, it's not a whisper.</param>
    public async ValueTask ChatMessageAsync(Player sender, string playerName, string message, bool whisper)
    {
        using var loggerScope = sender.Logger.BeginScope(this.GetType());
        ChatMessageType messageType = this.GetMessageType(message, whisper);

        if (sender.SelectedCharacter is null)
        {
            // Is possible to receive null?
            return;
        }

        if (messageType != ChatMessageType.Whisper && playerName != sender.SelectedCharacter?.Name)
        {
            sender.Logger.LogWarning("Maybe Hacker, Charname in chat packet != charname\t [{0}] <> [{1}]", sender.SelectedCharacter?.Name, playerName);
        }

        if (!this._chatProcessMessages.ContainsKey(messageType))
        {
            sender.Logger.LogDebug("Not implemented chat message type: {0}", messageType);
            return;
        }

        await this._chatProcessMessages[messageType].ProcessMessageAsync(sender, (message, playerName)).ConfigureAwait(true);
    }

    private ChatMessageType GetMessageType(string message, bool whisper)
    {
        if (whisper)
        {
            return ChatMessageType.Whisper;
        }

        // byte 13: begin message
        foreach (var keyValuePair in this._messagePrefixes)
        {
            if (message.StartsWith(keyValuePair.Key, StringComparison.InvariantCulture))
            {
                return keyValuePair.Value;
            }
        }

        return ChatMessageType.Normal;
    }

    /// <summary>
    /// We have to implement a reverse comparer, so that the strings which are longer, come first.
    /// </summary>
    private class ReverseComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return string.Compare(y, x, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}