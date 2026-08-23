// <copyright file="IIllusionTempleHolyItemRelicsViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation announces the player who just picked up the holy relic of
/// an illusion temple event.
/// </summary>
public interface IIllusionTempleHolyItemRelicsViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Announces the player who just picked up the holy relic.
    /// </summary>
    /// <param name="playerId">The id of the player who picked up the relic.</param>
    /// <param name="playerName">The name of the player who picked up the relic.</param>
    ValueTask ShowHolyItemRelicsAsync(ushort playerId, string playerName);
}
