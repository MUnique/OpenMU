// <copyright file="ItemPostState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemPost;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Holds the state for posted items so that other players can request the item details.
/// </summary>
public class ItemPostState
{
    private readonly ConcurrentDictionary<uint, Item> _postedItems = new();

    private int _postCounter;

    /// <summary>
    /// Tries to get the posted item for the specified <paramref name="postId"/>.
    /// </summary>
    /// <param name="postId">The id of the posted item.</param>
    /// <param name="item">The posted item, if found.</param>
    /// <returns><see langword="true"/> if an item was found.</returns>
    public bool TryGetItem(uint postId, out Item? item)
    {
        var result = this._postedItems.TryGetValue(postId, out item);
        return result;
    }

    /// <summary>
    /// Registers the specified <paramref name="item"/> as posted and returns the assigned id.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The assigned post id.</returns>
    public uint AddItem(Item item)
    {
        var id = unchecked((uint)Interlocked.Increment(ref this._postCounter));
        this._postedItems[id] = item;
        return id;
    }
}
