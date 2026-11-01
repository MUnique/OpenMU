// <copyright file="GensMembershipPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Loads the gens membership of a character when it enters the game, and shows it to the player.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensMembershipPlugIn_Name), Description = nameof(PlugInResources.GensMembershipPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("8F4E1A62-0B7C-4D39-A5E8-2C6D9B3F7E14")]
public class GensMembershipPlugIn : IPlayerStateChangedPlugIn, IObjectAddedToMapPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState == PlayerState.CharacterSelection)
        {
            player.GensMember = null;
            return;
        }

        if (previousState != PlayerState.CharacterSelection
            || currentState != PlayerState.EnteredWorld
            || player.SelectedCharacter is not { } character
            || GensFeaturePlugIn.GetConfiguration(player.GameContext) is null)
        {
            return;
        }

        // It's loaded before the player is added to the map, so that the other players see its gens.
        player.GensMember = await player.RunPersistenceExclusiveAsync(
                () => player.PersistenceContext.GetGensMemberAsync(character.Id))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        // The game client resets the gens of the own character when it enters a map or respawns.
        if (addedObject is Player { GensMember.Gens: not GensType.None } player)
        {
            await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowGensInfoAsync()).ConfigureAwait(false);
        }
    }
}
