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
    /// It will cause a response with <see cref="IFriendSystemSubscriber.InitializeMessenger" />
    /// and a state update for friends.
    /// </summary>
    /// <param name="data">The data.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGame))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGame))]
    public void PlayerEnteredGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._friendServer.PlayerEnteredGame(data.ServerId, data.CharacterId, data.CharacterName);
    }

    /// <summary>
    /// Is called when a player leaves the game.
    /// It will cause a state update for friends.
    /// </summary>
    /// <param name="data">The data.</param>
    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGame))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGame))]
    public void PlayerLeftGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._friendServer.PlayerLeftGame(data.CharacterId, data.CharacterName);
    }

    /// <summary>
    /// Sets the online visibility state of a character.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.SetPlayerVisibilityState))]
    public void SetPlayerInvisibilityState([FromBody] PlayerFriendOnlineStateArguments data)
    {
        this._friendServer.SetPlayerVisibilityState(data.ServerId, data.CharacterId, data.CharacterName, data.IsVisible);
    }

    /// <summary>
    /// Handles the friend request response.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.FriendResponse))]
    public void FriendResponse([FromBody] FriendResponseArguments data)
    {
        this._friendServer.FriendResponse(data.CharacterName, data.FriendName, data.Accepted);
    }

    /// <summary>
    /// Sends a friend request to the friend, and adds a new friend view item to the players friend list.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>If a new friend view item got added to the players friend list.</returns>
    [HttpPost(nameof(IFriendServer.FriendRequest))]
    public bool FriendRequest([FromBody] RequestArguments data)
    {
        return this._friendServer.FriendRequest(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Deletes the friend.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.DeleteFriend))]
    public void DeleteFriend([FromBody] RequestArguments data)
    {
        this._friendServer.DeleteFriend(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Creates a new chat room.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost(nameof(IFriendServer.CreateChatRoom))]
    public void CreateChatRoom([FromBody] RequestArguments data)
    {
        this._friendServer.CreateChatRoom(data.Requester, data.Receiver);
    }

    /// <summary>
    /// Invites a friend to an existing chat room.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    [HttpPost(nameof(IFriendServer.InviteFriendToChatRoom))]
    public bool InviteFriendToChatRoom([FromBody] ChatRoomInvitationArguments data)
    {
        return this._friendServer.InviteFriendToChatRoom(data.CharacterName, data.FriendName, data.RoomNumber);
    }
}