// <copyright file="ChatViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Discord;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ChatViewPlugIn_Name), Description = nameof(PlugInResources.ChatViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F0B5BAD4-B97C-49F1-84E0-25EDC796B0E4")]
public class ChatViewPlugIn : IChatViewPlugIn
{
    /// <summary>
    /// The prefix of a chat message to the gens.
    /// </summary>
    private const string GensMessagePrefix = "$";

    /// <summary>
    /// The prefix of a chat message to the guild.
    /// </summary>
    private const string GuildMessagePrefix = "@";

    /// <summary>
    /// The prefix of a chat message to the alliance.
    /// </summary>
    private const string AllianceMessagePrefix = "@@";

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChatViewPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public async ValueTask ChatMessageAsync(string message, string sender, ChatMessageType type)
    {
        if (await this.TryShowAsExternalChatMessageAsync(message, sender, type).ConfigureAwait(false))
        {
            return;
        }

        if (type == ChatMessageType.Gens && message.StartsWith(GensMessagePrefix, StringComparison.Ordinal))
        {
            // The game client removes two characters of a message to the gens, instead of just the prefix.
            message = GensMessagePrefix + message;
        }

        await this._player.Connection.SendChatMessageAsync(ConvertChatMessageType(type), sender, message).ConfigureAwait(false);
    }

    private static ChatMessage.ChatMessageType ConvertChatMessageType(ChatMessageType type)
    {
        if (type == ChatMessageType.Whisper)
        {
            return Network.Packets.ServerToClient.ChatMessage.ChatMessageType.Whisper;
        }

        return Network.Packets.ServerToClient.ChatMessage.ChatMessageType.Normal;
    }

    /// <summary>
    /// Sends a guild or alliance message of a sender which was bridged from Discord as external chat message,
    /// when the client supports it. Then the client doesn't need to recognize the sender by its prefix.
    /// </summary>
    private async ValueTask<bool> TryShowAsExternalChatMessageAsync(string message, string sender, ChatMessageType type)
    {
        if (!BridgedChatSender.IsBridged(sender)
            || type is not (ChatMessageType.Guild or ChatMessageType.Alliance)
            || this._player.ViewPlugIns.GetPlugIn<IDiscordIntegrationViewPlugIn>() is not { } discordView)
        {
            return false;
        }

        var (scope, chatPrefix) = type == ChatMessageType.Guild
            ? (ExternalChatScope.Guild, GuildMessagePrefix)
            : (ExternalChatScope.Alliance, AllianceMessagePrefix);

        // The prefix of the chat says the same as the scope.
        var text = message.StartsWith(chatPrefix, StringComparison.Ordinal) ? message[chatPrefix.Length..] : message;
        return await discordView.TryShowExternalChatMessageAsync(scope, BridgedChatSender.GetName(sender), text).ConfigureAwait(false);
    }
}
