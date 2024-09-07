// <copyright file="IDataSource.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using System.Threading;

/// <summary>
/// Interface for a class which allows an easier access to the whole object tree
/// of an instance of an owner type, usually a composition root.
/// </summary>
public interface IDataSource : IDisposable
{
    /// <summary>
    /// Gets the context which was used to load the owner.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The context.
    /// </returns>
    ValueTask<IContext> GetContextAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the owner with the specified id.
    /// Loads it from the current context, if not loaded yet.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner. If none is provided,
    /// it loads the first instance of the owner type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The owner.
    /// </returns>
    ValueTask<object> GetOwnerAsync(Guid ownerId = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified type is supported by this instance.
    /// This is usually the case for a type of the object tree of the owner.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if the specified type is supported; otherwise, <c>false</c>.
    /// </returns>
    bool IsSupporting(Type type);

    /// <summary>
    /// Discards the changes of the context, if there are any.
    /// </summary>
    ValueTask DiscardChangesAsync();

    /// <summary>
    /// Gets all objects of the given type.
    /// </summary>
    /// <typeparam name="T">The type of the requested objects.</typeparam>
    /// <returns>All objects of the given type.</returns>
    IEnumerable<T> GetAll<T>();

    /// <summary>
    /// Gets all objects of the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>All objects of the given type.</returns>
    IEnumerable GetAll(Type type);

    /// <summary>
    /// Gets the object with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the requested object.</param>
    /// <returns>The object, if found.</returns>
    IIdentifiable? Get(Guid id);
}

/// <summary>
/// A typed <see cref="IDataSource"/>.
/// </summary>
/// <typeparam name="TOwner">The type of the owner.</typeparam>
public interface IDataSource<TOwner> : IDataSource
    where TOwner : class
{
    /// <summary>
    /// Gets the owner with the specified id.
    /// Loads it from the current context, if not loaded yet.
    /// </summary>
    /// <param name="ownerId">The unique identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The owner.
    /// </returns>
    new ValueTask<TOwner> GetOwnerAsync(Guid ownerId = default, CancellationToken cancellationToken = default);
}