// <copyright file="VulcanusWarpIndexTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the index of the warp entry of Vulcanus, which the season 6 game client requests with 42.
/// </summary>
[TestFixture]
internal class VulcanusWarpIndexTest
{
    /// <summary>
    /// The number of the map Vulcanus.
    /// </summary>
    private const short VulcanusMapNumber = 63;

    /// <summary>
    /// Tests that a new season 6 database has the warp entry of Vulcanus at index 42.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasVulcanusAtIndex42Async()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        Assert.That(GetVulcanus(gameConfiguration).Index, Is.EqualTo(42));
    }

    /// <summary>
    /// Tests that the update corrects the index of an existing database, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateCorrectsTheIndexAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var vulcanus = GetVulcanus(gameConfiguration);
        vulcanus.Index = 37;

        var update = new FixVulcanusWarpIndexUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(vulcanus.Index, Is.EqualTo(42));
        Assert.That(gameConfiguration.WarpList.Count(warpInfo => warpInfo.Index == 42), Is.EqualTo(1));
    }

    private static WarpInfo GetVulcanus(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.WarpList.Single(warpInfo => warpInfo.Gate?.Map?.Number == VulcanusMapNumber);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
