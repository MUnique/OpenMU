// <copyright file="ItemPriceCalculator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This calculator calculates the item prices.
/// </summary>
/// <remarks>
/// Without an <see cref="ItemDefinition.PriceDefinition"/>, an item gets the automatic equipment price: a base price
/// which depends on the drop level, increased by the skill and the options of the item.
/// Items with a different price reference an <see cref="ItemPriceDefinition"/>, see there for the calculation steps.
/// The prices have to match the ones which the game client calculates and shows.
/// </remarks>
public class ItemPriceCalculator
{
    private const long MaximumPrice = 3_000_000_000;
    private const float DestroyedPetPenalty = 2.0f;
    private const float DestroyedItemPenalty = 1.4f;

    /// <summary>
    /// The drop level increase of excellent items, for the price calculation.
    /// </summary>
    private const int ExcellentDropLevelIncrease = 25;

    private static readonly Dictionary<byte, int> DropLevelIncreaseByLevel = new()
    {
        { 5, 4 },
        { 6, 10 },
        { 7, 25 },
        { 8, 45 },
        { 9, 65 },
        { 10, 95 },
        { 11, 135 },
        { 12, 185 },
        { 13, 245 },
        { 14, 305 },
        { 15, 365 },
    };

    /// <summary>
    /// Calculates the selling price of the item for its maximum durability.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The selling price.</returns>
    public long CalculateSellingPrice(Item item) => this.CalculateSellingPrice(item, item.GetMaximumDurabilityOfOnePiece());

    /// <summary>
    /// Calculates the selling price of the item, which the player gets if he is selling an item to a merchant.
    /// It's usually a third of the buying price, minus a durability factor.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="durability">The current durability of the <paramref name="item"/>.</param>
    /// <returns>The selling price.</returns>
    public long CalculateSellingPrice(Item item, byte durability)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var sellingPrice = CalculateBuyingPrice(item) / 3;
        if (item.Definition.PriceDefinition?.SellingPriceRounding == ItemPriceRounding.Tens)
        {
            return sellingPrice / 10 * 10;
        }

        if (!item.IsTrainablePet())
        {
            var maxDurability = item.GetMaximumDurabilityOfOnePiece();
            if (maxDurability > 1 && maxDurability > durability)
            {
                float multiplier = 1.0f - ((float)durability / maxDurability);
                long loss = (long)(sellingPrice * 0.6 * multiplier);
                sellingPrice -= loss;
            }
        }

