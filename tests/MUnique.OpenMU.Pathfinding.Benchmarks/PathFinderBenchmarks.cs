// <copyright file="PathFinderBenchmarks.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Benchmarks;

/// <summary>
/// Benchmarks for the <see cref="PathFinder"/> A* implementation.
/// Covers the production scenarios: short-range <see cref="ScopedGridNetwork"/>
/// searches (pooled in <c>GameLogic</c>) and long-range <see cref="FullGridNetwork"/>
/// searches (bot travel), for both successful and failed searches.
/// </summary>
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class PathFinderBenchmarks
{
    private PathFinder _scopedFinder = null!;
    private PathFinder _fullFinder = null!;
    private byte[,] _grid = null!;

    /// <summary>
    /// Global setup: builds a 256x256 terrain resembling the unit tests,
    /// plus a wall with a gap to force non-trivial searches.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        this._grid = new byte[0x100, 0x100];
        for (int x = 100; x < 200; x++)
        {
            for (int y = 100; y < 200; y++)
            {
                this._grid[x, y] = 10;
            }
        }

        // Safezone block:
        for (int x = 50; x < 100; x++)
        {
            for (int y = 50; y < 100; y++)
            {
                this._grid[x, y] = 0b1000_0001;
            }
        }

        // Vertical wall at x=120 (y 100..130) with a gap at y=115,
        // to force the longer benchmarks around an obstacle.
        for (int y = 100; y <= 130; y++)
        {
            if (y != 115)
            {
                this._grid[120, y] = 0;
            }
        }

        this._scopedFinder = new PathFinder(new ScopedGridNetwork());
        this._fullFinder = new PathFinder(new FullGridNetwork(true));
    }

    /// <summary>
    /// Short straight path on the scoped network (5 steps).
    /// </summary>
    [Benchmark(Baseline = true)]
    public IList<PathResultNode>? Scoped_ShortStraightPath()
    {
        return this._scopedFinder.FindPath(new Point(110, 100), new Point(115, 100), this._grid, false);
    }

    /// <summary>
    /// Diagonal path on the scoped network (10 steps).
    /// </summary>
    [Benchmark]
    public IList<PathResultNode>? Scoped_DiagonalPath()
    {
        return this._scopedFinder.FindPath(new Point(100, 100), new Point(110, 110), this._grid, false);
    }

    /// <summary>
    /// Longer path around a wall on the scoped network (max 16x16 segment).
    /// Start and end are on opposite sides of a wall with a single gap,
    /// forcing the search to detour.
    /// </summary>
    [Benchmark]
    public IList<PathResultNode>? Scoped_LongerPathAroundWall()
    {
        return this._scopedFinder.FindPath(new Point(118, 110), new Point(122, 120), this._grid, false);
    }

    /// <summary>
    /// Unreachable target on the scoped network (failure path).
    /// </summary>
    [Benchmark]
    public IList<PathResultNode>? Scoped_UnreachableTarget()
    {
        return this._scopedFinder.FindPath(new Point(110, 100), new Point(115, 99), this._grid, false);
    }

    /// <summary>
    /// Longer path on the full-grid network (bot travel scenario).
    /// </summary>
    [Benchmark]
    public IList<PathResultNode>? FullGrid_LongerPath()
    {
        return this._fullFinder.FindPath(new Point(100, 100), new Point(150, 150), this._grid, false);
    }

    /// <summary>
    /// Longer path with a maximum distance constraint (exercises distance checks).
    /// </summary>
    [Benchmark]
    public IList<PathResultNode>? Scoped_WithMaximumDistance()
    {
        this._scopedFinder.MaximumDistance = 100;
        try
        {
            return this._scopedFinder.FindPath(new Point(118, 110), new Point(122, 120), this._grid, false);
        }
        finally
        {
            this._scopedFinder.MaximumDistance = 0;
        }
    }
}
