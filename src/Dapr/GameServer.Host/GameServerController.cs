using Dapr;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

namespace MUnique.OpenMU.GameServer.Host;

[ApiController]
[Route("")]
public class GameServerController : ControllerBase
{
    private readonly IGameServer _gameServer;

    public GameServerController(GameServer gameServer)
    {
        this._gameServer = gameServer;
    }

    [HttpPost(nameof(IGameServer.GuildChatMessage))]
    [Topic("pubsub", nameof(IGameServer.GuildChatMessage))]
    public void GuildChatMessage([FromBody] GuildMessageArguments data)
    {
        this._gameServer.GuildChatMessage(data.GuildId, data.Sender, data.Message);
    }

    [HttpPost(nameof(IGameServer.AllianceChatMessage))]
    [Topic("pubsub", nameof(IGameServer.AllianceChatMessage))]
    public void AllianceChatMessage([FromBody] GuildMessageArguments data)
    {
        this._gameServer.AllianceChatMessage(data.GuildId, data.Sender, data.Message);
    }

    [HttpPost(nameof(IGameServer.GuildDeleted))]
    [Topic("pubsub", nameof(GuildDeleted))]
    public void GuildDeleted([FromBody] uint guildId)
    {
        this._gameServer.GuildDeleted(guildId);
    }

    [HttpPost(nameof(IGameServer.GuildPlayerKicked))]
    [Topic("pubsub", nameof(IGameServer.GuildPlayerKicked))]
    public void GuildPlayerKicked([FromBody] string playerName)
    {
        this._gameServer.GuildPlayerKicked(playerName);
    }

    [HttpPost(nameof(IGameServer.LetterReceived))]
    [Topic("pubsub", nameof(IGameServer.LetterReceived))]
    public void LetterReceived([FromBody] LetterHeader letter)
    {
        this._gameServer.LetterReceived(letter);
    }

    [HttpPost(nameof(IGameServer.AssignGuildToPlayer))]
    public void AssignGuildToPlayer([FromBody] GuildMemberAssignArguments data)
    {
        this._gameServer.AssignGuildToPlayer(data.CharacterName, data.MemberStatus);
    }

    [HttpPost(nameof(IGameServer.InitializeMessenger))]
    public void InitializeMessenger([FromBody] MessengerInitializationData initializationData)
    {
        this._gameServer.InitializeMessenger(initializationData);
    }

    [HttpPost(nameof(IGameServer.SendGlobalMessage))]
    public void SendGlobalMessage([FromBody] MessageArguments data)
    {
        this._gameServer.SendGlobalMessage(data.Message, data.Type);
    }

    [HttpPost(nameof(IGameServer.FriendRequest))]
    public void FriendRequest([FromBody] RequestArguments data)
    {
        this._gameServer.FriendRequest(data.Requester, data.Receiver);
    }

    [HttpPost(nameof(IGameServer.FriendOnlineStateChanged))]
    public void FriendOnlineStateChanged([FromBody] FriendOnlineStateChangedArguments data)
    {
        this._gameServer.FriendOnlineStateChanged(data.Player, data.Friend, data.ServerId);
    }

    [HttpPost(nameof(IGameServer.ChatRoomCreated))]
    public void ChatRoomCreated([FromBody] ChatRoomCreationArguments data)
    {
        this._gameServer.ChatRoomCreated(data.AuthenticationInfo, data.FriendName);
    }

    [HttpPost(nameof(IGameServer.DisconnectPlayer))]
    public bool DisconnectPlayer([FromBody] string playerName)
    {
        return this._gameServer.DisconnectPlayer(playerName);
    }

    [HttpPost(nameof(IGameServer.BanPlayer))]
    public bool BanPlayer([FromBody] string playerName)
    {
        return this._gameServer.BanPlayer(playerName);
    }
}