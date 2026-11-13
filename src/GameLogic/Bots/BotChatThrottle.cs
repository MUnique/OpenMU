// <copyright file="BotChatThrottle.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using MUnique.OpenMU.Pathfinding;

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
    /// The shortest gap between two merchant adverts, so an interval of zero doesn't make the bots talk
    /// every second.
    /// </summary>
    private static readonly TimeSpan MinimumMerchantTalkInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// How long the bots remember whom they answered a party search of: the same search posted again
    /// within this time is answered by other bots.
    /// </summary>
    private static readonly TimeSpan PartySearchMemory = TimeSpan.FromMinutes(30);

    /// <summary>
    /// How often the bots look over the players - who is due to overhear a conversation, whose reset
    /// count grew. A second-by-second look over all players would cost for nothing; a few seconds late
    /// doesn't matter.
    /// </summary>
    private static readonly TimeSpan PlayerScanInterval = TimeSpan.FromSeconds(5);

    /// <summary>
    /// How often the expired occasions are forgotten. Their keys include player names (greetings,
    /// congratulations), so without it they'd pile up until the server restarts.
    /// </summary>
    private static readonly TimeSpan OccasionCleanupInterval = TimeSpan.FromMinutes(10);

    private readonly ConcurrentDictionary<string, DateTime> _occasions = new();

    private readonly ConditionalWeakTable<Player, BotCooldowns> _bots = new();

    private readonly Lock _merchantTalkLock = new();

    private readonly ConditionalWeakTable<Player, BotPartyOffer> _partyOffers = new();

    private readonly Lock _partyOfferLock = new();

    private readonly ConditionalWeakTable<Player, PartySearchVolunteers> _partySearches = new();

    private readonly ConditionalWeakTable<Player, StrongBox<DateTime>> _nextConversations = new();

    private readonly HashSet<BotConversation> _usedConversations = [];

    private readonly Lock _conversationLock = new();

    private readonly ConcurrentDictionary<string, int> _knownResets = new();

    private readonly ConcurrentDictionary<(string Character, string Wings), bool> _seenWings = new();

    private readonly ConditionalWeakTable<Player, Dictionary<object, object>> _lastLines = new();

    private readonly List<(GameMap Map, Point Position, DateTime Time)> _humanChat = [];

    private readonly List<(Player First, Player Second, DateTime Until)> _recentConversations = [];

    private readonly ConditionalWeakTable<Player, HashSet<string>> _friends = new();

    private long _nextPlayerScanTicks;

    private long _nextOccasionCleanupTicks;

    private long _nextPostTicks;

    private DateTime _nextMerchantTalkUtc;

    private int _handledForceStartCount;

    /// <summary>
    /// Gets the number of remembered conversations, expired ones included until they're pruned.
    /// </summary>
    internal int RecentConversationCount
    {
        get
        {
            lock (this._recentConversations)
            {
                return this._recentConversations.Count;
            }
        }
    }

    /// <summary>
    /// Gets the number of remembered occasions, expired ones included until they're cleaned up.
    /// </summary>
    internal int OccasionCount => this._occasions.Count;

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
        if (TryReserve(ref this._nextOccasionCleanupTicks, now, OccasionCleanupInterval))
        {
            foreach (var occasion in this._occasions)
            {
                if (occasion.Value <= now)
                {
                    // Removes the entry only if it wasn't reserved again meanwhile.
                    this._occasions.TryRemove(occasion);
                }
            }
        }

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
    /// Determines whether the next merchant advert of this server is due, and if so, schedules the one
    /// after it. A forced start (from the admin panel) makes it due at once.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <param name="interval">The configured interval between two merchant adverts.</param>
    /// <param name="forceStartCount">The current number of forced starts of the plugin.</param>
    /// <returns><c>true</c>, if a bot may advertise now.</returns>
    public bool TryStartMerchantTalk(DateTime now, TimeSpan interval, int forceStartCount)
    {
        lock (this._merchantTalkLock)
        {
            if (this._nextMerchantTalkUtc == default)
            {
                // A freshly started server waits one interval first - the bots are just logging in.
                this._nextMerchantTalkUtc = now + interval;
            }

            if (now < this._nextMerchantTalkUtc && forceStartCount == this._handledForceStartCount)
            {
                return false;
            }

            this._handledForceStartCount = forceStartCount;
            this._nextMerchantTalkUtc = now + (interval > MinimumMerchantTalkInterval ? interval : MinimumMerchantTalkInterval);
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
    /// Reserves the bot for a player's party search: it answers it and waits for the meeting place.
    /// A bot with an open offer to another player isn't taken - two players searching at the same
    /// time don't both get it.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="offer">The offer.</param>
    /// <param name="now">The current time.</param>
    /// <returns><c>true</c>, if the bot was free and is now reserved for the offer.</returns>
    public bool TryReservePartyOffer(Player bot, BotPartyOffer offer, DateTime now)
    {
        lock (this._partyOfferLock)
        {
            if (this.GetOpenPartyOffer(bot, now) is not null)
            {
                return false;
            }

            this._partyOffers.AddOrUpdate(bot, offer);
            return true;
        }
    }

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
    /// Determines whether it's time for the next look over the players.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <returns><c>true</c>, if the caller should look now.</returns>
    public bool TryStartPlayerScan(DateTime now)
        => TryReserve(ref this._nextPlayerScanTicks, now, PlayerScanInterval);

    /// <summary>
    /// Remembers the reset count of a character and tells whether it grew since the last look. A
    /// character seen for the first time didn't reset in front of the bots.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <param name="resets">The current reset count.</param>
    /// <returns><c>true</c>, if the count grew.</returns>
    public bool TryUpdateKnownResets(string characterName, int resets)
    {
        var previous = this._knownResets.GetOrAdd(characterName, resets);
        if (resets < previous)
        {
            // Lowered by an admin: start counting from there.
            this._knownResets.TryUpdate(characterName, resets, previous);
            return false;
        }

        return resets > previous && this._knownResets.TryUpdate(characterName, resets, previous);
    }

    /// <summary>
    /// Remembers that the bots saw a character put on a pair of wings, and tells whether it's the
    /// first time - the wings go on again after every change of equipment, but a bot comments only
    /// once per character and pair until the server restarts.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <param name="wingsKey">The pair of wings, e.g. its item definition and level.</param>
    /// <returns><c>true</c>, if the bots didn't see the character put on this pair before.</returns>
    public bool TryRememberWings(string characterName, string wingsKey)
        => this._seenWings.TryAdd((characterName, wingsKey), true);

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

    /// <summary>
    /// Gets the line of the list which the bots said last to the player.
    /// </summary>
    /// <param name="player">The player the line was said to.</param>
    /// <param name="list">The list of the line, e.g. the whisper replies.</param>
    /// <returns>The line, or <c>null</c> when none of the list was said to the player yet.</returns>
    public object? GetLastLine(Player player, object list)
    {
        var lines = this._lastLines.GetOrCreateValue(player);
        lock (lines)
        {
            return lines.GetValueOrDefault(list);
        }
    }

    /// <summary>
    /// Remembers the line of the list which the bots said to the player, so the next one differs.
    /// </summary>
    /// <param name="player">The player the line was said to.</param>
    /// <param name="list">The list of the line, e.g. the whisper replies.</param>
    /// <param name="line">The line.</param>
    public void SetLastLine(Player player, object list, object line)
    {
        var lines = this._lastLines.GetOrCreateValue(player);
        lock (lines)
        {
            lines[list] = line;
        }
    }

    /// <summary>
    /// Remembers that a human player said something in the normal chat.
    /// </summary>
    /// <param name="map">The map of the player.</param>
    /// <param name="position">The position of the player.</param>
    /// <param name="now">The current time.</param>
    /// <param name="window">How long a message counts; older ones are forgotten.</param>
    public void RecordHumanChat(GameMap map, Point position, DateTime now, TimeSpan window)
    {
        lock (this._humanChat)
        {
            this._humanChat.RemoveAll(message => now - message.Time > window);
            this._humanChat.Add((map, position, now));
        }
    }

    /// <summary>
    /// Determines whether the human players near a place are busy talking: at least the given number
    /// of their normal chat messages were said within the window and the range.
    /// </summary>
    /// <param name="map">The map of the place.</param>
    /// <param name="position">The position of the place.</param>
    /// <param name="range">The range around the place.</param>
    /// <param name="now">The current time.</param>
    /// <param name="window">How long a message counts.</param>
    /// <param name="count">The number of messages from which the chat is busy; 0 never.</param>
    /// <returns><c>true</c>, if the players near the place are busy talking.</returns>
    public bool IsHumanChatBusy(GameMap map, Point position, double range, DateTime now, TimeSpan window, int count)
    {
        if (count <= 0)
        {
            return false;
        }

        lock (this._humanChat)
        {
            return this._humanChat.Count(message =>
                ReferenceEquals(message.Map, map)
                && now - message.Time <= window
                && message.Position.EuclideanDistanceTo(position) <= range) >= count;
        }
    }

    /// <summary>
    /// Remembers that two bots just had a conversation, so a player who chimes in gets an answer.
    /// </summary>
    /// <param name="first">The bot which opened the conversation.</param>
    /// <param name="second">The bot which replied.</param>
    /// <param name="until">Until when a player's message counts as chiming in.</param>
    /// <param name="now">The current time; conversations which ended before it are forgotten.</param>
    public void RememberConversation(Player first, Player second, DateTime until, DateTime now)
    {
        lock (this._recentConversations)
        {
            // Pruned here as well: on a quiet server no player chimes in to prune them, while the bots
            // keep talking.
            this._recentConversations.RemoveAll(c => c.Until < now);
            this._recentConversations.Add((first, second, until));
        }
    }

    /// <summary>
    /// Takes the recent conversation a player chimes in with a message: one which is still open and
    /// of which one bot stands near the player. Each conversation is answered once.
    /// </summary>
    /// <param name="isNear">Determines whether a bot stands near the player.</param>
    /// <param name="now">The current time.</param>
    /// <returns>The two bots, or <c>null</c> without such a conversation.</returns>
    public (Player First, Player Second)? TakeRecentConversation(Func<Player, bool> isNear, DateTime now)
    {
        lock (this._recentConversations)
        {
            this._recentConversations.RemoveAll(c => c.Until < now);
            var index = this._recentConversations.FindIndex(c => isNear(c.First) || isNear(c.Second));
            if (index < 0)
            {
                return null;
            }

            var conversation = this._recentConversations[index];
            this._recentConversations.RemoveAt(index);
            return (conversation.First, conversation.Second);
        }
    }

    /// <summary>
    /// Remembers that the bot was in a party with the players - until the server restarts.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="characterNames">The names of the players' characters.</param>
    public void RememberFriends(Player bot, IEnumerable<string> characterNames)
    {
        var friends = this._friends.GetOrCreateValue(bot);
        lock (friends)
        {
            friends.UnionWith(characterNames);
        }
    }

    /// <summary>
    /// Determines whether the bot was in a party with the player before.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="characterName">The name of the player's character.</param>
    /// <returns><c>true</c>, if the bot knows the player.</returns>
    public bool IsFriend(Player bot, string characterName)
    {
        if (!this._friends.TryGetValue(bot, out var friends))
        {
            return false;
        }

        lock (friends)
        {
            return friends.Contains(characterName);
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
