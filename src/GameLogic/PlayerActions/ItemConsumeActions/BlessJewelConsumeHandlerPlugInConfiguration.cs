// <copyright file="BlessJewelConsumeHandlerPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The configuration for the <see cref="BlessJewelConsumeHandlerPlugIn"/>.
/// </summary>
public class BlessJewelConsumeHandlerPlugInConfiguration : UpgradeItemLevelConfiguration
{
    /// <summary>
    /// Gets or sets the items which can be repaired by consuming a bless on them.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.BlessJewelConsumeHandlerPlugInConfiguration_RepairTargetItems_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public ICollection<ItemDefinition> RepairTargetItems { get; set; } = new List<ItemDefinition>();
}