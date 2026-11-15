// <copyright file="ItemCraftAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The action to craft items with crafting NPCs.
/// </summary>
public class ItemCraftAction
{
    private readonly ConcurrentDictionary<ItemCrafting, IItemCraftingHandler> _craftingHandlerCache = new();

    /// <summary>
    /// Mixes the items at the currently opened crafting NPC.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="mixTypeId">The mix type identifier.</param>
    /// <param name="socketSlot">The socket slot.</param>
    public async ValueTask MixItemsAsync(Player player, byte mixTypeId, byte socketSlot)
    {
        var npcStats = player.OpenedNpc?.Definition;

        var crafting = this.FindCrafting(player, npcStats, mixTypeId);
        if (crafting is null)
        {
            await player.InvokeViewPlugInAsync<IShowItemCraftingResultPlugIn>(p => p.ShowResultAsync(CraftingResult.IncorrectMixItems, null)).ConfigureAwait(false);
            return;
        }

        var craftingHandler = this.GetCraftingHandler(crafting);

        (CraftingResult, Item?) result;
        try
        {
            result = await craftingHandler.DoMixAsync(player, socketSlot).ConfigureAwait(false);
        }
        catch
        {
            result = (CraftingResult.LackingMixItems, null);
        }

        var itemList = player.TemporaryStorage?.Items.ToList() ?? new List<Item>();
        await player.InvokeViewPlugInAsync<IShowItemCraftingResultPlugIn>(p => p.ShowResultAsync(result.Item1, itemList.Count > 1 ? null : result.Item2)).ConfigureAwait(false);
        if (npcStats!.NpcWindow != NpcWindow.CombineLuckyItem)
        {
            // The lucky item dialog gets its result item with the crafting result. The item list would put the
            // items into the chaos machine dialog of the client instead.
            await player.InvokeViewPlugInAsync<IShowMerchantStoreItemListPlugIn>(
                p => p.ShowMerchantStoreItemListAsync(
                    itemList,
                    npcStats.NpcWindow == NpcWindow.PetTrainer && result.Item1 != CraftingResult.Success ? StoreKind.ResurrectionFailed : StoreKind.ChaosMachine))
                .ConfigureAwait(false);
        }

        await this.ShowCraftingPreviewAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Finds the relevant <see cref="ItemCrafting"/> by testing the mix items against every crafting's item requirements.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The relevant <see cref="ItemCrafting"/>.</returns>
    public ItemCrafting? FindAppropriateCraftingByItems(Player player)
    {
        if (player.OpenedNpc?.Definition is { } npc)
        {
            // Wing crafting is similar but has one extra requirement (chaos weapon) than
            // chaos weapon crafting, so we set a descending order so it gets checked first
            foreach (var itemCrafting in npc.ItemCraftings.OrderByDescending(c => c.Number))
            {
                if (this.GetCraftingHandler(itemCrafting).TryGetRequiredItems(player, out _, out _) is null)
                {
                    return itemCrafting;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Shows the crafting which matches the items of the crafting dialog, with its success rate and price,
    /// if the client of the player supports it.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowCraftingPreviewAsync(Player player)
    {
        if (player.ViewPlugIns.GetPlugIn<IShowItemCraftingPreviewPlugIn>() is not { } previewPlugIn)
        {
            return;
        }

        if (this.FindAppropriateCraftingByItems(player) is { } crafting
            && this.GetCraftingHandler(crafting) is BaseItemCraftingHandler craftingHandler
            && craftingHandler.GetRequiredItemsAndPrice(player) is (null, _, var successRate, var price))
        {
            await previewPlugIn.ShowCraftingPreviewAsync(crafting, successRate, price).ConfigureAwait(false);
            return;
        }

        await previewPlugIn.ShowCraftingPreviewAsync(null, 0, 0).ConfigureAwait(false);
    }

    /// <summary>
    /// Finds the crafting of the npc with the mix type identifier which the client sent.
    /// </summary>
    /// <remarks>
    /// The client sends the same identifier for some different combinations, e.g. for creating
    /// the Golden Box and the Silver Box. Then, the crafting whose required items are in the dialog is taken.
    /// </remarks>
    private ItemCrafting? FindCrafting(Player player, MonsterDefinition? npc, byte mixTypeId)
    {
        var craftings = npc?.ItemCraftings.Where(c => c.Number == mixTypeId).ToList();
        if (craftings is not { Count: > 0 })
        {
            return null;
        }

        if (craftings.Count == 1)
        {
            return craftings[0];
        }

        return craftings.FirstOrDefault(c => this.GetCraftingHandler(c).TryGetRequiredItems(player, out _, out _) is null)
               ?? craftings[0];
    }

    private IItemCraftingHandler GetCraftingHandler(ItemCrafting crafting)
    {
        // This action may be shared between players, e.g. by the message handlers, so the cache must be thread-safe.
        return this._craftingHandlerCache.GetOrAdd(crafting, this.CreateCraftingHandler);
    }

    private IItemCraftingHandler CreateCraftingHandler(ItemCrafting crafting)
    {
        if (!string.IsNullOrWhiteSpace(crafting.ItemCraftingHandlerClassName))
        {
            if (ItemCraftingHandlerFactory.Create(crafting.ItemCraftingHandlerClassName, crafting.SimpleCraftingSettings) is { } handler)
            {
                return handler;
            }

            // Not a handler of this assembly, e.g. an assembly qualified type name of another assembly.
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