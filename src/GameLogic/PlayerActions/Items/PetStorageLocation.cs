// <copyright file="PetStorageLocation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Defines the kind of location of a pet.
/// </summary>
public enum PetStorageLocation
{
    /// <summary>
    /// The inventory of the player.
    /// </summary>
    Inventory,

    /// <summary>
    /// The vault of the player.
    /// </summary>
    Vault,

    /// <summary>
    /// The own trading storage.
    /// </summary>
    TradeOwn,

    /// <summary>
    /// The trading storage of the other player.
    /// </summary>
    TradeOther,

    /// <summary>
    /// The crafting storage of the player.
    /// </summary>
    Crafting,

    /// <summary>
    /// The shop storage of another player.
    /// </summary>
    PersonalShop,

    /// <summary>
    /// The inventory slot of the pet. That's used when a pet leveled up.
    /// </summary>
    InventoryPetSlot,
}

/// <summary>
/// Defines the pet (raven) behavior.
/// </summary>
public enum PetBehaviour
{
    /// <summary>
    /// The pet is in a idle mode, where it doesn't attack.
    /// </summary>
    Idle,

    /// <summary>
    /// The pet attacks random targets.
    /// </summary>
    AttackRandom,

    /// <summary>
    /// The pet attacks the same targets as the owner.
    /// </summary>
    AttackWithOwner,

    /// <summary>
    /// The pet attacks a specific target until it's dead.
    /// </summary>
    AttackTarget,
}