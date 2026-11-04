// <copyright file="CastleSiegeCrownAccessState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Defines the state of a Castle Siege Crown capture attempt.
/// </summary>
public enum CastleSiegeCrownAccessState : byte
{
    /// <summary>
    /// The player is actively operating the Crown.
    /// </summary>
    Attempt = 0,

    /// <summary>
    /// The player captured the Crown successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The player's Crown operation was interrupted.
    /// </summary>
    Fail = 2,
}
