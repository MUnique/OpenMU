// <copyright file="IGameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// The state of the server.
/// </summary>
public enum ServerState
{
    /// <summary>
    /// The server has finished stopping.
    /// </summary>
    Stopped,

    /// <summary>
    /// The server is currently starting, but has not yet finished initialization.
    /// </summary>
    Starting,

    /// <summary>
    /// The server started and is available.
    /// </summary>
    Started,

    /// <summary>
    /// The server is not available anymore and is stopping it's services.
    /// </summary>
    Stopping,

    /// <summary>
    /// The server state is unknown, because of a timeout.
    /// </summary>
    Timeout,
}

/// <summary>
/// Types of messages.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// The message is shown as centered golden message in the client.
    /// </summary>
    GoldenCenter = 0,

    /// <summary>
    /// The message is shown as blue entry.
    /// </summary>
    BlueNormal = 1,

    /// <summary>
    /// The message is a guild notice (green center).
    /// </summary>
    GuildNotice = 2,
}

/// <summary>
/// Interface for the inter-server communication.
/// </summary>
public interface IGameServer : IManageableServer, IFriendSystemSubscriber
{
    /// <summary>
    /// Sends a chat message to all connected guild members.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="sender">The sender character name.</param>
    /// <param name="message">The message which should be sent.</param>
    ValueTask GuildChatMessageAsync(uint guildId, string sender, string message);

    /// <summary>
    /// Notifies the game server that a guild got deleted.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    ValueTask GuildDeletedAsync(uint guildId);

    /// <summary>
    /// Notifies the game server that a guild member got removed from a guild.
    /// </summary>
    /// <param name="playerName">Name of the player which got removed from a guild.</param>
    ValueTask GuildPlayerKickedAsync(string playerName);

    /// <summary>
    /// Sends a chat message to all connected alliance members.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="sender">The sender character name.</param>
    /// <param name="message">The message.</param>
    ValueTask AllianceChatMessageAsync(uint guildId, string sender, string message);

    /// <summary>
    /// Sends a global message to all connected players with the specified message type.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    ValueTask SendGlobalMessageAsync(string message, MessageType messageType);

    /// <summary>
    /// Disconnects the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    ValueTask<bool> DisconnectPlayerAsync(string playerName);

    /// <summary>
    /// Disconnects the account from the game.
    /// </summary>
    /// <param name="accountName">Name of the account.</param>
    /// <returns>True, if the account has been disconnected; False, otherwise.</returns>
    ValueTask<bool> DisconnectAccountAsync(string accountName);

    /// <summary>
    /// Bans the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been banned; False, otherwise.</returns>
    ValueTask<bool> BanPlayerAsync(string playerName);

    /// <summary>
    /// Assigns the guild to the player.
    /// </summary>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="guildStatus">The guild status of the character.</param>
    ValueTask AssignGuildToPlayerAsync(string characterName, GuildMemberStatus guildStatus);
}
