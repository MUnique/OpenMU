// <copyright file="AdminMcpOptions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

/// <summary>Settings for the optional read-only MCP endpoint.</summary>
public sealed class AdminMcpOptions : MUnique.OpenMU.Web.AdminPanel.Operations.AdminWriteSettings
{
    /// <summary>Gets or sets a value indicating whether the endpoint is enabled. Defaults to off.</summary>
    public bool Enabled { get; set; }

    /// <summary>Gets or sets the maximum requests per API client in each minute.</summary>
    public int RequestsPerMinute { get; set; } = 60;

    /// <summary>Gets or sets the maximum HTTP request body size in bytes.</summary>
    public long MaxRequestBodyBytes { get; set; } = 65536;
}
