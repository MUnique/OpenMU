// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using System.Collections.Immutable;

/// <summary>
/// Class which holds the script exports of this project.
/// </summary>
public static class Exports
{
    /// <summary>
    /// Gets the url prefix to the scripts of this project.
    /// </summary>
    public static string Prefix { get; } = $"_content/{typeof(Exports).Namespace}";

    /// <summary>
    /// Gets the scripts.
    /// </summary>
    public static ImmutableList<string> Scripts { get; } = new[]
    {
        $"{Prefix}/js/system-production.js",
        $"{Prefix}/js/map.js",
        $"{Prefix}/js/app.js",
        $"{Prefix}/js/Stats.js",
    }.ToImmutableList();

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    public static ImmutableList<(string Key, string Path)> ScriptMappings { get; } = new (string Key, string Path)[]
    {
        ("three", $"{Prefix}/js/three.min.js"),
        ("tween", $"{Prefix}/js/tween.min.js"),
    }.ToImmutableList();

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    public static ImmutableList<string> Stylesheets { get; } = [];
}