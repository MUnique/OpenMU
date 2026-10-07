// <copyright file="ICashShopPackageBuyingPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a player is about to buy a package of the cash shop,
/// for himself or as gift. A plugin can refuse the purchase, e.g. to apply custom restrictions.
/// </summary>
/// <remarks>
/// It's called after the standard checks and before the coins are taken, while the player's
/// persistence is locked; keep it fast.
/// </remarks>
[Guid("9A049232-237A-440F-A624-DB5198B0EA12")]
[PlugInPoint("Cash shop package buying", "Plugins which are called when a player is about to buy a package of the cash shop. They can refuse the purchase.")]
public interface ICashShopPackageBuyingPlugIn
{
    /// <summary>
    /// Is called when a player is about to buy a package of the cash shop.
    /// </summary>
    /// <param name="player">The buying player.</param>
    /// <param name="package">The package.</param>
    /// <param name="products">The products which the player gets: all products of a bundle, or the chosen price option.</param>
    /// <param name="giftRecipientName">The name of the character which receives the gift; <c>null</c>, if it's not a gift.</param>
    /// <param name="eventArgs">The event arguments. Set <see cref="CancelEventArgs.Cancel"/> to refuse the purchase.</param>
    /// <returns>A value task which completes when the plugin is done.</returns>
    ValueTask PackageBuyingAsync(Player player, CashShopPackage package, IReadOnlyCollection<CashShopProduct> products, string? giftRecipientName, CancelEventArgs eventArgs);
}
