// <copyright file="FruitCalculationStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// The calculation strategy for maximum fruit points.
    /// </summary>
    public enum FruitCalculationStrategy
    {
        /// <summary>
        /// The default strategy (maximum 127).
        /// </summary>
        Default = 0,

        /// <summary>
        /// The strategy to calculate the fruits for magic gladiator classes (maximum 100).
        /// </summary>
        MagicGladiator = 1,

        /// <summary>
        /// The strategy to calculate the fruits for dark lord classes (maximum 115).
        /// </summary>
        DarkLord = 2,
    }
}