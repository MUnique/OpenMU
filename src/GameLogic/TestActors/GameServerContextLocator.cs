// <copyright file="GameServerContextLocator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.Interfaces;

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
    public IReadOnlyList<(int ServerId, IGameServerContext Context)> Contexts =>
        this._gameServers
            .Where(pair => pair.Value is IGameServerContextProvider)
            .Select(pair => (pair.Key, ((IGameServerContextProvider)pair.Value).Context))
            .OrderBy(pair => pair.Key)
            .ToList();

    /// <inheritdoc />
    public IGameServerContext? GetContext(int serverId)
    {
        return this._gameServers.TryGetValue(serverId, out var gameServer) && gameServer is IGameServerContextProvider provider
            ? provider.Context
            : null;
    }
}
