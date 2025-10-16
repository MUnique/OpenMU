// <copyright file="IShowItemCraftingResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Interface of a view whose implementation informs about the crafting result.
/// </summary>
public interface IShowItemCraftingResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the crafting result.
    /// </summary>
    /// <param name="result">The crafting result.</param>
    /// <param name="successRate">The success rate in percent.</param>
    /// <param name="bonusRate">The additional bonus rate in percent.</param>
    /// <param name="createdItem">The created item.</param>
    ValueTask ShowResultAsync(CraftingResult result, byte successRate, byte bonusRate, Item? createdItem);
}