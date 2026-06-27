// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.Collections.Immutable;

/// <summary>
/// Class that holds the script exports of this project.
/// </summary>
public static class Exports
{
    /// <summary>
    /// Gets the scripts.
    /// </summary>
    public static ImmutableList<string> Scripts { get; } = [
        .. Web.Shared.Exports.Scripts,
        $"{GetPrefix()}/js/system-production.js",
        $"{GetPrefix()}/js/map-launcher.js",
        $"{GetPrefix()}/js/MUnique.OpenMU.Web.Map.js",
        $"{GetPrefix()}/js/Stats.js",
    ];

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    public static ImmutableList<(string Key, string Path)> ScriptMappings { get; } = new (string Key, string Path)[]
    {
        ("three", $"{GetPrefix()}/js/three.min.js"),
        ("tween", $"{GetPrefix()}/js/tween.min.js"),
    }.ToImmutableList();

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    public static ImmutableList<string> Stylesheets { get; } = [
        .. Web.Shared.Exports.Stylesheets,
        $"{GetPrefix()}/MUnique.OpenMU.Web.Map.styles.css",
    ];

    private static string GetPrefix() => $"_content/{typeof(Exports).Namespace}";
}