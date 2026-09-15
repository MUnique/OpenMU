// <copyright file="ActorEventSubscription.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Threading;
using System.Threading.Channels;

/// <summary>
/// A live follower of an <see cref="ActorEventLog"/>.
/// </summary>
public sealed class ActorEventSubscription : IDisposable
{
    private readonly ActorEventLog _log;
    private readonly Channel<ActorEvent> _channel;

    private int _dropCount;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorEventSubscription"/> class.
    /// </summary>
    /// <param name="log">The log this subscription belongs to.</param>
    /// <param name="capacity">The buffer size before the oldest events are dropped.</param>
    internal ActorEventSubscription(ActorEventLog log, int capacity)
    {
        this._log = log;
        this._channel = Channel.CreateBounded<ActorEvent>(
            new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false,
            },
            _ => Interlocked.Increment(ref this._dropCount));
    }

    /// <summary>
    /// Gets the reader of the live stream.
    /// </summary>
    public ChannelReader<ActorEvent> Reader => this._channel.Reader;

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        this._disposed = true;
        this._log.Unsubscribe(this);
        this._channel.Writer.TryComplete();
    }

    /// <summary>
    /// Hands an event to this follower. Never blocks: when the buffer is full, the oldest event is
    /// dropped and counted, so the next append is preceded by a <c>lag</c> event.
    /// </summary>
    /// <param name="actorEvent">The event to deliver.</param>
    internal void Write(ActorEvent actorEvent)
    {
        this._channel.Writer.TryWrite(actorEvent);
    }

    /// <summary>
    /// Reads and resets the number of events dropped since the last call.
    /// </summary>
    /// <returns>The number of dropped events.</returns>
    internal int TakeDropCount()
    {
        return Interlocked.Exchange(ref this._dropCount, 0);
    }
}
