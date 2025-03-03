// <copyright file="AddShortSwordForDarkLord.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds a short sword to a created dark lord character.
/// </summary>
[Guid("BAC120D0-D981-4EBB-8F5A-0EC19434AF16")]
[PlugIn(nameof(AddShortSwordForDarkLord), "Adds a short sword to a created dark lord character.")]
public class AddShortSwordForDarkLord : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddShortSwordForDarkLord" /> class.
    /// </summary>
    public AddShortSwordForDarkLord()
        : base((byte)CharacterClassNumber.DarkLord, (byte)ItemGroups.Swords, 1, 0)
    {
    }
}