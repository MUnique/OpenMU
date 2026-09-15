// <copyright file="ActorEventLog.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using System.Threading;
using System.Threading.Channels;

/// <summary>
/// The event stream of one <see cref="ScriptedPlayer"/>: a bounded ring of the most recent events
/// plus a fan-out to live followers.
/// </summary>
/// <remarks>
/// Every writer is a game thread (a view call, the hit plugin point, a command), so appending must
/// never block and never wait for a reader. A follower which does not keep up therefore loses the
/// oldest events of its own channel and is told about it with a <c>lag</c> event instead of stalling
/// the game. The ring itself is unaffected by slow readers.
/// </remarks>
public sealed class ActorEventLog
{
    /// <summary>
    /// The number of events kept per actor. The specification asks for at least 1000; 4096 gives a
    /// scenario room to run for a while before it has to read.
    /// </summary>
    public const int DefaultCapacity = 4096;

    /// <summary>
    /// The number of events a live follower may fall behind before it starts losing the oldest ones.
    /// </summary>
    public const int DefaultSubscriptionCapacity = 1024;

    private readonly object _syncRoot = new();
    private readonly ActorEvent?[] _ring;
    private readonly List<ActorEventSubscription> _subscriptions = new();

    private long _nextSequence = 1;
    private int _count;
    private int _nextIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActorEventLog"/> class.
    /// </summary>
    /// <param name="capacity">The number of events to keep; defaults to <see cref="DefaultCapacity"/>.</param>
    public ActorEventLog(int capacity = DefaultCapacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "The event log needs room for at least one event.");
        }

        this._ring = new ActorEvent?[capacity];
    }

    /// <summary>
    /// Gets the number of events this log keeps.
    /// </summary>
    public int Capacity => this._ring.Length;

    /// <summary>
    /// Gets the sequence number of the most recently assigned event, or 0 when nothing happened yet.
    /// </summary>
    public long LastSequence
    {
        get
        {
            lock (this._syncRoot)
            {
                return this._nextSequence - 1;
            }
        }
    }

    /// <summary>
    /// Appends an event to the log and hands it to every live follower.
    /// </summary>
    /// <param name="type">The event type.</param>
    /// <param name="fields">The fields of the event.</param>
    /// <returns>The appended event, including the sequence number it was given.</returns>
    public ActorEvent Append(string type, params ActorEventField[] fields)
    {
        lock (this._syncRoot)
        {
            // Lag events are created (and therefore numbered) BEFORE the event they precede, so a
            // follower's stream stays strictly increasing.
            foreach (var subscription in this._subscriptions)
            {
                var dropped = subscription.TakeDropCount();
                if (dropped > 0)
                {
                    subscription.Write(this.CreateEvent("lag", [new ActorEventField("dropped", dropped)]));
                }
            }

            var appended = this.CreateEvent(type, fields);
            this._ring[this._nextIndex] = appended;
            this._nextIndex = (this._nextIndex + 1) % this._ring.Length;
            if (this._count < this._ring.Length)
            {
                this._count++;
            }

            foreach (var subscription in this._subscriptions)
            {
                subscription.Write(appended);
            }

            return appended;
        }
    }

    /// <summary>
    /// Gets the kept events which are newer than the given sequence number.
    /// </summary>
    /// <param name="sequence">The last sequence number the reader has already seen; 0 returns everything kept.</param>
    /// <returns>The matching events, oldest first.</returns>
    public IReadOnlyList<ActorEvent> Since(long sequence)
    {
        lock (this._syncRoot)
        {
            return this.SnapshotSince(sequence);
        }
    }

    /// <summary>
    /// Subscribes to the live stream of this log.
    /// </summary>
    /// <param name="sinceSequence">
    /// When given, the kept events newer than this sequence number are delivered first, so a follower
    /// misses nothing between reading the history and subscribing.
    /// </param>
    /// <param name="capacity">The follower's buffer size; defaults to <see cref="DefaultSubscriptionCapacity"/>.</param>
    /// <returns>The subscription; disposing it ends the stream.</returns>
    public ActorEventSubscription Subscribe(long? sinceSequence = null, int capacity = DefaultSubscriptionCapacity)
    {
        var subscription = new ActorEventSubscription(this, capacity);
        lock (this._syncRoot)
        {
            if (sinceSequence is { } since)
            {
                foreach (var backlogEvent in this.SnapshotSince(since))
                {
                    subscription.Write(backlogEvent);
                }
            }

            this._subscriptions.Add(subscription);
        }

        return subscription;
    }

    /// <summary>
    /// Removes a subscription; called by <see cref="ActorEventSubscription.Dispose"/>.
    /// </summary>
    /// <param name="subscription">The subscription to remove.</param>
    internal void Unsubscribe(ActorEventSubscription subscription)
    {
        lock (this._syncRoot)
        {
            this._subscriptions.Remove(subscription);
        }
    }

    private ActorEvent CreateEvent(string type, IReadOnlyList<ActorEventField> fields)
    {
        // Lag events take a sequence number from the same counter but are not kept in the ring: they
        // belong to one follower's stream, not to the actor's history. A reader therefore sees
        // strictly increasing numbers, with a gap where a lag event was.
        return new ActorEvent(this._nextSequence++, DateTime.UtcNow, type, fields);
    }

    private IReadOnlyList<ActorEvent> SnapshotSince(long sequence)
    {
        var result = new List<ActorEvent>(this._count);
        var start = (this._nextIndex - this._count + this._ring.Length) % this._ring.Length;
        for (var i = 0; i < this._count; i++)
        {
            if (this._ring[(start + i) % this._ring.Length] is { } kept && kept.Seq > sequence)
            {
                result.Add(kept);
            }
        }

        return result;
    }
}
