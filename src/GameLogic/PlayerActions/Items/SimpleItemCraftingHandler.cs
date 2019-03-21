// <copyright file="SimpleItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.Inventory;

    /// <summary>
    /// The simple item crafting handler which can be configured to handle the most crafting requirements.
    /// </summary>
    public class SimpleItemCraftingHandler : IItemCraftingHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SimpleItemCraftingHandler));

        private readonly SimpleCraftingSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleItemCraftingHandler"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SimpleItemCraftingHandler(SimpleCraftingSettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        public bool DoMix(Player player)
        {
            var successRate = this.settings.SuccessPercent;

            // Find all Required Items first
            var items = new List<CraftingRequiredItemLink>(this.settings.RequiredItems.Count);
            var storage = player.TemporaryStorage.Items.ToList();
            foreach (var requiredItems in this.settings.RequiredItems)
            {
                var foundItems = storage
                    .Where(item => item.Definition == requiredItems.ItemDefinition)
                    .Where(item => requiredItems.RequiredItemOptions.All(r => item.ItemOptions.Any(o => o.ItemOption.OptionType == r)))
                    .Where(item => item.Level >= requiredItems.MinLvl).ToList();

                if (foundItems.Count < requiredItems.MinAmount)
                {
                    Log.WarnFormat("Suspicious action for player with name: {0}, could be hack attempt.", player.Name);
                    return false;
                }

                foundItems.ForEach(i => storage.Remove(i));
                items.Add(new CraftingRequiredItemLink(foundItems, requiredItems));
            }

            var success = Rand.NextRandomBool(successRate);
            if (success)
            {
                this.DoTheMix(items, player);
            }
            else
            {
                items.ForEach(i => this.RequiredItemChange(player, i, false));
            }

            return success;
        }

        private void DoTheMix(IList<CraftingRequiredItemLink> items, Player player)
        {
            var referencedItems = new List<CraftingRequiredItemLink>();

            foreach (var i in items)
            {
                if (i.ItemRequirement.RefID != 0)
                {
                    referencedItems.Add(i);
                }

                this.RequiredItemChange(player, i, true);
            }

            foreach (var i in this.settings.ResultItems)
            {
                if (i.ItemDefinition == null)
                {
                    return;
                }

                // Create new Item
                var resultItem = player.PersistenceContext.CreateNew<Item>();
                resultItem.Definition = i.ItemDefinition;
                resultItem.Level = (byte)Rand.NextInt(i.RandLvlMin, i.RandLvlMax);
                resultItem.Durability = resultItem.GetMaximumDurabilityOfOnePiece(); // TODO: I think sometimes that's not correct, e.g. Potions of Bless/Soul!
                player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()?.ItemAppear(resultItem); // TODO: item appear needs to know in which storage the item appears
            }
        }

        private void RequiredItemChange(Player player, CraftingRequiredItemLink i, bool success)
        {
            MixResult mr = success ? i.ItemRequirement.SuccessResult : i.ItemRequirement.FailResult;

            switch (mr)
            {
                case MixResult.Disappear:
                    var point = player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>();
                    foreach (var item in i.StoredItem)
                    {
                        player.TemporaryStorage.RemoveItem(item);
                        player.PersistenceContext.Delete(item);
                        point.ItemDestroyed(item);
                    }

                    //// TODO: Send ItemDisappear?
                    break;
                case MixResult.DowngradedRandom:
                    i.StoredItem.ForEach(item => item.Level = (byte)Rand.NextInt(0, item.Level));

                    // TODO: Send item updated
                    break;
                case MixResult.DowngradedTo0:
                    i.StoredItem.ForEach(item => item.Level = 0);

                    // TODO: Send item updated
                    break;
                default:
                    // The item stays as is.
                    break;
            }
        }
    }
}
