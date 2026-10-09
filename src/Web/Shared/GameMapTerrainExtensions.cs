// <copyright file="GameMapTerrainExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for the <see cref="GameMapTerrain"/>.
/// </summary>
public static class GameMapTerrainExtensions
{
    private const int SideLength = 0x100;

    private static readonly (byte R, byte G, byte B) WalkableColor = (80, 200, 80);

    private static readonly (byte R, byte G, byte B) SafezoneColor = (128, 128, 128);

    /// <summary>
    /// Renders the terrain into a PNG image.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <returns>The PNG encoded image.</returns>
    public static byte[] ToPng(this GameMapTerrain terrain)
    {
        var pixels = new byte[SideLength * SideLength * 3];
        var index = 0;
        for (int y = 0; y < SideLength; y++)
        {
            for (int x = 0; x < SideLength; x++)
            {
                (byte r, byte g, byte b) = (0, 0, 0);
                if (terrain.SafezoneMap[y, x])
                {
                    (r, g, b) = SafezoneColor;
                }
                else if (terrain.WalkMap[y, x])
                {
                    (r, g, b) = WalkableColor;
                }
                else
                {
                    // we use the default color (black).
                }

                pixels[index++] = r;
                pixels[index++] = g;
                pixels[index++] = b;
            }
        }

        return SimplePngEncoder.EncodeRgb(pixels, SideLength, SideLength);
    }

    /// <summary>
    /// Renders the terrain into a PNG image and returns it as data url,
    /// which can be directly used as source of an html img-element.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <returns>The data url of the PNG image.</returns>
    public static string ToPngDataUrl(this GameMapTerrain terrain)
    {
        return "data:image/png;base64," + Convert.ToBase64String(terrain.ToPng());
    }
}
