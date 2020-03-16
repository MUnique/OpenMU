// <copyright file="GameConfigurationJsonQueryBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// <see cref="JsonQueryBuilder"/> for <see cref="GameConfiguration"/>.
    /// </summary>
    public class GameConfigurationJsonQueryBuilder : JsonQueryBuilder
    {
        /// <inheritdoc/>
        protected override IEnumerable<INavigation> GetNavigations(IEntityType entityType)
        {
            if (entityType.ClrType != typeof(GameConfiguration))
            {
                return base.GetNavigations(entityType);
            }

            var navigations = base.GetNavigations(entityType).ToList();

            // We move the maps with their spawn points to the end because they depend on all other data
            var mapsProperty = navigations.First(nav => nav.Name == "RawMaps");
            navigations.Remove(mapsProperty);
            navigations.Add(mapsProperty);
            return navigations;
        }
    }
}
