// <copyright file="IAdminOperationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>Contributes one discoverable, validated administration operation from an active plugin.</summary>
/// <remarks>Implementations validate input against their schema and return only safe scalar projections, never secrets. Preview must have no side effects. Read-only execution must have no side effects. Writes must serialize with their own gameplay state and atomically compare the approved preview before applying in ExecuteAsync.</remarks>
[Guid("66D17990-86E4-4665-B91A-39905F54F168")]
[PlugInPoint("Admin Operations", "Contributes live administrative operations with role and safety metadata.")]
public interface IAdminOperationPlugIn : IStrategyPlugIn<string>
{
    /// <summary>Gets the operation's schema, minimum role and safety annotations.</summary>
    AdminPluginOperationInfo Operation { get; }

    /// <summary>Validates input and returns the intended diff and current state fingerprint without changing state.</summary>
    /// <param name="context">The owning game context.</param>
    /// <param name="arguments">The validated-size JSON input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe proposed diff, including relevant before-values.</returns>
    ValueTask<JsonObject> PreviewAsync(IGameContext context, JsonObject arguments, CancellationToken cancellationToken);

    /// <summary>Validates input and executes the operation, respecting its safety metadata.</summary>
    /// <param name="context">The owning game context.</param>
    /// <param name="arguments">The validated-size JSON input.</param>
    /// <param name="expectedPreview">Null for reads; for writes, the approved diff and state fingerprint which must be compared atomically with the mutation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe result, excluding secrets and bounded to 16 KiB.</returns>
    ValueTask<JsonObject> ExecuteAsync(IGameContext context, JsonObject arguments, JsonObject? expectedPreview, CancellationToken cancellationToken);
}
