// <copyright file="MapImportError.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Defines why an import file cannot be used.
/// </summary>
public enum MapImportError
{
    /// <summary>
    /// The content is not valid JSON or not a map export file.
    /// </summary>
    InvalidJson,

    /// <summary>
    /// The format version of the file is not supported.
    /// </summary>
    UnsupportedFormatVersion,

    /// <summary>
    /// The file does not contain any maps.
    /// </summary>
    NoMaps,

    /// <summary>
    /// The file does not declare any object kinds.
    /// </summary>
    NoKinds,

    /// <summary>
    /// A single-map import was requested, but the file does not contain the selected map.
    /// </summary>
    MapNotFound,
}
