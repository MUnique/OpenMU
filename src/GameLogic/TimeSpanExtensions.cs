// <copyright file="TimeSpanExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// Extension methods for <see cref="TimeSpan"/>s.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns at least a <see cref="TimeSpan"/> with a value of a minimum value.
        /// </summary>
        /// <param name="timeSpan">This <see cref="TimeSpan"/>.</param>
        /// <param name="minimum">The minimum <see cref="TimeSpan"/>.</param>
        /// <returns>The maximum value of both.</returns>
        public static TimeSpan AtLeast(this TimeSpan timeSpan, TimeSpan minimum)
        {
            if (timeSpan > minimum)
            {
                return timeSpan;
            }

            return minimum;
        }

        /// <summary>
        /// Returns at most a <see cref="TimeSpan"/> with a value of a maximum value.
        /// </summary>
        /// <param name="timeSpan">This <see cref="TimeSpan"/>.</param>
        /// <param name="maximum">The maximum <see cref="TimeSpan"/>.</param>
        /// <returns>The minimum value of both.</returns>
        public static TimeSpan AtMost(this TimeSpan timeSpan, TimeSpan maximum)
        {
            if (timeSpan < maximum)
            {
                return timeSpan;
            }

            return maximum;
        }
    }
}
