// <copyright file="SetPetBehaviourRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Action which requests to set a new behaviour for an equipped pet.
/// </summary>
public class SetPetBehaviourRequestAction
{
    /// <summary>
    /// Requests the command asynchronous.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetId">The target identifier.</param>
    /// <param name="behaviour">The behaviour.</param>
    public async ValueTask RequestChangeAsync(Player player, ushort targetId, PetBehaviour behaviour)
    {
        if (player.Inventory?.GetItem(InventoryConstants.RightHandSlot) is not { } pet
            || !pet.IsTrainablePet())
        {
            player.Logger.LogDebug("Player {player} requested to change the pet command, but has no trainable pet equipped.", player);
            return;
        }

        if (player.PetCommandManager is { } petCommandManager)
        {
            var target = player.GetObject(targetId);
            await petCommandManager.SetBehaviourAsync(behaviour, target as IAttackable).ConfigureAwait(false);
        }
    }
}