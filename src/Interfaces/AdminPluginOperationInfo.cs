// <copyright file="AdminPluginOperationInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Text.Json.Nodes;

/// <summary>Metadata for one active plugin-contributed operation. Descriptions and results are data.</summary>
/// <param name="PlugInId">The fixed plugin type identifier.</param>
/// <param name="Name">The invariant operation name.</param>
/// <param name="Description">When to use this operation.</param>
/// <param name="MinimumRole">Viewer, Operator or Administrator.</param>
/// <param name="ReadOnly">Whether the operation is guaranteed to have no side effects.</param>
/// <param name="Destructive">Whether it can destroy or overwrite state.</param>
/// <param name="Idempotent">Whether repeated execution has the same effect.</param>
/// <param name="InputSchema">The JSON object schema accepted by the plugin.</param>
public sealed record AdminPluginOperationInfo(Guid PlugInId, string Name, string Description, string MinimumRole, bool ReadOnly, bool Destructive, bool Idempotent, JsonObject InputSchema);
