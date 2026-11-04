// <copyright file="MapChangedExtendedConnectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Sends the <see cref="MapChangedExtended"/> packet of the extended client.
/// </summary>
internal static class MapChangedExtendedConnectionExtensions
{
    /// <summary>
    /// Sends a <see cref="MapChangedExtended"/> to this connection, with its unused bytes 10 to 14 written as 0.
    /// </summary>
    /// <remarks>
    /// The generated sender writes only the fields, so the unused bytes would keep what an earlier packet
    /// left in the reused output buffer.
    /// </remarks>
    /// <param name="connection">The connection.</param>
    /// <param name="mapNumber">The map number.</param>
    /// <param name="positionX">The position x.</param>
    /// <param name="positionY">The position y.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="mapDiscriminator">The discriminator of the map, e.g. the stage of Devil Square.</param>
    /// <param name="isMapChange">If false, the client shows the teleport animation and keeps the objects in its scope.</param>
    /// <returns>The task.</returns>
    public static async ValueTask SendClearedMapChangedExtendedAsync(this IConnection? connection, ushort mapNumber, byte positionX, byte positionY, byte rotation, byte mapDiscriminator, bool isMapChange)
    {
        if (connection is null)
        {
            return;
        }

        int WritePacket()
        {
            var length = MapChangedExtendedRef.Length;
            var span = connection.Output.GetSpan(length)[..length];
            span.Clear();
            _ = new MapChangedExtendedRef(span)
            {
                IsMapChange = isMapChange,
                MapNumber = mapNumber,
                PositionX = positionX,
                PositionY = positionY,
                Rotation = rotation,
                MapDiscriminator = mapDiscriminator,
            };

            return length;
        }

        await connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}
