// <copyright file="MapExportFileReader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Text.Json;

/// <summary>
/// Reads map export files of the current format version.
/// </summary>
/// <remarks>
/// Files of older format versions are intentionally not supported. They have to be converted
/// to the current format before they can be imported.
/// </remarks>
public sealed class MapExportFileReader
{
    /// <summary>
    /// Tries to read the given JSON content.
    /// </summary>
    /// <param name="json">The JSON content.</param>
    /// <param name="file">The read file, if successful.</param>
    /// <param name="error">The reason why the file could not be read, if unsuccessful.</param>
    /// <returns><see langword="true"/> if the file could be read; otherwise, <see langword="false"/>.</returns>
    public bool TryRead(string json, out MapExportFile file, out MapImportError error)
    {
        file = null!;
        error = default;

        string? formatVersion;
        try
        {
            formatVersion = GetFormatVersion(json);
        }
        catch (JsonException)
        {
            error = MapImportError.InvalidJson;
            return false;
        }

        if (formatVersion is null)
        {
            error = MapImportError.InvalidJson;
            return false;
        }

        if (formatVersion != MapExportFile.CurrentFormatVersion)
        {
            error = MapImportError.UnsupportedFormatVersion;
            return false;
        }

        try
        {
            return TryReadCurrent(json, out file, out error);
        }
        catch (JsonException)
        {
            error = MapImportError.InvalidJson;
            return false;
        }
    }

    private static string? GetFormatVersion(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (string.Equals(property.Name, "formatVersion", StringComparison.OrdinalIgnoreCase)
                && property.Value.ValueKind == JsonValueKind.String)
            {
                return property.Value.GetString();
            }
        }

        return null;
    }

    private static bool TryReadCurrent(string json, out MapExportFile file, out MapImportError error)
    {
        file = null!;
        error = default;

        var readFile = JsonSerializer.Deserialize<MapExportFile>(json, MapExportJson.Options);
        if (readFile is null)
        {
            error = MapImportError.InvalidJson;
            return false;
        }

        if (readFile.Maps.Count == 0)
        {
            error = MapImportError.NoMaps;
            return false;
        }

        if (readFile.Kinds.Count == 0)
        {
            error = MapImportError.NoKinds;
            return false;
        }

        file = readFile;
        return true;
    }
}
