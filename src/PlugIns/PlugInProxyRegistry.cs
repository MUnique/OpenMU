// <copyright file="PlugInProxyRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Registry for the proxies of plugin points, which are generated at compile time
/// by the <c>PlugInProxyGenerator</c> of the <c>MUnique.OpenMU.PlugIns.Generators</c> project.
/// </summary>
/// <remarks>
/// The generated proxies register themselves with a module initializer. For plugin points without
/// a generated proxy, the <see cref="PlugInManager"/> generates the proxy at runtime with the <see cref="PlugInProxyTypeGenerator"/>.
/// </remarks>
public static class PlugInProxyRegistry
{
    private static readonly ConcurrentDictionary<Type, Func<PlugInManager, object>> Factories = new();

    /// <summary>
    /// Registers the factory of a generated proxy.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plugin point interface.</typeparam>
    /// <param name="factory">The factory which creates the proxy.</param>
    public static void Register<TPlugIn>(Func<PlugInManager, IPlugInContainer<TPlugIn>> factory)
        where TPlugIn : class
    {
        Factories[typeof(TPlugIn)] = factory;
    }

    /// <summary>
    /// Tries to create the generated proxy for the plugin point.
    /// </summary>
    /// <typeparam name="TPlugIn">The type of the plugin point interface.</typeparam>
    /// <param name="manager">The plugin manager.</param>
    /// <param name="proxy">The created proxy.</param>
    /// <returns><see langword="true"/>, if a generated proxy has been found and created.</returns>
    internal static bool TryCreate<TPlugIn>(PlugInManager manager, [NotNullWhen(true)] out IPlugInContainer<TPlugIn>? proxy)
        where TPlugIn : class
    {
        if (!Factories.TryGetValue(typeof(TPlugIn), out var factory))
        {
            // The module initializer, which registers the proxy, runs before the first access to the module.
            // We make sure here that it ran, because the runtime is free to postpone it until then.
            RuntimeHelpers.RunModuleConstructor(typeof(TPlugIn).Module.ModuleHandle);
            Factories.TryGetValue(typeof(TPlugIn), out factory);
        }

        proxy = factory?.Invoke(manager) as IPlugInContainer<TPlugIn>;
        return proxy is not null;
    }
}
