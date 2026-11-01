// <copyright file="NpcItemPurchaseArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The purchase of an item from a merchant NPC, see <see cref="INpcItemPurchasePlugIn"/>.
/// </summary>
public class NpcItemPurchaseArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpcItemPurchaseArgs"/> class.
    /// </summary>
    /// <param name="storeItem">The item of the merchant store which was clicked.</param>
    public NpcItemPurchaseArgs(Item storeItem)
    {
        this.StoreItem = storeItem;
    }

    /// <summary>
    /// Gets the item of the merchant store which was clicked.
    /// </summary>
    public Item StoreItem { get; }

    /// <summary>
    /// Gets or sets the item which the player gets instead of a copy of <see cref="StoreItem"/>.
    /// It can be a temporary item; its values are copied into a new persistent item.
    /// </summary>
    public Item? ReplacementItem { get; set; }

    /// <summary>
    /// Gets or sets the price (before taxes) which is paid instead of the price of <see cref="StoreItem"/>.
    /// </summary>
    public long? Price { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the purchase is refused.
    /// The plugin which refuses it shows the reason to the player.
    /// </summary>
    public bool Cancel { get; set; }
}
