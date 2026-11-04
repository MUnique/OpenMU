// <copyright file="PacketCaptureTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Tests for the packet capturing of a <see cref="Connection"/>.
/// </summary>
[TestFixture]
public class PacketCaptureTest
{
    /// <summary>
    /// Tests if a received data packet is reported to a registered sink.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ReceivedPacketIsCapturedAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        var duplexPipe = new DuplexPipe();
        using var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);
        _ = connection.BeginReceiveAsync();

        await duplexPipe.ReceivePipe.Writer.WriteAsync(packet).ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(1));
        Assert.That(captured[0].Packet, Is.EqualTo(packet));
        Assert.That(captured[0].Sent, Is.False);
    }

    /// <summary>
    /// Tests if a sent data packet is reported to a registered sink.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SentPacketIsCapturedAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(1));
        Assert.That(captured[0].Packet, Is.EqualTo(packet));
        Assert.That(captured[0].Sent, Is.True);
    }

    /// <summary>
    /// Tests if a data packet which is written in more than one chunk is reported as one packet.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task FragmentedPacketIsCapturedAsOnePacketAsync()
    {
        var packet = new byte[] { 0xC1, 0x06, 0xF1, 0x00, 0x11, 0x22 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        Write(connection, packet.AsSpan(0, 3));
        await connection.Output.FlushAsync().ConfigureAwait(false);
        Assert.That(sink.Snapshot(), Is.Empty, "The packet is not complete yet.");

        Write(connection, packet.AsSpan(3));
        await connection.Output.FlushAsync().ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(1));
        Assert.That(captured[0].Packet, Is.EqualTo(packet));
    }

    /// <summary>
    /// Tests if more than one data packet which is written at once is reported as separate packets.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task MultiplePacketsInOneWriteAreCapturedSeparatelyAsync()
    {
        var first = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        var second = new byte[] { 0xC1, 0x03, 0xF3 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        await connection.Output.WriteAsync(first.Concat(second).ToArray()).ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(2).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(2));
        Assert.That(captured[0].Packet, Is.EqualTo(first));
        Assert.That(captured[1].Packet, Is.EqualTo(second));
    }

    /// <summary>
    /// Tests if a packet with a two byte length header (C2) is captured with its correct length.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task BigPacketIsCapturedAsync()
    {
        var packet = new byte[300];
        packet[0] = 0xC2;
        packet[1] = 0x01;
        packet[2] = 0x2C;
        packet[3] = 0xF1;
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(1));
        Assert.That(captured[0].Packet, Has.Length.EqualTo(300));
        Assert.That(captured[0].Packet, Is.EqualTo(packet));
    }

    /// <summary>
    /// Tests if an incomplete data packet is only reported as soon as the rest is written,
    /// even when a complete packet was written before it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task IncompleteTrailingPacketIsCapturedWhenCompletedAsync()
    {
        var complete = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        var trailing = new byte[] { 0xC1, 0x04, 0xF3, 0x01 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        await connection.Output.WriteAsync(complete.Concat(trailing.Take(2)).ToArray()).ConfigureAwait(false);
        var captured = await sink.WaitForPacketsAsync(1).ConfigureAwait(false);
        Assert.That(captured, Has.Count.EqualTo(1), "Only the complete packet should be reported.");

        await connection.Output.WriteAsync(trailing.AsMemory(2)).ConfigureAwait(false);
        captured = await sink.WaitForPacketsAsync(2).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(2));
        Assert.That(captured[1].Packet, Is.EqualTo(trailing));
    }

    /// <summary>
    /// Tests if all registered sinks get the data packets, and that a removed sink doesn't
    /// get them anymore.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task AllRegisteredSinksAreNotifiedAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        using var connection = CreateConnection();
        var first = new CapturingSink();
        var second = new CapturingSink();
        connection.AddCaptureSink(first);
        connection.AddCaptureSink(second);
        connection.AddCaptureSink(first); // registering twice should have no effect

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        await first.WaitForPacketsAsync(1).ConfigureAwait(false);
        await second.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(first.Snapshot(), Has.Count.EqualTo(1));
        Assert.That(second.Snapshot(), Has.Count.EqualTo(1));

        connection.RemoveCaptureSink(first);
        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        await second.WaitForPacketsAsync(2).ConfigureAwait(false);

        Assert.That(first.Snapshot(), Has.Count.EqualTo(1), "The removed sink should not get further packets.");
        Assert.That(second.Snapshot(), Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests if nothing is captured anymore after the last sink has been removed.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task NothingIsCapturedAfterLastSinkWasRemovedAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);
        connection.RemoveCaptureSink(sink);

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        await Task.Delay(50).ConfigureAwait(false);

        Assert.That(sink.Snapshot(), Is.Empty);
    }

    /// <summary>
    /// Tests if the capturing can be started again after it has been stopped, because the
    /// last sink was removed.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task CapturingCanBeRestartedAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        using var connection = CreateConnection();
        var sink = new CapturingSink();

        connection.AddCaptureSink(sink);
        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        await sink.WaitForPacketsAsync(1).ConfigureAwait(false);
        Assert.That(sink.Snapshot(), Has.Count.EqualTo(1));

        connection.RemoveCaptureSink(sink);
        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        await Task.Delay(50).ConfigureAwait(false);
        Assert.That(sink.Snapshot(), Has.Count.EqualTo(1), "Nothing should be captured without a sink.");

        var secondSink = new CapturingSink();
        connection.AddCaptureSink(secondSink);
        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        var captured = await secondSink.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(captured, Has.Count.EqualTo(1));
        Assert.That(captured[0].Packet, Is.EqualTo(packet));
    }

    /// <summary>
    /// Tests if the written data still arrives at the target pipe unchanged, with and without
    /// a registered capture sink.
    /// </summary>
    /// <param name="withSink">If set to <c>true</c>, a capture sink is registered.</param>
    /// <returns>The async task.</returns>
    [TestCase(true)]
    [TestCase(false)]
    public async Task WrittenDataIsForwardedUnchangedAsync(bool withSink)
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        var duplexPipe = new DuplexPipe();
        using var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        if (withSink)
        {
            connection.AddCaptureSink(new CapturingSink());
        }

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);

        var result = await duplexPipe.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        Assert.That(result.Buffer.ToArray(), Is.EqualTo(packet));
    }

    /// <summary>
    /// Tests if an exception of a sink doesn't bubble up to the connection.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ExceptionInSinkIsCaughtAsync()
    {
        var packet = new byte[] { 0xC1, 0x04, 0xF1, 0x00 };
        using var connection = CreateConnection();
        var working = new CapturingSink();
        connection.AddCaptureSink(new ThrowingSink());
        connection.AddCaptureSink(working);

        await connection.Output.WriteAsync(packet).ConfigureAwait(false);
        var captured = await working.WaitForPacketsAsync(1).ConfigureAwait(false);

        Assert.That(connection.Connected, Is.True);
        Assert.That(captured, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests if malformed captured data stops the capturing without affecting the connection.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task MalformedDataStopsCapturingWithoutBreakingTheConnectionAsync()
    {
        var malformed = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        using var connection = CreateConnection();
        var sink = new CapturingSink();
        connection.AddCaptureSink(sink);

        await connection.Output.WriteAsync(malformed).ConfigureAwait(false);
        await Task.Delay(50).ConfigureAwait(false);

        Assert.That(sink.Snapshot(), Is.Empty);
        Assert.That(connection.Connected, Is.True);

        // The connection still works, it's just not captured anymore.
        await connection.Output.WriteAsync(new byte[] { 0xC1, 0x04, 0xF1, 0x00 }).ConfigureAwait(false);
        Assert.That(connection.Connected, Is.True);
    }

    /// <summary>
    /// Tests if each connection has its own identifier.
    /// </summary>
    [Test]
    public void EachConnectionHasItsOwnId()
    {
        using var first = CreateConnection();
        using var second = CreateConnection();

        Assert.That(first.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(first.Id, Is.Not.EqualTo(second.Id));
    }

    private static Connection CreateConnection()
    {
        return new Connection(new DuplexPipe(), null, null, new NullLogger<Connection>());
    }

    private static void Write(Connection connection, ReadOnlySpan<byte> data)
    {
        var span = connection.Output.GetSpan(data.Length);
        data.CopyTo(span);
        connection.Output.Advance(data.Length);
    }

    private sealed class CapturingSink : IPacketCaptureSink
    {
        private readonly List<(byte[] Packet, bool Sent)> _captured = new();

        public void PacketCaptured(ReadOnlySpan<byte> packet, bool sent)
        {
            var entry = (packet.ToArray(), sent);
            lock (this._captured)
            {
                this._captured.Add(entry);
            }
        }

        public IList<(byte[] Packet, bool Sent)> Snapshot()
        {
            lock (this._captured)
            {
                return this._captured.ToList();
            }
        }

        public async Task<IList<(byte[] Packet, bool Sent)>> WaitForPacketsAsync(int count)
        {
            for (int i = 0; i < 100; i++)
            {
                var snapshot = this.Snapshot();
                if (snapshot.Count >= count)
                {
                    return snapshot;
                }

                await Task.Delay(10).ConfigureAwait(false);
            }

            return this.Snapshot();
        }
    }

    private sealed class ThrowingSink : IPacketCaptureSink
    {
        public void PacketCaptured(ReadOnlySpan<byte> packet, bool sent)
        {
            throw new InvalidOperationException("This sink is broken.");
        }
    }
}
