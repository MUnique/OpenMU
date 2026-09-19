// <copyright file="SkippableDelay.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Threading;
using Microsoft.Extensions.Logging;

/// <summary>
/// A delay which can be skipped, e.g. by game masters testing mini game events without
/// waiting out long entering or standby phases. At most one skippable wait is active per
/// instance at a time.
/// </summary>
public sealed class SkippableDelay
{
    private readonly ILogger _logger;
    private readonly object _owner;
    private readonly object _lock = new();
    private TaskCompletionSource? _activeWait;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkippableDelay"/> class.
    /// </summary>
    /// <param name="logger">The logger which records skipped waits.</param>
    /// <param name="owner">The owner which is named in the log entries.</param>
    public SkippableDelay(ILogger logger, object owner)
    {
        this._logger = logger;
        this._owner = owner;
    }

    /// <summary>
    /// Gets a value indicating whether a skippable wait is currently running.
    /// </summary>
    public bool IsWaitActive
    {
        get
        {
            lock (this._lock)
            {
                return this._activeWait is not null;
            }
        }
    }

    /// <summary>
    /// Wakes the currently running wait, so that it returns immediately.
    /// A skip only affects the wait which is running right now; it never arms future waits.
    /// Arming skips would let them leak into later, unrelated waits (for example a standby
    /// phase minutes later), which bricks the event flow and confuses the players.
    /// </summary>
    /// <returns><c>true</c> if a running wait has been woken; <c>false</c> if no wait is currently running.</returns>
    public bool TrySkip()
    {
        TaskCompletionSource? activeWait;
        lock (this._lock)
        {
            activeWait = this._activeWait;
        }

        return activeWait?.TrySetResult() is true;
    }

    /// <summary>
    /// Waits for the specified duration, unless the wait is skipped through
    /// <see cref="TrySkip"/> or the <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="duration">The duration to wait.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> when the wait was skipped; otherwise, <c>false</c>.</returns>
    public async Task<bool> WaitAsync(TimeSpan duration, CancellationToken cancellationToken)
    {
        if (duration <= TimeSpan.Zero)
        {
            return false;
        }

        TaskCompletionSource skip;
        lock (this._lock)
        {
            if (this._activeWait is not null)
            {
                this._logger.LogWarning("{context}: A wait is started while another wait is still active; the previous wait becomes unskippable.", this._owner);
            }

            skip = this._activeWait = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        try
        {
            using var delayCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var delayTask = Task.Delay(duration, delayCts.Token);
            if (await Task.WhenAny(delayTask, skip.Task).ConfigureAwait(false) == skip.Task)
            {
                await delayCts.CancelAsync().ConfigureAwait(false);
                try
                {
                    // Observe the cancelled delay, so it never surfaces as unobserved.
                    await delayTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected: we just cancelled it because the wait was skipped.
                }

                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("{context}: Wait of {duration} skipped.", this._owner, duration);
                return true;
            }

            await delayTask.ConfigureAwait(false);
            return false;
        }
        finally
        {
            lock (this._lock)
            {
                if (ReferenceEquals(this._activeWait, skip))
                {
                    this._activeWait = null;
                }
            }
        }
    }
}
