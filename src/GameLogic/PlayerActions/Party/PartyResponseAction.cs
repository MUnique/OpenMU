// <copyright file="PartyResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party
{
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
        public void HandleResponse(Player player, bool accepted)
        {
            if (player.PlayerState.CurrentState != PlayerState.PartyRequest)
            {
                player.LastPartyRequester = null;
                return;
            }

            player.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);

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

            if (player.LastPartyRequester.Party != null)
            {
                // The Requester got a party already, so add him to his party
                player.LastPartyRequester.Party.Add(player);
            }
            else
            {
                player.LastPartyRequester.Party = new Party(player.GameContext.Configuration.MaximumPartySize);
                player.LastPartyRequester.Party.Add(player.LastPartyRequester); // Party Master first
                player.LastPartyRequester.Party.Add(player);
            }

            player.LastPartyRequester = null;
        }
    }
}
