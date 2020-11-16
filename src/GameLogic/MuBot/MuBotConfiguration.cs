// <copyright file="MuBotConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuBot
{
    /// <summary>
    /// Configuration for MuBot.
    /// </summary>
    public class MuBotConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether mu bot is available.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or Sets the base cost of the system.
        /// </summary>
        public int Cost { get; set; } = 10000;

        /// <summary>
        /// Gets or Sets the min level.
        /// </summary>
        public int MinLevel { get; set; } = 1;

        /// <summary>
        /// Gets or Sets the max level.
        /// </summary>
        public int MaxLevel { get; set; } = 400;
    }
}
