﻿// <copyright file="GameConfigurationRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// The game configuration repository, which loads the configuration by using the
/// <see cref="JsonObjectLoader"/>, to speed up loading the whole object graph.
/// </summary>
internal class GameConfigurationRepository : GenericRepository<GameConfiguration>
{
    private readonly JsonObjectLoader _objectLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changePublisher">The change publisher.</param>
    public GameConfigurationRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory, IConfigurationChangePublisher? changePublisher)
        : base(repositoryProvider, loggerFactory, changePublisher)
    {
        this._objectLoader = new GameConfigurationJsonObjectLoader();
    }

    /// <inheritdoc />
    public override async ValueTask<GameConfiguration?> GetByIdAsync(Guid id)
    {
        var currentContext = this.RepositoryProvider.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
        if (currentContext is null)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            if (await this._objectLoader.LoadObjectAsync<GameConfiguration>(id, currentContext.Context).ConfigureAwait(false) is { } config)
            {
                currentContext.Attach(config);
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
        var currentContext = this.RepositoryProvider.ContextStack.GetCurrentContext() as EntityFrameworkContextBase;
        if (currentContext is null)
        {
            throw new InvalidOperationException("There is no current context set.");
        }

        var database = currentContext.Context.Database;
        await database.OpenConnectionAsync().ConfigureAwait(false);
        try
        {
            var configs = (await this._objectLoader.LoadAllObjectsAsync<GameConfiguration>(currentContext.Context).ConfigureAwait(false)).ToList();
            configs.ForEach(currentContext.Attach);
            return configs;
        }
        finally
        {
            await database.CloseConnectionAsync().ConfigureAwait(false);
        }
    }
}