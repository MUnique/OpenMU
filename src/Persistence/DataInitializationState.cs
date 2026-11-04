// <copyright file="DataInitializationState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// The state of the data initialization of a database.
/// </summary>
public enum DataInitializationState
{
    /// <summary>
    /// It could not be determined if the data is initialized, e.g. because the
    /// check failed or timed out. It's not safe to assume that the database is empty.
    /// </summary>
    Unknown,

    /// <summary>
    /// The database doesn't contain a game configuration yet.
    /// </summary>
    NotInitialized,

    /// <summary>
    /// The database contains a game configuration.
    /// </summary>
    Initialized,
}
