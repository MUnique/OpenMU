// <copyright file="IApiKeyRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.AdminAuth;

using System.Threading;

/// <summary>
/// A repository for the <see cref="ApiKey"/>s of the public API.
/// </summary>
/// <remarks>
/// Like the <see cref="IAdminUserRepository"/>, the implementation must be usable independently of
/// the game database.
/// </remarks>
public interface IApiKeyRepository
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
    /// Gets all stored keys, ordered by their name.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stored keys.</returns>
    ValueTask<IList<ApiKey>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the enabled key with the specified hash.
    /// </summary>
    /// <param name="keyHash">The hash of the presented key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key, if found and enabled; otherwise, <c>null</c>.</returns>
    ValueTask<ApiKey?> GetEnabledByHashAsync(string keyHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="apiKey">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask AddAsync(ApiKey apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified key.
    /// </summary>
    /// <param name="apiKey">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask UpdateAsync(ApiKey apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified key.
    /// </summary>
    /// <param name="apiKey">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask DeleteAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
}
