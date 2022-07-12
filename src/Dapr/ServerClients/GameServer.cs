// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using System.ComponentModel;
using MUnique.OpenMU.Interfaces;
using Dapr.Client;

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
    public async ValueTask StartAsync()
    {
        await this.StartAsync(default);
    }

    /// <inheritdoc />
    public async ValueTask ShutdownAsync()
    {
        await this.StopAsync(default);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return this._client.InvokeMethodAsync(this._targetAppId, nameof(this.StartAsync), cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return this._client.InvokeMethodAsync(this._targetAppId, nameof(this.ShutdownAsync), cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask GuildChatMessageAsync(uint guildId, string sender, string message)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildChatMessageAsync), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public async ValueTask AllianceChatMessageAsync(uint guildId, string sender, string message)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.AllianceChatMessageAsync), new GuildMessageArguments(guildId, sender, message));
    }

    /// <inheritdoc />
    public async ValueTask GuildDeletedAsync(uint guildId)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildDeletedAsync), guildId);
    }

    /// <inheritdoc />
    public async ValueTask GuildPlayerKickedAsync(string playerName)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.GuildPlayerKickedAsync), playerName);
    }

    /// <inheritdoc />
    public async ValueTask AssignGuildToPlayerAsync(string characterName, GuildMemberStatus guildStatus)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.AssignGuildToPlayerAsync), new GuildMemberAssignArguments(characterName, guildStatus));
    }

    /// <inheritdoc />
    public async ValueTask LetterReceivedAsync(LetterHeader letter)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.LetterReceivedAsync), letter);
    }

    /// <inheritdoc />
    public async ValueTask InitializeMessengerAsync(MessengerInitializationData initializationData)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.InitializeMessengerAsync), initializationData);
    }

    /// <inheritdoc />
    public async ValueTask SendGlobalMessageAsync(string message, MessageType messageType)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.SendGlobalMessageAsync), new MessageArguments(message, messageType));
    }

    /// <inheritdoc />
    public async ValueTask FriendRequestAsync(string requester, string receiver)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.FriendRequestAsync), new RequestArguments(requester, receiver));
    }

    /// <inheritdoc />
    public async ValueTask FriendOnlineStateChangedAsync(string player, string friend, int serverId)
    {
        // TODO: Use PubSub!
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.FriendOnlineStateChangedAsync), new FriendOnlineStateChangedArguments(player, friend, serverId));
    }

    /// <inheritdoc />
    public async ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        await this._client.InvokeMethodAsync(this._targetAppId, nameof(this.ChatRoomCreatedAsync), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));
    }

    /// <inheritdoc />
    public async ValueTask<bool> DisconnectPlayerAsync(string playerName)
    {
        return await this._client.InvokeMethodAsync<string, bool>(this._targetAppId, nameof(this.DisconnectPlayerAsync), playerName);
    }

    /// <inheritdoc />
    public async ValueTask<bool> BanPlayerAsync(string playerName)
    {
        return await this._client.InvokeMethodAsync<string, bool>(this._targetAppId, nameof(this.BanPlayerAsync), playerName);
    }
}