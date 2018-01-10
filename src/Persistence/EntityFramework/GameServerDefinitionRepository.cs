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
        /// <param name="repositoryManager">The repository manager.</param>
        public GameServerDefinitionRepository(IRepositoryManager repositoryManager)
            : base(repositoryManager)
        {
        }

        /// <inheritdoc />
        protected override void LoadDependentData(object obj, DbContext currentContext)
        {
            // TODO: TEST IF REALLY REQUIRED!
            if (obj is GameServerDefinition definition)
            {
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