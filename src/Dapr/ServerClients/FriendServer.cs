// <copyright file="FriendServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using Microsoft.Extensions.Logging;
using Dapr.Client;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Implementation of an <see cref="IFriendServer"/> which accesses another friend server remotely over Dapr.
/// </summary>
public class FriendServer : IFriendServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<FriendServer> _logger;
    private readonly string _targetAppId;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendServer"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public FriendServer(DaprClient daprClient, ILogger<FriendServer> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
        this._targetAppId = "friendServer";
    }

    /// <inheritdoc />
    public async ValueTask ForwardLetterAsync(LetterHeader letter)
    {
        try
        {
            await this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.LetterReceivedAsync), letter).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when forwarding a letter.");
        }
    }

    /// <inheritdoc />
    public async ValueTask FriendResponseAsync(string characterName, string friendName, bool accepted)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.FriendResponseAsync), new FriendResponseArguments(characterName, friendName, accepted)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a friend response.");
        }
    }

    /// <inheritdoc />
    public ValueTask PlayerEnteredGameAsync(byte serverId, Guid characterId, string characterName)
    {
        // no action required - the friend server listens to the common pubsub, published by EventPublisher
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask PlayerLeftGameAsync(Guid characterId, string characterName)
    {
        // no action required - the friend server listens to the common pubsub, published by EventPublisher
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask SetPlayerVisibilityStateAsync(byte serverId, Guid characterId, string characterName, bool isVisible)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.SetPlayerVisibilityStateAsync), new PlayerFriendOnlineStateArguments(characterId, characterName, serverId, isVisible)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when setting the friend visibility state.");
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> FriendRequestAsync(string playerName, string friendName)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<RequestArguments, bool>(this._targetAppId, nameof(this.FriendRequestAsync), new RequestArguments(playerName, friendName)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when sending a friend request.");
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask DeleteFriendAsync(string name, string friendName)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.DeleteFriendAsync), new RequestArguments(name, friendName)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when deleting a friend.");
        }
    }

    /// <inheritdoc />
    public async ValueTask CreateChatRoomAsync(string playerName, string friendName)
    {
        try
        {
            await this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.CreateChatRoomAsync), new RequestArguments(playerName, friendName)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when creating a chat room.");
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> InviteFriendToChatRoomAsync(string selectedCharacterName, string friendName, ushort roomNumber)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<ChatRoomInvitationArguments, bool>(this._targetAppId, nameof(this.InviteFriendToChatRoomAsync), new ChatRoomInvitationArguments(selectedCharacterName, friendName, roomNumber)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when inviting a friend to a chat room.");
            return false;
        }
    }
}