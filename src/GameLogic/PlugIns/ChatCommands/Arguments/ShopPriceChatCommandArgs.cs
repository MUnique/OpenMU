// <copyright file="ShopPriceChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="ShopPriceChatCommandPlugIn"/>.
/// </summary>
public class ShopPriceChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the position of the item in the store (1 to 32), or its inventory slot.
    /// When it's 0, the prices of the store are listed.
    /// </summary>
    [Argument("slot", false)]
    public int Slot { get; set; }

    /// <summary>
    /// Gets or sets the price amount. A value of 0 removes the price.
    /// </summary>
    [Argument("amount", false)]
    public int Amount { get; set; } = -1;

    /// <summary>
    /// Gets or sets the currency of the price, e.g. "zen", "bless", "gp" or the group and number of an item like "14/13".
    /// When it's empty, it's zen.
    /// </summary>
    [Argument("currency", false)]
    public string? Currency { get; set; }
}
