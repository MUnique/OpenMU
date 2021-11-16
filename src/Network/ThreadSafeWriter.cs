// <copyright file="ThreadSafeWriter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Threading;

/// <summary>
/// A helper struct to write safely to a <see cref="IConnection.Output" />.
/// </summary>
/// <remarks>
/// We use a readonly ref struct here, so we don't allocate more memory on the stack.
/// </remarks>
public readonly ref struct ThreadSafeWriter
{
    private readonly IConnection _connection;
    private readonly int _expectedPacketSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeWriter" /> struct.
    /// </summary>
    /// <param name="connection">The connection.</param>
    /// <param name="packetType">Type of the packet (e.g. 0xC1).</param>
    /// <param name="expectedPacketSize">Size of the expected packet.</param>
    public ThreadSafeWriter(IConnection connection, byte packetType, int expectedPacketSize)
    {
        this._connection = connection;
        this._expectedPacketSize = expectedPacketSize;
        connection.OutputLock.Wait();
        try
        {
            var span = this.Span;
            span.Clear();
            span[0] = packetType;
            span.SetPacketSize();
        }
        catch (InvalidOperationException)
        {
            connection.OutputLock.Release();
            throw;
        }
    }

    /// <summary>
    /// Gets the span to write at.
    /// </summary>
    public Span<byte> Span => this._connection.Output.GetSpan(this._expectedPacketSize).Slice(0, this._expectedPacketSize);

    /// <summary>
    /// Commits the data of the <see cref="Span"/> with the expected packet size.
    /// </summary>
    public void Commit() => this.Commit(this._expectedPacketSize);

    /// <summary>
    /// Commits the data of the <see cref="Span"/> with specified packet size.
    /// </summary>
    /// <param name="packetSize">Size of the packet.</param>
    public void Commit(int packetSize)
    {
        this._connection.Output.AdvanceSafely(packetSize);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this._connection.OutputLock.Release();
    }
}