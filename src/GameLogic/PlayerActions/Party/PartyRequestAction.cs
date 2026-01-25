// <copyright file="PartyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic.Views.Party;

/// <summary>
/// The party request action.
/// </summary>
public class PartyRequestAction
{
    /// <summary>
    /// Handles the party request from the <paramref name="player"/> to <paramref name="toRequest"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="toRequest">The player which receives the request.</param>
    public async ValueTask HandlePartyRequestAsync(Player player, Player toRequest)
    {
        var isPartyMember = player.Party != null && !Equals(player.Party.PartyMaster, player);
        if (player.CurrentMiniGame?.Definition.AllowParty is false)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PartyNotPossibleDuringEvent)).ConfigureAwait(false);
            return;
        }

        if (toRequest.Party != null || toRequest.LastPartyRequester != null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerIsAlreadyInParty), toRequest.Name).ConfigureAwait(false);
            return;
        }

        if (isPartyMember)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.YouAreNotPartyMaster)).ConfigureAwait(false);
            return;
        }

        if (await toRequest.PlayerState.TryAdvanceToAsync(PlayerState.PartyRequest).ConfigureAwait(false))
        {
            await this.SendPartyRequestAsync(toRequest, player).ConfigureAwait(false);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.RequestedPlayerForParty), toRequest.Name).ConfigureAwait(false);
        }
    }

    private async ValueTask SendPartyRequestAsync(IPartyMember toRequest, IPartyMember requester)
    {
        if (Equals(requester, toRequest))
        {
            return;
        }

        toRequest.LastPartyRequester = requester;
        await toRequest.InvokeViewPlugInAsync<IShowPartyRequestPlugIn>(p => p.ShowPartyRequestAsync(requester)).ConfigureAwait(false);
    }
}