// <copyright file="ObjectMovedPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The implementation of the <see cref="IObjectMovedPlugIn"/> for version 0.75 which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ObjectMovedPlugIn 0.75", "The default implementation of the IObjectMovedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("3B387A61-F9E2-4866-BBD6-F236582E350A")]
public class ObjectMovedPlugIn075 : ObjectMovedPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectMovedPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ObjectMovedPlugIn075(RemotePlayer player)
        : base(player)
    {
    }

    /// <inheritdoc />
    protected override ValueTask SendWalkAsync(IConnection connection, ushort objectId, Point sourcePoint, Point targetPoint, Memory<Direction> steps, Direction rotation, int stepsLength)
    {
        return connection.SendObjectWalked075Async(objectId, targetPoint.X, targetPoint.Y, rotation.ToPacketByte());
    }
}