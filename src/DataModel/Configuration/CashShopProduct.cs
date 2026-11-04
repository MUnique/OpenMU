// <copyright file="CashShopProduct.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// A product of a <see cref="CashShopPackage"/>, which is one price option of the
/// package, or one component of a bundle package.
/// </summary>
/// <remarks>
/// The sequence numbers must match the product script (IBSProduct.txt) of the client,
/// which looks up name, quantity and period of an item in the cash shop storage by them.
/// </remarks>
[Cloneable]
public partial class CashShopProduct
{
    /// <summary>
    /// Gets or sets the product sequence number of the client script.
    /// </summary>
    public int ProductSequence { get; set; }

    /// <summary>
    /// Gets or sets the price sequence number of the client script.
    /// </summary>
    /// <remarks>
    /// It identifies the product in the client script. The same product can be part of
    /// several packages, e.g. of bundles, so it's only unique within a package.
    /// </remarks>
    public int PriceSequence { get; set; }

    /// <summary>
    /// Gets or sets the price, if the product is bought as price option of its package.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Gets or sets the definition of the item which is created when the product is used.
    /// </summary>
    public virtual ItemDefinition? ItemDefinition { get; set; }

    /// <summary>
    /// Gets or sets the level of the item which is created when the product is used.
    /// </summary>
    public byte ItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the number of items, or the durability of a stackable item.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the duration for which the item is usable.
    /// </summary>
    /// <remarks>
    /// <see cref="TimeSpan.Zero"/> means, that the item isn't limited.
    /// </remarks>
    public TimeSpan Duration { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.PriceSequence}: {this.ItemDefinition?.ToString() ?? this.ProductSequence.ToString()}";
    }
}
