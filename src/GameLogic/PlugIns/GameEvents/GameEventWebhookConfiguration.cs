// <copyright file="GameEventWebhookConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.GameEvents;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configuration for the <see cref="GameEventWebhookPlugIn"/>.
/// </summary>
public class GameEventWebhookConfiguration
{
    /// <summary>
    /// Gets or sets the URL to which the events are posted, e.g. <c>https://example.com/api/realm-events</c>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_Url_Name), Description = nameof(PlugInResources.GameEventWebhookConfiguration_Url_Description))]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the shared secret with which the requests are signed.
    /// </summary>
    [Secret]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_Secret_Name), Description = nameof(PlugInResources.GameEventWebhookConfiguration_Secret_Description))]
    public string? Secret { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the kills of boss monsters are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardBossKills_Name))]
    public bool ForwardBossKills { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether level milestones of characters are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardLevelMilestones_Name))]
    public bool ForwardLevelMilestones { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether item drops are forwarded, as far as they pass <see cref="DropItemNames"/>
    /// and <see cref="DropMinimumItemLevel"/>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardItemDrops_Name))]
    public bool ForwardItemDrops { get; set; } = true;

    /// <summary>
    /// Gets or sets the names of the items whose drops are forwarded. When empty, every published drop is forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_DropItemNames_Name), Description = nameof(PlugInResources.GameEventWebhookConfiguration_DropItemNames_Description))]
    public IList<string> DropItemNames { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the minimum item level of a forwarded drop.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_DropMinimumItemLevel_Name))]
    public byte DropMinimumItemLevel { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the results of mini games are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardMiniGameResults_Name))]
    public bool ForwardMiniGameResults { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the openings and starts of mini games are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardMiniGameOpenings_Name))]
    public bool ForwardMiniGameOpenings { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the start and end of invasions are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardInvasions_Name))]
    public bool ForwardInvasions { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the state changes of the castle siege are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardCastleSiege_Name))]
    public bool ForwardCastleSiege { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the global notices of game masters are forwarded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GameEventWebhookConfiguration_ForwardGlobalNotices_Name))]
    public bool ForwardGlobalNotices { get; set; } = true;
}
