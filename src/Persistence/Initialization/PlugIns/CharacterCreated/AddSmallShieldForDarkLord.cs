// <copyright file="AddSmallShieldForDarkLord.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds a small shield to a created dark lord character.
/// </summary>
[Guid("CD60BD4A-2BD5-4E36-95D0-EDB6B94CDDD8")]
[PlugIn(nameof(AddShortSwordForDarkLord), "Adds a small shield to a created dark lord character.")]
public class AddSmallShieldForDarkLord : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSmallShieldForDarkLord" /> class.
    /// </summary>
    public AddSmallShieldForDarkLord()
        : base((byte)CharacterClassNumber.DarkLord, (byte)ItemGroups.Shields, 0, 1)
    {
    }
}