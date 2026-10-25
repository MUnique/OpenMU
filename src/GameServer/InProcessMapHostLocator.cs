// <copyright file="InProcessMapHostLocator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Locates the hosts of maps between the game servers which run in the same process,
/// as in the all-in-one deployment.
/// </summary>
/// <remarks>
/// The located host provides its own map instance, so a player of another game server can simply
/// enter it - without reconnecting its game client to the hosting game server. The player stays
/// connected to its own game server, which still handles its packets, persistence, guild and friends.
/// </remarks>
public sealed class InProcessMapHostLocator : IMapHostLocator
{
    private readonly IDictionary<int, IGameServer> _gameServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="InProcessMapHostLocator"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers of this process, by their id.</param>
    public InProcessMapHostLocator(IDictionary<int, IGameServer> gameServers)
    {
        this._gameServers = gameServers;
    }

    /// <inheritdoc />
    public async ValueTask<MapHost?> LocateAsync(IGameServerContext requester, ushort mapNumber)
    {
        // The game server with the lowest id wins, so that all game servers agree on the same
        // host when more than one of them hosts the map.
        var candidates = this._gameServers.Values
            .Where(server => server.Id != requester.Id && server.ServerState == ServerState.Started)
            .OfType<IGameServerContextProvider>()
            .Select(provider => provider.Context)
            .OrderBy(context => context.Id)
            .ToList();

        foreach (var context in candidates)
        {
            if (await context.GetMapAsync(mapNumber).ConfigureAwait(false) is { } map)
            {
                return new MapHost(context.Id, map);
            }
        }

        return null;
    }
}
