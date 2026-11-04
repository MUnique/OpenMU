// <copyright file="InventoryExtensionDeliveryConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CashShop;

/// <summary>
/// The configuration of the <see cref="InventoryExtensionDeliveryPlugIn"/>.
/// </summary>
public class InventoryExtensionDeliveryConfiguration
{
    /// <summary>
    /// Gets or sets the maximum number of inventory extensions which a character can get from the cash shop.
    /// It can't exceed the number which the client supports, <see cref="InventoryConstants.MaximumNumberOfExtensions"/>.
    /// </summary>
    public int MaximumExtensions { get; set; } = InventoryConstants.MaximumNumberOfExtensions;
}
