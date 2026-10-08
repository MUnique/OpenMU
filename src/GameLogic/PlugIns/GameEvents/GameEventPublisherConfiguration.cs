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

    /// <summary>
    /// Gets or sets a value indicating whether the kills of boss monsters are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishBossKills_Name))]
    public bool PublishBossKills { get; set; } = true;

    /// <summary>
    /// Gets or sets the numbers of the monsters which are bosses, e.g. 275 for Kundun.
    /// </summary>
    /// <remarks>
    /// The monster definitions don't tell which monsters are bosses, so the server owner decides it here.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_BossMonsterNumbers_Name))]
    public IList<short> BossMonsterNumbers { get; set; } = new List<short>();

    /// <summary>
    /// Gets or sets a value indicating whether characters which reach one of the <see cref="LevelMilestones"/>
    /// or <see cref="MasterLevelMilestones"/> are published.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_PublishLevelMilestones_Name))]
    public bool PublishLevelMilestones { get; set; } = true;

    /// <summary>
    /// Gets or sets the levels which are published when a character reaches them.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_LevelMilestones_Name))]
    public IList<int> LevelMilestones { get; set; } = new List<int> { 400 };

    /// <summary>
    /// Gets or sets the master levels which are published when a character reaches them.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventPublisherConfiguration_MasterLevelMilestones_Name))]
    public IList<int> MasterLevelMilestones { get; set; } = new List<int>();
}
