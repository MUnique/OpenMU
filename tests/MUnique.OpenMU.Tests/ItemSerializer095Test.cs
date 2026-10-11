// <copyright file="ItemSerializer095Test.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Unit tests for the <see cref="ItemSerializer095"/>.
/// </summary>
[TestFixture]
public class ItemSerializer095Test
{
    private GameConfiguration _gameConfiguration = null!;
    private IPersistenceContextProvider _contextProvider = null!;

    /// <summary>
    /// Sets up the test environment by initializing configuration data and a <see cref="IPersistenceContextProvider"/>.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        this._contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(this._contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(1, false).ConfigureAwait(false);
        this._gameConfiguration = (await this._contextProvider.CreateNewConfigurationContext().GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    /// <summary>
    /// Tests that the item option number and the additional option of a second wing are serialized
    /// into the exc option byte, as the client of version 0.97 expects it.
    /// </summary>
    [Test]
    public void SecondWingOptions()
    {
        using var context = this._contextProvider.CreateNewContext(this._gameConfiguration);
        var item = context.CreateNew<Item>();
        item.Definition = this._gameConfiguration.Items.Single(i => i is { Group: 12, Number: 3 }); // Wings of Spirits
        item.Durability = 200;

        var option = context.CreateNew<ItemOptionLink>();
        option.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def => def.PossibleOptions)
            .Single(o => o.OptionType == ItemOptionTypes.Option && o.Number == 2);
        option.Level = 1;
        item.ItemOptions.Add(option);

        var wingOption = context.CreateNew<ItemOptionLink>();
        wingOption.ItemOption = item.Definition.PossibleItemOptions.SelectMany(def => def.PossibleOptions)
            .Single(o => o.OptionType == ItemOptionTypes.Wing && o.Number == 1);
        item.ItemOptions.Add(wingOption);

        var array = new byte[new ItemSerializer095().NeededSpace];
        new ItemSerializer095().SerializeItem(array, item);

        Assert.That(array[1] & 0b11, Is.EqualTo(1), "option level");
        Assert.That(array[3] & 0x80, Is.EqualTo(0x80), "item number above 255");
        Assert.That(array[3] & 0x30, Is.EqualTo(0x20), "item option number");
        Assert.That(array[3] & 0x0F, Is.EqualTo(0x01), "additional wing option");
    }
}
