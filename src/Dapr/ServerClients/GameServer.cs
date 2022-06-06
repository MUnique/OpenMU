// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using System.ComponentModel;
using MUnique.OpenMU.Interfaces;
using Dapr.Client;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Implementation of an <see cref="IGameServer"/> which accesses another game server remotely over Dapr.
/// </summary>
public class GameServer : IGameServer
{
    private readonly DaprClient _client;
    private readonly string _targetAppId;
    private Guid? _configurationId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServer"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="serverId">The server identifier.</param>
    public GameServer(DaprClient daprClient, int serverId)
    {
        this.Id = serverId;
        this.MaximumConnections = 1000;
        this.CurrentConnections = 0;

        this._client = daprClient;
        this._targetAppId = $"gameServer{serverId}";
        this.Description = $"Game Server {serverId}";
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public Guid ConfigurationId => this._configurationId ??= this._client.InvokeMethodAsync<Guid>(this._targetAppId, $"get{nameof(this.ConfigurationId)}").Result;

    /// <inheritdoc />
    public string Description { get; }

    /// <inheritdoc />
    public ServerType Type => ServerType.GameServer;

    /// <inheritdoc />
    public ServerState ServerState => this._client.InvokeMethodAsync<ServerState>(this._targetAppId, nameof(this.ServerState)).Result;

    /// <inheritdoc />
    public int MaximumConnections { get; } // TODO

    /// <inheritdoc />
    public int CurrentConnections { get; } // TODO

    /// <inheritdoc />
    public void Start()
    {
        Task.Run(() => this.StartAsync(default)).WaitAndUnwrapException();
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        Task.Run(() => this.StopAsync(default)).WaitAndUnwrapException();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return this._client.InvokeMethodAsync(this._targetAppId, nameof(this.Start), cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._client.InvokeMethodAsync(this._targetAppId, nameof(this.Shutdown), cancellationToken);
    }

    /// <inheritdoc />
    public void GuildChatMessage(uint guildId, string sender, string message)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildChatMessage), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public void AllianceChatMessage(uint guildId, string sender, string message)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.AllianceChatMessage), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public void GuildDeleted(uint guildId)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildDeleted), guildId);
    }

    /// <inheritdoc />
    public void GuildPlayerKicked(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildPlayerKicked), playerName);
    }

    /// <inheritdoc />
    public void AssignGuildToPlayer(string characterName, GuildMemberStatus guildStatus)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.AssignGuildToPlayer), new GuildMemberAssignArguments(characterName, guildStatus));
    }

    /// <inheritdoc />
    public void LetterReceived(LetterHeader letter)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.LetterReceived), letter);
    }

    /// <inheritdoc />
    public void InitializeMessenger(MessengerInitializationData initializationData)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.InitializeMessenger), initializationData);
    }

    /// <inheritdoc />
    public void SendGlobalMessage(string message, MessageType messageType)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.SendGlobalMessage), new MessageArguments(message, messageType));
    }

    /// <inheritdoc />
    public void FriendRequest(string requester, string receiver)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.FriendRequest), new RequestArguments(requester, receiver));
    }

    /// <inheritdoc />
    public void FriendOnlineStateChanged(string player, string friend, int serverId)
    {
        // TODO: Use PubSub!
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.FriendOnlineStateChanged), new FriendOnlineStateChangedArguments(player, friend, serverId));
    }

    /// <inheritdoc />
    public void ChatRoomCreated(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.LetterReceived), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));
    }

    /// <inheritdoc />
    public bool DisconnectPlayer(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.DisconnectPlayer), playerName);
        return true;
    }

    /// <inheritdoc />
    public bool BanPlayer(string playerName)
    {
        this._client.InvokeMethodAsync(this._targetAppId, nameof(this.BanPlayer), playerName);
        return true;
    }
}