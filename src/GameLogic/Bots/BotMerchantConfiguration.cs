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
    /// Gets or sets the names of the stores, separated by commas; every shift draws one of them.
    /// </summary>
    /// <remarks>
    /// A store name is a single text which every player sees in the same language, so it can't be translated.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.BotMerchantConfiguration_StoreNames_Name), Description = nameof(PlugInResources.BotMerchantConfiguration_StoreNames_Description))]
    public string StoreNames { get; set; } = "Good stuff, Cheap items, Best prices, Exc items, Jewels here, Look inside, Fair prices, Rare finds";

    /// <summary>
    /// Gets the store names of <see cref="StoreNames"/>, without the empty ones.
    /// </summary>
    /// <returns>The store names.</returns>
    internal IReadOnlyList<string> GetStoreNames()
        => (this.StoreNames ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
