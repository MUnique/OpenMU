// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

/// <summary>
/// Extensions for <see cref="Item"/>.
/// </summary>
public static class ItemExtensions
{
    /// <summary>
    /// Moves the item one place to the left.
    /// </summary>
    /// <param name="item">The item.</param>
    public static void MoveLeft(this Item item)
    {
        item.ItemSlot--;
    }

    /// <summary>
    /// Moves the item one place to the right.
    /// </summary>
    /// <param name="item">The item.</param>
    public static void MoveRight(this Item item)
    {
        item.ItemSlot++;
    }

    /// <summary>
    /// Moves the item one place up.
    /// </summary>
    /// <param name="item">The item.</param>
    public static void MoveUp(this Item item)
    {
        item.ItemSlot -= (byte)InventoryConstants.RowSize;
    }

    /// <summary>
    /// Moves the item one place down.
    /// </summary>
    /// <param name="item">The item.</param>
    public static void MoveDown(this Item item)
    {
        item.ItemSlot += (byte)InventoryConstants.RowSize;
    }
}