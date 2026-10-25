// <copyright file="CachingGameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Loading;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// The game configuration repository, which loads the configuration by using the
/// <see cref="JsonObjectLoader"/>, to speed up loading the whole object graph.
/// </summary>
internal class CachingGameConfigurationRepository : CachingGenericRepository<GameConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingGameConfigurationRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CachingGameConfigurationRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory)
        : base(repositoryProvider, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<GameConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this.RepositoryProvider.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (GameConfigurationLoader.IsEnabled)
            {
                var configurations = await new GameConfigurationLoader().LoadAsync(database.GetDbConnection(), cancellationToken).ConfigureAwait(false);
                return configurations.FirstOrDefault(configuration => configuration.Id == id);
            }

            return await CreateObjectLoader().LoadObjectAsync<GameConfiguration>(id, currentContext.Context, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async ValueTask<IEnumerable<GameConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (this.RepositoryProvider.ContextStack.GetCurrentContext() is not EntityFrameworkContextBase currentContext)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var configs = GameConfigurationLoader.IsEnabled
                ? (await new GameConfigurationLoader().LoadAsync(database.GetDbConnection(), cancellationToken).ConfigureAwait(false)).ToList()
                : (await CreateObjectLoader().LoadAllObjectsAsync<GameConfiguration>(currentContext.Context, cancellationToken).ConfigureAwait(false)).ToList();

            var oldConfig = ((EntityDataContext)currentContext.Context).CurrentGameConfiguration;
            try
            {
                configs.ForEach(config =>
                {
                    ((EntityDataContext)currentContext.Context).CurrentGameConfiguration = config;
                    (this.RepositoryProvider as ICacheAwareRepositoryProvider)?.EnsureCachesForCurrentGameConfiguration();
                });
            }
            finally
            {
                ((EntityDataContext)currentContext.Context).CurrentGameConfiguration = oldConfig;
            }

            return configs;
        }
        finally
        {
            await database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new object loader for each load. Its reference handler keeps the loaded objects,
    /// so a shared loader would return the instances of a previous load again and overwrite their values.
    /// </summary>
    /// <returns>The new object loader.</returns>
    private static JsonObjectLoader CreateObjectLoader() => new GameConfigurationJsonObjectLoader();
}