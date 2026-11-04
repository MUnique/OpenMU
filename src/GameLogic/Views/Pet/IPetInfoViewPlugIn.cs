// <copyright file="IPetInfoViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Pet;

using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Interface of a view whose implementation informs about the pet information about an item in the inventory.
/// </summary>
public interface IPetInfoViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the pet information.
    /// </summary>
    /// <param name="petItem">The pet item.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="petStorageLocation">The pet storage location.</param>
    ValueTask ShowPetInfoAsync(Item petItem, byte slot, PetStorageLocation petStorageLocation);
}