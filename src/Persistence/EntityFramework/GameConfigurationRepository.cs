// <copyright file="GameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Threading;
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
    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changeListener">The change publisher.</param>
    public GameConfigurationRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory, IConfigurationChangeListener? changeListener)
        : base(repositoryProvider, loggerFactory, changeListener)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<GameConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var currentContext = this.RepositoryProvider.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
        if (currentContext is null)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (await CreateObjectLoader().LoadObjectAsync<GameConfiguration>(id, currentContext.Context, cancellationToken).ConfigureAwait(false) is { } config)
            {
                currentContext.Context.Attach(config);
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
    public override async ValueTask<IEnumerable<GameConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var currentContext = this.RepositoryProvider.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
        if (currentContext is null)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var configs = (await CreateObjectLoader().LoadAllObjectsAsync<GameConfiguration>(currentContext.Context, cancellationToken).ConfigureAwait(false)).ToList();
            configs.ForEach(c => currentContext.Context.Attach(c));
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