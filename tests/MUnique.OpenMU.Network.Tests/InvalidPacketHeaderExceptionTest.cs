// <copyright file="InvalidPacketHeaderExceptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using System.IO.Pipelines;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Tests if <see cref="InvalidPacketHeaderException"/> are thrown when malformed data is read by a <see cref="PacketPipeReaderBase"/>.
/// </summary>
[TestFixture]
public class InvalidPacketHeaderExceptionTest
{
    private readonly byte[] _malformedData = { 0xC1, 0x03, 0xFF, 0x00, 0x00, 0x00 };

    /// <summary>
    /// Tests if the exception is thrown.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ThrownAsync()
    {
        await this.TestExceptionAsync(e => { });
    }

    /// <summary>
    /// Tests if <see cref="InvalidPacketHeaderException.Header"/> is assigned correctly.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task TestHeaderAsync()
    {
        await this.TestExceptionAsync(e => Assert.That(e.Header, Is.EquivalentTo(new byte[] { 0x00, 0x00, 0x00 })));
    }

    /// <summary>
    /// Tests if <see cref="InvalidPacketHeaderException.Position"/> is assigned correctly.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task TestPositionAsync()
    {
        await this.TestExceptionAsync(e => Assert.That(e.Position, Is.EqualTo(3)));
    }

    /// <summary>
    /// Tests if <see cref="InvalidPacketHeaderException.BufferContent"/> is assigned correctly.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task TestBufferContentAsync()
    {
        await this.TestExceptionAsync(e => Assert.That(e.BufferContent, Is.EquivalentTo(this._malformedData)));
    }

    private async ValueTask TestExceptionAsync(Action<InvalidPacketHeaderException> check)
    {
        bool thrown = false;
        var duplexPipe = new DuplexPipe(new PipeOptions(pauseWriterThreshold: 1, resumeWriterThreshold: 1));
        using var connection = new Connection(duplexPipe, null, new Xor.PipelinedXor32Encryptor(duplexPipe.Output), new NullLogger<Connection>());
        _ = connection.BeginReceiveAsync();

        try
        {
            _ = await duplexPipe.ReceivePipe.Writer.WriteAsync(this._malformedData).ConfigureAwait(false);
        }
        catch (InvalidPacketHeaderException e)
        {
            thrown = true;
            check(e);
        }
        catch (Exception e)
        {
            Assert.Fail($"Wrong exception type {e}", e);
        }

        Assert.That(thrown);
    }
}