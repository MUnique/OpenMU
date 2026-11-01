// <copyright file="DarkHorseCanFlyTest.cs" company="MUnique">
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
/// Tests that the Dark Horse can fly (<see cref="Stats.CanFly"/>) in the season 6 data.
/// </summary>
[TestFixture]
internal class DarkHorseCanFlyTest
{
    /// <summary>
    /// Tests that a new season 6 database lets the Dark Horse fly.
    /// </summary>
    [Test]
    public async Task NewDatabaseLetsDarkHorseFlyAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        Assert.That(CountCanFly(GetDarkHorse(gameConfiguration)), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the update adds the power-up to a database which was created before it existed,
    /// and that applying it twice doesn't add it twice.
    /// </summary>
    [Test]
    public async Task UpdateLetsDarkHorseFlyOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var darkHorse = GetDarkHorse(gameConfiguration);
        foreach (var powerUp in darkHorse.BasePowerUpAttributes.Where(p => p.TargetAttribute == Stats.CanFly).ToList())
        {
            darkHorse.BasePowerUpAttributes.Remove(powerUp);
        }

        var update = new AddDarkHorseCanFlyPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(CountCanFly(darkHorse), Is.EqualTo(1));
    }

    private static int CountCanFly(ItemDefinition item)
    {
        return item.BasePowerUpAttributes.Count(powerUp => powerUp.TargetAttribute == Stats.CanFly && powerUp.BaseValue > 0);
    }

    private static ItemDefinition GetDarkHorse(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Single(item => item is { Group: 13, Number: 4 });
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
