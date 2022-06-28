// <copyright file="FriendNotifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer.Host;

using System.Collections.ObjectModel;
using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Implementation of a <see cref="IFriendNotifier"/> which notifies the game server
/// about notifications for a player about changes in the friend system.
/// </summary>
public class FriendNotifier : IFriendNotifier
{
    private readonly DaprClient _daprClient;
    private readonly IReadOnlyDictionary<int, string> _appIds;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendNotifier"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    public FriendNotifier(DaprClient daprClient)
    {
        this._daprClient = daprClient;

        var appIds = new Dictionary<int, string>();
        for (int i = 0; i < 100; i++)
        {
            appIds.Add(i, $"gameServer{i + 1}");
        }

        this._appIds = new ReadOnlyDictionary<int, string>(appIds);
    }

    /// <inheritdoc />
    public void FriendRequest(string requester, string receiver, int serverId)
    {
        this._daprClient.InvokeMethodAsync(this._appIds[serverId], nameof(IGameServer.FriendRequest), new RequestArguments(requester, receiver));
    }

    /// <inheritdoc />
    /// <remarks>It's usually never called here, but at <see cref="ServerClients.FriendServer.ForwardLetter"/>.</remarks>
    public void LetterReceived(LetterHeader letter)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.LetterReceived), letter);
    }

    /// <inheritdoc />
    public void FriendOnlineStateChanged(int playerServerId, string player, string friend, int friendServerId)
    {
        // todo: find out if this is correct when logging out
        if (this._appIds.TryGetValue(playerServerId, out var gameServer))
        {
            this._daprClient.InvokeMethodAsync(gameServer, nameof(IGameServer.FriendOnlineStateChanged), new FriendOnlineStateChangedArguments(player, friend, friendServerId));
        }
    }

    /// <inheritdoc />
    public void ChatRoomCreated(int serverId, ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        this._daprClient.InvokeMethodAsync(this._appIds[serverId], nameof(IGameServer.ChatRoomCreated), new ChatRoomCreationArguments(playerAuthenticationInfo, friendName));
    }

    /// <inheritdoc />
    public void InitializeMessenger(int serverId, MessengerInitializationData initializationData)
    {
        if (this._appIds.TryGetValue(serverId, out var gameServer))
        {
            this._daprClient.InvokeMethodAsync(gameServer, nameof(IGameServer.InitializeMessenger), initializationData);
        }
    }
}