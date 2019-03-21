// <copyright file="PartyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party
{
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
        public void HandlePartyRequest(Player player, Player toRequest)
        {
            if (toRequest.Party != null || toRequest.LastPartyRequester != null)
            {
                this.SendMessageToPlayer(player, $"{player.Name} is already in a party.", MessageType.BlueNormal);
            }
            else if (player.Party == null || (player.Party != null && Equals(player.Party.PartyMaster, player)))
            {
                if (toRequest.PlayerState.TryAdvanceTo(PlayerState.PartyRequest))
                {
                    this.SendPartyRequest(toRequest, player);
                    this.SendMessageToPlayer(player, $"Requested {toRequest.Name} for Party.", MessageType.BlueNormal);
                }
            }
            else
            {
                this.SendMessageToPlayer(player, "You are not the Party Master.", MessageType.BlueNormal);
            }
        }

        private void SendPartyRequest(IPartyMember toRequest, IPartyMember requester)
        {
            if (Equals(requester, toRequest))
            {
                return;
            }

            toRequest.LastPartyRequester = requester;
            toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()?.ShowPartyRequest(requester);
        }

        private void SendMessageToPlayer(IPartyMember partyMember, string message, MessageType type)
        {
            if (partyMember is Player player)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, type);
            }
        }
    }
}
