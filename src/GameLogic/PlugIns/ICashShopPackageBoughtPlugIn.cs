// <copyright file="ICashShopPackageBoughtPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called after a player bought a package of the cash shop,
/// for himself or as gift, e.g. to grant a bonus or to notify other systems.
/// </summary>
/// <remarks>
/// It's called after the purchase is saved. The packet handler still holds the player's persistence lock,
/// so don't save other players here (see the lock order in <c>PlayerPersistence</c>).
/// </remarks>
[Guid("F6F3ABA9-30BC-488A-A65B-8F5E4AB0BA04")]
[PlugInPoint("Cash shop package bought", "Plugins which are called after a player bought a package of the cash shop.")]
public interface ICashShopPackageBoughtPlugIn
{
    /// <summary>
    /// Is called after a player bought a package of the cash shop.
    /// </summary>
    /// <param name="player">The buying player.</param>
    /// <param name="package">The package.</param>
    /// <param name="products">The products which were added to the storage.</param>
    /// <param name="price">The price which was paid, in coins of the coin type of the package.</param>
    /// <param name="giftRecipientName">The name of the character which received the gift; <c>null</c>, if it's not a gift.</param>
    /// <returns>A value task which completes when the plugin is done.</returns>
    ValueTask PackageBoughtAsync(Player player, CashShopPackage package, IReadOnlyCollection<CashShopProduct> products, int price, string? giftRecipientName);
}
