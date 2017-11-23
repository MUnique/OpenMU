// <copyright file="IRandomizer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    /// <summary>
    /// Description of IRandomizer.
    /// </summary>
    public interface IRandomizer
    {
        /// <summary>
        /// Returns the next random boolean value.
        /// </summary>
        /// <returns>The next random boolean value.</returns>
        bool NextRandomBool();

        /// <summary>
        /// Returns the next random boolean value, with a chance of <paramref name="percent"/> of being true.
        /// </summary>
        /// <param name="percent">The percent of the chance of being true.</param>
        /// <returns>The next random boolean value.</returns>
        bool NextRandomBool(int percent);

        /// <summary>
        /// Returns the next random boolean value, with a <paramref name="chance"/> of <paramref name="basis"/> of being true.
        /// </summary>
        /// <param name="chance">The chance of <paramref name="basis"/> of being true.</param>
        /// <param name="basis">The basis.</param>
        /// <returns>The next random boolean value.</returns>
        bool NextRandomBool(int chance, int basis);

        /// <summary>
        /// Returns the next random boolean value, with the <paramref name="chance"/> of being true.
        /// </summary>
        /// <param name="chance">The chance of being true.</param>
        /// <returns>The next random boolean value.</returns>
        bool NextRandomBool(double chance);

        /// <summary>
        /// The next random integer.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The next random integer between <paramref name="min"/> and <paramref name="max"/>.</returns>
        int NextInt(int min, int max);

        /// <summary>
        /// The next random integer.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The next random integer between <paramref name="min"/> and <paramref name="max"/>.</returns>
        int NextInt(uint min, uint max);

        /// <summary>
        /// The next random unsigned integer.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The next random integer between <paramref name="min"/> and <paramref name="max"/>.</returns>
        uint NextUInt(uint min, uint max);

        /// <summary>
        /// The next random double between 0 and 1.
        /// </summary>
        /// <returns>The next random integer between 0 and 1.</returns>
        double NextDouble();
    }
}
