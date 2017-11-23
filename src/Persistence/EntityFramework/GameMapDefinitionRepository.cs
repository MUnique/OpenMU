// <copyright file="GameMapDefinitionRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// Repository for <see cref="GameMapDefinition"/>s. It implements <see cref="ILoadByProperty"/>, so that loading <see cref="GameServerConfiguration.RawMaps"/> is possible.
    /// </summary>
    internal class GameMapDefinitionRepository : ConfigurationTypeRepository<GameMapDefinition>, ILoadByProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapDefinitionRepository"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="collectionSelector">The collection selector which returns the game map definitions of the game configuration.</param>
        public GameMapDefinitionRepository(IRepositoryManager repositoryManager, Func<GameConfiguration, ICollection<GameMapDefinition>> collectionSelector)
            : base(repositoryManager, collectionSelector)
        {
        }

        /// <summary>
        /// Loads the objects by property.
        /// </summary>
        /// <param name="property">The property of the object which should be compared.</param>
        /// <param name="propertyValue">The value of the property.</param>
        /// <returns>The enumeration of the loaded objects.</returns>
        /// <exception cref="System.NotImplementedException">Loading by a property other than GameServerConfigurationId is not implemented.</exception>
        public IEnumerable LoadByProperty(IProperty property, object propertyValue)
        {
            if (property.Name.StartsWith(nameof(GameServerConfiguration)))
            {
                var context = this.RepositoryManager.GetCurrentContext() as EntityFrameworkContext;
                if (context == null)
                {
                    throw new InvalidOperationException($"Current context is not set or not of type {nameof(EntityFrameworkContext)}.");
                }

                return this.GetAll().Where(map => context.Context.Entry(map).Property(property.Name).CurrentValue == propertyValue);
            }

            throw new NotImplementedException($"Loading by property {property} is not implemented.");
        }
    }
}