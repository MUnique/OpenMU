using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MUnique.OpenMU.GameLogic;
using SixLabors.ImageSharp;

namespace MUnique.OpenMU.Web.Map.Map;

public static class GameMapInfoExtensions
{
    private static ConcurrentDictionary<short, string> _mapTerrainsBase64 = new ();

    private static ConcurrentDictionary<short, byte[]> _mapTerrainsPng = new ();

    public static Stream GetTerrainStream(this IGameMapInfo map)
    {
        if (_mapTerrainsPng.TryGetValue(map.MapNumber, out var data))
        {
            return new MemoryStream(data);
        }

        data = RenderTerrain(map);
        _mapTerrainsPng.TryAdd(map.MapNumber, data);

        return new MemoryStream(data);
    }

    public static string GetTerrainString(this IGameMapInfo map)
    {
        if (_mapTerrainsBase64.TryGetValue(map.MapNumber, out var base64String))
        {
            return base64String;
        }

        if (!_mapTerrainsPng.TryGetValue(map.MapNumber, out var rawData))
        {
            rawData = RenderTerrain(map);
            _mapTerrainsPng.TryAdd(map.MapNumber, rawData);
        }

        base64String = "data:image/png;base64," + Convert.ToBase64String(rawData);
        _mapTerrainsBase64.TryAdd(map.MapNumber, base64String);
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