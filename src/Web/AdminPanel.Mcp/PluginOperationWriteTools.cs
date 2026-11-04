// <copyright file="PluginOperationWriteTools.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Thin adapters for active plugin-contributed administration operations.</summary>
[McpServerToolType]
public sealed class PluginOperationWriteTools
{
    private PluginOperationWriteTools()
    {
    }

    /// <summary>Preview or execute an owner-allowed plugin write with its declared minimum role and durable audit. Request an identical preview before applying. Inspect state before retrying uncertain outcomes.</summary>
    /// <param name="operations">The shared plugin operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="serverId">Actual game server identifier.</param>
    /// <param name="plugInId">Plugin type identifier from plugin_operations_list.</param>
    /// <param name="name">Exact operation name from the catalog.</param>
    /// <param name="arguments">JSON object matching the contributed input schema.</param>
    /// <param name="apply">Apply only after approving the preview.</param>
    /// <param name="confirmationToken">Single-use token from the identical preview.</param>
    /// <returns>The safe catalog, result or preview.</returns>
    [McpServerTool(Name = "plugin_operations_execute", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Preview or execute an owner-allowed plugin write with its declared minimum role and durable audit. Request an identical preview before applying. Inspect state before retrying uncertain outcomes.")]
    [Authorize(Roles = "Operator")]
    [ExposesOperation(typeof(PluginAdminOperations), nameof(PluginAdminOperations.ExecuteAsync))]
    public static ValueTask<JsonObject> ExecuteAsync(
        [FromServices] PluginAdminOperations operations,
        CancellationToken cancellationToken,
        [Description("Actual game server identifier.")] int serverId,
        [Description("Plugin type identifier from plugin_operations_list.")] Guid plugInId,
        [Description("Exact operation name from the catalog.")] string name,
        [Description("JSON object matching the contributed input schema.")] string arguments,
        [Description("Apply only after approving the preview.")] bool apply = false,
        [Description("Single-use token from the identical preview.")] string? confirmationToken = null)
        => AdminToolExecution.ExecuteAsync(() => operations.ExecuteAsync(serverId, plugInId, name, arguments, apply, confirmationToken, cancellationToken));
}
