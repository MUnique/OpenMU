// <copyright file="RepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// A repository provider which does not use caching.
/// </summary>
internal class RepositoryProvider : BaseRepositoryProvider, IContextAwareRepositoryProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryProvider" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="changeListener">The change publisher.</param>
    /// <param name="contextStack">The context stack.</param>
    public RepositoryProvider(ILoggerFactory loggerFactory, IConfigurationChangeListener? changeListener, IContextStack contextStack)
    {
        this.LoggerFactory = loggerFactory;
        this.ChangeListener = changeListener;
        this.ContextStack = contextStack;
    }

    /// <summary>
    /// Gets the context stack. When loading an object, the current context should be pushed onto the stack.
    /// </summary>
    public IContextStack ContextStack { get; }

    /// <summary>
    /// Gets the logger factory.
    /// </summary>
    protected ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// Gets the change publisher.
    /// </summary>
    protected IConfigurationChangeListener? ChangeListener { get; }

    /// <summary>
    /// Creates the generic repository for the specified type.
    /// </summary>
    /// <param name="entityType">Type of the entity.</param>
    /// <returns>The created repository.</returns>
    protected virtual IRepository CreateGenericRepository(Type entityType, IContextAwareRepositoryProvider repositoryProvider)
    {
        var repositoryType = typeof(GenericRepository<>).MakeGenericType(entityType);
        return (IRepository)Activator.CreateInstance(repositoryType, repositoryProvider, this.LoggerFactory, this.ChangeListener)!;
    }

    /// <summary>
    /// Registers the repository. Adapts the type, so that the base type gets registered.
    /// </summary>
    /// <param name="type">The generic type which the repository handles.</param>
    /// <param name="repository">The repository.</param>
    protected override void RegisterRepository(Type type, IRepository repository)
    {
        if (type.Assembly.Equals(this.GetType().Assembly) && type.BaseType != typeof(object))
        {
            base.RegisterRepository(type.BaseType!, repository);
        }
        else
        {
            base.RegisterRepository(type, repository);
        }
    }
}