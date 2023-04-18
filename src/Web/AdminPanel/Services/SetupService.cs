// <copyright file="SetupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Service which allows to set the servers database up.
/// </summary>
public class SetupService
{
    private readonly IMigratableDatabaseContextProvider _contextProvider;

    private readonly PlugInManager _plugInManager;

    private ICollection<IDataInitializationPlugIn>? _availableInitializationPlugIns;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetupService"/> class.
    /// </summary>
    /// <param name="contextProvider">The context provider.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    public SetupService(IMigratableDatabaseContextProvider contextProvider, PlugInManager plugInManager)
    {
        this._contextProvider = contextProvider;
        this._plugInManager = plugInManager;
    }

    /// <summary>
    /// Occurs when the database got initialized.
    /// </summary>
    public event AsyncEventHandler? DatabaseInitialized;

    /// <summary>
    /// Gets a value indicating whether this application can connect to database.
    /// </summary>
    public bool CanConnectToDatabase
    {
        get
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            return this._contextProvider.CanConnectToDatabaseAsync(cts.Token).WaitAndUnwrapException();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the data is installed.
    /// </summary>
    public bool IsInstalled
    {
        get
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            return this._contextProvider.DatabaseExistsAsync(cts.Token).WaitAndUnwrapException();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the database requires an update.
    /// </summary>
    public bool IsUpdateRequired
    {
        get
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            return !this._contextProvider.IsDatabaseUpToDateAsync(cts.Token).WaitAndUnwrapException();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the data is initialized.
    /// </summary>
    public async ValueTask<bool> IsDataInitializedAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            using var context = this._contextProvider.CreateNewConfigurationContext();
            var id = await context.GetDefaultGameConfigurationIdAsync(cts.Token).ConfigureAwait(false);
            return id is not null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the current game client definition.
    /// </summary>
    public async ValueTask<ClientVersion?> GetCurrentGameClientVersionAsync()
    {
        using var context = this._contextProvider.CreateNewConfigurationContext();
        var definition = (await context.GetAsync<GameClientDefinition>().ConfigureAwait(false)).FirstOrDefault();
        return definition is { } ? new ClientVersion(definition.Season, definition.Episode, definition.Language) : null;
    }

    /// <summary>
    /// Gets the versions.
    /// </summary>
    public ICollection<IDataInitializationPlugIn> Versions => this._availableInitializationPlugIns
        ??= (this._plugInManager.GetStrategyProvider<string, IDataInitializationPlugIn>() ?? throw new InvalidOperationException("No data initialization plugins were found."))
            .AvailableStrategies
            .OrderByDescending(s => s.Caption)
            .ToList();

    /// <summary>
    /// Installs the updates asynchronous.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task InstallUpdatesAsync(CancellationToken cancellationToken)
    {
        await this._contextProvider.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);
        await this._contextProvider.WaitForUpdatedDatabaseAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the database.
    /// </summary>
    /// <param name="dataInitialization">The data initialization action.</param>
    public async Task CreateDatabaseAsync(Func<Task> dataInitialization)
    {
        using var update = await this._contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
        await dataInitialization().ConfigureAwait(false);
        if (this.DatabaseInitialized is { } eventHandler)
        {
            await eventHandler.Invoke().ConfigureAwait(false);
        }
    }
}