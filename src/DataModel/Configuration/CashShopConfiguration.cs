// <copyright file="CashShopConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// The configuration of the cash shop (in-game shop).
/// </summary>
/// <remarks>
/// The client holds its own copy of the product catalog as script files, which it loads by
/// the script version this server sends. The <see cref="Packages"/> have to match the
/// script of this version.
/// </remarks>
[Cloneable]
public partial class CashShopConfiguration
{
    /// <summary>
    /// Gets or sets the sale zone of the script version.
    /// </summary>
    /// <remarks>
    /// The client loads the script from the folder 'Data\InGameShopScript\[SaleZone].[Year].[YearId]'.
    /// </remarks>
    public short ScriptSaleZone { get; set; }

    /// <summary>
    /// Gets or sets the year of the script version.
    /// </summary>
    public short ScriptYear { get; set; }

    /// <summary>
    /// Gets or sets the identifier within the year of the script version.
    /// </summary>
    public short ScriptYearId { get; set; }

    /// <summary>
    /// Gets or sets the sale zone of the banner version.
    /// </summary>
    /// <remarks>
    /// The client loads the banner from the folder 'Data\InGameShopBanner\[SaleZone].[Year].[YearId]'.
    /// </remarks>
    public short BannerSaleZone { get; set; }

    /// <summary>
    /// Gets or sets the year of the banner version.
    /// </summary>
    public short BannerYear { get; set; }

    /// <summary>
    /// Gets or sets the identifier within the year of the banner version.
    /// </summary>
    public short BannerYearId { get; set; }

    /// <summary>
    /// Gets or sets the packages which are offered in the cash shop.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CashShopPackage> Packages { get; protected set; } = null!;
}
