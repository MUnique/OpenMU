// <copyright file="PartyAutoRejoinPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Listens for the <see cref="IPlayerStateChangedPlugIn"/> and automatically rejoins the player
/// to their previous party if it still exists when they enter the world.
/// </summary>
[PlugIn]
[Display(
    Name = nameof(PlugInResources.PartyAutoRejoinPlugIn_Name),
    Description = nameof(PlugInResources.PartyAutoRejoinPlugIn_Description),
    ResourceType = typeof(PlugInResources))]
[Guid("013406B3-02AD-45EF-906B-177CBEC9B2D4")]
public sealed class PartyAutoRejoinPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState != PlayerState.EnteredWorld)
        {
            return;
        }

        await player.GameContext.PartyManager.OnMemberReconnectedAsync(player).ConfigureAwait(false);

        if (player.Party is not null)
        {
            player.Logger.LogDebug("Player {PlayerName} rejoined their previous party.", player.Name);
            await player.InvokeViewPlugInAsync<IPartyHealthViewPlugIn>(p => p.UpdatePartyHealthAsync()).ConfigureAwait(false);
        }
    }
}