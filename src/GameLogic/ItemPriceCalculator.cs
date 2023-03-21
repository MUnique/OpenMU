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
/// At the moment it looks all pretty hard coded (like at the original server), so maybe in a future version we could
/// write a more generic calculator, which can be influenced by configuration instead of hard-coded item ids.
/// A nice approach would be to have a "rule" for each item definition, about how to calculate the price.
/// </remarks>
public class ItemPriceCalculator
{
    private const short ForceWaveSkillId = 66;
    private const long MaximumPrice = 3000000000;
    private const float DestroyedPetPenalty = 2.0f;
    private const float DestroyedItemPenalty = 1.4f;

    private static readonly Dictionary<byte, int> DropLevelIncreaseByLevel = new ()
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

    private static readonly IDictionary<int, Func<Item, long>> SpecialItemDictionary = new Dictionary<int, Func<Item, long>>
    {
        {
            (int)SpecialItems.Arrow, item =>
            {
                int gold = 0;
                int baseprice = 70;
                if (item.Level == 1)
                {
                    baseprice = 1200;
                }
                else if (item.Level == 2)
                {
                    baseprice = 2000;
                }

                if (item.Durability > 0)
                {
                    gold = baseprice * item.Durability() / item.Definition?.Durability ?? 1;
                }

                return gold;
            }
        },
        {
            (int)SpecialItems.Bolt, item =>
            {
                int gold = 0;
                int baseprice = 100;
                if (item.Level == 1)
                {
                    baseprice = 1400;
                }
                else if (item.Level == 2)
                {
                    baseprice = 2200;
                }

                if (item.Durability > 0)
                {
                    gold = baseprice * item.Durability() / item.Definition?.Durability ?? 1;
                }

                return gold;
            }
        },
        { (int)SpecialItems.Bless, _ => 9000000 },
        { (int)SpecialItems.Soul, _ => 6000000 },
        { (int)SpecialItems.Chaos, _ => 810000 },
        { (int)SpecialItems.Life, _ => 45000000 },
        { (int)SpecialItems.Creation, _ => 36000000 },
        { (int)SpecialItems.PackedBless, item => (item.Level + 1) * 9000000 * 10 },
        { (int)SpecialItems.PackedSoul, item => (item.Level + 1) * 6000000 * 10 },
        { (int)SpecialItems.PackedLife, item => (item.Level + 1) * 45000000 * 10 },
        { (int)SpecialItems.PackedCreation, item => (item.Level + 1) * 36000000 * 10 },
        { (int)SpecialItems.PackedGuardian, item => (item.Level + 1) * 60000000 * 10 },
        { (int)SpecialItems.PackedGemstone, item => (item.Level + 1) * 186000 * 10 },
        { (int)SpecialItems.PackedHarmony, item => (item.Level + 1) * 186000 * 10 },
        { (int)SpecialItems.PackedChaos, item => (item.Level + 1) * 810000 * 10 },
        { (int)SpecialItems.PackedLowerRefineStone, item => (item.Level + 1) * 186000 * 10 },
        { (int)SpecialItems.PackedHigherRefineStone, item => (item.Level + 1) * 186000 * 10 },
        { (int)SpecialItems.Fruits, _ => 33000000 },
        { (int)SpecialItems.LochFeather, item => item.Level == 1 ? 7500000 : 180000 },
        { (int)SpecialItems.JewelGuardian, _ => 60000000 },
        { (int)SpecialItems.SiegePotion, item => item.Durability() * (item.Level == 0 ? 900000 : 450000) },
        { (int)SpecialItems.OrderGuardianLifeStone, item => item.Level == 1 ? 2400000 : 0 },
        { (int)SpecialItems.ContractSummon, item => item.Level == 0 ? 1500000 : item.Level == 1 ? 1200000 : 0 },
        { (int)SpecialItems.SplinterOfArmor, item => item.Durability() * 150 },
        { (int)SpecialItems.BlessOfGuardian, item => item.Durability() * 300 },
        { (int)SpecialItems.ClawOfBeast, item => item.Durability() * 3000 },
        { (int)SpecialItems.FragmentOfHorn, _ => 30000 },
        { (int)SpecialItems.BrokenHorn, _ => 90000 },
        { (int)SpecialItems.HornFenrir, _ => 150000 },
        { (int)SpecialItems.SmallSdPotion, item => item.Durability() * 2000 },
        { (int)SpecialItems.SdPotion, item => item.Durability() * 4000 },
        { (int)SpecialItems.LargeSdPotion, item => item.Durability() * 6000 },
        { (int)SpecialItems.LargeHealPotion, item => item.Durability() * 1500 * (item.Level + 1) },
        { (int)SpecialItems.LargeManaPotion, item => item.Durability() * 1500 * (item.Level + 1) },
        { (int)SpecialItems.SmallComplexPotion, item => item.Durability() * 2500 },
        { (int)SpecialItems.ComplexPotion, item => item.Durability() * 5000 },
        { (int)SpecialItems.LargeComplexPotion, item => item.Durability() * 7500 },
        {
            (int)SpecialItems.Dinorant, item =>
            {
                var gold = 960000;
                var opt = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
                var optionLevel = opt?.Level ?? 0;
                gold += 300000 * optionLevel;
                return gold;
            }
        },
        { (int)SpecialItems.DevilEye, item => item.Level == 1 ? 15000 : item.Level == 2 ? 21000 : (item.Level - 1) * 15000 },
        { (int)SpecialItems.DevilKey, item => item.Level == 1 ? 15000 : item.Level == 2 ? 21000 : (item.Level - 1) * 15000 },
        { (int)SpecialItems.DevilInvitation, item => item.Level == 1 ? 60000 : item.Level == 2 ? 84000 : (item.Level - 1) * 60000 },
        { (int)SpecialItems.RedemyOfLove, _ => 900 },
        { (int)SpecialItems.Rena, item => item.Level == 3 ? item.Durability() * 3900 : 9000 },
        { (int)SpecialItems.Ale, _ => 1000 },
        {
            (int)SpecialItems.InvisibleCloak, item => item.Level == 1 ? 150000 :
                item.Level == 2 ? 660000 :
                item.Level == 3 ? 720000 :
                item.Level == 4 ? 780000 :
                item.Level == 5 ? 840000 :
                item.Level == 6 ? 900000 :
                item.Level == 7 ? 960000 :
                item.Level == 8 ? 1200000 : 60000
        },
        {
            (int)SpecialItems.BloodBone, item => item.Level == 1 ? 15000 :
                item.Level == 2 ? 21000 :
                item.Level == 3 ? 30000 :
                item.Level == 4 ? 39000 :
                item.Level == 5 ? 48000 :
                item.Level == 6 ? 60000 :
                item.Level == 7 ? 75000 :
                item.Level == 8 ? 90000 : 15000
        },
        {
            (int)SpecialItems.ScrollOfArchangel, item => item.Level == 1 ? 15000 :
                item.Level == 2 ? 21000 :
                item.Level == 3 ? 30000 :
                item.Level == 4 ? 39000 :
                item.Level == 5 ? 48000 :
                item.Level == 6 ? 60000 :
                item.Level == 7 ? 75000 :
                item.Level == 8 ? 90000 : 15000
        },
        {
            (int)SpecialItems.OldScroll, item => item.Level == 1 ? 500000 : (item.Level + 1) * 200000
        },
        {
            (int)SpecialItems.IllusionSorcererCovenant, item => item.Level == 1 ? 500000 : (item.Level + 1) * 200000
        },
        { (int)SpecialItems.ArmorGuardman, _ => 5000 },
        { (int)SpecialItems.WizardsRing, item => item.Level == 0 ? 30000 : 0 },
        { (int)SpecialItems.SpiritPet, item => item.Level == 0 ? 30000000 : item.Level == 1 ? 15000000 : 0 },
        { (int)SpecialItems.LostMap, _ => 600000 },
        { (int)SpecialItems.SymbolKundun, item => 30000 * item.Durability() },
        { (int)SpecialItems.Halloween1, item => 150 * item.Durability() },
        { (int)SpecialItems.Halloween2, item => 150 * item.Durability() },
        { (int)SpecialItems.Halloween3, item => 150 * item.Durability() },
        { (int)SpecialItems.Halloween4, item => 150 * item.Durability() },
        { (int)SpecialItems.Halloween5, item => 150 * item.Durability() },
        { (int)SpecialItems.Halloween6, item => 150 * item.Durability() },
        { (int)SpecialItems.GemOfSecret, item => item.Level == 0 ? 60000 : 0 },
    };

