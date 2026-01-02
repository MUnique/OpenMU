// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

using System.Collections.Immutable;

/// <summary>
/// Class which holds the script exports of this project.
/// </summary>
/// <remarks>
/// TODO: Instead of a static class, create an interface, so we can inject an instance into the layout.
///       For example, we could further add some common Components which render the Scripts, Stylesheets, etc.
/// </remarks>
public static class Exports
{
    /// <summary>
    /// Gets the url prefix to the scripts of this project.
    /// </summary>
    private static string Prefix { get; } = $"_content/{typeof(Exports).Namespace}";

    /// <summary>
    /// Gets the scripts.
    /// </summary>
    public static ImmutableList<string> Scripts { get; } = SharedScripts.ToImmutableList();

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    public static ImmutableList<(string Key, string Path)> ScriptMappings { get; } = [];

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    public static ImmutableList<string> Stylesheets { get; } = SharedStylesheets.ToImmutableList();

    private static IEnumerable<string> SharedScripts
    {
        get
        {
            yield return "_content/Blazored.Typeahead/blazored-typeahead.js";
            yield return "_content/BlazorInputFile/inputfile.js";
        }
    }

    private static IEnumerable<string> SharedStylesheets
    {
        get
        {
            yield return "_content/Blazored.Typeahead/blazored-typeahead.css";
            yield return $"{Prefix}/css/shared.css";
        }
    }
}