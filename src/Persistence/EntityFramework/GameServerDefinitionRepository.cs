// <copyright file="GameServerDefinitionRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for the <see cref="GameServerDefinition"/>.
    /// It sets the <see cref="EntityDataContext.CurrentGameConfiguration"/> before loading other dependent data which tries to load the configured maps of a server.
    /// </summary>
    internal class GameServerDefinitionRepository : CachingGenericRepository<GameServerDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerDefinitionRepository"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public GameServerDefinitionRepository(RepositoryManager repositoryManager)
            : base(repositoryManager)
        {
        }

        /// <inheritdoc />
        protected override void LoadDependentData(object obj, DbContext currentContext)
        {
            if (obj is GameServerDefinition definition)
            {
                var entityEntry = currentContext.Entry(obj);
                foreach (var collection in entityEntry.Collections.Where(c => !c.IsLoaded))
                {
                    this.LoadCollection(entityEntry, collection.Metadata, currentContext);
                    collection.IsLoaded = true;
                }

                if (definition.GameConfigurationId.HasValue)
                {
                    definition.RawGameConfiguration =
                        this.RepositoryManager.GetRepository<GameConfiguration>()
                            .GetById(definition.GameConfigurationId.Value);

                    if (currentContext is EntityDataContext context)
                    {
                       context.CurrentGameConfiguration = definition.RawGameConfiguration;
                    }
                }

                if (definition.ServerConfigurationId.HasValue)
                {
                    definition.ServerConfiguration = this.RepositoryManager.GetRepository<GameServerConfiguration>()
                        .GetById(definition.ServerConfigurationId.Value);
                }
            }
        }
    }
}