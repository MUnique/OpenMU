// <copyright file="NodeIndexer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// An indexer for a <see cref="Node"/>, which can be used to calculate the index on the binary heap.
/// The index is the predicted total cost to reach the node, divided by 10. So every 10 values,
/// there is one index entry in the <see cref="IndexedLinkedList{T}._index"/>.
/// </summary>
internal class NodeIndexer : IIndexer<Node>
{
    /// <inheritdoc/>
    public int GetIndexValue(Node item)
    {
        return item.PredictedTotalCost / 10;
    }
}