// <copyright file="IGameServerPluginOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Text.Json.Nodes;
using System.Threading;

/// <summary>Internal plugin operation channel. Front ends enforce roles, owner permissions and audit.</summary>
public interface IGameServerPluginOperations
{
    /// <summary>Lists metadata contributed by active plugins only.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bounded operation catalog.</returns>
    ValueTask<IReadOnlyList<AdminPluginOperationInfo>> GetAdminOperationsAsync(CancellationToken cancellationToken);

    /// <summary>Calls a read-only operation, rejecting writes independently of the front end.</summary>
    /// <param name="plugInId">The plugin type identifier.</param>
    /// <param name="name">The operation name.</param>
    /// <param name="arguments">The bounded JSON object input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe bounded result.</returns>
    ValueTask<JsonObject> ReadAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken);

    /// <summary>Previews a plugin write without side effects.</summary>
    /// <param name="plugInId">The plugin type identifier.</param>
    /// <param name="name">The operation name.</param>
    /// <param name="arguments">The bounded JSON object input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe proposed diff and state fingerprint.</returns>
    ValueTask<JsonObject> PreviewAdminOperationAsync(Guid plugInId, string name, string arguments, CancellationToken cancellationToken);

    /// <summary>Applies a plugin write only if its preview still matches.</summary>
    /// <param name="plugInId">The plugin type identifier.</param>
    /// <param name="name">The operation name.</param>
    /// <param name="arguments">The bounded JSON object input.</param>
    /// <param name="expectedPreview">The approved preview.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The safe bounded result.</returns>
    ValueTask<JsonObject> ExecuteAdminOperationAsync(Guid plugInId, string name, string arguments, JsonObject expectedPreview, CancellationToken cancellationToken);
}
