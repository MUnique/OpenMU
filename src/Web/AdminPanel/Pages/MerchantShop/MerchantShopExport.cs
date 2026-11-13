// <copyright file="MerchantShopExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages.MerchantShop;

/// <summary>
/// DTO for exporting and importing the shop inventory of a single merchant.
/// </summary>
public sealed record MerchantShopExport
{
    /// <summary>
    /// The current format version of the file.
    /// </summary>
    public const string CurrentFormatVersion = "1.0";

    /// <summary>
    /// Gets or sets the format version for forward compatibility.
    /// </summary>
    public string FormatVersion { get; set; } = CurrentFormatVersion;

    /// <summary>
    /// Gets or sets the exported shop items.
    /// </summary>
    public ICollection<ShopItemExport> Items { get; set; } = new List<ShopItemExport>();
}
