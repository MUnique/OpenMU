// <copyright file="KanturuRuins3LandingTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using IIdentifiable = MUnique.OpenMU.Persistence.IIdentifiable;

/// <summary>
/// Tests that the Kanturu Ruins 3 warp cannot randomly place a player on blocked terrain.
/// </summary>
[TestFixture]
internal class KanturuRuins3LandingTest
{
    /// <summary>
    /// Tests every possible random landing against the terrain of the freshly initialized map.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasOnlyWalkableRuins3LandingPointsAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        AssertWalkableLandings(GetRuins3Gate(gameConfiguration));
    }

    /// <summary>
    /// Tests that the old seed is repaired in place, preserving owner settings and all other placements.
    /// Reapplication keeps the repaired data and the update entry's identity.
    /// </summary>
    [Test]
    public async Task UpdateRepairsOldSeedInPlaceAndCanBeReappliedAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var warp = gameConfiguration.WarpList.Single(warp => warp.Index == 45);
        var gate = GetRuins3Gate(gameConfiguration);
        var map = gate.Map ?? throw new InvalidOperationException("The Kanturu Ruins 3 exit gate has no map.");
        SetBounds(gate, 66, 183, 74, 191);
        warp.Costs = 12345;
        warp.LevelRequirement = 171;
        gate.Direction = Direction.North;
        gate.IsSpawnGate = true;
        var terrainData = map.TerrainData!.ToArray();
        var placements = CapturePlacementData(gameConfiguration, gate);
        var oldPoints = gate.GetPossibleLandingPoints().ToArray();
        var terrain = new GameMapTerrain(map);
        Assert.That(oldPoints, Has.Length.EqualTo(64));
        Assert.That(oldPoints.Count(point => !terrain.WalkMap[point.X, point.Y]), Is.EqualTo(19));

        var update = new FixKanturuRuins3LandingUpdatePlugIn();
        foreach (var entry in (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Where(entry => entry.Key == update.Key).ToArray())
        {
            await context.DeleteAsync(entry).ConfigureAwait(false);
        }

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertWalkableLandings(gate);
        Assert.That((gate.X1, gate.Y1, gate.X2, gate.Y2), Is.EqualTo(((byte)67, (byte)183, (byte)74, (byte)189)));
        Assert.That(CapturePlacementData(gameConfiguration, gate), Is.EqualTo(placements));
        Assert.That(gameConfiguration.WarpList.Single(entry => entry.Index == 45), Is.SameAs(warp));
        Assert.That(warp.Gate, Is.SameAs(gate));
        Assert.That(gate.Map, Is.SameAs(map));
        Assert.That(map.TerrainData, Is.EqualTo(terrainData));
        var repairedPlacements = CapturePlacementData(gameConfiguration);
        var installedEntry = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Single(entry => entry.Key == update.Key);
        var installedEntryId = ((IIdentifiable)installedEntry).Id;

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(CapturePlacementData(gameConfiguration), Is.EqualTo(repairedPlacements));
        Assert.That(gameConfiguration.WarpList.Single(entry => entry.Index == 45), Is.SameAs(warp));
        Assert.That(warp.Gate, Is.SameAs(gate));
        Assert.That(gate.Map, Is.SameAs(map));
        Assert.That(map.TerrainData, Is.EqualTo(terrainData));
        var entries = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Where(entry => entry.Key == update.Key).ToArray();
        Assert.That(entries, Has.Length.EqualTo(1));
        Assert.That(entries[0], Is.SameAs(installedEntry));
        Assert.That(((IIdentifiable)entries[0]).Id, Is.EqualTo(installedEntryId));
        Assert.That(entries[0].Version, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that changing any old seeded bound prevents the update from replacing an owner's destination.
    /// </summary>
    /// <param name="x1">The customized lower x-coordinate.</param>
    /// <param name="y1">The customized lower y-coordinate.</param>
    /// <param name="x2">The customized upper x-coordinate.</param>
    /// <param name="y2">The customized upper y-coordinate.</param>
    [TestCase(65, 183, 74, 191)]
    [TestCase(66, 182, 74, 191)]
    [TestCase(66, 183, 75, 191)]
    [TestCase(66, 183, 74, 192)]
    public async Task UpdateKeepsCustomizedBoundsAsync(byte x1, byte y1, byte x2, byte y2)
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        SetBounds(GetRuins3Gate(gameConfiguration), x1, y1, x2, y2);
        var placements = CapturePlacementData(gameConfiguration);
        var update = new FixKanturuRuins3LandingUpdatePlugIn();

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(CapturePlacementData(gameConfiguration), Is.EqualTo(placements));
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(CapturePlacementData(gameConfiguration), Is.EqualTo(placements));
    }

    /// <summary>
    /// Tests that the update skips other destinations and incomplete or detached references.
    /// </summary>
    /// <param name="change">The difference from the original owning configuration.</param>
    [TestCase("warp-index")]
    [TestCase("map-number")]
    [TestCase("map-discriminator")]
    [TestCase("missing-gate")]
    [TestCase("missing-map")]
    [TestCase("detached-map")]
    [TestCase("detached-gate")]
    [TestCase("missing-warp")]
    public async Task UpdateKeepsOtherOrMissingReferencesAsync(string change)
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var warp = gameConfiguration.WarpList.Single(warp => warp.Index == 45);
        var gate = GetRuins3Gate(gameConfiguration);
        var map = gate.Map!;
        SetBounds(gate, 66, 183, 74, 191);
        switch (change)
        {
            case "warp-index":
                warp.Index = 46;
                break;
            case "map-number":
                map.Number = 38;
                break;
            case "map-discriminator":
                map.Discriminator = 1;
                break;
            case "missing-gate":
                warp.Gate = null;
                break;
            case "missing-map":
                gate.Map = null;
                break;
            case "detached-map":
                gameConfiguration.Maps.Remove(map);
                break;
            case "detached-gate":
                map.ExitGates.Remove(gate);
                break;
            case "missing-warp":
                gameConfiguration.WarpList.Remove(warp);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(change));
        }

        var placements = CapturePlacementData(gameConfiguration);
        var targetGate = CaptureExitGateData(gate);
        var update = new FixKanturuRuins3LandingUpdatePlugIn();
        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            Assert.That(CapturePlacementData(gameConfiguration), Is.EqualTo(placements));
            Assert.That(CaptureExitGateData(gate), Is.EqualTo(targetGate));
        }
    }

    /// <summary>
    /// Tests that detached gate and map instances are not repaired merely because their identifiers match owned objects.
    /// </summary>
    /// <param name="detachMap">Whether the candidate map is also a detached instance with the owned map's identifier.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task UpdateKeepsDetachedInstancesWithOwnedIdsAsync(bool detachMap)
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var warp = gameConfiguration.WarpList.Single(entry => entry.Index == 45);
        var ownedGate = GetRuins3Gate(gameConfiguration);
        var ownedMap = ownedGate.Map!;
        var mapId = ((IIdentifiable)ownedMap).Id;
        var gateId = ((IIdentifiable)ownedGate).Id;
        var candidateMap = detachMap
            ? new BasicModel.GameMapDefinition { Id = mapId, Number = ownedMap.Number, Discriminator = ownedMap.Discriminator }
            : ownedMap;
        var candidateGate = ownedGate.Clone(gameConfiguration);
        candidateGate.Map = candidateMap;
        SetBounds(candidateGate, 66, 183, 74, 191);
        if (detachMap)
        {
            candidateMap.ExitGates.Add(candidateGate);
        }

        warp.Gate = candidateGate;
        Assert.That(candidateGate, Is.Not.SameAs(ownedGate));
        Assert.That(((IIdentifiable)candidateGate).Id, Is.EqualTo(gateId));
        if (detachMap)
        {
            Assert.That(candidateMap, Is.Not.SameAs(ownedMap));
            Assert.That(((IIdentifiable)candidateMap).Id, Is.EqualTo(mapId));
        }

        var placements = CapturePlacementData(gameConfiguration);
        var candidateData = CaptureExitGateData(candidateGate);
        var update = new FixKanturuRuins3LandingUpdatePlugIn();
        for (var application = 0; application < 2; application++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            Assert.That(CapturePlacementData(gameConfiguration), Is.EqualTo(placements));
            Assert.That(CaptureExitGateData(candidateGate), Is.EqualTo(candidateData));
            Assert.That(gameConfiguration.Maps.Single(map => ((IIdentifiable)map).Id == mapId), Is.SameAs(ownedMap));
            Assert.That(ownedMap.ExitGates.Single(gate => ((IIdentifiable)gate).Id == gateId), Is.SameAs(ownedGate));
            Assert.That(warp.Gate, Is.SameAs(candidateGate));
            Assert.That(candidateGate.Map, Is.SameAs(candidateMap));
        }
    }

    private static ExitGate GetRuins3Gate(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.WarpList.Single(warp => warp.Index == 45).Gate
            ?? throw new InvalidOperationException("The Kanturu Ruins 3 warp has no exit gate.");
    }

    private static void SetBounds(ExitGate gate, byte x1, byte y1, byte x2, byte y2)
    {
        gate.X1 = x1;
        gate.Y1 = y1;
        gate.X2 = x2;
        gate.Y2 = y2;
    }

    private static void AssertWalkableLandings(ExitGate gate)
    {
        var map = gate.Map ?? throw new InvalidOperationException("The Kanturu Ruins 3 exit gate has no map.");
        Assert.That(map.Number, Is.EqualTo(37));
        Assert.That(map.Discriminator, Is.Zero);
        Assert.That(map.TerrainData, Is.Not.Null.And.Not.Empty);

        var terrain = new GameMapTerrain(map);
        var possiblePoints = gate.GetPossibleLandingPoints().ToArray();
        var blockedPoints = possiblePoints.Where(point => !terrain.WalkMap[point.X, point.Y]).ToArray();

        Assert.That(possiblePoints, Has.Length.EqualTo(42));
        Assert.That(blockedPoints, Is.Empty, $"Blocked Ruins 3 landing points out of {possiblePoints.Length}: {string.Join(", ", blockedPoints)}");
    }

    private static object CaptureExitGateData(ExitGate gate, bool ignoreBounds = false)
    {
        return new
        {
            Gate = gate,
            ((IIdentifiable)gate).Id,
            gate.Map,
            gate.Direction,
            gate.IsSpawnGate,
            Bounds = ignoreBounds ? default : (gate.X1, gate.Y1, gate.X2, gate.Y2),
        };
    }

    private static object[] CapturePlacementData(GameConfiguration configuration, ExitGate? repairedGate = null)
    {
        var maps = configuration.Maps.Select(map => (object)new
        {
            Map = map,
            ((IIdentifiable)map).Id,
            map.Number,
            map.Discriminator,
            map.Name,
            map.TerrainData,
            map.SafezoneMap,
        });
        var warps = configuration.WarpList.Select(warp => (object)new
        {
            Warp = warp,
            ((IIdentifiable)warp).Id,
            warp.Index,
            warp.Name,
            warp.Costs,
            warp.LevelRequirement,
            warp.Gate,
        });
        var exits = configuration.Maps.SelectMany(map => map.ExitGates)
            .Select(gate => CaptureExitGateData(gate, ReferenceEquals(gate, repairedGate)));
        var entrances = configuration.Maps.SelectMany(map => map.EnterGates).Select(gate => (object)new
        {
            Gate = gate,
            ((IIdentifiable)gate).Id,
            gate.Number,
            gate.LevelRequirement,
            gate.TargetGate,
            gate.X1,
            gate.Y1,
            gate.X2,
            gate.Y2,
        });
        return maps.Concat(warps).Concat(exits).Concat(entrances).ToArray();
    }
}
