// <copyright file="ICastleSiegeTributeWithdrawResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which reports a Castle Siege treasury withdrawal result.
/// </summary>
public interface ICastleSiegeTributeWithdrawResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the treasury withdrawal result.
    /// </summary>
    /// <param name="result">The request result.</param>
    /// <param name="amount">The withdrawn amount, or zero on failure.</param>
    ValueTask ShowTributeWithdrawResultAsync(CastleSiegeRequestResult result, long amount);
}
