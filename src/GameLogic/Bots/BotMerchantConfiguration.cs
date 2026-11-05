// <copyright file="BotMerchantConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

/// <summary>
/// Configuration of the <see cref="BotMerchantPlugIn"/>: how many bots sell their goods on the market
/// map at the same time, and for how long.
/// </summary>
/// <remarks>
/// A bot goes to the market when it collected enough goods in its personal store, stays there for a
/// shift of random length, and goes back hunting afterward. The limits apply per game server.
/// </remarks>
public class BotMerchantConfiguration
{
    /// <summary>
    /// The default of <see cref="MarketMapNumber"/>: Loren Market, the map which is made for player stores.
    /// </summary>
    internal const short DefaultMarketMapNumber = 79;

    /// <summary>
    /// Gets or sets the number of the map on which the bots open their stores.
    /// </summary>
    /// <remarks>
    /// The bot gets there through the map's spawn gate directly, regardless of the warp list - its level
    /// requirement and costs are for the players.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MarketMapNumber_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MarketMapNumber_Description))]
    public short MarketMapNumber { get; set; } = DefaultMarketMapNumber;

    /// <summary>
    /// Gets or sets the maximum number of bots with an open store on the market at the same time, per game server.
    /// </summary>
    [Range(0, 200)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MaximumMerchants_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MaximumMerchants_Description))]
    public int MaximumMerchants { get; set; } = 15;

    /// <summary>
    /// Gets or sets the minimum level of a bot to go to the market.
    /// </summary>
    /// <remarks>
    /// The game allows opening a personal store from level 6, so lower values have the same effect.
    /// </remarks>
    [Range(0, 1000)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MinimumLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MinimumLevel_Description))]
    public int MinimumLevel { get; set; } = 6;

    /// <summary>
    /// Gets or sets the minimum number of priced goods in the personal store of a bot to go to the market.
    /// </summary>
    [Range(1, 32)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MinimumGoodsToStart_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MinimumGoodsToStart_Description))]
    public int MinimumGoodsToStart { get; set; } = 4;

    /// <summary>
    /// Gets or sets the number of goods below which a bot closes its store and leaves the market early.
    /// By default, it stays as long as it has anything to sell.
    /// </summary>
    [Range(1, 32)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MinimumGoodsToStay_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MinimumGoodsToStay_Description))]
    public int MinimumGoodsToStay { get; set; } = 1;

    /// <summary>
    /// Gets or sets the shortest shift on the market. The actual length is random between this and
    /// <see cref="MaximumShiftDuration"/>, so the merchants don't all leave at once.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MinimumShiftDuration_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MinimumShiftDuration_Description))]
    public TimeSpan MinimumShiftDuration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the longest shift on the market.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MaximumShiftDuration_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MaximumShiftDuration_Description))]
    public TimeSpan MaximumShiftDuration { get; set; } = TimeSpan.FromMinutes(90);

    /// <summary>
    /// Gets or sets after how long without a sale a bot leaves the market before its shift is over,
    /// making room for another one; zero switches it off.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_LeaveWithoutSalesAfter_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_LeaveWithoutSalesAfter_Description))]
    public TimeSpan LeaveWithoutSalesAfter { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the time after a shift before the same bot goes to the market again, so that the
    /// market sees different bots.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_TimeBetweenShifts_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_TimeBetweenShifts_Description))]
    public TimeSpan TimeBetweenShifts { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Gets or sets after how many market shifts without a buyer a good leaves the store and is sold to
    /// an NPC, making room for fresh goods.
    /// </summary>
    /// <remarks>
    /// There are no discounts: players who know that the price drops would never buy at the first one.
    /// The count is kept while the game server runs.
    /// </remarks>
    [Range(1, 100)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_UnsoldShiftsUntilNpc_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_UnsoldShiftsUntilNpc_Description))]
    public int UnsoldShiftsUntilNpc { get; set; } = 3;

    /// <summary>
    /// Gets or sets the names of the stores, separated by commas; every shift draws one of them.
    /// </summary>
    /// <remarks>
    /// A store name is a single text which every player sees in the same language, so it can't be translated.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_StoreNames_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_StoreNames_Description))]
    public string StoreNames { get; set; } = "Good stuff, Cheap items, Best prices, Exc items, Jewels here, Look inside, Fair prices, Rare finds";

    /// <summary>
    /// Gets or sets the drop level from which an item counts as one of the higher sets: only such normal items are worth a stall, and above it ancient and the best excellent items get the surcharge of <see cref="HighSetSurcharge"/>.
    /// </summary>
    [Range(0, 400)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_HighSetMinimumDropLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_HighSetMinimumDropLevel_Description))]
    public int HighSetMinimumDropLevel { get; set; } = 60;

    /// <summary>
    /// Gets or sets the minimum level (+) of a normal item of a higher set with luck to be stocked.
    /// </summary>
    [Range(0, 15)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MinimumNormalItemLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MinimumNormalItemLevel_Description))]
    public int MinimumNormalItemLevel { get; set; } = 7;

    /// <summary>
    /// Gets or sets how far, in percent up or down, a zen price may differ from the calculated one. The price is
    /// drawn once, when the good goes into the store, and rounded to an amount a player would ask for - so the
    /// same good costs a bit differently at two merchants. Prices in jewels stay exact; 0 keeps all prices exact.
    /// </summary>
    [Range(0, 50)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_PriceSpreadPercent_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_PriceSpreadPercent_Description))]
    public int PriceSpreadPercent { get; set; } = 10;

    /// <summary>
    /// Gets or sets the currency of the cheap goods: normal items, and excellent items with only a cheap option. "zen" or the name of a jewel ("bless", "soul", "chaos", "life", "creation").
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CheapCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CheapCurrency_Description))]
    public string CheapCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the base price of the cheap goods.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CheapPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CheapPrice_Description))]
    public int CheapPrice { get; set; } = 2_000_000;

    /// <summary>
    /// Gets or sets the surcharge of the cheap goods per item level above +7.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CheapPricePerLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CheapPricePerLevel_Description))]
    public int CheapPricePerLevel { get; set; } = 1_000_000;

    /// <summary>
    /// Gets or sets the surcharge of the cheap goods per +4 of the additional option; the sum is rounded down.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CheapPricePerOption_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CheapPricePerOption_Description))]
    public double CheapPricePerOption { get; set; } = 500_000;

    /// <summary>
    /// Gets or sets the currency of the medium goods: excellent items with one wanted option, without luck.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MediumCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MediumCurrency_Description))]
    public string MediumCurrency { get; set; } = "bless";

    /// <summary>
    /// Gets or sets the base price of the medium goods.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MediumPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MediumPrice_Description))]
    public int MediumPrice { get; set; } = 2;

    /// <summary>
    /// Gets or sets the surcharge of the medium goods per item level above +7.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MediumPricePerLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MediumPricePerLevel_Description))]
    public int MediumPricePerLevel { get; set; } = 1;

    /// <summary>
    /// Gets or sets the surcharge of the medium goods per +4 of the additional option; the sum is rounded down.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MediumPricePerOption_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MediumPricePerOption_Description))]
    public double MediumPricePerOption { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the currency of the expensive goods: ancient items, excellent items with two or more options, and with one wanted option and luck.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveCurrency_Description))]
    public string ExpensiveCurrency { get; set; } = "soul";

    /// <summary>
    /// Gets or sets the base price of an excellent item with one wanted option and luck.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveOneOptionPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveOneOptionPrice_Description))]
    public int ExpensiveOneOptionPrice { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base price of an excellent item with two options and of an ancient item.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePrice_Description))]
    public int ExpensivePrice { get; set; } = 5;

    /// <summary>
    /// Gets or sets the base price of an excellent item with three or more options.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveThreeOptionsPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensiveThreeOptionsPrice_Description))]
    public int ExpensiveThreeOptionsPrice { get; set; } = 10;

    /// <summary>
    /// Gets or sets the surcharge of the expensive goods per item level above +7.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePricePerLevel_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePricePerLevel_Description))]
    public int ExpensivePricePerLevel { get; set; } = 1;

    /// <summary>
    /// Gets or sets the surcharge of the expensive goods per +4 of the additional option; the sum is rounded down.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePricePerOption_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ExpensivePricePerOption_Description))]
    public double ExpensivePricePerOption { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the surcharge of an ancient item with luck.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_AncientLuckSurcharge_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_AncientLuckSurcharge_Description))]
    public int AncientLuckSurcharge { get; set; } = 1;

    /// <summary>
    /// Gets or sets the surcharge of an ancient item, and of an excellent item with a wanted option, luck and an additional option, per <see cref="HighSetSurchargeDropLevels"/> drop levels above <see cref="HighSetMinimumDropLevel"/>.
    /// </summary>
    [Range(0, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_HighSetSurcharge_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_HighSetSurcharge_Description))]
    public int HighSetSurcharge { get; set; } = 1;

    /// <summary>
    /// Gets or sets how many drop levels make one step of <see cref="HighSetSurcharge"/>.
    /// </summary>
    [Range(1, 400)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_HighSetSurchargeDropLevels_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_HighSetSurchargeDropLevels_Description))]
    public int HighSetSurchargeDropLevels { get; set; } = 20;

    /// <summary>
    /// Gets or sets how many jewels of one kind a bot puts into its store at most, so that its stall isn't full of jewels; 0 keeps the jewels out of the stores.
    /// </summary>
    [Range(0, 32)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_MaximumJewelsPerKind_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_MaximumJewelsPerKind_Description))]
    public int MaximumJewelsPerKind { get; set; } = 5;

    /// <summary>
    /// Gets or sets the currency for which a bot sells its surplus of the Jewel of Bless: "zen" or another jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_BlessCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_BlessCurrency_Description))]
    public string BlessCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the price of one Jewel of Bless.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_BlessPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_BlessPrice_Description))]
    public int BlessPrice { get; set; } = 9_000_000;

    /// <summary>
    /// Gets or sets the currency for which a bot sells its surplus of the Jewel of Soul: "zen" or another jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_SoulCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_SoulCurrency_Description))]
    public string SoulCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the price of one Jewel of Soul.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_SoulPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_SoulPrice_Description))]
    public int SoulPrice { get; set; } = 6_000_000;

    /// <summary>
    /// Gets or sets the currency for which a bot sells its surplus of the Jewel of Chaos: "zen" or another jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ChaosCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ChaosCurrency_Description))]
    public string ChaosCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the price of one Jewel of Chaos.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_ChaosPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_ChaosPrice_Description))]
    public int ChaosPrice { get; set; } = 810_000;

    /// <summary>
    /// Gets or sets the currency for which a bot sells its surplus of the Jewel of Life: "zen" or another jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_LifeCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_LifeCurrency_Description))]
    public string LifeCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the price of one Jewel of Life.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_LifePrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_LifePrice_Description))]
    public int LifePrice { get; set; } = 45_000_000;

    /// <summary>
    /// Gets or sets the currency for which a bot sells its surplus of the Jewel of Creation: "zen" or another jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CreationCurrency_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CreationCurrency_Description))]
    public string CreationCurrency { get; set; } = "zen";

    /// <summary>
    /// Gets or sets the price of one Jewel of Creation.
    /// </summary>
    [Range(1, int.MaxValue)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_CreationPrice_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_CreationPrice_Description))]
    public int CreationPrice { get; set; } = 36_000_000;

    /// <summary>
    /// Gets the store names of <see cref="StoreNames"/>, without the empty ones.
    /// </summary>
    /// <returns>The store names.</returns>
    internal IReadOnlyList<string> GetStoreNames()
        => (this.StoreNames ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
