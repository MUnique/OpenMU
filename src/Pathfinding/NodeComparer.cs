// <copyright file="NodeComparer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A comparer to sort nodes based on their <see cref="Node.PredictedTotalCost"/> value.
    /// </summary>
    public class NodeComparer : IComparer<Node>
    {
        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "We are sure that these parameters are never null. A check would reduce performance.")]
        public int Compare(Node a, Node b) => a.PredictedTotalCost - b.PredictedTotalCost;
    }
}
