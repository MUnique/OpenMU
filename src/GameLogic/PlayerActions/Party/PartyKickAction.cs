// <copyright file="PartyKickAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Party;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Action to kick players from the party.
/// </summary>
public class PartyKickAction
{
    /// <summary>
    /// Kicks the player.
    /// Only the party master is allowed to kick other players. However, players can kick themselves out of the party.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="index">The index.</param>
    public async ValueTask KickPlayerAsync(Player player, byte index)
    {
        if (player.Party is not { } party)
        {
            return;
        }

        if (!Equals(player, party.PartyList[0]) &&
            !Equals(player, party.PartyList[index]))
        {
            player.Logger.LogWarning("Suspicious party kick request of {0}, could be hack attempt.", player);
            return;
        }

        await party.KickPlayerAsync(index).ConfigureAwait(false);
    }
}