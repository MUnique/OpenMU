// <copyright file="IShowCastleSiegeRegistrationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;

/// <summary>
/// Interface of a view whose implementation shows the result of a castle siege registration.
/// </summary>
public interface IShowCastleSiegeRegistrationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of a castle siege registration operation.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowResultAsync(CastleSiegeRegistrationResult result);
}
