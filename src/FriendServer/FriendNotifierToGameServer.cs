// <copyright file="FriendNotifierToGameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.FriendServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of a <see cref="IFriendNotifier"/> which forwards the calls to the available game server instances.
/// </summary>
public class FriendNotifierToGameServer : IFriendNotifier
{
    private readonly IDictionary<int, IGameServer> _gameServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendNotifierToGameServer"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    public FriendNotifierToGameServer(IDictionary<int, IGameServer> gameServers)
    {
        this._gameServers = gameServers;
    }

    /// <inheritdoc />
    public async ValueTask FriendRequestAsync(string requester, string receiver, int serverId)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            await gameServer.FriendRequestAsync(requester, receiver).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask LetterReceivedAsync(LetterHeader letter)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            await gameServer.LetterReceivedAsync(letter).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask FriendOnlineStateChangedAsync(int playerServerId, string player, string friend, int friendServerId)
    {
        if (this._gameServers.TryGetValue(playerServerId, out var gameServer))
        {
            await gameServer.FriendOnlineStateChangedAsync(player, friend, friendServerId).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ChatRoomCreatedAsync(int serverId, ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            await gameServer.ChatRoomCreatedAsync(playerAuthenticationInfo, friendName).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask InitializeMessengerAsync(int serverId, MessengerInitializationData initializationData)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            await gameServer.InitializeMessengerAsync(initializationData).ConfigureAwait(false);
        }
    }
}