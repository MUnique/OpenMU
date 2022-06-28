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
    public void FriendRequest(string requester, string receiver, int serverId)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            gameServer.FriendRequest(requester, receiver);
        }
    }

    /// <inheritdoc />
    public void LetterReceived(LetterHeader letter)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            gameServer.LetterReceived(letter);
        }
    }

    /// <inheritdoc />
    public void FriendOnlineStateChanged(int playerServerId, string player, string friend, int friendServerId)
    {
        if (this._gameServers.TryGetValue(playerServerId, out var gameServer))
        {
            gameServer.FriendOnlineStateChanged(player, friend, friendServerId);
        }
    }

    /// <inheritdoc />
    public void ChatRoomCreated(int serverId, ChatServerAuthenticationInfo playerAuthenticationInfo, string friendName)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            gameServer.ChatRoomCreated(playerAuthenticationInfo, friendName);
        }
    }

    /// <inheritdoc />
    public void InitializeMessenger(int serverId, MessengerInitializationData initializationData)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            gameServer.InitializeMessenger(initializationData);
        }
    }
}