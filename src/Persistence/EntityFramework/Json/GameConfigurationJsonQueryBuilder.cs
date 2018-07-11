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
        /// <summary>
        /// The property of <see cref="GameMapDefinition.RawSafezoneMap"/> which should be serialized as reference.
        /// </summary>
        private static readonly PropertyInfo SafezoneMapProperty = typeof(GameMapDefinition).GetProperty(nameof(GameMapDefinition.RawSafezoneMap));

        /// <summary>
        /// The property of <see cref="EnterGate.RawTargetGate"/> which should be serialized as reference.
        /// </summary>
        private static readonly PropertyInfo TargetGateProperty = typeof(EnterGate).GetProperty(nameof(EnterGate.RawTargetGate));

        /// <inheritdoc/>
        protected override bool SelectReferences(string parentAlias, INavigation navigation)
        {
            return base.SelectReferences(parentAlias, navigation)
                || navigation.PropertyInfo == SafezoneMapProperty
                || navigation.PropertyInfo == TargetGateProperty;
        }

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
