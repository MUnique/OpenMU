// <copyright file="ConfigurationCaptionService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Captions;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Service to keep the captions of the configuration in sync with their sources,
/// e.g. when new translations become available.
/// </summary>
public class ConfigurationCaptionService
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly PlugInManager _plugInManager;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationCaptionService"/> class.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConfigurationCaptionService(IPersistenceContextProvider contextProvider, PlugInManager plugInManager, ILoggerFactory loggerFactory)
    {
        this._contextProvider = contextProvider;
        this._plugInManager = plugInManager;
        this._loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Compares the configured captions with their sources.
    /// </summary>
    /// <returns>The result of the comparison.</returns>
    public async ValueTask<CaptionComparison> CompareAsync()
    {
        using var context = this._contextProvider.CreateNewContext();
        var gameConfiguration = await GetGameConfigurationAsync(context).ConfigureAwait(false);
        return new CaptionComparison(
            ConfigurationCaptions.CountLinkedCaptions(gameConfiguration),
            ConfigurationCaptions.DetermineChanges(gameConfiguration),
            ConfigurationCaptions.FindUnresolvedSourceKeys(gameConfiguration));
    }

    /// <summary>
    /// Applies the selected changes and saves them.
    /// </summary>
    /// <param name="selectedChangeIds">The identifiers of the selected changes.</param>
    /// <returns>The number of applied changes.</returns>
    public async ValueTask<int> ApplyChangesAsync(IEnumerable<string> selectedChangeIds)
    {
        using var context = this._contextProvider.CreateNewContext();
        var gameConfiguration = await GetGameConfigurationAsync(context).ConfigureAwait(false);
        var applied = ConfigurationCaptions.ApplyChanges(gameConfiguration, selectedChangeIds);
        await SaveWithoutChangeNotificationsAsync(context).ConfigureAwait(false);
        return applied;
    }

    /// <summary>
    /// Links the built-in captions of the configuration to their sources, if they're not linked yet.
    /// This is required once for configurations which were created before captions got source keys.
    /// To determine the source keys, the data initialization of the configuration is executed in memory.
    /// </summary>
    /// <param name="progress">The progress, which receives the current step.</param>
    /// <returns>The number of linked captions and the number of captions which were skipped because their neutral text was customized.</returns>
    public async ValueTask<(int Linked, int SkippedBecauseOfCustomizedNeutralText)> LinkBuiltInCaptionsAsync(IProgress<CaptionLinkStep>? progress = null)
    {
        progress?.Report(CaptionLinkStep.LoadingConfiguration);
        using var context = this._contextProvider.CreateNewContext();
        var initializationKey = await DataUpdateService.DetermineInitializationKeyAsync(context).ConfigureAwait(false);
        var gameConfiguration = await GetGameConfigurationAsync(context).ConfigureAwait(false);

        // The initialization and the linking are mostly synchronous, CPU bound work.
        // They're executed on the thread pool, so that the caller (e.g. a Blazor circuit) stays responsive.
        progress?.Report(CaptionLinkStep.CreatingReferenceConfiguration);
        var referenceConfiguration = await Task.Run(() => this.CreateReferenceConfigurationAsync(initializationKey).AsTask()).ConfigureAwait(false);

        progress?.Report(CaptionLinkStep.LinkingCaptions);
        var result = await Task.Run(() => ConfigurationCaptions.LinkSourceKeys(gameConfiguration, referenceConfiguration)).ConfigureAwait(false);

        progress?.Report(CaptionLinkStep.Saving);
        await SaveWithoutChangeNotificationsAsync(context).ConfigureAwait(false);

        progress?.Report(CaptionLinkStep.Completed);
        return result;
    }

    /// <summary>
    /// Saves the changes without publishing them as configuration changes to running servers.
    /// Publishing is expensive for many changed captions (the parent of every changed object is searched),
    /// and the captions take effect after a restart anyway, like configuration updates.
    /// </summary>
    /// <param name="context">The context.</param>
    private static async ValueTask SaveWithoutChangeNotificationsAsync(IContext context)
    {
        using var suspension = context.SuspendChangeNotifications();
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async ValueTask<GameConfiguration> GetGameConfigurationAsync(IContext context)
    {
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).FirstOrDefault()
               ?? throw new InvalidOperationException("No game configuration installed.");
    }

    private async ValueTask<GameConfiguration> CreateReferenceConfigurationAsync(string initializationKey)
    {
        var plugIn = this._plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>()?[initializationKey]
                     ?? throw new InvalidOperationException($"The data initialization '{initializationKey}' is not available.");

        var inMemoryProvider = new InMemoryPersistenceContextProvider();
        var referenceInitialization = Activator.CreateInstance(plugIn.GetType(), inMemoryProvider, this._loggerFactory) as IDataInitializationPlugIn
                                      ?? throw new InvalidOperationException($"The data initialization '{initializationKey}' can't be executed in memory.");
        await referenceInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using var referenceContext = inMemoryProvider.CreateNewContext();
        return await GetGameConfigurationAsync(referenceContext).ConfigureAwait(false);
    }
}
