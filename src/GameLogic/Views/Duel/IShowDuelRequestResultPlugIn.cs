// <copyright file="IShowDuelRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a duel request result.
/// </summary>
public interface IShowDuelRequestResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// The requested player has answered the request, or the duel couldn't be started.
    /// </summary>
    /// <param name="result">if set to <c>true</c> the trade has been accepted and will be opened.</param>
    /// <param name="opponent">The opponent.</param>
    ValueTask ShowDuelRequestResultAsync(DuelStartResult result, Player opponent);
}