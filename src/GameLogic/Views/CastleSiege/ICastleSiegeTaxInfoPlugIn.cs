// <copyright file="ICastleSiegeTaxInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows the current Castle Siege tax configuration.
/// </summary>
public interface ICastleSiegeTaxInfoPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the current tax rates and visible treasury balance.
    /// </summary>
    /// <param name="result">The request result.</param>
    /// <param name="chaosTax">The Chaos Machine tax percentage.</param>
    /// <param name="storeTax">The NPC store tax percentage.</param>
    /// <param name="tributeMoney">The visible treasury balance.</param>
    ValueTask ShowTaxInfoAsync(
        CastleSiegeRequestResult result,
        byte chaosTax,
        byte storeTax,
        long tributeMoney);
}
