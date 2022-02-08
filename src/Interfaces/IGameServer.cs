// <copyright file="IGameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;

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

public interface IFriendSystemSubscriber
{
    /// <summary>
    /// Notifies the game server that a letter got received for an online player.
    /// </summary>
    /// <param name="letter">The letter header.</param>
    void LetterReceived(LetterHeader letter);
    
    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="requester">The requester.</param>
    /// <param name="receiver">The receiver.</param>
    void FriendRequest(string requester, string receiver);

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="player">The player who is playing on the server, and needs to get notified.</param>
    /// <param name="friend">The friend whose state changed.</param>
    /// <param name="serverId">The new server identifier of the <paramref name="friend"/>.</param>
    void FriendOnlineStateChanged(string player, string friend, int serverId);

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="playerAuthenticationInfo">Authentication information of the player who should get notified about the created chat room.</param>
    /// <param name="friendName">Name of the friend player which is expected to be in the chat room.</param>
    void ChatRoomCreated(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName);

    void InitializeMessenger(MessengerInitializationData initializationData);
}

public record MessengerInitializationData(string PlayerName, IImmutableList<string> Friends, IImmutableList<string> OpenFriendRequests);


/// <summary>
/// Interface for the inter-server communication.
/// </summary>
public interface IGameServer : IManageableServer, IFriendSystemSubscriber
{
    /// <summary>
    /// Gets the server information.
    /// </summary>
    ////IGameServerInfo ServerInfo { get; }

    /// <summary>
    /// Sends a chat message to all connected guild members.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="sender">The sender character name.</param>
    /// <param name="message">The message which should be sent.</param>
    void GuildChatMessage(uint guildId, string sender, string message);

    /// <summary>
    /// Notifies the game server that a guild got deleted.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    void GuildDeleted(uint guildId);

    /// <summary>
    /// Notifies the game server that a guild member got removed from a guild.
    /// </summary>
    /// <param name="playerName">Name of the player which got removed from a guild.</param>
    void GuildPlayerKicked(string playerName);

    /// <summary>
    /// Sends a chat message to all connected alliance members.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="sender">The sender character name.</param>
    /// <param name="message">The message.</param>
    void AllianceChatMessage(uint guildId, string sender, string message);



    ///// <summary>
    ///// Determines whether the player with the specified name is online.
    ///// </summary>
    ///// <param name="playerName">Name of the character.</param>
    ///// <returns>True, if online; False, otherwise.</returns>
    //bool IsPlayerOnline(string playerName);

    /// <summary>
    /// Sends a global message to all connected players with the specified message type.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageType">Type of the message.</param>
    void SendGlobalMessage(string message, MessageType messageType);



    /// <summary>
    /// Disconnects the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    bool DisconnectPlayer(string playerName);

    /// <summary>
    /// Bans the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been banned; False, otherwise.</returns>
    bool BanPlayer(string playerName);

    void AssignGuildToPlayer(string characterName, GuildMemberStatus guildStatus);
}

/////// <summary>
/////// Informations about a game server.
/////// </summary>
////public interface IGameServerInfo : INotifyPropertyChanged
////{


////    /// <summary>
////    /// Gets the description.
////    /// </summary>
////    string Description { get; }

////    /// <summary>
////    /// Gets the state.
////    /// </summary>
////    ServerState State { get; }

////    /// <summary>
////    /// Gets the online player count.
////    /// </summary>
////    int OnlinePlayerCount { get; }

////    /// <summary>
////    /// Gets the maximum number of players.
////    /// </summary>
////    int MaximumPlayers { get; }

////    ///// <summary>
////    ///// Gets the maps which are hosted on this server.
////    ///// </summary>
////    //IList<IGameMapInfo> Maps { get; }

////    /// <summary>
////    /// Creates an instance of <see cref="ServerInfo"/> with the data of this instance.
////    /// </summary>
////    /// <returns>The created <see cref="ServerInfo"/>.</returns>
////    public ServerInfo CreateServerInfo() => new (this.Id, this.Description, this.OnlinePlayerCount, this.MaximumPlayers);
////}
