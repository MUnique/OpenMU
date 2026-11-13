// <copyright file="IShowWarpListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Interface of a view whose implementation informs about the warps which the player can select in the warp window.
/// </summary>
/// <remarks>
/// Only clients which can replace the warp list of their data files have an implementation of it.
/// </remarks>
public interface IShowWarpListPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the warps which the player can select in the warp window.
    /// </summary>
    /// <param name="warps">The warps.</param>
    ValueTask ShowWarpListAsync(IReadOnlyCollection<WarpInfo> warps);
}
