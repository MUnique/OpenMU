// <copyright file="GameMapTerrainExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using MUnique.OpenMU.GameLogic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Extensions for the <see cref="GameMapTerrain"/>.
/// TODO: It's duplicated. Move to a common project.
/// </summary>
public static class GameMapTerrainExtensions
{
    /// <summary>
    /// Renders the terrain into an image.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <returns>The rendered image.</returns>
    public static Image<Rgba32> ToImage(this GameMapTerrain terrain)
    {
        var bitmap = new Image<Rgba32>(0x100, 0x100);
        for (int y = 0; y < 0x100; y++)
        {
            for (int x = 0; x < 0x100; x++)
            {
                var color = Rgba32.Black;
                if (terrain.SafezoneMap[y, x])
                {
                    color = Rgba32.Gray;
                }
                else if (terrain.WalkMap[y, x])
                {
                    color = Rgba32.SpringGreen;
                }
                else
                {
                    // we use the default color.
                }

                bitmap[x, y] = color;
            }
        }

        return bitmap;
    }
}