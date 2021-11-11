﻿// <copyright file="ItemCraftAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The action to craft items with crafting NPCs.
/// </summary>
public class ItemCraftAction
{
    private readonly IDictionary<ItemCrafting, IItemCraftingHandler> _craftingHandlerCache = new Dictionary<ItemCrafting, IItemCraftingHandler>();

    /// <summary>
    /// Mixes the items at the currently opened crafting NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="mixTypeId">The mix type identifier.</param>
    /// <param name="socketSlot">The socket slot.</param>
    public void MixItems(Player player, byte mixTypeId, byte socketSlot)
    {
        var npcStats = player.OpenedNpc?.Definition;

        var crafting = npcStats?.ItemCraftings.FirstOrDefault(c => c.Number == mixTypeId);
        if (crafting is null)
        {
            return;
        }

        if (!this._craftingHandlerCache.TryGetValue(crafting, out var craftingHandler))
        {
            craftingHandler = this.CreateCraftingHandler(crafting);
            this._craftingHandlerCache.Add(crafting, craftingHandler);
        }

        (CraftingResult, Item?) result;
        try
        {
            result = craftingHandler.DoMix(player, socketSlot);
        }
        catch
        {
            result = (CraftingResult.LackingMixItems, null);
        }

        var itemList = player.TemporaryStorage?.Items.ToList() ?? new List<Item>();
        player.ViewPlugIns.GetPlugIn<IShowItemCraftingResultPlugIn>()?.ShowResult(result.Item1, itemList.Count > 1 ? null : result.Item2);
        player.ViewPlugIns.GetPlugIn<IShowMerchantStoreItemListPlugIn>()?.ShowMerchantStoreItemList(
            itemList,
            npcStats!.NpcWindow == NpcWindow.PetTrainer && result.Item1 != CraftingResult.Success ? StoreKind.ResurrectionFailed : StoreKind.ChaosMachine);
    }

    private IItemCraftingHandler CreateCraftingHandler(ItemCrafting crafting)
    {
        if (!string.IsNullOrWhiteSpace(crafting.ItemCraftingHandlerClassName))
        {
            var type = Type.GetType(crafting.ItemCraftingHandlerClassName);
            if (type != null)
            {
                if (type.BaseType == typeof(SimpleItemCraftingHandler))
                {
                    return (IItemCraftingHandler)Activator.CreateInstance(type, crafting.SimpleCraftingSettings)!;
                }

                return (IItemCraftingHandler)Activator.CreateInstance(type)!;
            }

            throw new ArgumentException($"Item crafting handler '{crafting.ItemCraftingHandlerClassName}' not found.", nameof(crafting));
        }

        if (crafting.SimpleCraftingSettings != null)
        {
            return new SimpleItemCraftingHandler(crafting.SimpleCraftingSettings);
        }

        throw new ArgumentException("No simple crafting settings or item crafting handler name specified.", nameof(crafting));
    }
}