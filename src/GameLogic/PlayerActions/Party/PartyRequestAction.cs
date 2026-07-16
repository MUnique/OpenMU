// <copyright file="PartyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic.MuHelper;
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
            // A server-side bot is asked as well when it is a plain member of its (bot) party: a living
            // player takes precedence over the bot's own company, so it leaves that party and joins the
            // inviter (see BotPartyHandler). Everyone else keeps the original rule - only the master of a
            // party can answer an invitation.
            var isBot = toRequest.Account?.IsBot == true;
            if (toRequest.Party != null && (isBot || Equals(toRequest.Party.PartyMaster, toRequest)))
            {
                if (await PartyRequestHandler.TryAutoAcceptPartyRequestAsync(toRequest, player).ConfigureAwait(false))
                {
                    return;
                }
            }

            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerIsAlreadyInParty), toRequest.Name).ConfigureAwait(false);
            return;
        }

        if (isPartyMember)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.YouAreNotPartyMaster)).ConfigureAwait(false);
            return;
        }

        if (toRequest is Offline.OfflinePlayer)
        {
            if (await PartyRequestHandler.TryAutoAcceptPartyRequestAsync(toRequest, player).ConfigureAwait(false))
            {
                return;
            }

            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PlayerIsAlreadyInParty), toRequest.Name).ConfigureAwait(false);
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

        if (toRequest is Player receiver && requester is Player requesterPlayer)
        {
            if (await PartyRequestHandler.TryAutoAcceptPartyRequestAsync(receiver, requesterPlayer).ConfigureAwait(false))
            {
                return;
            }
        }

        await toRequest.InvokeViewPlugInAsync<IShowPartyRequestPlugIn>(p => p.ShowPartyRequestAsync(requester)).ConfigureAwait(false);
    }
}