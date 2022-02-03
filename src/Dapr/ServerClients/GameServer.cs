// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace MUnique.OpenMU.ServerClients;

using System.ComponentModel;
using MUnique.OpenMU.Interfaces;
using Dapr.Client;

public class GameServer : IGameServer
{
    private readonly DaprClient _client;

    private readonly string _targetAppId;
    private Guid? _configurationId;

    public event PropertyChangedEventHandler? PropertyChanged;

    public GameServer(DaprClient daprClient, int serverId)
    {
        this.Id = serverId;
        this.MaximumConnections = 1000;
        this.CurrentConnections = 0;

        this._client = daprClient;
        this._targetAppId = $"gameServer{serverId}";
        this.Description = $"Game Server {serverId}";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _client.InvokeMethodAsync(this._targetAppId, nameof(Start), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _client.InvokeMethodAsync(this._targetAppId, nameof(Shutdown), cancellationToken);
    }

    public int Id { get; }

    public Guid ConfigurationId => _configurationId ??= this._client.InvokeMethodAsync<Guid>(this._targetAppId, "getConfigurationId").Result;

    public string Description { get; }

    public ServerType Type => ServerType.GameServer;

    public ServerState ServerState => this._client.InvokeMethodAsync<ServerState>(this._targetAppId, nameof(ServerState)).Result;

    public int MaximumConnections { get; } // TODO

    public int CurrentConnections { get; } // TODO

    /// <inheritdoc />
    public void Start()
    {
        Task.Run(() => this.StartAsync(default));
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        Task.Run(() => this.StopAsync(default));
    }

    /// <inheritdoc />
    public void GuildChatMessage(uint guildId, string sender, string message)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(GuildChatMessage), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public void AllianceChatMessage(uint guildId, string sender, string message)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(AllianceChatMessage), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public void GuildDeleted(uint guildId)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(GuildDeleted), guildId);
    }

    /// <inheritdoc />
    public void GuildPlayerKicked(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(GuildPlayerKicked), playerName);
    }

    /// <inheritdoc />
    public void AssignGuildToPlayer(string characterName, GuildMemberStatus guildStatus)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(AssignGuildToPlayer), new GuildMemberAssignArguments(characterName, guildStatus));
    }

    /// <inheritdoc />
    public void LetterReceived(LetterHeader letter)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(LetterReceived), letter);
    }

    /// <inheritdoc />
    public void InitializeMessenger(MessengerInitializationData initializationData)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(InitializeMessenger), initializationData);
    }

    /// <inheritdoc />
    public void SendGlobalMessage(string message, MessageType messageType)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(SendGlobalMessage), new MessageArguments(message, messageType));
    }

    /// <inheritdoc />
    public void FriendRequest(string requester, string receiver)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(FriendRequest), new RequestArguments(requester, receiver));
    }

    /// <inheritdoc />
    public void FriendOnlineStateChanged(string player, string friend, int serverId)
    {
        // TODO: Use PubSub!
        this._client.InvokeMethodAsync(this._targetAppId, nameof(FriendOnlineStateChanged), new FriendOnlineStateChangedArguments(player, friend, serverId));
    }

    /// <inheritdoc />
    public void ChatRoomCreated(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(LetterReceived), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));
    }

    /// <inheritdoc />
    public void RegisterMapObserver(ushort mapId, object worldObserver)
    {
        throw new NotImplementedException(nameof(RegisterMapObserver));
    }

    /// <inheritdoc />
    public void UnregisterMapObserver(ushort mapId, ushort worldObserverId)
    {
        //this._client.InvokeMethodAsync(this._targetAppId, nameof(UnregisterMapObserver), (mapId, worldObserverId));
    }

    /// <inheritdoc />
    public bool DisconnectPlayer(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(DisconnectPlayer), playerName);
        return true;
    }

    /// <inheritdoc />
    public bool BanPlayer(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(BanPlayer), playerName);
        return true;
    }
}