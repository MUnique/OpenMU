// <copyright file="LiveGameWriteTools.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Mcp;

using System.ComponentModel;
using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Operations;

/// <summary>Thin MCP adapters for authorized live game administration.</summary>
[McpServerToolType]
public sealed class LiveGameWriteTools
{
    private LiveGameWriteTools()
    {
    }

    /// <summary>Preview or apply a live teleport to a walkable hosted map. Mini game participants and unstable sessions are refused.</summary>
    /// <param name="operations">The shared live operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="characterName">Exact online character name.</param>
    /// <param name="mapNumber">Hosted target map number.</param>
    /// <param name="x">Target horizontal coordinate.</param>
    /// <param name="y">Target vertical coordinate.</param>
    /// <param name="apply">Apply only after approving the preview.</param>
    /// <param name="confirmationToken">Single-use token from the identical preview.</param>
    /// <returns>The safe live result or preview.</returns>
    [McpServerTool(Name = "players_teleport", ReadOnly = false, Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Preview or apply a live teleport to a walkable hosted map. Mini game participants and unstable sessions are refused.")]
    [Authorize(Roles = "Operator")]
    [ExposesOperation(typeof(LiveGameOperations), nameof(LiveGameOperations.TeleportAsync))]
    public static ValueTask<JsonObject> PlayersTeleportAsync(
        [FromServices] LiveGameOperations operations,
        CancellationToken cancellationToken,
        [Description("Exact online character name.")] string characterName,
        [Description("Hosted target map number.")] ushort mapNumber,
        [Description("Target horizontal coordinate.")] byte x,
        [Description("Target vertical coordinate.")] byte y,
        [Description("Apply only after approving the preview.")] bool apply = false,
        [Description("Single-use token from the identical preview.")] string? confirmationToken = null)
        => AdminToolExecution.ExecuteAsync(() => operations.TeleportAsync(characterName, mapNumber, x, y, apply, confirmationToken, cancellationToken));

    /// <summary>Preview or give one structured item to a live inventory, using the owning player context. Never retry an uncertain outcome without inspecting the inventory.</summary>
    /// <param name="operations">The shared live operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="characterName">Exact online character name.</param>
    /// <param name="item">Item group, number, level and configured options.</param>
    /// <param name="apply">Apply only after approving the preview.</param>
    /// <param name="confirmationToken">Single-use token from the identical preview.</param>
    /// <returns>The safe live result or preview.</returns>
    [McpServerTool(Name = "players_give_item", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Preview or give one structured item to a live inventory, using the owning player context. Never retry an uncertain outcome without inspecting the inventory.")]
    [Authorize(Roles = "Operator")]
    [ExposesOperation(typeof(LiveGameOperations), nameof(LiveGameOperations.GiveItemAsync))]
    public static ValueTask<JsonObject> PlayersGiveItemAsync(
        [FromServices] LiveGameOperations operations,
        CancellationToken cancellationToken,
        [Description("Exact online character name.")] string characterName,
        [Description("Item group, number, level and configured options.")] ItemSpecification item,
        [Description("Apply only after approving the preview.")] bool apply = false,
        [Description("Single-use token from the identical preview.")] string? confirmationToken = null)
        => AdminToolExecution.ExecuteAsync(() => operations.GiveItemAsync(characterName, item, apply, confirmationToken, cancellationToken));

    /// <summary>Preview or set a live regular or master level within configured limits, notifying the client and observers.</summary>
    /// <param name="operations">The shared live operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="characterName">Exact online character name.</param>
    /// <param name="level">Requested level within the configured maximum.</param>
    /// <param name="master">Set master level instead of regular level.</param>
    /// <param name="apply">Apply only after approving the preview.</param>
    /// <param name="confirmationToken">Single-use token from the identical preview.</param>
    /// <returns>The safe live result or preview.</returns>
    [McpServerTool(Name = "players_set_level", ReadOnly = false, Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Preview or set a live regular or master level within configured limits, notifying the client and observers.")]
    [Authorize(Roles = "Operator")]
    [ExposesOperation(typeof(LiveGameOperations), nameof(LiveGameOperations.SetLevelAsync))]
    public static ValueTask<JsonObject> PlayersSetLevelAsync(
        [FromServices] LiveGameOperations operations,
        CancellationToken cancellationToken,
        [Description("Exact online character name.")] string characterName,
        [Description("Requested level within the configured maximum.")] int level,
        [Description("Set master level instead of regular level.")] bool master = false,
        [Description("Apply only after approving the preview.")] bool apply = false,
        [Description("Single-use token from the identical preview.")] string? confirmationToken = null)
        => AdminToolExecution.ExecuteAsync(() => operations.SetLevelAsync(characterName, level, master, apply, confirmationToken, cancellationToken));

    /// <summary>Preview or confirm an enabled periodic event start. restart=true removes existing participants. Review the active state and participant count, then apply the caller-bound confirmation token. Completion means the start was requested; inspect events_schedule for progress.</summary>
    /// <param name="operations">The shared live operations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="serverId">Actual game server identifier.</param>
    /// <param name="eventType">Invariant type from events_schedule.</param>
    /// <param name="restart">Explicitly allow removal of a running event before restarting it.</param>
    /// <param name="apply">Apply only after approving the event preview.</param>
    /// <param name="confirmationToken">The single-use preview token.</param>
    /// <returns>The safe live result or preview.</returns>
    [McpServerTool(Name = "events_start", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Preview or confirm an enabled periodic event start. restart=true removes existing participants. Review the active state and participant count, then apply the caller-bound confirmation token. Completion means the start was requested; inspect events_schedule for progress.")]
    [Authorize(Roles = "Operator")]
    [ExposesOperation(typeof(LiveGameOperations), nameof(LiveGameOperations.StartEventAsync))]
    public static ValueTask<JsonObject> EventsStartAsync(
        [FromServices] LiveGameOperations operations,
        CancellationToken cancellationToken,
        [Description("Actual game server identifier.")] int serverId,
        [Description("Invariant type from events_schedule.")] string eventType,
        [Description("Explicitly allow removal of a running event before restarting it.")] bool restart = false,
        [Description("Apply only after approving the event preview.")] bool apply = false,
        [Description("Single-use token from the identical preview.")] string? confirmationToken = null)
        => AdminToolExecution.ExecuteAsync(() => operations.StartEventAsync(serverId, eventType, restart, apply, confirmationToken, cancellationToken));
}
