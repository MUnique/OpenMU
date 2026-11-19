// <copyright file="ReferenceConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Creates the configuration of a data initialization in memory, e.g. to compare the installed configuration with its defaults.
/// </summary>
public static class ReferenceConfiguration
{
    /// <summary>
    /// What the users derived from the configurations, by the key of the data initialization and the name which the user chose.
    /// It only depends on the initialization code and the resources, so it's derived once per process.
    /// </summary>
    private static readonly ConcurrentDictionary<(string InitializationKey, string Name), object> DerivedValues = new();

    /// <summary>
    /// The failures which can't heal, by the key of the data initialization and the name of the derived value, or <c>null</c>
    /// for the creation: the data initialization isn't available or can't be executed in memory, or the derivation failed.
    /// They depend on the code and the installed plug-ins, so they're remembered for the process and not executed again
    /// on every request. Other failures, e.g. an <see cref="OutOfMemoryException"/> of a busy server, aren't remembered,
    /// so the next request tries again.
    /// </summary>
    private static readonly ConcurrentDictionary<(string InitializationKey, string? Name), Exception> Failures = new();

    /// <summary>
    /// The configurations by the key of their data initialization, as long as they're in memory anyway.
    /// They're big, so they're only kept weakly: users which derive their values close together share one,
    /// but a server process doesn't keep it after that.
    /// </summary>
    private static readonly ConcurrentDictionary<string, WeakReference<GameConfiguration>> Configurations = new(StringComparer.Ordinal);

    /// <summary>
    /// The lock which ensures that a value is derived only once, even if it's requested concurrently.
    /// </summary>
    private static readonly SemaphoreSlim CreationLock = new(1, 1);

    /// <summary>
    /// Gets a value which is derived from the game configuration of the data initialization, e.g. its captions.
    /// When it's requested for the first time in the process, the data initialization is executed in memory, which takes a while.
    /// </summary>
    /// <typeparam name="T">The type of the derived value.</typeparam>
    /// <param name="plugInManager">The plug in manager, which knows the data initializations.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="initializationKey">The key of the data initialization, e.g. "season6".</param>
    /// <param name="name">The name of the derived value, unique among the users, e.g. "captions"; one value is kept per name and data initialization.</param>
    /// <param name="derive">Derives the value from the configuration; it must not change the configuration or keep it.</param>
    /// <param name="cancellationToken">The cancellation token; it ends the wait for another creation, not a running one.</param>
    /// <returns>The derived value.</returns>
    /// <exception cref="ReferenceConfigurationFailedException">The data initialization isn't available or can't be executed in memory, or the derivation failed, also earlier in the process.
    /// Other failures, e.g. of the data initialization while it creates its data, propagate unchanged and aren't remembered.</exception>
    public static async ValueTask<T> GetDerivedAsync<T>(PlugInManager plugInManager, ILoggerFactory loggerFactory, string initializationKey, string name, Func<GameConfiguration, T> derive, CancellationToken cancellationToken = default)
        where T : class
    {
        var cacheKey = (initializationKey, name);
        if (DerivedValues.TryGetValue(cacheKey, out var value))
        {
            return (T)value;
        }

        ThrowIfFailedBefore(initializationKey, name);
        await CreationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!DerivedValues.TryGetValue(cacheKey, out value))
            {
                ThrowIfFailedBefore(initializationKey, name);
                var configuration = await GetConfigurationAsync(plugInManager, loggerFactory, initializationKey).ConfigureAwait(false);
                try
                {
                    value = await Task.Run(() => derive(configuration)).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OutOfMemoryException)
                {
                    // The derivation only depends on the code, so it fails again; only a lack of memory may heal.
                    throw Remember(loggerFactory, initializationKey, name, ex);
                }

                DerivedValues[cacheKey] = value;
            }

