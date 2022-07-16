// <copyright file="SetupService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.PlugIns;

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
    public bool CanConnectToDatabase => this._contextProvider.CanConnectToDatabaseAsync().WaitAndUnwrapException();

    /// <summary>
    /// Gets a value indicating whether the data is installed.
    /// </summary>
    public bool IsInstalled => this._contextProvider.DatabaseExistsAsync().WaitAndUnwrapException();

    /// <summary>
    /// Gets a value indicating whether the database requires an update.
    /// </summary>
    public bool IsUpdateRequired => !this._contextProvider.IsDatabaseUpToDateAsync().WaitAndUnwrapException();

    /// <summary>
    /// Gets a value indicating whether the data is initialized.
    /// </summary>
    public bool IsDataInitialized => this.CurrentGameClientVersion is not null;

    /// <summary>
    /// Gets the current game client definition.
    /// </summary>
    public ClientVersion? CurrentGameClientVersion
    {
        get
        {
            using var context = this._contextProvider.CreateNewTypedContext<GameClientDefinition>();
            var definition = context.GetAsync<GameClientDefinition>().AsTask().WaitAndUnwrapException().FirstOrDefault();
            return definition is { } ? new ClientVersion(definition.Season, definition.Episode, definition.Language) : null;
        }
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
        await this._contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
        await dataInitialization().ConfigureAwait(false);
        if (this.DatabaseInitialized is { } eventHandler)
        {
            await eventHandler.Invoke().ConfigureAwait(false);
        }
    }
}