// <copyright file="SymbolOfKundunStackedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin transforms a stack of symbol of kundun into a lost map.
/// </summary>
[PlugIn(nameof(SymbolOfKundunStackedPlugIn), "This plugin transforms a stack of symbol of kundun into a lost map.")]
[Guid("F07A9CED-F43E-4824-9587-F5C3C3187A13")]
public sealed class SymbolOfKundunStackedPlugIn : IItemStackedPlugIn
{
    private const byte SymbolOfKundunGroup = 14;
    private const byte SymbolOfKundunNumber = 29;
    private const byte LostMapGroup = 14;
    private const byte LostMapNumber = 28;

    /// <inheritdoc />
    public async ValueTask ItemStackedAsync(Player player, Item sourceItem, Item targetItem)
    {
        if (targetItem.Definition is not { Group: SymbolOfKundunGroup, Number: SymbolOfKundunNumber })
        {
            return;
        }

        if (targetItem.Durability() < targetItem.Definition.Durability)
        {
            return;
        }

        var lostMap = player.GameContext.Configuration.Items.FirstOrDefault(item => item is { Group: LostMapGroup, Number: LostMapNumber });
        if (lostMap is null)
        {
            player.Logger.LogWarning("Lost map definition not found.");
            return;
        }

        await player.InvokeViewPlugInAsync<Views.Inventory.IItemRemovedPlugIn>(p => p.RemoveItemAsync(targetItem.ItemSlot)).ConfigureAwait(false);
        targetItem.Definition = lostMap;
        targetItem.Durability = 1;
        await player.InvokeViewPlugInAsync<Views.Inventory.IItemAppearPlugIn>(p => p.ItemAppearAsync(targetItem)).ConfigureAwait(false);
    }
}
