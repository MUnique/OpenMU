// <copyright file="DataSourceBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

/// <summary>
/// Provider which provides the latest <see cref="Owner" /> and it's containing
/// child objects.
/// </summary>
/// <typeparam name="TOwner">The type of the owner.</typeparam>
/// <remarks>
/// Approach: One context for each composition root type. When child data is going to be edited, the whole type
/// should be loaded.
/// </remarks>
public abstract class DataSourceBase<TOwner> : IDataSource<TOwner>
    where TOwner : class
{
    private readonly ILogger<DataSourceBase<TOwner>> _logger;

    private readonly AsyncLock _loadLock = new();

    private IContext? _context;

    private TOwner? _owner;

    private IDictionary<Guid, IIdentifiable>? _subObjects;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceBase{TOwner}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    protected DataSourceBase(ILogger<DataSourceBase<TOwner>> logger, IPersistenceContextProvider persistenceContextProvider)
    {
        this._logger = logger;
        this.ContextProvider = persistenceContextProvider;
    }

    /// <summary>
    /// Gets the mapping of a <see cref="Type"/> to their <see cref="IEnumerable{T}"/> of the <see cref="TOwner"/>.
    /// </summary>
    protected abstract IReadOnlyDictionary<Type, Func<TOwner, IEnumerable>> TypeToEnumerables { get; }

    /// <summary>
    /// Gets the <see cref="IPersistenceContextProvider"/> which can be used to create new contexts.
    /// </summary>
    protected IPersistenceContextProvider ContextProvider { get; }

    private TOwner Owner => this._owner ?? throw new InvalidOperationException("owner is not loaded.");

    private IDictionary<Guid, IIdentifiable> SubObjects => this._subObjects ??= this.BuildDictionary();

    /// <inheritdoc />
    public bool IsSupporting(Type type)
    {
        return this.TypeToEnumerables.ContainsKey(type);
    }

    /// <inheritdoc />
    async ValueTask<TOwner> IDataSource<TOwner>.GetOwnerAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        return (TOwner)(await this.GetOwnerAsync(ownerId, cancellationToken).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async ValueTask<IContext> GetContextAsync(CancellationToken cancellationToken)
    {
        return this._context ??= await this.CreateNewContextAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<object> GetOwnerAsync(Guid ownerId = default, CancellationToken cancellationToken = default)
    {
        using var l = await this._loadLock.LockAsync(cancellationToken).ConfigureAwait(false);

        if (this._owner is { } owner
            && (ownerId == Guid.Empty || owner.GetId() == ownerId))
        {
            return owner;
        }

        var context = await this.GetContextAsync(cancellationToken).ConfigureAwait(false);
        this._logger.LogDebug("Loading owner ...");
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        if (ownerId == Guid.Empty)
        {
            owner = (await context.GetAsync<TOwner>().ConfigureAwait(false)).FirstOrDefault();
        }
        else
        {
            owner = (await context.GetByIdAsync<TOwner>(ownerId, cancellationToken).ConfigureAwait(false));
        }

        this._owner = owner;
        this._subObjects = null;
        stopwatch.Stop();
        this._logger.LogDebug("Loaded owner in {duration}", stopwatch.Elapsed);
        return owner ?? throw new InvalidOperationException("Owner not found");
    }

    /// <inheritdoc />
    public async ValueTask DiscardChangesAsync()
    {
        using var l = await _loadLock.LockAsync().ConfigureAwait(false);
        if (this._context?.HasChanges is true)
        {
            // next time, we have to load again.
            // if we would be able to clone objects, that wouldn't be necessary.
            this.Reset();
        }
    }

    /// <inheritdoc />
    public IEnumerable<T> GetAll<T>()
    {
        return this.GetAll(typeof(T)).OfType<T>();
    }

    /// <inheritdoc />
    public IEnumerable GetAll(Type type)
    {
        var gameConfiguration = this._owner ?? throw new InvalidOperationException("config is not loaded.");

        if (TypeToEnumerables.TryGetValue(type, out var getter))
        {
            return getter(gameConfiguration);
        }

        throw new ArgumentOutOfRangeException($"The type {type} is not registered as child of the {nameof(Owner)}", nameof(type));
    }

    /// <inheritdoc />
    public IIdentifiable? Get(Guid id)
    {
        using var l = _loadLock.Lock();
        if (this.SubObjects.TryGetValue(id, out var obj))
        {
            return obj;
        }

        return null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Reset();
    }

    /// <summary>
    /// Creates a new <see cref="IContext"/>.
    /// </summary>
    /// <returns>The created <see cref="IContext"/>.</returns>
    protected virtual ValueTask<IContext> CreateNewContextAsync()
    {
        return ValueTask.FromResult(this.ContextProvider.CreateNewContext());
    }

    private IDictionary<Guid, IIdentifiable> BuildDictionary()
    {
        var owner = this.Owner;

        var result = new Dictionary<Guid, IIdentifiable>();

        foreach (var (type, objects) in TypeToEnumerables)
        {
            foreach (var obj in objects(owner).OfType<IIdentifiable>())
            {
                if (!result.TryAdd(obj.Id, obj))
                {
                    this._logger.LogDebug($"Duplicate key {obj.Id}, type {type}.");
                }
            }
        }

        return result;
    }

    private void Reset()
    {
        this._owner = null;
        this._context?.Dispose();
        this._context = null;
        this._subObjects = null;
    }
}