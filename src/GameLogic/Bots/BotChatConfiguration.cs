// <copyright file="BotChatConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Composition;

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
    /// Gets or sets the lines the bots choose from. A situation without lines stays silent.
    /// </summary>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotChatConfiguration_Lines_Name), Description = nameof(PlugInResources.BotChatConfiguration_Lines_Description))]
    public ICollection<BotChatLine> Lines { get; set; } = new List<BotChatLine>();

    /// <summary>
    /// Creates the configuration a server starts with, including a small set of English lines.
    /// </summary>
    /// <returns>The default configuration.</returns>
    /// <remarks>
    /// The lines are configuration data like the names of the seeded items, not texts of the game
    /// logic: a server owner is expected to rewrite them and to add translations.
    /// </remarks>
    public static BotChatConfiguration CreateDefault()
    {
        var configuration = new BotChatConfiguration();
        Add(
            BotChatSituation.EventEntranceOpened,
            "{event} open, anyone going? im in {map}",
            "lf party {event}, waiting in {map}",
            "{event} now, who wants to join? {map}",
            "going {event}, party anyone? at {map}");
        Add(
            BotChatSituation.InvasionStarted,
            "{event} in {map}!",
            "{event} on {map}, go go",
            "{map} - {event} started");
        Add(
            BotChatSituation.LevelMilestone,
            "{level} finally :)",
            "lvl {level}, nice",
            "{level} done");
        Add(
            BotChatSituation.WhisperReply,
            "sry busy hunting",
            "hi, cant talk now",
            "?",
            "afk a bit");
        return configuration;

        void Add(BotChatSituation situation, params string[] texts)
        {
            foreach (var text in texts)
            {
                configuration.Lines.Add(new BotChatLine { Situation = situation, Text = text });
            }
        }
    }
}
