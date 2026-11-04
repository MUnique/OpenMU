// <copyright file="SignOfDimensionsStackedPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Tests for the <see cref="SignOfDimensionsStackedPlugIn"/>.
/// </summary>
[TestFixture]
public class SignOfDimensionsStackedPlugInTest
{
    private readonly SignOfDimensionsStackedPlugIn _plugIn = new();

    /// <summary>
    /// Tests that a full stack of five signs of dimensions transforms into a mirror of dimensions.
    /// </summary>
    [Test]
    public async Task FullStackTransformsIntoMirrorAsync()
    {
        var (player, sign, mirror) = await CreatePlayerWithSignAsync(5).ConfigureAwait(false);

        await this._plugIn.ItemStackedAsync(player, new Item(), sign).ConfigureAwait(false);

        Assert.That(sign.Definition, Is.SameAs(mirror));
        Assert.That(sign.Durability, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that an incomplete stack of signs of dimensions stays as it is.
    /// </summary>
    [Test]
    public async Task IncompleteStackStaysAsync()
    {
        var (player, sign, _) = await CreatePlayerWithSignAsync(4).ConfigureAwait(false);
        var signDefinition = sign.Definition;

        await this._plugIn.ItemStackedAsync(player, new Item(), sign).ConfigureAwait(false);

        Assert.That(sign.Definition, Is.SameAs(signDefinition));
        Assert.That(sign.Durability, Is.EqualTo(4));
    }

    private static async Task<(GameLogic.Player Player, Item Sign, ItemDefinition Mirror)> CreatePlayerWithSignAsync(byte stackedSigns)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var signDefinition = new ItemDefinition { Group = 14, Number = 110, Width = 1, Height = 1, Durability = 5 };
        var mirrorDefinition = new ItemDefinition { Group = 14, Number = 111, Width = 1, Height = 1, Durability = 1 };
        player.GameContext.Configuration.Items.Add(signDefinition);
        player.GameContext.Configuration.Items.Add(mirrorDefinition);

        var sign = new Item { Definition = signDefinition, Durability = stackedSigns, ItemSlot = 12 };
        await player.Inventory!.AddItemAsync(12, sign).ConfigureAwait(false);
        return (player, sign, mirrorDefinition);
    }
}
