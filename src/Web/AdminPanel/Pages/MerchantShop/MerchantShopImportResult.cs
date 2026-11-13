// <copyright file="MerchantShopImportResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

/// <summary>
/// The outcome of importing a merchant shop file.
/// </summary>
/// <param name="ImportedItemCount">The number of items which were added to the shop.</param>
/// <param name="SkippedItemCount">The number of items which were skipped because their item definition is unknown.</param>
/// <param name="SkippedOptionCount">
/// The number of item options and ancient set assignments which were skipped because the item definition
/// doesn't know them.
/// </param>
public sealed record MerchantShopImportResult(int ImportedItemCount, int SkippedItemCount, int SkippedOptionCount);
