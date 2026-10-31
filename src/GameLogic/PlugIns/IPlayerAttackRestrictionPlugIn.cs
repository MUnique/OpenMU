// <copyright file="IPlayerAttackRestrictionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which can prevent a player from damaging another player,
/// e.g. between the players of the same side of a Castle Siege battle.
/// </summary>
[Guid("9130ECFA-9916-4074-92DC-5AEA17BC4F1A")]
[PlugInPoint("Player attack restriction", "Plugins which can prevent a player from damaging another player.")]
public interface IPlayerAttackRestrictionPlugIn
{
    /// <summary>
    /// Checks if the attacker may damage the defender.
    /// </summary>
    /// <param name="attacker">The attacking player, or the owner of the attacking pet.</param>
    /// <param name="defender">The attacked player.</param>
    /// <param name="eventArgs">The event args. A plugin sets <see cref="CancelEventArgs.Cancel"/> to prevent the attack.</param>
    void CheckAttack(Player attacker, Player defender, CancelEventArgs eventArgs);
}
