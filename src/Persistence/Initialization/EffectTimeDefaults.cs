// <copyright file="EffectTimeDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Knows which magic effects send their remaining time to the client (<see cref="MagicEffectDefinition.SendDuration"/>)
/// in the data of a data initialization, e.g. the effects which had a time in Season 6.
/// </summary>
public class EffectTimeDefaults
{
    /// <summary>
    /// The defaults by the key of the data initialization. They only depend on the initialization code, so they're created once per process.
    /// </summary>
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<short, bool>> Defaults = new(StringComparer.Ordinal);

    /// <summary>
    /// The lock which ensures that the defaults are created only once, even if they're requested concurrently.
    /// </summary>
    private static readonly SemaphoreSlim CreationLock = new(1, 1);

    private readonly PlugInManager _plugInManager;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EffectTimeDefaults"/> class.
    /// </summary>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public EffectTimeDefaults(PlugInManager plugInManager, ILoggerFactory loggerFactory)
    {
        this._plugInManager = plugInManager;
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Gets whether the magic effects send their remaining time in the data of the data initialization, by the number of the effect.
    /// When it's requested for the first time, the data initialization is executed in memory, which takes a while.
    /// </summary>
    /// <param name="initializationKey">The key of the data initialization, e.g. <see cref="VersionSeasonSix.DataInitialization.Id"/>.</param>
    /// <returns>Whether the effects send their remaining time, by their number.</returns>
    public virtual async ValueTask<IReadOnlyDictionary<short, bool>> GetAsync(string initializationKey)
    {
        if (Defaults.TryGetValue(initializationKey, out var defaults))
        {
            return defaults;
        }

        await CreationLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!Defaults.TryGetValue(initializationKey, out defaults))
            {
                // Executed on the thread pool, so that the caller (e.g. a Blazor circuit) stays responsive.
                var configuration = await Task.Run(() => ReferenceConfiguration.CreateAsync(this._plugInManager, this._loggerFactory, initializationKey).AsTask()).ConfigureAwait(false);
                defaults = configuration.MagicEffects
                    .GroupBy(effect => effect.Number)
                    .ToDictionary(group => group.Key, group => group.Any(effect => effect.SendDuration));
                Defaults[initializationKey] = defaults;
            }

            return defaults;
        }
        finally
        {
            CreationLock.Release();
        }
    }
}
