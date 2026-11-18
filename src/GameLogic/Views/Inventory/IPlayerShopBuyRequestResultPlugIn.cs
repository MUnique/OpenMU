// <copyright file="IPlayerShopBuyRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Interface of a view whose implementation informs about an item which has been bought
/// from another players shop.
/// </summary>
public interface IPlayerShopBuyRequestResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Notifies the client if an item has been bought from another players shop.
    /// </summary>
    /// <param name="seller">The seller.</param>
    /// <param name="result">The result.</param>
    /// <param name="item">The item.</param>
    ValueTask ShowResultAsync(IIdentifiable? seller, ItemBuyResult result, Item? item);
}