    private enum SpecialItems
    {
        Arrow = 0xF04, // getId(4, 15),
        Bolt = 0x704, // getId(4, 7),
        Bless = 0xD0E, // getId(14,13),
        Soul = 0xE0E, // getId(14,14),
        Chaos = 0xF0C, // getId(12,15),
        Life = 0x100E, // getId(14,16),
        Creation = 0x160E, // getId(14,22),
        PackedBless = 0x1E0C, // getId(12,30),
        PackedSoul = 0x1F0C, // getId(12,31),
        PackedLife = 0x880C,
        PackedCreation = 0x890C,
        PackedGuardian = 0x8A0C,
        PackedGemstone = 0x8B0C,
        PackedHarmony = 0x8C0C,
        PackedChaos = 0x8D0C,
        PackedLowerRefineStone = 0x8E0C,
        PackedHigherRefineStone = 0x8F0C,
        Fruits = 0xF0D, // getId(13,15),
        LochFeather = 0xE0D, // getId(13,14),
        JewelGuardian = 0x1F0E, // getId(14,31),
        LargeHealPotion = 0x030E,
        LargeManaPotion = 0x060E,
        SiegePotion = 0x70E, // getId(14,7),
        OrderGuardianLifeStone = 0xB0D, // getId(13,11),
        ContractSummon = 0x70D, // getId(13,7),
        SplinterOfArmor = 0x200D, // getId(13,32),
        BlessOfGuardian = 0x210D, // getId(13,33),
        ClawOfBeast = 0x220D, // getId(13,34),
        FragmentOfHorn = 0x230D, // getId(13,35),
        BrokenHorn = 0x240D, // getId(13,36),
        HornFenrir = 0x250D, // getId(13,37),
        SmallSdPotion = 0x230E, // getId(14,35),
        SdPotion = 0x240E, // getId(14, 36),
        LargeSdPotion = 0x250E, // getId(14,37),
        SmallComplexPotion = 0x280E, // getId(14,38),
        ComplexPotion = 0x290E, // getId(14,39),
        LargeComplexPotion = 0x2A0E, // getId(14,40),
        Dinorant = 0x30D, // getId(13,3),
        DevilEye = 0x110E, // getId(14,17),
        DevilKey = 0x120E, // getId(14,18),
        DevilInvitation = 0x130E, // getId(14,19),
        RedemyOfLove = 0x140E, // getId(14,20),
        Rena = 0x150E, // getId(14,21),
        Ale = 0x90E, // getId(14,9),
        InvisibleCloak = 0x120D, // getId(13,18),
        ScrollOfArchangel = 0x100D, // getId(13, 16),
        BloodBone = 0x110D, // getId(13,17),
        ArmorGuardman = 0x1D0D, // getId(13,29),
        WizardsRing = 0x140D, // getId(13,20),
        SpiritPet = 0x1F0D, // getId(13,31),
        LostMap = 0x1C0E, // getId(14,28),
        SymbolKundun = 0x1D0E, // getId(14,29),
        Halloween1 = 0x2D0E, // getId(14, 45),
        Halloween2 = 0x2E0E, // getId(14, 46),
        Halloween3 = 0x2F0E, // getId(14, 47),
        Halloween4 = 0x300E, // getId(14, 48),
        Halloween5 = 0x310E, // getId(14, 49),
        Halloween6 = 0x320E, // getId(14, 50),
        GemOfSecret = 0x1A0C, // getId(12,26),
        OldScroll = 0x310D,
        IllusionSorcererCovenant = 0x320D,
    }

