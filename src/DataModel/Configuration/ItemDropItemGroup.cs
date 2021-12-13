// <copyright file="ItemDropItemGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A <see cref="DropItemGroup"/> which acts a definition of possible items when a special item (e.g. Box of Luck) is dropped by the player.
/// </summary>
public class ItemDropItemGroup : DropItemGroup
{
    /// <summary>
    /// Gets or sets the <see cref="Item.Level"/> of the source item which was dropped by the player.
    /// </summary>
    public byte SourceItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the amount of money, in case <see cref="DropItemGroup.ItemType"/> is <see cref="SpecialItemType.Money"/>.
    /// </summary>
    public int MoneyAmount { get; set; }
}