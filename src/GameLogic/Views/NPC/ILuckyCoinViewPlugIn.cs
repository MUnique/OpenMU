// <copyright file="ILuckyCoinViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// A view plugin which shows the results of the lucky coin dialogs of the NPC Delgado.
/// </summary>
public interface ILuckyCoinViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the count of the lucky coins which the character registered.
    /// </summary>
    /// <param name="count">The count of the registered lucky coins.</param>
    ValueTask ShowRegisteredCountAsync(int count);

    /// <summary>
    /// Shows the result of a lucky coin registration.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="count">The count of the registered lucky coins after the registration.</param>
    ValueTask ShowRegistrationResultAsync(LuckyCoinRegistrationResult result, int count);

    /// <summary>
    /// Shows the result of a lucky coin exchange.
    /// </summary>
    /// <param name="result">The result.</param>
    ValueTask ShowExchangeResultAsync(LuckyCoinExchangeResult result);
}
