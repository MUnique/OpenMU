// <copyright file="WalkerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the <see cref="Walker"/>.
/// </summary>
[TestFixture]
public class WalkerTest
{
    private static readonly Point StartPoint = new(100, 100);

    /// <summary>
    /// Tests that starting a walk while another one is still running does not leave the previous walk
    /// loop behind. A loop which is orphaned this way used to keep its own (never cancelled) token,
    /// find no step to take, and then retry without ever awaiting anything - spinning a CPU core for
    /// the lifetime of the process and keeping the walk supporter alive with it.
    /// </summary>
    [Test]
    public async Task StartingASecondWalkDoesNotLeaveASpinningLoopAsync()
    {
        var supporter = new TestWalkSupporter();
        using var walker = new Walker(supporter.Object, _ => TimeSpan.FromMilliseconds(1));

        // Deliberately start twice without a StopAsync in between - that is the caller mistake which
        // stranded the first loop.
        var firstToken = await walker.InitializeWalkToAsync(new Point(110, 100), CreateSteps()).ConfigureAwait(false);
        await walker.StartWalkAsync(firstToken).ConfigureAwait(false);

        var secondToken = await walker.InitializeWalkToAsync(new Point(120, 100), CreateSteps()).ConfigureAwait(false);
        await walker.StartWalkAsync(secondToken).ConfigureAwait(false);

        await AssertNoLoopKeepsPollingAsync(supporter).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that starting the very same walk twice does not leave the first loop behind either. This
    /// is the orphaning path which does not go through <see cref="Walker.InitializeWalkToAsync"/>, so
    /// it is only <see cref="Walker.StartWalkAsync"/> itself which can still end the previous loop.
    /// </summary>
    [Test]
    public async Task StartingTheSameWalkTwiceDoesNotLeaveASpinningLoopAsync()
    {
        var supporter = new TestWalkSupporter();
        using var walker = new Walker(supporter.Object, _ => TimeSpan.FromMilliseconds(1));

        var token = await walker.InitializeWalkToAsync(new Point(110, 100), CreateSteps()).ConfigureAwait(false);
        await walker.StartWalkAsync(token).ConfigureAwait(false);
        await walker.StartWalkAsync(token).ConfigureAwait(false);

        await AssertNoLoopKeepsPollingAsync(supporter).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that concurrent walk requests for the same walker leave no loop behind. This is the shape
    /// the production bug actually had: several requests for the same object overlap, so a loop can be
    /// on its way into a stop while a newer walk is already installed. Every iteration starts its walk
    /// twice, so each one strands a loop unless the walker ends it.
    /// </summary>
    [Test]
    public async Task ConcurrentWalkRestartsLeaveNoRunningLoopAsync()
    {
        var supporter = new TestWalkSupporter();
        using var walker = new Walker(supporter.Object, _ => TimeSpan.FromMilliseconds(1));

        var tasks = new Task[8];
        for (var t = 0; t < tasks.Length; t++)
        {
            tasks[t] = Task.Run(async () =>
            {
                for (var i = 0; i < 50; i++)
                {
                    var token = await walker.InitializeWalkToAsync(new Point(110, 100), CreateSteps()).ConfigureAwait(false);

                    // Twice on purpose. A single start would lose the race against the other tasks
                    // most of the time - StartWalkAsync returns early once another task initialized
                    // its own walk in between - and then no loop would ever be started to strand.
                    await walker.StartWalkAsync(token).ConfigureAwait(false);
                    await walker.StartWalkAsync(token).ConfigureAwait(false);
                }
            });
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        await walker.StopAsync().ConfigureAwait(false);

        await AssertNoLoopKeepsPollingAsync(supporter).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that a walk loop ends when the walker is disposed while a walk is still in progress.
    /// </summary>
    [Test]
    public async Task DisposingWhileWalkingEndsTheLoopAsync()
    {
        var supporter = new TestWalkSupporter();
        var walker = new Walker(supporter.Object, _ => TimeSpan.FromMilliseconds(1));

        var token = await walker.InitializeWalkToAsync(new Point(110, 100), CreateSteps()).ConfigureAwait(false);
        await walker.StartWalkAsync(token).ConfigureAwait(false);

        walker.Dispose();

        await AssertNoLoopKeepsPollingAsync(supporter).ConfigureAwait(false);
    }

    /// <summary>
    /// Asserts that no walk loop is still running, by letting every started walk drain and then
    /// counting how often the supporter is polled while the walker should be idle. A stranded loop
    /// re-reads <see cref="IAttackable.IsAlive"/> through ShouldWalkerStop on every single pass, so
    /// it racks up hundreds of thousands of reads in this window; a finished walker reads none.
    /// </summary>
    /// <param name="supporter">The supporter which counts the reads.</param>
    private static async Task AssertNoLoopKeepsPollingAsync(TestWalkSupporter supporter)
    {
        await Task.Delay(500).ConfigureAwait(false);

        var readsBefore = supporter.IsAliveReadCount;
        await Task.Delay(200).ConfigureAwait(false);
        var readsDuringIdleWindow = supporter.IsAliveReadCount - readsBefore;

        Assert.That(readsDuringIdleWindow, Is.LessThan(10));
    }

    private static Memory<WalkingStep> CreateSteps()
    {
        var steps = new WalkingStep[4];
        var current = StartPoint;
        for (var i = 0; i < steps.Length; i++)
        {
            var next = new Point((byte)(current.X + 1), current.Y);
            steps[i] = new WalkingStep(current, next, Direction.East);
            current = next;
        }

        return steps;
    }

    /// <summary>
    /// A minimal walk supporter which counts how often the walker asked whether it is still alive.
    /// <see cref="Walker"/> only ever touches <see cref="ILocateable.Position"/> and, through
    /// <see cref="LocateableExtensions.IsActive"/>, <see cref="IAttackable.IsAlive"/> and
    /// <see cref="IAttackable.IsTeleporting"/> - everything else the two interfaces carry is left to
    /// the mock's defaults.
    /// </summary>
    private sealed class TestWalkSupporter
    {
        private readonly Mock<ISupportWalk> _mock = new();
        private int _isAliveReadCount;

        public TestWalkSupporter()
        {
            this._mock.SetupProperty(s => s.Position, StartPoint);

            var attackable = this._mock.As<IAttackable>();
            attackable.SetupGet(a => a.IsTeleporting).Returns(false);
            attackable.SetupGet(a => a.IsAlive).Returns(() =>
            {
                Interlocked.Increment(ref this._isAliveReadCount);
                return true;
            });
        }

        /// <summary>
        /// Gets the supporter to hand to the <see cref="Walker"/>.
        /// </summary>
        public ISupportWalk Object => this._mock.Object;

        /// <summary>
        /// Gets the number of times <see cref="IAttackable.IsAlive"/> has been read.
        /// </summary>
        public int IsAliveReadCount => Volatile.Read(ref this._isAliveReadCount);
    }
}
