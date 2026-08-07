// <copyright file="WorldCoordinatesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.World;

/// <summary>
/// Tests for global world coordinates.
/// </summary>
[TestFixture]
internal class WorldCoordinatesTest
{
    [Test]
    public void GlobalPoint_ConvertsToChunkAndLocalCoordinates()
    {
        var result = new GlobalPoint(513, 767).ToChunkLocal();

        Assert.That(result.Chunk, Is.EqualTo(new ChunkId(2, 2)));
        Assert.That(result.Local, Is.EqualTo(new LocalPoint(1, 255)));
    }

    [Test]
    public void ChunkAndLocalCoordinates_ConvertBackToGlobalCoordinates()
    {
        var global = new ChunkId(255, 255).ToGlobal(new LocalPoint(255, 255));

        Assert.That(global, Is.EqualTo(new GlobalPoint(ushort.MaxValue, ushort.MaxValue)));
    }

    [Test]
    public void GlobalPoint_RoundTripsAtChunkBoundaries()
    {
        var original = new GlobalPoint(256, 512);
        var coordinates = original.ToChunkLocal();

        Assert.That(coordinates.Local, Is.EqualTo(new LocalPoint(0, 0)));
        Assert.That(coordinates.Local.ToGlobal(coordinates.Chunk), Is.EqualTo(original));
    }

    [Test]
    public void PointSupportsCoordinatesBeyondLegacyByteRange()
    {
        var point = new MUnique.OpenMU.Pathfinding.Point(511, 1023);

        Assert.That(point.X, Is.EqualTo(511));
        Assert.That(point.Y, Is.EqualTo(1023));
    }
}
