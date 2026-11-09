// <copyright file="LuckyCoinDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the lucky coin and the NPC Delgado.
/// </summary>
[TestFixture]
internal class LuckyCoinDataTest
{
    private const short DelgadoNumber = 478;

    /// <summary>
    /// Tests that a new season 6 database contains the lucky coin, the attribute of the registered coins,
    /// and that Delgado opens the lucky coin dialog.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsLuckyCoinAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertLuckyCoinData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the lucky coin data to a database which was created before it existed.
    /// Applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsLuckyCoinToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Items.Where(IsLuckyCoin).ToList().ForEach(item => gameConfiguration.Items.Remove(item));
        gameConfiguration.Attributes.Where(a => a.Id == Stats.RegisteredLuckyCoins.Id).ToList().ForEach(a => gameConfiguration.Attributes.Remove(a));
        gameConfiguration.Monsters.Single(m => m.Number == DelgadoNumber).NpcWindow = NpcWindow.Undefined;

        var update = new AddLuckyCoinUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLuckyCoinData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update keeps another window which a server owner configured for Delgado.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomizedNpcWindowAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var delgado = gameConfiguration.Monsters.Single(m => m.Number == DelgadoNumber);
        delgado.NpcWindow = NpcWindow.Merchant;

        await new AddLuckyCoinUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(delgado.NpcWindow, Is.EqualTo(NpcWindow.Merchant));
    }

    private static bool IsLuckyCoin(ItemDefinition item) => item is { Group: 14, Number: 100 };

    private static void AssertLuckyCoinData(GameConfiguration gameConfiguration)
    {
        var coin = gameConfiguration.Items.Single(IsLuckyCoin);
        Assert.That(coin.Durability, Is.EqualTo(255));
        Assert.That(coin.Name.ValueInNeutralLanguage, Is.EqualTo("Lucky Coin"));
        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.RegisteredLuckyCoins.Id), Is.EqualTo(1));
        Assert.That(gameConfiguration.Monsters.Single(m => m.Number == DelgadoNumber).NpcWindow, Is.EqualTo(NpcWindow.DelgadoLuckyCoinRegistration));
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
