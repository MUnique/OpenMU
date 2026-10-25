// <copyright file="PersistenceConflictException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>Reports a persistence rejection which is known not to have committed the requested change.</summary>
/// <param name="innerException">The provider's constraint or concurrency rejection.</param>
public sealed class PersistenceConflictException(Exception innerException)
    : Exception("Persistence rejected a conflicting or invalid change.", innerException);
