// <copyright file="BucketAreaOfInterestConcurrencyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;
using System.Threading;

/// <summary>
/// Tests for the concurrent usage of the area of interest management of the <see cref="GameMap"/>.
/// Several threads can move the same object at the same time, e.g. a monster pushing a player while the player walks.
/// </summary>
/// <remarks>
/// A deadlock can't be detected by the runtime, it just makes a test hang. So each test awaits its work with a timeout,
/// and fails with a message instead.
/// </remarks>
[TestFixture]
public class BucketAreaOfInterestConcurrencyTest
{
    private const byte ChunkSize = 8;

    private static readonly TimeSpan CompletionTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Points which are far enough from each other, that the observed buckets of a player on one
    /// point never include the buckets of another point (info range is 20).
    /// </summary>
    private static readonly Point[] GridPoints = Enumerable.Range(0, 4)
        .SelectMany(y => Enumerable.Range(0, 4).Select(x => new Point((byte)(16 + (x * 64)), (byte)(16 + (y * 64)))))
        .ToArray();

    /// <summary>
    /// An interface which combines several other interfaces which are needed in combination for this test.
    /// </summary>
    public interface ITestPlayer : ILocateable, IBucketMapObserver, IObservable, ISupportIdUpdate
    {
    }

    /// <summary>
    /// Tests that many concurrent moves of the same observer don't corrupt its observed buckets,
    /// and that they match the last position of the observer afterwards.
    /// </summary>
    /// <remarks>
    /// Before the fix, this failed with a <see cref="NullReferenceException"/> in the area of interest manager
    /// (the list of observed buckets was modified by several threads), or with duplicate or stale subscriptions.
    /// </remarks>
    [Test]
    public async ValueTask ConcurrentMovesKeepObservedBucketsConsistentAsync()
    {
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), ChunkSize);
        await AddBeaconsAsync(map).ConfigureAwait(false);
        var player = this.GetPlayer();

        // Not on one of the grid points, so that every move changes the bucket.
        player.Object.Position = new Point(250, 250);
        await map.AddAsync(player.Object).ConfigureAwait(false);

        var moveLock = new AsyncLock();
        var workers = Enumerable.Range(0, GridPoints.Length / 2).Select(worker => Task.Run(async () =>
        {
            // Every worker alternates between its own two points, so no move targets the bucket of the current position.
            for (int i = 0; i < 200; i++)
            {
                var target = GridPoints[(worker * 2) + (i % 2)];
                await map.MoveAsync(player.Object, target, moveLock, MoveType.Instant).ConfigureAwait(false);
            }
        }));

        await AssertCompletesAsync(Task.WhenAll(workers), "The concurrent moves").ConfigureAwait(false);

        var observedBuckets = player.Object.ObservingBuckets;
        Assert.That(observedBuckets, Is.All.Not.Null);

        var distinctBuckets = new HashSet<Bucket<ILocateable>>(observedBuckets, ReferenceEqualityComparer.Instance);
        Assert.That(distinctBuckets, Has.Count.EqualTo(observedBuckets.Count));

        // The last move which was applied to the map decides the position, and the observed buckets have to match it.
        var finalPosition = player.Object.Position;
        var observedBeacons = observedBuckets.SelectMany(b => b).OfType<Beacon>().Select(b => b.Position).ToList();
        Assert.That(observedBeacons, Is.EqualTo(new[] { finalPosition }), "The observed buckets don't match the final position.");

        // Subscriptions must match, too: Each new object in the range must be announced exactly once,
        // and objects in buckets which are not observed anymore must not be announced at all.
        player.Invocations.Clear();
        var probes = new List<Beacon>();
        foreach (var point in GridPoints)
        {
            var probe = new Beacon { Position = new Point((byte)(point.X + 1), (byte)(point.Y + 1)) };
            probes.Add(probe);
            await map.AddAsync(probe).ConfigureAwait(false);
        }

        foreach (var probe in probes)
        {
            var isInRange = Math.Abs(probe.Position.X - finalPosition.X) < 8 && Math.Abs(probe.Position.Y - finalPosition.Y) < 8;
            player.Verify(p => p.LocateableAddedAsync(probe), isInRange ? Times.Once() : Times.Never());
        }
    }

    /// <summary>
    /// Tests that reading the observed objects of a player (as the game master commands do) is safe while the player is moved.
    /// </summary>
    [Test]
    public async ValueTask ReadingObservedObjectsWhileMovingDoesNotFailAsync()
    {
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), ChunkSize);
        await AddBeaconsAsync(map).ConfigureAwait(false);
        var player = this.GetPlayer();
        player.Object.Position = new Point(250, 250);
        await map.AddAsync(player.Object).ConfigureAwait(false);

        var moveLock = new AsyncLock();
        var isMoving = true;
        var mover = Task.Run(async () =>
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    await map.MoveAsync(player.Object, GridPoints[i % GridPoints.Length], moveLock, MoveType.Instant).ConfigureAwait(false);
                }
            }
            finally
            {
                Volatile.Write(ref isMoving, false);
            }
        });

        var reader = Task.Run(() =>
        {
            while (Volatile.Read(ref isMoving))
            {
                // Throws for example an InvalidOperationException ("collection was modified") without the synchronization.
                var observed = player.Object.GetObservedLocateables();
                Assert.That(observed, Is.All.Not.Null);
            }
        });

        await AssertCompletesAsync(Task.WhenAll(mover, reader), "Moving and reading the observed objects").ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that removing an observer from the map waits for an update of its observed buckets which is still in progress,
    /// and doesn't leave any subscriptions behind afterwards.
    /// </summary>
    [Test]
    public async ValueTask RemoveWaitsForRunningObserverUpdateAsync()
    {
        var map = new GameMap(new GameMapDefinition(), TimeSpan.FromSeconds(60), ChunkSize);
        var player = this.GetPlayer();
        player.Object.Position = new Point(250, 250);
        await map.AddAsync(player.Object).ConfigureAwait(false);

        // From now on, the notification about new objects in scope is slow.
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        player.Setup(p => p.NewLocateablesInScopeAsync(It.IsAny<IEnumerable<ILocateable>>()))
            .Returns(() =>
            {
                entered.TrySetResult();
                return new ValueTask(release.Task);
            });

        var target = GridPoints[0];
        var moveTask = Task.Run(() => map.MoveAsync(player.Object, target, new AsyncLock(), MoveType.Instant).AsTask());
        await AssertCompletesAsync(entered.Task, "The start of the notification").ConfigureAwait(false);

        var removeTask = Task.Run(() => map.RemoveAsync(player.Object).AsTask());
        await Task.Delay(250).ConfigureAwait(false);
        Assert.That(removeTask.IsCompleted, Is.False, "The removal should wait for the running update of the observed buckets.");

        release.SetResult();
        await AssertCompletesAsync(Task.WhenAll(moveTask, removeTask), "The move and the removal").ConfigureAwait(false);

        Assert.That(player.Object.ObservingBuckets, Is.Empty);
        player.Invocations.Clear();
        await map.AddAsync(new Beacon { Position = new Point((byte)(target.X + 1), (byte)(target.Y + 1)) }).ConfigureAwait(false);
        player.Verify(p => p.LocateableAddedAsync(It.IsAny<ILocateable>()), Times.Never);
    }

    /// <summary>
    /// Stress test with real players, which observe each other: Every player is moved concurrently by several threads
    /// (like a player walking while monsters are pushing it around). The movements, notifications and
    /// the registrations as observers take several locks, so this test fails if they can end up waiting for each other.
    /// </summary>
    [Test]
    public async ValueTask ConcurrentMovesOfObservingPlayersDoNotDeadlockAsync()
    {
        const int playerCount = 6;
        const int moversPerPlayer = 3;
        const int movesPerMover = 200;

        var gameContext = GameContextTestHelper.CreateGameContext();
        var terrain = new byte[ushort.MaxValue + 3];
        var mapDefinition = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = 0,
            TerrainData = terrain,
        };
        var map = new GameMap(mapDefinition, TimeSpan.FromMinutes(1), ChunkSize);

        var players = new List<Player>();
        for (int i = 0; i < playerCount; i++)
        {
            var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
            player.SetCurrentMapSilently(map);
            player.Position = new Point((byte)(30 + i), 30);
            await map.AddAsync(player).ConfigureAwait(false);
            players.Add(player);
        }

        // All players stay in an area which is only a few buckets wide, so they are permanently entering and leaving the view of each other.
        var workers = new List<Task>();
        for (int p = 0; p < playerCount; p++)
        {
            for (int m = 0; m < moversPerPlayer; m++)
            {
                var player = players[p];
                var random = new Random((p * 100) + m);
                workers.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < movesPerMover; i++)
                    {
                        var target = new Point((byte)random.Next(20, 80), (byte)random.Next(20, 80));
                        await player.MoveAsync(target).ConfigureAwait(false);
                    }
                }));
            }
        }

        await AssertCompletesAsync(Task.WhenAll(workers), "The concurrent moves of the players").ConfigureAwait(false);

        foreach (var player in players)
        {
            Assert.That(player.ObservingBuckets, Is.All.Not.Null);
            Assert.That(player.ObservingBuckets, Is.Unique);
        }

        var removals = players.Select(player => map.RemoveAsync(player).AsTask());
        await AssertCompletesAsync(Task.WhenAll(removals), "The removal of the players").ConfigureAwait(false);

        foreach (var player in players)
        {
            Assert.That(player.ObservingBuckets, Is.Empty, "A removed player must not observe buckets anymore.");
        }
    }

    private static async Task AssertCompletesAsync(Task task, string description)
    {
        var finished = await Task.WhenAny(task, Task.Delay(CompletionTimeout)).ConfigureAwait(false);
        Assert.That(finished, Is.SameAs(task), $"{description} did not complete within {CompletionTimeout} - probably a deadlock.");

        // Propagates exceptions of the task.
        await task.ConfigureAwait(false);
    }

    private static async ValueTask<List<Beacon>> AddBeaconsAsync(GameMap map)
    {
        var beacons = new List<Beacon>();
        foreach (var point in GridPoints)
        {
            var beacon = new Beacon { Position = point };
            beacons.Add(beacon);
            await map.AddAsync(beacon).ConfigureAwait(false);
        }

        return beacons;
    }

    private Mock<ITestPlayer> GetPlayer()
    {
        var player = new Mock<ITestPlayer>();
        player.SetupAllProperties();
        player.As<ILocateable>().SetupGet(p => p.Id).Returns(() => (player.Object as ISupportIdUpdate).Id);

        // Like the real player, the list of observed buckets is a plain list with a stable identity.
        var observingBuckets = new List<Bucket<ILocateable>>();
        player.Setup(p => p.ObservingBuckets).Returns(observingBuckets);
        player.Setup(p => p.Observers).Returns(new HashSet<IWorldObserver>());
        player.Setup(p => p.ObserverLock).Returns(new AsyncReaderWriterLock());
        player.Setup(p => p.InfoRange).Returns(20);
        return player;
    }

    /// <summary>
    /// A simple object which is only located on the map.
    /// </summary>
    private sealed class Beacon : ILocateable, ISupportIdUpdate
    {
        /// <inheritdoc />
        public ushort Id { get; set; }

        /// <inheritdoc />
        public GameMap? CurrentMap => null;

        /// <inheritdoc />
        public Point Position { get; set; }
    }
}
