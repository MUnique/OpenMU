// <copyright file="IShowItemCraftingPreviewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;

/// <summary>
/// Interface of a view whose implementation informs about the crafting which matches the
/// items of the crafting dialog, with its success rate and price, before the player starts it.
/// </summary>
public interface IShowItemCraftingPreviewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the crafting which matches the items of the crafting dialog.
    /// </summary>
    /// <param name="crafting">The matching crafting, or <c>null</c> if the items don't match any crafting.</param>
    /// <param name="successRate">The success rate in percent, including the crafting bonus of the player.</param>
    /// <param name="price">The price in zen, without the tax of the chaos machine.</param>
    ValueTask ShowCraftingPreviewAsync(ItemCrafting? crafting, byte successRate, int price);
}
