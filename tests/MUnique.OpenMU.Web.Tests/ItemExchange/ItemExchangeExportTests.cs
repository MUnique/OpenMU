// <copyright file="ItemExchangeExportTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemExchange;

using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Operations;
using MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

/// <summary>
/// Tests the item exchange file which <see cref="ItemExchangeExport"/> writes.
/// </summary>
[TestFixture]
public sealed class ItemExchangeExportTests
{
    private InMemoryPersistenceContextProvider _seasonSix = null!;
    private GameConfiguration _seasonSixConfiguration = null!;
    private IContext _seasonSixContext = null!;

    /// <summary>
    /// Creates a new Season 6 configuration once for the tests which read it.
    /// </summary>
    [OneTimeSetUp]
    public async Task CreateSeasonSixAsync()
    {
        this._seasonSix = new InMemoryPersistenceContextProvider();
        var initialization = new MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.DataInitialization(this._seasonSix, NullLoggerFactory.Instance);
        await initialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        this._seasonSixContext = this._seasonSix.CreateNewContext();
        this._seasonSixConfiguration = (await this._seasonSixContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    /// <summary>
    /// Disposes the context of the Season 6 configuration.
    /// </summary>
    [OneTimeTearDown]
    public void DisposeSeasonSix()
    {
        this._seasonSixContext.Dispose();
    }

    /// <summary>
    /// Tests the header of the file: a file with a <c>fields</c> list has the format version 2.
    /// </summary>
    [Test]
    public void HeaderNamesTheSourceAndTheFields()
    {
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration, ItemExchangeExport.ClientGameVersion));
        var root = file.RootElement;

        Assert.That(root.GetProperty("formatVersion").GetInt32(), Is.EqualTo(2));
        Assert.That(root.GetProperty("source").GetString(), Is.EqualTo("OpenMU"));
        Assert.That(root.GetProperty("gameVersion").GetString(), Is.EqualTo("season6"));
        Assert.That(
            root.GetProperty("fields").EnumerateArray().Select(field => field.GetString()),
            Is.EqualTo(ItemExchangeExport.Fields.Select(field => field.Key)));
        Assert.That(root.EnumerateObject().Select(property => property.Name), Is.EqualTo(new[] { "formatVersion", "source", "gameVersion", "fields", "items" }));
    }

