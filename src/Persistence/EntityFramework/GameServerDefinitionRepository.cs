// <copyright file="GameServerDefinitionRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for the <see cref="GameServerDefinition"/>.
    /// It sets the <see cref="EntityDataContext.CurrentGameConfiguration"/> before loading other dependent data which tries to load the configured maps of a server.
    /// </summary>
    internal class GameServerDefinitionRepository : GenericRepository<GameServerDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerDefinitionRepository"/> class.
        /// </summary>
        /// <param name="contextProvider">The repository manager.</param>
        public GameServerDefinitionRepository(PersistenceContextProvider contextProvider)
            : base(contextProvider)
        {
        }

        /// <inheritdoc />
        protected override void LoadDependentData(object obj, DbContext currentContext)
        {
            if (obj is GameServerDefinition definition)
            {
                if (definition.GameConfigurationId.HasValue)
                {
                    definition.RawGameConfiguration =
                        this.ContextProvider.RepositoryManager.GetRepository<GameConfiguration>()
                            .GetById(definition.GameConfigurationId.Value);

                    if (currentContext is EntityDataContext context)
                    {
                        context.CurrentGameConfiguration = definition.RawGameConfiguration;
                    }
                }

                if (definition.ServerConfigurationId.HasValue)
                {
                    definition.ServerConfiguration = this.ContextProvider.RepositoryManager.GetRepository<GameServerConfiguration>()
                        .GetById(definition.ServerConfigurationId.Value);
                }
            }
        }
    }
}