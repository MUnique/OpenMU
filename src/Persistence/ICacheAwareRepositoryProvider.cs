// <copyright file="ICacheAwareRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// A <see cref="IRepositoryProvider"/> which has access to caching repository providers.
/// </summary>
public interface ICacheAwareRepositoryProvider : IRepositoryProvider
{
    /// <summary>
    /// Ensures the caches for current game configuration.
    /// </summary>
    void EnsureCachesForCurrentGameConfiguration();

    /// <summary>
    /// Resets the cache, so that the data is reloaded on next access.
    /// </summary>
    void ResetCache();

    /// <summary>
    /// Updates the cached instance of the given entity.
    /// </summary>
    /// <param name="changedInstance">The changed instance.</param>
    ValueTask UpdateCachedInstanceAsync(object changedInstance);
}