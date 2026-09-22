// <copyright file="PetExperiencePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.Pet;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which gives the trainable pets of a player a share of the experience which the
/// player gained for a kill, and levels them up.
/// </summary>
[PlugIn]
[Display(Name = nameof(PetExperiencePlugIn), Description = "Gives the trainable pets of a player a share of the experience which the player gained for a kill.")]
[Guid("6B9A0A0E-5C6A-4E8F-A4E1-3C7B8B9A2D51")]
public class PetExperiencePlugIn : IPlayerGainedExperiencePlugIn
{
    /// <summary>
    /// The share of the player's experience which a pet gains. If both, a riding pet and an
    /// attacking pet are equipped, each of them gains the half of it.
    /// </summary>
    private const double PetShare = 0.2;

    /// <inheritdoc />
    public async ValueTask PlayerGainedExperienceAsync(Player player, int experience, IAttackable killedObject, bool isMasterExperience)
    {
        var movePet = GetTrainablePet(player, InventoryConstants.PetSlot);
        var attackPet = GetTrainablePet(player, InventoryConstants.RightHandSlot);

        if (movePet is null && attackPet is null)
        {
            return;
        }

        var petExperience = (int)(experience * PetShare);

        if (movePet is not null && attackPet is not null)
        {
            // Both are there, so each gains just the half.
            petExperience /= 2;
        }

        if (petExperience < 1)
        {
            return;
        }

        if (movePet is { })
        {
            await AddExpToPetAsync(player, movePet, petExperience).ConfigureAwait(false);
        }

        if (attackPet is { })
        {
            await AddExpToPetAsync(player, attackPet, petExperience).ConfigureAwait(false);
        }
    }

    private static Item? GetTrainablePet(Player player, byte inventorySlot)
    {
        var pet = player.Inventory?.GetItem(inventorySlot);
        if (pet is not null
            && pet.Definition is not null
            && pet.Definition.PetExperienceFormula is not null
            && pet.Definition.MaximumItemLevel > 0
            && pet.Durability > 0
            && pet.Level < pet.Definition.MaximumItemLevel)
        {
            return pet;
        }

        return null;
    }

    private static async ValueTask AddExpToPetAsync(Player player, Item pet, double experience)
    {
        pet.PetExperience += (int)experience;

        while (pet.PetExperience >= pet.Definition!.GetExperienceOfPetLevel((byte)(pet.Level + 1), pet.Definition!.MaximumItemLevel)
               && (!pet.IsDarkRaven() || pet.GetDarkRavenLeadershipRequirement(pet.Level + 1) <= player.Attributes![Stats.TotalLeadership]))
        {
            pet.Level++;
            player.Attributes!.ItemPowerUps[pet] = player.Attributes.ItemPowerUps[pet]
                .Append(new PowerUpWrapper(
                    new SimpleElement(1, AggregateType.AddRaw),
                    pet.IsDarkRaven() ? Stats.RavenLevel : Stats.HorseLevel,
                    player.Attributes)).ToList();

            await player.InvokeViewPlugInAsync<IPetInfoViewPlugIn>(p => p.ShowPetInfoAsync(pet, pet.ItemSlot, PetStorageLocation.InventoryPetSlot)).ConfigureAwait(false);
        }
    }
}
