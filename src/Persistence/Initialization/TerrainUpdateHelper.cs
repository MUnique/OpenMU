// <copyright file="TerrainUpdateHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Update helper for terrain data.
/// </summary>
internal static class TerrainUpdateHelper
{
    /// <summary>
    /// Updates the <see cref="GameMapDefinition.TerrainData"/> from the embedded resources.
    /// </summary>
    /// <param name="gameMapDefinition">The game map definition.</param>
    /// <param name="terrainVersionPrefix">The terrain version prefix.</param>
    public static void UpdateTerrainFromResources(this GameMapDefinition gameMapDefinition, string terrainVersionPrefix = "")
    {
        var assembly = Assembly.GetExecutingAssembly();
        var terrainResourceName = gameMapDefinition.GetTerrainFileName(terrainVersionPrefix);
        if (string.IsNullOrWhiteSpace(terrainResourceName))
        {
            return;
        }

        using var stream = assembly.GetManifestResourceStream(terrainResourceName);
        if (stream is not null)
        {
            using var reader = new BinaryReader(stream);
            var terrainData = reader.ReadBytes(3 * ushort.MaxValue);
            gameMapDefinition.TerrainData = terrainData;
        }
        else
        {
            Debug.Fail($"Couldn't find terrain resource {terrainResourceName} for map {gameMapDefinition.Name}.");
        }
    }

    private static string GetTerrainFileName(this GameMapDefinition gameMapDefinition, string terrainVersionPrefix = "")
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        for (var mapNumber = gameMapDefinition.Number + 1; mapNumber > 0 && mapNumber > gameMapDefinition.Number - 10; mapNumber--)
        {
            var candidate = $"{assembly.GetName().Name}.Resources.{terrainVersionPrefix}Terrain{mapNumber}{(gameMapDefinition.Discriminator > 0 ? ("_" + gameMapDefinition.Discriminator) : string.Empty)}.att";
            if (resourceNames.Contains(candidate))
            {
                return candidate;
            }

            if (gameMapDefinition.Discriminator > 0)
            {
                var candidate2 = $"{assembly.GetName().Name}.Resources.{terrainVersionPrefix}Terrain{mapNumber}.att";
                if (resourceNames.Contains(candidate2))
                {
                    return candidate2;
                }
            }

            if (!char.IsDigit(gameMapDefinition.Name[^1]))
            {
                break;
            }
        }

        Debug.Fail("Couldn't find terrain resource name for map " + gameMapDefinition.Name);
        return string.Empty;
    }
}