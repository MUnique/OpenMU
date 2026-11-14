// <copyright file="CrywolfGateTerrainTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Tests that the gate from Valley of Loren to Crywolf Fortress can be entered with both terrains of the client.
/// </summary>
[TestFixture]
internal class CrywolfGateTerrainTest
{
    /// <summary>
    /// The tiles of the south entry, which the client lets the player walk on outside of Castle Siege.
    /// </summary>
    private static readonly (byte X, byte Y)[] SouthEntry =
    [
        (161, 40),
        (161, 41), (162, 41), (163, 41), (164, 41),
        (161, 42), (162, 42), (163, 42), (164, 42),
        (161, 43), (162, 43), (163, 43), (164, 43),
    ];

    /// <summary>
    /// The tiles of the north entry, which the client lets the player walk on during Castle Siege.
    /// </summary>
    private static readonly (byte X, byte Y)[] NorthEntry = [(161, 37), (162, 37), (161, 38), (162, 38), (161, 39)];

    /// <summary>
    /// Tests that a freshly initialized database has both entries of the gate walkable.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasBothEntriesWalkableAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var map = GetValleyOfLoren(gameConfiguration);

        AssertBothEntriesWalkable(map);
        var enterGate = map.EnterGates.Single(gate => gate.Number == 117);
        Assert.That(SouthEntry.Concat(NorthEntry), Is.All.Matches<(byte X, byte Y)>(tile => IsInGate(enterGate, tile)));
    }

    /// <summary>
    /// Tests that the update opens the south entry of an old database, keeps the other flags of these tiles and
    /// all other tiles, and that applying it a second time changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateOpensSouthEntryAndCanBeReappliedAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var map = GetValleyOfLoren(gameConfiguration);
        var oldTerrain = map.TerrainData!.ToArray();
        foreach (var tile in SouthEntry)
        {
            oldTerrain[GetOffset(tile)] |= (byte)TerrainAttributeType.Blocked;
        }

        oldTerrain[GetOffset(SouthEntry[0])] |= (byte)TerrainAttributeType.Safezone;
        map.TerrainData = oldTerrain;
        Assert.That(SouthEntry, Is.All.Matches<(byte X, byte Y)>(tile => !new GameMapTerrain(map).WalkMap[tile.X, tile.Y]));
        var otherMaps = gameConfiguration.Maps.Where(other => other != map).Select(other => (other, other.TerrainData)).ToArray();

        var update = new FixCrywolfGateTerrainUpdatePlugIn();
        foreach (var entry in (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Where(entry => entry.Key == update.Key).ToArray())
        {
            await context.DeleteAsync(entry).ConfigureAwait(false);
        }

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertBothEntriesWalkable(map);
        var repairedTerrain = map.TerrainData!;
        Assert.That(repairedTerrain[GetOffset(SouthEntry[0])], Is.EqualTo((byte)TerrainAttributeType.Safezone));
        var changedOffsets = Enumerable.Range(0, oldTerrain.Length).Where(i => oldTerrain[i] != repairedTerrain[i]);
        Assert.That(changedOffsets, Is.EquivalentTo(SouthEntry.Select(GetOffset)));
        Assert.That(gameConfiguration.Maps.Where(other => other != map).Select(other => (other, other.TerrainData)), Is.EqualTo(otherMaps));

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(map.TerrainData, Is.SameAs(repairedTerrain));
        var entries = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Where(entry => entry.Key == update.Key).ToArray();
        Assert.That(entries, Has.Length.EqualTo(1));
    }

    /// <summary>
    /// Tests that the update doesn't touch other variants of the map or terrain data of an unexpected size.
    /// </summary>
    /// <param name="change">The difference from the seeded Valley of Loren.</param>
    [TestCase("discriminator")]
    [TestCase("short-terrain")]
    public async Task UpdateKeepsOtherMapsAndUnexpectedTerrainAsync(string change)
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var map = GetValleyOfLoren(gameConfiguration);
        var terrain = map.TerrainData!.ToArray();
        foreach (var tile in SouthEntry)
        {
            terrain[GetOffset(tile)] |= (byte)TerrainAttributeType.Blocked;
        }

        switch (change)
        {
            case "discriminator":
                map.Discriminator = 1;
                break;
            case "short-terrain":
                terrain = terrain[..^1];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(change));
        }

        map.TerrainData = terrain;
        await new FixCrywolfGateTerrainUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(map.TerrainData, Is.SameAs(terrain));
    }

    private static GameMapDefinition GetValleyOfLoren(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Maps.Single(map => map is { Number: ValleyOfLoren.Number, Discriminator: 0 });
    }

    private static int GetOffset((byte X, byte Y) tile) => 3 + (tile.Y * 256) + tile.X;

    private static bool IsInGate(EnterGate gate, (byte X, byte Y) tile)
    {
        return tile.X >= gate.X1 && tile.X <= gate.X2 && tile.Y >= gate.Y1 && tile.Y <= gate.Y2;
    }

    private static void AssertBothEntriesWalkable(GameMapDefinition map)
    {
        var walkMap = new GameMapTerrain(map).WalkMap;
        var blockedTiles = SouthEntry.Concat(NorthEntry).Where(tile => !walkMap[tile.X, tile.Y]).ToArray();
        Assert.That(blockedTiles, Is.Empty, $"Blocked tiles of the Crywolf gate: {string.Join(", ", blockedTiles)}");
    }
}
