// <copyright file="ICastleSiegeCrownStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which updates the Castle Siege Crown lock state.
/// </summary>
public interface ICastleSiegeCrownStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows whether the Crown is available for capture.
    /// </summary>
    /// <param name="isAvailable">Whether the Crown is unlocked.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowCrownStateAsync(bool isAvailable);
}
