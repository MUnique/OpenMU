// <copyright file="PathFinder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of the pathfinder which finds paths inside a two-dimensional grid.
    /// Please note, that this path finder is not thread safe,
    /// so only one search is allowed at the same time on one instance.
    /// </summary>
    public class PathFinder : IPathFinder
    {
        private readonly INetwork network;
        private readonly IPriorityQueue<Node> openList;
        private readonly IList<PathResultNode> resultList = new List<PathResultNode>();
        private bool stop;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFinder"/> class.
        /// </summary>
        /// <param name="network">The network on which the pathfinder should operate.</param>
        public PathFinder(INetwork network)
            : this(network, new BinaryMinHeap<Node>(new NodeComparer()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFinder"/> class.
        /// </summary>
        /// <param name="network">The network on which the pathfinder should operate.</param>
        /// <param name="openList">The open list.</param>
        public PathFinder(INetwork network, IPriorityQueue<Node> openList)
        {
            this.network = network;
            this.openList = openList;
        }

        /// <summary>
        /// Gets or sets the maximum distance until which the path should be resolved.
        /// </summary>
        public int MaximumDistance { get; set; }

        /// <summary>
        /// Gets or sets the search limit.
        /// </summary>
        public int SearchLimit { get; set; } = 500;

        /// <summary>
        /// Gets or sets the heuristic estimate.
        /// </summary>
        public int HeuristicEstimate { get; set; } = 2;

        /// <summary>
        /// Gets or sets the heuristic.
        /// </summary>
        public IHeuristic Heuristic { get; set; } = new NoHeuristic();

        /// <inheritdoc/>
        public IList<PathResultNode> FindPath(Point start, Point end)
        {
            if (this.MaximumDistanceExceeded(start, end))
            {
                return null;
            }

            var pathFound = false;
            this.stop = false;
            this.openList.Clear();
            this.network.ResetStatus();
            if (this.Heuristic != null)
            {
                this.Heuristic.HeuristicEstimateMultiplier = this.HeuristicEstimate;
            }

            var closeNodeCounter = 0;
            var startNode = this.network.GetNodeAt(start);
            startNode.PredictedTotalCost = 2;
            startNode.PreviousNode = startNode;
            startNode.Status = NodeStatus.Open;
            this.openList.Push(startNode);
            while (this.openList.Count > 0 && !this.stop)
            {
                var node = this.openList.Pop();
                if (node.Status == NodeStatus.Closed)
                {
                    continue;
                }

                if (node.X == end.X && node.Y == end.Y)
                {
                    node.Status = NodeStatus.Closed;
                    pathFound = true;
                    break;
                }

                if (closeNodeCounter > this.SearchLimit)
                {
                    return null;
                }

                this.ExpandNodes(node, start, end);
                node.Status = NodeStatus.Closed;
                closeNodeCounter++;
            }

            if (pathFound)
            {
                this.BuildResultList(end);
                return this.resultList;
            }

            return null;
        }

        /// <summary>
        /// Stops the pathfinder.
        /// </summary>
        public void StopPathfinder()
        {
            this.stop = true;
        }

        /// <summary>
        /// Resets the pathfinder.
        /// </summary>
        public void ResetPathFinder()
        {
            this.openList.Clear();
            this.resultList.Clear();
            this.stop = false;
        }

        private void ExpandNodes(Node node, Point start, Point end)
        {
            foreach (var newNode in this.network.GetPossibleNextNodes(node))
            {
                if (this.MaximumDistanceExceeded(start, end, newNode))
                {
                    continue;
                }

                var heuristicEstimate = this.Heuristic?.CalculateHeuristicDistance(newNode.Position, end) ?? 0;
                newNode.PredictedTotalCost = newNode.CostUntilNow + heuristicEstimate;
                newNode.Status = NodeStatus.Open;
                newNode.PreviousNode = node;
                this.openList.Push(newNode);
            }
        }

        private void BuildResultList(Point end)
        {
            this.resultList.Clear();
            foreach (var node in this.GetCalculatedPath(end).Reverse())
            {
                this.resultList.Add(node);
            }
        }

        private IEnumerable<PathResultNode> GetCalculatedPath(Point end)
        {
            Node node = this.network.GetNodeAt(end);
            while (node.PreviousNode != node)
            {
                yield return new PathResultNode(node.Position, node.PreviousNode.Position);
                node = node.PreviousNode;
            }
        }

        private bool MaximumDistanceExceeded(Point start, Point end)
        {
            if (this.MaximumDistance != 0)
            {
                return start.EuclideanDistanceTo(end) > this.MaximumDistance;
            }

            return false;
        }

        private bool MaximumDistanceExceeded(Point start, Point end, Node node)
        {
            if (this.MaximumDistance != 0)
            {
                var distance = start.EuclideanDistanceTo(node.Position);
                distance += node.Position.EuclideanDistanceTo(end);
                return distance > this.MaximumDistance;
            }

            return false;
        }
    }
}
