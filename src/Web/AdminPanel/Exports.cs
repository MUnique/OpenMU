// <copyright file="Exports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.Collections.Immutable;

/// <summary>
/// Class which holds the script exports of this project.
/// </summary>
public class Exports : IExports
{
    private readonly Lazy<ImmutableList<string>> _scripts;
    private readonly Lazy<ImmutableList<(string Key, string Path)>> _scriptMappings;
    private readonly Lazy<ImmutableList<string>> _stylesheets;

    /// <summary>
    /// Initializes a new instance of the <see cref="Exports"/> class.
    /// </summary>
    public Exports()
    {
        this._scripts = new Lazy<ImmutableList<string>>(() =>
        {
            var adminPanelScripts = this.AdminPanelScripts.ToList();
            return AdminPanelEnvironment.IsHostingEmbedded
                ? Web.Map.Exports.Scripts.Concat(adminPanelScripts).ToImmutableList()
                : adminPanelScripts.ToImmutableList();
        });

        this._scriptMappings = new Lazy<ImmutableList<(string Key, string Path)>>(() =>
        {
            return AdminPanelEnvironment.IsHostingEmbedded
                ? Web.Map.Exports.ScriptMappings.ToImmutableList()
                : ImmutableList<(string Key, string Path)>.Empty;
        });

        this._stylesheets = new Lazy<ImmutableList<string>>(() =>
        {
            var adminPanelStylesheets = this.AdminPanelStylesheets.ToList();
            return AdminPanelEnvironment.IsHostingEmbedded
                ? Web.Map.Exports.Stylesheets.Concat(adminPanelStylesheets).ToImmutableList()
                : adminPanelStylesheets.ToImmutableList();
        });
    }

    /// <summary>
    /// Gets the url prefix to the scripts of this project.
    /// </summary>
    private string Prefix { get; } = $"_content/{typeof(Exports).Namespace}";

    private IEnumerable<string> AdminPanelScripts
    {
        get
        {
            yield return "_content/Blazored.Typeahead/blazored-typeahead.js";
            yield return "_content/Blazored.Modal/blazored.modal.js";
            yield return "_content/BlazorInputFile/inputfile.js";
            yield return $"{this.Prefix}/js/map.js";
        }
    }

    private IEnumerable<string> AdminPanelStylesheets
    {
        get
        {
            yield return "_content/Blazored.Typeahead/blazored-typeahead.css";
            yield return "_content/Blazored.Modal/blazored-modal.css";
            yield return $"{this.Prefix}/css/site.css";
        }
    }

    /// <inheritdoc/>
    public ImmutableList<string> Scripts => this._scripts.Value;

    /// <inheritdoc/>
    public ImmutableList<(string Key, string Path)> ScriptMappings => this._scriptMappings.Value;

    /// <inheritdoc/>
    public ImmutableList<string> Stylesheets => this._stylesheets.Value;
}