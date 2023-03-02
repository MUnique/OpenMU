// <copyright file="DataUpdateManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Manager which applies updates of previously initialized data by a <see cref="IDataInitializationPlugIn"/>.
/// </summary>
internal class DataUpdateManager
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly PlugInManager _plugInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataUpdateManager"/> class.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    public DataUpdateManager(IPersistenceContextProvider contextProvider, PlugInManager plugInManager)
    {
        this._contextProvider = contextProvider;
        this._plugInManager = plugInManager;
    }

    /// <summary>
    /// Determines the available updates.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">The plugin manager is not initialized.</exception>
    public async ValueTask<IReadOnlyCollection<IConfigurationUpdatePlugIn>> DetermineAvailableUpdatesAsync(IDataInitializationPlugIn initializationPlugIn)
    {
        using var context = this._contextProvider.CreateNewContext();
        var updates = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).ToList();
        var maxVersion = updates.Where(up => up.InstalledAt is not null).Max(up => up.Version);
        var notInstalledUpdates = updates
            .Where(up => up.InstalledAt is null)
            .Select(up => up.Version)
            .ToHashSet();

        var updateStrategyProvider = this._plugInManager.GetStrategyProvider<int, IConfigurationUpdatePlugIn>()
                                     ?? throw new InvalidOperationException("The plugin manager is not initialized.");

        var availableUpdates = updateStrategyProvider.AvailableStrategies
            .Where(up => up.DataInitializationKey == initializationPlugIn.Key)
            .Where(up => up.Version > maxVersion || notInstalledUpdates.Contains(up.Version))
            .OrderBy(up => up.Version)
            .ToList();

        return availableUpdates;
    }

    public async ValueTask ApplyUpdatesAsync(IEnumerable<IConfigurationUpdatePlugIn> updates)
    {
        using var context = this._contextProvider.CreateNewContext();
        foreach (var update in updates)
        {
            await update.ApplyUpdateAsync(context).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}