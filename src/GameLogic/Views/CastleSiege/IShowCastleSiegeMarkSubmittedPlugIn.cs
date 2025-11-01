// <copyright file="IShowCastleSiegeMarkSubmittedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Interface of a view whose implementation shows the result of submitting a guild mark for castle siege.
/// </summary>
public interface IShowCastleSiegeMarkSubmittedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of submitting a guild mark.
    /// </summary>
    /// <param name="totalMarksSubmitted">The total number of marks submitted by the alliance.</param>
    ValueTask ShowMarkSubmittedAsync(int totalMarksSubmitted);
}
