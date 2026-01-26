// <copyright file="MiniGameStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// The minimal minigame start configuration.
/// </summary>
public abstract class MiniGameStartConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets or sets the entrance opened message.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.MiniGameStartConfiguration_EntranceOpenedMessage_Name))]
    public LocalizedString EntranceOpenedMessage { get; set; }

    /// <summary>
    /// Gets or sets the entrance closed message.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.MiniGameStartConfiguration_EntranceClosedMessage_Name))]
    public LocalizedString EntranceClosedMessage { get; set; }
}