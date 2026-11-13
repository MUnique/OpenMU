// <copyright file="PartyResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// The party response action.
/// </summary>
public class PartyResponseAction
{
    /// <summary>
    /// Handles the response of the party request of <see cref="IPartyMember.LastPartyRequester"/>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="accepted">if set to <c>true</c> the player accepted the party request of <see cref="IPartyMember.LastPartyRequester"/>.</param>
    public async ValueTask HandleResponseAsync(Player player, bool accepted)
    {
        IPartyMember? partyRequester;
        await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            context.Allowed &= player.PlayerState.CurrentState == PlayerState.PartyRequest;
            if (!context.Allowed)
            {
                if (player.PlayerState.CurrentState != PlayerState.PartyRequest)
                {
                    player.LastPartyRequester = null;
                }

                return;
            }

            partyRequester = player.LastPartyRequester;
            player.LastPartyRequester = null;
        }

        if (!player.IsAlive || partyRequester is null || partyRequester is Player { IsAlive: false })
        {
            return;
        }

        if (!accepted)
        {
            return;
        }

        if (player.Party != null)
        {
            return;
        }

        if (player.CurrentMiniGame?.Definition.AllowParty is false)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PartyNotPossibleDuringEvent)).ConfigureAwait(false);
            return;
        }

        // Checked again, because the players could have changed in the meantime, e.g. their gens.
        if (partyRequester is Player requester
            && await PartyRequestAction.IsPartyDeniedAsync(requester, player).ConfigureAwait(false))
        {
            return;
        }

        if (partyRequester.Party != null)
        {
            // The Requester got a party already, so add him to his party
            await partyRequester.Party.AddAsync(player).ConfigureAwait(false);
        }
        else
        {
            var master = partyRequester;
            var party = player.GameContext.PartyManager.CreateParty();
            await party.AddAsync(master).ConfigureAwait(false);
            await party.AddAsync(player).ConfigureAwait(false);
        }
    }
}