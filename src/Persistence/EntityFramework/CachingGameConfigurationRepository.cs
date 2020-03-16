// <copyright file="CachingGameConfigurationRepository.cs" company="MUnique">
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
    /// Additionally it fills the experience table, because the entity framework can't map arrays.
    /// </summary>
    internal class CachingGameConfigurationRepository : CachingGenericRepository<GameConfiguration>
    {
        private readonly JsonObjectLoader objectLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingGameConfigurationRepository" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public CachingGameConfigurationRepository(RepositoryManager repositoryManager)
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

                this.SetExperienceTables(config);
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
                configs.ForEach(this.SetExperienceTables);
                return configs;
            }
            finally
            {
                database.CloseConnection();
            }
        }

        private void SetExperienceTables(GameConfiguration gameConfiguration)
        {
            if (gameConfiguration != null)
            {
                gameConfiguration.ExperienceTable =
                    Enumerable.Range(0, gameConfiguration.MaximumLevel + 2)
                        .Select(level => this.CalculateNeededExperience(level))
                        .ToArray();
                gameConfiguration.MasterExperienceTable =
                    Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();
            }
        }

        private long CalcNeededMasterExp(long lvl)
        {
            // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
            return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
        }

        private long CalculateNeededExperience(long level)
        {
            if (level == 0)
            {
                return 0;
            }

            if (level < 256)
            {
                return 10 * (level + 8) * (level - 1) * (level - 1);
            }

            return (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256));
        }
    }
}
