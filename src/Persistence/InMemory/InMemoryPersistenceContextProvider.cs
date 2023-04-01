// <copyright file="InMemoryPersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A context provider which uses in-memory repositories to hold its data, e.g. for testing or demo purposes.
/// Changes in one context directly have effect in other contexts! Calling SaveChanges or not doesn't matter.
/// </summary>
public class InMemoryPersistenceContextProvider : IMigratableDatabaseContextProvider
{
    private InMemoryRepositoryProvider _repositoryProvider = new ();

    /// <inheritdoc/>
    public IContext CreateNewContext()
    {
        return new InMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc/>
    public IContext CreateNewContext(GameConfiguration gameConfiguration)
    {
        return new InMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc/>
    public IContext CreateNewTradeContext()
    {
        return new InMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc/>
    public IPlayerContext CreateNewPlayerContext(GameConfiguration gameConfiguration)
    {
        return new PlayerInMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc/>
    public IConfigurationContext CreateNewConfigurationContext()
    {
        return new ConfigurationInMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc />
    public IFriendServerContext CreateNewFriendServerContext()
    {
        return new FriendServerInMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc/>
    public IGuildServerContext CreateNewGuildContext()
    {
        return new GuildServerInMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc />
    public IContext CreateNewTypedContext<T>(bool useCache, GameConfiguration? gameConfiguration = null)
    {
        return new InMemoryContext(this._repositoryProvider);
    }

    /// <inheritdoc />
    public Task<bool> DatabaseExistsAsync()
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> IsDatabaseUpToDateAsync()
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task ApplyAllPendingUpdatesAsync()
    {
        // we don't need to do anything.
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task WaitForUpdatedDatabaseAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> CanConnectToDatabaseAsync()
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task ReCreateDatabaseAsync()
    {
        this._repositoryProvider = new();
        return Task.CompletedTask;
    }
}