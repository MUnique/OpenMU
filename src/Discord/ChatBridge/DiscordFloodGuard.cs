// <copyright file="DiscordFloodGuard.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.ChatBridge;

using System.Collections.Concurrent;

/// <summary>
/// Limits the number of messages which a Discord user can send to the game per minute.
/// </summary>
public sealed class DiscordFloodGuard
{
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    private readonly int _maximumMessages;
    private readonly TimeProvider _timeProvider;
    private readonly ConcurrentDictionary<ulong, Queue<DateTimeOffset>> _messageTimes = new();

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
    /// Registers a message of the user, if the user didn't send too many messages.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns><c>true</c>, if the message is allowed; otherwise, <c>false</c>.</returns>
    public bool TryRegister(ulong userId)
    {
        var now = this._timeProvider.GetUtcNow();
        var times = this._messageTimes.GetOrAdd(userId, _ => new Queue<DateTimeOffset>());
        lock (times)
        {
            while (times.TryPeek(out var oldest) && now - oldest >= Window)
            {
                times.Dequeue();
            }

            if (times.Count >= this._maximumMessages)
            {
                return false;
            }

            times.Enqueue(now);
            return true;
        }
    }
}
