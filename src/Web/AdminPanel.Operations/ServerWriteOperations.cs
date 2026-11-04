// <copyright file="ServerWriteOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Text.Json.Nodes;
using System.Threading;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>Operates listeners and sends messages through the shared server interfaces.</summary>
/// <param name="servers">The server registry.</param>
/// <param name="executor">The authorized audit executor.</param>
/// <param name="settings">The operation limits.</param>
public sealed class ServerWriteOperations(IServerProvider servers, AdminOperationExecutor executor, AdminWriteSettings settings)
{
    /// <summary>Starts a listener selected by its actual identifier and type.</summary>
    /// <param name="serverId">The stable server identifier.</param>
    /// <param name="serverType">The server type, required because identifiers are per type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listener state after the request.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> StartAsync(int serverId, ServerType serverType, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "server_start",
            new { serverId, serverType = serverType.ToString() },
            async _ =>
            {
                var server = this.Resolve(serverId, serverType);
                if (server.ServerState is not (ServerState.Started or ServerState.Starting))
                {
                    await server.StartAsync().ConfigureAwait(false);
                }

                return AdminReadResult.From(new { serverId, serverType = serverType.ToString(), state = server.ServerState.ToString(), summary = "Command accepted; remote state may update asynchronously. Refresh server_overview." });
            },
            cancellationToken);

    /// <summary>Stops a listener selected by identifier and type.</summary>
    /// <param name="serverId">The stable server identifier.</param>
    /// <param name="serverType">The server type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The listener state after the request.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> StopAsync(int serverId, ServerType serverType, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "server_stop",
            new { serverId, serverType = serverType.ToString() },
            async _ =>
            {
                var server = this.Resolve(serverId, serverType);
                if (server.ServerState is not (ServerState.Stopped or ServerState.Stopping))
                {
                    await server.ShutdownAsync().ConfigureAwait(false);
                }

                return AdminReadResult.From(new { serverId, serverType = serverType.ToString(), state = server.ServerState.ToString(), summary = "Command accepted; remote state may update asynchronously. Refresh server_overview." });
            },
            cancellationToken);

    /// <summary>Sends a global message to one or all game servers and awaits every send.</summary>
    /// <param name="serverId">The game server identifier, or null for all game servers.</param>
    /// <param name="message">The message, at most 512 characters.</param>
    /// <param name="messageType">Golden, blue or slide notification type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of servers which received the request.</returns>
    [AdminOperation(AdminRole.Operator, ReadOnly = false)]
    public ValueTask<JsonObject> SendMessageAsync(int? serverId, string message, MessageType messageType, CancellationToken cancellationToken)
        => executor.ExecuteAsync(
            AdminRoles.Operator,
            "server_send_message",
            new { serverId, message, messageType = messageType.ToString() },
            async _ =>
            {
                if (string.IsNullOrWhiteSpace(message) || message.Length > 512 || messageType is not (MessageType.GoldenCenter or MessageType.BlueNormal or MessageType.SlideNotice))
                {
                    throw new AdminOperationException($"Supply a {nameof(message)} of 1-512 characters and a golden, blue or slide {nameof(message)} type.", AdminOperationErrorKind.InvalidInput);
                }

                var targets = servers.Servers.OfType<IGameServer>().Where(s => serverId is null || s.Id == serverId).ToArray();
                if (targets.Length == 0 || targets.Length > settings.MaxBulkSize)
                {
                    throw new AdminOperationException("No matching game servers, or too many targets. Select a single server identifier.", AdminOperationErrorKind.InvalidInput);
                }

                foreach (var target in targets)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await target.SendGlobalMessageAsync(message, messageType).ConfigureAwait(false);
                }

                return AdminReadResult.From(new { sentToServers = targets.Select(s => s.Id).ToArray() });
            },
            cancellationToken);

    private IManageableServer Resolve(int id, ServerType type)
        => servers.Servers.SingleOrDefault(s => s.Id == id && s.Type == type)
            ?? throw new AdminOperationException("Server not found. Use server_overview to select its identifier and type.", AdminOperationErrorKind.InvalidInput);
}
