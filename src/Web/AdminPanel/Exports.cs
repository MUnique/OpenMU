// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.Collections.Immutable;
using System.Linq;
using MUnique.OpenMU.Web.Shared;

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

    private static IEnumerable<string> AdminPanelScripts => [];

    private static IEnumerable<string> AdminPanelStylesheets =>
        Web.Shared.Exports.Stylesheets.Concat([
            $"{Prefix}/MUnique.OpenMU.Web.AdminPanel.styles.css",
        ]);

    /// <summary>
    /// Gets the scripts.
    /// </summary>
    public static ImmutableList<string> Scripts { get; } = AdminPanelEnvironment.IsHostingEmbedded
        ? Web.Map.Exports.Scripts.Concat(AdminPanelScripts).ToImmutableList()
        : AdminPanelScripts.ToImmutableList();

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    public static ImmutableList<(string Key, string Path)> ScriptMappings { get; } = AdminPanelEnvironment.IsHostingEmbedded
        ? Web.Map.Exports.ScriptMappings.ToImmutableList()
        : ImmutableList<(string Key, string Path)>.Empty;

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    public static ImmutableList<string> Stylesheets { get; } = AdminPanelEnvironment.IsHostingEmbedded
        ? Web.Map.Exports.Stylesheets.Concat(AdminPanelStylesheets).Distinct().ToImmutableList()
        : AdminPanelStylesheets.Distinct().ToImmutableList();
}
