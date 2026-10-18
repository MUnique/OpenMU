// <copyright file="DiscordFloodGuard.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Limits the number of messages which a Discord user can send to the game per minute.
/// </summary>
public sealed class DiscordFloodGuard
{
    /// <summary>
    /// The number of registrations after which the inactive users are removed.
    /// </summary>
    private const int SweepInterval = 100;

    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    private readonly int _maximumMessages;
    private readonly TimeProvider _timeProvider;
    private readonly ConcurrentDictionary<ulong, Queue<DateTimeOffset>> _messageTimes = new();
    private int _registrationCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordFloodGuard"/> class.
    /// </summary>
    /// <param name="maximumMessages">The maximum number of messages per minute.</param>
    /// <param name="timeProvider">The time provider.</param>
    public DiscordFloodGuard(int maximumMessages, TimeProvider timeProvider)
    {
        this._maximumMessages = Math.Max(maximumMessages, 1);
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// Gets the number of users which are tracked, for tests.
    /// </summary>
    internal int TrackedUserCount => this._messageTimes.Count;

    /// <summary>
    /// Registers a message of the user, if the user didn't send too many messages.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns><c>true</c>, if the message is allowed; otherwise, <c>false</c>.</returns>
    public bool TryRegister(ulong userId)
    {
        var now = this._timeProvider.GetUtcNow();
        if (Interlocked.Increment(ref this._registrationCount) % SweepInterval == 0)
        {
            this.RemoveInactiveUsers(now);
        }

        while (true)
        {
            var times = this._messageTimes.GetOrAdd(userId, _ => new Queue<DateTimeOffset>());
            lock (times)
            {
                // The queue might have been removed by RemoveInactiveUsers in the meantime.
                if (!this._messageTimes.TryGetValue(userId, out var current) || current != times)
                {
                    continue;
                }

                RemoveExpired(times, now);
                if (times.Count >= this._maximumMessages)
                {
                    return false;
                }

                times.Enqueue(now);
                return true;
            }
        }
    }

    private static void RemoveExpired(Queue<DateTimeOffset> times, DateTimeOffset now)
    {
        while (times.TryPeek(out var oldest) && now - oldest >= Window)
        {
            times.Dequeue();
        }
    }

    /// <summary>
    /// Removes the users who didn't send a message within the window, so that the dictionary doesn't grow forever.
    /// </summary>
    private void RemoveInactiveUsers(DateTimeOffset now)
    {
        foreach (var (userId, times) in this._messageTimes)
        {
            lock (times)
            {
                RemoveExpired(times, now);
                if (times.Count == 0)
                {
                    this._messageTimes.TryRemove(new KeyValuePair<ulong, Queue<DateTimeOffset>>(userId, times));
                }
            }
        }
    }
}
