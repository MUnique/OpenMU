// <copyright file="FriendNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer.Host;

using System.Collections.ObjectModel;
using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Implementation of a <see cref="IFriendNotifier"/> which notifies the game server
/// about notifications for a player about changes in the friend system.
/// </summary>
public class FriendNotifier : IFriendNotifier
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<FriendNotifier> _logger;
    private readonly IReadOnlyDictionary<int, string> _appIds;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendNotifier" /> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public FriendNotifier(DaprClient daprClient, ILogger<FriendNotifier> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;

        var appIds = new Dictionary<int, string>();
        for (int i = 0; i < 100; i++)
        {
            appIds.Add(i, $"gameServer{i + 1}");
        }

        this._appIds = new ReadOnlyDictionary<int, string>(appIds);
    }

    /// <inheritdoc />
    public async ValueTask FriendRequestAsync(string requester, string receiver, int serverId)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._appIds[serverId], nameof(IGameServer.FriendRequestAsync), new RequestArguments(requester, receiver));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <inheritdoc />
    /// <remarks>It's usually never called here, but at <see cref="FriendServer.ForwardLetterAsync"/>.</remarks>
    public async ValueTask LetterReceivedAsync(LetterHeader letter)
    {
        try
        {
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.LetterReceivedAsync), letter);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask FriendOnlineStateChangedAsync(int playerServerId, string player, string friend, int friendServerId)
    {
        try
        {
            // todo: find out if this is correct when logging out
            if (this._appIds.TryGetValue(playerServerId, out var gameServer))
            {
                await this._daprClient.InvokeMethodAsync(gameServer, nameof(IGameServer.FriendOnlineStateChangedAsync), new FriendOnlineStateChangedArguments(player, friend, friendServerId));
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask ChatRoomCreatedAsync(int serverId, ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._appIds[serverId], nameof(IGameServer.ChatRoomCreatedAsync), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <inheritdoc />
    public async ValueTask InitializeMessengerAsync(int serverId, MessengerInitializationData initializationData)
    {
        try
        {
            if (this._appIds.TryGetValue(serverId, out var gameServer))
            {
                await this._daprClient.InvokeMethodAsync(gameServer, nameof(IGameServer.InitializeMessengerAsync), initializationData);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }
}