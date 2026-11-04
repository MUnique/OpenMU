// <copyright file="SignOfDimensionsStackedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin transforms a full stack of signs of dimensions into a mirror of dimensions,
/// which is the ticket of the doppelganger event.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.SignOfDimensionsStackedPlugIn_Name), Description = nameof(PlugInResources.SignOfDimensionsStackedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6D2F8B41-0C7E-4A93-B15D-E84A3F29C706")]
public sealed class SignOfDimensionsStackedPlugIn : IItemStackedPlugIn
{
    private const byte ItemGroup = 14;
    private const short SignOfDimensionsNumber = 110;
    private const short MirrorOfDimensionsNumber = 111;

    /// <inheritdoc />
    public async ValueTask ItemStackedAsync(Player player, Item sourceItem, Item targetItem)
    {
        if (targetItem.Definition is not { Group: ItemGroup, Number: SignOfDimensionsNumber } signOfDimensions
            || targetItem.Durability < signOfDimensions.Durability)
        {
            return;
        }

        var mirrorOfDimensions = player.GameContext.Configuration.Items.FirstOrDefault(item => item is { Group: ItemGroup, Number: MirrorOfDimensionsNumber });
        if (mirrorOfDimensions is null)
        {
            player.Logger.LogWarning("Mirror of dimensions definition not found.");
            return;
        }

        await player.InvokeViewPlugInAsync<Views.Inventory.IItemRemovedPlugIn>(p => p.RemoveItemAsync(targetItem.ItemSlot)).ConfigureAwait(false);
        targetItem.Definition = mirrorOfDimensions;
        targetItem.Durability = 1;
        await player.InvokeViewPlugInAsync<Views.Inventory.IItemAppearPlugIn>(p => p.ItemAppearAsync(targetItem)).ConfigureAwait(false);
    }
}
