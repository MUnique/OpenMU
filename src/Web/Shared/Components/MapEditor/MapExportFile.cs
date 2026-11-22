// <copyright file="MapExportFile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Root of a map export file.
/// </summary>
/// <remarks>
/// A single-map export and an all-maps export use the same structure. A single-map file
/// simply contains one entry in <see cref="Maps"/>.
/// </remarks>
public sealed record MapExportFile
{
    /// <summary>
    /// Gets the current format version written by the exporter.
    /// </summary>
    public static string CurrentFormatVersion { get; } = "2.0";

    /// <summary>
    /// Gets or sets the format version of the file.
    /// </summary>
    public string FormatVersion { get; set; } = CurrentFormatVersion;

    /// <summary>
    /// Gets or sets the kinds which are declared by the file.
    /// </summary>
    /// <remarks>
    /// Import only touches kinds which are both declared here and selected by the user.
    /// </remarks>
    public ICollection<MapObjectKind> Kinds { get; set; } = new List<MapObjectKind>();

    /// <summary>
    /// Gets or sets the maps of the file.
    /// </summary>
    public ICollection<MapExport> Maps { get; set; } = new List<MapExport>();
}
