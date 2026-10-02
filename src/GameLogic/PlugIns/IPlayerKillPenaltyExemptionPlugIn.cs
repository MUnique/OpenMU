// <copyright file="IPlayerKillPenaltyExemptionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which can exempt the attacks and kills between two players from the player killer penalty,
/// e.g. between the members of different gens in a battle zone.
/// An exempted attack doesn't start the self-defense, and an exempted kill doesn't make the killer an outlaw.
/// </summary>
[Guid("5E2B8C41-7D93-4A06-B1F8-3C6A9E0D4B72")]
[PlugInPoint("Player kill penalty exemption", "Plugins which can exempt the attacks and kills between two players from the player killer penalty and the self-defense.")]
public interface IPlayerKillPenaltyExemptionPlugIn
{
    /// <summary>
    /// Checks if the attacks and kills of the attacker on the defender are exempted from the player killer penalty.
    /// </summary>
    /// <param name="attacker">The attacking player.</param>
    /// <param name="defender">The attacked player.</param>
    /// <param name="eventArgs">The event args. A plugin sets <see cref="PlayerKillPenaltyExemptionArgs.IsExempted"/> to exempt the attack.</param>
    void CheckExemption(Player attacker, Player defender, PlayerKillPenaltyExemptionArgs eventArgs);
}
