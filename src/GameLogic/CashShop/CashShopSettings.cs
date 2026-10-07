// <copyright file="CashShopSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// The settings of the cash shop, which are configured at the <see cref="CashShopFeaturePlugIn"/>.
/// </summary>
/// <remarks>
/// The catalog isn't part of it, but of the <see cref="GameConfiguration.CashShopConfiguration"/>,
/// because it refers to item definitions.
/// </remarks>
public class CashShopSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether players can send packages as gift.
    /// </summary>
    public bool IsGiftingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether players can send gifts to the characters of their own account.
    /// </summary>
    public bool CanGiftToOwnAccount { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of items in the cash shop storage of an account, including the gifts.
    /// 0 means unlimited.
    /// </summary>
    public int MaximumStorageItems { get; set; }
}
