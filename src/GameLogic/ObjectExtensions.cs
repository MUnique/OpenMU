// <copyright file="ObjectExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets a single object as enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The enumerable with <paramref name="obj"/> as the only result.</returns>
        public static IEnumerable<T> GetAsEnumerable<T>(this T obj)
            where T : class
        {
            if (obj == null)
            {
                yield break;
            }

            yield return obj;
        }
    }
}