        return RoundPrice(sellingPrice);
    }

    /// <summary>
    /// Calculates the repair price of the item, which the player has to pay if he wants to repair the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="npcDiscount">If set to <c>true</c>, the item is repaired through an NPC which gives a discount.</param>
    /// <returns>The repair price.</returns>
    public long CalculateRepairPrice(Item item, bool npcDiscount)
    {
        if (item.GetMaximumDurabilityOfOnePiece() == 0)
        {
            return 0;
        }

        const long maximumBasePrice = 400_000_000;
        var isPet = item.IsTrainablePet();
        var basePrice = Math.Min(this.CalculateFinalBuyingPrice(item) / (isPet ? 1 : 3), maximumBasePrice);
        basePrice = RoundPrice(basePrice);

        float squareRootOfBasePrice = (float)Math.Sqrt(basePrice);
        float squareRootOfSquareRoot = (float)Math.Sqrt(squareRootOfBasePrice);
        float missingDurability = 1 - ((float)item.Durability() / item.GetMaximumDurabilityOfOnePiece());
        float repairPrice = (3.0f * squareRootOfBasePrice * squareRootOfSquareRoot * missingDurability) + 1.0f;
        if (item.Durability <= 0)
        {
            if (isPet)
            {
                repairPrice *= DestroyedPetPenalty;
            }
            else
            {
                repairPrice *= DestroyedItemPenalty;
            }
        }

        if (!npcDiscount)
        {
            repairPrice *= 2.5f;
        }

        return RoundPrice((long)repairPrice);
    }

    /// <summary>
    /// Calculates the final buying price of the item, which the player has to pay if he wants to buy the item from a merchant.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The buying price.</returns>
    public long CalculateFinalBuyingPrice(Item item) => RoundPrice(CalculateBuyingPrice(item));

    /// <summary>
    /// Calculates the final "old" buying price of the item.
    /// Supposedly in earlier versions jewel reference prices were different, and those were used since for Chaos Weapon and First Wings craftings rate calculations.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The "old" buying price.</returns>
    public long CalculateFinalOldBuyingPrice(Item item)
    {
        if (item.Definition?.PriceDefinition?.CraftingReferencePrice is { } craftingReferencePrice)
        {
            return RoundPrice(craftingReferencePrice);
        }

        return this.CalculateFinalBuyingPrice(item);
    }

    /// <summary>
    /// Calculates the buying price of the item, before it's rounded.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The buying price, before it's rounded.</returns>
    internal static long CalculateBuyingPrice(Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var priceDefinition = item.Definition.PriceDefinition;
        var dropLevel = item.Definition.DropLevel + (item.Level * 3);
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            dropLevel += ExcellentDropLevelIncrease;
        }

        var price = GetBasePrice(item, priceDefinition, dropLevel);
        price = ScaleByQuantity(item, price, priceDefinition?.QuantityScaling ?? ItemPriceQuantityScaling.None);
        price = ApplyOptionModifiers(item, price, priceDefinition?.Modifiers ?? ItemPriceModifiers.All);

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.GuardianOption))
        {
            price += price * 16 / 100;
        }

        return Math.Min(price, MaximumPrice);
    }

    private static long RoundPrice(long price)
    {
        var result = price;
        if (result >= 1000)
        {
            result = result / 100 * 100;
        }
        else if (result >= 100)
        {
            result = result / 10 * 10;
        }
        else
        {
            // no rounding for smaller values.
        }

        return result;
    }

    private static long GetBasePrice(Item item, ItemPriceDefinition? priceDefinition, int dropLevel)
    {
        if (priceDefinition is null)
        {
            return CalculateAutomaticPrice(item, dropLevel);
        }

        if (priceDefinition.PricePerLevel.FirstOrDefault(p => p.Level == item.Level) is { } levelPrice)
        {
            return levelPrice.Price;
        }

        if (string.IsNullOrWhiteSpace(priceDefinition.BasePriceFormula))
        {
            return CalculateAutomaticPrice(item, dropLevel);
        }

        var definition = item.Definition!;
        var normalOptions = item.ItemOptions.Where(o => o.ItemOption?.OptionType == ItemOptionTypes.Option).ToList();
        var firstNormalOption = normalOptions.FirstOrDefault();
        var healthRecoveryOptionLevel = firstNormalOption?.ItemOption?.PowerUpDefinition?.TargetAttribute == Stats.HealthRecoveryMultiplier
            ? firstNormalOption.Level
            : 0;
        var variables = new ItemPriceFormula.Variables(
            Level: item.Level,
            Durability: item.Durability(),
            MaxDurability: definition.Durability,
            Value: definition.Value,
            DropLevel: dropLevel,
            OptionLevel: firstNormalOption?.Level ?? 0,
            OptionCount: normalOptions.Count,
            HealthRecoveryOptionLevel: healthRecoveryOptionLevel,
            AutomaticPrice: CalculateAutomaticPrice(item, dropLevel));
        return ItemPriceFormula.Get(priceDefinition.BasePriceFormula).Evaluate(variables);
    }

    private static long CalculateAutomaticPrice(Item item, int dropLevel)
    {
        if (DropLevelIncreaseByLevel.TryGetValue(item.Level, out var dropLevelIncrease))
        {
            dropLevel += dropLevelIncrease;
        }

        if (item.IsWing())
        {
            return ((dropLevel + 40) * dropLevel * dropLevel * 11) + 40000000;
        }

        return ((dropLevel + 40) * dropLevel * dropLevel / 8) + 100;
    }

    private static long ScaleByQuantity(Item item, long price, ItemPriceQuantityScaling scaling)
    {
        return scaling switch
        {
            ItemPriceQuantityScaling.PerPiece => price * item.Durability(),
            ItemPriceQuantityScaling.ByFillRatio when item.Definition!.Durability > 0 => price * item.Durability() / item.Definition.Durability,
            _ => price,
        };
    }

    private static long ApplyOptionModifiers(Item item, long price, ItemPriceModifiers modifiers)
    {
        if (modifiers.HasFlag(ItemPriceModifiers.Skill) && item.HasSkill)
        {
            price += (long)(price * 1.5);
        }

        if (modifiers.HasFlag(ItemPriceModifiers.Luck) && item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
        {
            price += price * 25 / 100;
        }

        if (modifiers.HasFlag(ItemPriceModifiers.Option))
        {
            var optionLevel = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option)?.Level ?? 0;
            switch (optionLevel)
            {
                case 0:
                    break;
                case 1:
                    price += (long)(price * 0.6);
                    break;
                default:
                    price += (long)(price * 0.7 * Math.Pow(2, optionLevel - 1));
                    break;
            }
        }

        if (modifiers.HasFlag(ItemPriceModifiers.WingOption))
        {
            var wingOptionCount = item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Wing);
            for (int i = 0; i < wingOptionCount; i++)
            {
                price += (long)(price * 0.25);
            }
        }

        if (modifiers.HasFlag(ItemPriceModifiers.Excellent))
        {
            var excellentOptionCount = item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent);
            for (int i = 0; i < excellentOptionCount; i++)
            {
                price += price;
            }
        }

        return price;
    }
}
