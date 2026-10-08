// <copyright file="FriendNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.FriendServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Implementation of a <see cref="IFriendNotifier"/> which notifies the game server
/// about notifications for a player about changes in the friend system.
/// </summary>
/// <remarks>
/// The notifications are published to all game servers, and the game server which hosts the player handles them.
/// This way, game servers don't need a unique dapr app id.
/// </remarks>
public class FriendNotifier : IFriendNotifier
{
    private const string PubSubName = "pubsub";

    private readonly DaprClient _daprClient;
    private readonly ILogger<FriendNotifier> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendNotifier" /> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public FriendNotifier(DaprClient daprClient, ILogger<FriendNotifier> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask FriendRequestAsync(string requester, string receiver, int serverId)
    {
        try
        {
            await this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.FriendRequestAsync), new RequestArguments(requester, receiver)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <inheritdoc />
    /// <remarks>It's usually never called here, but at <see cref="MUnique.OpenMU.FriendServer.FriendServer.ForwardLetterAsync"/>.</remarks>
    public async ValueTask LetterReceivedAsync(LetterHeader letter)
    {
        try
        {
            await this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.LetterReceivedAsync), letter).ConfigureAwait(false);
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
            if (IsOnGameServer(playerServerId))
            {
                await this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.FriendOnlineStateChangedAsync), new FriendOnlineStateChangedArguments(player, friend, friendServerId)).ConfigureAwait(false);
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
            await this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.ChatRoomCreatedAsync), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName)).ConfigureAwait(false);
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
            if (IsOnGameServer(serverId))
            {
                await this._daprClient.PublishEventAsync(PubSubName, nameof(IGameServer.InitializeMessengerAsync), initializationData).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, nameof(this.FriendRequestAsync));
        }
    }

    /// <summary>
    /// Determines whether the server id is the id of a game server, and not e.g. the id which means that the player is offline.
    /// </summary>
    private static bool IsOnGameServer(int serverId) => serverId is >= 0 and <= byte.MaxValue;
}
