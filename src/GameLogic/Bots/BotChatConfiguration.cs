// <copyright file="BotChatConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Globalization;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Configuration of the <see cref="BotChatPlugIn"/>: what the server-side bots say, and how often.
/// </summary>
/// <remarks>
/// The limits are what keeps a few hundred bots from flooding the chat. The <c>/post</c> messages
/// reach the whole server, so they are the rarest: one server-wide gap
/// (<see cref="MinTimeBetweenPosts"/>) applies to all bots together, and each opened event or
/// started invasion is announced at most once. Both post chances are 30 % by default, so not every
/// opened event or invasion gets a post.
/// </remarks>
public class BotChatConfiguration
{
    /// <summary>
    /// Gets or sets the chance, in percent, that a bot announces with <c>/post</c> that the entrance
    /// of a Blood Castle, Devil Square or Chaos Castle opened. Rolled once per event type and
    /// opening, not per event level.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventPostChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventPostChancePercent_Description))]
    public int EventPostChancePercent { get; set; } = 30;

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot announces a started invasion with <c>/post</c>.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_InvasionPostChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_InvasionPostChancePercent_Description))]
    public int InvasionPostChancePercent { get; set; } = 30;

    /// <summary>
    /// Gets or sets the minimum time between two <c>/post</c> messages of bots on one game server.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MinTimeBetweenPosts_Name), Description = nameof(PlugInResources.BotChatConfiguration_MinTimeBetweenPosts_Description))]
    public TimeSpan MinTimeBetweenPosts { get; set; } = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Gets or sets the longest time a bot waits before it posts about an event. The actual delay is
    /// random up to this value, so the post doesn't arrive in the same second as the event's own
    /// announcement.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MaxPostDelay_Name), Description = nameof(PlugInResources.BotChatConfiguration_MaxPostDelay_Description))]
    public TimeSpan MaxPostDelay { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// Gets or sets every how many levels a bot may comment on its level-up; 0 switches it off.
    /// </summary>
    [Range(0, 1000)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneStep_Name), Description = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneStep_Description))]
    public int LevelMilestoneStep { get; set; } = 50;

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot comments on reaching a level milestone.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneChancePercent_Description))]
    public int LevelMilestoneChancePercent { get; set; } = 25;

    /// <summary>
    /// Gets or sets the minimum time between two normal chat lines of the same bot.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_BotChatCooldown_Name), Description = nameof(PlugInResources.BotChatConfiguration_BotChatCooldown_Description))]
    public TimeSpan BotChatCooldown { get; set; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot answers a whisper of a player.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WhisperReplyChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_WhisperReplyChancePercent_Description))]
    public int WhisperReplyChancePercent { get; set; } = 100;

    /// <summary>
    /// Gets or sets the minimum time between two whisper replies of the same bot. A bot which
    /// answered recently stays silent, so a player can't keep it talking.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WhisperReplyCooldown_Name), Description = nameof(PlugInResources.BotChatConfiguration_WhisperReplyCooldown_Description))]
    public TimeSpan WhisperReplyCooldown { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the chance, in percent, that the nearest bot greets a player who enters the game.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_GreetingChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_GreetingChancePercent_Description))]
    public int GreetingChancePercent { get; set; } = 50;

    /// <summary>
    /// Gets or sets the minimum time between two greetings of the same player, so logging out and in
    /// again doesn't start a round of greetings.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_GreetingCooldown_Name), Description = nameof(PlugInResources.BotChatConfiguration_GreetingCooldown_Description))]
    public TimeSpan GreetingCooldown { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot says something in the party chat of a player:
    /// thanks for the invitation, "gg" after an event.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyChatChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyChatChancePercent_Description))]
    public int PartyChatChancePercent { get; set; } = 70;

    /// <summary>
    /// Gets or sets how many bots answer a player's party search posted with <c>/post</c>, e.g.
    /// "party 300-350 bk" (see <see cref="BotPartyRequest"/>); 0 switches it off. The same search
    /// posted again is answered by other bots.
    /// </summary>
    [Range(0, 20)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerCount_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerCount_Description))]
    public int PartyVolunteerCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the comma-separated words which mark a post as a party search; the post needs a
    /// level as well.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyRequestKeywords_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyRequestKeywords_Description))]
    public string PartyRequestKeywords { get; set; } = "party, pt, grupa, grupy, grupe, grupę, grp";

    /// <summary>
    /// Gets or sets the comma-separated words with which a player tells an answering bot to come to
    /// where the player stands, instead of naming a map and coordinates.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyHereKeywords_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyHereKeywords_Description))]
    public string PartyHereKeywords { get; set; } = "here, come here, to me, tu, tutaj, do mnie";

    /// <summary>
    /// Gets or sets how long a bot which answered a party search waits for the player to whisper
    /// back where to meet.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyOfferTimeout_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyOfferTimeout_Description))]
    public TimeSpan PartyOfferTimeout { get; set; } = TimeSpan.FromMinutes(3);

    /// <summary>
    /// Gets or sets how long a bot waits at the meeting place for the player's party invitation
    /// before it goes back to hunting.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyMeetingWaitTime_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyMeetingWaitTime_Description))]
    public TimeSpan PartyMeetingWaitTime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot killed by a player says something about it.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_KilledByPlayerChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_KilledByPlayerChancePercent_Description))]
    public int KilledByPlayerChancePercent { get; set; } = 70;

    /// <summary>
    /// Gets or sets how often a bot in a safezone may make small talk, per game server.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_SmallTalkInterval_Name), Description = nameof(PlugInResources.BotChatConfiguration_SmallTalkInterval_Description))]
    public TimeSpan SmallTalkInterval { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot makes small talk when the interval is over.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_SmallTalkChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_SmallTalkChancePercent_Description))]
    public int SmallTalkChancePercent { get; set; } = 50;

    /// <summary>
    /// Gets or sets how often a human player may overhear a conversation of two bots near them (see
    /// <see cref="Conversations"/>). The time counts per player: each player who stands next to two
    /// bots hears them talk about once per interval.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_ConversationInterval_Name), Description = nameof(PlugInResources.BotChatConfiguration_ConversationInterval_Description))]
    public TimeSpan ConversationInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the chance, in percent, that two bots near a player talk when the interval is over.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_ConversationChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_ConversationChancePercent_Description))]
    public int ConversationChancePercent { get; set; } = 100;

    /// <summary>
    /// Gets or sets the lines a bot posts with <c>/post</c> when the entrance of a Blood Castle, Devil Square
    /// or Chaos Castle opened, before it waits for players at the meeting point (see <see cref="BotEventMeeting"/>).
    /// Placeholders: <c>{event}</c>, <c>{map}</c> (the map of the meeting point), <c>{location}</c>
    /// (the meeting point with its coordinates), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventLines_Description))]
    public ICollection<BotChatLine> EventLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot posts with <c>/post</c> when an invasion (White Wizard, Golden, Red Dragon)
    /// started. Placeholders: <c>{event}</c>, <c>{map}</c>, <c>{location}</c> (the map alone), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Description))]
    public ICollection<BotChatLine> InvasionLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the normal chat when it reached a level milestone
    /// (see <see cref="LevelMilestoneStep"/>). Placeholders: <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Description))]
    public ICollection<BotChatLine> LevelMilestoneLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the answers of a bot to a player who whispered to it, or mentioned its name in the normal
    /// chat next to it. The bot answers the same way: by whisper, or in the normal chat.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Name), Description = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Description))]
    public ICollection<BotChatReply> WhisperReplies { get; set; } = new List<BotChatReply>();

    /// <summary>
    /// Gets or sets the lines the nearest bot greets a player with who entered the game.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Description))]
    public ICollection<BotChatLine> GreetingLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the party chat after it accepted a player's party invitation.
    /// Placeholders: <c>{player}</c> (the inviter), <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyJoinedLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyJoinedLines_Description))]
    public ICollection<BotChatLine> PartyJoinedLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines one bot per party says in the party chat when an event it went through with a
    /// player's party ended. Placeholders: <c>{event}</c>, <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventFinishedLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventFinishedLines_Description))]
    public ICollection<BotChatLine> EventFinishedLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot answers a player's party search.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{class}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerLines_Description))]
    public ICollection<BotChatLine> PartyVolunteerLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot asks again when it didn't understand where to meet.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{class}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyAskWhereLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyAskWhereLines_Description))]
    public ICollection<BotChatLine> PartyAskWhereLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot confirms that it's on its way to the meeting place.
    /// Placeholders: <c>{player}</c>, <c>{map}</c> and <c>{location}</c> (the meeting place).
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyOnTheWayLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyOnTheWayLines_Description))]
    public ICollection<BotChatLine> PartyOnTheWayLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot declines a meeting place it may not go to - its level
    /// is too low for the map, or the map can't be reached by warp.
    /// Placeholders: <c>{player}</c>, <c>{map}</c> (the named map), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyCantGoLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyCantGoLines_Description))]
    public ICollection<BotChatLine> PartyCantGoLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the words players use for the character classes in a party search.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyClassAliases_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyClassAliases_Description))]
    public ICollection<BotClassAlias> PartyClassAliases { get; set; } = new List<BotClassAlias>();

    /// <summary>
    /// Gets or sets the lines a bot says in the normal chat when a player killed it outside of an event, right
    /// before it respawns. Placeholders: <c>{player}</c> (the killer), <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_KilledLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_KilledLines_Description))]
    public ICollection<BotChatLine> KilledLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says instead of the <see cref="KilledLines"/> when the same player killed
    /// it again within a short time - the bot gives up its revenge (see <see cref="BotRevengePlugIn"/>).
    /// Placeholders as for the <see cref="KilledLines"/>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_KilledAgainLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_KilledAgainLines_Description))]
    public ICollection<BotChatLine> KilledAgainLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot standing in a safezone says now and then in the normal chat (see
    /// <see cref="SmallTalkInterval"/>) - only when a player sees it. Placeholders: <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_SmallTalkLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_SmallTalkLines_Description))]
    public ICollection<BotChatLine> SmallTalkLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the conversations two bots near a player have now and then: one bot says the
    /// opener, the other one answers. They are drawn without repetition until all of a place were
    /// used. Placeholders: <c>{map}</c>, <c>{location}</c>, <c>{level}</c>, <c>{class}</c> of the
    /// speaking bot, and <c>{partner}</c> - the name of the other bot.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_Conversations_Name), Description = nameof(PlugInResources.BotChatConfiguration_Conversations_Description))]
    public ICollection<BotConversation> Conversations { get; set; } = new List<BotConversation>();

    /// <summary>
    /// Creates the configuration a server starts with: a set of English lines, each with a Polish
    /// translation.
    /// </summary>
    /// <returns>The default configuration.</returns>
    /// <remarks>
    /// The class words leave out "me" and "he" (Muse Elf, High Elf): "party 300 for me" would
    /// otherwise call elves only.
    /// The lines are configuration data like the names of the seeded items, not texts of the game
    /// logic: a server owner is expected to rewrite them and to add translations. Event, map and item
    /// names stay English in the Polish lines - the game data names them that way. The keywords hold
    /// the Polish phrases with and without diacritics, since players often type without them.
    /// </remarks>
    public static BotChatConfiguration CreateDefault()
    {
        var configuration = new BotChatConfiguration();
        Add(
            configuration.EventLines,
            ("{event} open, anyone going? im in {map}", "{event} otwarte, ktoś idzie? jestem w {map}"),
            ("lf party {event}, waiting in {map}", "szukam grupy na {event}, czekam w {map}"),
            ("{event} now, who wants to join? {map}", "{event} teraz, kto chętny? {map}"),
            ("going {event}, party anyone? at {map}", "idę na {event}, ktoś do grupy? {map}"));
        Add(
            configuration.InvasionLines,
            ("{event} in {map}!", "{event} w {map}!"),
            ("{event} on {map}, go go", "{event} na {map}, lecimy"),
            ("{map} - {event} started", "{map} - start: {event}"));
        Add(
            configuration.LevelMilestoneLines,
            ("{level} finally :)", "{level} w końcu :)"),
            ("lvl {level}, nice", "lvl {level}, nieźle"),
            ("{level} done", "{level} wbite"));
        AddReplies(
            string.Empty,
            ("sry busy hunting", "sry, poluję"),
            ("hi, cant talk now", "hej, nie mogę teraz gadać"),
            ("hi, what's up?", "hej, co tam?"),
            ("afk a bit", "chwilę afk"));
        Add(
            configuration.GreetingLines,
            ("hi {player}, have a nice game", "siema {player}, miłej gry"),
            ("welcome back {player}", "witaj z powrotem {player}"),
            ("yo {player} :)", "yo {player} :)"),
            ("hey {player}", "hej {player}"));
        Add(
            configuration.PartyJoinedLines,
            ("thx for invite", "dzięki za zaproszenie"),
            ("hi all", "hej wszystkim"),
            ("ty {player}", "dzięki {player}"),
            ("hey, lets go", "hej, lecimy"));
        Add(
            configuration.EventFinishedLines,
            ("gg", "gg"),
            ("gg wp", "gg wp"),
            ("gg, thx for party", "gg, dzięki za grupę"),
            ("nice run", "niezły run"));
        Add(
            configuration.KilledLines,
            ("why??", "czemu??"),
            ("wtf {player}", "wtf {player}"),
            ("really?", "serio?"),
            ("ok {player}, remember that", "ok {player}, zapamiętam"));
        Add(
            configuration.KilledAgainLines,
            ("you again...", "znowu ty..."),
            ("ok ok, i leave", "dobra dobra, idę stąd"),
            ("{player} pls stop", "{player} weź przestań"),
            ("enough, im out", "dość, spadam"));
        Add(
            configuration.SmallTalkLines,
            ("lf party {map}", "szukam grupy {map}"),
            ("lag?", "lag?"),
            ("need a party for exp", "potrzebna grupa na expa"),
            ("anyone up for bc later?", "ktoś na bc później?"),
            ("so many people today", "dużo ludzi dzisiaj"),
            ("gm online?", "jest gm?"));
        Add(
            configuration.PartyVolunteerLines,
            ("me? {level} {class}", "ja? {level} {class}"),
            ("i can come, {class} {level}", "mogę przyjść, {class} {level}"),
            ("{class} {level} here, where are you?", "{class} {level}, gdzie jesteś?"),
            ("count me in, lvl {level}. where?", "wchodzę, lvl {level}. gdzie?"));
        Add(
            configuration.PartyAskWhereLines,
            ("where? write map and coords, e.g. devias 215 40", "gdzie? napisz mapę i koordy, np. devias 215 40"),
            ("which map? like lorencia 130 120", "jaka mapa? np. lorencia 130 120"));
        Add(
            configuration.PartyOnTheWayLines,
            ("omw to {location}", "idę, {location}"),
            ("ok, coming to {location}", "ok, już idę do {location}"),
            ("on my way", "już lecę"));
        Add(
            configuration.PartyCantGoLines,
            ("cant go to {map}, sry", "nie mogę iść do {map}, sry"),
            ("{map} is too much for me", "{map} to za dużo dla mnie"));
        BotConversationDefaults.AddTo(configuration.Conversations);
        AddClass("dk, bk, bm, knight, dark knight, blade knight, blade master, rycerz", BotClassNumbers.DarkKnightNumber);
        AddClass("dw, sm, gm, wizard, dark wizard, soul master, grand master, mag", BotClassNumbers.DarkWizardNumber);
        AddClass("elf, fe, muse, fairy elf, muse elf, high elf, elfka", BotClassNumbers.FairyElfNumber);
        AddClass("mg, dm, gladiator, magic gladiator, duel master", BotClassNumbers.MagicGladiatorNumber);
        AddClass("dl, le, lord, dark lord, lord emperor", BotClassNumbers.DarkLordNumber);
        AddClass("sum, summoner, bs, dim, bloody summoner, dimension master", BotClassNumbers.SummonerNumber);
        AddClass("rf, fm, fighter, rage fighter, fist master", BotClassNumbers.RageFighterNumber);
        AddReplies(
            "how are you, how r u, how are u, hru, how's it going, how is it going, jak sie masz, jak się masz, co slychac, co słychać, co tam",
            ("fine, thanks for asking", "dobrze, dzięki że pytasz"),
            ("good, you?", "spoko, a ty?"),
            ("all good, just grinding", "wszystko ok, expię"));
        AddReplies(
            "hi, hello, hey, yo, hiho, sup, siema, czesc, cześć, hej, elo, witam",
            ("hi, what's up?", "hej, co tam?"),
            ("hey :)", "hej :)"),
            ("hello", "siema"));
        AddReplies(
            "level, lvl, what level, poziom, jaki lvl, jaki poziom",
            ("{level} now", "{level} teraz"),
            ("lvl {level}", "lvl {level}"));
        AddReplies(
            "where are you, where r u, where, gdzie jestes, gdzie jesteś, gdzie",
            ("{location}", "{location}"),
            ("in {location}", "w {location}"));
        AddReplies(
            "party, pt, grupa, grupe, grupę, do grupy",
            ("sure, invite me", "jasne, zaproś mnie"),
            ("ok, send invite", "ok, wysyłaj zaproszenie"));
        AddReplies(
            "thanks, thank you, thx, ty, dzieki, dzięki, dziekuje, dziękuję",
            ("np", "np"),
            ("no problem", "nie ma sprawy"));
        AddReplies(
            "bye, cya, see you, gn, nara, pa, do zobaczenia, dobranoc",
            ("bye", "nara"),
            ("cya", "na razie"));
        return configuration;

        void Add(ICollection<BotChatLine> lines, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                lines.Add(new BotChatLine { Text = Translated(text) });
            }
        }

        void AddReplies(string keywords, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                configuration.WhisperReplies.Add(new BotChatReply { Keywords = keywords, Text = Translated(text) });
            }
        }

        void AddClass(string keywords, byte classNumber)
            => configuration.PartyClassAliases.Add(new BotClassAlias { Keywords = keywords, ClassNumber = classNumber });

        static LocalizedString Translated((string English, string Polish) text)
            => new LocalizedString(text.English).WithTranslation(CultureInfo.GetCultureInfo("pl"), text.Polish);
    }
}
