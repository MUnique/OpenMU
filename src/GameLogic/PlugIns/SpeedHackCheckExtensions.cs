// <copyright file="SpeedHackCheckExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extensions for running the attack speed check at accepted attack points.
/// </summary>
internal static class SpeedHackCheckExtensions
{
    /// <summary>
    /// Determines whether an already validated attack may be executed.
    /// </summary>
    /// <param name="player">The attacking player.</param>
    /// <param name="skill">The skill which is used for the attack, or <c>null</c> for a regular hit.</param>
    /// <returns><c>true</c> if no check plugin is registered or the attack is allowed; otherwise, <c>false</c>.</returns>
    internal static async ValueTask<bool> IsAttackAllowedAsync(this Player player, Skill? skill)
    {
        if (player.GameContext.PlugInManager.GetPlugInPoint<IAttackSpeedCheckPlugIn>() is not { } speedCheck)
        {
            return true;
        }

        var eventArgs = new SpeedHackCheckEventArgs();
        await speedCheck.AttackCheatCheckAsync(player, skill, eventArgs).ConfigureAwait(false);
        return !eventArgs.IsCheatDetected;
    }
}
