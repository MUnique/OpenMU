// <copyright file="PathFinder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// An implementation of the pathfinder which finds paths inside a two-dimensional grid.
/// Please note, that this path finder is not thread safe,
/// so only one search is allowed at the same time on one instance.
/// </summary>
public class PathFinder : IPathFinder
{
    private readonly INetwork _network;
    private readonly IPriorityQueue<Node> _openList;
    private readonly IList<PathResultNode> _resultList = new List<PathResultNode>();
    private bool _stop;

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
        this._network = network;
        this._openList = openList;
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
    public IList<PathResultNode>? FindPath(Point start, Point end)
    {
        if (this.MaximumDistanceExceeded(start, end))
        {
            return null;
        }

        var pathFound = false;
        this._stop = false;
        this._openList.Clear();
        this._network.ResetStatus();
        if (this.Heuristic != null)
        {
            this.Heuristic.HeuristicEstimateMultiplier = this.HeuristicEstimate;
        }

        var closeNodeCounter = 0;
        var startNode = this._network.GetNodeAt(start);
        startNode.PredictedTotalCost = 2;
        startNode.PreviousNode = startNode;
        startNode.Status = NodeStatus.Open;
        this._openList.Push(startNode);
        while (this._openList.Count > 0 && !this._stop)
        {
            var node = this._openList.Pop();
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
            return this._resultList;
        }

        return null;
    }

    /// <summary>
    /// Stops the pathfinder.
    /// </summary>
    public void StopPathfinder()
    {
        this._stop = true;
    }

    /// <summary>
    /// Resets the pathfinder.
    /// </summary>
    public void ResetPathFinder()
    {
        this._openList.Clear();
        this._resultList.Clear();
        this._stop = false;
    }

    private void ExpandNodes(Node node, Point start, Point end)
    {
        foreach (var newNode in this._network.GetPossibleNextNodes(node))
        {
            if (this.MaximumDistanceExceeded(start, end, newNode))
            {
                continue;
            }

            var heuristicEstimate = this.Heuristic?.CalculateHeuristicDistance(newNode.Position, end) ?? 0;
            newNode.PredictedTotalCost = newNode.CostUntilNow + heuristicEstimate;
            newNode.Status = NodeStatus.Open;
            newNode.PreviousNode = node;
            this._openList.Push(newNode);
        }
    }

    private void BuildResultList(Point end)
    {
        this._resultList.Clear();
        foreach (var node in this.GetCalculatedPath(end).Reverse())
        {
            this._resultList.Add(node);
        }
    }

    private IEnumerable<PathResultNode> GetCalculatedPath(Point end)
    {
        var node = this._network.GetNodeAt(end);
        while (node!.PreviousNode != node)
        {
            yield return new PathResultNode(node.Position, node.PreviousNode!.Position);
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