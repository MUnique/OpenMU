// <copyright file="ItemEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Event args containing the involved instance of an <see cref="Item"/>.
/// </summary>
/// <seealso cref="System.EventArgs" />
public class ItemEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemEventArgs"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="isEquipped">Whether equipped or not.</param>
    /// <param name="hasDurabilityFactorChanged">Whether the durability factor has changed or not.</param>
    public ItemEventArgs(Item item, bool isEquipped, bool hasDurabilityFactorChanged = false)
    {
        this.Item = item;
        this.IsEquipped = isEquipped;
        this.HasDurabilityFactorChanged = isEquipped && hasDurabilityFactorChanged;
    }

    /// <summary>
    /// Gets the item which is involved at the event.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    /// Gets a value indicating whether the item is equipped at the event.
    /// </summary>
    public bool IsEquipped { get; }

    /// <summary>
    /// Gets a value indicating whether the item's durability factor has changed at the event.
    /// </summary>
    public bool HasDurabilityFactorChanged { get; }
}