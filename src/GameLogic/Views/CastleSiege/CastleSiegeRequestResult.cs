// <copyright file="CastleSiegeRequestResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Defines the common result codes of Castle Siege management requests.
/// </summary>
public enum CastleSiegeRequestResult : byte
{
    /// <summary>
    /// The request failed.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The request succeeded.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The player is not authorized to perform the request.
    /// </summary>
    NotAuthorized = 2,
}
