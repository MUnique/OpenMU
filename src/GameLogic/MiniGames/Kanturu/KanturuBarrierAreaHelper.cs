// <copyright file="KanturuBarrierAreaHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Enumerates the terrain cells of the Elphis barrier areas.
/// </summary>
internal static class KanturuBarrierAreaHelper
{
    /// <summary>
    /// Enumerates every cell covered by the given areas, inclusive.
    /// </summary>
    /// <param name="areas">The terrain areas to expand.</param>
    /// <returns>Every cell covered by the areas.</returns>
    public static IEnumerable<(byte X, byte Y)> EnumerateCells(IEnumerable<KanturuTerrainArea> areas)
    {
        // int loop variables: byte would wrap 255 -> 0 and loop forever
        // when an area touches the map border.
        foreach (var area in areas)
        {
            for (var x = (int)area.StartX; x <= area.EndX; x++)
            {
                for (var y = (int)area.StartY; y <= area.EndY; y++)
                {
                    yield return ((byte)x, (byte)y);
                }
            }
        }
    }
}
