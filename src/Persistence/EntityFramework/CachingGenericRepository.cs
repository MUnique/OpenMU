// <copyright file="CachingGenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

/// <summary>
/// A generic repository which wraps the access to the dbset of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal class CachingGenericRepository<T> : GenericRepositoryBase<T>
    where T : class
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingGenericRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CachingGenericRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory)
        : base(repositoryProvider, loggerFactory.CreateLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(CachingGenericRepository<T>)))
    {
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Gets a context to work with. If no originating context is given, a new temporary one is getting created.
    /// </summary>
    /// <param name="origin">The originating context, or <c>null</c> to create a temporary context.</param>
    /// <returns>The context.</returns>
    protected override EntityFrameworkContextBase GetContext(EntityFrameworkContextBase? origin)
    {
        return new CachingEntityFrameworkContext(origin?.Context ?? new EntityDataContext(), this.RepositoryProvider, origin is null, null, this._loggerFactory.CreateLogger<CachingEntityFrameworkContext>());
    }

    /// <inheritdoc/>
    protected override IEnumerable<INavigation> GetNavigations(EntityEntry entityEntry)
    {
        return this.FullEntityType.GetNavigations();
    }
}