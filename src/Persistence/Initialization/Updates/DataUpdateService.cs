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
    /// Determines the available updates which are not installed yet,
    /// or which are installed in a previous version.
    /// </summary>
    /// <returns>The available plugins.</returns>
    /// <exception cref="System.InvalidOperationException">The plugin manager is not initialized.</exception>
    public async ValueTask<IReadOnlyCollection<IConfigurationUpdatePlugIn>> DetermineAvailableUpdatesAsync()
    {
        using var context = this._contextProvider.CreateNewContext();
        var installedVersions = await GetInstalledVersionsAsync(context).ConfigureAwait(false);

        var initializationKey = await this.DetermineInitializationKeyAsync(context).ConfigureAwait(false);
        var installedKeys = installedVersions.Keys.ToHashSet();

        var updateStrategyProvider = this._plugInManager.GetStrategyProvider<Guid, IConfigurationUpdatePlugIn>();
        if (updateStrategyProvider is null)
        {
            // it's null when there are no plugins yet ...
            return [];
        }

        var pending = updateStrategyProvider.AvailableStrategies
            .Where(up => up.DataInitializationKey == initializationKey)
            .Where(up => !installedVersions.TryGetValue(up.Key, out var installedVersion) || installedVersion < up.Version)
            .ToList();

        return OrderByDependencies(pending, installedKeys);
    }

    /// <summary>
    /// Gets the installed versions and installation dates of all configuration updates, by update key.
    /// </summary>
    /// <returns>A dictionary of the installed versions and installation dates by update key.</returns>
    public async ValueTask<IReadOnlyDictionary<Guid, (int Version, DateTime? InstalledAt)>> GetInstalledUpdatesAsync()
    {
        using var context = this._contextProvider.CreateNewContext();
        return (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .Where(up => up.InstalledAt is not null)
            .GroupBy(up => up.Key)
            .ToDictionary(group => group.Key, group => (group.Max(up => up.Version), group.Max(up => up.InstalledAt)));
    }

    /// <summary>
    /// Applies the updates asynchronous.
    /// </summary>
    /// <param name="updates">The updates. They are validated and ordered by their dependencies before anything is applied, so callers may pass a partial or unordered selection.</param>
    /// <param name="progress">The progress provider. Reports the progress back to the caller.</param>
    public async ValueTask ApplyUpdatesAsync(IReadOnlyList<IConfigurationUpdatePlugIn> updates, IProgress<(Guid CurrentUpdatingKey, bool IsCompleted)> progress)
    {
        using var context = this._contextProvider.CreateNewContext();
        var installedKeys = (await GetInstalledVersionsAsync(context).ConfigureAwait(false)).Keys.ToHashSet();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        var orderedUpdates = OrderByDependencies(updates, installedKeys);
        var updateStates = await context.GetAsync<ConfigurationUpdateState>().ConfigureAwait(false);
        var updateState = updateStates.FirstOrDefault() ?? context.CreateNew<ConfigurationUpdateState>();
        foreach (var update in orderedUpdates)
        {
            progress.Report((update.Key, false));
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
            updateState.InitializationKey = update.DataInitializationKey;
            await context.SaveChangesAsync().ConfigureAwait(false);
            progress.Report((update.Key, true));
        }

        progress.Report((Guid.Empty, true));
        this.UpdatesInstalled?.SafeInvokeAsync();
    }

    /// <summary>
    /// Orders the pending updates by their dependencies. Independent updates are ordered by <see cref="IConfigurationUpdatePlugIn.UpdatedAt"/>.
    /// </summary>
    /// <param name="pending">The pending updates.</param>
    /// <param name="alreadyInstalled">The keys of the already installed updates.</param>
    /// <returns>The ordered updates.</returns>
    internal static IReadOnlyList<IConfigurationUpdatePlugIn> OrderByDependencies(
        IReadOnlyCollection<IConfigurationUpdatePlugIn> pending, IReadOnlySet<Guid> alreadyInstalled)
    {
        var pendingByKey = pending.ToDictionary(p => p.Key);

        foreach (var plugin in pending)
        {
            var missing = plugin.DependsOn
                .Where(dep => !alreadyInstalled.Contains(dep) && !pendingByKey.ContainsKey(dep))
                .ToList();
            if (missing.Count > 0)
            {
                throw new MissingUpdateDependencyException(plugin, missing);
            }
        }

        var result = new List<IConfigurationUpdatePlugIn>();
        var visited = new HashSet<Guid>();
        var visiting = new HashSet<Guid>();

        void Visit(IConfigurationUpdatePlugIn plugin)
        {
            if (visited.Contains(plugin.Key))
            {
                return;
            }

            if (!visiting.Add(plugin.Key))
            {
                throw new CircularUpdateDependencyException(plugin);
            }

            foreach (var depKey in plugin.DependsOn.Where(pendingByKey.ContainsKey))
            {
                Visit(pendingByKey[depKey]);
            }

            visiting.Remove(plugin.Key);
            visited.Add(plugin.Key);
            result.Add(plugin);
        }

        foreach (var plugin in pending.OrderBy(p => p.UpdatedAt).ThenBy(p => p.Key))
        {
            Visit(plugin);
        }

        return result;
    }

    private static async ValueTask<IReadOnlyDictionary<Guid, int>> GetInstalledVersionsAsync(IContext context)
    {
        return (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .Where(up => up.InstalledAt is not null)
            .GroupBy(up => up.Key)
            .ToDictionary(group => group.Key, group => group.Max(up => up.Version));
    }

    private async ValueTask<string> DetermineInitializationKeyAsync(IContext context)
    {
        var updateStates = await context.GetAsync<ConfigurationUpdateState>().ConfigureAwait(false);
        if (updateStates.FirstOrDefault() is { InitializationKey: not null } updateState)
        {
            return updateState.InitializationKey;
        }

        // Now it's getting tricky ...
        var clientDefinitions = await context.GetAsync<GameClientDefinition>().ConfigureAwait(false);
        if (clientDefinitions.FirstOrDefault() is not { } clientDefinition)
        {
            throw new InvalidOperationException("No data installed");
        }

        return (clientDefinition.Season, clientDefinition.Episode) switch
        {
            (6, 3) => VersionSeasonSix.DataInitialization.Id,
            (0, 75) => Version075.DataInitialization.Id,
            (0, 95) => Version095d.DataInitialization.Id,
            _ => throw new InvalidOperationException($"Unknown client version: {clientDefinition}."),
        };
    }
}
