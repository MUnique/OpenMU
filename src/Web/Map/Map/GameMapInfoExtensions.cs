// <copyright file="GameMapInfoExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map.Map;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic;
using SixLabors.ImageSharp;

/// <summary>
/// Extensions for the <see cref="IGameMapInfo"/>.
/// </summary>
public static class GameMapInfoExtensions
{
    private static readonly ConcurrentDictionary<short, string> CachedMapTerrainsBase64 = new();

    private static readonly ConcurrentDictionary<short, byte[]> CachedMapTerrainsPng = new();

    /// <summary>
    /// Gets the terrain stream of the map image.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns>The stream with the terrain as image.</returns>
    public static Stream GetTerrainStream(this IGameMapInfo map)
    {
        if (CachedMapTerrainsPng.TryGetValue(map.MapNumber, out var data))
        {
            return new MemoryStream(data);
        }

        data = RenderTerrain(map);
        CachedMapTerrainsPng.TryAdd(map.MapNumber, data);

        return new MemoryStream(data);
    }

    /// <summary>
    /// Gets the terrain image as base64 string, which can be directly embedded into an html img-element.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns>The terrain image as base64 string, which can be directly embedded into an html img-element.</returns>
    public static string GetTerrainString(this IGameMapInfo map)
    {
        if (CachedMapTerrainsBase64.TryGetValue(map.MapNumber, out var base64String))
        {
            return base64String;
        }

        if (!CachedMapTerrainsPng.TryGetValue(map.MapNumber, out var rawData))
        {
            rawData = RenderTerrain(map);
            CachedMapTerrainsPng.TryAdd(map.MapNumber, rawData);
        }

        base64String = "data:image/png;base64," + Convert.ToBase64String(rawData);
        CachedMapTerrainsBase64.TryAdd(map.MapNumber, base64String);
        return base64String;
    }

    private static byte[] RenderTerrain(IGameMapInfo map)
    {
        var terrain = new GameMapTerrain(map.TerrainData);
        using var bitmap = terrain.ToImage();
        using var memoryStream = new MemoryStream();
        bitmap.SaveAsPng(memoryStream);
        memoryStream.Position = 0;
        return memoryStream.ToArray();
    }
}