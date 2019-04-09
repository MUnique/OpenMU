// <copyright file="ItemCraftAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

    /// <summary>
    /// The action to craft items with crafting NPCs.
    /// </summary>
    public class ItemCraftAction
    {
        private readonly IDictionary<ItemCrafting, IItemCraftingHandler> craftingHandlerCache = new Dictionary<ItemCrafting, IItemCraftingHandler>();

        /// <summary>
        /// Mixes the items at the currently opened Monster crafter.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="mixTypeId">The mix type identifier.</param>
        public void MixItems(Player player, byte mixTypeId)
        {
            var npcStats = player.OpenedNpc.Definition;

            ItemCrafting crafting = npcStats?.ItemCraftings.FirstOrDefault(c => c.Number == mixTypeId);
            if (crafting == null)
            {
                return;
            }

            IItemCraftingHandler craftingHandler;
            if (!this.craftingHandlerCache.TryGetValue(crafting, out craftingHandler))
            {
                craftingHandler = this.CreateCraftingHandler(player, crafting);
                this.craftingHandlerCache.Add(crafting, craftingHandler);
            }

            craftingHandler.DoMix(player);
        }

        private IItemCraftingHandler CreateCraftingHandler(Player player, ItemCrafting crafting)
        {
            if (crafting.SimpleCraftingSettings != null)
            {
                return new SimpleItemCraftingHandler(crafting.SimpleCraftingSettings);
            }

            if (crafting.ItemCraftingHandlerClassName != null)
            {
                var type = Type.GetType(crafting.ItemCraftingHandlerClassName);
                if (type != null)
                {
                    return Activator.CreateInstance(type, player.GameContext) as IItemCraftingHandler;
                }

                throw new ArgumentException($"Item crafting handler '{crafting.ItemCraftingHandlerClassName}' not found.", nameof(crafting));
            }

            throw new ArgumentException("No simple crafting settings or item crafting handler name specified.", nameof(crafting));
        }
    }
}
