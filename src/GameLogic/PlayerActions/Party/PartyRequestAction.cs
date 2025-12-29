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
        var isPartyMember = player.Party != null && !Equals(player.Party.PartyMaster, player);
        if (player.CurrentMiniGame?.Definition.AllowParty is false)
        {
            await this.SendMessageToPlayerAsync(player, "No es posible formar party durante este evento.", MessageType.BlueNormal).ConfigureAwait(false);
            return;
        }

        if (toRequest.Party != null || toRequest.LastPartyRequester != null)
        {
            await this.SendMessageToPlayerAsync(player, $"{toRequest.Name} ya está en un party.", MessageType.BlueNormal).ConfigureAwait(false);
            return;
        }

        if (isPartyMember)
        {
            await this.SendMessageToPlayerAsync(player, "No eres el líder del party.", MessageType.BlueNormal).ConfigureAwait(false);
            return;
        }

        if (await toRequest.PlayerState.TryAdvanceToAsync(PlayerState.PartyRequest).ConfigureAwait(false))
        {
            await this.SendPartyRequestAsync(toRequest, player).ConfigureAwait(false);
            await this.SendMessageToPlayerAsync(player, $"Has enviado una solicitud de party a {toRequest.Name}.", MessageType.BlueNormal).ConfigureAwait(false);
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
        if (partyMember is not Player player)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, type)).ConfigureAwait(false);
    }
}
