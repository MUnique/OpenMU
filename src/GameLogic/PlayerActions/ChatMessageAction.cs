// <copyright file="ChatMessageAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions
{
    using System.Collections.Generic;
    using log4net;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to send chat messages.
    /// </summary>
    public class ChatMessageAction
    {
        private static ILog log = LogManager.GetLogger(typeof(ChatMessageAction));

        private readonly IGameContext gameContext;

        private readonly IDictionary<string, ChatMessageType> messagePrefixes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessageAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChatMessageAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
            this.messagePrefixes = new SortedDictionary<string, ChatMessageType>(new ReverseComparer())
            {
                { "~", ChatMessageType.Party },
                { "@", ChatMessageType.Guild },
                { "@@", ChatMessageType.Alliance },
                { "$", ChatMessageType.Gens },
                { "/", ChatMessageType.Command }
            };
        }

        /// <summary>
        /// Sends a chat message from the player to other players.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="playerName">Name of the <paramref name="sender"/>, except for <see cref="ChatMessageType.Whisper"/>, then its the receiver name.</param>
        /// <param name="message">The message which should be sent.</param>
        /// <param name="whisper">If set to <c>true</c> the message is whispered to the player with the <paramref name="playerName"/>; Otherwise, it's not a whisper.</param>
        public void ChatMessage(Player sender, string playerName, string message, bool whisper)
        {
            ChatMessageType messageType = this.GetMessageType(message, whisper);
            if (messageType != ChatMessageType.Whisper && playerName != sender.SelectedCharacter.Name)
            {
                log.WarnFormat("Maybe Hacker, Charname in chat packet != charname\t [{0}] <> [{1}]", sender.SelectedCharacter.Name, playerName);
            }

            if (messageType == ChatMessageType.Command)
            {
                this.ChatCommand(sender, message);
                return;
            }

            if (messageType == ChatMessageType.Whisper)
            {
                log.DebugFormat("Whisper Message Received From [{0}] to [{1}]:[{1}]", sender.SelectedCharacter.Name, playerName, message);
                var whisperReceiver = this.gameContext.GetPlayerByCharacterName(playerName);
                if (whisperReceiver != null)
                {
                    whisperReceiver.PlayerView.ChatMessage(message, sender.SelectedCharacter.Name, ChatMessageType.Whisper);
                }
            }
            else
            {
                log.DebugFormat("Chat Message Received: [{0}]:[{1}]", sender.SelectedCharacter.Name, message);
                if (messageType == ChatMessageType.Party)
                {
                    if (sender.Party != null)
                    {
                        sender.Party.SendChatMessage(message, sender.SelectedCharacter.Name);
                    }
                }
                else if (messageType == ChatMessageType.Alliance)
                {
                    // gameContext.GuildServer.AllianceMessage(Player.ShortGuildID, Player.SelectedCharacter.Name, message);
                }
                else if (messageType == ChatMessageType.Guild && sender.ShortGuildID != 0)
                {
                    var guildServer = (this.gameContext as IGameServerContext)?.GuildServer;
                    if (guildServer != null)
                    {
                        var guildId = sender.SelectedCharacter?.GuildMemberInfo?.GuildId;
                        if (guildId.HasValue)
                        {
                            guildServer.GuildMessage(guildId.Value, sender.SelectedCharacter.Name, message);
                        }
                    }
                }
                else
                {
                    log.DebugFormat("Sending Chat Message to Observers, Count: {0}", sender.Observers.Count);
                    sender.ForEachObservingPlayer(p => p.PlayerView.ChatMessage(message, sender.SelectedCharacter.Name, ChatMessageType.Normal), true);
                }
            }
        }

        private void ChatCommand(Player player, string message)
        {
            // TODO: implement plugin system to be able to add custom commands.
            string[] sa = message.Split(' ');
            switch (sa[0])
            {
                /* after Season 5.4 it works by a separate packet. look for WarpAction.
                case "/move":
                case "/warp":
                    ReadWarp(sa);
                    break;
                    */
                case "/teleport":
                    if (sa.Length > 2)
                    {
                        player.Move(byte.Parse(sa[1]), byte.Parse(sa[2]), MoveType.Instant);
                    }

                    break;
            }
        }

        private ChatMessageType GetMessageType(string message, bool whisper)
        {
            if (whisper)
            {
                return ChatMessageType.Whisper;
            }

            // byte 13: begin message
            foreach (var keyValuePair in this.messagePrefixes)
            {
                if (message.StartsWith(keyValuePair.Key))
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
            public int Compare(string x, string y)
            {
                return string.Compare(y, x);
            }
        }
    }
}
