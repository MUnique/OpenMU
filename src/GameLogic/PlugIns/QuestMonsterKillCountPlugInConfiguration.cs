// <copyright file="QuestMonsterKillCountPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Configuration for the <see cref="QuestMonsterKillCountPlugInConfiguration"/>.
/// </summary>
public class QuestMonsterKillCountPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the save interval.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.QuestMonsterKillCountPlugInConfiguration_Message_Name))]
    public LocalizedString Message { get; set; } = "[{0}] Defeat {1} - {2}/{3}";
}