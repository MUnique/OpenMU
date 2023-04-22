// <copyright file="ICacheAwareRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using MUnique.OpenMU.Persistence.EntityFramework.Json;

/// <summary>
/// A <see cref="IRepositoryProvider"/> which has access to caching repository providers.
/// </summary>
interface ICacheAwareRepositoryProvider : IRepositoryProvider
{
    /// <summary>
    /// Ensures the caches for current game configuration.
    /// It's meant to fill the caches also in <see cref="ConfigurationIdReferenceResolver"/>.
    /// </summary>
    void EnsureCachesForCurrentGameConfiguration();
}