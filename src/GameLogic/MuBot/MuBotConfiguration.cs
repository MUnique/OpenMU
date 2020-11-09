// <copyright file="MuBotConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuBot
{
    /// <summary>
    /// Static MuBotConfiguration.
    /// </summary>
    public static class MuBotConfiguration
    {
        /// <summary>
        /// is mu bot available?.
        /// </summary>
        public const bool IsEnabled = true;

        /// <summary>
        /// the base cost of the system, later will be multiplied by player level + mlevel.
        /// </summary>
        public const uint Cost = 10000;

        /// <summary>
        /// the min level.
        /// </summary>
        public const uint MinLevel = 1;

        /// <summary>
        /// the max level.
        /// </summary>
        public const uint MaxLevel = 400;
    }
}
