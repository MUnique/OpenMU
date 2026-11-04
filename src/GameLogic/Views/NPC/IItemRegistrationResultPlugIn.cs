// <copyright file="IItemRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

using System.Threading.Tasks;

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

/// <summary>
/// A view plugin which provides the result of registering an item at the Golden Archer.
/// </summary>
public interface IItemRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Sends the registration result to the client.
    /// </summary>
    /// <param name="npcNumber">The NPC number.</param>
    /// <param name="operation">The registration operation.</param>
    ValueTask RegistrationResultAsync(short npcNumber, ItemRegistrationOperation operation);
}
