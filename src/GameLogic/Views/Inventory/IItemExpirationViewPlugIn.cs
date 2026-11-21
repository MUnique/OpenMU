// <copyright file="IItemExpirationViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Interface of a view whose implementation informs the client about the expiration of time-limited items
/// (see <see cref="Item.ExpiresAt"/>), so it can show the time left.
/// </summary>
public interface IItemExpirationViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the expirations of the time-limited items of the inventory, after the inventory list was shown.
    /// </summary>
    /// <param name="items">The items of the inventory.</param>
    ValueTask ShowExpirationsAsync(IEnumerable<Item> items);

    /// <summary>
    /// Shows the expiration of a time-limited item, after the item was shown in the inventory.
    /// Nothing is shown for an item which isn't time-limited.
    /// </summary>
    /// <param name="item">The item.</param>
    ValueTask ShowExpirationAsync(Item item);
}
