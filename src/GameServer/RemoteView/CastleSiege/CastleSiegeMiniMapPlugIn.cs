// <copyright file="CastleSiegeMiniMapPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeMiniMapPlugIn"/>
/// which forwards mini-map position data to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMiniMapPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMiniMapPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("CDD72871-1133-4546-9E53-6E1BB00D4441")]
public class CastleSiegeMiniMapPlugIn : ICastleSiegeMiniMapPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMiniMapPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeMiniMapPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowPlayerPositionsAsync(IReadOnlyList<CastleSiegeMiniMapPlayerInfo> players)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = CastleSiegeMiniMapPlayerPositionsRef.GetRequiredSize(players.Count);
            var packet = new CastleSiegeMiniMapPlayerPositionsRef(connection.Output.GetSpan(size)[..size])
            {
                PlayerCount = checked((uint)players.Count),
            };

            for (var i = 0; i < players.Count; i++)
            {
                var entry = packet[i];
                entry.PositionX = players[i].PositionX;
                entry.PositionY = players[i].PositionY;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowNpcPositionsAsync(IReadOnlyList<CastleSiegeMiniMapNpcInfo> npcs)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        int Write()
        {
            var size = CastleSiegeMiniMapNpcPositionsRef.GetRequiredSize(npcs.Count);
            var packet = new CastleSiegeMiniMapNpcPositionsRef(connection.Output.GetSpan(size)[..size])
            {
                NpcCount = checked((byte)npcs.Count),
            };

            for (var i = 0; i < npcs.Count; i++)
            {
                var entry = packet[i];
                entry.NpcType = npcs[i].IsGate ? CastleSiegeMiniMapNpcType.Gate : CastleSiegeMiniMapNpcType.GuardianStatue;
                entry.PositionX = npcs[i].PositionX;
                entry.PositionY = npcs[i].PositionY;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
