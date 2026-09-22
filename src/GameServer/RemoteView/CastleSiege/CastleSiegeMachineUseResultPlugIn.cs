// <copyright file="CastleSiegeMachineUseResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using MachinePacketType = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeMachineType;
using MachineType = MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeMachineType;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeMachineUseResultPlugIn"/>
/// which forwards warfare-machine shots to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineUseResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineUseResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7BC6B9B3-C5C3-4627-A5F9-836589126E54")]
public class CastleSiegeMachineUseResultPlugIn : ICastleSiegeMachineUseResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineUseResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeMachineUseResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowMachineUseResultAsync(
        bool success,
        ushort machineId,
        MachineType machineType,
        Point target)
        => this._player.Connection?.SendCastleSiegeMachineUseResultAsync(
            success ? (byte)1 : (byte)0,
            machineId,
            (MachinePacketType)machineType,
            target.X,
            target.Y) ?? default;
}
