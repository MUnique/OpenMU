// <copyright file="PlayerShopCurrenciesConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerShopCurrencies;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The configuration of the <see cref="PlayerShopCurrenciesFeaturePlugIn"/>.
/// </summary>
/// <remarks>
/// Zen is always allowed. The account coins have one flag each, because the admin panel has no editor for
/// collections of enum values.
/// </remarks>
public class PlayerShopCurrenciesConfiguration
{
    /// <summary>
    /// The default of <see cref="MaximumItemAmount"/>.
    /// </summary>
    internal const int DefaultMaximumItemAmount = 1000;

    /// <summary>
    /// The default of <see cref="MaximumCoinAmount"/>.
    /// </summary>
    internal const int DefaultMaximumCoinAmount = 1_000_000;

    /// <summary>
    /// Gets or sets the items with which prices can be paid, e.g. jewels.
    /// </summary>
    /// <remarks>
    /// A new database gets the Jewels of Bless, Soul, Chaos, Creation and Life. The default configuration which
    /// is created for an existing database can't reference items, so they have to be added when activating the plugin.
    /// </remarks>
    public ICollection<ItemDefinition> PriceItems { get; set; } = new List<ItemDefinition>();

    /// <summary>
    /// Gets or sets a value indicating whether prices in WCoin (C) are allowed.
    /// </summary>
    /// <remarks>
    /// The account coins are usually bought with real money, so trading them has to be allowed explicitly.
    /// </remarks>
    public bool IsWCoinCAllowed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether prices in WCoin (P) are allowed.
    /// </summary>
    /// <remarks>
    /// The account coins are usually bought with real money, so trading them has to be allowed explicitly.
    /// </remarks>
    public bool IsWCoinPAllowed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether prices in Goblin Points are allowed.
    /// </summary>
    public bool IsGoblinPointsAllowed { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of item pieces of a price.
    /// </summary>
    /// <remarks>
    /// The extended client protocol transfers the amount in a 16-bit field, so values above 65535 are treated as 65535.
    /// A value of 0 or less is treated as the default value.
    /// </remarks>
    public int MaximumItemAmount { get; set; } = DefaultMaximumItemAmount;

    /// <summary>
    /// Gets or sets the maximum amount of account coins of a price.
    /// </summary>
    /// <remarks>
    /// A value of 0 or less is treated as the default value.
    /// </remarks>
    public int MaximumCoinAmount { get; set; } = DefaultMaximumCoinAmount;
}
