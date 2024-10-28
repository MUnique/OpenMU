// <copyright file="IShowMiniGameEnterResultPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface for view plugins which show the result of entering a mini game event.
/// </summary>
public interface IShowMiniGameEnterResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of an enter request.
    /// </summary>
    /// <param name="miniGameType">The type of the mini game.</param>
    /// <param name="enterResult">The result.</param>
    ValueTask ShowResultAsync(MiniGameType miniGameType, EnterResult enterResult);
}