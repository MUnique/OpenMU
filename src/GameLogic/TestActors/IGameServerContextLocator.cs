// <copyright file="IGameServerContextLocator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Resolves the game server contexts which run in this process.
/// </summary>
public interface IGameServerContextLocator
{
    /// <summary>
    /// Gets the context of the game server with the given id.
    /// </summary>
    /// <param name="serverId">The server id.</param>
    /// <returns>The context, or <c>null</c> when this process does not host that server.</returns>
    IGameServerContext? GetContext(int serverId);

    /// <summary>
    /// Gets every game server context of this process, by server id.
    /// </summary>
    /// <returns>The contexts.</returns>
    IReadOnlyList<(int ServerId, IGameServerContext Context)> GetContexts();
}

/// <summary>
/// Resolves the contexts from the game servers of the all-in-one host, which implement
/// <see cref="IGameServerContextProvider"/>.
/// </summary>
public sealed class GameServerContextLocator : IGameServerContextLocator
{
    private readonly IDictionary<int, IGameServer> _gameServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerContextLocator"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers of this process.</param>
    public GameServerContextLocator(IDictionary<int, IGameServer> gameServers)
    {
        this._gameServers = gameServers;
    }

    /// <inheritdoc />
    public IGameServerContext? GetContext(int serverId)
    {
        return this._gameServers.TryGetValue(serverId, out var gameServer) && gameServer is IGameServerContextProvider provider
            ? provider.Context
            : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<(int ServerId, IGameServerContext Context)> GetContexts()
    {
        return this._gameServers
            .Where(pair => pair.Value is IGameServerContextProvider)
            .Select(pair => (pair.Key, ((IGameServerContextProvider)pair.Value).Context))
            .OrderBy(pair => pair.Key)
            .ToList();
    }
}
