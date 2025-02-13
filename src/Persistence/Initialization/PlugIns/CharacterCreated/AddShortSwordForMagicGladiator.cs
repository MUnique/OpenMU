// <copyright file="AddShortSwordForMagicGladiator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds a short sword to a created magic gladiator character.
/// </summary>
[Guid("3D2790E3-B757-46FD-8618-2441B7E9E2B3")]
[PlugIn(nameof(AddShortSwordForMagicGladiator), "Adds a short sword to a created magic gladiator character.")]
public class AddShortSwordForMagicGladiator : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddShortSwordForMagicGladiator" /> class.
    /// </summary>
    public AddShortSwordForMagicGladiator()
        : base((byte)CharacterClassNumber.MagicGladiator, (byte)ItemGroups.Swords, 1, 0)
    {
    }
}