// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

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
    public static string Prefix { get; } = $"_content/{typeof(Exports).Namespace}";

    /// <summary>
    /// Gets the scripts.
    /// </summary>
    public static ImmutableList<string> Scripts { get; } = new[]
    {
        "_content/Blazored.Typeahead/blazored-typeahead.js",
        "_content/Blazored.Modal/blazored.modal.js",
        "_content/BlazorInputFile/inputfile.js",
    }.ToImmutableList();

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    public static ImmutableList<(string Key, string Path)> ScriptMappings { get; } = new (string Key, string Path)[]
    {
    }.ToImmutableList();

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    public static ImmutableList<string> Stylesheets { get; } = new[]
    {
        "_content/Blazored.Typeahead/blazored-typeahead.css",
        "_content/Blazored.Modal/blazored-modal.css",
        $"{Prefix}/css/site.css",
    }.ToImmutableList();
}