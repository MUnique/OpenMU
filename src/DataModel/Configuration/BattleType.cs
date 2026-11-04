// <copyright file="BattleType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines the type of battle which can be done on a battle zone between two teams.
/// </summary>
public enum BattleType
{
    /// <summary>
    /// A normal pvp battle.
    /// </summary>
    Normal,

    /// <summary>
    /// A battle soccer match.
    /// </summary>
    Soccer,
}