// <copyright file="ItemPickFailReason.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// The item pick fail reason for the <see cref="IItemPickUpFailedPlugIn"/>.
/// </summary>
public enum ItemPickFailReason
{
    /// <summary>
    /// The undefined enum value. Should never be passed to <see cref="IItemPickUpFailedPlugIn"/>.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The general, non-specific reason. It just failed.
    /// </summary>
    General,

    /// <summary>
    /// The picked up item was combined into an existing item of the players inventory.
    /// A separate durability update will be sent to the client.
    /// </summary>
    ItemStacked,

    /// <summary>
    /// The maximum inventory money has been reached, so the money wasn't picked up.
    /// </summary>
    /// <remarks>
    /// Unused, because we never drop money and therefore can't pick it up.
    /// </remarks>
    MaximumInventoryMoneyReached,
}