// <copyright file="PreCalculatedPathFinder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Path finder which uses pre calculated paths.
    /// </summary>
    /// <seealso cref="OpenMU.Pathfinding.IPathFinder" />
    public class PreCalculatedPathFinder : IPathFinder
    {
        private readonly IDictionary<PointCombination, Point> nextSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreCalculatedPathFinder"/> class.
        /// </summary>
        /// <param name="pathInfos">The path infos.</param>
        public PreCalculatedPathFinder(IEnumerable<PathInfo> pathInfos)
        {
            this.nextSteps = pathInfos.ToDictionary(info => info.Combination, info => info.NextStep);
        }

        /// <inheritdoc/>
        public IList<PathResultNode> FindPath(Point start, Point end)
        {
            var result = new List<PathResultNode>();
            Point nextStep;
            while (this.nextSteps.TryGetValue(new PointCombination(start, end), out nextStep))
            {
                result.Add(new PathResultNode(nextStep, start));
                start = nextStep;
            }

            if (result.Count == 0 || nextStep != end)
            {
                return null;
            }

            return result;
        }
    }
}
