// <copyright file="PartyKickAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party
{
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Action to kick players from the party.
    /// </summary>
    public class PartyKickAction
    {
        /// <summary>
        /// Kicks the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="index">The index.</param>
        public void KickPlayer(Player player, byte index)
        {
            if (player.Party != null)
            {
                player.Party.KickPlayer(player, index);
            }
        }
    }
}
