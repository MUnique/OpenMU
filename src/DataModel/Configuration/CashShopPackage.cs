// <copyright file="CashShopPackage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// A package which is offered in the cash shop.
/// </summary>
/// <remarks>
/// The client shows the packages of its package script (IBSPackage.txt). The server
/// configuration has to match it, because the client sends the sequence numbers of
/// the script when a package is bought.
/// </remarks>
[Cloneable]
public partial class CashShopPackage
{
    /// <summary>
    /// Gets or sets the package sequence number of the client script.
    /// </summary>
    public int PackageSequence { get; set; }

    /// <summary>
    /// Gets or sets the name, which is only shown in the admin panel.
    /// The client shows the name of its script.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of a bundle package.
    /// </summary>
    /// <remarks>
    /// If the package isn't a bundle, the price of the chosen <see cref="CashShopProduct"/> applies.
    /// </remarks>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the coin type with which the package is paid.
    /// </summary>
    public CashShopCoinType CoinType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the package is currently sold.
    /// </summary>
    public bool IsForSale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the package can be sent as gift to another player.
    /// </summary>
    public bool IsGiftable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the package is a bundle.
    /// </summary>
    /// <remarks>
    /// When a bundle is bought, the player gets all of its products for the <see cref="Price"/>
    /// of the package. Otherwise, the products are price options of which the player buys one.
    /// </remarks>
    public bool IsBundle { get; set; }

    /// <summary>
    /// Gets or sets the products of the package.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<CashShopProduct> Products { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.PackageSequence}: {this.Name}";
    }
}
