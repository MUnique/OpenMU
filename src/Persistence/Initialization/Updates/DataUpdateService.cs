// <copyright file="DataUpdateService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Service which applies updates of previously initialized data by a <see cref="IDataInitializationPlugIn"/>.
/// </summary>
public class DataUpdateService
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly PlugInManager _plugInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataUpdateService"/> class.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    public DataUpdateService(IPersistenceContextProvider contextProvider, PlugInManager plugInManager)
    {
        this._contextProvider = contextProvider;
        this._plugInManager = plugInManager;
    }

    /// <summary>
    /// Occurs when updates have been installed.
    /// </summary>
    public event AsyncEventHandler? UpdatesInstalled; 

    /// <summary>
    /// Determines the available updates.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">The plugin manager is not initialized.</exception>
    public async ValueTask<IReadOnlyCollection<IConfigurationUpdatePlugIn>> DetermineAvailableUpdatesAsync()
    {
        using var context = this._contextProvider.CreateNewContext();
        var updates = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).ToList();

        var initializationKey = await DetermineInitializationKeyAsync(context);
        var installedUpdates = updates
            .Where(up => up.InstalledAt is not null)
            .Select(up => up.Version)
            .ToHashSet();

        var updateStrategyProvider = this._plugInManager.GetStrategyProvider<int, IConfigurationUpdatePlugIn>();
        if (updateStrategyProvider is null)
        {
            // it's null when there are no plugins yet ...
            return Array.Empty<IConfigurationUpdatePlugIn>();
        }

        var availableUpdates = updateStrategyProvider.AvailableStrategies
            .Where(up => up.DataInitializationKey == initializationKey)
            .Where(up => !installedUpdates.Contains(up.Version))
            .OrderBy(up => up.Version)
            .ToList();

        return availableUpdates;
    }

    /// <summary>
    /// Applies the updates asynchronous.
    /// </summary>
    /// <param name="updates">The updates.</param>
    /// <param name="progress">The progress provider. Reports b</param>
    public async ValueTask ApplyUpdatesAsync(IReadOnlyList<IConfigurationUpdatePlugIn> updates, IProgress<(int CurrentUpdatingVersion, bool IsCompleted)> progress)
    {
        using var context = this._contextProvider.CreateNewContext();
        var updateStates = await context.GetAsync<ConfigurationUpdateState>();
        var updateState = updateStates.FirstOrDefault() ?? context.CreateNew<ConfigurationUpdateState>();
        foreach (var update in updates)
        {
            progress.Report((update.Version, false));
            await update.ApplyUpdateAsync(context).ConfigureAwait(false);
            
            updateState.CurrentInstalledVersion = Math.Max(update.Version, updateState.CurrentInstalledVersion);
            updateState.InitializationKey = update.DataInitializationKey;

            await context.SaveChangesAsync().ConfigureAwait(false);
            progress.Report((update.Version, true));
        }

        progress.Report((-1, true));
        this.UpdatesInstalled?.SafeInvokeAsync();
    }

    private async ValueTask<string> DetermineInitializationKeyAsync(IContext context)
    {
        var updateStates = await context.GetAsync<ConfigurationUpdateState>();
        if (updateStates.FirstOrDefault() is { InitializationKey: not null } updateState)
        {
            return updateState.InitializationKey;
        }
        
        // Now it's getting tricky ...
        var clientDefinitions = await context.GetAsync<GameClientDefinition>();
        if (clientDefinitions.FirstOrDefault() is not { } clientDefinition)
        {
            throw new InvalidOperationException("No data installed");
        }

        return (clientDefinition.Season, clientDefinition.Episode) switch
        {
            (6, 3) => VersionSeasonSix.DataInitialization.Id,
            (0, 75) => Version075.DataInitialization.Id,
            (0, 95) => Version095d.DataInitialization.Id,
            _ => throw new InvalidOperationException($"Unknown client version: {clientDefinition}.")
        };
    }
}