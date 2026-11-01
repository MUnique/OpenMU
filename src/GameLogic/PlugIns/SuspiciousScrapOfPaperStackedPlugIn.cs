// <copyright file="SuspiciousScrapOfPaperStackedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Transforms a full stack of suspicious scraps of paper into a Gaion's Order,
/// which is the ticket of the imperial guardian event.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SuspiciousScrapOfPaperStackedPlugIn_Name), Description = nameof(PlugInResources.SuspiciousScrapOfPaperStackedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E1A74C92-8B35-4F0D-9D26-3C5B8E1F7A40")]
public sealed class SuspiciousScrapOfPaperStackedPlugIn : IItemStackedPlugIn
{
    private const byte ItemGroup = 14;
    private const short SuspiciousScrapOfPaperNumber = 101;
    private const short GaionsOrderNumber = 102;

    /// <inheritdoc />
    public async ValueTask ItemStackedAsync(Player player, Item sourceItem, Item targetItem)
    {
        if (targetItem.Definition is not { Group: ItemGroup, Number: SuspiciousScrapOfPaperNumber } scrapOfPaper
            || targetItem.Durability < scrapOfPaper.Durability)
        {
            return;
        }

        var gaionsOrder = player.GameContext.Configuration.Items.FirstOrDefault(item => item is { Group: ItemGroup, Number: GaionsOrderNumber });
        if (gaionsOrder is null)
        {
            player.Logger.LogWarning("Gaion's Order definition not found.");
            return;
        }

        await player.InvokeViewPlugInAsync<Views.Inventory.IItemRemovedPlugIn>(p => p.RemoveItemAsync(targetItem.ItemSlot)).ConfigureAwait(false);
        targetItem.Definition = gaionsOrder;
        targetItem.Durability = 1;
        await player.InvokeViewPlugInAsync<Views.Inventory.IItemAppearPlugIn>(p => p.ItemAppearAsync(targetItem)).ConfigureAwait(false);
    }
}
