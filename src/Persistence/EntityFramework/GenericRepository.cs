// <copyright file="GenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Reflection;
using Microsoft.Extensions.Logging;

/// <summary>
/// A generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal class GenericRepository<T> : GenericRepositoryBase<T>
    where T : class
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfigurationChangeListener? _changeListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryProvider">The repository provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changeListener">The change publisher.</param>
    public GenericRepository(IContextAwareRepositoryProvider repositoryProvider, ILoggerFactory loggerFactory, IConfigurationChangeListener? changeListener)
        : base(repositoryProvider, loggerFactory.CreateLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(GenericRepository<T>)))
    {
        this._loggerFactory = loggerFactory;
        this._changeListener = changeListener;
    }

    /// <summary>
    /// Gets a context to work with. If no originating context is given, a new temporary one is getting created.
    /// </summary>
    /// <param name="origin">The originating context, or <c>null</c> to create a temporary context.</param>
    /// <returns>The context.</returns>
    protected override EntityFrameworkContextBase GetContext(EntityFrameworkContextBase? origin)
    {
        return new EntityFrameworkContext(origin?.Context ?? new TypedContext(typeof(T)), this._loggerFactory, this.RepositoryProvider, origin is null, this._changeListener);
    }
}