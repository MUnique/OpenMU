// <copyright file="IItemRegistrationStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Defines a strategy for item registration specific to an NPC.
/// </summary>
[Guid("182FC652-3277-4CDB-8BA8-DE70311E67C0")]
[PlugInPoint("Item Registration Strategies", "Plugins which implement item registration for specific NPCs.")]
public interface IItemRegistrationStrategy : IStrategyPlugIn<short>
{
    /// <summary>
    /// Gets the NPC definition number this strategy applies to (e.g., 236 for Golden Archer).
    /// </summary>
    short NpcNumber { get; }

    /// <summary>
    /// Gets the target stat attribute which counts the current registered items.
    /// </summary>
    AttributeDefinition? TargetStat { get; }

    /// <summary>
    /// Gets the target stat attribute which counts the total historical registered items.
    /// </summary>
    AttributeDefinition? TargetTotalStat { get; }

    /// <summary>
    /// Opens the item registration dialog for this strategy.
    /// </summary>
    /// <param name="player">The player.</param>
    ValueTask OpenDialogAsync(Player player);

    /// <summary>
    /// Registers the item for the player according to the specified rule.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="rule">The item registration rule.</param>
    ValueTask RegisterAsync(Player player, PlugIns.ItemRegistration.NpcItemRegistrationRule rule);
}