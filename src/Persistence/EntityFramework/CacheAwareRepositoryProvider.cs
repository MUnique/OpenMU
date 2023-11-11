// <copyright file="CacheAwareRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.DataModel.Configuration;

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// This provider holds two other repository providers:
/// One which provides repositories which actually load the data from the database,
/// and another one which returns repositories with cached data, based on the
/// loaded GameConfiguration. The CacheAwareRepositoryProvider first tries to retrieve
/// a repository for the cached data. If none is found, it takes the other.
/// </summary>
internal class CacheAwareRepositoryProvider : ICacheAwareRepositoryProvider, IContextAwareRepositoryProvider
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly IRepositoryProvider _nonCachingRepositoryProvider;

    private CachingRepositoryProvider _cachingRepositoryProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheAwareRepositoryProvider"/> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="configurationChangePublisher">The configuration change publisher.</param>
    public CacheAwareRepositoryProvider(ILoggerFactory loggerFactory, IConfigurationChangeListener? configurationChangeListener)
    {
        this._loggerFactory = loggerFactory;
        this._cachingRepositoryProvider = new CachingRepositoryProvider(loggerFactory, this);
        this._nonCachingRepositoryProvider = new NonCachingRepositoryProvider(loggerFactory, this, configurationChangeListener, this.ContextStack);
    }

    /// <inheritdoc />
    public IContextStack ContextStack { get; } = new ContextStack();

    /// <inheritdoc />
    public IRepository? GetRepository(Type objectType)
    {
        if (this.ContextStack.GetCurrentContext() is EntityFrameworkContextBase { Context: ITypedContext editContext }
            && editContext.IsIncluded(objectType))
        {
            return this._nonCachingRepositoryProvider.GetRepository(objectType);
        }

        return this._cachingRepositoryProvider.GetRepository(objectType)
               ?? this._nonCachingRepositoryProvider.GetRepository(objectType);
    }

    /// <inheritdoc />
    public IRepository<T>? GetRepository<T>()
        where T : class
    {
        if (this.ContextStack.GetCurrentContext() is EntityFrameworkContextBase { Context: ITypedContext editContext }
            && (editContext.IsIncluded(typeof(T)) || editContext.IsIncluded(typeof(T).BaseType!)))
        {
            return this._nonCachingRepositoryProvider.GetRepository<T>();
        }

        return this._cachingRepositoryProvider.GetRepository<T>()
               ?? this._nonCachingRepositoryProvider.GetRepository<T>();
    }

    /// <inheritdoc />
    public TRepository? GetRepository<T, TRepository>()
        where T : class
        where TRepository : IRepository
    {
        if (this.ContextStack.GetCurrentContext() is EntityFrameworkContextBase { Context: ITypedContext editContext }
            && (editContext.IsIncluded(typeof(T)) || editContext.IsIncluded(typeof(T).BaseType!)))
        {
            return this._nonCachingRepositoryProvider.GetRepository<T, TRepository>();
        }

        return this._cachingRepositoryProvider.GetRepository<T, TRepository>()
               ?? this._nonCachingRepositoryProvider.GetRepository<T, TRepository>();
    }

    /// <inheritdoc />
    public void EnsureCachesForCurrentGameConfiguration()
    {
        this._cachingRepositoryProvider.EnsureCachesForCurrentGameConfiguration();
    }

    /// <inheritdoc />
    public void ResetCache()
    {
        this._cachingRepositoryProvider = new CachingRepositoryProvider(this._loggerFactory, this);
    }

    /// <inheritdoc />
    public async ValueTask UpdateCachedInstanceAsync(object changedInstance)
    {
        if (this._cachingRepositoryProvider.GetRepository(changedInstance.GetType().BaseType ?? changedInstance.GetType()) is IConfigurationTypeRepository repository)
        {
            repository.UpdateCachedInstances(changedInstance);
        }
        else
        {
            // not all types have an own repository.
            var gameConfigRepo = this._cachingRepositoryProvider.GetRepository<GameConfiguration>();
            if (gameConfigRepo is null)
            {
                return;
            }

            foreach (var config in await gameConfigRepo.GetAllAsync().ConfigureAwait(false))
            {
                var obj = config.GetObjectOfConfig(changedInstance) as IAssignable;
                obj?.AssignValuesOf(changedInstance, config);
            }
        }
    }
}