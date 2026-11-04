// <copyright file="IMuHelperStatusUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.MuHelper;

/// <summary>
/// Interface of a view whose implementation toggles the MU Helper status.
/// </summary>
public interface IMuHelperStatusUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Starts the MU Helper.
    /// </summary>
    ValueTask StartAsync();

    /// <summary>
    /// Stops the MU Helper.
    /// </summary>
    ValueTask StopAsync();

    /// <summary>
    /// Consumes the money during running the helper.
    /// </summary>
    /// <param name="money">Cost of the helper for the current usage.</param>
    ValueTask ConsumeMoneyAsync(uint money);
}