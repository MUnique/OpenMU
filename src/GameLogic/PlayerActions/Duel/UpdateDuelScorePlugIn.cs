// <copyright file="UpdateDuelScorePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin increases the score of the duel result, when a kill occurred.
/// </summary>
[PlugIn(nameof(UpdateDuelScorePlugIn), "This plugin increases the score of the duel, when a kill occurred.")]
[Guid("C5CAC184-F8A9-41A0-A34E-04A07DB81F5E")]
public class UpdateDuelScorePlugIn : IAttackableGotKilledPlugIn
{
    /// <summary>
    /// Is called when an <see cref="IAttackable" /> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable" />.</param>
    /// <param name="killer">The killer.</param>
    public async ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killer is Player { DuelRoom: not null } killerPlayer
            && killed is Player { DuelRoom: not null } killedPlayer
            && killerPlayer.DuelRoom == killedPlayer.DuelRoom)
        {
            var duelRoom = killerPlayer.DuelRoom;
            using var l = await duelRoom.Lock.LockAsync();
            if (duelRoom.State is not DuelState.DuelStarted)
            {
                return;
            }

            if (duelRoom.Requester == killerPlayer)
            {
                duelRoom.ScoreRequester++;
            }
            else
            {
                duelRoom.ScoreOpponent++;
            }
        }
    }
}