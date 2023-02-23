// <copyright file="CachingGameConfigurationRepository.cs" company="MUnique">
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
    private const long MinLevel = 0;
    private const long MaxLevel = 256;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingGameConfigurationRepository" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CachingGameConfigurationRepository(RepositoryManager repositoryManager, ILoggerFactory loggerFactory)
        : base(repositoryManager, loggerFactory)
    {
        this._objectLoader = new GameConfigurationJsonObjectLoader();
    }

    /// <inheritdoc />
    public override async ValueTask<GameConfiguration?> GetByIdAsync(Guid id)
    {
        if (this.RepositoryManager.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            if (await this._objectLoader.LoadObjectAsync<GameConfiguration>(id, currentContext.Context).ConfigureAwait(false) is { } config)
            {
                this.SetExperienceTables(config);
                return config;
            }

            return null;
        }
        finally
        {
            await database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async ValueTask<IEnumerable<GameConfiguration>> GetAllAsync()
    {
        if (this.RepositoryManager.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            var configs = (await this._objectLoader.LoadAllObjectsAsync<GameConfiguration>(currentContext.Context).ConfigureAwait(false)).ToList();
            configs.ForEach(this.SetExperienceTables);
            return configs;
        }
        finally
        {
            await database.CloseConnectionAsync().ConfigureAwait(false);
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

    /// <summary>
    /// The equation
    ///  f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
    /// </summary>
    /// <param name="lvl"></param>
    /// <returns> long. </returns>
    private long CalcNeededMasterExp(long lvl)
    {
        return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
    }

    /// <summary>
    /// The equation for calculate needed experience.
    /// </summary>
    /// <param name="level"></param>
    /// <returns> long. </returns>
    private long CalculateNeededExperience(long level)
    {
        return level switch
        {
            MinLevel => 0,
            < MaxLevel => 10 * (level + 8) * (level - 1) * (level - 1),
            _ => (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256)),
        };
    }
}