// <copyright file="BotChatPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views;
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
    ISupportCustomConfiguration<BotChatConfiguration>,
    ISupportDefaultCustomConfiguration,
    IDisabledByDefault
{
    /// <summary>
    /// How close a bot must stand to greet an entering player - roughly the range in which the
    /// player sees it, so the greeting comes from a character on the screen.
    /// </summary>
    private const double GreetingRange = 12;

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
    /// The delay range of a whisper reply - the time a player needs to read and type.
    /// </summary>
    private static readonly (int Min, int Max) WhisperReplyDelaySeconds = (2, 7);

    /// <summary>
    /// The delay range of a greeting: the entering player's client needs a moment to load the world.
    /// </summary>
    private static readonly (int Min, int Max) GreetingDelaySeconds = (3, 8);

    /// <summary>
    /// The delay range of a party chat line - after the party window or the event's score popped up.
    /// </summary>
    private static readonly (int Min, int Max) PartyChatDelaySeconds = (2, 6);

    /// <summary>
    /// The delay range of a comment on being killed: said while the bot still lies there, before it
    /// respawns, so the killer sees it.
    /// </summary>
    private static readonly (int Min, int Max) KilledDelaySeconds = (1, 2);

    private readonly ConditionalWeakTable<IGameContext, BotChatThrottle> _throttles = new();

    /// <summary>
    /// The number of calls of <see cref="ForceStart"/>; a server which didn't see the current number
    /// yet makes its small talk without waiting for the interval.
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
        if (this.Configuration is not { LevelMilestoneStep: > 0, LevelMilestoneChancePercent: > 0 } configuration
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
            () => IsAvailable(player)
                ? BotChatSpeaker.SayAsync(player, line.Text, CreateBotValues(player))
                : ValueTask.CompletedTask);
    }

    /// <inheritdoc />
    public void WhisperMessageReceived(Player sender, Player receiver, string message, CancelEventArgs cancelEventArgs)
    {
        if (this.Configuration is not { WhisperReplyChancePercent: > 0 } configuration
            || !IsBot(receiver)
            || sender is OfflinePlayer
            || !Rand.NextRandomBool(configuration.WhisperReplyChancePercent)
            || BotChatComposer.PickWhisperReply(configuration.WhisperReplies, message, count => Rand.NextInt(0, count)) is not { } line
            || !this.GetThrottle(receiver.GameContext).TryReserveWhisperReply(receiver, DateTime.UtcNow, configuration.WhisperReplyCooldown))
        {
            return;
        }

        var values = CreateBotValues(receiver);
        var senderName = sender.SelectedCharacter?.Name ?? string.Empty;
        values["player"] = _ => senderName;
        RunLater(
            receiver.GameContext,
            RandomDelay(WhisperReplyDelaySeconds),
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
        RunLater(bot.GameContext, RandomDelay(PartyChatDelaySeconds), () => BotChatSpeaker.PartySayAsync(bot, line.Text, values));
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
            RunLater(bot.GameContext, RandomDelay(PartyChatDelaySeconds), () => BotChatSpeaker.PartySayAsync(bot, line.Text, values));
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
            if (PickLine(lines) is not { } line)
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
            || BotChatComposer.PickWhisperReply(configuration.WhisperReplies, message, count => Rand.NextInt(0, count)) is not { } line
            || !this.GetThrottle(bot.GameContext).TryReserveWhisperReply(bot, DateTime.UtcNow, configuration.WhisperReplyCooldown))
        {
            return;
        }

        var values = CreateBotValues(bot);
        values["player"] = _ => senderName;
        RunLater(
            bot.GameContext,
            RandomDelay(WhisperReplyDelaySeconds),
            () => IsAvailable(bot) ? BotChatSpeaker.SayAsync(bot, line.Text, values) : ValueTask.CompletedTask);
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this.Configuration is not { SmallTalkChancePercent: > 0 } configuration
            || !this.GetThrottle(gameContext).TryStartSmallTalk(DateTime.UtcNow, configuration.SmallTalkInterval, Volatile.Read(ref this._forceStartCount))
            || !Rand.NextRandomBool(configuration.SmallTalkChancePercent))
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
            || PickLine(configuration.SmallTalkLines) is not { } line)
        {
            return;
        }

        var speaker = candidates[Rand.NextInt(0, candidates.Count)];
        if (this.GetThrottle(gameContext).TryReserveChat(speaker, DateTime.UtcNow, configuration.BotChatCooldown))
        {
            await BotChatSpeaker.SayAsync(speaker, line.Text, CreateBotValues(speaker)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void ForceStart() => Interlocked.Increment(ref this._forceStartCount);

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
    /// Finds the bot which greets an entering player: the nearest available bot on the player's map,
    /// within the range in which the player sees it.
    /// </summary>
    /// <param name="players">The players of the game server.</param>
    /// <param name="player">The entering player.</param>
    /// <param name="map">The player's map.</param>
    /// <returns>The greeting bot, or <c>null</c> when none is close enough.</returns>
    internal static Player? FindGreeter(IEnumerable<Player> players, Player player, GameMap map)
        => players
            .Where(p => IsBot(p) && IsAvailable(p) && ReferenceEquals(p.CurrentMap, map))
            .Select(p => (Bot: p, Distance: p.GetDistanceTo(player)))
            .Where(candidate => candidate.Distance <= GreetingRange)
            .OrderBy(candidate => candidate.Distance)
            .Select(candidate => candidate.Bot)
            .FirstOrDefault();

    private static bool IsBot(Player player) => player is OfflinePlayer && player.Account?.IsBot == true;

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
        => BotChatComposer.PickLine(lines, count => Rand.NextInt(0, count));

    private static TimeSpan RandomDelay(TimeSpan min, TimeSpan max)
        => max <= min ? min : min + TimeSpan.FromMilliseconds(Rand.NextDouble() * (max - min).TotalMilliseconds);

    private static TimeSpan RandomDelay((int Min, int Max) seconds)
        => RandomDelay(TimeSpan.FromSeconds(seconds.Min), TimeSpan.FromSeconds(seconds.Max));

    private static Dictionary<string, Func<CultureInfo, string>> CreateBotValues(Player bot)
    {
        var level = GetLevel(bot).ToString(CultureInfo.InvariantCulture);
        var map = bot.CurrentMap?.Definition.Name;
        return new Dictionary<string, Func<CultureInfo, string>>
        {
            ["level"] = _ => level,
            ["map"] = culture => map?.GetTranslation(culture) ?? string.Empty,
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
        var displayName = (Attribute.GetCustomAttribute(invasion.GetType(), typeof(DisplayAttribute)) as DisplayAttribute)?.Name;
        return displayName is null
            ? invasion.GetType().Name
            : PlugInResources.ResourceManager.GetString(displayName, culture) ?? displayName;
    }

    private BotChatThrottle GetThrottle(IGameContext gameContext) => this._throttles.GetOrCreateValue(gameContext);

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
            || PickLine(configuration.GreetingLines) is not { } line
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
        await BotChatSpeaker.PostAsync(bot, line.Text, values).ConfigureAwait(false);
    }
}
