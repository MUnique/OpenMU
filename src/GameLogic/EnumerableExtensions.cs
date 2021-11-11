﻿// <copyright file="EnumerableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Executes the <paramref name="action"/> for each element of <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action which should be executed for each element.</param>
    /// <exception cref="System.ArgumentNullException">action.</exception>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>
    /// Selects a random element of an enumerable.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="randomizer">The randomizer.</param>
    /// <returns>The randomly selected element.</returns>
    public static T? SelectRandom<T>(this IEnumerable<T> enumerable, IRandomizer randomizer)
    {
        var list = enumerable as IList<T> ?? enumerable.ToList();
        if (list.Count > 0)
        {
            var index = randomizer.NextInt(0, list.Count);
            return list[index];
        }

        return default;
    }

    /// <summary>
    /// Selects a random element of an enumerable.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The randomly selected element.</returns>
    public static T? SelectRandom<T>(this IEnumerable<T> enumerable) => SelectRandom(enumerable, Rand.GetRandomizer());
}