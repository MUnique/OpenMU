// <copyright file="IGuildWarScoreUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation informs about a guild war score update.
/// </summary>
public interface IGuildWarScoreUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the score.
    /// </summary>
    ValueTask UpdateScoreAsync();
}