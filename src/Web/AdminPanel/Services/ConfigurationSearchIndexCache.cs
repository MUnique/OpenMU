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
public class ConfigurationSearchIndexCache : IDisposable
{
    private readonly IMigratableDatabaseContextProvider _persistenceContextProvider;
    private readonly IDataSource<GameConfiguration> _configDataSource;
    private readonly ILogger<ConfigurationSearchIndexCache> _logger;
    private readonly SetupService _setupService;
    private readonly SemaphoreSlim _loadingLock = new(1, 1);

    private bool _isLoaded;
    private IReadOnlyList<ConfigurationSearchEntry> _entries = Array.Empty<ConfigurationSearchEntry>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationSearchIndexCache"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="configDataSource">The configuration data source.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="setupService">The setup service.</param>
    public ConfigurationSearchIndexCache(
        IMigratableDatabaseContextProvider persistenceContextProvider,
        IDataSource<GameConfiguration> configDataSource,
        ILogger<ConfigurationSearchIndexCache> logger,
        SetupService setupService)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._configDataSource = configDataSource;
        this._logger = logger;
        this._setupService = setupService;

        this._setupService.DatabaseInitialized += this.OnDatabaseInitializedAsync;

        _ = Task.Run(async () =>
        {
            try
            {
                await this.EnsureLoadedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Could not warmup configuration search index.");
            }
        });
    }

    /// <summary>
    /// Gets a value indicating whether the cache was loaded at least once.
    /// </summary>
    public bool IsLoaded => this._isLoaded;

    /// <summary>
    /// Gets the cached entries.
    /// </summary>
    public IReadOnlyList<ConfigurationSearchEntry> Entries => this._entries;

    /// <inheritdoc/>
    public void Dispose()
    {
        this._setupService.DatabaseInitialized -= this.OnDatabaseInitializedAsync;
        this._loadingLock.Dispose();
    }

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

    private async ValueTask OnDatabaseInitializedAsync()
    {
        await this._loadingLock.WaitAsync().ConfigureAwait(false);
        try
        {
            this._entries = Array.Empty<ConfigurationSearchEntry>();
            this._isLoaded = false;
        }
        finally
        {
            this._loadingLock.Release();
        }

        try
        {
            await this.EnsureLoadedAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not load configuration search index on database initialization.");
        }
    }

    private async Task<IReadOnlyList<ConfigurationSearchEntry>> LoadEntriesAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            if (!await this._persistenceContextProvider.CanConnectToDatabaseAsync(cancellationToken).ConfigureAwait(false)
                || !await this._persistenceContextProvider.DatabaseExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            using var context = this._persistenceContextProvider.CreateNewConfigurationContext();
            var gameConfigurationId = await context.GetDefaultGameConfigurationIdAsync(cancellationToken).ConfigureAwait(false);
            if (gameConfigurationId is not { } id || id == Guid.Empty)
            {
                return Array.Empty<ConfigurationSearchEntry>();
            }

            var gameConfiguration = await this._configDataSource.GetOwnerAsync(id, cancellationToken).ConfigureAwait(false);
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