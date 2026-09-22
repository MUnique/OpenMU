// <copyright file="CastleSiegeMachineRegionNotifyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MachineType = MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeMachineType;
using MachinePacketType = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeMachineType;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeMachineRegionNotifyPlugIn"/>
/// which forwards warfare-machine impact regions to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineRegionNotifyPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineRegionNotifyPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9056C2A5-7506-47D7-B248-FAB73E5CAB02")]
public class CastleSiegeMachineRegionNotifyPlugIn : ICastleSiegeMachineRegionNotifyPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineRegionNotifyPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeMachineRegionNotifyPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowMachineRegionAsync(MachineType machineType, Point target)
        => this._player.Connection?.SendCastleSiegeMachineRegionNotifyAsync(
            (MachinePacketType)machineType,
            target.X,
            target.Y) ?? default;
}
