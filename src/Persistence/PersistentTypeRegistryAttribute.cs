// <copyright file="PersistentTypeRegistryAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marks an assembly which contains a <see cref="IPersistentTypeRegistry"/> of its persistent types.
/// It's added by the <c>PersistentTypeRegistryGenerator</c> of the <c>MUnique.OpenMU.Persistence.Generators</c> project,
/// for assemblies which are marked with the <see cref="GeneratePersistentTypeRegistryAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class PersistentTypeRegistryAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentTypeRegistryAttribute"/> class.
    /// </summary>
    /// <param name="registryType">The type of the registry, which implements <see cref="IPersistentTypeRegistry"/>.</param>
    public PersistentTypeRegistryAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type registryType)
    {
        this.RegistryType = registryType;
    }

    /// <summary>
    /// Gets the type of the registry, which implements <see cref="IPersistentTypeRegistry"/>.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type RegistryType { get; }
}
