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
    /// Gets or sets the lines a bot posts with <c>/post</c> when the entrance of a Blood Castle, Devil Square
    /// or Chaos Castle opened, before it waits for players at the meeting point (see <see cref="BotEventMeeting"/>).
    /// Placeholders: <c>{event}</c>, <c>{map}</c> (the map of the meeting point), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventLines_Description))]
    public ICollection<BotChatLine> EventLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot posts with <c>/post</c> when an invasion (White Wizard, Golden, Red Dragon)
    /// started. Placeholders: <c>{event}</c>, <c>{map}</c>, <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Description))]
    public ICollection<BotChatLine> InvasionLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the normal chat when it reached a level milestone
    /// (see <see cref="LevelMilestoneStep"/>). Placeholders: <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Description))]
    public ICollection<BotChatLine> LevelMilestoneLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the answers of a bot to a player who whispered to it, or mentioned its name in the normal
    /// chat next to it. The bot answers the same way: by whisper, or in the normal chat.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Name), Description = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Description))]
    public ICollection<BotChatReply> WhisperReplies { get; set; } = new List<BotChatReply>();

    /// <summary>
    /// Gets or sets the lines the nearest bot greets a player with who entered the game.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Description))]
    public ICollection<BotChatLine> GreetingLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the party chat after it accepted a player's party invitation.
    /// Placeholders: <c>{player}</c> (the inviter), <c>{level}</c>, <c>{map}</c>.
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
    /// Gets or sets the lines a bot says in the normal chat when a player killed it outside of an event, right
    /// before it respawns. Placeholders: <c>{player}</c> (the killer), <c>{level}</c>, <c>{map}</c>.
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
    /// <see cref="SmallTalkInterval"/>) - only when a player sees it. Placeholders: <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_SmallTalkLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_SmallTalkLines_Description))]
    public ICollection<BotChatLine> SmallTalkLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Creates the configuration a server starts with: a set of English lines, each with a Polish
    /// translation.
    /// </summary>
    /// <returns>The default configuration.</returns>
    /// <remarks>
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
            ("{map}", "{map}"),
            ("in {map}", "w {map}"));
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

        static LocalizedString Translated((string English, string Polish) text)
            => new LocalizedString(text.English).WithTranslation(CultureInfo.GetCultureInfo("pl"), text.Polish);
    }
}
