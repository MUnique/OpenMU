// <copyright file="IPersistentTypeRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// A registry of the persistent types of an assembly, which is generated at compile time.
/// </summary>
/// <seealso cref="PersistentTypeRegistryAttribute"/>
public interface IPersistentTypeRegistry
{
    /// <summary>
    /// Gets the persistent types, in the order of their definition.
    /// </summary>
    IReadOnlyList<PersistentType> Types { get; }
}
