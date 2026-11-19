// <copyright file="IShowMerchantStoreItemListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Interface of a view whose implementation informs about the available items in an opened npc merchant dialog.
/// </summary>
public interface IShowMerchantStoreItemListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the merchant store item list.
    /// </summary>
    /// <param name="storeItems">The store items.</param>
    /// <param name="storeKind">Kind of the store.</param>
    ValueTask ShowMerchantStoreItemListAsync(ICollection<Item> storeItems, StoreKind storeKind);
}