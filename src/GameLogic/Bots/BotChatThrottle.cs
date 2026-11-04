// <copyright file="BotChatThrottle.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// The chat limits and the short-term memory of the bots of one game server: the server-wide gap
/// between two <c>/post</c> messages, the once-per-occasion announcement of an event, the per-bot
/// cooldowns of the normal chat and of whisper replies, and the bots' answers to party searches.
/// </summary>
/// <remarks>
/// Every check reserves its slot atomically, so two bots reacting to the same event at the same
/// moment can't both get through. The time is passed in, which keeps the class testable.
/// </remarks>
internal sealed class BotChatThrottle
{
    /// <summary>
    /// The shortest gap between two small talks, so an interval of zero doesn't make the bots talk
    /// every second.
    /// </summary>
    private static readonly TimeSpan MinimumSmallTalkInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// How long the bots remember whom they answered a party search of: the same search posted again
    /// within this time is answered by other bots.
    /// </summary>
    private static readonly TimeSpan PartySearchMemory = TimeSpan.FromMinutes(30);

    /// <summary>
    /// How often the bots look for players who are due to overhear a conversation. A second-by-second
    /// look over all players would cost for nothing; a few seconds late doesn't matter.
    /// </summary>
    private static readonly TimeSpan ConversationScanInterval = TimeSpan.FromSeconds(5);

    private readonly ConcurrentDictionary<string, DateTime> _occasions = new();

    private readonly ConditionalWeakTable<Player, BotCooldowns> _bots = new();

    private readonly Lock _smallTalkLock = new();

    private readonly ConditionalWeakTable<Player, BotPartyOffer> _partyOffers = new();

    private readonly ConditionalWeakTable<Player, PartySearchVolunteers> _partySearches = new();

    private readonly ConditionalWeakTable<Player, StrongBox<DateTime>> _nextConversations = new();

    private readonly HashSet<BotConversation> _usedConversations = [];

    private readonly Lock _conversationLock = new();

    private long _nextConversationScanTicks;

    private long _nextPostTicks;

    private DateTime _nextSmallTalkUtc;

    private int _handledForceStartCount;

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

    /// <summary>
    /// Determines whether the next small talk of this server is due, and if so, schedules the one
    /// after it. A forced start (from the admin panel) makes it due at once.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <param name="interval">The configured interval between two small talks.</param>
    /// <param name="forceStartCount">The current number of forced starts of the plugin.</param>
    /// <returns><c>true</c>, if a bot may make small talk now.</returns>
    public bool TryStartSmallTalk(DateTime now, TimeSpan interval, int forceStartCount)
    {
        lock (this._smallTalkLock)
        {
            if (this._nextSmallTalkUtc == default)
            {
                // A freshly started server waits one interval first - the bots are just logging in.
                this._nextSmallTalkUtc = now + interval;
            }

            if (now < this._nextSmallTalkUtc && forceStartCount == this._handledForceStartCount)
            {
                return false;
            }

            this._handledForceStartCount = forceStartCount;
            this._nextSmallTalkUtc = now + (interval > MinimumSmallTalkInterval ? interval : MinimumSmallTalkInterval);
            return true;
        }
    }

    /// <summary>
    /// Picks the bots which answer a player's party search: up to the given number of the candidates,
    /// at random, leaving out the bots which already answered this player's earlier searches.
    /// </summary>
    /// <param name="requester">The searching player.</param>
    /// <param name="candidates">The bots which could answer.</param>
    /// <param name="count">The maximum number of answering bots.</param>
    /// <param name="now">The current time.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <returns>The answering bots.</returns>
    public IReadOnlyList<Player> TakeNewVolunteers(Player requester, IEnumerable<Player> candidates, int count, DateTime now, Func<int, int> pickIndex)
    {
        var memory = this._partySearches.GetOrCreateValue(requester);
        lock (memory)
        {
            if (now - memory.LastSearchUtc > PartySearchMemory)
            {
                memory.Bots.Clear();
            }

            memory.LastSearchUtc = now;
            var remaining = candidates.Where(bot => !memory.Bots.Contains(bot)).ToList();
            var volunteers = new List<Player>();
            while (volunteers.Count < count && remaining.Count > 0)
            {
                var index = pickIndex(remaining.Count);
                volunteers.Add(remaining[index]);
                remaining.RemoveAt(index);
            }

            memory.Bots.UnionWith(volunteers);
            return volunteers;
        }
    }

