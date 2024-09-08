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
    /// Shuts down the server gracefully.
    /// </summary>
    /// <returns></returns>
    [HttpPost(nameof(IGameServer.ShutdownAsync))]
    public async ValueTask ShutdownAsync()
    {
        await this._gameServer.ShutdownAsync().ConfigureAwait(false);
        Environment.Exit(0);
    }

    /// <summary>
    /// Sends a chat message to all connected guild members.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.GuildChatMessageAsync))]
    [Topic("pubsub", nameof(IGameServer.GuildChatMessageAsync))]
    public ValueTask GuildChatMessageAsync([FromBody] GuildMessageArguments data)
    {
        return this._gameServer.GuildChatMessageAsync(data.GuildId, data.Sender, data.Message);
    }

    /// <summary>
    /// Sends a chat message to all connected alliance members.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.AllianceChatMessageAsync))]
    [Topic("pubsub", nameof(IGameServer.AllianceChatMessageAsync))]
    public ValueTask AllianceChatMessageAsync([FromBody] GuildMessageArguments data)
    {
        return this._gameServer.AllianceChatMessageAsync(data.GuildId, data.Sender, data.Message);
    }

    /// <summary>
    /// Notifies the game server that a guild got deleted.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    [HttpPost(nameof(IGameServer.GuildDeletedAsync))]
    [Topic("pubsub", nameof(GuildDeletedAsync))]
    public ValueTask GuildDeletedAsync([FromBody] uint guildId)
    {
        return this._gameServer.GuildDeletedAsync(guildId);
    }

    /// <summary>
    /// Notifies the game server that a guild member got removed from a guild.
    /// </summary>
    /// <param name="playerName">Name of the player which got removed from a guild.</param>
    [HttpPost(nameof(IGameServer.GuildPlayerKickedAsync))]
    [Topic("pubsub", nameof(IGameServer.GuildPlayerKickedAsync))]
    public ValueTask GuildPlayerKickedAsync([FromBody] string playerName)
    {
        return this._gameServer.GuildPlayerKickedAsync(playerName);
    }

    /// <summary>
    /// Notifies the game server that a letter got received for an online player.
    /// </summary>
    /// <param name="letter">The letter header.</param>
    [HttpPost(nameof(IGameServer.LetterReceivedAsync))]
    [Topic("pubsub", nameof(IGameServer.LetterReceivedAsync))]
    public ValueTask LetterReceivedAsync([FromBody] LetterHeader letter)
    {
        return this._gameServer.LetterReceivedAsync(letter);
    }

    /// <summary>
    /// Assigns the guild to the player.
    /// </summary>
    /// <param name="data">The assignment arguments.</param>
    [HttpPost(nameof(IGameServer.AssignGuildToPlayerAsync))]
    public ValueTask AssignGuildToPlayerAsync([FromBody] GuildMemberAssignArguments data)
    {
        return this._gameServer.AssignGuildToPlayerAsync(data.CharacterName, data.MemberStatus);
    }

    /// <summary>
    /// Initializes the messenger of a player.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    [HttpPost(nameof(IGameServer.InitializeMessengerAsync))]
    public ValueTask InitializeMessengerAsync([FromBody] MessengerInitializationData initializationData)
    {
        return this._gameServer.InitializeMessengerAsync(initializationData);
    }

    /// <summary>
    /// Sends a global message to all connected players with the specified message type.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.SendGlobalMessageAsync))]
    public ValueTask SendGlobalMessageAsync([FromBody] MessageArguments data)
    {
        return this._gameServer.SendGlobalMessageAsync(data.Message, data.Type);
    }

    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="data">The request arguments.</param>
    [HttpPost(nameof(IGameServer.FriendRequestAsync))]
    public ValueTask FriendRequestAsync([FromBody] RequestArguments data)
    {
        return this._gameServer.FriendRequestAsync(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="data">The state change arguments.</param>
    [HttpPost(nameof(IGameServer.FriendOnlineStateChangedAsync))]
    public ValueTask FriendOnlineStateChangedAsync([FromBody] FriendOnlineStateChangedArguments data)
    {
        return this._gameServer.FriendOnlineStateChangedAsync(data.Player, data.Friend, data.ServerId);
    }

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="data">The chat room creation arguments.</param>
    [HttpPost(nameof(IGameServer.ChatRoomCreatedAsync))]
    public ValueTask ChatRoomCreatedAsync([FromBody] ChatRoomCreationArguments data)
    {
        return this._gameServer.ChatRoomCreatedAsync(data.AuthenticationInfo, data.FriendName);
    }

    /// <summary>
    /// Disconnects the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.DisconnectPlayerAsync))]
    public ValueTask<bool> DisconnectPlayerAsync([FromBody] string playerName)
    {
        return this._gameServer.DisconnectPlayerAsync(playerName);
    }

    /// <summary>
    /// Disconnects the account from the game.
    /// </summary>
    /// <param name="accountName">Name of the account.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.DisconnectPlayerAsync))]
    public ValueTask<bool> DisconnectAccountAsync([FromBody] string accountName)
    {
        return this._gameServer.DisconnectAccountAsync(accountName);
    }

    /// <summary>
    /// Bans the player from the game.
    /// </summary>
    /// <param name="playerName">Name of the player.</param>
    /// <returns>True, if the player has been banned; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.BanPlayerAsync))]
    public ValueTask<bool> BanPlayerAsync([FromBody] string playerName)
    {
        return this._gameServer.BanPlayerAsync(playerName);
    }
}