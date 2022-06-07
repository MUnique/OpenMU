// <copyright file="GameServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// The API controller for the game server which handles the calls from other services.
/// </summary>
[ApiController]
[Route("")]
public class GameServerController : ControllerBase
{
    private readonly IGameServer _gameServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerController"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    public GameServerController(GameServer gameServer)
    {
        this._gameServer = gameServer;
    }

    /// <summary>
    /// Sends a chat message to all connected guild members.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.GuildChatMessage))]
    [Topic("pubsub", nameof(IGameServer.GuildChatMessage))]
    public void GuildChatMessage([FromBody] GuildMessageArguments data)
    {
        this._gameServer.GuildChatMessage(data.GuildId, data.Sender, data.Message);
    }

    /// <summary>
    /// Sends a chat message to all connected alliance members.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.AllianceChatMessage))]
    [Topic("pubsub", nameof(IGameServer.AllianceChatMessage))]
    public void AllianceChatMessage([FromBody] GuildMessageArguments data)
    {
        this._gameServer.AllianceChatMessage(data.GuildId, data.Sender, data.Message);
    }

    /// <summary>
    /// Notifies the game server that a guild got deleted.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    [HttpPost(nameof(IGameServer.GuildDeleted))]
    [Topic("pubsub", nameof(GuildDeleted))]
    public void GuildDeleted([FromBody] uint guildId)
    {
        this._gameServer.GuildDeleted(guildId);
    }

    /// <summary>
    /// Notifies the game server that a guild member got removed from a guild.
    /// </summary>
    /// <param name="playerName">Name of the player which got removed from a guild.</param>
    [HttpPost(nameof(IGameServer.GuildPlayerKicked))]
    [Topic("pubsub", nameof(IGameServer.GuildPlayerKicked))]
    public void GuildPlayerKicked([FromBody] string playerName)
    {
        this._gameServer.GuildPlayerKicked(playerName);
    }

    /// <summary>
    /// Notifies the game server that a letter got received for an online player.
    /// </summary>
    /// <param name="letter">The letter header.</param>
    [HttpPost(nameof(IGameServer.LetterReceived))]
    [Topic("pubsub", nameof(IGameServer.LetterReceived))]
    public void LetterReceived([FromBody] LetterHeader letter)
    {
        this._gameServer.LetterReceived(letter);
    }

    /// <summary>
    /// Assigns the guild to the player.
    /// </summary>
    /// <param name="data">The assignment arguments.</param>
    [HttpPost(nameof(IGameServer.AssignGuildToPlayer))]
    public void AssignGuildToPlayer([FromBody] GuildMemberAssignArguments data)
    {
        this._gameServer.AssignGuildToPlayer(data.CharacterName, data.MemberStatus);
    }

    /// <summary>
    /// Initializes the messenger of a player.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    [HttpPost(nameof(IGameServer.InitializeMessenger))]
    public void InitializeMessenger([FromBody] MessengerInitializationData initializationData)
    {
        this._gameServer.InitializeMessenger(initializationData);
    }

    /// <summary>
    /// Sends a global message to all connected players with the specified message type.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.SendGlobalMessage))]
    public void SendGlobalMessage([FromBody] MessageArguments data)
    {
        this._gameServer.SendGlobalMessage(data.Message, data.Type);
    }

    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="data">The request arguments.</param>
    [HttpPost(nameof(IGameServer.FriendRequest))]
    public void FriendRequest([FromBody] RequestArguments data)
    {
        this._gameServer.FriendRequest(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="data">The state change arguments.</param>
    [HttpPost(nameof(IGameServer.FriendOnlineStateChanged))]
    public void FriendOnlineStateChanged([FromBody] FriendOnlineStateChangedArguments data)
    {
        this._gameServer.FriendOnlineStateChanged(data.Player, data.Friend, data.ServerId);
    }

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="data">The chat room creation arguments.</param>
    [HttpPost(nameof(IGameServer.ChatRoomCreated))]
    public void ChatRoomCreated([FromBody] ChatRoomCreationArguments data)
    {
        this._gameServer.ChatRoomCreated(data.AuthenticationInfo, data.FriendName);
    }

    /// <summary>
    /// Disconnects the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.DisconnectPlayer))]
    public bool DisconnectPlayer([FromBody] string playerName)
    {
        return this._gameServer.DisconnectPlayer(playerName);
    }

    /// <summary>
    /// Bans the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been banned; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.BanPlayer))]
    public bool BanPlayer([FromBody] string playerName)
    {
        return this._gameServer.BanPlayer(playerName);
    }
}