// <copyright file="GuardianOptionCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Crafting to add the Guardian Options (Level 380) to corresponding items.
/// </summary>
public class GuardianOptionCrafting : SimpleItemCraftingHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuardianOptionCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public GuardianOptionCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <summary>
    /// Gets the reference to the affected item which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte ItemReference { get; } = 0x88;

    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRate)
    {
        if (base.TryGetRequiredItems(player, out items, out successRate) is { } error)
        {
            return error;
        }

        if (items.Where(i => i.ItemRequirement.Reference == ItemReference).Sum(i => i.Items.Count()) > 1)
        {
            return CraftingResult.TooManyItems;
        }

        return default;
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        var item = requiredItems.First(i => i.ItemRequirement.Reference == ItemReference && i.Items.Any()).Items.First();
        foreach (var optionDefinition in item.Definition!.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.GuardianOption)).PossibleOptions)
        {
            var optionLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = optionDefinition;
            item.ItemOptions.Add(optionLink);
        }

        return new List<Item> { item };
    }

    /// <inheritdoc />
    protected override bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
    {
        if (requiredItem.Reference == 0)
        {
            return base.RequiredItemMatches(item, requiredItem);
        }

        return base.RequiredItemMatches(item, requiredItem)
               && item.Definition!.PossibleItemOptions.Any(o =>
                   o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.GuardianOption))
               && item.ItemOptions.All(o => o.ItemOption!.OptionType != ItemOptionTypes.GuardianOption);
    }
}