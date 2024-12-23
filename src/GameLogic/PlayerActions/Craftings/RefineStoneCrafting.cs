// <copyright file="RefineStoneCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Crafting which creates refine stones.
/// </summary>
public class RefineStoneCrafting : SimpleItemCraftingHandler
{
    #region List of excluded Items (if item level < 4)

    private readonly ISet<(byte Group, short Number)> _excludedItems = new HashSet<(byte, short)>
    {
        (0, 0), // Kris
        (0, 1), // Short Sword
        (0, 2), // Rapier
        (0, 4), // Sword of Assassin
        (1, 0), // Small Axe
        (1, 1), // Hand Axe
        (1, 2), // Double Axe
        (2, 0), // Mace
        (2, 1), // Morning Star
        (2, 2), // Flail
        (3, 1), // Spear
        (3, 2), // Dragon Lance
        (3, 3), // Giant Trident
        (3, 5), // Double Poleaxe
        (4, 0), // Short Bow
        (4, 1), // Bow
        (4, 2), // Elven Bow
        (4, 3), // Battle Bow
        (4, 8), // Crossbow
        (4, 9), // Golden Crossbow
        (4, 10), // Arquebus
        (4, 11), // Light Crossbow
        (5, 0), // Skull Staff
        (5, 1), // Angelic Staff
        (5, 2), // Serpent Staff
        (6, 0), // Small Shield
        (6, 1), // Horn Shield
        (6, 2), // Kite Shield
        (6, 3), // Elven Shield
        (6, 4), // Buckler
        (6, 6), // Skull Shield
        (6, 7), // Spiked Shield
        (6, 9), // Plate Shield
        (6, 10), // Big Round Shield
        (7, 0), // Bronze Helm
        (7, 2), // Pad Helm
        (7, 4), // Bone Helm
        (7, 5), // Leather Helm
        (7, 6), // Scale Helm
        (7, 7), // Sphinx Mask
        (7, 8), // Brass Helm
        (7, 10), // Vine Helm
        (7, 11), // Silk Helm
        (7, 12), // Wind Helm
        (8, 0), // Bronze Armor
        (8, 2), // Pad Armor
        (8, 4), // Bone Armor
        (8, 5), // Leather Armor
        (8, 6), // Scale Armor
        (8, 7), // Sphinx Armor
        (8, 8), // Brass Armor
        (8, 10), // Vine Armor
        (8, 11), // Silk Armor
        (8, 12), // Wind Armor
        (9, 0), // Bronze Pants
        (9, 2), // Pad Pants
        (9, 4), // Bone Pants
        (9, 5), // Leather Pants
        (9, 6), // Scale Pants
        (9, 7), // Sphinx Pants
        (9, 8), // Brass Pants
        (9, 10), // Vine Pants
        (9, 11), // Silk Pants
        (9, 12), // Wind Pants
        (10, 0), // Bronze Gloves
        (10, 2), // Pad Gloves
        (10, 4), // Bone Gloves
        (10, 5), // Leather Gloves
        (10, 6), // Scale Gloves
        (10, 7), // Sphinx Gloves
        (10, 8), // Brass Gloves
        (10, 10), // Vine Gloves
        (10, 11), // Silk Gloves
        (10, 12), // Wind Gloves
        (11, 0), // Bronze Boots
        (11, 2), // Pad Boots
        (11, 4), // Bone Boots
        (11, 5), // Leather Boots
        (11, 6), // Scale Boots
        (11, 7), // Sphinx Boots
        (11, 8), // Brass Boots
        (11, 10), // Vine Boots
        (11, 11), // Silk Boots
        (11, 12), // Wind Boots
    };

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="RefineStoneCrafting"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public RefineStoneCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
    }

    /// <summary>
    /// Gets the reference of the lower refine stone which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte LowerRefineStoneReference { get; } = 0x11;

    /// <summary>
    /// Gets the reference of the higher refine stone which must be specified in the <see cref="ItemCraftingRequiredItem.Reference"/>.
    /// </summary>
    public static byte HigherRefineStoneReference { get; } = 0x22;

    /// <inheritdoc />
    protected override bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
    {
        return base.RequiredItemMatches(item, requiredItem)
               && item.IsWearable()
               && !item.IsAncient()
               && !item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.HarmonyOption)
               && (!this._excludedItems.Contains((item.Definition!.Group, item.Definition.Number)) || item.Level > 3);
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> referencedItems, Player player, byte socketSlot, byte successRate)
    {
        var higherRefineStoneItems = referencedItems
            .FirstOrDefault(r => r.ItemRequirement.Reference == HigherRefineStoneReference)?.Items.Count() ?? 0;

        var lowerRefineStoneItems = referencedItems
            .FirstOrDefault(r => r.ItemRequirement.Reference == LowerRefineStoneReference)?.Items.Count() ?? 0;

        var result = new List<Item>();
        if (higherRefineStoneItems > 0)
        {
            result.AddRange(await this.CreateRefineStonesAsync(higherRefineStoneItems, 50, 44, player).ConfigureAwait(false));
        }

        if (lowerRefineStoneItems > 0)
        {
            result.AddRange(await this.CreateRefineStonesAsync(lowerRefineStoneItems, 20, 43, player).ConfigureAwait(false));
        }

        return result;
    }

    private async Task<List<Item>> CreateRefineStonesAsync(int count, int chance, byte refineStoneNumber, Player player)
    {
        var createdItems = new List<Item>();
        for (int i = 0; i < count; i++)
        {
            if (Rand.NextRandomBool(chance))
            {
                var refineStone = player.PersistenceContext.CreateNew<Item>();
                refineStone.Definition = player.GameContext.Configuration.Items.First(item => item.Group == 14 && item.Number == refineStoneNumber);
                refineStone.Durability = 1;
                await player.TemporaryStorage!.AddItemAsync(refineStone).ConfigureAwait(false);
                createdItems.Add(refineStone);
            }
        }

        return createdItems;
    }
}