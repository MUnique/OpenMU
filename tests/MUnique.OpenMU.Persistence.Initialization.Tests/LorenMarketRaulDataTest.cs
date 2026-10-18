// <copyright file="LorenMarketRaulDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of Jeweler Raul of the Loren Market.
/// </summary>
[TestFixture]
internal class LorenMarketRaulDataTest
{
    private const short RaulNumber = 546;

    /// <summary>
    /// Tests that Raul of a new database opens the jewel bundle window.
    /// </summary>
    [Test]
    public async Task NewDatabaseLetsRaulOpenTheJewelBundleWindowAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        Assert.That(GetRaul(gameConfiguration).NpcWindow, Is.EqualTo(NpcWindow.Lahap));
    }

    /// <summary>
    /// Tests that the update sets the window of Raul of an existing database,
    /// and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateLetsRaulOfExistingDatabaseOpenTheJewelBundleWindowAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        GetRaul(gameConfiguration).NpcWindow = NpcWindow.Undefined;

        var update = new AddLorenMarketRaulJewelBundlePlugIn();
        for (var i = 0; i < 2; i++)
        {
            await update.ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);
        }

        Assert.That(GetRaul(gameConfiguration).NpcWindow, Is.EqualTo(NpcWindow.Lahap));
    }

    /// <summary>
    /// Tests that the update keeps a window, which was already configured for Raul.
    /// </summary>
    [Test]
    public async Task UpdateKeepsConfiguredWindowOfRaulAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        GetRaul(gameConfiguration).NpcWindow = NpcWindow.Merchant;

        await new AddLorenMarketRaulJewelBundlePlugIn().ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);

        Assert.That(GetRaul(gameConfiguration).NpcWindow, Is.EqualTo(NpcWindow.Merchant));
    }

    private static async Task<GameConfiguration> CreateConfigurationAsync(InMemoryPersistenceContextProvider contextProvider)
    {
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static MonsterDefinition GetRaul(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Monsters.Single(monster => monster.Number == RaulNumber);
    }
}
