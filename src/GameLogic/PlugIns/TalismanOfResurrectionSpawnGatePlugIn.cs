// <copyright file="TalismanOfResurrectionSpawnGatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which respawns a dead player at the place of its death, when it carries a Talisman of Resurrection
/// in its inventory. One talisman is used up by it.
/// </summary>
/// <remarks>
/// It doesn't apply in mini games, duels and guild wars, which have their own spawn gates.
/// </remarks>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.TalismanOfResurrectionSpawnGatePlugIn_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.TalismanOfResurrectionSpawnGatePlugIn_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("5C057E6E-FECF-4216-AB78-1918AB00224E")]
public class TalismanOfResurrectionSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
{
    /// <inheritdoc />
    public async ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
    {
        if (args.Gate is not null
            || player.IsAlive
            || player.CurrentMap is not { } map
            || player.CurrentMiniGame is not null
            || player.DuelRoom is not null
            || player.GuildWarContext is not null
            || player.Inventory is not { } inventory
            || inventory.Items.FirstOrDefault(IsTalismanOfResurrection) is not { } talisman)
        {
            return;
        }

        var position = player.Position;
        args.Gate = new ExitGate
        {
            Map = map.Definition,
            X1 = position.X,
            X2 = position.X,
            Y1 = position.Y,
            Y2 = position.Y,
        };

        var slot = talisman.ItemSlot;
        await inventory.RemoveItemAsync(talisman).ConfigureAwait(false);
        await player.PersistenceContext.DeleteAsync(talisman).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemRemovedPlugIn>(p => p.RemoveItemAsync(slot)).ConfigureAwait(false);
    }

    private static bool IsTalismanOfResurrection(Item item)
    {
        return item.Definition is { } definition
               && definition.Group == ItemConstants.TalismanOfResurrection.Group
               && definition.Number == ItemConstants.TalismanOfResurrection.Number;
    }
}
