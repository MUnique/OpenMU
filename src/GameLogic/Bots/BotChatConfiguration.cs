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
    /// Gets or sets the chance, in percent, that a bot in a town where a player stands reminds of an
    /// opened Blood Castle, Devil Square or Chaos Castle in the normal chat - once per town and
    /// opening, and only a bot which meets the entry requirements.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventReminderChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventReminderChancePercent_Description))]
    public int EventReminderChancePercent { get; set; } = 50;

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
    /// Gets or sets the chance, in percent, that the nearest bot congratulates a player on a level-up
    /// or a reset - "gz".
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_CongratulationChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_CongratulationChancePercent_Description))]
    public int CongratulationChancePercent { get; set; } = 30;

    /// <summary>
    /// Gets or sets the minimum time between two congratulations of the same player on a level-up
    /// (and, separately, on a reset) - with a high experience rate, a player levels up every minute.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_CongratulationCooldown_Name), Description = nameof(PlugInResources.BotChatConfiguration_CongratulationCooldown_Description))]
    public TimeSpan CongratulationCooldown { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the chance, in percent, that the nearest bot comments on an excellent or ancient
    /// item a player's kill dropped - "nice drop!".
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_RareDropChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_RareDropChancePercent_Description))]
    public int RareDropChancePercent { get; set; } = 50;

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot comments on wings: its own new pair ("finally,
    /// my wings!"), or the wings a player next to it put on ("nice wings!").
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WingsChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_WingsChancePercent_Description))]
    public int WingsChancePercent { get; set; } = 50;

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot calls out a notable monster which appeared
    /// near it while a player sees it (see <see cref="NotableMonsterNumbers"/>).
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_NotableMonsterChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_NotableMonsterChancePercent_Description))]
    public int NotableMonsterChancePercent { get; set; } = 70;

    /// <summary>
    /// Gets or sets the comma-separated numbers of the monsters a bot calls out when they appear near
    /// it - by default the golden monsters, the Red Dragon and the White Wizard of the invasions.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_NotableMonsterNumbers_Name), Description = nameof(PlugInResources.BotChatConfiguration_NotableMonsterNumbers_Description))]
    public string NotableMonsterNumbers { get; set; } = string.Join(
        ", ",
        new[]
        {
            PlugIns.InvasionEvents.InvasionMonsters.GoldenBudgeDragon,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenTitan,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenSoldier,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenGoblin,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenDragon,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenLizardKing,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenVepar,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenTantallos,
            PlugIns.InvasionEvents.InvasionMonsters.GoldenWheel,
            PlugIns.InvasionEvents.InvasionMonsters.RedDragon,
            PlugIns.InvasionEvents.InvasionMonsters.WhiteWizard,
        });

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot says why it turns down a player's trade or duel
    /// request, by whisper. A bot turns them down in any case - it has no trade window, and it
    /// doesn't duel.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_DeclineReplyChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_DeclineReplyChancePercent_Description))]
    public int DeclineReplyChancePercent { get; set; } = 70;

    /// <summary>
    /// Gets or sets from how many normal chat messages of human players near a bot the bot keeps
    /// quiet - it says nothing unasked (conversations, greetings, congratulations, comments) while the
    /// players around it are busy talking; 0 switches it off. Answers to a player stay unaffected.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_BusyChatMessageCount_Name), Description = nameof(PlugInResources.BotChatConfiguration_BusyChatMessageCount_Description))]
    public int BusyChatMessageCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets how long a message of a human player counts for <see cref="BusyChatMessageCount"/>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_BusyChatWindow_Name), Description = nameof(PlugInResources.BotChatConfiguration_BusyChatWindow_Description))]
    public TimeSpan BusyChatWindow { get; set; } = TimeSpan.FromSeconds(30);

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
    /// Gets or sets the comma-separated words for the Blood Castle in a party search: followed by the
    /// event level, like "party bc7", they stand for the level bracket of that Blood Castle.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyBloodCastleKeywords_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyBloodCastleKeywords_Description))]
    public string PartyBloodCastleKeywords { get; set; } = "bc, blood castle";

    /// <summary>
    /// Gets or sets the comma-separated words for the Devil Square in a party search, like "party ds5".
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyDevilSquareKeywords_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyDevilSquareKeywords_Description))]
    public string PartyDevilSquareKeywords { get; set; } = "ds, devil square";

    /// <summary>
    /// Gets or sets the comma-separated words for the Chaos Castle in a party search, like "party cc3".
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyChaosCastleKeywords_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyChaosCastleKeywords_Description))]
    public string PartyChaosCastleKeywords { get; set; } = "cc, chaos castle";

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
    /// Gets or sets how often a bot selling its goods on the market may advertise them (see
    /// <see cref="MerchantLines"/> and <see cref="BotMerchantPlugIn"/>), per game server.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MerchantTalkInterval_Name), Description = nameof(PlugInResources.BotChatConfiguration_MerchantTalkInterval_Description))]
    public TimeSpan MerchantTalkInterval { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Gets or sets the chance, in percent, that a bot advertises when the interval is over. Only bots
    /// behind their open store on the market advertise, so nothing happens while the
    /// <see cref="BotMerchantPlugIn"/> is off.
    /// </summary>
    [Range(0, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MerchantTalkChancePercent_Name), Description = nameof(PlugInResources.BotChatConfiguration_MerchantTalkChancePercent_Description))]
    public int MerchantTalkChancePercent { get; set; } = 80;

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
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventLines_Description))]
    public ICollection<BotChatLine> EventLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot in a town says in the normal chat when the entrance of a Blood
    /// Castle, Devil Square or Chaos Castle opened. Placeholders: <c>{event}</c>, <c>{map}</c>,
    /// <c>{location}</c>, <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventReminderLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventReminderLines_Description))]
    public ICollection<BotChatLine> EventReminderLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot posts with <c>/post</c> when an invasion (White Wizard, Golden, Red Dragon)
    /// started. Placeholders: <c>{event}</c>, <c>{map}</c>, <c>{location}</c> (the map alone), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_InvasionLines_Description))]
    public ICollection<BotChatLine> InvasionLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the normal chat when it reached a level milestone
    /// (see <see cref="LevelMilestoneStep"/>). Placeholders: <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_LevelMilestoneLines_Description))]
    public ICollection<BotChatLine> LevelMilestoneLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the answers of a bot to a player who whispered to it, or mentioned its name in the normal
    /// chat next to it. The bot answers the same way: by whisper, or in the normal chat.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>, <c>{wings}</c> (see
    /// <see cref="WithoutWingsLines"/>).
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Name), Description = nameof(PlugInResources.BotChatConfiguration_WhisperReplies_Description))]
    public ICollection<BotChatReply> WhisperReplies { get; set; } = new List<BotChatReply>();

    /// <summary>
    /// Gets or sets the answers of a bot behind its open store on the market (see <see cref="BotMerchantPlugIn"/>)
    /// to a whisper, or a mention of its name in the normal chat next to it, which asks what it sells: they
    /// point to the store, which shows the goods and prices. Only a reply whose keyword matches is used,
    /// before the <see cref="WhisperReplies"/>; any other message gets a usual answer. The keywords cover
    /// those of the usual trade replies, so a merchant never answers that it has nothing to sell.
    /// Placeholders as in <see cref="WhisperReplies"/>, and <c>{store}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MerchantWhisperReplies_Name), Description = nameof(PlugInResources.BotChatConfiguration_MerchantWhisperReplies_Description))]
    public ICollection<BotChatReply> MerchantWhisperReplies { get; set; } = new List<BotChatReply>();

    /// <summary>
    /// Gets or sets the typical words and letters by which a bot recognizes the language of a
    /// player's message (whisper, mention, party chat, meeting place): it answers in that language,
    /// even when the player's account language is another one. A message which doesn't tell is
    /// answered in the player's own language.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MessageLanguages_Name), Description = nameof(PlugInResources.BotChatConfiguration_MessageLanguages_Description))]
    public ICollection<BotChatLanguageHint> MessageLanguages { get; set; } = new List<BotChatLanguageHint>();

    /// <summary>
    /// Gets or sets the lines the nearest bot greets a player with who entered the game.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_GreetingLines_Description))]
    public ICollection<BotChatLine> GreetingLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which a bot greets a player it was in a party with before - instead
    /// of the <see cref="GreetingLines"/> when the player enters the game, and of the
    /// <see cref="PartyJoinedLines"/> when the player invites it again. The bots remember their party
    /// companions until the server restarts. Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_FriendLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_FriendLines_Description))]
    public ICollection<BotChatLine> FriendLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says in the party chat after it accepted a player's party invitation.
    /// Placeholders: <c>{player}</c> (the inviter), <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyJoinedLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyJoinedLines_Description))]
    public ICollection<BotChatLine> PartyJoinedLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines one bot per party says in the party chat when an event it went through with a
    /// player's party ended. Placeholders: <c>{event}</c>, <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_EventFinishedLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_EventFinishedLines_Description))]
    public ICollection<BotChatLine> EventFinishedLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which the nearest bot congratulates a player on a level-up.
    /// Placeholders: <c>{player}</c>, <c>{level}</c> and <c>{resets}</c> of the player, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_CongratulationLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_CongratulationLines_Description))]
    public ICollection<BotChatLine> CongratulationLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which the nearest bot congratulates a player on a reset.
    /// Placeholders: <c>{player}</c>, <c>{level}</c> and <c>{resets}</c> of the player, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_ResetCongratulationLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_ResetCongratulationLines_Description))]
    public ICollection<BotChatLine> ResetCongratulationLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which the nearest bot comments on an excellent or ancient item a
    /// player's kill dropped. Placeholders: <c>{player}</c>, <c>{item}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_RareDropLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_RareDropLines_Description))]
    public ICollection<BotChatLine> RareDropLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which a bot shows off the new wings it earned at a level milestone,
    /// while a player sees it. Placeholders: <c>{wings}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_OwnWingsLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_OwnWingsLines_Description))]
    public ICollection<BotChatLine> OwnWingsLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which the nearest bot comments on the wings a player put on - once
    /// per player and pair until the server restarts. Placeholders: <c>{player}</c>, <c>{wings}</c>
    /// (the player's), <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PlayerWingsLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PlayerWingsLines_Description))]
    public ICollection<BotChatLine> PlayerWingsLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the answers of a bot without wings to a question about them: a whisper reply with
    /// <c>{wings}</c> would name nothing, so the bot answers with one of these instead.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_WithoutWingsLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_WithoutWingsLines_Description))]
    public ICollection<BotChatLine> WithoutWingsLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines with which a bot calls out a notable monster which appeared near it.
    /// Placeholders: <c>{monster}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_NotableMonsterLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_NotableMonsterLines_Description))]
    public ICollection<BotChatLine> NotableMonsterLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot turns down a player's trade request.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_TradeDeclineLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_TradeDeclineLines_Description))]
    public ICollection<BotChatLine> TradeDeclineLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot turns down a player's duel challenge.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_DuelDeclineLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_DuelDeclineLines_Description))]
    public ICollection<BotChatLine> DuelDeclineLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot answers a player's party search.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{class}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyVolunteerLines_Description))]
    public ICollection<BotChatLine> PartyVolunteerLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot asks again when it didn't understand where to meet.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{class}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyAskWhereLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyAskWhereLines_Description))]
    public ICollection<BotChatLine> PartyAskWhereLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot confirms that it's on its way to the meeting place.
    /// Placeholders: <c>{player}</c>, <c>{map}</c> and <c>{location}</c> (the meeting place).
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyOnTheWayLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyOnTheWayLines_Description))]
    public ICollection<BotChatLine> PartyOnTheWayLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the whispers with which a bot declines a meeting place it may not go to - its level
    /// is too low for the map, or the map can't be reached by warp.
    /// Placeholders: <c>{player}</c>, <c>{map}</c> (the named map), <c>{level}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyCantGoLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyCantGoLines_Description))]
    public ICollection<BotChatLine> PartyCantGoLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the words players use for the character classes in a party search.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_PartyClassAliases_Name), Description = nameof(PlugInResources.BotChatConfiguration_PartyClassAliases_Description))]
    public ICollection<BotClassAlias> PartyClassAliases { get; set; } = new List<BotClassAlias>();

    /// <summary>
    /// Gets or sets the lines a bot says in the normal chat when a player killed it outside of an event, right
    /// before it respawns. Placeholders: <c>{player}</c> (the killer), <c>{level}</c>, <c>{map}</c>, <c>{location}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_KilledLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_KilledLines_Description))]
    public ICollection<BotChatLine> KilledLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot says instead of the <see cref="KilledLines"/> when the same player killed
    /// it again within a short time - the bot gives up its revenge (see <see cref="BotRevengePlugIn"/>).
    /// Placeholders as for the <see cref="KilledLines"/>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_KilledAgainLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_KilledAgainLines_Description))]
    public ICollection<BotChatLine> KilledAgainLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the lines a bot behind its open store on the market advertises with, now and then
    /// (see <see cref="MerchantTalkInterval"/>) - only when a player sees it.
    /// Placeholders: <c>{level}</c>, <c>{map}</c>, <c>{location}</c>, <c>{store}</c> (the name of its store).
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_MerchantLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_MerchantLines_Description))]
    public ICollection<BotChatLine> MerchantLines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Gets or sets the conversations two bots near a player have now and then: one bot says the
    /// opener, the other one answers. They are drawn without repetition until all of a place were
    /// used. Placeholders: <c>{map}</c>, <c>{location}</c>, <c>{level}</c>, <c>{class}</c> of the
    /// speaking bot, and <c>{partner}</c> - the name of the other bot.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_Conversations_Name), Description = nameof(PlugInResources.BotChatConfiguration_Conversations_Description))]
    public ICollection<BotConversation> Conversations { get; set; } = new List<BotConversation>();

    /// <summary>
    /// Gets or sets the lines with which a bot answers a player who chimes in right after a
    /// conversation of two bots near them - unless the player's message matches a whisper reply's
    /// keyword. Placeholders: <c>{player}</c>, <c>{map}</c>, <c>{level}</c>, <c>{class}</c>.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [CollapsedByDefault]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_ConversationJoinLines_Name), Description = nameof(PlugInResources.BotChatConfiguration_ConversationJoinLines_Description))]
    public ICollection<BotChatLine> ConversationJoinLines { get; set; } = new List<BotChatLine>();

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
            ("lf party {event}, waiting in {map}", "szukam pt na {event}, czekam w {map}"),
            ("{event} now, who's in? {location}", "{event} startuje, kto chętny? {location}"),
            ("going {event}, need two more, {map}", "idę na {event}, brakuje dwóch, {map}"),
            ("{event} soon, meet me at {location}", "zaraz {event}, zbiórka {location}"),
            ("anyone for {event}? waiting in {map}", "ktoś na {event}? czekam w {map}"),
            ("{event}! last call, {location}", "{event}! ostatnie wołanie, {location}"));
        Add(
            configuration.EventReminderLines,
            ("{event} is open, anyone going?", "{event} otwarte, ktoś idzie?"),
            ("{event} now, who's coming?", "{event} teraz, kto idzie?"),
            ("guys, {event} is open", "ludzie, {event} otwarte"),
            ("{event}! got a ticket, anyone?", "{event}! mam bilet, ktoś chętny?"),
            ("{event} time :)", "czas na {event} :)"));
        Add(
            configuration.InvasionLines,
            ("{event} in {map}!", "{event} w {map}!"),
            ("{event} on {map}, go go", "{event} na {map}, lecimy!"),
            ("{map} - {event} started", "{map} - {event} ruszyło"),
            ("{event}! everyone to {map}", "{event}! wszyscy na {map}"),
            ("{event} at {map}, the loot is waiting!", "{event} w {map}, drop czeka!"));
        Add(
            configuration.LevelMilestoneLines,
            ("{level} finally :)", "{level} w końcu :)"),
            ("lvl {level}, nice", "lvl {level}, jest moc"),
            ("{level} done", "{level} wbite"),
            ("ding! {level}", "ding! {level}"),
            ("{level}, slowly but surely", "{level}, powoli do przodu"),
            ("yes! {level}", "jest! {level}"));
        AddReplies(
            string.Empty,
            ("sry, busy hunting", "sorki, poluję"),
            ("hi, can't talk now", "hej, nie mogę teraz gadać"),
            ("hi, what's up?", "hej, co tam?"),
            ("afk a bit", "chwilę afk"),
            ("hm?", "hm?"),
            ("sorry, didn't get that", "sorki, nie załapałem"),
            ("brb, monsters", "zaraz, potwory"));
        Add(
            configuration.GreetingLines,
            ("hi {player}, have a nice game", "siema {player}, miłej gry"),
            ("welcome back {player}", "witaj z powrotem, {player}"),
            ("yo {player} :)", "elo {player} :)"),
            ("hey {player}", "hej {player}"),
            ("{player}! long time no see", "{player}! kopę lat"),
            ("look who's here, {player}", "o proszę, {player} jest"));
        AddAt(
            configuration.GreetingLines,
            BotChatTimeOfDay.Morning,
            ("good morning {player}", "dzień dobry, {player}"),
            ("morning {player}, coffee first?", "hej {player}, kawa już była?"),
            ("early bird, {player}", "ranny ptaszek z ciebie, {player}"));
        AddAt(configuration.GreetingLines, BotChatTimeOfDay.Day, ("hey {player}, nice day for exp", "hej {player}, dobry dzień na expa"));
        AddAt(
            configuration.GreetingLines,
            BotChatTimeOfDay.Evening,
            ("good evening {player}", "dobry wieczór, {player}"),
            ("evening {player}, done with work?", "siema {player}, już po robocie?"));
        AddAt(
            configuration.GreetingLines,
            BotChatTimeOfDay.Night,
            ("late night grind, {player}?", "nocny exp, {player}?"),
            ("{player}, can't sleep either?", "{player}, też nie możesz spać?"));
        Add(
            configuration.FriendLines,
            ("{player}! good to see you again", "{player}! dobrze cię znowu widzieć"),
            ("hey {player}, together again?", "hej {player}, znowu razem?"),
            ("my old party buddy {player} :)", "mój stary kompan {player} :)"),
            ("{player}, ready for round two?", "{player}, gotowy na drugą rundę?"));
        Add(
            configuration.PartyJoinedLines,
            ("thx for invite", "dzięki za zaproszenie"),
            ("hi all", "siema wszystkim"),
            ("ty {player}", "dzięki, {player}"),
            ("hey, let's go", "hej, lecimy"),
            ("where are we going?", "gdzie idziemy?"),
            ("ready when you are", "gotowy, jak tylko powiesz"),
            ("nice, a party at last", "o, nareszcie grupa"));
        Add(
            configuration.EventFinishedLines,
            ("gg", "gg"),
            ("gg wp", "gg wp"),
            ("gg, thx for the party", "gg, dzięki za grupę"),
            ("nice run", "niezły run"),
            ("again next time?", "powtórka następnym razem?"),
            ("that was close", "było blisko"),
            ("gg, easy", "gg, łatwizna"));
        Add(
            configuration.KilledLines,
            ("why??", "czemu??"),
            ("wtf {player}", "wtf {player}"),
            ("really?", "serio?"),
            ("ok {player}, i'll remember that", "ok {player}, zapamiętam"),
            ("i was just hunting...", "ja tu tylko expiłem..."),
            ("not cool, {player}", "słabo, {player}"),
            ("pk? seriously?", "pk? no serio?"));
        Add(
            configuration.KilledAgainLines,
            ("you again...", "znowu ty..."),
            ("ok ok, i'm leaving", "dobra, dobra, idę stąd"),
            ("{player} pls stop", "{player}, weź przestań"),
            ("enough, i'm out", "dość, spadam"),
            ("fine, the spot is yours", "dobra, spot twój"),
            ("you must be bored", "chyba się nudzisz"));
        Add(
            configuration.MerchantLines,
            ("selling my loot, take a look at {location}", "sprzedaję dropa, zajrzyj na {location}"),
            ("check my store '{store}', good prices", "zajrzyj do sklepu '{store}', dobre ceny"),
            ("store open at {location}, come and see", "sklep otwarty na {location}, zapraszam"),
            ("good items for sale, {location}", "dobre itemy na sprzedaż, {location}"),
            ("'{store}' is open, first come first served", "'{store}' otwarty, kto pierwszy ten lepszy"),
            ("selling what i found today, {location}", "sprzedaję, co dziś wypadło, {location}"));
        Add(
            configuration.CongratulationLines,
            ("gz", "gz"),
            ("gz {player}", "gratki, {player}"),
            ("nice, {level}!", "nieźle, {level}!"),
            ("grats :)", "gratulacje :)"),
            ("gz! keep going", "gz! dawaj dalej"),
            ("wow, {level} already?", "wow, już {level}?"));
        Add(
            configuration.ResetCongratulationLines,
            ("gz on the reset!", "gratki resetu!"),
            ("reset {resets}, nice {player}", "reset {resets}, nieźle {player}"),
            ("another reset? gz", "kolejny reset? gz"),
            ("{resets} resets? respect", "{resets} resetów? szacun"),
            ("gz {player}, back to the start :)", "gz {player}, i od nowa :)"));
        Add(
            configuration.RareDropLines,
            ("nice drop, {player}!", "niezły drop, {player}!"),
            ("whoa, {item}!", "o kurczę, {item}!"),
            ("lucky! gz on the {item}", "fart! gratki za {item}"),
            ("{player}, that's a good one", "{player}, to jest dobre"));
        Add(
            configuration.OwnWingsLines,
            ("finally, my wings!", "w końcu moje skrzydła!"),
            ("{wings}! all that grinding paid off", "{wings}! cały ten grind się opłacił"),
            ("look, new wings :)", "patrzcie, nowe skrzydła :)"),
            ("{level} and {wings}, what a day", "{level} i {wings}, co za dzień"),
            ("at last, {wings}", "nareszcie, {wings}"));
        Add(
            configuration.PlayerWingsLines,
            ("nice wings, {player}!", "ładne skrzydła, {player}!"),
            ("whoa, {wings}!", "o kurczę, {wings}!"),
            ("{wings}? respect", "{wings}? szacun"),
            ("gz on the wings, {player}", "gratki skrzydeł, {player}"),
            ("i want those wings too", "też chcę takie skrzydła"));
        Add(
            configuration.WithoutWingsLines,
            ("no wings yet, saving up", "jeszcze bez skrzydeł, zbieram"),
            ("not yet, a few more levels", "jeszcze nie, jeszcze parę lvli"),
            ("none yet, still grinding", "jeszcze nie mam, expię dalej"));
        Add(
            configuration.NotableMonsterLines,
            ("{monster} here!", "{monster} tutaj!"),
            ("look, {monster}! at {location}", "patrz, {monster}! {location}"),
            ("{monster}!! everyone come", "{monster}!! wszyscy tu"),
            ("careful, {monster}", "uważajcie, {monster}"));
        Add(
            configuration.TradeDeclineLines,
            ("sry, no trade", "sorki, nie handluję"),
            ("not trading now, busy", "teraz nie handluję, jestem zajęty"),
            ("nothing to sell, sry", "nie mam nic na sprzedaż, sorki"),
            ("my bag is full, sry", "plecak pełny, sorki"),
            ("only exp today", "dziś tylko exp"));
        Add(
            configuration.DuelDeclineLines,
            ("not now, maybe later", "nie teraz, może później"),
            ("no duels today, sry", "dziś bez duelów, sorki"),
            ("haha no, i'm here for the exp", "haha nie, jestem tu dla expa"),
            ("maybe after this spot", "może po tym spocie"),
            ("you'd win anyway :)", "i tak byś wygrał :)"));
        Add(
            configuration.PartyVolunteerLines,
            ("me? {level} {class}", "ja? {level} {class}"),
            ("i can come, {class} {level}", "mogę przyjść, {class} {level}"),
            ("{class} {level} here, where are you?", "{class} {level}, gdzie jesteś?"),
            ("count me in, lvl {level}. where?", "wchodzę, lvl {level}. gdzie?"),
            ("i'm free, {class} {level}", "jestem wolny, {class} {level}"),
            ("{level} {class}, where to?", "{level} {class}, dokąd?"));
        Add(
            configuration.PartyAskWhereLines,
            ("where? write map and coords, e.g. devias 215 40", "gdzie? napisz mapę i koordy, np. devias 215 40"),
            ("which map? like lorencia 130 120", "jaka mapa? np. lorencia 130 120"),
            ("i'm in, but where? map and coords pls", "wchodzę, ale gdzie? mapa i koordy pls"));
        Add(
            configuration.PartyOnTheWayLines,
            ("omw to {location}", "idę, {location}"),
            ("ok, coming to {location}", "ok, już idę do {location}"),
            ("on my way", "już lecę"),
            ("be there in a minute", "zaraz będę"),
            ("coming, wait for me at {location}", "idę, czekaj na mnie w {location}"));
        Add(
            configuration.PartyCantGoLines,
            ("can't go to {map}, sry", "nie mogę iść do {map}, sorki"),
            ("{map} is too much for me", "{map} to dla mnie za dużo"),
            ("my level is too low for {map}", "za niski lvl na {map}"),
            ("{map}? no way, not yet", "{map}? jeszcze nie dam rady"));
        configuration.MessageLanguages.Add(new BotChatLanguageHint
        {
            Language = "pl",
            Words = "jak, się, sie, masz, cześć, czesc, siema, siemka, gdzie, jesteś, jestes, dzięki, dzieki, dziękuję, dziekuje, nara, poziom, grupa, grupę, grupe, muszę, musze, kończyć, konczyc, spadam, co, tam, słychać, slychac, jaki, jaka, który, ktory, czy, nie, tak, dobra, pomoc, pomożesz, pomozesz, kupię, kupie, sprzedam, idę, ide, jestem, mam, tutaj, chodź, chodz, witam, dobranoc, dzień, dzien, dobry, czym, grasz, expisz, gdzie, potrzebuję, potrzebuje",
            Letters = "ąćęłńóśźż",
        });
        configuration.MessageLanguages.Add(new BotChatLanguageHint
        {
            Language = "en",
            Words = "how, are, you, hello, hi, where, what, level, thanks, thank, bye, the, is, need, help, your, which, can, come, here, see, good, have, going, sell, buy",
        });
        Add(
            configuration.ConversationJoinLines,
            ("right, {player}?", "no nie, {player}?"),
            ("haha, exactly", "haha, dokładnie"),
            ("you think so too?", "też tak myślisz?"),
            ("agreed", "zgadzam się"),
            ("see, {player} gets it", "widzisz, {player} rozumie"),
            ("hehe", "hehe"));
        BotConversationDefaults.AddTo(configuration.Conversations);
        AddClass("dk, bk, bm, knight, dark knight, blade knight, blade master, rycerz", BotClassNumbers.DarkKnightNumber);
        AddClass("dw, sm, gm, wizard, dark wizard, soul master, grand master, mag", BotClassNumbers.DarkWizardNumber);
        AddClass("elf, fe, muse, fairy elf, muse elf, high elf, elfka", BotClassNumbers.FairyElfNumber);
        AddClass("mg, dm, gladiator, magic gladiator, duel master", BotClassNumbers.MagicGladiatorNumber);
        AddClass("dl, le, lord, dark lord, lord emperor", BotClassNumbers.DarkLordNumber);
        AddClass("sum, summoner, bs, dim, bloody summoner, dimension master", BotClassNumbers.SummonerNumber);
        AddClass("rf, fm, fighter, rage fighter, fist master", BotClassNumbers.RageFighterNumber);
        AddReplies(
            "how are you, how r u, how are u, hru, how's it going, how is it going, jak sie masz, jak się masz, co slychac, co słychać, co tam, jak leci",
            ("fine, thanks for asking", "dobrze, dzięki że pytasz"),
            ("good, you?", "spoko, a ty?"),
            ("all good, just grinding", "wszystko ok, expię"),
            ("tired but ok", "zmęczony, ale ok"),
            ("never better", "lepiej być nie może"));
        AddReplies(
            "hi, hello, hey, yo, hiho, sup, siema, siemka, czesc, cześć, hej, elo, witam, dzien dobry, dzień dobry",
            ("hi, what's up?", "hej, co tam?"),
            ("hey :)", "hej :)"),
            ("hello", "siema"),
            ("yo", "elo"),
            ("hey {player}", "hej {player}"));
        AddReplies(
            "level, lvl, what level, poziom, jaki lvl, jaki poziom, ktory lvl, który lvl",
            ("{level} now", "{level} teraz"),
            ("lvl {level}", "lvl {level}"),
            ("{level}, and you?", "{level}, a ty?"));
        AddReplies(
            "class, what class, jaka klasa, jaka postac, jaka postać, czym grasz",
            ("{class}", "{class}"),
            ("{class}, lvl {level}", "{class}, lvl {level}"));
        AddReplies(
            "where are you, where r u, where, gdzie jestes, gdzie jesteś, gdzie",
            ("{location}", "{location}"),
            ("in {location}", "w {location}"),
            ("{location}, come by", "{location}, wpadaj"));
        AddReplies(
            "where to exp, good spot, exp spot, gdzie expic, gdzie expić, gdzie na expa, dobry spot, gdzie expisz",
            ("depends on your level, try lost tower", "zależy od lvla, spróbuj lost tower"),
            ("atlans is good if you have a party", "atlans jest spoko, jak masz grupę"),
            ("i like {map} at my level", "na moim lvlu lubię {map}"));
        AddReplies(
            "wings, wing, your wings, nice wings, skrzydla, skrzydła, skrzydelka, skrzydełka, jakie skrzydla, jakie skrzydła",
            ("{wings}", "{wings}"),
            ("{wings}, took me a while", "{wings}, trochę to trwało"),
            ("{wings} :)", "{wings} :)"));
        AddReplies(
            "party, pt, grupa, grupe, grupę, do grupy",
            ("sure, invite me", "jasne, zaproś mnie"),
            ("ok, send invite", "ok, wysyłaj zaproszenie"),
            ("sure, where are you?", "jasne, gdzie jesteś?"));
        AddReplies(
            "help, can you help, pomoc, pomocy, pomozesz, pomożesz",
            ("what do you need?", "czego potrzebujesz?"),
            ("sure, what's up?", "jasne, o co chodzi?"));
        AddReplies(
            "sell, buy, wtb, wts, trade, sprzedasz, kupisz, sprzedam, kupie, kupię, handel",
            ("nothing to sell, sry", "nie mam nic na sprzedaż, sorki"),
            ("ask at the market in town", "popytaj na rynku w mieście"));
        AddReplies(
            "bot, are you a bot, r u bot, u bot, jestes botem, jesteś botem",
            ("what do you think? :)", "a jak myślisz? :)"),
            ("beep boop", "bip bup"));
        AddReplies(
            "thanks, thank you, thx, ty, dzieki, dzięki, dziekuje, dziękuję, dzieks, thx bro",
            ("np", "np"),
            ("no problem", "nie ma sprawy"),
            ("anytime", "spoko, zawsze"));
        AddReplies(
            "bye, cya, see you, see ya, gn, gtg, g2g, got to go, gotta go, have to go, must go, i'm off, im off, logging off, nara, pa, papa, do zobaczenia, dobranoc, na razie, musze konczyc, muszę kończyć, koncze, kończę, musze isc, muszę iść, spadam, zmykam, lece, lecę, do jutra",
            ("bye", "nara"),
            ("cya", "na razie"),
            ("ok, see you later", "ok, do później"),
            ("take care", "trzymaj się"));
        AddMerchantReplies(
            "sell, selling, wts, wtb, trade, price, how much, shop, store, buy, sprzedajesz, sprzedasz, co sprzedajesz, sprzedam, kupisz, handel, masz coś, masz cos, cena, za ile, ile za, ile chcesz, sklep, kupię, kupie",
            ("check my store, everything's priced", "zajrzyj do sklepu, wszystko z cenami"),
            ("click my shop, it's all there", "kliknij mój sklep, wszystko tam jest"),
            ("have a look at '{store}' :)", "zerknij do '{store}' :)"),
            ("prices are in the shop, no haggling", "ceny są w sklepie, nie targuję się"),
            ("open my store, maybe something catches your eye", "otwórz sklep, może coś wpadnie w oko"),
            ("all i sell is on the shelf, take a look", "co sprzedaję, to na półce, zobacz sam"));
        SetGesture(configuration.GreetingLines, BotChatGesture.Greeting);
        SetGesture(configuration.PartyJoinedLines, BotChatGesture.Salute);
        SetGesture(configuration.LevelMilestoneLines, BotChatGesture.Win);
        SetGesture(configuration.EventFinishedLines, BotChatGesture.Cheer);
        SetGesture(configuration.CongratulationLines, BotChatGesture.Clap);
        SetGesture(configuration.ResetCongratulationLines, BotChatGesture.Respect);
        SetGesture(configuration.KilledLines, BotChatGesture.Provocation);
        SetGesture(configuration.KilledAgainLines, BotChatGesture.Cry);
        SetGesture(configuration.DuelDeclineLines, BotChatGesture.Awkward);
        SetGesture(configuration.ConversationJoinLines, BotChatGesture.Smile);
        SetGesture(configuration.RareDropLines, BotChatGesture.Clap);
        SetGesture(configuration.OwnWingsLines, BotChatGesture.Win);
        SetGesture(configuration.PlayerWingsLines, BotChatGesture.Respect);
        SetGesture(configuration.FriendLines, BotChatGesture.Greeting);
        foreach (var reply in configuration.WhisperReplies)
        {
            reply.Gesture = reply.Keywords.StartsWith("hi,", StringComparison.Ordinal) ? BotChatGesture.Greeting
                : reply.Keywords.StartsWith("bye,", StringComparison.Ordinal) ? BotChatGesture.Goodbye
                : reply.Keywords.StartsWith("thanks,", StringComparison.Ordinal) ? BotChatGesture.Smile
                : BotChatGesture.None;
        }

        return configuration;

        static void SetGesture(IEnumerable<BotChatLine> lines, BotChatGesture gesture)
        {
            foreach (var line in lines)
            {
                line.Gesture = gesture;
            }
        }

        void Add(ICollection<BotChatLine> lines, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                lines.Add(new BotChatLine { Text = Translated(text) });
            }
        }

        void AddAt(ICollection<BotChatLine> lines, BotChatTimeOfDay when, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                lines.Add(new BotChatLine { Text = Translated(text), When = when });
            }
        }

        void AddReplies(string keywords, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                configuration.WhisperReplies.Add(new BotChatReply { Keywords = keywords, Text = Translated(text) });
            }
        }

        void AddMerchantReplies(string keywords, params (string English, string Polish)[] texts)
        {
            foreach (var text in texts)
            {
                configuration.MerchantWhisperReplies.Add(new BotChatReply { Keywords = keywords, Text = Translated(text) });
            }
        }

        void AddClass(string keywords, byte classNumber)
            => configuration.PartyClassAliases.Add(new BotClassAlias { Keywords = keywords, ClassNumber = classNumber });

        static LocalizedString Translated((string English, string Polish) text)
            => new LocalizedString(text.English).WithTranslation(CultureInfo.GetCultureInfo("pl"), text.Polish);
    }
}
