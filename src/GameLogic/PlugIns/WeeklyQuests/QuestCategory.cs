// <copyright file="QuestCategory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The category of a <see cref="WeeklyQuestDefinition"/>, which decides under which tab the quest is shown.
/// </summary>
/// <remarks>
/// The values are sent to the client, so they must not be renumbered.
/// </remarks>
public enum QuestCategory
{
    /// <summary>
    /// A weekly quest.
    /// </summary>
    [Display(Name = "Semanal")]
    Weekly = 0,

    /// <summary>
    /// A daily quest.
    /// </summary>
    [Display(Name = "Diaria")]
    Daily = 1,

    /// <summary>
    /// A chapter of the main story.
    /// </summary>
    [Display(Name = "Historia")]
    Main = 2,

    /// <summary>
    /// A quest for a character class.
    /// </summary>
    [Display(Name = "Clase")]
    Class = 3,

    /// <summary>
    /// A quest of a map or region.
    /// </summary>
    [Display(Name = "Zona")]
    Zone = 4,
}
