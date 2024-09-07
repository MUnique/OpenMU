// <copyright file="IConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using Nito.AsyncEx;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Interface for a connection.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IConnection : IDisposable
{
    /// <summary>
    /// Occurs when a new packet got received.
    /// </summary>
    /// <remarks>
    /// Remove and implement <see cref="IDuplexPipe"/> instead?
    /// </remarks>
    event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;

    /// <summary>
    /// Occurs when the client disconnected.
    /// </summary>
    event AsyncEventHandler? Disconnected;

    /// <summary>
    /// Gets a value indicating whether this <see cref="IConnection"/> is connected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if connected; otherwise, <c>false</c>.
    /// </value>
    bool Connected { get; }

    /// <summary>
    /// Gets the remote endpoint of the connection.
    /// </summary>
    EndPoint? EndPoint { get; }

    /// <summary>
    /// Gets the local endpoint of the connection.
    /// </summary>
    EndPoint? LocalEndPoint { get; }

    /// <summary>
    /// Gets the pipe writer to send data.
    /// </summary>
    /// <value>
    /// The pipe writer.
    /// </value>
    PipeWriter Output { get; }

    /// <summary>
    /// Gets an <see cref="AsyncLock"/> to synchronize writes to the <see cref="Output"/>.
    /// </summary>
    AsyncLock OutputLock { get; }

    /// <summary>
    /// Begins receiving from the client.
    /// </summary>
    /// <returns>The async task.</returns>
    Task BeginReceiveAsync();

    /// <summary>
    /// Disconnects this instance.
    /// </summary>
    ValueTask DisconnectAsync();
}