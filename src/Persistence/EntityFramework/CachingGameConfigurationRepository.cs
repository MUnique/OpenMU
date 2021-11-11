﻿// <copyright file="CachingGameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// The game configuration repository, which loads the configuration by using the
/// <see cref="JsonObjectLoader"/>, to speed up loading the whole object graph.
/// Additionally it fills the experience table, because the entity framework can't map arrays.
/// </summary>
internal class CachingGameConfigurationRepository : CachingGenericRepository<GameConfiguration>
{
    private readonly JsonObjectLoader _objectLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingGameConfigurationRepository" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="logger">The logger for this class.</param>
    public CachingGameConfigurationRepository(RepositoryManager repositoryManager, ILogger<CachingGameConfigurationRepository> logger)
        : base(repositoryManager, logger)
    {
        this._objectLoader = new GameConfigurationJsonObjectLoader();
    }

    /// <inheritdoc />
    public override GameConfiguration? GetById(Guid id)
    {
        if (this.RepositoryManager.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        database.OpenConnection();
        try
        {
            if (this._objectLoader.LoadObject<GameConfiguration>(id, currentContext.Context) is { } config)
            {
                this.SetExperienceTables(config);
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
        if (this.RepositoryManager.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        database.OpenConnection();
        try
        {
            var configs = this._objectLoader.LoadAllObjects<GameConfiguration>(currentContext.Context).ToList();
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
        gameConfiguration.ExperienceTable =
            Enumerable.Range(0, gameConfiguration.MaximumLevel + 2)
                .Select(level => this.CalculateNeededExperience(level))
                .ToArray();
        gameConfiguration.MasterExperienceTable =
            Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();
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