// <copyright file="IGensViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Gens;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The view plugin for the gens system of the own player.
/// </summary>
public interface IGensViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of a request to join a gens.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="gens">The gens which the player requested to join.</param>
    ValueTask ShowJoinResultAsync(GensJoinResult result, GensType gens);

    /// <summary>
    /// Shows the result of a request to leave a gens.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowLeaveResultAsync(GensLeaveResult result);

    /// <summary>
    /// Shows the gens, rank and contribution of the own player.
    /// </summary>
    ValueTask ShowGensInfoAsync();
}
