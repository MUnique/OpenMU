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

    /*
    public Guid ConfigurationId { get; }
    public string Description { get; }
    public ServerType Type { get; }
    public ServerState ServerState { get; }
    public int MaximumConnections { get; }
    public int CurrentConnections { get; }
    */
    //[HttpPost(nameof(IGameServer.Start))]
    //public void Start()
    //{
    //    if (this._gameServer.ServerState == ServerState.Stopped)
    //    {
    //        this._gameServer.Start();
    //    }
    //}

    //[HttpPost(nameof(IGameServer.Shutdown))]
    //public void Shutdown()
    //{
    //    if (this._gameServer.ServerState == ServerState.Started)
    //    {
    //        this._gameServer.Shutdown();
    //    }
    //}

    // public IGameServerInfo ServerInfo { get; }
    

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

    //[HttpPost(nameof(IGameServer.IsPlayerOnline))]
    //public bool IsPlayerOnline([FromBody] string playerName)
    //{
    //    return this._gameServer.IsPlayerOnline(playerName);
    //}

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

    //[HttpPost]
    //public void RegisterMapObserver(ushort mapId, object worldObserver)
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPost("unregister-observer")]
    //public void UnregisterMapObserver(ushort mapId, ushort worldObserverId)
    //{
    //    throw new NotImplementedException();
    //}
}