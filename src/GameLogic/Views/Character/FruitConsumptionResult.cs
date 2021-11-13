// <copyright file="FruitConsumptionResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character;

/// <summary>
/// Defines the result of the fruit consumption request.
/// </summary>
public enum FruitConsumptionResult
{
    /// <summary>
    /// Consumption to add points was successful.
    /// </summary>
    PlusSuccess,

    /// <summary>
    /// Consumption to add points failed.
    /// </summary>
    PlusFailed,

    /// <summary>
    /// Consumption to add points was prevented because some conditions were not correct.
    /// </summary>
    PlusPrevented,

    /// <summary>
    /// Consumption to remove points was successful.
    /// </summary>
    MinusSuccess,

    /// <summary>
    /// Consumption to remove points failed.
    /// </summary>
    MinusFailed,

    /// <summary>
    /// Consumption to remove points was prevented because some conditions were not correct.
    /// </summary>
    MinusPrevented,

    /// <summary>
    /// Consumption to remove points was successful, removed by a fruit acquired through the cash shop.
    /// </summary>
    MinusSuccessCashShopFruit,

    /// <summary>
    /// Consumption was prevented because an item was equipped.
    /// </summary>
    PreventedByEquippedItems,

    /// <summary>
    /// Consumption to add points was prevented because the maximum amount of points have been added.
    /// </summary>
    PlusPreventedByMaximum,

    /// <summary>
    /// Consumption to remove points was prevented because the maximum amount of points have been removed.
    /// </summary>
    MinusPreventedByMaximum,

    /// <summary>
    /// Consumption to remove points was prevented because the base amount of stat points of the character class cannot be undercut.
    /// </summary>
    MinusPreventedByDefault,
}