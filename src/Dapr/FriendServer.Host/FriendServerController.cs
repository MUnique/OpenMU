namespace MUnique.OpenMU.FriendServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

[ApiController]
[Route("")]
public class FriendServerController : ControllerBase
{
    private readonly IFriendServer _friendServer;

    private readonly ILogger<FriendServerController> _logger;

    public FriendServerController(IFriendServer friendServer, ILogger<FriendServerController> logger)
    {
        this._friendServer = friendServer;
        this._logger = logger;
    }

    [Topic("pubsub", nameof(IEventPublisher.PlayerEnteredGame))]
    [HttpPost(nameof(IEventPublisher.PlayerEnteredGame))]
    public void PlayerEnteredGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._friendServer.PlayerEnteredGame(data.ServerId, data.CharacterId, data.CharacterName);
    }

    [Topic("pubsub", nameof(IEventPublisher.PlayerLeftGame))]
    [HttpPost(nameof(IEventPublisher.PlayerLeftGame))]
    public void PlayerLeftGame([FromBody] PlayerOnlineStateArguments data)
    {
        this._friendServer.PlayerLeftGame(data.CharacterId, data.CharacterName);
    }

    [HttpPost(nameof(IFriendServer.SetPlayerVisibilityState))]
    public void SetPlayerInvisibilityState([FromBody] PlayerFriendOnlineStateArguments data)
    {
        this._friendServer.SetPlayerVisibilityState(data.ServerId, data.CharacterId, data.CharacterName, data.IsVisible);
    }

    [HttpPost(nameof(IFriendServer.FriendResponse))]
    public void FriendResponse([FromBody] FriendResponseArguments data)
    {
        this._friendServer.FriendResponse(data.CharacterName, data.FriendName, data.Accepted);
    }

    [HttpPost(nameof(IFriendServer.FriendRequest))]
    public bool FriendRequest([FromBody] RequestArguments data)
    {
        return this._friendServer.FriendRequest(data.Requester, data.Receiver);
    }

    [HttpPost(nameof(IFriendServer.DeleteFriend))]
    public void DeleteFriend([FromBody] RequestArguments data)
    {
        this._friendServer.DeleteFriend(data.Requester, data.Receiver);
    }

    [HttpPost(nameof(IFriendServer.CreateChatRoom))]
    public void CreateChatRoom([FromBody] RequestArguments data)
    {
        this._friendServer.CreateChatRoom(data.Requester, data.Receiver);
    }

    [HttpPost(nameof(IFriendServer.InviteFriendToChatRoom))]
    public bool InviteFriendToChatRoom([FromBody] ChatRoomInvitationArguments data)
    {
        return this._friendServer.InviteFriendToChatRoom(data.CharacterName, data.FriendName, data.RoomNumber);
    }
}