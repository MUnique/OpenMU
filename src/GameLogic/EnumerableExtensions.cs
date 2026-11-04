// <copyright file="EnumerableExtensions.cs" company="MUnique">
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
    /// Executes the <paramref name="action"/> for each element of <paramref name="enumerable"/>.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action which should be executed for each element.</param>
    /// <exception cref="System.ArgumentNullException">action.</exception>
    public static async ValueTask ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, ValueTask> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var item in enumerable)
        {
            await action(item).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Returns the enumerable as list, by either casting it or creating it.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>The list.</returns>
    public static IList<T> AsList<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is IList<T> list)
        {
            return list;
        }

        return enumerable.ToList();
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

    /// <summary>
    /// Selects a random weighted element of an enumerable.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="weights">The weights associated with <paramref name="enumerable" />, respectively.</param>
    /// <param name="randomizer">The randomizer.</param>
    /// <returns>The randomly selected weighted element.</returns>
    public static T? SelectWeightedRandom<T>(this IEnumerable<T> enumerable, IEnumerable<int> weights, IRandomizer randomizer)
    {
        var list = enumerable as IList<T> ?? enumerable.ToList();
        var weightList = weights as IList<int> ?? weights.ToList();
        if (list.Count > 0 && weightList.Count == list.Count)
        {
            var roll = randomizer.NextInt(0, weights.Sum());
            int inc = 0;
            for (int i = 0; i < weightList.Count; i++)
            {
                inc += weightList[i];
                if (roll < inc)
                {
                    return list[i];
                }
            }

            return SelectRandom(enumerable, randomizer);  // Fallback in case there are no weights assigned (>0)
        }

        return default;
    }

    /// <summary>
    /// Selects a random weighted element of an enumerable.
    /// </summary>
    /// <typeparam name="T">The generic type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="weights">The weights associated with <paramref name="enumerable" />, respectively.</param>
    /// <returns>The randomly selected weighted element.</returns>
    public static T? SelectWeightedRandom<T>(this IEnumerable<T> enumerable, IEnumerable<int> weights) => SelectWeightedRandom(enumerable, weights, Rand.GetRandomizer());
}