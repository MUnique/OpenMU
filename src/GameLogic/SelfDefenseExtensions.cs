// <copyright file="SelfDefenseExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

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
