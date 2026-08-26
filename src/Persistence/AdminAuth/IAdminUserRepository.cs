// <copyright file="IAdminUserRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

using System.Threading;

/// <summary>
/// A repository for the <see cref="AdminUser"/>s of the admin panel.
/// </summary>
/// <remarks>
/// The implementation must be usable independently of the game database:
/// The admin panel is the tool which creates the game database in the first place,
/// so its users can't be stored within it.
/// </remarks>
public interface IAdminUserRepository
{
    /// <summary>
    /// Ensures that the underlying storage exists and is up to date.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>true</c>, if the storage is available; otherwise, <c>false</c>, e.g. when no database server is reachable.
    /// </returns>
    ValueTask<bool> EnsureStorageAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the number of stored users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of stored users.</returns>
    ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stored users, ordered by their login name.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stored users.</returns>
    ValueTask<IList<AdminUser>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user, if found; otherwise, <c>null</c>.</returns>
    ValueTask<AdminUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user with the specified normalized login name.
    /// </summary>
    /// <param name="normalizedLoginName">The normalized login name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user, if found; otherwise, <c>null</c>.</returns>
    ValueTask<AdminUser?> GetByNormalizedLoginNameAsync(string normalizedLoginName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask AddAsync(AdminUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask UpdateAsync(AdminUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask DeleteAsync(AdminUser user, CancellationToken cancellationToken = default);
}
