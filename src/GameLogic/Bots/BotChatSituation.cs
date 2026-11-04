// <copyright file="BotChatSituation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// The situations in which a server-side bot says something. Each line of the
/// <see cref="BotChatConfiguration.Lines"/> belongs to one of them.
/// </summary>
public enum BotChatSituation
{
    /// <summary>
    /// The entrance of a Blood Castle, Devil Square or Chaos Castle opened. A bot which meets the
    /// event's entry requirements posts about it with <c>/post</c> and then waits for players at the
    /// meeting point (see <see cref="BotEventMeeting"/>) until the entrance closes.
    /// Placeholders: <c>{event}</c>, <c>{map}</c> (the map of the meeting point), <c>{level}</c>.
    /// </summary>
    EventEntranceOpened,

    /// <summary>
    /// An invasion (White Wizard, Golden, Red Dragon) started. A bot which may warp to the invaded
    /// map posts about it with <c>/post</c>.
    /// Placeholders: <c>{event}</c>, <c>{map}</c>, <c>{level}</c>.
    /// </summary>
    InvasionStarted,

    /// <summary>
    /// The bot reached a level milestone (see <see cref="BotChatConfiguration.LevelMilestoneStep"/>).
    /// Said in the normal chat, so only players nearby see it.
    /// Placeholders: <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    LevelMilestone,

    /// <summary>
    /// A human player whispered to the bot. The bot whispers back.
    /// Placeholders: <c>{player}</c>, <c>{level}</c>, <c>{map}</c>.
    /// </summary>
    WhisperReply,
}
