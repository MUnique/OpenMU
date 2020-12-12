// <copyright file="GameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Persistence.EntityFramework.Json;
    using MUnique.OpenMU.Persistence.EntityFramework.Model;

    /// <summary>
    /// The game configuration repository, which loads the configuration by using the
    /// <see cref="JsonObjectLoader"/>, to speed up loading the whole object graph.
    /// </summary>
    internal class GameConfigurationRepository : GenericRepository<GameConfiguration>
    {
        private readonly JsonObjectLoader objectLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConfigurationRepository" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="logger">The logger.</param>
        public GameConfigurationRepository(RepositoryManager repositoryManager, ILogger<GameConfigurationRepository> logger)
            : base(repositoryManager, logger)
        {
            this.objectLoader = new GameConfigurationJsonObjectLoader();
        }

        /// <inheritdoc />
        public override GameConfiguration? GetById(Guid id)
        {
            var currentContext = this.RepositoryManager.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
            if (currentContext is null)
            {
                throw new InvalidOperationException("There is no current context set.");
            }

            var database = currentContext.Context.Database;
            database.OpenConnection();
            try
            {
                if (this.objectLoader.LoadObject<GameConfiguration>(id, currentContext.Context) is { } config)
                {
                    currentContext.Attach(config);
                    return config;
                }

                return null;
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
            if (currentContext is null)
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
