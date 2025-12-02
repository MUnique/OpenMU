// <copyright file="IMigratableDatabaseContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.Persistence;

using System.Threading;

/// <summary>
/// Provider for persistence contexts which supports database creation and migration.
/// TODO: find better name? ;-)
/// </summary>
public interface IMigratableDatabaseContextProvider : IPersistenceContextProvider
{
    /// <summary>
    /// Determines if the database exists already, by checking if any migration has been applied.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>True</c>, if the database exists; Otherwise, <c>false</c>.
    /// </returns>
    Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the database schema is up to date.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>true</c> if the database is up to date; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsDatabaseUpToDateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies all pending updates to the database schema.
    /// </summary>
    Task ApplyAllPendingUpdatesAsync();

    /// <summary>
    /// Waits until all database updates are applied.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task WaitForUpdatedDatabaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether this instance can connect to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   <c>true</c> if this instance can connect to the database; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether this instance should do an automatic schema update
    /// without asking the user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// <c>true</c> if this instance should do an automatic schema update
    /// without asking the user.; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ShouldDoAutoSchemaUpdateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Recreates the database by deleting and creating it again.
    /// </summary>
    /// <returns>The disposable which should be disposed when the data creation process is finished.</returns>
    Task<IDisposable> ReCreateDatabaseAsync();

    /// <summary>
    /// Resets the cache of this instance.
    /// </summary>
    void ResetCache();
}