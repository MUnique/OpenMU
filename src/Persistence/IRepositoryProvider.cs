// <copyright file="IRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// An interface for an instance which provides repositories.
/// </summary>
public interface IRepositoryProvider
{
    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The repository of the specified type.</returns>
    IRepository? GetRepository(Type objectType);

    /// <summary>
    /// Gets the repository of the specified generic type.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <returns>The repository of the specified generic type.</returns>
    IRepository<T>? GetRepository<T>()
        where T : class;

    /// <summary>
    /// Gets the repository of the specified generic type.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <typeparam name="TRepository">The type of the repository.</typeparam>
    /// <returns>
    /// The repository of the specified generic type.
    /// </returns>
    TRepository? GetRepository<T, TRepository>()
        where T : class
        where TRepository : IRepository;
}