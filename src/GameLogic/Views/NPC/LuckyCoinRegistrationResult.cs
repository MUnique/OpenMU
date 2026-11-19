// <copyright file="LuckyCoinRegistrationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The result of a lucky coin registration at the NPC Delgado.
/// </summary>
public enum LuckyCoinRegistrationResult
{
    /// <summary>
    /// The player has no lucky coin in the inventory.
    /// </summary>
    NotEnoughItems,

    /// <summary>
    /// The lucky coin was registered.
    /// </summary>
    Registered,
}
