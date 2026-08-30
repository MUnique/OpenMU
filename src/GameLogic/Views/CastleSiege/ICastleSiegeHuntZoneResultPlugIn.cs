// <copyright file="ICastleSiegeHuntZoneResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which reports Land of Trials management and entrance results.
/// </summary>
public interface ICastleSiegeHuntZoneResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of changing public Land of Trials access.
    /// </summary>
    /// <param name="result">The request result.</param>
    /// <param name="isPublic">Whether public access is enabled.</param>
    ValueTask ShowEntranceSettingResultAsync(CastleSiegeRequestResult result, bool isPublic);

    /// <summary>
    /// Shows the result of a Land of Trials entrance request.
    /// </summary>
    /// <param name="success">Whether the request succeeded.</param>
    ValueTask ShowEnterResultAsync(bool success);
}
