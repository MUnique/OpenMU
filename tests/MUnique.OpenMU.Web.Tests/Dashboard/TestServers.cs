// <copyright file="TestServers.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using Moq;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Creates <see cref="IManageableServer"/>s for the dashboard tests.
/// </summary>
internal static class TestServers
{
    /// <summary>
    /// Creates a server.
    /// </summary>
    /// <param name="type">The server type.</param>
    /// <param name="state">The server state.</param>
    /// <param name="currentConnections">The current connections.</param>
    /// <param name="maximumConnections">The maximum connections.</param>
    /// <returns>The created server.</returns>
    public static IManageableServer Create(ServerType type, ServerState state, int currentConnections, int maximumConnections = 1000)
    {
        var server = new Mock<IManageableServer>();
        server.Setup(s => s.Type).Returns(type);
        server.Setup(s => s.ServerState).Returns(state);
        server.Setup(s => s.CurrentConnections).Returns(currentConnections);
        server.Setup(s => s.MaximumConnections).Returns(maximumConnections);
        server.Setup(s => s.Description).Returns($"{type} {currentConnections}");
        return server.Object;
    }

    /// <summary>
    /// Creates a server provider.
    /// </summary>
    /// <param name="servers">The servers.</param>
    /// <returns>The created server provider.</returns>
    public static IServerProvider CreateProvider(params IManageableServer[] servers)
    {
        var provider = new Mock<IServerProvider>();
        provider.Setup(p => p.Servers).Returns(servers.ToList());
        return provider.Object;
    }
}
