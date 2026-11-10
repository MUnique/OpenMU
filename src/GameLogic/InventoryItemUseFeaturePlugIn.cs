// <copyright file="InventoryItemUseFeaturePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The use of items inside the inventory, like the figurines and charms.
/// While such an item is in use, its power-ups apply, although it isn't equipped.
/// When it's deactivated, no item can be used inside the inventory.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.InventoryItemUseFeaturePlugIn_Name), Description = nameof(PlugInResources.InventoryItemUseFeaturePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("0C9C3D52-7B0E-4E55-9D1B-2E6A35C4F1A8")]
public class InventoryItemUseFeaturePlugIn : IFeaturePlugIn, ISupportCustomConfiguration<InventoryItemUseConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public InventoryItemUseConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets the configuration of the items which are used inside the inventory.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration; <c>null</c>, if the feature is deactivated.</returns>
    public static InventoryItemUseConfiguration? GetActiveConfiguration(IGameContext gameContext)
    {
        return gameContext.FeaturePlugIns.GetPlugIn<InventoryItemUseFeaturePlugIn>() is { } plugIn
            ? plugIn.Configuration ?? new InventoryItemUseConfiguration()
            : null;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new InventoryItemUseConfiguration();
    }
}
