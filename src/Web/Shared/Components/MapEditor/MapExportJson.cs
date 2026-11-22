// <copyright file="MapExportJson.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// The JSON settings of the map export format.
/// </summary>
internal static class MapExportJson
{
    /// <summary>
    /// Gets the settings used to read and write map export files.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };
}
