// <copyright file="TalismanOfResurrectionSpawnGatePlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="TalismanOfResurrectionSpawnGatePlugIn"/>.
/// </summary>
public class TalismanOfResurrectionSpawnGatePlugInConfiguration
{
    /// <summary>
    /// Gets or sets the group of the item which respawns a dead player at the place of its death.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.TalismanOfResurrectionSpawnGatePlugInConfiguration_ItemGroup_Caption))]
    public byte ItemGroup { get; set; } = ItemConstants.TalismanOfResurrection.Group;

    /// <summary>
    /// Gets or sets the number of the item which respawns a dead player at the place of its death.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.TalismanOfResurrectionSpawnGatePlugInConfiguration_ItemNumber_Caption))]
    public short ItemNumber { get; set; } = ItemConstants.TalismanOfResurrection.Number!.Value;
}
