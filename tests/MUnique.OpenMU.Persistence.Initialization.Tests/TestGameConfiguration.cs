// <copyright file="TestGameConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Creates initialized game configurations on an in-memory database for the initialization tests.
/// </summary>
internal static class TestGameConfiguration
{
    /// <summary>
    /// Creates a freshly initialized Season 6 game configuration.
    /// </summary>
    /// <returns>The persistence context and the game configuration.</returns>
    public static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6Async()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        return await InitializeAsync(contextProvider, dataInitialization).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a freshly initialized 0.95d game configuration.
    /// </summary>
    /// <returns>The persistence context and the game configuration.</returns>
    public static async Task<(IContext Context, GameConfiguration GameConfiguration)> Create095dAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version095d.DataInitialization(contextProvider, new NullLoggerFactory());
        return await InitializeAsync(contextProvider, dataInitialization).ConfigureAwait(false);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> InitializeAsync(
        InMemoryPersistenceContextProvider contextProvider,
        DataInitializationBase dataInitialization)
    {
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
