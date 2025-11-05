// <copyright file="IExports.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.Collections.Immutable;

/// <summary>
/// Interface for exports of scripts and stylesheets for the admin panel.
/// </summary>
public interface IExports
{
    /// <summary>
    /// Gets the scripts.
    /// </summary>
    ImmutableList<string> Scripts { get; }

    /// <summary>
    /// Gets the script mappings.
    /// </summary>
    ImmutableList<(string Key, string Path)> ScriptMappings { get; }

    /// <summary>
    /// Gets the stylesheets.
    /// </summary>
    ImmutableList<string> Stylesheets { get; }
}
