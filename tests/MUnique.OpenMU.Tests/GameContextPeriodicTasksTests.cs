// <copyright file="GameContextPeriodicTasksTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the lifecycle of the game context's periodic tasks (<see cref="GameContext.StopPeriodicTasks"/>):
/// the per-second plug-in timer must keep firing while the server runs and go silent once the server
/// starts shutting down - otherwise a maintenance pass races the shutdown's disconnect loop on the
/// same player instances (save vs. dispose).
/// </summary>
[TestFixture]
public class GameContextPeriodicTasksTests
{
    /// <summary>
    /// The periodic tasks run while the server is up and stop firing once they are stopped.
    /// </summary>
    [Test]
    public async ValueTask PeriodicTasksStopFiringAfterStopAsync()
    {
        CountingPeriodicTaskPlugIn.Reset();
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext(
        [
            new PlugInConfiguration
            {
                TypeId = CountingPeriodicTaskPlugIn.PlugInId,
                IsActive = true,
            },
        ]);
        try
        {
            // The timer fires every second: within a generous timeout at least one pass must run,
            // otherwise the environment (not the code under test) is broken.
            Assert.That(await WaitUntilAsync(() => CountingPeriodicTaskPlugIn.CountFor(gameContext) > 0, TimeSpan.FromSeconds(10)).ConfigureAwait(false), Is.True, "The periodic task never fired before the stop.");

            gameContext.StopPeriodicTasks();
            Assert.That(gameContext.ArePeriodicTasksStopped, Is.True);

            // Wait until no pass has run for a full timer period: passes which started (or were
            // already queued) before the stop have finished - the timer callback is fire-and-forget,
            // so under load its continuations may lag behind - and no new pass may start anymore.
            Assert.That(await WaitForQuiescenceAsync(gameContext, TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(15)).ConfigureAwait(false), Is.True, "The periodic tasks did not come to rest after the stop.");
            var invocationsAfterStop = CountingPeriodicTaskPlugIn.CountFor(gameContext);
            await Task.Delay(TimeSpan.FromSeconds(2.5)).ConfigureAwait(false);
            Assert.That(CountingPeriodicTaskPlugIn.CountFor(gameContext), Is.EqualTo(invocationsAfterStop), "A periodic task ran after the tasks were stopped.");
        }
        finally
        {
            await gameContext.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A pass may run on a fresh context, but not once the tasks are stopped or the context is gone.
    /// </summary>
    [Test]
    public async ValueTask ShouldExecutePeriodicTasksReflectsLifecycleAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        try
        {
            Assert.That(gameContext.ShouldExecutePeriodicTasks, Is.True);

            gameContext.StopPeriodicTasks();
            Assert.That(gameContext.ShouldExecutePeriodicTasks, Is.False);
        }
        finally
        {
            await gameContext.DisposeAsync().ConfigureAwait(false);
        }

        Assert.That(gameContext.ShouldExecutePeriodicTasks, Is.False);
    }

    /// <summary>
    /// Stopping is a no-op when nothing runs yet and stays effective when called twice.
    /// </summary>
    [Test]
    public async ValueTask StopPeriodicTasksIsIdempotentAsync()
    {
        var gameContext = (GameContext)GameContextTestHelper.CreateGameContext();
        try
        {
            Assert.That(gameContext.ArePeriodicTasksStopped, Is.False);

            gameContext.StopPeriodicTasks();
            Assert.That(gameContext.ArePeriodicTasksStopped, Is.True);

            Assert.DoesNotThrow(() => gameContext.StopPeriodicTasks());
            Assert.That(gameContext.ArePeriodicTasksStopped, Is.True);
        }
        finally
        {
            await gameContext.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async ValueTask<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (!condition())
        {
            if (DateTime.UtcNow >= deadline)
            {
                return false;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false);
        }

        return true;
    }

    private static async ValueTask<bool> WaitForQuiescenceAsync(GameContext gameContext, TimeSpan quietPeriod, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var invocations = CountingPeriodicTaskPlugIn.CountFor(gameContext);
            await Task.Delay(quietPeriod).ConfigureAwait(false);
            if (CountingPeriodicTaskPlugIn.CountFor(gameContext) == invocations)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// A periodic task which only counts its invocations, so the tests can observe whether the
    /// game context's timer is (still) running. Discovered automatically like any other plug-in.
    /// </summary>
    [PlugIn]
    [Guid("7D2A4C91-3B5E-4F8A-9C1D-6E5B0A7F3D82")]
    public sealed class CountingPeriodicTaskPlugIn : IPeriodicTaskPlugIn
    {
        /// <summary>
        /// Gets the plug-in id, referenced by the test's plug-in configuration.
        /// </summary>
        public static Guid PlugInId { get; } = new ("7D2A4C91-3B5E-4F8A-9C1D-6E5B0A7F3D82");

        /// <summary>
        /// The invocations per game context: discovered plug-ins are active in every game context of
        /// the process by default (see <see cref="PlugInManager"/>), and the test contexts of other
        /// tests are never disposed - so a global counter would also observe their timers. Scoping the
        /// counts by context keeps this test independent of whatever else runs in the same process.
        /// </summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<GameContext, int> InvocationsPerContext = new();

        /// <summary>
        /// Gets the number of invocations for the given game context.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public static int CountFor(GameContext gameContext)
            => InvocationsPerContext.TryGetValue(gameContext, out var count) ? count : 0;

        /// <summary>
        /// Clears all recorded invocations.
        /// </summary>
        public static void Reset() => InvocationsPerContext.Clear();

        /// <inheritdoc />
        public ValueTask ExecuteTaskAsync(GameContext gameContext)
        {
            InvocationsPerContext.AddOrUpdate(gameContext, 1, (_, count) => count + 1);
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public void ForceStart()
        {
            // Nothing to force, every pass counts.
        }
    }
}
