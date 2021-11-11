﻿// <copyright file="PathFinderTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Tests;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests for the pathfinder.
/// </summary>
[TestFixture]
public class PathFinderTest
{
    private IPathFinder _pathFinder = null!;

    /// <summary>
    /// Sets up the path finder with a basic, unrestricted grid.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var grid = new byte[0x100, 0x100];
        for (int x = 100; x < 200; x++)
        {
            for (int y = 100; y < 200; y++)
            {
                grid[x, y] = 10;
            }
        }

        this._pathFinder = new PathFinder(new GridNetwork(grid, true));
    }

    /// <summary>
    /// Tests the straight path.
    /// </summary>
    [Test]
    public void TestStraightPath()
    {
        var start = new Point(110, 100);
        var end = new Point(115, 100);
        var result = this._pathFinder.FindPath(start, end);
        Assert.That(result, Is.Not.Null);
        var lastNode = result!.LastOrDefault();
        Assert.That(lastNode, Is.Not.Null);
        Assert.That(lastNode.X, Is.EqualTo(end.X));
        Assert.That(lastNode.Y, Is.EqualTo(end.Y));
    }

    /// <summary>
    /// Tests the diagonal path.
    /// </summary>
    [Test]
    public void TestDiagonalPath()
    {
        var start = new Point(100, 100);
        var end = new Point(110, 110);
        var result = this._pathFinder.FindPath(start, end);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Count, Is.EqualTo(10));
        for (int i = 1; i <= 10; i++)
        {
            Assert.That(result[i - 1].X, Is.EqualTo(start.X + i));
            Assert.That(result[i - 1].Y, Is.EqualTo(start.Y + i));
        }

        Assert.That(result.Last().X, Is.EqualTo(end.X));
        Assert.That(result.Last().Y, Is.EqualTo(end.Y));
    }

    /// <summary>
    /// Tests if no path can be found if the end is on an unreachable coordinate.
    /// </summary>
    [Test]
    public void TestNoPathFound()
    {
        var start = new Point(110, 100);
        var end = new Point(115, 99);
        var result = this._pathFinder.FindPath(start, end);
        Assert.That(result, Is.Null);
    }
}