// <copyright file="SelfDefenseExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Extensions to query the self defense state which is maintained by the
/// <see cref="PlugIns.SelfDefensePlugIn"/>.
/// </summary>
public static class SelfDefenseExtensions
{
    /// <summary>
    /// Determines whether the self defense is active for the specified attacker.
    /// </summary>
    /// <param name="player">The player which defends itself.</param>
    /// <param name="attacker">The attacker.</param>
    /// <returns>
    ///   <c>true</c> if the self defense is active for the specified attacker; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSelfDefenseActive(this Player player, Player attacker)
    {
        if (player.GameContext.SelfDefenseState.TryGetValue((attacker, player), out var timeout))
        {
            return timeout > DateTime.UtcNow;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the attacks and kills of the player on the defender are exempted from the
    /// player killer penalty and the self-defense by a <see cref="IPlayerKillPenaltyExemptionPlugIn"/>.
    /// </summary>
    /// <param name="attacker">The attacking player.</param>
    /// <param name="defender">The attacked player.</param>
    /// <returns><c>true</c>, if the attacks and kills are exempted; otherwise, <c>false</c>.</returns>
    public static bool IsExemptedFromPlayerKillPenalty(this Player attacker, Player defender)
    {
        if (attacker.GameContext.PlugInManager.GetPlugInPoint<IPlayerKillPenaltyExemptionPlugIn>() is not { } plugInPoint)
        {
            return false;
        }

        var eventArgs = new PlayerKillPenaltyExemptionArgs();
        plugInPoint.CheckExemption(attacker, defender, eventArgs);
        return eventArgs.IsExempted;
    }

    /// <summary>
    /// Determines whether the self-defense is active for any attacker.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>
    ///   <c>true</c> if any self-defense is active; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAnySelfDefenseActive(this Player player)
    {
        var selfDefenses = player.GameContext.SelfDefenseState.Keys.Where(c => c.Attacker == player).ToList();
        return selfDefenses.Any(sd =>
            player.GameContext.SelfDefenseState.TryGetValue(sd, out var timeout)
            && timeout >= DateTime.UtcNow);
    }
}
