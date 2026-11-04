// <copyright file="ActorEventLogTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.TestActors;

using System.Threading.Tasks;
using System.Threading;
using MUnique.OpenMU.GameLogic.TestActors;

/// <summary>
/// Tests for the bounded event ring and its live fan-out.
/// </summary>
[TestFixture]
public class ActorEventLogTests
{
    /// <summary>
    /// Events get strictly increasing sequence numbers and a UTC timestamp.
    /// </summary>
    [Test]
    public void AppendAssignsIncreasingSequenceNumbers()
    {
        var log = new ActorEventLog(16);

        var first = log.Append("chat", new ActorEventField("message", "hello"));
        var second = log.Append("chat", new ActorEventField("message", "world"));

        Assert.That(first.Seq, Is.EqualTo(1));
        Assert.That(second.Seq, Is.GreaterThan(first.Seq));
        Assert.That(log.LastSequence, Is.EqualTo(second.Seq));
        Assert.That(first.Utc.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(first.Fields.Single().Value, Is.EqualTo("hello"));
    }

    /// <summary>
    /// The ring keeps the last N events, and the sequence numbers stay strictly increasing across
    /// the wrap-around.
    /// </summary>
    [Test]
    public void RingKeepsTheLastEventsWithIncreasingSequences()
    {
        var log = new ActorEventLog(4);
        for (var i = 0; i < 10; i++)
        {
            log.Append("tick", new ActorEventField("i", i));
        }

        var kept = log.Since(0);

        Assert.That(kept.Count, Is.EqualTo(4));
        Assert.That(kept.Select(e => e.Seq), Is.EqualTo(new long[] { 7, 8, 9, 10 }));
        Assert.That(kept.Select(e => e.Seq), Is.Ordered.Ascending);
        Assert.That(kept.First().Fields.Single().Value, Is.EqualTo(6));
    }

    /// <summary>
    /// A reader can fetch only what it has not seen yet.
    /// </summary>
    [Test]
    public void SinceReturnsOnlyNewerEvents()
    {
        var log = new ActorEventLog(16);
        log.Append("a");
        var second = log.Append("b");
        log.Append("c");

        var newer = log.Since(second.Seq);

        Assert.That(newer.Select(e => e.Type), Is.EqualTo(new[] { "c" }));
        Assert.That(log.Since(log.LastSequence), Is.Empty);
    }

    /// <summary>
    /// A follower receives events live, and gets the backlog it asks for first.
    /// </summary>
    [Test]
    public async Task SubscriberReceivesBacklogAndLiveEventsAsync()
    {
        var log = new ActorEventLog(16);
        log.Append("old");

        using var subscription = log.Subscribe(sinceSequence: 0);
        log.Append("live");

        var first = await this.ReadNextAsync(subscription).ConfigureAwait(false);
        var second = await this.ReadNextAsync(subscription).ConfigureAwait(false);

        Assert.That(first.Type, Is.EqualTo("old"));
        Assert.That(second.Type, Is.EqualTo("live"));
    }

    /// <summary>
    /// A follower which does not read never blocks the writer: it loses the oldest events of its own
    /// buffer and is told so with a <c>lag</c> event.
    /// </summary>
    [Test]
    public async Task SlowSubscriberGetsALagEventAndNeverBlocksTheWriterAsync()
    {
        var log = new ActorEventLog(64);
        using var subscription = log.Subscribe(capacity: 4);

        for (var i = 0; i < 20; i++)
        {
            log.Append("tick", new ActorEventField("i", i));
        }

        var received = new List<ActorEvent>();
        while (subscription.Reader.TryRead(out var actorEvent))
        {
            received.Add(actorEvent);
        }

        Assert.That(log.LastSequence, Is.GreaterThanOrEqualTo(20), "the writer appended everything without blocking");
        Assert.That(received.Any(e => e.Type == "lag"), Is.True, "the slow follower was told that it lost events");
        Assert.That(received.Last().Type, Is.EqualTo("tick"), "the most recent events survive");
        Assert.That(received.Select(e => e.Seq), Is.Ordered.Ascending);
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Disposing a subscription ends the stream and detaches it from the log.
    /// </summary>
    [Test]
    public void DisposedSubscriptionStopsReceiving()
    {
        var log = new ActorEventLog(16);
        var subscription = log.Subscribe();
        subscription.Dispose();

        log.Append("after");

        Assert.That(subscription.Reader.TryRead(out _), Is.False);
        Assert.That(subscription.Reader.Completion.IsCompleted, Is.True);
    }

    private async ValueTask<ActorEvent> ReadNextAsync(ActorEventSubscription subscription)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        return await subscription.Reader.ReadAsync(timeout.Token).ConfigureAwait(false);
    }
}
