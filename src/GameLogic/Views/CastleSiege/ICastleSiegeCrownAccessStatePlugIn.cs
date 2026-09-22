// <copyright file="ICastleSiegeCrownAccessStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which updates a player's Castle Siege Crown capture attempt.
/// </summary>
public interface ICastleSiegeCrownAccessStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the Crown access state and accumulated capture time.
    /// </summary>
    /// <param name="state">The access state.</param>
    /// <param name="accumulatedTime">The accumulated capture time.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowCrownAccessStateAsync(
        CastleSiegeCrownAccessState state,
        TimeSpan accumulatedTime);
}
