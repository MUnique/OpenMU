// <copyright file="StatResetConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Configuration of the Stat Reset System.
/// </summary>
public class StatResetConfiguration
{
    /// <summary>
    /// Gets or sets the required level for a stat reset.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_RequiredLevel_Name))]
    public int RequiredLevel { get; set; }

    /// <summary>
    /// Gets or sets the required money for a stat reset.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_RequiredMoney_Name))]
    public int RequiredMoney { get; set; } = 1000000;

    /// <summary>
    /// Gets or sets the required item for a stat reset.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_RequiredResetItem_Name))]
    public ItemDefinition? RequiredResetItem { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the chat command is enabled.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_ChatCommandEnabled_Name))]
    public bool ChatCommandEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a stat reset moves the player home (safe zone).
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_MoveHome_Name))]
    public bool MoveHome { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a stat reset logs the player out back to character selection.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.StatResetConfiguration_LogOut_Name))]
    public bool LogOut { get; set; } = true;
}
