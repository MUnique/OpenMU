// <copyright file="IContextAwareRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// A <see cref="IRepositoryProvider"/> which can resolve repositories for an explicitly given
/// originating context, instead of relying on an ambient context.
/// </summary>
internal interface IContextAwareRepositoryProvider : IRepositoryProvider
{
    /// <summary>
    /// Gets the repository of the specified type for the given originating context.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="context">The originating context, or <c>null</c>.</param>
    /// <returns>The repository of the specified type.</returns>
    IRepository? GetRepository(Type objectType, EntityFrameworkContextBase? context);

    /// <summary>
    /// Gets the repository of the specified generic type for the given originating context.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <param name="context">The originating context, or <c>null</c>.</param>
    /// <returns>The repository of the specified generic type.</returns>
    IRepository<T>? GetRepository<T>(EntityFrameworkContextBase? context)
        where T : class;

    /// <summary>
    /// Gets the repository of the specified generic type for the given originating context.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <typeparam name="TRepository">The type of the repository.</typeparam>
    /// <param name="context">The originating context, or <c>null</c>.</param>
    /// <returns>The repository of the specified generic type.</returns>
    TRepository? GetRepository<T, TRepository>(EntityFrameworkContextBase? context)
        where T : class
        where TRepository : IRepository;

    /// <summary>
    /// Ensures the caches for the game configuration of the given originating context.
    /// </summary>
    /// <param name="context">The originating context which holds the current game configuration.</param>
    void EnsureCachesForCurrentGameConfiguration(EntityFrameworkContextBase context);
}
