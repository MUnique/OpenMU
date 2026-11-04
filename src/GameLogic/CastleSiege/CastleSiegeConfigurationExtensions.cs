// <copyright file="CastleSiegeConfigurationExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;

/// <summary>
/// Provides access to Castle Siege structure-upgrade configuration.
/// </summary>
internal static class CastleSiegeConfigurationExtensions
{
    /// <summary>
    /// Gets the upgrade definitions for a structure and upgrade type.
    /// </summary>
    /// <param name="configuration">The Castle Siege configuration.</param>
    /// <param name="monsterNumber">The structure monster number.</param>
    /// <param name="upgradeType">The upgrade type.</param>
    /// <returns>The matching definitions, or <see langword="null"/> when the combination is unsupported.</returns>
    internal static ICollection<CastleSiegeUpgradeDefinition>? GetUpgrades(
        this CastleSiegeConfiguration configuration,
        short monsterNumber,
        CastleSiegeUpgradeType upgradeType)
    {
        return (monsterNumber, upgradeType) switch
        {
            var (number, type) when number == CastleSiegeGate.MonsterNumber
                                    && type == CastleSiegeUpgradeType.Defense => configuration.GateDefenseUpgrades,
            var (number, type) when number == CastleSiegeGate.MonsterNumber
                                    && type == CastleSiegeUpgradeType.Life => configuration.GateLifeUpgrades,
            var (number, type) when number == CastleSiegeStatue.MonsterNumber
                                    && type == CastleSiegeUpgradeType.Defense => configuration.StatueDefenseUpgrades,
            var (number, type) when number == CastleSiegeStatue.MonsterNumber
                                    && type == CastleSiegeUpgradeType.Life => configuration.StatueLifeUpgrades,
            var (number, type) when number == CastleSiegeStatue.MonsterNumber
                                    && type == CastleSiegeUpgradeType.Regen => configuration.StatueRegenUpgrades,
            _ => null,
        };
    }

    /// <summary>
    /// Gets the configured value of an upgrade level.
    /// </summary>
    /// <param name="definitions">The upgrade definitions.</param>
    /// <param name="level">The upgrade level.</param>
    /// <returns>The configured value, or <see langword="null"/> when the level is missing.</returns>
    internal static int? GetValue(this IEnumerable<CastleSiegeUpgradeDefinition> definitions, byte level)
    {
        return definitions.FirstOrDefault(definition => definition.Level == level)?.Value;
    }
}
