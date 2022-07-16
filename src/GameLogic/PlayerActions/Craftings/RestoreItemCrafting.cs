// <copyright file="RestoreItemCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting which removes the <see cref="ItemOptionTypes.HarmonyOption"/> from an item.
/// </summary>
public class RestoreItemCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RestoreItemCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public RestoreItemCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot)
    {
        var item = requiredItems.First().Items.First();
        var johOptionLink = item.ItemOptions.First(link => link.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption);
        item.ItemOptions.Remove(johOptionLink);
        await player.PersistenceContext.DeleteAsync(johOptionLink).ConfigureAwait(false);
        return new List<Item> { item };
    }
}