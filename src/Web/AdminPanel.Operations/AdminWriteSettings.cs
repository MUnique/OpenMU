// <copyright file="AdminWriteSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Shared safety limits for administrative writes.</summary>
public class AdminWriteSettings
{
    /// <summary>Gets or sets a value indicating whether MCP exposes only read tools. Defaults to true.</summary>
    public bool ReadOnly { get; set; } = true;

    /// <summary>Gets or sets the maximum number of targets in a single operational request.</summary>
    public int MaxBulkSize { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum length, in characters, of a plugin configuration saved by the panel's graphical editor.
    /// </summary>
    /// <remarks>
    /// The panel always saves the whole configuration, so plugins with large text lists need more than a scalar patch.
    /// The editor serializes a typed model on the server, which is why the default is far above the MCP request limit.
    /// </remarks>
    public int MaxPanelPluginConfigurationLength { get; set; } = 1024 * 1024;

    /// <summary>Gets or sets exact write operation names explicitly allowed by the owner, keyed by plugin type GUID. Empty denies all plugin writes.</summary>
    public Dictionary<string, string[]> AllowedPluginWrites { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
