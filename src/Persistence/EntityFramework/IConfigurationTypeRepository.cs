// <copyright file="IConfigurationTypeRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Non-generic interface for <see cref="ConfigurationTypeRepository{T}"/>.
/// </summary>
internal interface IConfigurationTypeRepository
{
    /// <summary>
    /// Ensures the cache for the current configuration.
    /// </summary>
    void EnsureCacheForCurrentConfiguration();

    /// <summary>
    /// Updates the cached instance.
    /// </summary>
    /// <param name="changedInstance">The changed instance.</param>
    void UpdateCachedInstances(object changedInstance);
}