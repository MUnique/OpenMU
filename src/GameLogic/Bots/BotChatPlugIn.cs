// <copyright file="BotChatPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.Duel;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Lets the server-side bots talk: they announce opened events and started invasions with
/// <c>/post</c>, comment on their level milestones and answer whispers - with lines from the
/// configuration, no language model.
/// </summary>
/// <remarks>
/// A bot only talks about what it could actually do: it posts about a Blood Castle only when it
/// meets that event's entry requirements (the same check that decides whether a bot may follow its
/// leader in, see <see cref="BotMiniGameHandler.IsEligible"/>), and about an invasion only when it may
/// warp to the invaded map. After announcing an event, the bot waits at the event's entry NPC (or in
/// its home town for Chaos Castle) until the entrance closes, so the players can find and invite it
/// (see <see cref="BotEventMeeting"/>). The limits of the <see cref="BotChatConfiguration"/> keep a large
/// population from flooding the chat; they are tracked per game server.
/// It's a feature plugin of the bots, listed in the admin panel next to them.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.BotChatPlugIn_Name), Description = nameof(PlugInResources.BotChatPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B3234CC4-1365-463A-B0DE-603F9D7D86A2")]
public class BotChatPlugIn
    : IFeaturePlugIn,
    IMiniGameEntranceOpenedPlugIn,
    IInvasionStartedPlugIn,
    ICharacterLevelUpPlugIn,
    IWhisperMessageReceivedPlugIn,
    IPlayerStateChangedPlugIn,
    IBotPartyJoinedPlugIn,
    IMiniGameEndedPlugIn,
    IAttackableGotKilledPlugIn,
    IChatMessageSentPlugIn,
    IPeriodicTaskPlugIn,
    IPostMessageSentPlugIn,
    ITradeRequestedPlugIn,
    IDuelRequestedPlugIn,
    ISupportCustomConfiguration<BotChatConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    /// <summary>
    /// How close a bot must stand to a player to talk to them - to greet an entering player, or to
    /// let them overhear a conversation: roughly the range in which the player sees it, so the line
    /// comes from a character on the screen.
    /// </summary>
    private const double HearingRange = 12;

    /// <summary>
    /// How close two bots must stand to each other to have a conversation - close enough to look like
    /// they are talking to each other.
    /// </summary>
    private const double ConversationPairRange = 5;

    /// <summary>
    /// How long an invasion counts as announced. Long enough that a second invasion of the same kind
    /// started by the same schedule run doesn't get its own post.
    /// </summary>
    private static readonly TimeSpan InvasionAnnouncementWindow = TimeSpan.FromMinutes(10);

    /// <summary>
    /// The least time an event's entrance must still stay open for a bot to announce it: the bot
    /// waits at the meeting point until the entrance closes (see <see cref="BotEventMeeting"/>), and a
    /// player who reads the post needs the time to get there and invite it.
    /// </summary>
    private static readonly TimeSpan MinimumMeetingTime = TimeSpan.FromMinutes(2);

    /// <summary>
    /// The delay range of a level milestone comment: a moment after the level-up effect.
    /// </summary>
    private static readonly (int Min, int Max) LevelMilestoneDelaySeconds = (1, 4);

    /// <summary>
    /// The delay range of a greeting: the entering player's client needs a moment to load the world.
    /// </summary>
    private static readonly (int Min, int Max) GreetingDelaySeconds = (3, 8);

    /// <summary>
    /// The delay range of a comment on being killed: said while the bot still lies there, before it
    /// respawns, so the killer sees it.
    /// </summary>
    private static readonly (int Min, int Max) KilledDelaySeconds = (1, 2);

    /// <summary>
    /// The delay range of a congratulation: the level-up or reset effect first, then the "gz".
    /// </summary>
    private static readonly (int Min, int Max) CongratulationDelaySeconds = (2, 5);

    /// <summary>
    /// The delay range before a bot starts typing an answer: the time to notice and read the message.
    /// </summary>
    private static readonly (int Min, int Max) TypingReactionSeconds = (1, 3);

    /// <summary>
    /// The range of the time a bot needs per typed character - a casual typist in a game chat.
    /// </summary>
    private static readonly (int Min, int Max) TypingMillisecondsPerCharacter = (120, 220);

    /// <summary>
    /// The longest a bot types, so a long line doesn't arrive after the player gave up waiting.
    /// </summary>
    private static readonly TimeSpan MaximumTypingTime = TimeSpan.FromSeconds(12);

    /// <summary>
    /// The gap between the whispers of two bots answering the same party search, so the answers
    /// trickle in like those of players who read the post at different moments.
    /// </summary>
    private static readonly (int Min, int Max) PartyVolunteerSpacingSeconds = (2, 6);

    private readonly ConditionalWeakTable<IGameContext, BotChatThrottle> _throttles = new();

    /// <summary>
    /// The number of calls of <see cref="ForceStart"/>; a server which didn't see the current number
    /// yet lets a bot advertise like a merchant without waiting for the interval.
    /// </summary>
    private int _forceStartCount;

    /// <inheritdoc />
    public BotChatConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => BotChatConfiguration.CreateDefault();

    /// <inheritdoc />
    public ValueTask MiniGameEntranceOpenedAsync(MiniGameContext miniGame)
    {
        if (this.Configuration is not { EventPostChancePercent: > 0 } configuration
            || miniGame.Definition.Type is not (MiniGameType.BloodCastle or MiniGameType.DevilSquare or MiniGameType.ChaosCastle)
            || !IsPostCommandActive(miniGame.GameContext))
        {
            return ValueTask.CompletedTask;
        }

        // Every level of an event (Blood Castle 1..8) reports its own opening. Each one waits a random
        // time; the first one which finds a qualified bot takes the announcement of the whole event type.
        RunLater(
            miniGame.GameContext,
            RandomDelay(TimeSpan.Zero, configuration.MaxPostDelay),
            () => this.PostAboutEventAsync(miniGame, configuration));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask InvasionStartedAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps)
    {
        if (this.Configuration is not { InvasionPostChancePercent: > 0 } configuration
            || maps.Count == 0
            || !IsPostCommandActive(gameContext))
        {
            return ValueTask.CompletedTask;
        }

        RunLater(
            gameContext,
            RandomDelay(TimeSpan.Zero, configuration.MaxPostDelay),
            () => this.PostAboutInvasionAsync(gameContext, invasion, maps, configuration));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        if (player is not OfflinePlayer)
        {
            this.Congratulate(player, configuration, configuration.CongratulationLines, $"congratulation {player.SelectedCharacter?.Name}");
            return;
        }

        if (configuration is not { LevelMilestoneStep: > 0, LevelMilestoneChancePercent: > 0 }
            || !IsBot(player))
        {
            return;
        }

        var level = GetLevel(player);
        if (level < configuration.LevelMilestoneStep
            || level % configuration.LevelMilestoneStep != 0
            || !Rand.NextRandomBool(configuration.LevelMilestoneChancePercent)
            || PickLine(configuration.LevelMilestoneLines) is not { } line
            || !this.GetThrottle(player.GameContext).TryReserveChat(player, DateTime.UtcNow, configuration.BotChatCooldown))
        {
            return;
        }

        RunLater(
            player.GameContext,
            RandomDelay(LevelMilestoneDelaySeconds),
            () => IsAvailable(player) && this.MayTalkUnasked(player, configuration)
                ? BotChatSpeaker.SayAsync(player, line.Text, CreateBotValues(player))
                : ValueTask.CompletedTask);
    }

    /// <inheritdoc />
    public void WhisperMessageReceived(Player sender, Player receiver, string message, CancelEventArgs cancelEventArgs)
    {
        // A bot which answered the sender's party search waits for the meeting place - that whisper is
        // part of a conversation the player started, so it's always answered.
        if (this.Configuration is { } partyConfiguration
            && receiver is OfflinePlayer partyBot
            && IsBot(partyBot)
            && sender is not OfflinePlayer
            && this.GetThrottle(receiver.GameContext).GetOpenPartyOffer(partyBot, DateTime.UtcNow) is { } offer
            && ReferenceEquals(offer.Requester, sender))
        {
            this.AnswerMeetingPlace(partyBot, sender, message, partyConfiguration);
            return;
        }

        if (this.Configuration is not { WhisperReplyChancePercent: > 0 } configuration
            || !IsBot(receiver)
            || sender is OfflinePlayer
            || !Rand.NextRandomBool(configuration.WhisperReplyChancePercent)
            || this.PickReplyFor(sender, configuration.WhisperReplies, message) is not { } line
            || !this.GetThrottle(receiver.GameContext).TryReserveWhisperReply(receiver, DateTime.UtcNow, configuration.WhisperReplyCooldown))
        {
            return;
        }

        var values = CreateBotValues(receiver);
        var senderName = sender.SelectedCharacter?.Name ?? string.Empty;
        values["player"] = _ => senderName;
        RunLater(
            receiver.GameContext,
            TypingDelay(line.Text),
            () => IsAvailable(receiver) && sender.PlayerState.CurrentState == PlayerState.EnteredWorld
                ? BotChatSpeaker.WhisperAsync(receiver, sender, line.Text, values)
                : ValueTask.CompletedTask);
    }

    /// <inheritdoc />
    public ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        // Only entering the game counts - a warp or a map change keeps the player in the world.
        if (this.Configuration is not { GreetingChancePercent: > 0 } configuration
            || player is OfflinePlayer
            || currentState != PlayerState.EnteredWorld
            || previousState != PlayerState.CharacterSelection
            || !Rand.NextRandomBool(configuration.GreetingChancePercent))
        {
            return ValueTask.CompletedTask;
        }

        RunLater(player.GameContext, RandomDelay(GreetingDelaySeconds), () => this.GreetAsync(player, configuration));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void BotJoinedParty(Player bot, Player inviter)
    {
        if (this.Configuration is not { PartyChatChancePercent: > 0 } configuration
            || !Rand.NextRandomBool(configuration.PartyChatChancePercent)
            || PickLine(configuration.PartyJoinedLines) is not { } line
            || !this.GetThrottle(bot.GameContext).TryReserveChat(bot, DateTime.UtcNow, configuration.BotChatCooldown))
        {
            return;
        }

        var values = CreateBotValues(bot);
        var inviterName = inviter.SelectedCharacter?.Name ?? string.Empty;
        values["player"] = _ => inviterName;
        RunLater(bot.GameContext, TypingDelay(line.Text), () => BotChatSpeaker.PartySayAsync(bot, line.Text, values));
    }

    /// <inheritdoc />
    public ValueTask MiniGameEndedAsync(MiniGameContext miniGame, Player? winner, IReadOnlyCollection<Player> finishers)
    {
        if (this.Configuration is not { PartyChatChancePercent: > 0 } configuration)
        {
            return ValueTask.CompletedTask;
        }

        // One bot per party with a player: a whole chorus of "gg" from the bots would give them away.
        var speakers = finishers
            .Where(p => IsBot(p) && p.Party is not null && BotPartyHandler.HasHumanCompanion(p))
            .GroupBy(p => p.Party)
            .Select(party => party.First())
            .ToList();
        var definition = miniGame.Definition;
        foreach (var bot in speakers)
        {
            if (!Rand.NextRandomBool(configuration.PartyChatChancePercent)
                || PickLine(configuration.EventFinishedLines) is not { } line
                || !this.GetThrottle(bot.GameContext).TryReserveChat(bot, DateTime.UtcNow, configuration.BotChatCooldown))
            {
                continue;
            }

            var values = CreateBotValues(bot);
            values["event"] = culture => definition.Name.GetTranslation(culture) ?? string.Empty;
            RunLater(bot.GameContext, TypingDelay(line.Text), () => BotChatSpeaker.PartySayAsync(bot, line.Text, values));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        // Same cases as the revenge (BotRevengePlugIn): a human player's kill outside of an event,
        // where dying is part of the game.
        if (this.Configuration is not { KilledByPlayerChancePercent: > 0 } configuration
            || killed is not OfflinePlayer bot
            || !IsBot(bot)
            || bot.CurrentMiniGame is not null
            || killer is not Player killerPlayer
            || killerPlayer is OfflinePlayer
            || !Rand.NextRandomBool(configuration.KilledByPlayerChancePercent)
            || !this.GetThrottle(bot.GameContext).TryReserveChat(bot, DateTime.UtcNow, configuration.BotChatCooldown))
        {
            return ValueTask.CompletedTask;
        }

        RunLater(bot.GameContext, RandomDelay(KilledDelaySeconds), () =>
        {
            // Read after the delay: the revenge plugin registers the death in the same notification.
            var lines = bot.GetRecentDeathCount(killerPlayer) >= OfflinePlayer.RepeatedKillThreshold ? configuration.KilledAgainLines : configuration.KilledLines;
            if (!this.MayTalkUnasked(bot, configuration)
                || this.PickLineFor(killerPlayer, lines) is not { } line)
            {
                return ValueTask.CompletedTask;
            }

            var values = CreateBotValues(bot);
            var killerName = killerPlayer.SelectedCharacter?.Name ?? string.Empty;
            values["player"] = _ => killerName;
            return BotChatSpeaker.SayAsync(bot, line.Text, values);
        });
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask ChatMessageSentAsync(Player sender, string message, ChatMessageType messageType, Player? receiver)
    {
        if (this.Configuration is { BusyChatMessageCount: > 0 } busyConfiguration
            && messageType == ChatMessageType.Normal
            && sender is not OfflinePlayer
            && sender.CurrentMap is { } senderMap)
        {
            this.GetThrottle(sender.GameContext).RecordHumanChat(senderMap, sender.Position, DateTime.UtcNow, busyConfiguration.BusyChatWindow);
        }

        if (this.Configuration is not { WhisperReplyChancePercent: > 0 } configuration
            || messageType != ChatMessageType.Normal
            || sender is OfflinePlayer
            || sender.SelectedCharacter?.Name is not { } senderName)
        {
            return;
        }

        // Only a bot which sees the speaker (and so got the message) may feel addressed.
        var mentionedBots = new List<Player>();
        await sender.ForEachObservingAsync<Player>(
            observer =>
            {
                if (IsBot(observer) && observer.SelectedCharacter?.Name is { } botName && BotChatComposer.MentionsName(message, botName))
                {
                    mentionedBots.Add(observer);
                }

                return ValueTask.CompletedTask;
            },
            false).ConfigureAwait(false);

        if (mentionedBots.FirstOrDefault(IsAvailable) is not { } bot
            || !Rand.NextRandomBool(configuration.WhisperReplyChancePercent)
            || this.PickReplyFor(sender, configuration.WhisperReplies, message) is not { } line
            || !this.GetThrottle(bot.GameContext).TryReserveWhisperReply(bot, DateTime.UtcNow, configuration.WhisperReplyCooldown))
        {
            return;
        }

        var values = CreateBotValues(bot);
        values["player"] = _ => senderName;
        RunLater(
            bot.GameContext,
            TypingDelay(line.Text),
            () => IsAvailable(bot) ? BotChatSpeaker.SayAsync(bot, line.Text, values) : ValueTask.CompletedTask);
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this.Configuration is not { } configuration)
        {
            return;
        }

        await this.MakeMerchantTalkAsync(gameContext, configuration).ConfigureAwait(false);
        if (this.GetThrottle(gameContext).TryStartPlayerScan(DateTime.UtcNow))
        {
            var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
            await this.HoldConversationsAsync(gameContext, configuration, players).ConfigureAwait(false);
            this.CongratulateResets(players, configuration);
        }
    }

    /// <inheritdoc />
    public void ForceStart() => Interlocked.Increment(ref this._forceStartCount);

    /// <inheritdoc />
    public ValueTask TradeRequestedAsync(Player requester, Player partner)
    {
        // The bot has no trade window, so the request goes nowhere - the bot says why.
        if (this.Configuration is { } configuration
            && IsBot(partner)
            && requester is not OfflinePlayer)
        {
            this.DeclineLater(partner, requester, configuration, configuration.TradeDeclineLines, null);
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DuelRequestedAsync(Player requester, Player target)
    {
        // A bot never answered a duel request, which left the challenger waiting in vain: it refuses,
        // like a player clicking "no", and says why.
        if (this.Configuration is { } configuration
            && IsBot(target)
            && requester is not OfflinePlayer)
        {
            this.DeclineLater(target, requester, configuration, configuration.DuelDeclineLines, () => RefuseDuelAsync(target, requester));
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask PostMessageSentAsync(Player sender, string message)
    {
        if (this.Configuration is not { PartyVolunteerCount: > 0 } configuration
            || sender is OfflinePlayer
            || sender.SelectedCharacter?.Name is not { } senderName
            || BotPartyRequest.TryParse(message, configuration.PartyRequestKeywords, configuration.PartyClassAliases) is not { } request)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var throttle = this.GetThrottle(sender.GameContext);
        var classes = sender.GameContext.Configuration.CharacterClasses;
        var players = await sender.GameContext.GetPlayersAsync().ConfigureAwait(false);
        var candidates = players
            .OfType<OfflinePlayer>()
            .Where(bot => IsBot(bot)
                          && IsAvailable(bot)
                          && IsFreeToMeet(bot)
                          && throttle.GetOpenPartyOffer(bot, now) is null
                          && request.IsWantedLevel(GetLevel(bot))
                          && IsOfWantedClass(bot, request, classes));
        var volunteers = throttle.TakeNewVolunteers(sender, candidates, configuration.PartyVolunteerCount, now, count => Rand.NextInt(0, count));
        var delay = TimeSpan.Zero;
        foreach (var bot in volunteers)
        {
            if (PickLine(configuration.PartyVolunteerLines) is not { } line)
            {
                return;
            }

            throttle.SetPartyOffer(bot, new BotPartyOffer(sender, now + configuration.PartyOfferTimeout));
            delay += RandomDelay(PartyVolunteerSpacingSeconds);
            var whisperDelay = delay + TypingDelay(line.Text);
            var values = CreateBotValues(bot);
            values["player"] = _ => senderName;
            RunLater(
                bot.GameContext,
                whisperDelay,
                () => IsAvailable(bot) && sender.PlayerState.CurrentState == PlayerState.EnteredWorld
                    ? BotChatSpeaker.WhisperAsync(bot, sender, line.Text, values)
                    : ValueTask.CompletedTask);
        }
    }

    /// <summary>
    /// Determines whether the bot may warp to the map according to the warp list - the same rule the
    /// bots' navigation follows (the plain level against the class-reduced warp requirement) - and the
    /// map isn't forbidden for bots.
    /// </summary>
    /// <param name="bot">The bot.</param>
    /// <param name="map">The map.</param>
    /// <returns><c>true</c>, if the bot may warp to the map.</returns>
    internal static bool MayWarpTo(Player bot, GameMapDefinition map)
    {
        if (bot.SelectedCharacter is not { } character)
        {
            return false;
        }

        if (BotFeaturePlugIn.GetConfiguration(bot.GameContext)?.IsMapForbidden(map) == true)
        {
            return false;
        }

        var level = GetLevel(bot);
        return bot.GameContext.Configuration.WarpList.Any(warp =>
            warp.Gate?.Map?.Number == map.Number
            && character.GetEffectiveMoveLevelRequirement(warp.LevelRequirement) <= level);
    }

    /// <summary>
    /// Gets the time of day of an hour of the server's local time (see <see cref="BotChatTimeOfDay"/>).
    /// </summary>
    /// <param name="hour">The hour, 0 to 23.</param>
    /// <returns>The time of day.</returns>
    internal static BotChatTimeOfDay GetTimeOfDay(int hour) => hour switch
    {
        >= 5 and < 11 => BotChatTimeOfDay.Morning,
        >= 11 and < 18 => BotChatTimeOfDay.Day,
        >= 18 and < 23 => BotChatTimeOfDay.Evening,
        _ => BotChatTimeOfDay.Night,
    };

    /// <summary>
    /// Finds the bot which talks to a player - greets the entering player, congratulates them: the
    /// nearest available bot on the player's map, within the range in which the player sees it.
    /// </summary>
    /// <param name="players">The players of the game server.</param>
    /// <param name="player">The entering player.</param>
    /// <param name="map">The player's map.</param>
    /// <returns>The greeting bot, or <c>null</c> when none is close enough.</returns>
    internal static Player? FindGreeter(IEnumerable<Player> players, Player player, GameMap map)
        => players
            .Where(p => IsBot(p) && IsAvailable(p) && ReferenceEquals(p.CurrentMap, map))
            .Select(p => (Bot: p, Distance: p.GetDistanceTo(player)))
            .Where(candidate => candidate.Distance <= HearingRange)
            .OrderBy(candidate => candidate.Distance)
            .Select(candidate => candidate.Bot)
            .FirstOrDefault();

    /// <summary>
    /// Gets the first class of the line the class belongs to, e.g. the Dark Knight for the Blade
    /// Master - by following the class evolution of the configuration backwards.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="classes">All character classes of the configuration.</param>
    /// <returns>The first class of the line.</returns>
    internal static CharacterClass GetFirstClassOfLine(CharacterClass characterClass, ICollection<CharacterClass> classes)
    {
        var current = characterClass;

        // Bounded, so a misconfigured evolution cycle can't hang the game logic.
        for (var step = 0; step < classes.Count; step++)
        {
            if (classes.FirstOrDefault(c => c.NextGenerationClass?.Number == current.Number) is not { } previous)
            {
                break;
            }

            current = previous;
        }

        return current;
    }

    /// <summary>
    /// Gets the time a bot needs to answer with a text of the given length: the reaction, then the
    /// typing of each character - "np" comes quickly, a whole sentence takes a while.
    /// </summary>
    /// <param name="length">The length of the text.</param>
    /// <param name="reaction">The time to notice and read the message.</param>
    /// <param name="perCharacter">The time per typed character.</param>
    /// <returns>The delay, at most <see cref="MaximumTypingTime"/>.</returns>
    internal static TimeSpan GetTypingTime(int length, TimeSpan reaction, TimeSpan perCharacter)
    {
        var time = reaction + (perCharacter * length);
        return time < MaximumTypingTime ? time : MaximumTypingTime;
    }

    private static bool IsBot(Player player) => player is OfflinePlayer && player.Account?.IsBot == true;

    private static bool IsOfWantedClass(Player bot, BotPartyRequest request, ICollection<CharacterClass> classes)
    {
        if (request.ClassNumbers.Count == 0)
        {
            return true;
        }

        if (bot.SelectedCharacter?.CharacterClass is not { } ownClass)
        {
            return false;
        }

        var ownLine = GetFirstClassOfLine(ownClass, classes).Number;
        return request.ClassNumbers.Any(number =>
            classes.FirstOrDefault(c => c.Number == number) is { } wanted
            && GetFirstClassOfLine(wanted, classes).Number == ownLine);
    }

    /// <summary>
    /// Determines whether the bot is free to wait for players at an event: not already waiting, not on
    /// an errand or a revenge, and not in a party with a player - it would look for company it has.
    /// </summary>
    private static bool IsFreeToMeet(OfflinePlayer bot)
        => bot.EventMeeting is null
           && !bot.IsOnShoppingTrip
           && !bot.IsOnBuffTrip
           && !bot.IsOnChaosMachineTrip
           && !bot.HasRevengeIntent
           && !BotPartyHandler.HasHumanCompanion(bot);

    /// <summary>
    /// Determines whether the bot is in the world and free to talk: alive and not inside an event.
    /// </summary>
    private static bool IsAvailable(Player bot)
        => bot.PlayerState.CurrentState == PlayerState.EnteredWorld
           && bot.IsAlive
           && bot.CurrentMiniGame is null;

    private static bool IsInSafezone(Player bot)
        => bot.CurrentMap is { } map && map.Terrain.SafezoneMap[bot.Position.X, bot.Position.Y];

    private static bool IsInHearingRange(Player speaker, Player listener)
        => ReferenceEquals(speaker.CurrentMap, listener.CurrentMap) && speaker.GetDistanceTo(listener) <= HearingRange;

    /// <summary>
    /// Finds two bots for a conversation among the bots near a player: a random pair standing close
    /// together, both in a safezone (a town) or both outside of it (out hunting).
    /// </summary>
    private static (Player First, Player Second, BotConversationPlace Place)? FindConversationPair(IReadOnlyList<Player> bots)
    {
        var pairs = new List<(Player First, Player Second, BotConversationPlace Place)>();
        for (var i = 0; i < bots.Count; i++)
        {
            for (var j = i + 1; j < bots.Count; j++)
            {
                var (a, b) = (bots[i], bots[j]);
                var inTown = IsInSafezone(a);
                if (ReferenceEquals(a.CurrentMap, b.CurrentMap)
                    && a.GetDistanceTo(b) <= ConversationPairRange
                    && inTown == IsInSafezone(b))
                {
                    var place = inTown ? BotConversationPlace.Town : BotConversationPlace.Hunting;

                    // Random order, so it's not always the same bot which starts.
                    pairs.Add(Rand.NextRandomBool() ? (a, b, place) : (b, a, place));
                }
            }
        }

        return pairs.Count == 0 ? null : pairs[Rand.NextInt(0, pairs.Count)];
    }

    private static async ValueTask<bool> HasHumanObserverAsync(Player bot)
    {
        var seen = false;
        await bot.ForEachObservingAsync<Player>(
            observer =>
            {
                seen |= observer is not OfflinePlayer;
                return ValueTask.CompletedTask;
            },
            false).ConfigureAwait(false);
        return seen;
    }

    private static int GetLevel(Player player) => (int)(player.Attributes?[Stats.Level] ?? 0);

    private static bool IsPostCommandActive(IGameContext gameContext)
        => gameContext.PlugInManager.IsPlugInActive(typeof(PostChatCommandPlugIn));

    private static BotChatLine? PickLine(IEnumerable<BotChatLine> lines)
        => BotChatComposer.PickLine(lines, GetTimeOfDay(DateTime.Now.Hour), count => Rand.NextInt(0, count));

    /// <summary>
    /// Gets a random, human-like time to answer with the text (see <see cref="GetTypingTime"/>). The
    /// English text stands in for all translations - they are about as long.
    /// </summary>
    private static TimeSpan TypingDelay(LocalizedString text)
        => GetTypingTime(
            text.ValueInNeutralLanguage.Length,
            RandomDelay(TypingReactionSeconds),
            TimeSpan.FromMilliseconds(Rand.NextInt(TypingMillisecondsPerCharacter.Min, TypingMillisecondsPerCharacter.Max)));

    private static TimeSpan RandomDelay(TimeSpan min, TimeSpan max)
        => max <= min ? min : min + TimeSpan.FromMilliseconds(Rand.NextDouble() * (max - min).TotalMilliseconds);

    private static TimeSpan RandomDelay((int Min, int Max) seconds)
        => RandomDelay(TimeSpan.FromSeconds(seconds.Min), TimeSpan.FromSeconds(seconds.Max));

    private static Dictionary<string, Func<CultureInfo, string>> CreateBotValues(Player bot)
    {
        var level = GetLevel(bot).ToString(CultureInfo.InvariantCulture);
        var map = bot.CurrentMap?.Definition.Name;
        var position = bot.Position;
        var characterClass = bot.SelectedCharacter?.CharacterClass?.Name;
        return new Dictionary<string, Func<CultureInfo, string>>
        {
            ["level"] = _ => level,
            ["class"] = culture => characterClass?.GetTranslation(culture) ?? string.Empty,
            ["map"] = culture => map?.GetTranslation(culture) ?? string.Empty,
            ["location"] = culture => BotChatComposer.FormatLocation(map?.GetTranslation(culture), position),
        };
    }

    /// <summary>
    /// Runs the action after the delay, without holding up the caller - the plugin points are called
    /// from the game logic, which must not wait for a bot's "typing time".
    /// </summary>
    private static void RunLater(IGameContext gameContext, TimeSpan delay, Func<ValueTask> action)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay).ConfigureAwait(false);
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                gameContext.LoggerFactory.CreateLogger<BotChatPlugIn>().LogError(ex, "Bot chat failed.");
            }
        });
    }

    /// <summary>
    /// Gets the display name of an invasion in the receiver's language, from the name of its plugin.
    /// </summary>
    private static string GetInvasionName(IPeriodicTaskPlugIn invasion, CultureInfo culture)
    {
        var displayName = invasion.GetType().GetCustomAttribute<DisplayAttribute>()?.Name;
        return displayName is null
            ? invasion.GetType().Name
            : PlugInResources.ResourceManager.GetString(displayName, culture) ?? displayName;
    }

    /// <summary>
    /// Refuses the duel the player challenged the bot to - unless the challenge is already over, e.g.
    /// the player took it back meanwhile.
    /// </summary>
    private static ValueTask RefuseDuelAsync(Player bot, Player requester)
        => bot.DuelRoom is { State: DuelState.DuelRequested } duelRoom && ReferenceEquals(duelRoom.Requester, requester)
            ? new DuelActions().HandleDuelResponseAsync(bot, requester, false)
            : ValueTask.CompletedTask;

    private BotChatThrottle GetThrottle(IGameContext gameContext) => this._throttles.GetOrCreateValue(gameContext);

    /// <summary>
    /// Determines whether the bot may say something nobody asked for in the normal chat: not while
    /// the human players around it are busy talking - the bots shouldn't drown them out.
    /// </summary>
    private bool MayTalkUnasked(Player bot, BotChatConfiguration configuration)
        => bot.CurrentMap is not { } map
           || !this.GetThrottle(bot.GameContext).IsHumanChatBusy(map, bot.Position, HearingRange, DateTime.UtcNow, configuration.BusyChatWindow, configuration.BusyChatMessageCount);

    /// <summary>
    /// Picks a line said to the player, other than the one of this list the player got last time.
    /// </summary>
    private BotChatLine? PickLineFor(Player listener, ICollection<BotChatLine> lines)
    {
        var throttle = this.GetThrottle(listener.GameContext);
        var line = BotChatComposer.PickLine(lines, GetTimeOfDay(DateTime.Now.Hour), count => Rand.NextInt(0, count), throttle.GetLastLine(listener, lines));
        if (line is not null)
        {
            throttle.SetLastLine(listener, lines, line);
        }

        return line;
    }

    /// <summary>
    /// Picks the reply to the player's message, other than the reply the player got last time.
    /// </summary>
    private BotChatReply? PickReplyFor(Player listener, ICollection<BotChatReply> replies, string message)
    {
        var throttle = this.GetThrottle(listener.GameContext);
        var reply = BotChatComposer.PickWhisperReply(replies, message, count => Rand.NextInt(0, count), throttle.GetLastLine(listener, replies));
        if (reply is not null)
        {
            throttle.SetLastLine(listener, replies, reply);
        }

        return reply;
    }

    /// <summary>
    /// Answers the whisper in which a player, whose party search the bot answered, names the meeting
    /// place: "devias 215 40", or "here" for where the player stands. A bot which may go there sets
    /// off and waits for the invitation (see <see cref="BotEventMeeting"/>); a place it may not go to
    /// is declined, and a whisper without a place gets asked again.
    /// </summary>
    private void AnswerMeetingPlace(OfflinePlayer bot, Player requester, string message, BotChatConfiguration configuration)
    {
        var throttle = this.GetThrottle(bot.GameContext);
        var values = CreateBotValues(bot);
        var requesterName = requester.SelectedCharacter?.Name ?? string.Empty;
        values["player"] = _ => requesterName;

        // A named map wins over "here": "i'm here, devias 215 40" names the place.
        var place = BotChatComposer.FindPlace(message, bot.GameContext.Configuration.Maps);
        if (place is null
            && BotChatComposer.ContainsAnyKeyword(message, configuration.PartyHereKeywords)
            && requester.CurrentMap is { } requesterMap)
        {
            place = (requesterMap.Definition, requester.Position);
        }

        if (place is not { } found)
        {
            if (throttle.TryReserveWhisperReply(bot, DateTime.UtcNow, configuration.WhisperReplyCooldown)
                && this.PickLineFor(requester, configuration.PartyAskWhereLines) is { } askLine)
            {
                this.WhisperLater(bot, requester, askLine, values);
            }

            return;
        }

        var (map, position) = found;
        throttle.RemovePartyOffer(bot);
        values["map"] = culture => map.Name.GetTranslation(culture) ?? string.Empty;
        values["location"] = culture => BotChatComposer.FormatLocation(map.Name.GetTranslation(culture), position);
        if (!IsAvailable(bot) || !IsFreeToMeet(bot))
        {
            // Taken into another party, or busy otherwise, since it answered: it lets the player be.
            return;
        }

        if (!MayWarpTo(bot, map) || !map.ExitGates.Any(gate => gate.IsSpawnGate))
        {
            if (PickLine(configuration.PartyCantGoLines) is { } cantGoLine)
            {
                this.WhisperLater(bot, requester, cantGoLine, values);
            }

            return;
        }

        bot.EventMeeting = new BotEventMeeting(map, position, DateTime.UtcNow + configuration.PartyMeetingWaitTime);
        if (PickLine(configuration.PartyOnTheWayLines) is { } onTheWayLine)
        {
            this.WhisperLater(bot, requester, onTheWayLine, values);
        }
    }

    /// <summary>
    /// Declines a player's request after a human-like moment: runs the refusal, if any, and whispers
    /// one of the lines - by chance, and not more often than the whisper reply cooldown allows.
    /// </summary>
    private void DeclineLater(Player bot, Player requester, BotChatConfiguration configuration, ICollection<BotChatLine> lines, Func<ValueTask>? refuse)
    {
        var whisper = configuration.DeclineReplyChancePercent > 0
                      && Rand.NextRandomBool(configuration.DeclineReplyChancePercent)
                      && this.GetThrottle(bot.GameContext).TryReserveWhisperReply(bot, DateTime.UtcNow, configuration.WhisperReplyCooldown);
        var line = whisper ? this.PickLineFor(requester, lines) : null;
        var values = CreateBotValues(bot);
        var requesterName = requester.SelectedCharacter?.Name ?? string.Empty;
        values["player"] = _ => requesterName;
        RunLater(bot.GameContext, line is null ? RandomDelay(TypingReactionSeconds) : TypingDelay(line.Text), async () =>
        {
            if (refuse is not null)
            {
                await refuse().ConfigureAwait(false);
            }

            if (line is not null && requester.PlayerState.CurrentState == PlayerState.EnteredWorld)
            {
                await BotChatSpeaker.WhisperAsync(bot, requester, line.Text, values).ConfigureAwait(false);
            }
        });
    }

    private void WhisperLater(Player bot, Player receiver, BotChatLine line, Dictionary<string, Func<CultureInfo, string>> values)
        => RunLater(
            bot.GameContext,
            TypingDelay(line.Text),
            () => receiver.PlayerState.CurrentState == PlayerState.EnteredWorld
                ? BotChatSpeaker.WhisperAsync(bot, receiver, line.Text, values)
                : ValueTask.CompletedTask);

    private async ValueTask MakeMerchantTalkAsync(GameContext gameContext, BotChatConfiguration configuration)
    {
        if (configuration.MerchantTalkChancePercent <= 0
            || !this.GetThrottle(gameContext).TryStartMerchantTalk(DateTime.UtcNow, configuration.MerchantTalkInterval, Volatile.Read(ref this._forceStartCount))
            || !Rand.NextRandomBool(configuration.MerchantTalkChancePercent))
        {
            return;
        }

        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        var candidates = new List<Player>();
        foreach (var bot in players.Where(p => IsBot(p) && IsAvailable(p) && IsInSafezone(p)))
        {
            // Talking to nobody would be wasted: only a bot which a player sees may talk.
            if (await HasHumanObserverAsync(bot).ConfigureAwait(false))
            {
                candidates.Add(bot);
            }
        }

        if (candidates.Count == 0
            || PickLine(configuration.MerchantLines) is not { } line)
        {
            return;
        }

        var speaker = candidates[Rand.NextInt(0, candidates.Count)];
        if (this.MayTalkUnasked(speaker, configuration)
            && this.GetThrottle(gameContext).TryReserveChat(speaker, DateTime.UtcNow, configuration.BotChatCooldown))
        {
            await BotChatSpeaker.SayAsync(speaker, line.Text, CreateBotValues(speaker)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Lets two bots near a human player talk, for every player whose interval is over: the pair
    /// stands close together - both in a town or both out hunting - and the conversation fits the
    /// place. Every player who hears it waits a whole interval for the next one.
    /// </summary>
    private async ValueTask HoldConversationsAsync(GameContext gameContext, BotChatConfiguration configuration, IList<Player> players)
    {
        var now = DateTime.UtcNow;
        var throttle = this.GetThrottle(gameContext);
        if (configuration.ConversationChancePercent <= 0
            || configuration.Conversations.Count == 0)
        {
            return;
        }

        var humans = players.Where(p => p is not OfflinePlayer && p.PlayerState.CurrentState == PlayerState.EnteredWorld && p.CurrentMap is not null).ToList();
        var bots = players.Where(p => IsBot(p) && IsAvailable(p)).ToList();
        foreach (var human in humans)
        {
            if (!throttle.IsConversationDue(human, now, configuration.ConversationInterval)
                || FindConversationPair(bots.Where(bot => IsInHearingRange(bot, human)).ToList()) is not { } pair)
            {
                continue;
            }

            // Whoever hears this one waits a whole interval - also when the dice said no.
            foreach (var listener in humans.Where(listener => IsInHearingRange(pair.First, listener) || IsInHearingRange(pair.Second, listener)))
            {
                throttle.PostponeConversation(listener, now + configuration.ConversationInterval);
            }

            var timeOfDay = GetTimeOfDay(DateTime.Now.Hour);
            var candidates = configuration.Conversations
                .Where(c => (c.Where == BotConversationPlace.Anywhere || c.Where == pair.Place)
                            && BotChatComposer.FitsTimeOfDay(c.When, timeOfDay)
                            && !string.IsNullOrWhiteSpace(c.Opener.ValueInNeutralLanguage)
                            && !string.IsNullOrWhiteSpace(c.Reply.ValueInNeutralLanguage))
                .ToList();
            if (!Rand.NextRandomBool(configuration.ConversationChancePercent)
                || !this.MayTalkUnasked(pair.First, configuration)
                || !throttle.TryReserveChat(pair.First, now, configuration.BotChatCooldown)
                || !throttle.TryReserveChat(pair.Second, now, configuration.BotChatCooldown)
                || throttle.DrawConversation(candidates, count => Rand.NextInt(0, count)) is not { } conversation)
            {
                continue;
            }

            var (first, second) = (pair.First, pair.Second);
            var firstValues = CreateBotValues(first);
            var secondName = second.SelectedCharacter?.Name ?? string.Empty;
            firstValues["partner"] = _ => secondName;
            var secondValues = CreateBotValues(second);
            var firstName = first.SelectedCharacter?.Name ?? string.Empty;
            secondValues["partner"] = _ => firstName;

            await BotChatSpeaker.SayAsync(first, conversation.Opener, firstValues).ConfigureAwait(false);
            RunLater(
                gameContext,
                TypingDelay(conversation.Reply),
                () => IsAvailable(second) ? BotChatSpeaker.SayAsync(second, conversation.Reply, secondValues) : ValueTask.CompletedTask);
        }
    }

    /// <summary>
    /// Lets the nearest bot congratulate the players whose reset count grew since the last look - the
    /// reset may log the player out to the character selection or not, so it's noticed by the count.
    /// </summary>
    private void CongratulateResets(IEnumerable<Player> players, BotChatConfiguration configuration)
    {
        foreach (var player in players)
        {
            if (player is OfflinePlayer
                || player.SelectedCharacter?.Name is not { } name
                || player.Attributes is not { } attributes)
            {
                continue;
            }

            var resets = (int)attributes[Stats.Resets];
            if (this.GetThrottle(player.GameContext).TryUpdateKnownResets(name, resets))
            {
                this.Congratulate(player, configuration, configuration.ResetCongratulationLines, $"reset congratulation {name}");
            }
        }
    }

    /// <summary>
    /// Lets the nearest bot congratulate a player in the normal chat - "gz" - after a moment, at most
    /// once per player and cooldown for each kind of occasion.
    /// </summary>
    private void Congratulate(Player player, BotChatConfiguration configuration, ICollection<BotChatLine> lines, string occasionKey)
    {
        if (configuration.CongratulationChancePercent <= 0
            || player.SelectedCharacter?.Name is not { } playerName
            || !Rand.NextRandomBool(configuration.CongratulationChancePercent))
        {
            return;
        }

        var playerLevel = GetLevel(player).ToString(CultureInfo.InvariantCulture);
        var playerResets = ((int)(player.Attributes?[Stats.Resets] ?? 0)).ToString(CultureInfo.InvariantCulture);
        RunLater(player.GameContext, RandomDelay(CongratulationDelaySeconds), async () =>
        {
            if (player.PlayerState.CurrentState != PlayerState.EnteredWorld || player.CurrentMap is not { } map)
            {
                return;
            }

            var now = DateTime.UtcNow;
            var throttle = this.GetThrottle(player.GameContext);
            var players = await player.GameContext.GetPlayersAsync().ConfigureAwait(false);
            if (FindGreeter(players, player, map) is not { } bot
                || !this.MayTalkUnasked(bot, configuration)
                || this.PickLineFor(player, lines) is not { } line
                || !throttle.TryReserveOccasion(occasionKey, now, configuration.CongratulationCooldown)
                || !throttle.TryReserveChat(bot, now, configuration.BotChatCooldown))
            {
                return;
            }

            var values = CreateBotValues(bot);
            values["player"] = _ => playerName;
            values["level"] = _ => playerLevel;
            values["resets"] = _ => playerResets;
            await BotChatSpeaker.SayAsync(bot, line.Text, values).ConfigureAwait(false);
        });
    }

    private async ValueTask GreetAsync(Player player, BotChatConfiguration configuration)
    {
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || player.CurrentMap is not { } map
            || player.SelectedCharacter?.Name is not { } playerName)
        {
            return;
        }

        var players = await player.GameContext.GetPlayersAsync().ConfigureAwait(false);
        var bot = FindGreeter(players, player, map);
        var now = DateTime.UtcNow;
        var throttle = this.GetThrottle(player.GameContext);
        if (bot is null
            || !this.MayTalkUnasked(bot, configuration)
            || this.PickLineFor(player, configuration.GreetingLines) is not { } line
            || !throttle.TryReserveOccasion($"greeting {playerName}", now, configuration.GreetingCooldown)
            || !throttle.TryReserveChat(bot, now, configuration.BotChatCooldown))
        {
            return;
        }

        var values = CreateBotValues(bot);
        values["player"] = _ => playerName;
        await BotChatSpeaker.SayAsync(bot, line.Text, values).ConfigureAwait(false);
    }

    private async ValueTask PostAboutEventAsync(MiniGameContext miniGame, BotChatConfiguration configuration)
    {
        var now = DateTime.UtcNow;
        if (now >= miniGame.EnterEndsAtUtc)
        {
            return;
        }

        // A player reading the post needs the time to get to the meeting point.
        if (miniGame.EnterEndsAtUtc - now < MinimumMeetingTime)
        {
            return;
        }

        var definition = miniGame.Definition;
        var players = await miniGame.GameContext.GetPlayersAsync().ConfigureAwait(false);
        var candidates = players
            .OfType<OfflinePlayer>()
            .Where(bot => IsBot(bot) && IsAvailable(bot) && IsFreeToMeet(bot) && BotMiniGameHandler.IsEligible(bot, definition, out _))
            .Select(bot => (Bot: bot, Meeting: BotEventMeetingPlanner.TryPlan(bot, definition.Type, miniGame.EnterEndsAtUtc)))
            .Where(candidate => candidate.Meeting is not null)
            .ToList();
        var throttle = this.GetThrottle(miniGame.GameContext);
        if (candidates.Count == 0
            || !throttle.TryReserveOccasion($"mini game {definition.Type}", now, miniGame.EnterEndsAtUtc - now)
            || !Rand.NextRandomBool(configuration.EventPostChancePercent)
            || PickLine(configuration.EventLines) is not { } line
            || !throttle.TryReservePost(now, configuration.MinTimeBetweenPosts))
        {
            return;
        }

        var (chosen, meeting) = candidates[Rand.NextInt(0, candidates.Count)];
        chosen.EventMeeting = meeting;
        var values = CreateBotValues(chosen);
        values["event"] = culture => definition.Name.GetTranslation(culture) ?? string.Empty;
        values["map"] = culture => meeting!.Map.Name.GetTranslation(culture) ?? string.Empty;
        values["location"] = culture => BotChatComposer.FormatLocation(meeting!.Map.Name.GetTranslation(culture), meeting.NpcPosition);
        await BotChatSpeaker.PostAsync(chosen, line.Text, values).ConfigureAwait(false);
    }

    private async ValueTask PostAboutInvasionAsync(IGameContext gameContext, IPeriodicTaskPlugIn invasion, IReadOnlyCollection<GameMapDefinition> maps, BotChatConfiguration configuration)
    {
        var now = DateTime.UtcNow;
        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        var candidates = players
            .Where(p => IsBot(p) && IsAvailable(p))
            .SelectMany(p => maps.Where(map => MayWarpTo(p, map)).Select(map => (Bot: p, Map: map)))
            .ToList();
        var throttle = this.GetThrottle(gameContext);
        if (candidates.Count == 0
            || !throttle.TryReserveOccasion($"invasion {invasion.GetType().Name}", now, InvasionAnnouncementWindow)
            || !Rand.NextRandomBool(configuration.InvasionPostChancePercent)
            || PickLine(configuration.InvasionLines) is not { } line
            || !throttle.TryReservePost(now, configuration.MinTimeBetweenPosts))
        {
            return;
        }

        var (bot, map) = candidates[Rand.NextInt(0, candidates.Count)];
        var values = CreateBotValues(bot);
        values["event"] = culture => GetInvasionName(invasion, culture);
        values["map"] = culture => map.Name.GetTranslation(culture) ?? string.Empty;
        values["location"] = values["map"];
        await BotChatSpeaker.PostAsync(bot, line.Text, values).ConfigureAwait(false);
    }
}
