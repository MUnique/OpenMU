// <copyright file="FriendServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// API Controller which handles the calls from the <see cref="ServerClients.FriendServer"/>.
/// </summary>
[ApiController]
[Route("")]
public class FriendServerController : ControllerBase
{
    private readonly IFriendServer _friendServer;

    private readonly ILogger<FriendServerController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendServerController"/> class.
    /// </summary>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="logger">The logger.</param>
    public FriendServerController(IFriendServer friendServer, ILogger<FriendServerController> logger)
    {
        this._friendServer = friendServer;
        this._logger = logger;
    }

    /// <summary>
    /// Is called when a player entered the game.
    /// It will cause a response with <see cref="IFriendSystemSubscriber.InitializeMessengerAsync" />
    /// and a state update for friends.
    /// </summary>
    /// <param name="data">The data.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGameAsync))]
    public async Task PlayerEnteredGameAsync([FromBody] PlayerOnlineStateArguments data)
    {
        await this._friendServer.PlayerEnteredGameAsync(data.ServerId, data.CharacterId, data.CharacterName).ConfigureAwait(false);
    }

    /// <summary>
    /// Is called when a player leaves the game.
    /// It will cause a state update for friends.
    /// </summary>
    /// <param name="data">The data.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGameAsync))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGameAsync))]
    public async Task PlayerLeftGameAsync([FromBody] PlayerOnlineStateArguments data)
    {
        await this._friendServer.PlayerLeftGameAsync(data.CharacterId, data.CharacterName).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the online visibility state of a character.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.SetPlayerVisibilityStateAsync))]
    public async Task SetPlayerInvisibilityStateAsync([FromBody] PlayerFriendOnlineStateArguments data)
    {
        await this._friendServer.SetPlayerVisibilityStateAsync(data.ServerId, data.CharacterId, data.CharacterName, data.IsVisible).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the friend request response.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.FriendResponseAsync))]
    public async Task FriendResponseAsync([FromBody] FriendResponseArguments data)
    {
        await this._friendServer.FriendResponseAsync(data.CharacterName, data.FriendName, data.Accepted).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a friend request to the friend, and adds a new friend view item to the players friend list.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>If a new friend view item got added to the players friend list.</returns>
    [HttpPost(nameof(IFriendServer.FriendRequestAsync))]
    public async Task<bool> FriendRequestAsync([FromBody] RequestArguments data)
    {
        return await this._friendServer.FriendRequestAsync(data.Requester, data.Receiver).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the friend.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.DeleteFriendAsync))]
    public async Task DeleteFriendAsync([FromBody] RequestArguments data)
    {
        await this._friendServer.DeleteFriendAsync(data.Requester, data.Receiver).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new chat room.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.CreateChatRoomAsync))]
    public async Task CreateChatRoomAsync([FromBody] RequestArguments data)
    {
        await this._friendServer.CreateChatRoomAsync(data.Requester, data.Receiver).ConfigureAwait(false);
    }

    /// <summary>
    /// Invites a friend to an existing chat room.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The success of the invitation.</returns>
    [HttpPost(nameof(IFriendServer.InviteFriendToChatRoomAsync))]
    public async Task<bool> InviteFriendToChatRoomAsync([FromBody] ChatRoomInvitationArguments data)
    {
        return await this._friendServer.InviteFriendToChatRoomAsync(data.CharacterName, data.FriendName, data.RoomNumber).ConfigureAwait(false);
    }
}