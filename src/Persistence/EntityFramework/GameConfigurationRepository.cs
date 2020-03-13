// <copyright file="GameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    using MUnique.OpenMU.Persistence.EntityFramework.Json;

    /// <summary>
    /// The game configuration repository, which loads the configuration by using the
    /// <see cref="JsonObjectLoader"/>, to speed up loading the whole object graph.
    /// </summary>
    internal class GameConfigurationRepository : GenericRepository<GameConfiguration>
    {
        private readonly JsonObjectLoader objectLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfigurationRepository"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public GameConfigurationRepository(RepositoryManager repositoryManager)
            : base(repositoryManager)
        {
            this.objectLoader = new GameConfigurationJsonObjectLoader();
        }

        /// <inheritdoc />
        public override GameConfiguration GetById(Guid id)
        {
            var currentContext = this.RepositoryManager.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
            if (currentContext == null)
            {
                throw new InvalidOperationException("There is no current context set.");
            }

            var database = currentContext.Context.Database;
            database.OpenConnection();
            try
            {
                var config = this.objectLoader.LoadObject<GameConfiguration>(id, currentContext.Context);
                currentContext.Attach(config);
                return config;
            }
            finally
            {
                database.CloseConnection();
            }
        }

        /// <inheritdoc />
        public override IEnumerable<GameConfiguration> GetAll()
        {
            var currentContext = this.RepositoryManager.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
            if (currentContext == null)
            {
                throw new InvalidOperationException("There is no current context set.");
            }

            var database = currentContext.Context.Database;
            database.OpenConnection();
            try
            {
                var configs = this.objectLoader.LoadAllObjects<GameConfiguration>(currentContext.Context).ToList();
                configs.ForEach(currentContext.Attach);
                return configs;
            }
            finally
            {
                database.CloseConnection();
            }
        }
    }
}
