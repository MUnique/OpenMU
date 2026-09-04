// <copyright file="ArchivedSessionMetadata.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The metadata of an archived session, which is saved next to the captured packets.
/// </summary>
/// <remarks>
/// The packets themselves are saved in the same format as the capture files of the analyzer
/// tool, which has no place for this kind of information.
/// </remarks>
public sealed class ArchivedSessionMetadata
{
    /// <summary>
    /// Gets or sets the name of the observed account.
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the names of the characters which were played during the session.
    /// </summary>
    public IList<string> CharacterNames { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the type of the server which handled the connection.
    /// </summary>
    public ServerType ServerType { get; set; } = ServerType.GameServer;

    /// <summary>
    /// Gets or sets the identifier of the server which handled the connection.
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// Gets or sets the description of the server which handled the connection.
    /// </summary>
    public string? ServerDescription { get; set; }

    /// <summary>
    /// Gets or sets the remote endpoint of the connection.
    /// </summary>
    public string? RemoteEndPoint { get; set; }

    /// <summary>
    /// Gets or sets the client version which applied to the connection.
    /// </summary>
    /// <remarks>
    /// The archive starts when the player is logged in, so the version is already the one
    /// which the client reported at the version check - it doesn't change afterwards.
    /// </remarks>
    public ClientVersion ClientVersion { get; set; }

    /// <summary>
    /// Gets or sets the point in time (UTC) at which the session started.
    /// </summary>
    public DateTime StartTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the point in time (UTC) at which the session ended. It's <c>null</c> as
    /// long as it's running - and stays <c>null</c> when the server process died.
    /// </summary>
    public DateTime? EndTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the number of packets which were archived.
    /// </summary>
    public long PacketCount { get; set; }

    /// <summary>
    /// Gets or sets the number of packets which had to be dropped, because they arrived
    /// faster than they could be written.
    /// </summary>
    public long DroppedPacketCount { get; set; }

    /// <summary>
    /// Gets or sets the names of the files which contain the packets of this session, in the
    /// order in which they were written.
    /// </summary>
    public IList<string> Parts { get; set; } = new List<string>();
}
