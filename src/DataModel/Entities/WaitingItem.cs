// <copyright file="WaitingItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// An item of a character which waits for space in the inventory.
/// The game server puts it into the inventory as soon as it fits there.
/// </summary>
public class WaitingItem
{
    /// <summary>
    /// Gets or sets the item.
    /// </summary>
    [MemberOfAggregate]
    public virtual Item? Item { get; set; }

    /// <summary>
    /// Gets or sets the reason why the item waits.
    /// </summary>
    public WaitingItemReason Reason { get; set; }

    /// <summary>
    /// Gets or sets the time when the item was added to the waiting items.
    /// </summary>
    public DateTime AddedAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Item} ({this.Reason})";
    }
}