    /// <summary>
    /// Remembers that the bot answered a player's party search and waits for the meeting place.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="offer">The offer.</param>
    public void SetPartyOffer(Player bot, BotPartyOffer offer) => this._partyOffers.AddOrUpdate(bot, offer);

    /// <summary>
    /// Gets the bot's open offer to a player's party search, if it has one which didn't expire yet.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The open offer, or <c>null</c>.</returns>
    public BotPartyOffer? GetOpenPartyOffer(Player bot, DateTime now)
        => this._partyOffers.TryGetValue(bot, out var offer) && offer.ExpiresUtc > now ? offer : null;

    /// <summary>
    /// Forgets the bot's offer to a player's party search.
    /// </summary>
    /// <param name="bot">The bot.</param>
    public void RemovePartyOffer(Player bot) => this._partyOffers.Remove(bot);

    /// <summary>
    /// Determines whether it's time for the next look for players who are due to overhear a
    /// conversation.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <returns><c>true</c>, if the caller should look now.</returns>
    public bool TryStartConversationScan(DateTime now)
        => TryReserve(ref this._nextConversationScanTicks, now, ConversationScanInterval);

    /// <summary>
    /// Determines whether the player is due to overhear a conversation. A player seen for the first
    /// time waits one interval first - the player just entered the game.
    /// </summary>
    /// <param name="player">The human player.</param>
    /// <param name="now">The current time.</param>
    /// <param name="interval">The configured interval between two conversations a player hears.</param>
    /// <returns><c>true</c>, if the player is due.</returns>
    public bool IsConversationDue(Player player, DateTime now, TimeSpan interval)
    {
        var next = this._nextConversations.GetValue(player, _ => new StrongBox<DateTime>(now + interval));
        lock (next)
        {
            return now >= next.Value;
        }
    }

    /// <summary>
    /// Postpones the next conversation the player may overhear.
    /// </summary>
    /// <param name="player">The human player.</param>
    /// <param name="until">The time of the player's next conversation.</param>
    public void PostponeConversation(Player player, DateTime until)
    {
        var next = this._nextConversations.GetValue(player, _ => new StrongBox<DateTime>(until));
        lock (next)
        {
            next.Value = until;
        }
    }

    /// <summary>
    /// Draws a conversation from the candidates without repetition: a conversation comes again only
    /// after all candidates were used.
    /// </summary>
    /// <param name="candidates">The conversations which fit the place.</param>
    /// <param name="pickIndex">Returns a random index below the given count.</param>
    /// <returns>The drawn conversation, or <c>null</c> without candidates.</returns>
    public BotConversation? DrawConversation(IReadOnlyCollection<BotConversation> candidates, Func<int, int> pickIndex)
    {
        if (candidates.Count == 0)
        {
            return null;
        }

        lock (this._conversationLock)
        {
            var unused = candidates.Where(c => !this._usedConversations.Contains(c)).ToList();
            if (unused.Count == 0)
            {
                this._usedConversations.ExceptWith(candidates);
                unused = candidates.ToList();
            }

            var drawn = unused[pickIndex(unused.Count)];
            this._usedConversations.Add(drawn);
            return drawn;
        }
    }

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
    /// The bots which answered the party searches of one player.
    /// </summary>
    private sealed class PartySearchVolunteers
    {
        /// <summary>
        /// Gets the bots which answered.
        /// </summary>
        public HashSet<Player> Bots { get; } = [];

        /// <summary>
        /// Gets or sets the time of the player's last search.
        /// </summary>
        public DateTime LastSearchUtc { get; set; }
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