    /// <summary>
    /// Tests that the file of a new Season 6 configuration has every item, sorted by group and number.
    /// </summary>
    [Test]
    public void SeasonSixFileHasEveryItemSorted()
    {
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration, ItemExchangeExport.ClientGameVersion));
        var items = file.RootElement.GetProperty("items").EnumerateArray()
            .Select(item => (Group: item.GetProperty("group").GetInt32(), Number: item.GetProperty("number").GetInt32()))
            .ToList();
        var expected = this._seasonSixConfiguration.Items
            .Select(item => (Group: (int)item.Group, Number: (int)item.Number))
            .OrderBy(item => item.Group)
            .ThenBy(item => item.Number)
            .ToList();

        Assert.That(items, Has.Count.EqualTo(this._seasonSixConfiguration.Items.Count));
        Assert.That(items, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that an item carries only its group, its number and the fields of the <c>fields</c> list.
    /// </summary>
    [Test]
    public void ItemsCarryOnlyTheListedFields()
    {
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration, ItemExchangeExport.ClientGameVersion));
        var allowed = new[] { "group", "number" }.Concat(ItemExchangeExport.Fields.Select(field => field.Key)).ToHashSet();
        var keys = file.RootElement.GetProperty("items").EnumerateArray()
            .SelectMany(item => item.EnumerateObject().Select(property => property.Name))
            .Distinct();

        Assert.That(keys, Is.SubsetOf(allowed));
    }

    /// <summary>
    /// Tests the text format: UTF-8 without a byte order mark, LF line endings and a final line break.
    /// </summary>
    [Test]
    public void FileIsUtf8WithLineFeeds()
    {
        var bytes = ItemExchangeExport.Write(this._seasonSixConfiguration, ItemExchangeExport.ClientGameVersion);
        var text = Encoding.UTF8.GetString(bytes);

        Assert.That(bytes.Take(3), Is.Not.EqualTo(Encoding.UTF8.Preamble.ToArray()));
        Assert.That(text, Does.Not.Contain("\r"));
        Assert.That(text, Does.EndWith("}\n"));
    }

    /// <summary>
    /// Tests that the operation reads the stored configuration and writes the same file, as Season 6 data.
    /// </summary>
    [Test]
    public async Task OperationExportsTheStoredConfigurationAsync()
    {
        using var configurations = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, this._seasonSix);
        var operations = new ItemExchangeOperations(this._seasonSix, configurations, NullLogger<ItemExchangeOperations>.Instance);

        var file = await operations.ExportAsync(default).ConfigureAwait(false);

        Assert.That(file, Is.EqualTo(ItemExchangeExport.Write(this._seasonSixConfiguration, ItemExchangeExport.ClientGameVersion)));
    }

    /// <summary>
    /// Tests that the file names the game version of the configuration, so that the client refuses
    /// the file of another version than its own.
    /// </summary>
    [Test]
    public async Task FileNamesTheGameVersionOfTheConfigurationAsync()
    {
        var gameVersion = await ExportGameVersionAsync(context => context.CreateNew<ConfigurationUpdateState>().InitializationKey = "0.75").ConfigureAwait(false);

        Assert.That(gameVersion, Is.EqualTo("0.75"));
    }

    /// <summary>
    /// Tests that a configuration from before the stored update state is Season 6 data when it has
    /// a Season 6 client, the original one or the open source one.
    /// </summary>
    /// <param name="season">The season of the client.</param>
    [TestCase((byte)6)]
    [TestCase((byte)106)]
    public async Task SeasonSixClientTellsTheGameVersionWithoutUpdateStateAsync(byte season)
    {
        var gameVersion = await ExportGameVersionAsync(context => AddClient(context, season, 3)).ConfigureAwait(false);

        Assert.That(gameVersion, Is.EqualTo(ItemExchangeExport.ClientGameVersion));
    }

    /// <summary>
    /// Tests that the file is refused when the game version of the configuration is unknown.
    /// </summary>
    [Test]
    public void UnknownGameVersionIsRefused()
    {
        var error = Assert.ThrowsAsync<AdminOperationException>(() => ExportGameVersionAsync(context => AddClient(context, 0, 75)));

        Assert.That(error?.Kind, Is.EqualTo(AdminOperationErrorKind.Unavailable));
    }

    /// <summary>
    /// Tests that the admin panel's read operations include the export, which the controller needs.
    /// </summary>
    [Test]
    public void ReadOperationsIncludeTheExport()
    {
        var services = new ServiceCollection().AddAdminReadOperations();

        Assert.That(
            services.Where(service => service.ServiceType == typeof(ItemExchangeOperations)).Select(service => service.Lifetime),
            Is.EqualTo(new[] { ServiceLifetime.Scoped }));
    }

    /// <summary>
    /// Tests that edits which the admin panel hasn't saved yet aren't part of the file: while the
    /// admin panel has changes, the stored configuration is read.
    /// </summary>
    [Test]
    public async Task UnsavedChangesAreLeftOutAsync()
    {
        var (stored, configurations) = await CreateEditedConfigurationAsync(hasChanges: true).ConfigureAwait(false);

        var file = await new ItemExchangeOperations(stored, configurations, NullLogger<ItemExchangeOperations>.Instance).ExportAsync(default).ConfigureAwait(false);

        Assert.That(GetItemKeys(file), Is.EqualTo(new[] { (0, 1) }));
    }

    /// <summary>
    /// Tests that the configuration which the admin panel has already loaded is written when it
    /// has no changes, instead of loading the stored one again.
    /// </summary>
    [Test]
    public async Task LoadedConfigurationIsWrittenWithoutChangesAsync()
    {
        var (stored, configurations) = await CreateEditedConfigurationAsync(hasChanges: false).ConfigureAwait(false);

        var file = await new ItemExchangeOperations(stored, configurations, NullLogger<ItemExchangeOperations>.Instance).ExportAsync(default).ConfigureAwait(false);

        Assert.That(GetItemKeys(file), Is.EqualTo(new[] { (0, 1), (0, 2) }));
    }

    /// <summary>
    /// Tests that items outside the client's groups and numbers are left out, because the client refuses
    /// the whole file because of them, and that the others are sorted.
    /// </summary>
    [Test]
    public void ItemsOutsideTheClientsRangeAreLeftOut()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        AddItem(context, configuration, 14, 3);
        AddItem(context, configuration, 16, 0);
        AddItem(context, configuration, 0, 512);
        AddItem(context, configuration, 0, 1);
        AddItem(context, configuration, 14, 2);

        var items = ItemExchangeExport.GetExportedItems(configuration).Select(item => (item.Group, item.Number));
        var leftOut = ItemExchangeExport.GetLeftOutItems(configuration).Select(item => (item.Group, item.Number));

        Assert.That(items, Is.EqualTo(new (byte, short)[] { (0, 1), (14, 2), (14, 3) }));
        Assert.That(leftOut, Is.EqualTo(new (byte, short)[] { (0, 512), (16, 0) }));
    }

    /// <summary>
    /// Tests that the items which the file leaves out are logged, so that the server operator learns
    /// why the client editor doesn't get them.
    /// </summary>
    [Test]
    public async Task LeftOutItemsAreLoggedAsync()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using (var context = provider.CreateNewContext())
        {
            var configuration = context.CreateNew<GameConfiguration>();
            AddItem(context, configuration, 0, 1);
            AddItem(context, configuration, 16, 0);
            context.CreateNew<ConfigurationUpdateState>().InitializationKey = ItemExchangeExport.ClientGameVersion;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var configurations = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider);
        var logger = new Mock<ILogger<ItemExchangeOperations>>();

        await new ItemExchangeOperations(provider, configurations, logger.Object).ExportAsync(default).ConfigureAwait(false);

        logger.Verify(
            log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => $"{state}".Contains("leaves out 1 item(s)", StringComparison.Ordinal) && $"{state}".Contains("16/0", StringComparison.Ordinal)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that items which share their group and number are sorted by their id, so that the
    /// order in which the configuration was loaded doesn't change the file.
    /// </summary>
    [Test]
    public void RepeatedItemsAreSortedByTheirId()
    {
        var provider = new InMemoryPersistenceContextProvider();
        using var context = provider.CreateNewContext();
        var configuration = context.CreateNew<GameConfiguration>();
        var repeated = Enumerable.Range(0, 3).Select(_ => CreateItem(context, 14, 15)).ToList();

        // The configuration gets them in the reverse order of their ids.
        foreach (var item in repeated.OrderByDescending(item => item.GetId()))
        {
            configuration.Items.Add(item);
        }

        var items = ItemExchangeExport.GetExportedItems(configuration);

        Assert.That(items, Is.EqualTo(repeated.OrderBy(item => item.GetId())));
    }

    private static JsonDocument Parse(byte[] file) => JsonDocument.Parse(file);

    private static List<(int Group, int Number)> GetItemKeys(byte[] file)
    {
        using var document = Parse(file);
        return document.RootElement.GetProperty("items").EnumerateArray()
            .Select(item => (item.GetProperty("group").GetInt32(), item.GetProperty("number").GetInt32()))
            .ToList();
    }

    /// <summary>
    /// Exports a stored configuration with the item 0/1 and the data which tells its game version.
    /// </summary>
    private static async Task<string> ExportGameVersionAsync(Action<IContext> addVersionData)
    {
        var provider = new InMemoryPersistenceContextProvider();
        using (var context = provider.CreateNewContext())
        {
            var configuration = context.CreateNew<GameConfiguration>();
            AddItem(context, configuration, 0, 1);
            addVersionData(context);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        using var configurations = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider);
        var file = await new ItemExchangeOperations(provider, configurations, NullLogger<ItemExchangeOperations>.Instance).ExportAsync(default).ConfigureAwait(false);
        using var document = Parse(file);
        return document.RootElement.GetProperty("gameVersion").GetString() ?? string.Empty;
    }

    private static void AddClient(IContext context, byte season, byte episode)
    {
        var client = context.CreateNew<GameClientDefinition>();
        client.Season = season;
        client.Episode = episode;
    }

    /// <summary>
    /// Creates a stored configuration with the item 0/1, and the admin panel's copy of it, which
    /// has the item 0/2 as well.
    /// </summary>
    private static async Task<(IPersistenceContextProvider Stored, IDataSource<GameConfiguration> Configurations)> CreateEditedConfigurationAsync(bool hasChanges)
    {
        var stored = new InMemoryPersistenceContextProvider();
        Guid id;
        using (var context = stored.CreateNewContext())
        {
            var configuration = context.CreateNew<GameConfiguration>();
            AddItem(context, configuration, 0, 1);
            context.CreateNew<ConfigurationUpdateState>().InitializationKey = ItemExchangeExport.ClientGameVersion;
            await context.SaveChangesAsync().ConfigureAwait(false);
            id = configuration.GetId();
        }

        using var editContext = new InMemoryPersistenceContextProvider().CreateNewContext();
        var edited = editContext.CreateNew<GameConfiguration>();
        ((IIdentifiable)edited).Id = id;
        AddItem(editContext, edited, 0, 1);
        AddItem(editContext, edited, 0, 2);

        var adminPanelContext = new Mock<IContext>();
        adminPanelContext.Setup(context => context.HasChanges).Returns(hasChanges);
        var configurations = new Mock<IDataSource<GameConfiguration>>();
        configurations
            .Setup(source => source.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(edited);
        configurations
            .Setup(source => source.GetContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminPanelContext.Object);
        return (stored, configurations.Object);
    }

    private static void AddItem(IContext context, GameConfiguration configuration, byte group, short number)
    {
        configuration.Items.Add(CreateItem(context, group, number));
    }

    private static ItemDefinition CreateItem(IContext context, byte group, short number)
    {
        var item = context.CreateNew<ItemDefinition>();
        item.Group = group;
        item.Number = number;
        return item;
    }
}
