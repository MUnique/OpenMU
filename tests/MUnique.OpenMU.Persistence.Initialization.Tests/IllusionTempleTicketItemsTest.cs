// <copyright file="IllusionTempleTicketItemsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the illusion temple items 13/50, the Illusion Sorcerer Covenant, and 13/51, the Scroll of Blood which is
/// the ticket, of the season 6 data, and how the illusion temple update gives a configuration with the data of both
/// swapped the original data back.
/// </summary>
[TestFixture]
internal class IllusionTempleTicketItemsTest
{
    /// <summary>
    /// The number of the Illusion Sorcerer Covenant.
    /// </summary>
    internal const short CovenantNumber = 50;

    /// <summary>
    /// The number of the Scroll of Blood, the ticket of the event.
    /// </summary>
    internal const short TicketNumber = 51;

    private const byte ItemGroup = 13;

    private const short LumenTheBarmaidNumber = 255;

    /// <summary>
    /// The way in which a configuration got the data of the two items swapped.
    /// </summary>
    public enum SwappedState
    {
        /// <summary>
        /// The first version of the illusion temple update renumbered the item definitions, and created the
        /// temple definitions with the ticket at its new number 51.
        /// </summary>
        RenumberedByUpdate,

        /// <summary>
        /// The first version of the illusion temple update renumbered the item definitions, and the temple
        /// definitions which existed before kept the item which got the number 50.
        /// </summary>
        RenumberedByUpdateWithExistingTemples,

        /// <summary>
        /// The configuration was initialized with the swapped data.
        /// </summary>
        Initialized,
    }

    /// <summary>
    /// Tests that a new season 6 database has the original items: the Illusion Sorcerer Covenant (1x2), which
    /// drops from monsters and which a merchant sells, and the Scroll of Blood (2x2), which is the ticket of the
    /// six temples. The game client knows them by these numbers and sizes.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task NewDatabaseHasOriginalItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var covenant = GetItem(gameConfiguration, CovenantNumber);
        var ticket = GetItem(gameConfiguration, TicketNumber);
        Assert.Multiple(() =>
        {
            Assert.That(covenant.Name.ValueInNeutralLanguage, Is.EqualTo("Illusion Sorcerer Covenant"));
            Assert.That(covenant.Width, Is.EqualTo(1));
            Assert.That(covenant.Height, Is.EqualTo(2));
            Assert.That(
                GetDropItemGroups(gameConfiguration, covenant).Select(group => group.ItemLevel ?? 0),
                Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }));
            Assert.That(
                GetDropItemGroups(gameConfiguration, covenant).Select(group => group.MinimumMonsterLevel ?? 0),
                Is.EqualTo(new[] { 70, 76, 82, 88, 94, 100 }));
            Assert.That(
                gameConfiguration.Monsters.Single(monster => monster.Number == LumenTheBarmaidNumber).MerchantStore!.Items
                    .Where(item => item.Definition == covenant)
                    .Select(item => (int)item.Level),
                Is.EquivalentTo(new[] { 1, 2, 3, 4, 5 }));

