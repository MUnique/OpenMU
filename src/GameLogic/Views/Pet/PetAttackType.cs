// <copyright file="PetAttackType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Pet;

/// <summary>
/// Defines the type of attack.
/// </summary>
public enum PetAttackType
{
    /// <summary>
    /// A single target attack.
    /// </summary>
    SingleTarget,

    /// <summary>
    /// A range attack for multiple targets, usually up to 3 additional targets which all get their
    /// own attack with <see cref="SingleTarget"/> right after the first <see cref="RangeAttack"/>.
    /// </summary>
    RangeAttack,
}