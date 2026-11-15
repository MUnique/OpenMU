// <copyright file="MerchantShopImportResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

/// <summary>
/// The outcome of importing a merchant shop file.
/// </summary>
/// <param name="ImportedItemCount">The number of items which were added to the shop.</param>
/// <param name="Problems">
/// The entries of the file which can't be imported, e.g. because the item definition, an item option or an
/// ancient set is unknown. The import is all-or-nothing: if there is any problem, nothing was imported
/// and the shop is unchanged.
/// </param>
public sealed record MerchantShopImportResult(int ImportedItemCount, IReadOnlyList<string> Problems);
