// <copyright file="ChatServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using System.ComponentModel;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Implementation of an <see cref="IChatServer"/> which accesses another chat server remotely over Dapr.
/// </summary>
public class ChatServer : IChatServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<ChatServer> _logger;
    private readonly string _targetAppId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServer"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public ChatServer(DaprClient daprClient, ILogger<ChatServer> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
        this._targetAppId = "chatServer";
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public Guid ConfigurationId { get; }

    /// <inheritdoc />
    public string Description => "Chat Server";

    /// <inheritdoc />
    public ServerType Type => ServerType.ChatServer;

    /// <inheritdoc />
    public ServerState ServerState => ServerState.Started;

    /// <inheritdoc />
    public int MaximumConnections { get; }

    /// <inheritdoc />
    public int CurrentConnections { get; }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ValueTask StartAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ValueTask ShutdownAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async ValueTask<ChatServerAuthenticationInfo?> RegisterClientAsync(ushort roomId, string clientName)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<RegisterChatClientArguments, ChatServerAuthenticationInfo>(
                    this._targetAppId,
                    nameof(IChatServer.RegisterClientAsync),
                    new RegisterChatClientArguments(roomId, clientName))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.RegisterClientAsync));
        }

        return null;
    }

    /// <inheritdoc />
    public async ValueTask<ushort> CreateChatRoomAsync()
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<ushort>(this._targetAppId, nameof(IChatServer.CreateChatRoomAsync)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.CreateChatRoomAsync));
        }

        return default;
    }
}