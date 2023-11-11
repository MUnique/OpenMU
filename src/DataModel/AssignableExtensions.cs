// <copyright file="AssignableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extensions for <see cref="IAssignable{T}"/>.
/// </summary>
public static class AssignableExtensions
{
    /// <summary>
    /// Assigns a collection to another one, resolving the objects to the ones
    /// which are contained in the given <see cref="GameConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection elements.</typeparam>
    /// <param name="collection">The target collection.</param>
    /// <param name="other">The other collection.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void AssignCollection<T>(this ICollection<T> collection, ICollection<T> other, GameConfiguration gameConfiguration)
        where T : class
    {
        var newItems = collection.Except(other).ToList();
        var oldItems = other.Except(collection).ToList();

        oldItems.ForEach(i => collection.Remove(i));
        newItems.ForEach(i => collection.Add(
            gameConfiguration.GetObjectOfConfig(i)
            ?? (i as ICloneable<T>)?.Clone(gameConfiguration)
            ?? (i as ICloneable)?.Clone() as T
            ?? i));
    }

    /// <summary>
    /// Assigns a collection to another one.
    /// </summary>
    /// <typeparam name="T">The type of the collection values.</typeparam>
    /// <param name="collection">The target collection.</param>
    /// <param name="other">The other collection.</param>
    public static void AssignCollection<T>(this ICollection<T> collection, ICollection<T> other)
        where T : struct
    {
        var newItems = collection.Except(other).ToList();
        var oldItems = other.Except(collection).ToList();

        oldItems.ForEach(i => collection.Remove(i));
        newItems.ForEach(collection.Add);
    }
}