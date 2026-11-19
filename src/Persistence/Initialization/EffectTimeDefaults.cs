// <copyright file="EffectTimeDefaults.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Knows which magic effects send their remaining time to the client (<see cref="MagicEffectDefinition.SendDuration"/>)
/// in the data of a data initialization, e.g. the effects which had a time in Season 6.
/// </summary>
public class EffectTimeDefaults
{
    private readonly PlugInManager _plugInManager;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IPersistenceContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="EffectTimeDefaults"/> class.
    /// </summary>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="contextProvider">The context provider of the installed data.</param>
    public EffectTimeDefaults(PlugInManager plugInManager, ILoggerFactory loggerFactory, IPersistenceContextProvider contextProvider)
    {
        this._plugInManager = plugInManager;
        this._loggerFactory = loggerFactory;
        this._contextProvider = contextProvider;
    }

    /// <summary>
    /// Gets the defaults of the data initialization which created the installed configuration, e.g. of Season 6 or of 0.75.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The key and caption of the data initialization, and whether the effects send their remaining time, by their number.</returns>
    public virtual async ValueTask<(string InitializationKey, string Caption, IReadOnlyDictionary<short, bool> SendsTime)> GetForInstalledDataAsync(CancellationToken cancellationToken = default)
    {
        string initializationKey;
        using (var context = this._contextProvider.CreateNewContext())
        {
            initializationKey = await DataUpdateService.DetermineInitializationKeyAsync(context, cancellationToken).ConfigureAwait(false);
        }

        var caption = this._plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>()?[initializationKey]?.Caption ?? initializationKey;
        return (initializationKey, caption, await this.GetAsync(initializationKey, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Gets whether the magic effects send their remaining time in the data of the data initialization, by the number of the effect.
    /// When the data is requested for the first time in the process, the data initialization is executed in memory, which takes a while.
    /// </summary>
    /// <param name="initializationKey">The key of the data initialization, e.g. <see cref="VersionSeasonSix.DataInitialization.Id"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the effects send their remaining time, by their number.</returns>
    public virtual async ValueTask<IReadOnlyDictionary<short, bool>> GetAsync(string initializationKey, CancellationToken cancellationToken = default)
    {
        return await ReferenceConfiguration.GetDerivedAsync<IReadOnlyDictionary<short, bool>>(
                this._plugInManager,
                this._loggerFactory,
                initializationKey,
                "effectTimeDefaults",
                configuration => configuration.MagicEffects
                    .GroupBy(effect => effect.Number)
                    .ToDictionary(group => group.Key, group => group.Any(effect => effect.SendDuration)),
                cancellationToken)
            .ConfigureAwait(false);
    }
}
