// <copyright file="IPlugInRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// A registry of the plugins of an assembly, which is generated at compile time.
/// </summary>
/// <seealso cref="PlugInRegistryAttribute"/>
public interface IPlugInRegistry
{
    /// <summary>
    /// Gets the plugins of the assembly, in the order of their definition.
    /// </summary>
    IReadOnlyList<PlugInRegistration> PlugIns { get; }
}
