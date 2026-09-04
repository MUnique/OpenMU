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
        return GetTerrainStream(map.MapNumber, map.TerrainData);
    }

    /// <summary>
    /// Gets the terrain stream of the map image.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <returns>The stream with the terrain as image.</returns>
    public static Stream GetTerrainStream(this GameMap map)
    {
        return GetTerrainStream(map.Definition.Number, map.Definition.TerrainData);
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

        base64String = "data:image/png;base64," + Convert.ToBase64String(GetTerrainPng(map.MapNumber, map.TerrainData));
        CachedMapTerrainsBase64.TryAdd(map.MapNumber, base64String);
        return base64String;
    }

    private static Stream GetTerrainStream(short mapNumber, byte[]? terrainData)
    {
        return new MemoryStream(GetTerrainPng(mapNumber, terrainData));
    }

    private static byte[] GetTerrainPng(short mapNumber, byte[]? terrainData)
    {
        if (CachedMapTerrainsPng.TryGetValue(mapNumber, out var data))
        {
            return data;
        }

        data = RenderTerrain(terrainData);
        CachedMapTerrainsPng.TryAdd(mapNumber, data);

        return data;
    }

    private static byte[] RenderTerrain(byte[]? terrainData)
    {
        var terrain = new GameMapTerrain(terrainData);
        using var bitmap = terrain.ToImage();
        using var memoryStream = new MemoryStream();
        bitmap.SaveAsPng(memoryStream);
        memoryStream.Position = 0;
        return memoryStream.ToArray();
    }
}