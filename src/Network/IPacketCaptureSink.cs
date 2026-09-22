// <copyright file="IPacketCaptureSink.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

/// <summary>
/// A sink which gets the decrypted data packets of a connection, as long as it's registered
/// at <see cref="IConnection.AddCaptureSink"/>.
/// </summary>
/// <remarks>
/// This allows to watch the traffic of a connection without an external proxy, e.g. to show
/// it in the admin panel. As long as no sink is registered at a connection, it doesn't
/// capture anything.
/// </remarks>
public interface IPacketCaptureSink
{
    /// <summary>
    /// Is called when a complete, decrypted data packet was received from or sent to the
    /// remote endpoint of the connection.
    /// </summary>
    /// <param name="packet">The complete, decrypted data packet.</param>
    /// <param name="sent"><see langword="true"/>, if the packet was sent to the remote endpoint;
    /// <see langword="false"/>, if it was received from it.</param>
    /// <remarks>
    /// This is called on the network thread of the connection, so an implementation should
    /// return as fast as possible. The <paramref name="packet"/> is only valid during the call,
    /// so it must be copied if it's required afterwards.
    /// </remarks>
    void PacketCaptured(ReadOnlySpan<byte> packet, bool sent);
}
