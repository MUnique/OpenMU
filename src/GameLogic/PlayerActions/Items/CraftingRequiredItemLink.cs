// <copyright file="CraftingRequiredItemLink.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

/// <summary>
/// Defines which actual items of the <see cref="Player.TemporaryStorage"/> are fulfilling a specific item requirement.
/// </summary>
public class CraftingRequiredItemLink
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CraftingRequiredItemLink"/> class.
    /// </summary>
    /// <param name="items">The items which are linked to the <see cref="ItemCraftingRequiredItem"/>.</param>
    /// <param name="requiredItem">The required item.</param>
    public CraftingRequiredItemLink(IEnumerable<Item> items, ItemCraftingRequiredItem requiredItem)
    {
        this.Items = items;
        this.ItemRequirement = requiredItem;
    }

    /// <summary>
    /// Gets or sets the he items which are linked to the <see cref="ItemCraftingRequiredItem"/>.
    /// </summary>
    public IEnumerable<Item> Items { get; set; }

    /// <summary>
    /// Gets or sets the item requirement.
    /// </summary>
    public ItemCraftingRequiredItem ItemRequirement { get; set; }
}