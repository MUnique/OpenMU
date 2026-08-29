// <copyright file="BotWarpCandidateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;
using NUnit.Framework;

/// <summary>
/// Tests for the check which keeps a bot from warping to a map it cannot stand in.
/// </summary>
[TestFixture]
public class BotWarpCandidateTest
{
    /// <summary>
    /// A spawn gate without a single walkable tile strands whoever is placed there, so the map is
    /// not offered to a bot.
    /// </summary>
    [Test]
    public void MapWithBlockedSpawnGateIsRejected()
    {
        var map = CreateMap(gateIsWalkable: false);

        Assert.That(BotNavigator.HasWalkableSpawnGateCore(map), Is.False);
    }

    /// <summary>
    /// A spawn gate with walkable tiles is fine.
    /// </summary>
    [Test]
    public void MapWithWalkableSpawnGateIsAccepted()
    {
        var map = CreateMap(gateIsWalkable: true);

        Assert.That(BotNavigator.HasWalkableSpawnGateCore(map), Is.True);
    }

    /// <summary>
    /// A bot which lands on a blocked tile is not recovered to the destination's own spawn gate but
    /// to the one of its <see cref="GameMapDefinition.SafezoneMap"/> - which for dungeons and event
    /// maps (Icarus, Karutan 2, the Chaos Castles, ...) is a different map. So the destination's own
    /// gate being blocked does not make the map unusable.
    /// </summary>
    [Test]
    public void MapIsJudgedByItsSafezoneMapNotByItself()
    {
        var destination = CreateMap(gateIsWalkable: false);
        destination.SafezoneMap = CreateMap(gateIsWalkable: true);

        Assert.That(BotNavigator.HasWalkableSpawnGate(destination), Is.True);
    }

    /// <summary>
    /// The other half of the same rule: a destination with a perfectly good gate of its own is still
    /// rejected when the map it would be recovered to is the broken one.
    /// </summary>
    [Test]
    public void MapWithBrokenSafezoneMapIsRejected()
    {
        var destination = CreateMap(gateIsWalkable: true);
        destination.SafezoneMap = CreateMap(gateIsWalkable: false);

        Assert.That(BotNavigator.HasWalkableSpawnGate(destination), Is.False);
    }

    /// <summary>
    /// Creates a map with a single spawn gate, on terrain which is either walkable there or not.
    /// </summary>
    /// <param name="gateIsWalkable">Whether the tiles of the spawn gate are walkable.</param>
    /// <returns>The map definition.</returns>
    private static GameMapDefinition CreateMap(bool gateIsWalkable)
    {
        const byte gateX1 = 10;
        const byte gateY1 = 10;
        const byte gateX2 = 13;
        const byte gateY2 = 13;

        // Each map gets its own id, so the process-wide cache of HasWalkableSpawnGate can't carry a
        // verdict over from another test.
        var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Id = Guid.NewGuid(),
            Number = 0,
            TerrainData = new byte[ushort.MaxValue + 3],
        };

        if (!gateIsWalkable)
        {
            for (int x = gateX1; x <= gateX2; x++)
            {
                for (int y = gateY1; y <= gateY2; y++)
                {
                    // Only the values 0 (walkable) and 1 (safezone) are walkable - see GameMapTerrain.
                    map.TerrainData[3 + (y * 256) + x] = 4;
                }
            }
        }

        map.ExitGates.Add(new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = map,
            X1 = gateX1,
            Y1 = gateY1,
            X2 = gateX2,
            Y2 = gateY2,
            IsSpawnGate = true,
        });

        return map;
    }
}
