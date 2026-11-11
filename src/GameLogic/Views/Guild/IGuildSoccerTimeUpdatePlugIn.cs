// <copyright file="IGuildSoccerTimeUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation shows the remaining time of a battle soccer match.
/// </summary>
public interface IGuildSoccerTimeUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the remaining time of the match.
    /// </summary>
    /// <param name="remainingSeconds">The remaining seconds; 0 while the next goal decides a drawn match.</param>
    ValueTask UpdateSoccerTimeAsync(ushort remainingSeconds);
}
