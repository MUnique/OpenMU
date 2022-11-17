// <copyright file="GenericRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A generic repository which wraps the access to the DBSet of the <see cref="EntityDataContext"/>.
/// Entities are getting eagerly (=completely) loaded automatically.
/// </summary>
/// <typeparam name="T">The type which this repository should manage.</typeparam>
internal class GenericRepository<T> : GenericRepositoryBase<T>
    where T : class
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfigurationChangePublisher? _changePublisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}" /> class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changePublisher">The change publisher.</param>
    public GenericRepository(RepositoryManager repositoryManager, ILoggerFactory loggerFactory, IConfigurationChangePublisher? changePublisher)
        : base(repositoryManager, loggerFactory.CreateLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(GenericRepository<T>)))
    {
        this._loggerFactory = loggerFactory;
        this._changePublisher = changePublisher;
    }

    /// <summary>
    /// Gets a context to work with. If no context is currently registered at the repository manager, a new one is getting created.
    /// </summary>
    /// <returns>The context.</returns>
    protected override EntityFrameworkContextBase GetContext()
    {
        var context = this.RepositoryManager.ContextStack.GetCurrentContext() as EntityFrameworkContext;

        return new EntityFrameworkContext(context?.Context ?? new TypedContext<T>(), this._loggerFactory, this.RepositoryManager, context is null, this._changePublisher);
    }
}