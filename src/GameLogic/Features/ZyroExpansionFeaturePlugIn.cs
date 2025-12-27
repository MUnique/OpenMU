// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.Features;

using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Feature configuration for Zyro (Wandering Merchant) expansion quests.
/// Allows configuring the required reset counts per quest.
/// </summary>
[PlugIn("Zyro Expansion Feature", "Configures reset requirements for Zyro expansion quests.")]
[Guid("A3D031D4-282A-4C8C-9C1E-51C8C2E83E7B")]
public class ZyroExpansionFeaturePlugIn : IFeaturePlugIn,
    ISupportCustomConfiguration<ZyroExpansionConfiguration>,
    ISupportDefaultCustomConfiguration
{
    /// <summary>
    /// Gets or sets the configuration.
    /// </summary>
    public ZyroExpansionConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new ZyroExpansionConfiguration
    {
        QuestGroup = 200,
        ResetsRequiredForVault = 5,
        ResetsRequiredForInventory1 = 10,
        ResetsRequiredForInventory2 = 15,
    };
}

/// <summary>
/// Configuration for <see cref="ZyroExpansionFeaturePlugIn"/>.
/// </summary>
public class ZyroExpansionConfiguration
{
    /// <summary>
    /// Gets or sets the quest group id which is used for Zyro expansion quests.
    /// </summary>
    [Display(Name = "Quest Group")] 
    public short QuestGroup { get; set; }

    /// <summary>
    /// Gets or sets the reset requirement for the vault expansion quest.
    /// </summary>
    [Display(Name = "Resets for Vault Expansion")] 
    public int ResetsRequiredForVault { get; set; }

    /// <summary>
    /// Gets or sets the reset requirement for the first inventory expansion quest.
    /// </summary>
    [Display(Name = "Resets for Inventory Expansion #1")] 
    public int ResetsRequiredForInventory1 { get; set; }

    /// <summary>
    /// Gets or sets the reset requirement for the second inventory expansion quest.
    /// </summary>
    [Display(Name = "Resets for Inventory Expansion #2")] 
    public int ResetsRequiredForInventory2 { get; set; }
}

