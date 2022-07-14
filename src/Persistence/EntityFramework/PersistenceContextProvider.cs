// <copyright file="PersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

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
        this.CachingRepositoryManager = new CachingRepositoryManager(loggerFactory);
        this.CachingRepositoryManager.RegisterRepositories();
    }

    /// <summary>
    /// Gets the repository manager.
    /// </summary>
    /// <value>
    /// The repository manager.
    /// </value>
    internal CachingRepositoryManager CachingRepositoryManager { get; private set; }

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
            using var installationContext = new EntityDataContext();
            return !(await installationContext.Database.GetPendingMigrationsAsync()).Any();
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
        using var installationContext = new EntityDataContext();
        await installationContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Waits until all database updates are applied.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task WaitForUpdatedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        while (!await this.DatabaseExistsAsync()
               || !await this.IsDatabaseUpToDateAsync())
        {
            await Task.Delay(3000, cancellationToken);
        }

        while (!await this.ConfigurationExistsAsync(cancellationToken))
        {
            await Task.Delay(3000, cancellationToken);
        }

        await Task.Delay(5000, cancellationToken);
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
            return (await installationContext.Database.GetAppliedMigrationsAsync()).Any();
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
        return await installationContext.Database.CanConnectAsync();
    }

    /// <summary>
    /// Recreates the database by deleting and creating it again.
    /// </summary>
    public async Task ReCreateDatabaseAsync()
    {
        await using (var installationContext = new EntityDataContext())
        {
            await installationContext.Database.EnsureDeletedAsync();
        }

        await this.ApplyAllPendingUpdatesAsync();

        // We create a new repository manager, so that the previously loaded data is not effective anymore.
        this.CachingRepositoryManager = new CachingRepositoryManager(this._loggerFactory);
        this.CachingRepositoryManager.RegisterRepositories();
    }

    /// <inheritdoc />
    public IContext CreateNewContext()
    {
        return new CachingEntityFrameworkContext(new EntityDataContext(), this.CachingRepositoryManager, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewContext(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        return new CachingEntityFrameworkContext(new EntityDataContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this.CachingRepositoryManager, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IPlayerContext CreateNewPlayerContext(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        return new PlayerContext(new AccountContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this.CachingRepositoryManager, this._loggerFactory.CreateLogger<PlayerContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewConfigurationContext()
    {
        return new CachingEntityFrameworkContext(new ConfigurationContext(), this.CachingRepositoryManager, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewTradeContext()
    {
        return new CachingEntityFrameworkContext(new TradeContext(), this.CachingRepositoryManager, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc />
    public IFriendServerContext CreateNewFriendServerContext()
    {
        return new FriendServerContext(new FriendContext(), this.CachingRepositoryManager, this._loggerFactory.CreateLogger<FriendServerContext>());
    }

    /// <inheritdoc/>
    public IGuildServerContext CreateNewGuildContext()
    {
        return new GuildServerContext(new GuildContext(), this.CachingRepositoryManager, this._loggerFactory.CreateLogger<GuildServerContext>());
    }

    /// <inheritdoc />
    public IContext CreateNewTypedContext<T>()
    {
        return new EntityFrameworkContext(new TypedContext<T>(), this._loggerFactory, this._changePublisher);
    }
}