// <copyright file="IPetAttackViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Pet;

/// <summary>
/// Interface of a view whose implementation informs about a pet (raven) attack.
/// </summary>
public interface IPetAttackViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the pet attack animation.
    /// </summary>
    /// <param name="owner">The owner of the pet.</param>
    /// <param name="pet">The pet.</param>
    /// <param name="target">The target.</param>
    /// <param name="attackType">Type of the attack.</param>
    ValueTask ShowPetAttackAnimationAsync(IIdentifiable owner, Item pet, IAttackable target, PetAttackType attackType);
}