// <copyright file="GameServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// The API controller for the game server which handles the calls from other services.
/// </summary>
/// <remarks>
/// The calls are published to all game servers, so that game servers don't need a unique dapr app id.
/// Calls which concern a player are just handled by the game server which hosts the player.
/// The other calls carry the id of the game server, and the other game servers ignore them.
/// </remarks>
[ApiController]
[Route("")]
public class GameServerController : ControllerBase
{
    private readonly IGameServer _gameServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerController"/> class.
    /// </summary>
    /// <param name="gameServer">The game server.</param>
    public GameServerController(IGameServer gameServer)
    {
        this._gameServer = gameServer;
    }

    /// <summary>
    /// Shuts down the server gracefully and then ends the process, e.g. to apply a changed configuration.
    /// </summary>
    /// <param name="serverId">The identifier of the game server which should restart.</param>
    /// <param name="applicationLifetime">The application lifetime.</param>
    /// <remarks>
    /// The process has to be started again by its host, e.g. by the restart policy of its container.
    /// </remarks>
    [HttpPost(GameServerClient.RestartTopic)]
    [Topic("pubsub", GameServerClient.RestartTopic)]
    public async ValueTask RestartAsync([FromBody] int serverId, [FromServices] IHostApplicationLifetime applicationLifetime)
    {
        if (serverId != this._gameServer.Id)
        {
            return;
        }

        await this._gameServer.ShutdownAsync().ConfigureAwait(false);
        applicationLifetime.StopApplication();
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
    /// Sends a message of the world chat to all connected players.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.WorldChatMessageAsync))]
    [Topic("pubsub", nameof(IGameServer.WorldChatMessageAsync))]
    public ValueTask WorldChatMessageAsync([FromBody] WorldChatMessageArguments data)
    {
        return this._gameServer.WorldChatMessageAsync(data.Sender, data.Message);
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
    /// Notifies the game server that someone tried to log in with an account which is already logged in.
    /// </summary>
    /// <param name="data">The arguments of the login attempt.</param>
    [HttpPost(nameof(IGameServer.PlayerAlreadyLoggedInAsync))]
    [Topic("pubsub", nameof(IGameServer.PlayerAlreadyLoggedInAsync))]
    public ValueTask PlayerAlreadyLoggedInAsync([FromBody] PlayerLoggedInArguments data)
    {
        return this._gameServer.PlayerAlreadyLoggedInAsync(data.ServerId, data.LoginName);
    }

    /// <summary>
    /// Notifies the game server that a guild joined an alliance.
    /// </summary>
    /// <param name="data">The guilds of the alliance.</param>
    [HttpPost(nameof(IGameServer.AllianceCreatedAsync))]
    [Topic("pubsub", nameof(IGameServer.AllianceCreatedAsync))]
    public ValueTask AllianceCreatedAsync([FromBody] AllianceChangedArguments data)
    {
        return this._gameServer.AllianceCreatedAsync(data.MasterGuildId, data.MemberGuildId);
    }

    /// <summary>
    /// Notifies the game server that a guild left an alliance.
    /// </summary>
    /// <param name="data">The guilds of the alliance.</param>
    [HttpPost(nameof(IGameServer.AllianceDisbandedAsync))]
    [Topic("pubsub", nameof(IGameServer.AllianceDisbandedAsync))]
    public ValueTask AllianceDisbandedAsync([FromBody] AllianceChangedArguments data)
    {
        return this._gameServer.AllianceDisbandedAsync(data.MasterGuildId, data.MemberGuildId);
    }

    /// <summary>
    /// Notifies the game server that a hostility between two guilds was created or removed.
    /// </summary>
    /// <param name="data">The guilds and alliances of the hostility.</param>
    [HttpPost(nameof(IGameServer.GuildHostilityChangedAsync))]
    [Topic("pubsub", nameof(IGameServer.GuildHostilityChangedAsync))]
    public ValueTask GuildHostilityChangedAsync([FromBody] GuildHostilityChangedArguments data)
    {
        return this._gameServer.GuildHostilityChangedAsync(data.GuildIdA, data.AllianceGuildIdsA, data.GuildIdB, data.AllianceGuildIdsB, data.Created);
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
    [Topic("pubsub", nameof(IGameServer.AssignGuildToPlayerAsync))]
    public ValueTask AssignGuildToPlayerAsync([FromBody] GuildMemberAssignArguments data)
    {
        return this._gameServer.AssignGuildToPlayerAsync(data.CharacterName, data.MemberStatus);
    }

    /// <summary>
    /// Initializes the messenger of a player.
    /// </summary>
    /// <param name="initializationData">The initialization data.</param>
    [HttpPost(nameof(IGameServer.InitializeMessengerAsync))]
    [Topic("pubsub", nameof(IGameServer.InitializeMessengerAsync))]
    public ValueTask InitializeMessengerAsync([FromBody] MessengerInitializationData initializationData)
    {
        return this._gameServer.InitializeMessengerAsync(initializationData);
    }

    /// <summary>
    /// Sends a global message to all connected players with the specified message type.
    /// </summary>
    /// <param name="data">The message arguments.</param>
    [HttpPost(nameof(IGameServer.SendGlobalMessageAsync))]
    [Topic("pubsub", nameof(IGameServer.SendGlobalMessageAsync))]
    public ValueTask SendGlobalMessageAsync([FromBody] MessageArguments data)
    {
        if (data.ServerId != this._gameServer.Id)
        {
            return ValueTask.CompletedTask;
        }

        return this._gameServer.SendGlobalMessageAsync(data.Message, data.Type);
    }

    /// <summary>
    /// Notifies the server that a player made a friend request to another player, which is online on this server.
    /// </summary>
    /// <param name="data">The request arguments.</param>
    [HttpPost(nameof(IGameServer.FriendRequestAsync))]
    [Topic("pubsub", nameof(IGameServer.FriendRequestAsync))]
    public ValueTask FriendRequestAsync([FromBody] RequestArguments data)
    {
        return this._gameServer.FriendRequestAsync(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Notifies the game server that a friend online state changed.
    /// </summary>
    /// <param name="data">The state change arguments.</param>
    [HttpPost(nameof(IGameServer.FriendOnlineStateChangedAsync))]
    [Topic("pubsub", nameof(IGameServer.FriendOnlineStateChangedAsync))]
    public ValueTask FriendOnlineStateChangedAsync([FromBody] FriendOnlineStateChangedArguments data)
    {
        return this._gameServer.FriendOnlineStateChangedAsync(data.Player, data.Friend, data.ServerId);
    }

    /// <summary>
    /// Notifies the game server that a chat room got created on the chat server for a player which is online on this game server.
    /// </summary>
    /// <param name="data">The chat room creation arguments.</param>
    [HttpPost(nameof(IGameServer.ChatRoomCreatedAsync))]
    [Topic("pubsub", nameof(IGameServer.ChatRoomCreatedAsync))]
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
    [Topic("pubsub", nameof(IGameServer.DisconnectPlayerAsync))]
    public ValueTask<bool> DisconnectPlayerAsync([FromBody] string playerName)
    {
        return this._gameServer.DisconnectPlayerAsync(playerName);
    }

    /// <summary>
    /// Disconnects the account from the game.
    /// </summary>
    /// <param name="accountName">Name of the account.</param>
    /// <returns>True, if the player has been disconnected; False, otherwise.</returns>
    [HttpPost(nameof(IGameServer.DisconnectAccountAsync))]
    [Topic("pubsub", nameof(IGameServer.DisconnectAccountAsync))]
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
    [Topic("pubsub", nameof(IGameServer.BanPlayerAsync))]
    public ValueTask<bool> BanPlayerAsync([FromBody] string playerName)
    {
        return this._gameServer.BanPlayerAsync(playerName);
    }
}