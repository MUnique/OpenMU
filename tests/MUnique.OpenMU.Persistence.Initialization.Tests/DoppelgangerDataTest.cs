// <copyright file="DoppelgangerDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the doppelganger event.
/// </summary>
[TestFixture]
internal class DoppelgangerDataTest
{
    private const short LugardNumber = 540;
    private const short ElvenlandNumber = 51;
    private static readonly short[] EventMapNumbers = [65, 66, 67, 68];

    /// <summary>
    /// Tests that a new season 6 database contains the doppelganger event.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventAsync()
    {
        var gameConfiguration = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that a new database contains the configuration of the event, which references the
    /// monsters instead of containing copies of them, so that it can be edited in the admin panel.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventConfigurationAsync()
    {
        var gameConfiguration = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        var plugInConfiguration = gameConfiguration.PlugInConfigurations.Single(c => c.TypeId == typeof(DoppelgangerFeaturePlugIn).GUID);
        var json = plugInConfiguration.CustomConfiguration;
        TestContext.Out.WriteLine(json?[..Math.Min(json.Length, 1500)]);

        Assert.That(json, Is.Not.Null.And.Not.Empty);
        Assert.That(json, Does.Contain("\"HerdMonsters\""));
        Assert.That(json, Does.Contain("\"Paths\""));
        Assert.That(json, Does.Not.Contain("\"Designation\""), "The monsters should be referenced instead of being serialized completely.");
    }

    /// <summary>
    /// Tests that the update adds the doppelganger event to a database which was created
    /// before it existed, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateAddsEventToExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        // Revert the database to the state before the doppelganger event existed.
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.Doppelganger).ToList())
        {
            gameConfiguration.MiniGameDefinitions.Remove(definition);
        }

        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(item => item is { Group: 14, Number: 111 }));
        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(item => item is { Group: 13, Number: 125 }));
        var signDropGroup = gameConfiguration.DropItemGroups.Single(group => group.PossibleItems.Any(item => item is { Group: 14, Number: 110 }));
        gameConfiguration.DropItemGroups.Remove(signDropGroup);
        foreach (var map in gameConfiguration.Maps)
        {
            map.DropItemGroups.Remove(signDropGroup);
        }

        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(item => item is { Group: 14, Number: 110 }));
        foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.Number is >= 529 and <= 539).ToList())
        {
            gameConfiguration.Monsters.Remove(monster);
        }

        gameConfiguration.Monsters.Single(monster => monster.Number == LugardNumber).NpcWindow = NpcWindow.Undefined;
        foreach (var map in gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)))
        {
            map.SafezoneMap = map;
        }

        // The previous entrance gate of the first event map, which contains non-walkable coordinates.
        var entrance = gameConfiguration.Maps.Single(map => map.Number == 65).ExitGates.Single(gate => gate.IsSpawnGate);
        (entrance.X1, entrance.Y1, entrance.X2, entrance.Y2) = ((byte)193, (byte)26, (byte)200, (byte)32);

        var update = new AddDoppelgangerDataUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the monsters to a database which already got the event
    /// definitions by an earlier version of the update, without duplicating the definitions.
    /// </summary>
    [Test]
    public async Task UpdateAddsMissingMonstersAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.Number is >= 529 and <= 539).ToList())
        {
            gameConfiguration.Monsters.Remove(monster);
        }

        var mirror = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 111 });
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.Doppelganger))
        {
            definition.TicketItem = mirror;
        }

        await new AddDoppelgangerDataUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    private static async Task<GameConfiguration> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static void AssertEventData(GameConfiguration gameConfiguration)
    {
        Assert.That(gameConfiguration.Items.Count(item => item is { Group: 14, Number: 111 }), Is.EqualTo(1), "Mirror of Dimensions");
        Assert.That(gameConfiguration.Items.Count(item => item is { Group: 13, Number: 125 }), Is.EqualTo(1), "Doppelganger Free Ticket");
        var sign = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 110 });
        Assert.That(sign.Durability, Is.EqualTo(5), "A stack of five signs transforms into a mirror.");
        var signDropGroup = gameConfiguration.DropItemGroups.Single(group => group.PossibleItems.Contains(sign));
        Assert.That(gameConfiguration.Maps.Single(map => map.Number == 0 && map.Discriminator == 0).DropItemGroups, Does.Contain(signDropGroup));
        var definitions = gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.Doppelganger).ToList();

        Assert.That(definitions.Select(d => (short)d.GameLevel), Is.EquivalentTo(new short[] { 1, 2, 3, 4 }));
        Assert.That(definitions.Select(d => d.Entrance?.Map?.Number), Is.EquivalentTo(EventMapNumbers.Select(n => (short?)n)));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.MapCreationPolicy == MiniGameMapCreationPolicy.OnePerParty));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.TicketItem is null));
        Assert.That(
            gameConfiguration.Monsters.Where(monster => monster.Number is >= 529 and <= 539).Select(monster => monster.Number),
            Is.EquivalentTo(Enumerable.Range(529, 11).Select(n => (short)n)));
        foreach (var chestNumber in new short[] { 541, 542 })
        {
            var chest = gameConfiguration.Monsters.Single(monster => monster.Number == chestNumber);
            Assert.That(chest.ObjectKind, Is.EqualTo(NpcObjectKind.Destructible));
            Assert.That(chest.DropItemGroups, Has.Count.EqualTo(3));
            Assert.That(chest.NumberOfMaximumItemDrops, Is.GreaterThan(0));
        }

        Assert.That(gameConfiguration.Monsters.Single(monster => monster.Number == LugardNumber).NpcWindow, Is.EqualTo(NpcWindow.LugardDoppelgangerEntry));
        Assert.That(
            gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)).Select(map => map.SafezoneMap?.Number),
            Has.All.EqualTo(ElvenlandNumber));
        Assert.That(
            gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number))
                .Select(map => map.ExitGates.Single(gate => gate.IsSpawnGate))
                .Select(gate => (gate.Map!.Number, gate.X1, gate.Y1, gate.X2, gate.Y2)),
            Is.EquivalentTo(new (short, byte, byte, byte, byte)[]
            {
                (65, 194, 26, 199, 32),
                (66, 134, 69, 139, 74),
                (67, 106, 60, 111, 62),
                (68, 92, 13, 97, 17),
            }));
    }
}
