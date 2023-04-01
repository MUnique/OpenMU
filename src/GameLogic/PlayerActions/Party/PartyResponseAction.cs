// <copyright file="PartyResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;

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
        if (player.PlayerState.CurrentState != PlayerState.PartyRequest)
        {
            player.LastPartyRequester = null;
            return;
        }

        if (player.LastPartyRequester is null)
        {
            return;
        }

        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);

        if (!accepted)
        {
            player.LastPartyRequester = null;
            return;
        }

        if (player.Party != null)
        {
            player.LastPartyRequester = null;
            return;
        }

        if (player.CurrentMiniGame?.Definition.AllowParty is false)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("A party is not possible during this event.", MessageType.BlueNormal)).ConfigureAwait(false);
            player.LastPartyRequester = null;
            return;
        }

        if (player.LastPartyRequester.Party != null)
        {
            // The Requester got a party already, so add him to his party
            await player.LastPartyRequester.Party.AddAsync(player).ConfigureAwait(false);
        }
        else
        {
            var master = player.LastPartyRequester;
            var party = new Party(player.GameContext.Configuration.MaximumPartySize, player.GameContext.LoggerFactory.CreateLogger<Party>());
            await party.AddAsync(master).ConfigureAwait(false);
            await party.AddAsync(player).ConfigureAwait(false);
        }

        player.LastPartyRequester = null;
    }
}