// <copyright file="ChatServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.ServerClients;

public class ChatServer : IChatServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<ChatServer> _logger;
    private readonly string _targetAppId;

    public ChatServer(DaprClient daprClient, ILogger<ChatServer> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
        this._targetAppId = "chatServer";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public int Id { get; }
    public Guid ConfigurationId { get; }
    public string Description => "Chat Server";
    public ServerType Type => ServerType.ChatServer;
    public ServerState ServerState => ServerState.Started;
    public int MaximumConnections { get; }
    public int CurrentConnections { get; }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Shutdown()
    {
        throw new NotImplementedException();
    }

    public ChatServerAuthenticationInfo RegisterClient(ushort roomId, string clientName)
    {
        return this._daprClient.InvokeMethodAsync<RegisterChatClientArguments, ChatServerAuthenticationInfo>(this._targetAppId, nameof(RegisterClient), new RegisterChatClientArguments(roomId, clientName)).GetAwaiter().GetResult();
    }

    public ushort CreateChatRoom()
    {
        return this._daprClient.InvokeMethodAsync<ushort>(this._targetAppId, nameof(CreateChatRoom)).GetAwaiter().GetResult();
    }
}