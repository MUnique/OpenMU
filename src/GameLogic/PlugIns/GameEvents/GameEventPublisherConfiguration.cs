// <copyright file="GameEventPublisherConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

/// <summary>
/// Configuration for the <see cref="GameEventPublisherPlugIn"/>.
/// </summary>
public class GameEventPublisherConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether the opening, start and end of mini games are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishMiniGameEvents_Name))]
    public bool PublishMiniGameEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the start and end of invasions are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishInvasionEvents_Name))]
    public bool PublishInvasionEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the state changes of the castle siege are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishCastleSiegeEvents_Name))]
    public bool PublishCastleSiegeEvents { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the global notices of game masters are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishGlobalNotices_Name))]
    public bool PublishGlobalNotices { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether excellent items which are dropped by monsters are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishExcellentItemDrops_Name))]
    public bool PublishExcellentItemDrops { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether ancient items which are dropped by monsters are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishAncientItemDrops_Name))]
    public bool PublishAncientItemDrops { get; set; } = true;
}
