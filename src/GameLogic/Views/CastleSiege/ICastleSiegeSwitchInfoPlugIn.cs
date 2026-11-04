// <copyright file="ICastleSiegeSwitchInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which updates a Castle Siege Crown switch occupant.
/// </summary>
public interface ICastleSiegeSwitchInfoPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the current state of a Crown switch.
    /// </summary>
    /// <param name="switchInfo">The switch state.</param>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    ValueTask ShowSwitchInfoAsync(CastleSiegeSwitchInfo switchInfo);
}
