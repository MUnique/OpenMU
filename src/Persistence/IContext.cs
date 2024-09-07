// <copyright file="IContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using System.Threading;

/// <summary>
/// The context for repository actions.
/// </summary>
public interface IContext : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this instance has changes.
    /// </summary>
    bool HasChanges { get; }

    /// <summary>
    /// Saves the changes of the context.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>True</c>, if the saving was successful; <c>false</c>, otherwise.
    /// </returns>
    ValueTask<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Suspends the change notifications.
    /// The notifications are re-enabled when the returned disposable is disposed,
    /// but the notifications are not triggered for the changes which happened during the suspension.
    /// </summary>
    /// <returns>A disposable, which must be disposed to re-enable notifications.</returns>
    IDisposable SuspendChangeNotifications();

    /// <summary>
    /// Detaches the specified item from the context, if required.
    /// All reachable navigation properties are recursively detached from the context, too.
    /// </summary>
    /// <param name="item">The item which should be detached.</param>
    /// <returns><c>true</c>, if the object was persisted.</returns>
    /// <remarks>
    /// When calling this method, be sure to clear a back reference property before. Otherwise, you might detach more than you intended.
    /// </remarks>
    bool Detach(object item);

    /// <summary>
    /// Attaches the specified item to the context in an unmodified state.
    /// All reachable navigations are recursively attached to the context, too.
    /// </summary>
    /// <param name="item">The item which should be attached.</param>
    /// <remarks>
    /// When calling this method, be sure to clear a previous back reference property before. Otherwise, you might attach more than you intended.
    /// </remarks>
    void Attach(object item);

    /// <summary>
    /// Creates a new instance of <typeparamref name="T" />.
    /// Attention: This operation needs a currently used context in the current thread!.
    /// </summary>
    /// <typeparam name="T">The type which should get created.</typeparam>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// A new instance of <typeparamref name="T" />.
    /// </returns>
    T CreateNew<T>(params object?[] args)
        where T : class;

    /// <summary>
    /// Creates a new instance of the given type.
    /// Attention: This operation needs a currently used context in the current thread!.
    /// </summary>
    /// <param name="type">The type which should get created.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// A new instance of <paramref name="type" />.
    /// </returns>
    object CreateNew(Type type, params object?[] args);

    /// <summary>
    /// Deletes the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object.</param>
    /// <returns><c>True</c>, if successful; Otherwise, false.</returns>
    ValueTask<bool> DeleteAsync<T>(T obj)
        where T : class;

    /// <summary>
    /// Gets the object of the specified type by its identifier.
    /// </summary>
    /// <typeparam name="T">The type of the requested object.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The object of the specified type by its identifier.
    /// </returns>
    Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Gets the object of the specified type by its identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="type">The type of the requested object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The object of the specified type by its identifier.
    /// </returns>
    Task<object?> GetByIdAsync(Guid id, Type type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all objects of the specified type. Use with caution!.
    /// </summary>
    /// <typeparam name="T">The type of the requested objects.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// All objects of the specified type.
    /// </returns>
    ValueTask<IEnumerable<T>> GetAsync<T>(CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Gets all objects of the specified type. Use with caution!.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// All objects of the specified type.
    /// </returns>
    ValueTask<IEnumerable> GetAsync(Type type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified type is supported by this instance.
    /// This is usually the case for a type of the object tree of the owner.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if the specified type is supported; otherwise, <c>false</c>.
    /// </returns>
    bool IsSupporting(Type type);
}