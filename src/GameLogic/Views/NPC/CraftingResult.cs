// <copyright file="CraftingResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Defines the result of an item crafting.
/// </summary>
public enum CraftingResult
{
    /// <summary>
    /// The crafting failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The crafting succeeded.
    /// </summary>
    Success,

    /// <summary>
    /// The crafting wasn't executed because of missing money.
    /// </summary>
    NotEnoughMoney,

    /// <summary>
    /// The crafting wasn't executed because of too many items.
    /// </summary>
    TooManyItems,

    /// <summary>
    /// The crafting wasn't executed because the character level is too low.
    /// </summary>
    CharacterLevelTooLow,

    /// <summary>
    /// The crafting wasn't executed because of missing items.
    /// </summary>
    LackingMixItems,

    /// <summary>
    /// The crafting wasn't executed because of incorrect items.
    /// </summary>
    IncorrectMixItems,

    /// <summary>
    /// The crafting wasn't executed because of an invalid item level.
    /// </summary>
    InvalidItemLevel,

    /// <summary>
    /// The crafting wasn't executed because the character class is too low.
    /// </summary>
    CharacterClassTooLow,

    /// <summary>
    /// The blood castle ticket crafting wasn't executed because the BloodCastle items are not correct.
    /// </summary>
    IncorrectBloodCastleItems,

    /// <summary>
    /// The crafting wasn't executed because the player has not enough money for the blood castle ticket crafting.
    /// </summary>
    NotEnoughMoneyForBloodCastle,
}