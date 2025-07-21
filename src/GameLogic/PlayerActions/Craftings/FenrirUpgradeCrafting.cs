// <copyright file="FenrirUpgradeCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// A crafting to upgrade a Red Fenrir to a Blue or Black one.
/// </summary>
public class FenrirUpgradeCrafting : BaseItemCraftingHandler
{
    private readonly ItemPriceCalculator _priceCalculator = new();

    /// <inheritdoc/>
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems)
    {
        successRateByItems = 0;
        items = new List<CraftingRequiredItemLink>(4);
        var inputItems = player.TemporaryStorage!.Items.ToList();
        var itemsLevelAndOption4 = inputItems
            .Where(item => item.Level >= 4
                && item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Option))
            .ToList();
        var randomWeapons = itemsLevelAndOption4
            .Where(item => item.IsWearable()
                && item.Definition!.BasePowerUpAttributes.Any(a => a.TargetAttribute == Stats.AttackSpeedByWeapon))
            .ToList();
        var randomArmors = itemsLevelAndOption4
            .Where(item => item.IsWearable()
                && item.Definition!.BasePowerUpAttributes.Any(a => a.TargetAttribute == Stats.DefenseBase))
            .ToList();

        if (randomArmors.Any() && randomWeapons.Any())
        {
            // Either Weapons or Armors, not both
            return CraftingResult.IncorrectMixItems;
        }

        if (!randomArmors.Any() && !randomWeapons.Any())
        {
            return CraftingResult.LackingMixItems;
        }

        var hornOfFenrir = inputItems.FirstOrDefault(item => item.Definition?.Name == "Horn of Fenrir");
        var chaos = inputItems.FirstOrDefault(item => item.Definition?.Name == "Jewel of Chaos");
        var jewelsOfLife = inputItems.Where(item => item.Definition?.Name == "Jewel of Life").Take(5).ToList();

        if (hornOfFenrir is null
            || chaos is null
            || jewelsOfLife.Count < 5)
        {
            return CraftingResult.LackingMixItems;
        }

        inputItems.Remove(hornOfFenrir);
        inputItems.Remove(chaos);
        jewelsOfLife.ForEach(item => inputItems.Remove(item));
        randomWeapons.ForEach(item => inputItems.Remove(item));
        randomArmors.ForEach(item => inputItems.Remove(item));

        if (inputItems.Any())
        {
            return CraftingResult.TooManyItems;
        }

        items.Add(new CraftingRequiredItemLink(hornOfFenrir.GetAsEnumerable(), new TransientItemCraftingRequiredItem { PossibleItems = { hornOfFenrir.Definition! }, MinimumAmount = 1, MaximumAmount = 1, Reference = 1, SuccessResult = MixResult.StaysAsIs }));
        items.Add(new CraftingRequiredItemLink(chaos.GetAsEnumerable(), new TransientItemCraftingRequiredItem { PossibleItems = { chaos.Definition! }, MinimumAmount = 1, MaximumAmount = 1 }));
        items.Add(new CraftingRequiredItemLink(jewelsOfLife, new TransientItemCraftingRequiredItem { PossibleItems = { jewelsOfLife.First().Definition! }, MinimumAmount = 5, MaximumAmount = 5 }));
        if (randomWeapons.Any())
        {
            items.Add(new CraftingRequiredItemLink(randomWeapons, new TransientItemCraftingRequiredItem { MinimumAmount = 1, MaximumAmount = 1, Reference = 2 }));
            successRateByItems = (byte)Math.Min(79, randomWeapons.Sum(this._priceCalculator.CalculateSellingPrice) * 100 / 1_000_000);
        }
        else
        {
            items.Add(new CraftingRequiredItemLink(randomArmors, new TransientItemCraftingRequiredItem { MinimumAmount = 1, MaximumAmount = 1, Reference = 3 }));
            successRateByItems = (byte)Math.Min(79, randomArmors.Sum(this._priceCalculator.CalculateSellingPrice) * 100 / 1_000_000);
        }

        return null;
    }

    /// <inheritdoc />
    protected override int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems)
    {
        return 10_000_000;
    }

    /// <inheritdoc/>
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketIndex, byte successRate)
    {
        var fenrir = requiredItems.First(i => i.ItemRequirement.Reference == 1).Items.First();
        fenrir.Durability = 255;

        IEnumerable<IncreasableItemOption> fenrirOptions;
        if (requiredItems.Any(i => i.ItemRequirement.Reference == 2))
        {
            fenrirOptions = fenrir.Definition!.PossibleItemOptions.SelectMany(opt =>
                opt.PossibleOptions.Where(o => o.OptionType == ItemOptionTypes.BlackFenrir));
        }
        else
        {
            fenrirOptions = fenrir.Definition!.PossibleItemOptions.SelectMany(opt =>
                opt.PossibleOptions.Where(o => o.OptionType == ItemOptionTypes.BlueFenrir));
        }

        foreach (var option in fenrirOptions)
        {
            var optionLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            fenrir.ItemOptions.Add(optionLink);
        }

        return new List<Item> { fenrir };
    }
}