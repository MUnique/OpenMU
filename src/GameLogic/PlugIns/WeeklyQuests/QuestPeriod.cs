// <copyright file="QuestPeriod.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The period after which the progress of a <see cref="WeeklyQuestDefinition"/> starts over.
/// </summary>
/// <remarks>
/// The values are sent to the client, so they must not be renumbered.
/// </remarks>
public enum QuestPeriod
{
    /// <summary>
    /// The progress starts over every week, see <see cref="WeeklyQuestsConfiguration.ResetDay"/>.
    /// </summary>
    [Display(Name = "Semanal")]
    Weekly = 0,

    /// <summary>
    /// The progress starts over every day, see <see cref="WeeklyQuestsConfiguration.DailyResetTime"/>.
    /// </summary>
    [Display(Name = "Diaria")]
    Daily = 1,

    /// <summary>
    /// The progress never starts over, so the quest can be completed only once per character.
    /// </summary>
    [Display(Name = "Una sola vez")]
    Once = 2,
}
