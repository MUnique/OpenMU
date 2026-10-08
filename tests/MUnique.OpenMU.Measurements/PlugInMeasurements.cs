// <copyright file="PlugInMeasurements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Measures the runtime costs of the plugin system.
/// </summary>
internal static class PlugInMeasurements
{
    /// <summary>
    /// Measures the plugin discovery and registration, like the server does it at startup.
    /// </summary>
    public static void MeasureDiscoveryAndRegistration()
    {
        EnsurePlugInAssembliesAreLoaded();
        Measure.Section("Plugin discovery and registration");

        PlugInManager? manager = null;
        var first = Measure.Run(() =>
        {
            manager = new PlugInManager(null, NullLoggerFactory.Instance, new ServiceContainer(), null);
            manager.DiscoverAndRegisterPlugIns();
        });

        var plugInTypes = manager!.KnownPlugInTypes.ToList();
        var proxiedPlugInPoints = GetPlugInPointInterfaces()
            .Where(i => !IsStrategyPlugInPoint(i))
            .Count(i => plugInTypes.Any(i.IsAssignableFrom));
        Measure.Row("1st PlugInManager: DiscoverAndRegisterPlugIns (cold)", first, $"{plugInTypes.Count} plugins, {proxiedPlugInPoints} compiled proxies");

        // The server creates more than one PlugInManager, e.g. in Startup.PlugInConfigurationsFactory and the main one.
        var second = Measure.Run(() =>
        {
            var secondManager = new PlugInManager(null, NullLoggerFactory.Instance, new ServiceContainer(), null);
            secondManager.DiscoverAndRegisterPlugIns();
        });
        Measure.Row("2nd PlugInManager: DiscoverAndRegisterPlugIns", second, "proxies are compiled again");

        var scan = Measure.Run(() =>
        {
            _ = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.GetCustomAttribute<PlugInAttribute>() is not null)
                .ToList();
        });
        Measure.Row("Assembly scan for [PlugIn] types only", scan);

        var name = Measure.Run(() =>
        {
            _ = new PlugInConfiguration { TypeId = plugInTypes[0].GUID }.Name;
        });
        Measure.Row("PlugInConfiguration.Name (one access)", name, "scans all types of all assemblies on every access");
    }

    /// <summary>
    /// Measures only the runtime compilation of the plugin proxies.
    /// </summary>
    public static void MeasureProxyCompilation()
    {
        EnsurePlugInAssembliesAreLoaded();
        Measure.Section("Plugin proxy compilation (Roslyn at runtime)");

        // The proxy compilation references all assemblies which are loaded at the time of the first compilation.
        // In the server, the plugin discovery loads them before, so we do the same here, without measuring it.
        // Like the server, we only compile proxies for plugin points which have plugins.
        var plugInTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.GetCustomAttribute<PlugInAttribute>() is not null)
            .ToList();
        _ = Assembly.Load("netstandard");

        var manager = new PlugInManager(null, NullLoggerFactory.Instance, new ServiceContainer(), null);
        var generateProxyMethod = typeof(PlugInProxyTypeGenerator).GetMethod(nameof(PlugInProxyTypeGenerator.GenerateProxy))!;
        var interfaces = GetPlugInPointInterfaces()
            .Where(i => !IsStrategyPlugInPoint(i))
            .Where(i => plugInTypes.Any(i.IsAssignableFrom))
            .ToList();

        var results = new List<Measure.Result>();
        var all = Measure.Run(() =>
        {
            foreach (var plugInInterface in interfaces)
            {
                var generator = new PlugInProxyTypeGenerator();
                results.Add(Measure.Run(() => generateProxyMethod.MakeGenericMethod(plugInInterface).Invoke(generator, [manager])));
            }
        });

        Measure.Row("First proxy (cold)", results[0], "includes loading and warming up Roslyn");
        var warm = results.Skip(1).OrderBy(r => r.Duration).ToList();
        Measure.Row($"Following proxies (median of {warm.Count})", warm[warm.Count / 2]);
        Measure.Row($"All {interfaces.Count} proxies", all);

        var allAgain = Measure.Run(() =>
        {
            foreach (var plugInInterface in interfaces)
            {
                generateProxyMethod.MakeGenericMethod(plugInInterface).Invoke(new PlugInProxyTypeGenerator(), [manager]);
            }
        });
        Measure.Row($"All {interfaces.Count} proxies again (warm)", allAgain);
    }

    private static void EnsurePlugInAssembliesAreLoaded()
    {
        // The server references these assemblies, so they are loaded when the plugins are discovered.
        _ = typeof(GameServer.GameServer).Assembly;
        _ = typeof(GameLogic.GameContext).Assembly;
        _ = typeof(Persistence.Initialization.DataInitializationBase).Assembly;
        _ = typeof(ChatServer.ChatServer).Assembly;
        _ = typeof(ConnectServer.ConnectServer).Assembly;
        _ = typeof(Network.Connection).Assembly;
        _ = typeof(Persistence.EntityFramework.EntityDataContext).Assembly;
    }

    private static List<Type> GetPlugInPointInterfaces()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.FullName?.StartsWith("MUnique.OpenMU", StringComparison.Ordinal) is true)
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.IsInterface && t.GetCustomAttribute<PlugInPointAttribute>() is not null)
            .Select(t => t.AsType())
            .ToList();
    }

    private static bool IsStrategyPlugInPoint(Type plugInInterface)
    {
        return plugInInterface.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStrategyPlugIn<>));
    }
}