            return (T)value;
        }
        finally
        {
            CreationLock.Release();
        }
    }

    /// <summary>
    /// Throws a new exception for the failure, if creating the configuration or deriving the value failed before in the process
    /// in a way which can't heal. Each request gets its own exception, as requests may throw it concurrently.
    /// </summary>
    /// <param name="initializationKey">The key of the data initialization.</param>
    /// <param name="name">The name of the derived value.</param>
    private static void ThrowIfFailedBefore(string initializationKey, string name)
    {
        if (Failures.TryGetValue((initializationKey, null), out var failure) || Failures.TryGetValue((initializationKey, name), out failure))
        {
            throw new ReferenceConfigurationFailedException(initializationKey, failure);
        }
    }

    /// <summary>
    /// Remembers a failure which can't heal for the process and logs it once.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="initializationKey">The key of the data initialization.</param>
    /// <param name="name">The name of the derived value, or <c>null</c> for the creation of the configuration.</param>
    /// <param name="failure">The failure.</param>
    /// <returns>The exception to throw.</returns>
    private static ReferenceConfigurationFailedException Remember(ILoggerFactory loggerFactory, string initializationKey, string? name, Exception failure)
    {
        Failures[(initializationKey, name)] = failure;
        loggerFactory.CreateLogger(typeof(ReferenceConfiguration)).LogWarning(
            failure,
            "The reference configuration of the data initialization {InitializationKey} failed ({Name}); it isn't tried again until the next start: {Message}",
            initializationKey,
            name ?? "creation",
            failure.Message);
        return new ReferenceConfigurationFailedException(initializationKey, failure);
    }

    /// <summary>
    /// Gets the configuration while it's still in memory, or creates it. Must be called under the <see cref="CreationLock"/>.
    /// </summary>
    /// <param name="plugInManager">The plug in manager, which knows the data initializations.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="initializationKey">The key of the data initialization.</param>
    /// <returns>The game configuration of the data initialization.</returns>
    private static async ValueTask<GameConfiguration> GetConfigurationAsync(PlugInManager plugInManager, ILoggerFactory loggerFactory, string initializationKey)
    {
        if (Configurations.TryGetValue(initializationKey, out var reference) && reference.TryGetTarget(out var configuration))
        {
            return configuration;
        }

        try
        {
            // The initialization is mostly synchronous, CPU bound work.
            // It's executed on the thread pool, so that the caller (e.g. a Blazor circuit) stays responsive.
            configuration = await Task.Run(() => CreateAsync(plugInManager, loggerFactory, initializationKey).AsTask()).ConfigureAwait(false);
        }
        catch (ReferenceConfigurationFailedException ex) when (ex.InitializationKey == initializationKey)
        {
            throw Remember(loggerFactory, initializationKey, null, ex.InnerException!);
        }

        Configurations[initializationKey] = new WeakReference<GameConfiguration>(configuration);
        return configuration;
    }

    /// <summary>
    /// Executes the data initialization in memory and returns its game configuration.
    /// This is mostly synchronous, CPU bound work which takes a while; <see cref="GetDerivedAsync{T}"/> does it once per process.
    /// </summary>
    /// <param name="plugInManager">The plug in manager, which knows the data initializations.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="initializationKey">The key of the data initialization, e.g. "season6".</param>
    /// <returns>The game configuration of the data initialization.</returns>
    private static async ValueTask<GameConfiguration> CreateAsync(PlugInManager plugInManager, ILoggerFactory loggerFactory, string initializationKey)
    {
        var plugIn = plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>()?[initializationKey]
                     ?? throw Unavailable(initializationKey, "is not available");

        var inMemoryProvider = new InMemoryPersistenceContextProvider();
        IDataInitializationPlugIn referenceInitialization;
        try
        {
            // The plug in comes from the provider of data initializations, so its type implements the interface.
            referenceInitialization = (IDataInitializationPlugIn)Activator.CreateInstance(plugIn.GetType(), inMemoryProvider, loggerFactory)!;
        }
        catch (MissingMethodException ex)
        {
            throw Unavailable(initializationKey, "can't be executed in memory", ex);
        }

        await referenceInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using var referenceContext = inMemoryProvider.CreateNewContext();
        return (await referenceContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault()
               ?? throw Unavailable(initializationKey, "created no game configuration");
    }

    /// <summary>
    /// Creates the exception for a failure of the creation which can't heal: the data initialization isn't available,
    /// can't be executed in memory, or creates no game configuration. Its inner exception is the failure which is remembered.
    /// </summary>
    /// <param name="initializationKey">The key of the data initialization.</param>
    /// <param name="reason">The reason, e.g. "is not available".</param>
    /// <param name="innerException">The exception which caused it, if any.</param>
    /// <returns>The exception to throw.</returns>
    private static ReferenceConfigurationFailedException Unavailable(string initializationKey, string reason, Exception? innerException = null)
    {
        return new ReferenceConfigurationFailedException(initializationKey, new InvalidOperationException($"The data initialization '{initializationKey}' {reason}.", innerException));
    }
}
