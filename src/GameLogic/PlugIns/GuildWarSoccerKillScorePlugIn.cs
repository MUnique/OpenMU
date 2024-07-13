// <copyright file="GuildWarSoccerKillScorePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin increases the score of the soccer result, if a kill occurred.
/// </summary>
[PlugIn("Guild soccer kill score", "This plugin increases the score of the soccer result, if a kill occurred.")]
[Guid("2D4E16CD-B7FF-4ED3-B4B1-4AABD04BAD71")]
public class GuildWarKillScorePlugIn : IAttackableGotKilledPlugIn
{
    /// <summary>
    /// Is called when an <see cref="IAttackable" /> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable" />.</param>
    /// <param name="killer">The killer.</param>
    public async ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killer is Player { GuildWarContext: not null } killerPlayer
            && killed is Player { GuildWarContext: not null } killedPlayer
            && killerPlayer.GuildWarContext!.Team != killedPlayer.GuildWarContext!.Team)
        {
            var score = killedPlayer.GuildStatus?.Position == GuildPosition.GuildMaster ? (byte)2 : (byte)1;
            if (killerPlayer.GuildWarContext.Team == GuildWarTeam.First)
            {
                killerPlayer.GuildWarContext.Score.IncreaseFirstGuildScore(score);
            }
            else
            {
                killerPlayer.GuildWarContext.Score.IncreaseSecondGuildScore(score);
            }
        }
    }
}