// <copyright file="IItemSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Serializes the items into a byte array.
/// </summary>
public interface IItemSerializer : IViewPlugIn
{
    /// <summary>
    /// Gets the needed space for a serialized item.
    /// </summary>
    int NeededSpace { get; }

    /// <summary>
    /// Serializes the item into a byte array at the specified index.
    /// </summary>
    /// <param name="target">The target span.</param>
    /// <param name="item">The item.</param>
    /// <returns>The size of the serialized item.</returns>
    int SerializeItem(Span<byte> target, Item item);

    /// <summary>
    /// Deserializes the byte array into a new item instance.
    /// </summary>
    /// <param name="source">The source span.</param>
    /// <param name="gameConfiguration">The game configuration. Required to determine the item definition.</param>
    /// <param name="persistenceContext">The persistence context. Required to create new objects.</param>
    /// <returns>The created item instance.</returns>
    Item DeserializeItem(Span<byte> source, GameConfiguration gameConfiguration, IContext persistenceContext);
}