    /// <summary>
    /// Calculates the selling price of the item, which the player gets if he is selling an item to a merchant.
    /// It's usually a third of the buying price.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The selling price.</returns>
    public long CalculateSellingPrice(Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var sellingPrice = this.CalculateBuyingPrice(item) / 3;
        if (item.Definition.Group == 14 && (item.Definition.Number <= 8))
        {
            // Potions + Antidote
            return sellingPrice / 10 * 10;
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

        const long maximumBasePrice = 400000000;
        var isPet = item.IsTrainablePet();
        var maximumDurability = item.GetMaximumDurabilityOfOnePiece();
        var basePrice = Math.Min(isPet ? CalculateBuyingPrice(item, maximumDurability) : CalculateBuyingPrice(item, maximumDurability) / 3, maximumBasePrice);
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
    /// Calculates the buying price of the item, which the player has to pay if he wants to buy the item from a merchant.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The buying price.</returns>
    public long CalculateBuyingPrice(Item item) => CalculateBuyingPrice(item, item.Durability());

    private static long CalculateBuyingPrice(Item item, byte durability)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var definition = item.Definition!;
        if (definition.Value > 0 && (definition.Group == 15 || definition.Group == 12))
        {
            return RoundPrice(definition.Value);
        }

        long price = 0;
        int dropLevel = definition.DropLevel + (item.Level * 3);
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            // increased drop level of excellent item
            dropLevel += 25;
        }

        if (SpecialItemDictionary.TryGetValue(GetId(item.Definition.Group, item.Definition.Number), out var specialItemPriceFunction))
        {
            price = specialItemPriceFunction(item);
        }
        else if (definition.Value > 0)
        {
            price += (definition.Value * definition.Value * 10) / 12;
            if (item.Definition.Group == 14 && (item.Definition.Number <= 8))
            {
                // Potions + Antidote
                if (item.Level > 0)
                {
                    price *= (long)Math.Pow(2, item.Level);
                }

                price = price / 10 * 10;
                price *= item.Durability();
                return price;
            }
        }
        else if ((item.Definition.Group == 12 && item.Definition.Number > 6) || item.Definition.Group == 13 || item.Definition.Group == 15)
        {
            price = (dropLevel * dropLevel * dropLevel) + 100;
        }
        else
        {
            if (DropLevelIncreaseByLevel.TryGetValue(item.Level, out var dropLevelIncrease))
            {
                dropLevel += dropLevelIncrease;
            }

            // Wings
            if (item.IsWing())
            {
                // maybe we have to exclude small wings here
                price = ((dropLevel + 40) * dropLevel * dropLevel * 11) + 40000000;
            }
            else
            {
                price = ((dropLevel + 40) * dropLevel * dropLevel / 8) + 100;
            }

            var isOneHandedWeapon = item.Definition.Group < 6 && definition.Width < 2 && definition.BasePowerUpAttributes.Any(o => o.TargetAttribute == Stats.MinimumPhysBaseDmg);
            var isShield = item.Definition.Group == 6;
            if (isOneHandedWeapon || isShield)
            {
                price = price * 80 / 100;
            }

            if (item.HasSkill && definition.Skill?.Number != ForceWaveSkillId)
            {
                price += (long)(price * 1.5);
            }

            // add 25% for luck
            if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
            {
                price += price * 25 / 100;
            }

            var opt = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
            var optionLevel = opt?.Level ?? 0;

            // Item Options (1 to 4, or 4 to 16)
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

            // For each wing option, add 25%
            var wingOptionCount = item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Wing);
            for (int i = 0; i < wingOptionCount; i++)
            {
                price += (long)(price * 0.25);
            }

            // For each excellent option double the value
            var excCount = item.ItemOptions.Count(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent);
            for (int i = 0; i < excCount; i++)
            {
                price += price;
            }
        }

        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.GuardianOption))
        {
            price += price * 16 / 100;
        }

        if (price > MaximumPrice)
        {
            price = MaximumPrice;
        }

        var maxDurability = item.GetMaximumDurabilityOfOnePiece();
        if (maxDurability > 1 && maxDurability > durability)
        {
            float multiplier = 1.0f - ((float)durability / maxDurability);
            long loss = (long)(price * 0.6 * multiplier);
            price -= loss;
        }

        return RoundPrice(price);
    }

    private static int GetId(byte group, int id)
    {
        return (id << 8) + group;
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
}