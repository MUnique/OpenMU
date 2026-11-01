// <copyright file="ServerStatisticsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="ServerStatistics"/>.
/// </summary>
[TestFixture]
public class ServerStatisticsTests
{
    /// <summary>
    /// Only the players of the game servers are counted, not the connections of the other servers.
    /// </summary>
    [Test]
    public void OnlinePlayerCountIncludesOnlyGameServers()
    {
        var servers = new[]
        {
            TestServers.Create(ServerType.GameServer, ServerState.Started, 10),
            TestServers.Create(ServerType.GameServer, ServerState.Started, 5),
            TestServers.Create(ServerType.ConnectServer, ServerState.Started, 100),
            TestServers.Create(ServerType.ChatServer, ServerState.Started, 7),
        };

        Assert.That(ServerStatistics.GetOnlinePlayerCount(servers), Is.EqualTo(15));
    }

    /// <summary>
    /// The players of a game server which timed out are not counted, since its count is outdated.
    /// </summary>
    [Test]
    public void OnlinePlayerCountExcludesTimedOutServers()
    {
        var servers = new[]
        {
            TestServers.Create(ServerType.GameServer, ServerState.Started, 10),
            TestServers.Create(ServerType.GameServer, ServerState.Timeout, 5),
        };

        Assert.That(ServerStatistics.GetOnlinePlayerCount(servers), Is.EqualTo(10));
    }

    /// <summary>
    /// The capacity is the sum of the maximum connections of the started game servers.
    /// </summary>
    [Test]
    public void PlayerCapacityIncludesOnlyStartedGameServers()
    {
        var servers = new[]
        {
            TestServers.Create(ServerType.GameServer, ServerState.Started, 0, 1000),
            TestServers.Create(ServerType.GameServer, ServerState.Started, 0, 500),
            TestServers.Create(ServerType.GameServer, ServerState.Stopped, 0, 1000),
            TestServers.Create(ServerType.ConnectServer, ServerState.Started, 0, 5000),
        };

        Assert.That(ServerStatistics.GetPlayerCapacity(servers), Is.EqualTo(1500));
    }

    /// <summary>
    /// The capacity is unknown, when a game server doesn't limit its connections.
    /// </summary>
    [Test]
    public void PlayerCapacityIsNullForUnlimitedGameServer()
    {
        var servers = new[]
        {
            TestServers.Create(ServerType.GameServer, ServerState.Started, 0, 1000),
            TestServers.Create(ServerType.GameServer, ServerState.Started, 0, int.MaxValue),
        };

        Assert.That(ServerStatistics.GetPlayerCapacity(servers), Is.Null);
    }
}
