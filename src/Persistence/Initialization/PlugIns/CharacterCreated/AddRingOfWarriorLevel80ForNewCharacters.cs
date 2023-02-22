// <copyright file="AddRingOfWarriorLevel80ForNewCharacters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the ring, which can be dropped at level 80.
/// </summary>
[Guid("BF9A7B67-9404-4E86-B1A4-77B96C19F55C")]
[PlugIn(nameof(AddRingOfWarriorLevel80ForNewCharacters), "Adds the ring, which can be dropped at level 80.")]
public class AddRingOfWarriorLevel80ForNewCharacters : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddRingOfWarriorLevel80ForNewCharacters" /> class.
    /// </summary>
    public AddRingOfWarriorLevel80ForNewCharacters()
        : base(null, 13, 20, (byte)(InventoryConstants.LastEquippableItemSlotIndex + 2), 2)
    {
    }
}