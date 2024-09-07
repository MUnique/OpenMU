// <copyright file="GameServerDefinitionRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Repository for the <see cref="GameServerDefinition"/>.
/// It sets the <see cref="EntityDataContext.CurrentGameConfiguration"/> before loading other dependent data which tries to load the configured maps of a server.
/// </summary>
internal class GameServerDefinitionRepository : CachingGenericRepository<GameServerDefinition>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerDefinitionRepository" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public GameServerDefinitionRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory)
        : base(repositoryProvider, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override async ValueTask LoadDependentDataAsync(object obj, DbContext currentContext, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (obj is GameServerDefinition definition)
        {
            var entityEntry = currentContext.Entry(obj);
            foreach (var collection in entityEntry.Collections.Where(c => !c.IsLoaded && c.Metadata is INavigation))
            {
                await this.LoadCollectionAsync(entityEntry, (INavigation)collection.Metadata, currentContext, cancellationToken).ConfigureAwait(false);
                collection.IsLoaded = true;
            }

            if (definition.GameConfigurationId.HasValue)
            {
                definition.RawGameConfiguration =
                    await this.RepositoryProvider.GetRepository<GameConfiguration>()!
                        .GetByIdAsync(definition.GameConfigurationId.Value, cancellationToken).ConfigureAwait(false);

                if (currentContext is EntityDataContext context)
                {
                    context.CurrentGameConfiguration = definition.RawGameConfiguration;
                }
            }

            if (definition.ServerConfigurationId.HasValue)
            {
                definition.ServerConfiguration = await this.RepositoryProvider.GetRepository<GameServerConfiguration>()!
                    .GetByIdAsync(definition.ServerConfigurationId.Value, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}