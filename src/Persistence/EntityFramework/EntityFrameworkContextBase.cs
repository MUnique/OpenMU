// <copyright file="EntityFrameworkContextBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx;

/// <summary>
/// Abstract base class for an <see cref="IContext"/> which uses an <see cref="DbContext"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.IContext" />
public class EntityFrameworkContextBase : IContext
{
    private readonly bool _isOwner;
    private readonly IConfigurationChangePublisher? _changePublisher;
    private readonly AsyncLock _lock = new();
    private readonly ILogger _logger;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkContextBase" /> class.
    /// </summary>
    /// <param name="context">The db context.</param>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="isOwner">If set to <c>true</c>, this instance owns the <see cref="Context" />. That means it will be disposed when this instance will be disposed.</param>
    /// <param name="changePublisher">The change publisher.</param>
    /// <param name="logger">The logger.</param>
    protected EntityFrameworkContextBase(DbContext context, RepositoryManager repositoryManager, bool isOwner, IConfigurationChangePublisher? changePublisher, ILogger logger)
    {
        this.Context = context;
        this.RepositoryManager = repositoryManager;
        this._isOwner = isOwner;
        this._changePublisher = changePublisher;
        this._logger = logger;
        if (this._changePublisher is { })
        {
            this.Context.SavedChanges += this.OnSavedChanges;
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="EntityFrameworkContextBase"/> class.
    /// </summary>
    ~EntityFrameworkContextBase() => this.Dispose(false);

    /// <summary>
    /// Gets the entity framework context.
    /// </summary>
    internal DbContext Context { get; }

    /// <summary>
    /// Gets the repository manager.
    /// </summary>
    protected RepositoryManager RepositoryManager { get; }

    /// <inheritdoc/>
    public bool SaveChanges()
    {
        using var l = this._lock.Lock();

        // when we have a change publisher attached, we want to get the changed entries before accepting them.
        // Otherwise, we can accept them.
        var acceptChanges = this._changePublisher is null;

        this.Context.SaveChanges(acceptChanges);

        return true;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> SaveChangesAsync()
    {
        using var l = await this._lock.LockAsync();
        // when we have a change publisher attached, we want to get the changed entries before accepting them.
        // Otherwise, we can accept them.
        var acceptChanges = this._changePublisher is null;

        await this.Context.SaveChangesAsync(acceptChanges).ConfigureAwait(false);

        return true;
    }

    /// <inheritdoc />
    public bool Detach(object item)
    {
        var entry = this.Context.Entry(item);
        if (entry is null)
        {
            return false;
        }

        var previousState = entry.State;
        entry.State = EntityState.Detached;
        this.ForEachAggregate(item, obj => this.Detach(obj));

        return previousState != EntityState.Added;
    }

    /// <inheritdoc />
    public void Attach(object item)
    {
        this.Context.Attach(item);
    }

    /// <summary>
    /// Creates a new instance of <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type which should get created.</typeparam>
    /// <param name="args">The arguments which are handed 1-to-1 to the constructor. If no arguments are given, the default constructor will be called.</param>
    /// <returns>
    /// A new instance of <typeparamref name="T" />.
    /// </returns>
    public T CreateNew<T>(params object?[] args)
        where T : class
    {
        using var l = this._lock.Lock();
        var instance = typeof(CachingEntityFrameworkContext).Assembly.CreateNew<T>(args);
        this.Context.Add(instance);
        return instance;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> DeleteAsync<T>(T obj)
        where T : class
    {
        using var l = await this._lock.LockAsync();

        var result = false;
        if (this.Context.Entry(obj) is not { } entry)
        {
            return result;
        }

        switch (entry.State)
        {
            case EntityState.Detached:
                return true;
            case EntityState.Added:
                this.Detach(obj);
                break;
            default:
                this.Context.Remove(obj);
                this.ForEachAggregate(obj, a => this.Context.Remove(a));
                break;
        }

        result = true;

        return result;
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync<T>(Guid id)
        where T : class
    {
        using var l = await this._lock.LockAsync();
        using var context = this.RepositoryManager.ContextStack.UseContext(this);
        return await this.RepositoryManager.GetRepository<T>().GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<object?> GetByIdAsync(Guid id, Type type)
    {
        using var l = await this._lock.LockAsync();
        using var context = this.RepositoryManager.ContextStack.UseContext(this);
        return await this.RepositoryManager.GetRepository(type).GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<T>> GetAsync<T>()
        where T : class
    {
        using var l = await this._lock.LockAsync();
        using var context = this.RepositoryManager.ContextStack.UseContext(this);
        return await this.RepositoryManager.GetRepository<T>().GetAllAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!this._isDisposed)
        {
            this.Dispose(true);
        }

        this._isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="dispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool dispose)
    {
        if (!dispose || !this._isOwner)
        {
            return;
        }

        this.Context.SavedChanges -= this.OnSavedChanges;
        this.Context.Dispose();
    }

    private void ForEachAggregate(object obj, Action<object> action)
    {
        var aggregateProperties = obj.GetType()
            .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<MemberOfAggregateAttribute>() is { }
                        || p.Name.StartsWith("Joined"));
        foreach (var propertyInfo in aggregateProperties)
        {
            var propertyValue = propertyInfo.GetMethod?.Invoke(obj, Array.Empty<object>());
            if (propertyValue is IEnumerable enumerable)
            {
                foreach (var value in enumerable)
                {
                    action(value);
                }
            }
            else if (propertyValue is { })
            {
                action(propertyValue);
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnSavedChanges(object? sender, SavedChangesEventArgs e)
    {
        try
        {
            if (this._changePublisher is null)
            {
                // should never be the case
                return;
            }

            var changedEntries = this.Context.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Unchanged
                                 && entity.Metadata.ClrType.IsConfigurationType()).ToList();
            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        await this._changePublisher.ConfigurationAddedAsync(entry.Metadata.ClrType, entry.Entity.GetId(), entry.Entity).ConfigureAwait(false);
                        break;
                    case EntityState.Deleted:
                        await this._changePublisher.ConfigurationRemovedAsync(entry.Metadata.ClrType, entry.Entity.GetId()).ConfigureAwait(false);
                        break;
                    case EntityState.Modified:
                        await this._changePublisher.ConfigurationChangedAsync(entry.Metadata.ClrType, entry.Entity.GetId(), entry.Entity).ConfigureAwait(false);
                        break;
                    default:
                        // no change publishing required.
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error publishing changes.");
        }
        finally
        {
            try
            {
                this.Context.ChangeTracker.AcceptAllChanges();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Unexpected error when accepting all saved changes.");
            }
        }
    }
}