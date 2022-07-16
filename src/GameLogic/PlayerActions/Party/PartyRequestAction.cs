// <copyright file="PartyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.Interfaces;

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
        if (toRequest.Party != null || toRequest.LastPartyRequester != null)
        {
            await this.SendMessageToPlayerAsync(player, $"{player.Name} is already in a party.", MessageType.BlueNormal).ConfigureAwait(false);
        }
        else if (player.Party is null || (player.Party != null && Equals(player.Party.PartyMaster, player)))
        {
            if (await toRequest.PlayerState.TryAdvanceToAsync(PlayerState.PartyRequest).ConfigureAwait(false))
            {
                await this.SendPartyRequestAsync(toRequest, player).ConfigureAwait(false);
                await this.SendMessageToPlayerAsync(player, $"Requested {toRequest.Name} for Party.", MessageType.BlueNormal).ConfigureAwait(false);
            }
        }
        else
        {
            await this.SendMessageToPlayerAsync(player, "You are not the Party Master.", MessageType.BlueNormal).ConfigureAwait(false);
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

    private async ValueTask SendMessageToPlayerAsync(IPartyMember partyMember, string message, MessageType type)
    {
        if (partyMember is Player player)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, type)).ConfigureAwait(false);
        }
    }
}