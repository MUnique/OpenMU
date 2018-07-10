// <copyright file="StatsExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Extensions for <see cref="Stats"/>.
    /// </summary>
    internal static class StatsExtensions
    {
        /// <summary>
        /// Gets the persistent instance of this attribute from the game configuration.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>The persistent instance of this attribute from the game configuration.</returns>
        public static AttributeDefinition GetPersistent(this AttributeDefinition attribute, GameConfiguration gameConfiguration)
        {
            return gameConfiguration.Attributes.First(a => attribute.Id == a.Id);
        }
    }
}
