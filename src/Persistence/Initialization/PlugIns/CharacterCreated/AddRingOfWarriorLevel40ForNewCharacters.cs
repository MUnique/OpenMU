// <copyright file="AddRingOfWarriorLevel40ForNewCharacters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the ring, which can be dropped at level 40.
/// </summary>
[Guid("19B0BB9D-32C9-4DBC-BD95-41801D078962")]
[PlugIn(nameof(AddRingOfWarriorLevel40ForNewCharacters), "Adds the ring, which can be dropped at level 40.")]
public class AddRingOfWarriorLevel40ForNewCharacters : AddInitialItemPlugInBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddRingOfWarriorLevel40ForNewCharacters" /> class.
    /// </summary>
    public AddRingOfWarriorLevel40ForNewCharacters()
        : base(null, 13, 20, (byte)(InventoryConstants.LastEquippableItemSlotIndex + 1), 1)
    {
    }
}