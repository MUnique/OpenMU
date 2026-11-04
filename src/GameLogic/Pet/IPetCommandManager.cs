// <copyright file="IPetCommandManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Pet;

using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Interface for a pet command manager.
/// </summary>
public interface IPetCommandManager : IDisposable
{
    /// <summary>
    /// Sets the behaviour of the pet.
    /// </summary>
    /// <param name="newBehaviour">The new behaviour.</param>
    /// <param name="target">The target.</param>
    ValueTask SetBehaviourAsync(PetBehaviour newBehaviour, IAttackable? target);
}