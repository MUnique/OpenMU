// <copyright file="IIllusionTempleEventStateViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the state of an illusion temple event.
/// </summary>
public interface IIllusionTempleEventStateViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Changes the state of the illusion temple event at the client.
    /// </summary>
    /// <param name="templeNumber">The number of the temple, from 1 to 6.</param>
    /// <param name="state">The new state of the event.</param>
    ValueTask ChangeEventStateAsync(byte templeNumber, IllusionTempleEventStatus state);
}
