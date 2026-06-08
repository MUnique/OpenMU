// <copyright file="IContextAwareRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.Collections;
using System.Threading;

/// <summary>
/// Interface for a repository which can operate within an explicitly given originating context,
/// instead of relying on an ambient context.
/// </summary>
/// <remarks>
/// The originating context is the <see cref="EntityFrameworkContextBase"/> which started the
/// current operation. It provides the <see cref="EntityFrameworkContextBase.Context"/> to work
/// with and, for configuration data, the current game configuration. When <c>null</c> is passed,
/// a new temporary context is used for the action.
/// </remarks>
internal interface IContextAwareRepository : IRepository
{
    /// <summary>
    /// Gets the object by an identifier, using the given originating context.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="context">The originating context, or <c>null</c> to use a temporary context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded object, or null if not found.</returns>
    ValueTask<object?> GetByIdAsync(Guid id, EntityFrameworkContextBase? context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all objects, using the given originating context.
    /// </summary>
    /// <param name="context">The originating context, or <c>null</c> to use a temporary context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All objects of the repository.</returns>
    ValueTask<IEnumerable> GetAllAsync(EntityFrameworkContextBase? context, CancellationToken cancellationToken = default);
}
