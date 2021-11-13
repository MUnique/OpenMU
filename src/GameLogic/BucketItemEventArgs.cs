// <copyright file="BucketItemEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Event args which includes the involved bucket item.
/// </summary>
/// <typeparam name="T">The type of the bucket item.</typeparam>
public class BucketItemEventArgs<T> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BucketItemEventArgs{T}"/> class.
    /// </summary>
    /// <param name="item">The bucket item.</param>
    public BucketItemEventArgs(T item)
    {
        this.Item = item;
    }

    /// <summary>
    /// Gets the bucket item which is involved in the event.
    /// </summary>
    public T Item { get; }
}