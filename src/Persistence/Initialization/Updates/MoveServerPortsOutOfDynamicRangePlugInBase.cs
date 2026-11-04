// <copyright file="MoveServerPortsOutOfDynamicRangePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update moves the ports of the game servers and the chat server from the former defaults
/// (55901 and following, 55980) to the new defaults (45901 and following, 45980).
/// </summary>
/// <remarks>
/// The former defaults are within the dynamic (ephemeral) port range 49152 – 65535, which operating
/// systems like Windows use for the local ports of outgoing connections. So, after a while, one
/// of these ports may already be in use by another process, and the server couldn't listen on it.
/// Only ports between <see cref="FormerPortRangeStart"/> and <see cref="FormerPortRangeEnd"/> are
/// moved, so custom ports outside of this range are kept. Alternative published ports are kept as well,
/// because they describe a mapping outside of the server, e.g. in a router.
/// </remarks>
public abstract class MoveServerPortsOutOfDynamicRangePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Move server ports out of the dynamic port range";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update moves the ports of the game servers and the chat server from 55900 – 55999 to 45900 – 45999 (e.g. 55901 to 45901 and 55980 to 45980), because the former ports are within the dynamic port range of the operating system and may already be in use. Adjust your firewall, port forwardings and docker compose files accordingly.";

    /// <summary>
    /// The first port of the range which is moved.
    /// </summary>
    internal const int FormerPortRangeStart = 55900;

    /// <summary>
    /// The last port of the range which is moved.
    /// </summary>
    internal const int FormerPortRangeEnd = 55999;

    /// <summary>
    /// The offset which is added to a port of the former range.
    /// </summary>
    internal const int PortOffset = -10000;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 9, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var gameServerEndpoints = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(false))
            .SelectMany(server => server.Endpoints)
            .ToList();
        var chatServerEndpoints = (await context.GetAsync<ChatServerDefinition>().ConfigureAwait(false))
            .SelectMany(server => server.Endpoints)
            .ToList();
        var connectServers = (await context.GetAsync<ConnectServerDefinition>().ConfigureAwait(false)).ToList();

        var usedPorts = gameServerEndpoints.Select(endpoint => endpoint.NetworkPort)
            .Concat(chatServerEndpoints.Select(endpoint => endpoint.NetworkPort))
            .Concat(connectServers.Select(server => server.ClientListenerPort))
            .ToHashSet();

        foreach (var endpoint in gameServerEndpoints.Concat<ServerEndpoint>(chatServerEndpoints))
        {
            if (endpoint.NetworkPort is < FormerPortRangeStart or > FormerPortRangeEnd)
            {
                continue;
            }

            var newPort = endpoint.NetworkPort + PortOffset;
            if (!usedPorts.Add(newPort))
            {
                // Another listener already uses the new port, so we keep the old one.
                continue;
            }

            usedPorts.Remove(endpoint.NetworkPort);
            endpoint.NetworkPort = newPort;
        }
    }
}
