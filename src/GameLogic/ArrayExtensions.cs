// <copyright file="ArrayExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// Extensions for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Clears the elements of an array to the default values of the element type.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="array">The array.</param>
        public static void ClearToDefaults<T>(this T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default(T);
            }
        }

        /// <summary>
        /// Clears the elements of an array to the default values of the element type.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="array">The array.</param>
        public static void ClearToDefaults<T>(this T[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = default(T);
                }
            }
        }
    }
}