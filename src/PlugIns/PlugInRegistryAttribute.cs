// <copyright file="PlugInRegistryAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marks an assembly which contains a <see cref="IPlugInRegistry"/> of its plugins.
/// It's added by the <c>PlugInRegistryGenerator</c> of the <c>MUnique.OpenMU.PlugIns.Generators</c> project.
/// </summary>
/// <remarks>
/// When the <see cref="PlugInManager"/> discovers the plugins of an assembly with this attribute,
/// it takes them from the registry instead of searching all types of the assembly.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class PlugInRegistryAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInRegistryAttribute"/> class.
    /// </summary>
    /// <param name="registryType">The type of the registry, which implements <see cref="IPlugInRegistry"/>.</param>
    public PlugInRegistryAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type registryType)
    {
        this.RegistryType = registryType;
    }

    /// <summary>
    /// Gets the type of the registry, which implements <see cref="IPlugInRegistry"/>.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public Type RegistryType { get; }
}
