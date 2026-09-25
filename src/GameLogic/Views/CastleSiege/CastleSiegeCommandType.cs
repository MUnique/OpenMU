// <copyright file="CastleSiegeCommandType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A directional command an alliance master can issue to same-side players.
/// </summary>
public enum CastleSiegeCommandType : byte
{
    /// <summary>Orders the team to attack.</summary>
    Attack = 0,

    /// <summary>Orders the team to defend.</summary>
    Defend = 1,

    /// <summary>Orders the team to wait.</summary>
    Wait = 2,
}
