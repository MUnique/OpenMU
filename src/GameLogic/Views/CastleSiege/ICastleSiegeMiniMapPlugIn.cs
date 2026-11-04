// <copyright file="ICastleSiegeMiniMapPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which shows Castle Siege mini-map positions to an alliance master.
/// </summary>
public interface ICastleSiegeMiniMapPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the positions of same-side players on the mini map.
    /// </summary>
    /// <param name="players">The player positions.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowPlayerPositionsAsync(IReadOnlyList<CastleSiegeMiniMapPlayerInfo> players);

    /// <summary>
    /// Shows the positions of alive gates and Guardian Statues on the mini map.
    /// </summary>
    /// <param name="npcs">The NPC positions.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowNpcPositionsAsync(IReadOnlyList<CastleSiegeMiniMapNpcInfo> npcs);
}
