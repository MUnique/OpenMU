// <copyright file="ICastleSiegeLifeStoneStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// A view which updates the creation state of a Castle Siege Life Stone.
/// </summary>
public interface ICastleSiegeLifeStoneStatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the current Life Stone creation stage.
    /// </summary>
    /// <param name="npcId">The map object identifier of the Life Stone.</param>
    /// <param name="buildTime">The client-visible creation stage.</param>
    /// <returns>A task that represents the notification.</returns>
    ValueTask ShowLifeStoneBuildTimeAsync(ushort npcId, byte buildTime);
}
