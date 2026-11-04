// <copyright file="BotChatThrottle.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// The chat limits of the bots of one game server: the server-wide gap between two <c>/post</c>
/// messages, the once-per-occasion announcement of an event, and the per-bot cooldowns of the
/// normal chat and of whisper replies.
/// </summary>
/// <remarks>
/// Every check reserves its slot atomically, so two bots reacting to the same event at the same
/// moment can't both get through. The time is passed in, which keeps the class testable.
/// </remarks>
internal sealed class BotChatThrottle
{
    private readonly ConcurrentDictionary<string, DateTime> _occasions = new();

    private readonly ConditionalWeakTable<Player, BotCooldowns> _bots = new();

    private long _nextPostTicks;

    /// <summary>
    /// Reserves the right to announce an occasion (e.g. "Blood Castle opened") for the given time
    /// window. Only the first caller within the window gets it.
    /// </summary>
    /// <param name="occasionKey">The key of the occasion, e.g. the mini game type.</param>
    /// <param name="now">The current time.</param>
    /// <param name="window">How long the occasion stays reserved.</param>
    /// <returns><c>true</c>, if the caller may announce the occasion.</returns>
    public bool TryReserveOccasion(string occasionKey, DateTime now, TimeSpan window)
    {
        while (true)
        {
            if (this._occasions.TryAdd(occasionKey, now + window))
            {
                return true;
            }

            if (!this._occasions.TryGetValue(occasionKey, out var reservedUntil) || reservedUntil > now)
            {
                // Either still reserved, or another caller just replaced the expired reservation.
                return false;
            }

            if (this._occasions.TryUpdate(occasionKey, now + window, reservedUntil))
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Reserves the next server-wide <c>/post</c> of a bot.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <param name="gap">The minimum time until the next post.</param>
    /// <returns><c>true</c>, if the caller may post now.</returns>
    public bool TryReservePost(DateTime now, TimeSpan gap)
        => TryReserve(ref this._nextPostTicks, now, gap);

    /// <summary>
    /// Reserves the next normal chat line of the bot.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="now">The current time.</param>
    /// <param name="cooldown">The minimum time until the bot's next line.</param>
    /// <returns><c>true</c>, if the bot may say something now.</returns>
    public bool TryReserveChat(Player bot, DateTime now, TimeSpan cooldown)
        => TryReserve(ref this._bots.GetOrCreateValue(bot).NextChatTicks, now, cooldown);

    /// <summary>
    /// Reserves the next whisper reply of the bot.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="now">The current time.</param>
    /// <param name="cooldown">The minimum time until the bot's next reply.</param>
    /// <returns><c>true</c>, if the bot may reply now.</returns>
    public bool TryReserveWhisperReply(Player bot, DateTime now, TimeSpan cooldown)
        => TryReserve(ref this._bots.GetOrCreateValue(bot).NextWhisperReplyTicks, now, cooldown);

    private static bool TryReserve(ref long nextAllowedTicks, DateTime now, TimeSpan gap)
    {
        while (true)
        {
            var current = Interlocked.Read(ref nextAllowedTicks);
            if (now.Ticks < current)
            {
                return false;
            }

            if (Interlocked.CompareExchange(ref nextAllowedTicks, (now + gap).Ticks, current) == current)
            {
                return true;
            }
        }
    }

    /// <summary>
    /// The cooldowns of one bot, as raw ticks for the interlocked updates.
    /// </summary>
    private sealed class BotCooldowns
    {
        private long _nextChatTicks;

        private long _nextWhisperReplyTicks;

        /// <summary>
        /// Gets a reference to the time (ticks) from which the bot may say the next normal chat line.
        /// </summary>
        public ref long NextChatTicks => ref this._nextChatTicks;

        /// <summary>
        /// Gets a reference to the time (ticks) from which the bot may answer the next whisper.
        /// </summary>
        public ref long NextWhisperReplyTicks => ref this._nextWhisperReplyTicks;
    }
}
