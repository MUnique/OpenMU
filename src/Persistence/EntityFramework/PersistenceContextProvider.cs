// <copyright file="PersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Diagnostics;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// The persistence context provider for the persistence implemented with entity framework core.
/// </summary>
public class PersistenceContextProvider : IMigratableDatabaseContextProvider
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfigurationChangePublisher? _changePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistenceContextProvider" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changePublisher">The change publisher.</param>
    public PersistenceContextProvider(ILoggerFactory loggerFactory, IConfigurationChangePublisher? changePublisher)
    {
        this._loggerFactory = loggerFactory;
        this._changePublisher = changePublisher;
        this.RepositoryProvider = new CacheAwareRepositoryProvider(loggerFactory, changePublisher);
    }

    /// <summary>
    /// Gets the repository provider.
    /// </summary>
    /// <value>
    /// The repository provider.
    /// </value>
    internal CacheAwareRepositoryProvider RepositoryProvider { get; private set; }

    /// <summary>
    /// Determines whether the database schema is up to date.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [is database up to date]; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsDatabaseUpToDateAsync()
    {
        try
        {
            await using var installationContext = new EntityDataContext();
            return !(await installationContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false)).Any();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Applies all pending updates to the database schema.
    /// </summary>
    public async Task ApplyAllPendingUpdatesAsync()
    {
        await using var installationContext = new EntityDataContext();
        await installationContext.Database.MigrateAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Waits until all database updates are applied.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task WaitForUpdatedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        while (!await this.DatabaseExistsAsync().ConfigureAwait(false)
               || !await this.IsDatabaseUpToDateAsync().ConfigureAwait(false))
        {
            await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
        }

        while (!await this.ConfigurationExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            await Task.Delay(3000, cancellationToken).ConfigureAwait(false);
        }

        await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines if a <see cref="GameConfiguration"/> exists on the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>True</c>, if a <see cref="GameConfiguration"/> exists; Otherwise, <c>false</c>.</returns>
    public async Task<bool> ConfigurationExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var installationContext = new EntityDataContext();
            return await installationContext.Set<GameConfiguration>().AnyAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines if the database exists already, by checking if any migration has been applied.
    /// </summary>
    /// <returns><c>True</c>, if the database exists; Otherwise, <c>false</c>.</returns>
    public async Task<bool> DatabaseExistsAsync()
    {
        try
        {
            await using var installationContext = new EntityDataContext();
            return (await installationContext.Database.GetAppliedMigrationsAsync().ConfigureAwait(false)).Any();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether this instance can connect to the database.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can connect to the database; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> CanConnectToDatabaseAsync()
    {
        await using var installationContext = new EntityDataContext();
        return await installationContext.Database.CanConnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Recreates the database by deleting and creating it again.
    /// </summary>
    public async Task ReCreateDatabaseAsync()
    {
        await using (var installationContext = new EntityDataContext())
        {
            await installationContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
        }

        await this.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);

        // We create a new repository provider, so that the previously loaded data is not effective anymore.
        this.RepositoryProvider = new CacheAwareRepositoryProvider(this._loggerFactory, this._changePublisher);
    }

    /// <inheritdoc />
    public IContext CreateNewContext()
    {
        var repositoryProvider = new NonCachingRepositoryProvider(this._loggerFactory, null, this._changePublisher, this.RepositoryProvider.ContextStack);
        return new EntityFrameworkContext(new EntityDataContext(), this._loggerFactory, repositoryProvider, true, this._changePublisher);
    }

    /// <inheritdoc />
    public IContext CreateNewContext(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        return new CachingEntityFrameworkContext(
            new EntityDataContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration },
            this.RepositoryProvider,
            this._changePublisher,
            this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IPlayerContext CreateNewPlayerContext(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        return new PlayerContext(new AccountContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this.RepositoryProvider, this._loggerFactory.CreateLogger<PlayerContext>());
    }

    /// <inheritdoc />
    public IConfigurationContext CreateNewConfigurationContext()
    {
        return new GameConfigurationContext(this.RepositoryProvider, this._loggerFactory.CreateLogger<GameConfigurationContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewTradeContext()
    {
        return new CachingEntityFrameworkContext(new TradeContext(), this.RepositoryProvider, null, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IFriendServerContext CreateNewFriendServerContext()
    {
        return new FriendServerContext(new FriendContext(), this.RepositoryProvider, this._loggerFactory.CreateLogger<FriendServerContext>());
    }

    /// <inheritdoc/>
    public IGuildServerContext CreateNewGuildContext()
    {
        return new GuildServerContext(new GuildContext(), this.RepositoryProvider, this._loggerFactory.CreateLogger<GuildServerContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewTypedContext<T>(bool useCache, DataModel.Configuration.GameConfiguration? gameConfiguration = null)
    {
        if (!typeof(T).IsConfigurationType() && gameConfiguration is null)
        {
            Debug.WriteLine($"Non-configuration type {typeof(T)} without game configuration");
        }

        if (useCache && gameConfiguration is null)
        {
            throw new ArgumentNullException(nameof(gameConfiguration), "When cache should be used, the game configuration must be provided.");
        }

        var dbContext = new TypedContext<T> { CurrentGameConfiguration = gameConfiguration as GameConfiguration };
        if (useCache)
        {
            return new CachingEntityFrameworkContext(dbContext, this.RepositoryProvider, this._changePublisher, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
        }

        var repositoryProvider = new NonCachingRepositoryProvider(this._loggerFactory, null, this._changePublisher, this.RepositoryProvider.ContextStack);
        return new EntityFrameworkContext(dbContext, this._loggerFactory, repositoryProvider, true, this._changePublisher);
    }
}