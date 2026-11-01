// <copyright file="SuspiciousScrapOfPaperStackedPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Tests the <see cref="SuspiciousScrapOfPaperStackedPlugIn"/>.
/// </summary>
[TestFixture]
public class SuspiciousScrapOfPaperStackedPlugInTest
{
    private readonly SuspiciousScrapOfPaperStackedPlugIn _plugIn = new();

    /// <summary>
    /// Tests that a full stack of five suspicious scraps of paper transforms into a Gaion's Order.
    /// </summary>
    [Test]
    public async Task FullStackTransformsIntoGaionsOrderAsync()
    {
        var (player, scrap, order) = await CreatePlayerWithScrapAsync(5).ConfigureAwait(false);

        await this._plugIn.ItemStackedAsync(player, new Item(), scrap).ConfigureAwait(false);

        Assert.That(scrap.Definition, Is.SameAs(order));
        Assert.That(scrap.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that an incomplete stack of suspicious scraps of paper stays as it is.
    /// </summary>
    [Test]
    public async Task IncompleteStackStaysAsync()
    {
        var (player, scrap, _) = await CreatePlayerWithScrapAsync(4).ConfigureAwait(false);
        var scrapDefinition = scrap.Definition;

        await this._plugIn.ItemStackedAsync(player, new Item(), scrap).ConfigureAwait(false);

        Assert.That(scrap.Definition, Is.SameAs(scrapDefinition));
        Assert.That(scrap.Durability, Is.EqualTo(4));
    }

    private static async Task<(GameLogic.Player Player, Item Scrap, ItemDefinition Order)> CreatePlayerWithScrapAsync(byte stackedScraps)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var scrapDefinition = new ItemDefinition { Group = 14, Number = 101, Width = 1, Height = 1, Durability = 5 };
        var orderDefinition = new ItemDefinition { Group = 14, Number = 102, Width = 1, Height = 1, Durability = 1 };
        player.GameContext.Configuration.Items.Add(scrapDefinition);
        player.GameContext.Configuration.Items.Add(orderDefinition);

        var scrap = new Item { Definition = scrapDefinition, Durability = stackedScraps, ItemSlot = 12 };
        await player.Inventory!.AddItemAsync(12, scrap).ConfigureAwait(false);
        return (player, scrap, orderDefinition);
    }
}
