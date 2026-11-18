// <copyright file="ItemRegistrationOperation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The possible outcomes of an item registration attempt, reported back to the view plug-in.
/// </summary>
public enum ItemRegistrationOperation
{
    /// <summary>
    /// The registration dialog was opened.
    /// </summary>
    OpenRegistrationDialog,

    /// <summary>
    /// The required item was missing from the player's inventory.
    /// </summary>
    MissingItem,

    /// <summary>
    /// The registration was completed.
    /// </summary>
    RegistrationCompleted,
}
