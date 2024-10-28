// <copyright file="IDuelHealthUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the health update of the duel players.
/// </summary>
public interface IDuelHealthUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the health of both players.
    /// </summary>
    /// <param name="duelRoom">The duel room.</param>
    ValueTask UpdateHealthAsync(DuelRoom duelRoom);
}