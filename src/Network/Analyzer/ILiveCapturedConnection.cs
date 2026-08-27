// <copyright file="ILiveCapturedConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// A <see cref="ICapturedConnection"/> which captures the traffic of a connection which is
/// currently handled by one of our servers.
/// </summary>
public interface ILiveCapturedConnection : ICapturedConnection
{
    /// <summary>
    /// Occurs when packets have been added or removed.
    /// </summary>
    event EventHandler? PacketsChanged;

    /// <summary>
    /// Gets the information about the captured connection.
    /// </summary>
    ICapturedConnectionInfo ConnectionInfo { get; }

    /// <summary>
    /// Gets a snapshot of the currently captured packets.
    /// </summary>
    /// <returns>A snapshot of the currently captured packets.</returns>
    /// <remarks>
    /// The packets are captured on the network threads of the connection, so the
    /// <see cref="ICapturedConnection.PacketList"/> must not be enumerated directly.
    /// </remarks>
    IReadOnlyList<Packet> GetPackets();

    /// <summary>
    /// Removes all captured packets.
    /// </summary>
    void Clear();
}
