// <copyright file="IMuHelperStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.MuHelper;

using MUnique.OpenMU.GameLogic.MuHelper;

/// <summary>
/// Interface of a view whose implementation toggles the MU Helper status.
/// </summary>
public interface IMuHelperStatusUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the MU Helper status.
    /// </summary>
    /// <param name="status">The desired status.</param>
    /// <param name="money">Cost of the helper for the current usage. 0, if the MU Helper is currently inactive.</param>
    ValueTask UpdateStatusAsync(MuHelperStatus status, uint money = 0);
}