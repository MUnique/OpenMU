// <copyright file="DinorantOptionNumbersTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the dinorant options have the numbers which the client expects as their bits.
/// </summary>
[TestFixture]
internal class DinorantOptionNumbersTest
{
    private static readonly Dictionary<AttributeDefinition, int> ExpectedNumbers = new()
    {
        { Stats.DamageReceiveDecrement, 1 },
        { Stats.MaximumAbility, 2 },
        { Stats.AttackSpeedAny, 4 },
    };

    /// <summary>
    /// Tests that a new database has the expected dinorant option numbers.
    /// </summary>
    /// <param name="season6">If set to <c>true</c>, the season 6 data is tested; otherwise, the 0.95d data.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task NewDatabaseHasDinorantOptionNumbersAsync(bool season6)
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(season6).ConfigureAwait(false);
        using var _ = context;

        AssertOptionNumbers(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update sets the dinorant option numbers of a database which was created when all of them had the same number,
    /// and that applying it twice doesn't change the result.
    /// </summary>
    /// <param name="season6">If set to <c>true</c>, the season 6 data is tested; otherwise, the 0.95d data.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task UpdateSetsDinorantOptionNumbersAsync(bool season6)
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(season6).ConfigureAwait(false);
        using var _ = context;
        foreach (var option in GetDinorantOptions(gameConfiguration).PossibleOptions)
        {
            option.Number = 4;
        }

        FixDinorantOptionNumbersPlugInBase update = season6 ? new FixDinorantOptionNumbersPlugInSeason6() : new FixDinorantOptionNumbersPlugIn095D();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertOptionNumbers(gameConfiguration);
    }

    private static void AssertOptionNumbers(GameConfiguration gameConfiguration)
    {
        var options = GetDinorantOptions(gameConfiguration).PossibleOptions;
        Assert.That(options, Has.Count.EqualTo(ExpectedNumbers.Count));
        foreach (var option in options)
        {
            Assert.That(option.Number, Is.EqualTo(ExpectedNumbers[option.PowerUpDefinition!.TargetAttribute!]), option.PowerUpDefinition.TargetAttribute!.Designation);
        }
    }

    private static ItemOptionDefinition GetDinorantOptions(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.ItemOptions.Single(o => o.Name == "Dinorant Options");
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(bool season6)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        DataInitializationBase dataInitialization = season6
            ? new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory())
            : new Version095d.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
