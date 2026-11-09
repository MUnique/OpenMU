// <copyright file="TalismanOfResurrectionSpawnGatePlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="TalismanOfResurrectionSpawnGatePlugIn"/>.
/// </summary>
[TestFixture]
public class TalismanOfResurrectionSpawnGatePlugInTest
{
    private static readonly Point DeathPosition = new(100, 120);

    /// <summary>
    /// A dead player with a talisman is spawned at the place of its death, and the talisman is used up.
    /// </summary>
    [Test]
    public async Task DeadPlayerWithTalismanSpawnsAtDeathPositionAsync()
    {
        var player = await CreateDeadPlayerAsync().ConfigureAwait(false);
        var talisman = await AddItemAsync(player, 13, 69).ConfigureAwait(false);
        var args = new SpawnGateSelectionArgs();

        await new TalismanOfResurrectionSpawnGatePlugIn().SelectSpawnGateAsync(player, args).ConfigureAwait(false);

        Assert.That(args.Gate, Is.Not.Null);
        Assert.That(args.Gate!.Map, Is.SameAs(player.CurrentMap!.Definition));
        Assert.That((args.Gate.X1, args.Gate.X2, args.Gate.Y1, args.Gate.Y2), Is.EqualTo((DeathPosition.X, DeathPosition.X, DeathPosition.Y, DeathPosition.Y)));
        Assert.That(player.Inventory!.Items, Does.Not.Contain(talisman));
    }

    /// <summary>
    /// A dead player without a talisman gets no gate from the plugin, so it's spawned at the safezone.
    /// </summary>
    [Test]
    public async Task DeadPlayerWithoutTalismanGetsNoGateAsync()
    {
        var player = await CreateDeadPlayerAsync().ConfigureAwait(false);
        var otherItem = await AddItemAsync(player, 13, 70).ConfigureAwait(false);
        var args = new SpawnGateSelectionArgs();

        await new TalismanOfResurrectionSpawnGatePlugIn().SelectSpawnGateAsync(player, args).ConfigureAwait(false);

        Assert.That(args.Gate, Is.Null);
        Assert.That(player.Inventory!.Items, Does.Contain(otherItem));
    }

    /// <summary>
    /// A living player which is moved to the safezone keeps its talisman.
    /// </summary>
    [Test]
    public async Task LivingPlayerKeepsTalismanAsync()
    {
        var player = await CreateDeadPlayerAsync().ConfigureAwait(false);
        player.IsAlive = true;
        var talisman = await AddItemAsync(player, 13, 69).ConfigureAwait(false);
        var args = new SpawnGateSelectionArgs();

        await new TalismanOfResurrectionSpawnGatePlugIn().SelectSpawnGateAsync(player, args).ConfigureAwait(false);

        Assert.That(args.Gate, Is.Null);
        Assert.That(player.Inventory!.Items, Does.Contain(talisman));
    }

    private static async ValueTask<Player> CreateDeadPlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Position = DeathPosition;
        player.IsAlive = false;
        return player;
    }

    private static async ValueTask<Item> AddItemAsync(Player player, byte group, short number)
    {
        var definition = new BasicModel.ItemDefinition { Group = group, Number = number, Width = 1, Height = 1, Durability = 1 };
        var item = new BasicModel.Item { Definition = definition, Durability = 1 };
        Assert.That(await player.Inventory!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        return item;
    }
}
