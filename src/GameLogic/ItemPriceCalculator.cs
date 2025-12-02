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
    private const short ExplosionSkillId = 223;
    private const short RequiemSkillId = 224;
    private const short PollutionSkillId = 225;
    private const long MaximumPrice = 3_000_000_000;
    private const float DestroyedPetPenalty = 2.0f;
    private const float DestroyedItemPenalty = 1.4f;

    private static readonly List<short> WorthlessSkills = [ForceWaveSkillId, ExplosionSkillId, RequiemSkillId, PollutionSkillId];

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

    private static readonly IDictionary<int, long> SpecialItemOldValueDictionary = new Dictionary<int, long>
    {
        { (int)SpecialItems.Bless, 100_000 },
        { (int)SpecialItems.Soul, 70_000 },
        { (int)SpecialItems.Chaos, 40_000 },
        { (int)SpecialItems.Life, 450_000 },
        { (int)SpecialItems.Creation, 450_000 },
    };

    private static readonly IDictionary<int, Func<Item, long>> SpecialItemDictionary = new Dictionary<int, Func<Item, long>>
    {
        {
            (int)SpecialItems.Arrow, item =>
            {
                int gold = 0;
                int baseprice = item.Level switch
                {
                    1 => 1200,
                    2 => 2000,
                    3 => 2800,
                    _ => 70,
                };

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
                int baseprice = item.Level switch
                {
                    1 => 1400,
                    2 => 2200,
                    3 => 3000,
                    _ => 100,
                };

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
        { (int)SpecialItems.Guardian, _ => 60000000 },
        { (int)SpecialItems.Gemstone, _ => 18600 },
        { (int)SpecialItems.Harmony, _ => 18600 },
        { (int)SpecialItems.LowerRefineStone, _ => 18600 },
        { (int)SpecialItems.HigherRefineStone, _ => 18600 },
        { (int)SpecialItems.PackedBless, item => (item.Level + 1) * 9000000 * 10 },
        { (int)SpecialItems.PackedSoul, item => (item.Level + 1) * 6000000 * 10 },
        { (int)SpecialItems.PackedChaos, item => (item.Level + 1) * 810000 * 10 },
        { (int)SpecialItems.PackedLife, item => (item.Level + 1) * 45000000 * 10 },
        { (int)SpecialItems.PackedCreation, item => (item.Level + 1) * 36000000 * 10 },
        { (int)SpecialItems.PackedGuardian, item => (item.Level + 1) * 60000000 * 10 },
        { (int)SpecialItems.PackedGemstone, item => (item.Level + 1) * 18600 * 10 },
        { (int)SpecialItems.PackedHarmony, item => (item.Level + 1) * 18600 * 10 },
        { (int)SpecialItems.PackedLowerRefineStone, item => (item.Level + 1) * 18600 * 10 },
        { (int)SpecialItems.PackedHigherRefineStone, item => (item.Level + 1) * 18600 * 10 },
        { (int)SpecialItems.Fruits, _ => 33000000 },
        { (int)SpecialItems.LochFeather, item => item.Level == 1 ? 7500000 : 180000 },
        { (int)SpecialItems.SiegePotion, item => item.Durability() * (item.Level == 0 ? 900000 : 450000) },
        { (int)SpecialItems.OrderGuardianLifeStone, item => item.Level == 1 ? 2400000 : 1000000 },
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
                var opts = item.ItemOptions.Where(o => o.ItemOption?.OptionType == ItemOptionTypes.Option).Count();
                return 960000 + (300000 * opts);
            }
        },
        {
            (int)SpecialItems.DevilEye, item => item.Level == 1 ? 10000 :
                item.Level == 2 ? 50000 :
                item.Level == 3 ? 100000 :
                item.Level == 4 ? 300000 :
                item.Level == 5 ? 500000 :
                item.Level == 6 ? 800000 :
                item.Level == 7 ? 1000000 : 10000
        },
        {
            (int)SpecialItems.DevilKey, item => item.Level == 1 ? 15000 :
                item.Level == 2 ? 75000 :
                item.Level == 3 ? 150000 :
                item.Level == 4 ? 450000 :
                item.Level == 5 ? 750000 :
                item.Level == 6 ? 1200000 :
                item.Level == 7 ? 1500000 : 15000
        },
        { (int)SpecialItems.DevilInvitation, item => item.Level is 1 ? 60000 : item.Level == 2 ? 84000 : (item.Level - 1) * 60000 },    // +7 sell price on S6E3 client is 60k (same as +4). Bug?
        { (int)SpecialItems.RemedyOfLove, _ => 900 },
        { (int)SpecialItems.Rena, item => item.Level == 3 ? item.Durability() * 3900 : 9000 },
        { (int)SpecialItems.Ale, _ => 750 },
        { (int)SpecialItems.InvisibleCloak, item => item.Level == 1 ? 150000 : 600000 + ((item.Level - 1) * 60000) },
        {
            (int)SpecialItems.ScrollOfArchangel, item => item.Level == 1 ? 10000 :
                item.Level == 2 ? 50000 :
                item.Level == 3 ? 100000 :
                item.Level == 4 ? 300000 :
                item.Level == 5 ? 500000 :
                item.Level == 6 ? 800000 :
                item.Level == 7 ? 1000000 :
                item.Level == 8 ? 1200000 : 10000
        },
        {
            (int)SpecialItems.BloodBone, item => item.Level == 1 ? 10000 :
                item.Level == 2 ? 50000 :
                item.Level == 3 ? 100000 :
                item.Level == 4 ? 300000 :
                item.Level == 5 ? 500000 :
                item.Level == 6 ? 800000 :
                item.Level == 7 ? 1000000 :
                item.Level == 8 ? 1200000 : 10000
        },
        { (int)SpecialItems.OldScroll, item => item.Level == 1 ? 500000 : (item.Level + 1) * 200000 },
        { (int)SpecialItems.IllusionSorcererCovenant, item => item.Level == 1 ? 500000 : (item.Level + 1) * 200000 },
        { (int)SpecialItems.ScrollOfBlood, item => item.Level == 1 ? 500000 : (item.Level + 1) * 200000 },
        { (int)SpecialItems.FlameOfCondor, _ => 3000000 },
        { (int)SpecialItems.FeatherOfCondor, _ => 3000000 },
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
        { (int)SpecialItems.SuspiciousScrapOfPaper, item => 30000 * item.Durability() },
        { (int)SpecialItems.GaionsOrder, item => 30000 * item.Durability() },
        { (int)SpecialItems.FirstSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.SecondSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.ThirdSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.FourthSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.FifthSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.SixthSecromiconFragment, item => 30000 * item.Durability() },
        { (int)SpecialItems.CompleteSecromicon, item => 30000 * item.Durability() },
        { (int)SpecialItems.ChristmasStar, _ => 200000 },
        { (int)SpecialItems.Firecracker, _ => 200000 },
        { (int)SpecialItems.CherryBlossomWine, item => 300 * item.Durability() },
        { (int)SpecialItems.CherryBlossomRiceCake, item => 300 * item.Durability() },
        { (int)SpecialItems.CherryBlossomFlowerPetal, item => 300 * item.Durability() },
        { (int)SpecialItems.GoldenCherryBlossomBranch, item => 300 * item.Durability() },
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
        Guardian = 0x1F0E, // getId(14,31),
        Gemstone = 0x290E,
        Harmony = 0x2A0E,
        LowerRefineStone = 0x2B0E,
        HigherRefineStone = 0x2C0E,
        PackedBless = 0x1E0C, // getId(12,30),
        PackedSoul = 0x1F0C, // getId(12,31),
        PackedChaos = 0x8D0C,
        PackedLife = 0x880C,
        PackedCreation = 0x890C,
        PackedGuardian = 0x8A0C,
        PackedGemstone = 0x8B0C,
        PackedHarmony = 0x8C0C,
        PackedLowerRefineStone = 0x8E0C,
        PackedHigherRefineStone = 0x8F0C,
        Fruits = 0xF0D, // getId(13,15),
        LochFeather = 0xE0D, // getId(13,14),
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
        SmallComplexPotion = 0x260E, // getId(14,38),
        ComplexPotion = 0x270E, // getId(14,39),
        LargeComplexPotion = 0x280E, // getId(14,40),
        Dinorant = 0x30D, // getId(13,3),
        DevilEye = 0x110E, // getId(14,17),
        DevilKey = 0x120E, // getId(14,18),
        DevilInvitation = 0x130E, // getId(14,19),
        RemedyOfLove = 0x140E, // getId(14,20),
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
        ScrollOfBlood = 0x330D,
        FlameOfCondor = 0x340D,
        FeatherOfCondor = 0x350D,
        SuspiciousScrapOfPaper = 0x650E,
        GaionsOrder = 0x660E,
        FirstSecromiconFragment = 0x670E,
        SecondSecromiconFragment = 0x680E,
        ThirdSecromiconFragment = 0x690E,
        FourthSecromiconFragment = 0x6A0E,
        FifthSecromiconFragment = 0x6B0E,
        SixthSecromiconFragment = 0x6C0E,
        CompleteSecromicon = 0x6D0E,
        ChristmasStar = 0x330E,
        Firecracker = 0x3F0E,
        CherryBlossomWine = 0x550E,
        CherryBlossomRiceCake = 0x560E,
        CherryBlossomFlowerPetal = 0x570E,
        GoldenCherryBlossomBranch = 0x5A0E,
    }

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
        if (item.Definition.Group == 14 && item.Definition.Number <= 8)
        {
            // Potions + Antidote
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
        if (SpecialItemOldValueDictionary.TryGetValue(GetId(item.Definition!.Group, item.Definition.Number), out var oldValue))
        {
            return RoundPrice(oldValue);
        }
        else
        {
            return this.CalculateFinalBuyingPrice(item);
        }
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

    private static long CalculateBuyingPrice(Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        var definition = item.Definition!;
        if (definition.Value > 0 && (definition.Group == 15 || definition.Group == 12))
        {
            return definition.Value;
        }

        if (item.IsTrainablePet())
        {
            if (item.IsDarkRaven())
            {
                return item.Level * 1_000_000;
            }
            else
            {
                return item.Level * 2_000_000;
            }
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
            price += definition.Value * definition.Value * 10 / 12;
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
        else if ((item.Definition.Group == 12
                && ((item.Definition.Number > 6 && item.Definition.Number < 36)
                    || (item.Definition.Number > 43 && item.Definition.Number != 50)))
            || item.Definition.Group == 13
            || item.Definition.Group == 15)
        {
            // Cape of Lord and Cape of Fighter go here
            price = (dropLevel * dropLevel * dropLevel) + 100;

            if (item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option) is { } opt
                && opt.ItemOption?.PowerUpDefinition?.TargetAttribute == Stats.HealthRecoveryMultiplier)
            {
                // Rings, pendants, and capes. Capes with physical damage option have no extra value (possibly a source bug).
                price += price * opt.Level;
            }
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

            var isOneHandedWeapon = item.Definition.Group < 6 && definition.Width < 2;
            var isShield = item.Definition.Group == 6;
            if (isOneHandedWeapon || isShield)
            {
                price = price * 80 / 100;
            }

            if (item.HasSkill && !WorthlessSkills.Contains(definition.Skill?.Number ?? 0))
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

        return price;
    }
}