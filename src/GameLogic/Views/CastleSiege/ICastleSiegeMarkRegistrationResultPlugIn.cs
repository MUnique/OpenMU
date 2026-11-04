// <copyright file="ICastleSiegeMarkRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which reports Sign of Lord submission results.
/// </summary>
public interface ICastleSiegeMarkRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the mark registration result.
    /// </summary>
    /// <param name="result">The registration result.</param>
    /// <param name="guildName">The registered guild name.</param>
    /// <param name="marks">The updated mark count.</param>
    /// <returns>A task which represents the asynchronous operation.</returns>
    ValueTask ShowMarkRegistrationResultAsync(CastleSiegeMarkRegistrationResult result, string guildName, int marks);
}
