// <copyright file="ICashShopViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CashShop;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The view plugin for the cash shop of the own player.
/// </summary>
public interface ICashShopViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the versions of the cash shop script and banner, which the client should load.
    /// </summary>
    /// <param name="configuration">The cash shop configuration.</param>
    ValueTask ShowVersionsAsync(CashShopConfiguration configuration);

    /// <summary>
    /// Shows the result of a request to open the cash shop.
    /// </summary>
    /// <param name="isAllowed">If set to <c>true</c>, the cash shop is opened.</param>
    ValueTask ShowOpenResultAsync(bool isAllowed);

    /// <summary>
    /// Shows the available cash shop points of the account.
    /// </summary>
    /// <param name="account">The account.</param>
    ValueTask ShowPointsAsync(Account account);

    /// <summary>
    /// Shows the result of a request to buy a package.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowBuyResultAsync(CashShopBuyResult result);

    /// <summary>
    /// Shows the result of a request to send a package as gift.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowGiftResultAsync(CashShopGiftResult result);

    /// <summary>
    /// Shows the result of a request to use an item of the storage.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowUseResultAsync(CashShopUseResult result);

    /// <summary>
    /// Shows a page of the cash shop storage or gift storage.
    /// </summary>
    /// <remarks>
    /// The index of an item in <paramref name="items"/> is its storage index, by which the player
    /// requests to use it with <see cref="PlayerActions.CashShop.CashShopActions.UseStorageItemAsync"/>.
    /// </remarks>
    /// <param name="items">All items of the storage of the account, including the gifts, ordered by the time they were added.</param>
    /// <param name="pageNumber">The one-based number of the requested page.</param>
    /// <param name="isGiftStorage">If set to <c>true</c>, the page of the gift storage is shown.</param>
    ValueTask ShowStorageAsync(IReadOnlyList<CashShopStorageItem> items, int pageNumber, bool isGiftStorage);
}
