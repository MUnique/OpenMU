// <copyright file="ArchivedSession.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using System.ComponentModel;
using System.Globalization;
using System.IO;

/// <summary>
/// An archived session of an observed account, which is read from the archive.
/// </summary>
/// <remarks>
/// The files can be read while the session is still running: they are opened without blocking
/// the writer, and a line which isn't completely written yet is simply skipped.
/// </remarks>
public sealed class ArchivedSession : ICapturedConnection
{
    /// <summary>
    /// The minimum size of a data packet: a type byte, the length and the code.
    /// </summary>
    private const int MinimumPacketSize = 3;

    private ArchivedSession(ArchivedSessionInfo info, BindingList<Packet> packets)
    {
        this.Info = info;
        this.PacketList = packets;
        this.Name = info.DisplayName;
        this.StartTimestamp = info.Metadata.StartTimestamp;
    }

    /// <summary>
    /// Gets the information about the session.
    /// </summary>
    public ArchivedSessionInfo Info { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public BindingList<Packet> PacketList { get; }

    /// <inheritdoc />
    public DateTime StartTimestamp { get; }

    /// <summary>
    /// Loads the packets of the specified session.
    /// </summary>
    /// <param name="info">The information about the session.</param>
    /// <param name="maximumPacketCount">The maximum number of packets which are loaded. When
    /// the session contains more, only the newest ones are returned.</param>
    /// <returns>The loaded session.</returns>
    public static async ValueTask<ArchivedSession> LoadAsync(ArchivedSessionInfo info, int maximumPacketCount)
    {
        var packets = new List<Packet>();
        foreach (var part in info.Metadata.Parts)
        {
            var path = Path.Combine(info.DirectoryPath, part);
            if (!File.Exists(path))
            {
                continue;
            }

            await ReadPartAsync(path, packets).ConfigureAwait(false);
        }

        if (maximumPacketCount > 0 && packets.Count > maximumPacketCount)
        {
            packets.RemoveRange(0, packets.Count - maximumPacketCount);
        }

        return new ArchivedSession(info, new BindingList<Packet>(packets));
    }

    private static async ValueTask ReadPartAsync(string path, List<Packet> packets)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream);

        // The first line is the timestamp of the session, which is already in the metadata.
        _ = await reader.ReadLineAsync().ConfigureAwait(false);
        while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
        {
            if (TryParsePacket(line, out var packet))
            {
                packets.Add(packet);
            }
        }
    }

    private static bool TryParsePacket(string line, out Packet packet)
    {
        packet = default!;

        // The fifth field is the sequence number of the packet, which is only used to see
        // whether packets are missing - the analyzer tool ignores it.
        var fields = line.Split(';');
        if (fields.Length < 4
            || !long.TryParse(fields[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
            || !bool.TryParse(fields[1], out var toServer)
            || !int.TryParse(fields[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var size)
            || size < MinimumPacketSize)
        {
            return false;
        }

        try
        {
            if (!CapturedConnectionExtensions.TryParseArray(fields[3], out var data) || data.Length != size)
            {
                return false;
            }

            packet = new Packet(new TimeSpan(ticks), data, toServer);
            return true;
        }
        catch (Exception)
        {
            // The line isn't completely written yet, or it's not a packet at all.
            return false;
        }
    }
}
