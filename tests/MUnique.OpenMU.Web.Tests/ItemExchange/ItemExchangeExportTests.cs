// <copyright file="ItemExchangeExportTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemExchange;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
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
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration));
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
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration));
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
        using var file = Parse(ItemExchangeExport.Write(this._seasonSixConfiguration));
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
        var bytes = ItemExchangeExport.Write(this._seasonSixConfiguration);
        var text = Encoding.UTF8.GetString(bytes);

        Assert.That(bytes.Take(3), Is.Not.EqualTo(Encoding.UTF8.Preamble.ToArray()));
        Assert.That(text, Does.Not.Contain("\r"));
        Assert.That(text, Does.EndWith("}\n"));
    }

    /// <summary>
    /// Tests that the operation reads the stored configuration and writes the same file.
    /// </summary>
    [Test]
    public async Task OperationExportsTheStoredConfigurationAsync()
    {
        var operations = new ItemExchangeOperations(this._seasonSix);

        var file = await operations.ExportAsync(default).ConfigureAwait(false);

        Assert.That(file, Is.EqualTo(ItemExchangeExport.Write(this._seasonSixConfiguration)));
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

        Assert.That(items, Is.EqualTo(new (byte, short)[] { (0, 1), (14, 2), (14, 3) }));
    }

    private static JsonDocument Parse(byte[] file) => JsonDocument.Parse(file);

    private static void AddItem(IContext context, GameConfiguration configuration, byte group, short number)
    {
        var item = context.CreateNew<ItemDefinition>();
        item.Group = group;
        item.Number = number;
        configuration.Items.Add(item);
    }
}
