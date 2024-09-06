// <copyright file="PathFinder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;

/// <summary>
/// An implementation of the pathfinder which finds paths inside a two-dimensional grid.
/// Please note, that this path finder is not thread safe,
/// so only one search is allowed at the same time on one instance.
/// </summary>
public class PathFinder : IPathFinder
{
    private static readonly Meter Meter = new(MeterName);
    private static readonly Counter<long> CurrentSearches = Meter.CreateCounter<long>("CurrentSearches");
    private static readonly Counter<long> CompletedSearches = Meter.CreateCounter<long>("CompletedSearches");
    private static readonly Counter<long> FailedSearches = Meter.CreateCounter<long>("FailedSearches");
    private static readonly Histogram<double> DurationCompletedMs = Meter.CreateHistogram<double>("DurationCompletedMs");
    private static readonly Histogram<double> DurationFailedMs = Meter.CreateHistogram<double>("DurationFailedMs");

    private readonly INetwork _network;
    private readonly IPriorityQueue<Node> _openList;

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
    /// Gets the name of the meter of this class.
    /// </summary>
    public static string MeterName => typeof(PathFinder).FullName ?? nameof(PathFinder);

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
    public IList<PathResultNode>? FindPath(Point start, Point end, byte[,] terrain, bool includeSafezone, CancellationToken cancellationToken = default)
    {
        CurrentSearches.Add(1);
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = this.FindPathInner(start, end, terrain, includeSafezone, cancellationToken);
            var elapsedMs = (double)stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond;
            if (result is null)
            {
                FailedSearches.Add(1);
                DurationFailedMs.Record(elapsedMs);
            }
            else
            {
                CompletedSearches.Add(1);
                DurationCompletedMs.Record(elapsedMs);
            }

            return result;
        }
        finally
        {
            CurrentSearches.Add(-1);
        }
    }

    private IList<PathResultNode>? FindPathInner(Point start, Point end, byte[,] terrain, bool includeSafezone, CancellationToken cancellationToken)
    {
        if (this.MaximumDistanceExceeded(start, end))
        {
            return null;
        }

        var pathFound = false;
        this._openList.Clear();
        if (!this._network.Prepare(start, end, terrain, includeSafezone))
        {
            return null;
        }

        if (this.Heuristic != null)
        {
            this.Heuristic.HeuristicEstimateMultiplier = this.HeuristicEstimate;
        }

        var closeNodeCounter = 0;
        var startNode = this._network.GetNodeAt(start);
        if (startNode is null)
        {
            return null;
        }

        startNode.PredictedTotalCost = 2;
        startNode.PreviousNode = startNode;
        startNode.Status = NodeStatus.Open;
        this._openList.Push(startNode);
        while (this._openList.Count > 0 && !cancellationToken.IsCancellationRequested)
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
            return this.GetCalculatedPath(end).Reverse().ToList();
        }

        return null;
    }

    /// <summary>
    /// Resets the pathfinder.
    /// </summary>
    public void ResetPathFinder()
    {
        this._openList.Clear();
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