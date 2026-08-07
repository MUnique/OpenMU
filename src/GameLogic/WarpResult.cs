// <copyright file="WarpResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Describes the outcome of a requested map warp.
/// </summary>
public enum WarpResult
{
    /// <summary>The map change was accepted.</summary>
    Success,

    /// <summary>The player is already waiting for a map change.</summary>
    AlreadyChangingMap,

    /// <summary>The target gate or terrain destination is invalid.</summary>
    InvalidGate,

    /// <summary>The player does not have enough money for the warp.</summary>
    InsufficientMoney,

    /// <summary>The map change failed and was rolled back.</summary>
    Failed,
}
