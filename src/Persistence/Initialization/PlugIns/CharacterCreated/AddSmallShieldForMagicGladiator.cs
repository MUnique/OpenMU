// <copyright file="AddSmallShieldForMagicGladiator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds a small shield to a created magic gladiator character.
/// </summary>
[Guid("74FC7D85-7AA0-4437-88BA-CE008FD31745")]
[PlugIn(nameof(AddSmallShieldForMagicGladiator), "Adds a small shield to a created magic gladiator character.")]
public class AddSmallShieldForMagicGladiator : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSmallShieldForMagicGladiator" /> class.
    /// </summary>
    public AddSmallShieldForMagicGladiator()
        : base((byte)CharacterClassNumber.MagicGladiator, (byte)ItemGroups.Shields, 0, 1)
    {
    }
}