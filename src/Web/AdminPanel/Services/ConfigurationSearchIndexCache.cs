// <copyright file="ConfigurationSearchIndexCache.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Caches configuration search entries for fast header search navigation.
/// </summary>
public class ConfigurationSearchIndexCache
{
    private readonly IMigratableDatabaseContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _configDataSource;
    private readonly ILogger<ConfigurationSearchIndexCache> _logger;
    private readonly SemaphoreSlim _loadingLock = new(1, 1);

    private bool _isLoaded;
    private IReadOnlyList<ConfigurationSearchEntry> _entries = Array.Empty<ConfigurationSearchEntry>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationSearchIndexCache"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="configDataSource">The configuration data source.</param>
    /// <param name="logger">The logger.</param>
    public ConfigurationSearchIndexCache(
        IMigratableDatabaseContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> configDataSource,
        ILogger<ConfigurationSearchIndexCache> logger)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._configDataSource = configDataSource;
        this._logger = logger;
    }

    /// <summary>
    /// Gets a value indicating whether the cache was loaded at least once.
    /// </summary>
    public bool IsLoaded => this._isLoaded;

    /// <summary>
    /// Gets the cached entries.
    /// </summary>
    public IReadOnlyList<ConfigurationSearchEntry> Entries => this._entries;

    /// <summary>
    /// Ensures the cache is populated.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task.</returns>
    public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
    {
        if (this._isLoaded)
        {
            return;
        }

        await this._loadingLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (this._isLoaded)
            {
                return;
            }

            this._entries = await this.LoadEntriesAsync(cancellationToken).ConfigureAwait(false);
            this._isLoaded = true;
        }
        finally
        {
            this._loadingLock.Release();
        }
    }

    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    public void Invalidate()
    {
        this._entries = Array.Empty<ConfigurationSearchEntry>();
        this._isLoaded = false;
    }

    private async Task<IReadOnlyList<ConfigurationSearchEntry>> LoadEntriesAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            if (!await this._persistenceContextProvider.CanConnectToDatabaseAsync(token).ConfigureAwait(false)
                || !await this._persistenceContextProvider.DatabaseExistsAsync(token).ConfigureAwait(false))
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            using var context = this._persistenceContextProvider.CreateNewConfigurationContext();
            var gameConfigurationId = await context.GetDefaultGameConfigurationIdAsync(token).ConfigureAwait(false);
            if (gameConfigurationId is not { } id || id == Guid.Empty)
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            var gameConfiguration = await this._configDataSource.GetOwnerAsync(id, token).ConfigureAwait(false);
            this._logger.LogInformation("Configuration search data loaded in {0} ms.", stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();

            var result = await ConfigurationSearchIndexer.BuildSearchIndexAsync(gameConfiguration, id).ConfigureAwait(false);
            stopwatch.Stop();
            this._logger.LogInformation(
                "Configuration search index loaded with {0} entries in {1} ms.",
                result.Count,
                stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Could not load the configuration search index.");
            return Array.Empty<ConfigurationSearchEntry>();
        }
    }
}