            Assert.That(ticket.Name.ValueInNeutralLanguage, Is.EqualTo("Scroll of Blood"));
            Assert.That(ticket.Width, Is.EqualTo(2));
            Assert.That(ticket.Height, Is.EqualTo(2));
            Assert.That(GetDropItemGroups(gameConfiguration, ticket), Is.Empty);
            Assert.That(
                gameConfiguration.MiniGameDefinitions.Where(definition => definition.TicketItem == ticket).Select(definition => definition.Type),
                Is.EqualTo(Enumerable.Repeat(MiniGameType.IllusionTemple, 6)));
        });
    }

    /// <summary>
    /// Tests that the illusion temple update gives a configuration with the swapped data the data of a new
    /// database, without renumbering the item definitions: the item definition at 13/51, which owned tickets
    /// reference, becomes the Scroll of Blood. It also tests that applying it twice keeps the result.
    /// A configuration with the swapped data has version one or two of the update installed.
    /// </summary>
    /// <param name="state">The way in which the configuration got the swapped data.</param>
    /// <param name="installedVersion">The installed version of the update.</param>
    /// <returns>The task.</returns>
    [Test]
    public async Task UpdateRestoresSwappedItemsAsync([Values] SwappedState state, [Values(1, 2)] int installedVersion)
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var expected = CaptureItems(gameConfiguration);
        Swap(gameConfiguration, state);
        var ownedTicketDefinition = GetItem(gameConfiguration, TicketNumber);
        var covenant = GetItem(gameConfiguration, CovenantNumber);
        Assert.That(ownedTicketDefinition.Name.ValueInNeutralLanguage, Is.EqualTo("Illusion Sorcerer Covenant"), "The swapped state isn't simulated.");

        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = installedVersion;
        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

            AssertItems(gameConfiguration, expected);
            Assert.That(GetItem(gameConfiguration, TicketNumber), Is.SameAs(ownedTicketDefinition));
            Assert.That(GetItem(gameConfiguration, CovenantNumber), Is.SameAs(covenant));
            Assert.That(installedUpdate.Version, Is.EqualTo(update.Version));
        }
    }

    /// <summary>
    /// Tests that the update changes neither item when only one of the two has the swapped name, because the server
    /// owner renamed the other: the data of one item alone can't be swapped.
    /// </summary>
    /// <param name="renamedNumber">The number of the item which the server owner renamed.</param>
    /// <returns>The task.</returns>
    [TestCase(CovenantNumber)]
    [TestCase(TicketNumber)]
    public async Task UpdateKeepsPartlySwappedItemsAsync(short renamedNumber)
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        Swap(gameConfiguration, SwappedState.RenumberedByUpdate);
        GetItem(gameConfiguration, renamedNumber).Name = "Renamed by the owner";
        var expected = CaptureItems(gameConfiguration);

        var update = new IllusionTempleDataUpdatePlugIn();
        var installedUpdate = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        installedUpdate.Version = 2;
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertItems(gameConfiguration, expected);
    }

    /// <summary>
    /// Captures the data of the two items, and the data which references them: the drop item groups, the mini
    /// game definitions which have them as ticket, and the items of the merchant stores.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The captured data of each item, by item number.</returns>
    internal static IReadOnlyDictionary<short, IReadOnlyDictionary<string, object?>> CaptureItems(GameConfiguration gameConfiguration)
    {
        return new[] { CovenantNumber, TicketNumber }.ToDictionary(number => number, number => CaptureItem(gameConfiguration, number));
    }

    /// <summary>
    /// Asserts that the two items have the captured data, property by property.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="expected">The expected data, captured by <see cref="CaptureItems"/>.</param>
    internal static void AssertItems(GameConfiguration gameConfiguration, IReadOnlyDictionary<short, IReadOnlyDictionary<string, object?>> expected)
    {
        Assert.Multiple(() =>
        {
            foreach (var (number, expectedData) in expected)
            {
                var actualData = CaptureItem(gameConfiguration, number);
                foreach (var (name, value) in expectedData)
                {
                    Assert.That(actualData[name], Is.EqualTo(value), $"{ItemGroup}/{number}: {name}");
                }
            }
        });
    }

    /// <summary>
    /// Swaps the numbers of the two item definitions, like the first version of the illusion temple update did.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void RenumberLikeVersionOne(GameConfiguration gameConfiguration)
    {
        var covenant = GetItem(gameConfiguration, CovenantNumber);
        var ticket = GetItem(gameConfiguration, TicketNumber);
        covenant.Number = TicketNumber;
        ticket.Number = CovenantNumber;
    }

    /// <summary>
    /// Gets the item definition with the specified number of the item group 13.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="number">The number.</param>
    /// <returns>The item definition.</returns>
    internal static ItemDefinition GetItem(GameConfiguration gameConfiguration, short number)
    {
        return gameConfiguration.Items.Single(item => item.Group == ItemGroup && item.Number == number);
    }

    private static void Swap(GameConfiguration gameConfiguration, SwappedState state)
    {
        if (state == SwappedState.Initialized)
        {
            // The data initialization created the covenant with the number 51 and the Scroll of Blood with 50.
            // The merchant store items and the temple definitions were created by number.
            var covenant = GetItem(gameConfiguration, CovenantNumber);
            var ticket = GetItem(gameConfiguration, TicketNumber);
            var covenantName = covenant.Name;
            covenant.Name = ticket.Name;
            covenant.Width = 2;
            ticket.Name = covenantName;
            ticket.Width = 1;
            foreach (var group in GetDropItemGroups(gameConfiguration, covenant))
            {
                group.PossibleItems.Remove(covenant);
                group.PossibleItems.Add(ticket);
            }

            return;
        }

        RenumberLikeVersionOne(gameConfiguration);
        if (state == SwappedState.RenumberedByUpdate)
        {
            // The temple definitions were created after the renumbering, with the item which got the number 51.
            var renumberedCovenant = GetItem(gameConfiguration, TicketNumber);
            foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(definition => definition.Type == MiniGameType.IllusionTemple))
            {
                definition.TicketItem = renumberedCovenant;
            }
        }
    }

    private static IReadOnlyDictionary<string, object?> CaptureItem(GameConfiguration gameConfiguration, short number)
    {
        var item = GetItem(gameConfiguration, number);
        var data = typeof(ItemDefinition)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetIndexParameters().Length == 0)
            .ToDictionary(property => property.Name, property => AsComparable(property.GetValue(item)));
        data["(drop item groups)"] = GetDropItemGroups(gameConfiguration, item);
        data["(ticket of mini games)"] = gameConfiguration.MiniGameDefinitions
            .Where(definition => ReferenceEquals(definition.TicketItem, item))
            .ToList();
        data["(merchant store items)"] = gameConfiguration.Monsters
            .SelectMany(monster => monster.MerchantStore?.Items ?? Enumerable.Empty<MUnique.OpenMU.DataModel.Entities.Item>())
            .Where(storeItem => ReferenceEquals(storeItem.Definition, item))
            .ToList();
        return data;
    }

    private static object? AsComparable(object? value)
    {
        return value is IEnumerable enumerable and not string ? enumerable.Cast<object>().ToList() : value;
    }

    private static List<DropItemGroup> GetDropItemGroups(GameConfiguration gameConfiguration, ItemDefinition item)
    {
        return gameConfiguration.DropItemGroups.Where(group => group.PossibleItems.Any(possibleItem => ReferenceEquals(possibleItem, item))).ToList();
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
