// <copyright file="InMemoryPersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.Disposables;

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A context provider which uses in-memory repositories to hold its data, e.g. for testing or demo purposes.
/// Changes in one context directly have effect in other contexts! Calling SaveChanges or not doesn't matter.
/// </summary>
public class InMemoryPersistenceContextProvider : IMigratableDatabaseContextProvider
{
    private readonly IConfigurationChangePublisher? _changePublisher;
    private InMemoryRepositoryProvider _repositoryProvider = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryPersistenceContextProvider"/> class.
    /// </summary>
    /// <param name="changePublisher">The change publisher.</param>
    public InMemoryPersistenceContextProvider(IConfigurationChangePublisher? changePublisher = null)
    {
        this._changePublisher = changePublisher;
    }

    /// <inheritdoc />
    public IRepositoryProvider RepositoryProvider => this._repositoryProvider;

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
        var context = new InMemoryContext(this._repositoryProvider);
        if (this._changePublisher is { })
        {
#pragma warning disable VSTHRD100
            async void OnContextOnSavedChanges(object? o, EventArgs e)
#pragma warning restore VSTHRD100
            {
                try
                {
                    foreach (var obj in await context.GetAsync(typeof(T), default).ConfigureAwait(false))
                    {
                        await this._changePublisher.ConfigurationChangedAsync(typeof(T), obj.GetId(), obj).ConfigureAwait(false);
                    }
                }
                catch
                {
                    // ignore all errors.
                }
            }

            context.SavedChanges += OnContextOnSavedChanges;

        }

        return context;
    }

    /// <inheritdoc />
    public Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> IsDatabaseUpToDateAsync(CancellationToken cancellationToken)
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
    public Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<bool> ShouldDoAutoSchemaUpdateAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task<IDisposable> ReCreateDatabaseAsync()
    {
        this._repositoryProvider = new();
        return Task.FromResult<IDisposable>(new Disposable(() => { }));
    }
}