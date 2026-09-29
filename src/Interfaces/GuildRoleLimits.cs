// <copyright file="GuildRoleLimits.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Role limits for guild position assignments: at most one assistant master,
/// and battle masters limited by the guild master's level.
/// </summary>
public static class GuildRoleLimits
{
    /// <summary>
    /// The combined guild master levels granting one additional battle master.
    /// </summary>
    public const int LevelsPerBattleMaster = 200;

    /// <summary>
    /// The battle masters a guild may have regardless of the guild master's level.
    /// </summary>
    public const int BaseBattleMasterCount = 1;

    /// <summary>
    /// Gets the maximum number of battle masters for the given combined level of the guild master.
    /// </summary>
    /// <param name="masterTotalLevel">The combined normal and master level of the guild master.</param>
    /// <returns>The maximum number of battle masters.</returns>
    public static int MaxBattleMasterCount(int masterTotalLevel)
    {
        return (masterTotalLevel / LevelsPerBattleMaster) + BaseBattleMasterCount;
    }
}
