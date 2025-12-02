// <copyright file="MapEventStateUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IMapEventStateUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(MapEventStateUpdatePlugIn), "The default implementation of the IMapEventStateUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("8A34C69D-59CC-4251-9E8E-D80154A7AC8C")]
public class MapEventStateUpdatePlugIn : IMapEventStateUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapEventStateUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public MapEventStateUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync(bool enabled, MapEventType mapEventType)
    {
        await this._player.Connection.SendMapEventStateAsync(enabled, Convert(mapEventType)).ConfigureAwait(false);
    }

    private static MapEventState.Events Convert(MapEventType eventType)
    {
        return eventType switch
        {
            MapEventType.RedDragonInvasion => MapEventState.Events.RedDragon,
            MapEventType.GoldenDragonInvasion => MapEventState.Events.GoldenDragon,
            _ => throw new ArgumentException($"Unknown map's event type {eventType}"),
        };
    }
}