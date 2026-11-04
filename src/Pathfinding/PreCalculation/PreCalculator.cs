// <copyright file="PreCalculator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.Threading;

/// <summary>
/// With this class you can pre-calculate all paths of a given map and save it in a compact way.
/// </summary>
/// <remarks>
/// For more information, see http://www.gamedev.net/page/resources/_/technical/artificial-intelligence/precalculated-pathfinding-revisited-r1939.
/// </remarks>
public class PreCalculator
{
    /// <summary>
    /// Pres calcuates the paths from every possible point of the <paramref name="aiGrid" /> to any point of the <paramref name="aiGrid" /> in the <paramref name="maximumRange" />.
    /// </summary>
    /// <param name="aiGrid">The ai grid of the map, which includes the costs of moving to a specific coordinate.</param>
    /// <param name="walkMap">The grid of walkable coordinates of the map.</param>
    /// <param name="maximumRange">The maximum range.</param>
    /// <returns>All calculated path informations.</returns>
    public IEnumerable<PathInfo> PreCalcuatePaths(byte[,] aiGrid, bool[,] walkMap, int maximumRange)
    {
        var grid = aiGrid;
        int finished = 0;
        var resultList = new List<PathInfo>[256];
        Parallel.For(0, 256, new ParallelOptions { MaxDegreeOfParallelism = 4 }, (x) =>
        {
            var network = new FullGridNetwork(true);
            var pathFinder = new PathFinder(network);
            var result = new List<PathInfo>();
            resultList[x] = result;
            for (int y = 0; y < 256; y++)
            {
                if (!walkMap[x, y])
                {
                    continue;
                }

                result.AddRange(this.FindPaths(new Point((byte)x, (byte)y), walkMap, aiGrid, pathFinder, maximumRange));
            }

            Interlocked.Increment(ref finished);
        });

        return resultList.SelectMany(pathInfo => pathInfo);
    }

    private IEnumerable<PathInfo> FindPaths(Point start, bool[,] map, byte[,] aiGrid, IPathFinder pathFinder, int maxDistance)
    {
        byte toX = (byte)Math.Min(start.X + maxDistance - 1, 0xFF);
        byte toY = (byte)Math.Min(start.Y + maxDistance - 1, 0xFF);
        byte fromX = (byte)Math.Max(start.X - maxDistance, 0);
        byte fromY = (byte)Math.Max(start.Y - maxDistance, 0);
        for (byte x = fromX; x <= toX; x++)
        {
            for (byte y = fromY; y <= toY; y++)
            {
                if (!map[x, y] || (x == start.X && y == start.Y))
                {
                    continue;
                }

                var nodes = pathFinder.FindPath(new Point(x, y), start, aiGrid, false);
                if (nodes is { Count: > 0 })
                {
                    var firstNode = nodes[0];

                    yield return new PathInfo(new PointCombination(new Point(start.X, start.Y), new Point(x, y)), new Point(firstNode.X, firstNode.Y));
                }
            }
        }
    }
}