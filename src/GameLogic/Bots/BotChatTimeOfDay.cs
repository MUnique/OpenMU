// <copyright file="BotChatTimeOfDay.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// When a bot may say a <see cref="BotChatLine"/> or have a <see cref="BotConversation"/>, by the
/// local time of the server: "good morning" in the morning, "late night grind?" at night.
/// </summary>
/// <remarks>
/// New values are appended: the configuration stores the time of day as a number. The hours are
/// fixed (see <see cref="BotChatPlugIn.GetTimeOfDay"/>); they only pick fitting lines, so a
/// configurable boundary wouldn't be worth the extra settings.
/// </remarks>
public enum BotChatTimeOfDay
{
    /// <summary>
    /// At any time.
    /// </summary>
    Anytime,

    /// <summary>
    /// From 5 to 11 o'clock.
    /// </summary>
    Morning,

    /// <summary>
    /// From 11 to 18 o'clock.
    /// </summary>
    Day,

    /// <summary>
    /// From 18 to 23 o'clock.
    /// </summary>
    Evening,

    /// <summary>
    /// From 23 to 5 o'clock.
    /// </summary>
    Night,
}
