// <copyright file="ServerPortsOutOfDynamicRangeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the ports of the game servers and the chat server are outside of the dynamic port range (49152 – 65535).
/// </summary>
[TestFixture]
internal class ServerPortsOutOfDynamicRangeTest
{
    /// <summary>
    /// The first port of the dynamic port range.
    /// </summary>
    private const int DynamicPortRangeStart = 49152;

    /// <summary>
    /// Tests that a new database uses ports below the dynamic port range.
    /// </summary>
    [Test]
    public async Task NewDatabaseUsesPortsBelowDynamicRangeAsync()
    {
        var (context, _) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var (gameServerEndpoints, chatServerEndpoints) = await GetEndpointsAsync(context).ConfigureAwait(false);

        Assert.That(gameServerEndpoints.Select(endpoint => endpoint.NetworkPort), Is.All.LessThan(DynamicPortRangeStart));
        Assert.That(gameServerEndpoints.Min(endpoint => endpoint.NetworkPort), Is.EqualTo(45901));
        Assert.That(chatServerEndpoints.Single().NetworkPort, Is.EqualTo(45980));
    }

    /// <summary>
    /// Tests that the update moves the former default ports of an existing database, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateMovesFormerDefaultPortsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var (gameServerEndpoints, chatServerEndpoints) = await GetEndpointsAsync(context).ConfigureAwait(false);
        var expectedGameServerPorts = gameServerEndpoints.Select(endpoint => endpoint.NetworkPort).ToList();
        SetFormerDefaultPorts(gameServerEndpoints, chatServerEndpoints);

        var update = new MoveServerPortsOutOfDynamicRangePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameServerEndpoints.Select(endpoint => endpoint.NetworkPort), Is.EqualTo(expectedGameServerPorts));
        Assert.That(chatServerEndpoints.Single().NetworkPort, Is.EqualTo(45980));
    }

    /// <summary>
    /// Tests that the update keeps custom ports, alternative published ports, and ports whose new port is already in use.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomAndConflictingPortsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var (gameServerEndpoints, chatServerEndpoints) = await GetEndpointsAsync(context).ConfigureAwait(false);
        Assert.That(gameServerEndpoints, Has.Count.GreaterThanOrEqualTo(3));
        SetFormerDefaultPorts(gameServerEndpoints, chatServerEndpoints);

        var customEndpoint = gameServerEndpoints[0];
        customEndpoint.NetworkPort = 50000;
        var publishedEndpoint = gameServerEndpoints[1];
        publishedEndpoint.AlternativePublishedPort = publishedEndpoint.NetworkPort;
        var conflictingEndpoint = gameServerEndpoints[2];
        var formerConflictingPort = conflictingEndpoint.NetworkPort;
        chatServerEndpoints.Single().NetworkPort = formerConflictingPort - 10000;

        var update = new MoveServerPortsOutOfDynamicRangePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(customEndpoint.NetworkPort, Is.EqualTo(50000));
        Assert.That(publishedEndpoint.NetworkPort, Is.EqualTo(publishedEndpoint.AlternativePublishedPort - 10000));
        Assert.That(conflictingEndpoint.NetworkPort, Is.EqualTo(formerConflictingPort));
    }

    private static void SetFormerDefaultPorts(IList<GameServerEndpoint> gameServerEndpoints, IList<ChatServerEndpoint> chatServerEndpoints)
    {
        foreach (var endpoint in gameServerEndpoints)
        {
            endpoint.NetworkPort += 10000;
        }

        chatServerEndpoints.Single().NetworkPort = 55980;
    }

    private static async Task<(IList<GameServerEndpoint> GameServerEndpoints, IList<ChatServerEndpoint> ChatServerEndpoints)> GetEndpointsAsync(IContext context)
    {
        var gameServerEndpoints = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(false))
            .SelectMany(server => server.Endpoints)
            .OrderBy(endpoint => endpoint.NetworkPort)
            .ToList();
        var chatServerEndpoints = (await context.GetAsync<ChatServerDefinition>().ConfigureAwait(false))
            .SelectMany(server => server.Endpoints)
            .ToList();
        return (gameServerEndpoints, chatServerEndpoints);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(2, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
