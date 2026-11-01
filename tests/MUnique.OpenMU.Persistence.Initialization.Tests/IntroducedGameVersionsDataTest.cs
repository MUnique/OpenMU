// <copyright file="IntroducedGameVersionsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the versions of the original game which introduced the content (<see cref="IntroducedGameVersions"/>).
/// </summary>
[TestFixture]
internal class IntroducedGameVersionsDataTest
{
    private GameConfiguration _season6 = null!;

    /// <summary>
    /// Creates the season 6 data once for the tests which only read it.
    /// </summary>
    [OneTimeSetUp]
    public async Task CreateSeason6DataAsync()
    {
        (_, this._season6) = await CreateConfigurationAsync(p => new VersionSeasonSix.DataInitialization(p, new NullLoggerFactory())).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that every listed entry exists in the season 6 data, so that no entry is listed by mistake.
    /// </summary>
    [Test]
    public void EveryListedEntryExists()
    {
        var gameConfiguration = this._season6;

        foreach (var (number, discriminator) in IntroducedGameVersions.Maps.Keys)
        {
            Assert.That(gameConfiguration.Maps.Count(m => m.Number == number && m.Discriminator == discriminator), Is.EqualTo(1), $"Map ({number},{discriminator})");
        }

        foreach (var number in IntroducedGameVersions.CharacterClasses.Keys)
        {
            Assert.That(gameConfiguration.CharacterClasses.Count(c => c.Number == number), Is.EqualTo(1), $"Character class {number}");
        }

        foreach (var (type, level) in IntroducedGameVersions.MiniGames.Keys)
        {
            Assert.That(gameConfiguration.MiniGameDefinitions.Count(m => m.Type == type && m.GameLevel == level), Is.EqualTo(1), $"Mini game {type} {level}");
        }

        foreach (var (from, to, _) in IntroducedGameVersions.Monsters)
        {
            Assert.That(from, Is.LessThanOrEqualTo(to));
            Assert.That(gameConfiguration.Monsters.Count(m => m.Number >= from && m.Number <= to), Is.EqualTo(to - from + 1), $"Monsters {from} - {to}");
        }

        foreach (var (group, from, to, _) in IntroducedGameVersions.Items)
        {
            Assert.That(from, Is.LessThanOrEqualTo(to));
            Assert.That(gameConfiguration.Items.Count(i => i.Group == group && i.Number >= from && i.Number <= to), Is.EqualTo(to - from + 1), $"Items ({group},{from} - {to})");
        }
    }

    /// <summary>
    /// Tests that the listed ranges don't overlap, so that every entry has a distinct version.
    /// </summary>
    [Test]
    public void RangesDontOverlap()
    {
        var monsterNumbers = IntroducedGameVersions.Monsters.SelectMany(r => Enumerable.Range(r.From, r.To - r.From + 1)).ToList();
        Assert.That(monsterNumbers, Is.Unique);

        var itemNumbers = IntroducedGameVersions.Items.SelectMany(r => Enumerable.Range(r.From, r.To - r.From + 1).Select(n => (r.Group, n))).ToList();
        Assert.That(itemNumbers, Is.Unique);
    }

    /// <summary>
    /// Tests that a new season 6 database contains the versions of its content.
    /// </summary>
    [Test]
    public void NewDatabaseContainsVersions()
    {
        var gameConfiguration = this._season6;

        Assert.That(gameConfiguration.Maps.Single(m => m.Number == 0).IntroducedIn, Is.EqualTo(GameVersion.Version029)); // Lorencia
        Assert.That(gameConfiguration.Maps.Single(m => m.Number == 80).IntroducedIn, Is.EqualTo(GameVersion.Season6Episode1)); // Karutan 1
        Assert.That(gameConfiguration.CharacterClasses.Single(c => c.Number == 16).IntroducedIn, Is.EqualTo(GameVersion.Version099GPlus)); // Dark Lord
        Assert.That(gameConfiguration.Maps.Single(m => m.Number == 11).IntroducedIn, Is.EqualTo(GameVersion.Version096y)); // Blood Castle 1
        Assert.That(gameConfiguration.Maps.Single(m => m.Number == 30).IntroducedIn, Is.EqualTo(GameVersion.Version100s)); // Valley of Loren
        Assert.That(gameConfiguration.Monsters.Single(m => m.Number == 459).IntroducedIn, Is.EqualTo(GameVersion.Season4Episode1)); // Selupan
        Assert.That(gameConfiguration.Items.Single(i => i.Group == 14 && i.Number == 42).IntroducedIn, Is.EqualTo(GameVersion.Season2)); // Jewel of Harmony
        Assert.That(gameConfiguration.MiniGameDefinitions.Single(m => m.Type == MiniGameType.BloodCastle && m.GameLevel == 8).IntroducedIn, Is.EqualTo(GameVersion.Season3Episode1));

        Assert.That(gameConfiguration.Maps.Select(m => m.IntroducedIn), Has.None.EqualTo(GameVersion.Unknown));
        Assert.That(gameConfiguration.CharacterClasses.Select(c => c.IntroducedIn), Has.None.EqualTo(GameVersion.Unknown));
        Assert.That(gameConfiguration.MiniGameDefinitions.Select(m => m.IntroducedIn), Has.None.EqualTo(GameVersion.Unknown));
    }

    /// <summary>
    /// Tests that the content of the older data initializations was introduced at the latest in their version.
    /// </summary>
    [Test]
    public async Task OlderVersionsOnlyContainTheirContentAsync()
    {
        var (context075, version075) = await CreateConfigurationAsync(p => new Version075.DataInitialization(p, new NullLoggerFactory())).ConfigureAwait(false);
        using var c075 = context075;
        AssertMaximumVersion(version075, GameVersion.Version075);

        var (context095d, version095d) = await CreateConfigurationAsync(p => new Version095d.DataInitialization(p, new NullLoggerFactory())).ConfigureAwait(false);
        using var c095d = context095d;
        AssertMaximumVersion(version095d, GameVersion.Version095d);
    }

    /// <summary>
    /// Tests that the update sets the same versions on a database which was created before they existed,
    /// that it keeps versions which were already set, and that it can be applied twice.
    /// </summary>
    [Test]
    public async Task UpdateSetsVersionsOnExistingDatabaseAsync()
    {
        var expected = this._season6;
        var (context, gameConfiguration) = await CreateConfigurationAsync(p => new VersionSeasonSix.DataInitialization(p, new NullLoggerFactory())).ConfigureAwait(false);
        using var c = context;

        // The state after the database migration: every version is unknown.
        gameConfiguration.Maps.ToList().ForEach(m => m.IntroducedIn = GameVersion.Unknown);
        gameConfiguration.CharacterClasses.ToList().ForEach(m => m.IntroducedIn = GameVersion.Unknown);
        gameConfiguration.Monsters.ToList().ForEach(m => m.IntroducedIn = GameVersion.Unknown);
        gameConfiguration.Items.ToList().ForEach(m => m.IntroducedIn = GameVersion.Unknown);
        gameConfiguration.MiniGameDefinitions.ToList().ForEach(m => m.IntroducedIn = GameVersion.Unknown);

        // A version which was set by the administrator is kept.
        var lorencia = gameConfiguration.Maps.Single(m => m.Number == 0);
        lorencia.IntroducedIn = GameVersion.Season1;

        var update = new SetIntroducedGameVersionsPlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(lorencia.IntroducedIn, Is.EqualTo(GameVersion.Season1));
        Assert.That(
            gameConfiguration.Maps.Where(m => m != lorencia).Select(m => (m.Number, m.Discriminator, m.IntroducedIn)),
            Is.EquivalentTo(expected.Maps.Where(m => m.Number != 0).Select(m => (m.Number, m.Discriminator, m.IntroducedIn))));
        Assert.That(
            gameConfiguration.Monsters.Select(m => (m.Number, m.IntroducedIn)),
            Is.EquivalentTo(expected.Monsters.Select(m => (m.Number, m.IntroducedIn))));
        Assert.That(
            gameConfiguration.Items.Select(i => (i.Group, i.Number, i.IntroducedIn)),
            Is.EquivalentTo(expected.Items.Select(i => (i.Group, i.Number, i.IntroducedIn))));
        Assert.That(
            gameConfiguration.CharacterClasses.Select(m => (m.Number, m.IntroducedIn)),
            Is.EquivalentTo(expected.CharacterClasses.Select(m => (m.Number, m.IntroducedIn))));
        Assert.That(
            gameConfiguration.MiniGameDefinitions.Select(m => (m.Type, m.GameLevel, m.IntroducedIn)),
            Is.EquivalentTo(expected.MiniGameDefinitions.Select(m => (m.Type, m.GameLevel, m.IntroducedIn))));
    }

    private static void AssertMaximumVersion(GameConfiguration gameConfiguration, GameVersion maximum)
    {
        Assert.That(gameConfiguration.Maps.Where(m => m.IntroducedIn > maximum).Select(m => m.Name.ValueInNeutralLanguage), Is.Empty);
        Assert.That(gameConfiguration.CharacterClasses.Where(c => c.IntroducedIn > maximum).Select(c => c.Name.ValueInNeutralLanguage), Is.Empty);
        Assert.That(gameConfiguration.Monsters.Where(m => m.IntroducedIn > maximum).Select(m => m.Designation.ValueInNeutralLanguage), Is.Empty);
        Assert.That(gameConfiguration.Items.Where(i => i.IntroducedIn > maximum).Select(i => i.Name.ValueInNeutralLanguage), Is.Empty);
        Assert.That(gameConfiguration.MiniGameDefinitions.Where(m => m.IntroducedIn > maximum).Select(m => m.Name.ValueInNeutralLanguage), Is.Empty);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(Func<InMemoryPersistenceContextProvider, DataInitializationBase> createInitialization)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await createInitialization(contextProvider).CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        return (context, (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First());
    }
}
