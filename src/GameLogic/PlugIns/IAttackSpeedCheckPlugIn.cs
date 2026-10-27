// <copyright file="IAttackSpeedCheckPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point interface for attack speed checks.
/// </summary>
[Guid("6E2D7A1B-5C3F-4F0E-9B7A-1D2C3E4F5A6B")]
[PlugInPoint("Attack speed check", "Is called for attack speedhack checks.")]
public interface IAttackSpeedCheckPlugIn
{
    /// <summary>
    /// Checks if the attack speed is within limits.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="skill">The skill which is used for the attack, or <c>null</c> for a regular hit.</param>
    /// <param name="eventArgs">The event args to mark a violation.</param>
    ValueTask AttackCheatCheckAsync(Player player, Skill? skill, SpeedHackCheckEventArgs eventArgs);
}
