// <copyright file="PlugInRegistries.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Collections.Concurrent;
using System.Reflection;

/// <summary>
/// Provides access to the plugin registries of the assemblies, which are generated at compile time.
/// </summary>
/// <seealso cref="IPlugInRegistry"/>
internal static class PlugInRegistries
{
    /// <summary>
    /// The generated plugin registries of the assemblies; <c>null</c> for assemblies without one.
    /// </summary>
    private static readonly ConcurrentDictionary<Assembly, IPlugInRegistry?> Registries = new();

    /// <summary>
    /// The plugin types of the generated registries by their <see cref="Type.GUID"/>, per assembly.
    /// </summary>
    private static readonly ConcurrentDictionary<Assembly, IReadOnlyDictionary<Guid, Type>> PlugInTypesById = new();

    /// <summary>
    /// Gets the plugin registry of the assembly, which has been generated at compile time.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>The plugin registry of the assembly; <c>null</c>, if it has none, or if the registries are disabled.</returns>
    public static IPlugInRegistry? Get(Assembly assembly)
    {
        if (AppContext.TryGetSwitch(PlugInManager.DisableGeneratedRegistriesSwitch, out var isDisabled) && isDisabled)
        {
            return null;
        }

        return Registries.GetOrAdd(
            assembly,
            static a => a.GetCustomAttribute<PlugInRegistryAttribute>() is { } attribute
                ? Activator.CreateInstance(attribute.RegistryType) as IPlugInRegistry
                : null);
    }

    /// <summary>
    /// Finds the plugin type with the specified <see cref="Type.GUID"/> in the generated plugin registries of the loaded assemblies.
    /// </summary>
    /// <param name="typeId">The <see cref="Type.GUID"/> of the plugin type.</param>
    /// <returns>The plugin type, if it's contained in a generated registry; Otherwise, <c>null</c>.</returns>
    public static Type? FindPlugInType(Guid typeId)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (Get(assembly) is not { } registry)
            {
                continue;
            }

            var plugInTypes = PlugInTypesById.GetOrAdd(assembly, static (_, r) => CreatePlugInTypesById(r), registry);
            if (plugInTypes.TryGetValue(typeId, out var plugInType))
            {
                return plugInType;
            }
        }

        return null;
    }

    private static IReadOnlyDictionary<Guid, Type> CreatePlugInTypesById(IPlugInRegistry registry)
    {
        var result = new Dictionary<Guid, Type>();
        foreach (var plugIn in registry.PlugIns)
        {
            result.TryAdd(plugIn.Type.GUID, plugIn.Type);
        }

        return result;
    }
}
