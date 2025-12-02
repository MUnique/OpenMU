// <copyright file="CreateGameServerConfig.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Razor page which shows objects of the specified type in a grid.
/// </summary>
public partial class CreateConnectServerConfig : ComponentBase, IAsyncDisposable
{
    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;

    private ConnectServerViewModel? _viewModel;
    private string? _initState;

    /// <summary>
    /// Gets or sets the context provider.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider ContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the server initializer.
    /// </summary>
    [Inject]
    public IConnectServerInstanceManager ServerInstanceManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        try
        {
            await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // we can ignore that ...
        }
        catch
        {
            // and we should not throw exceptions in the dispose method ...
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var gameConfiguration = await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
        using var persistenceContext = this.ContextProvider.CreateNewContext(gameConfiguration);

        var clients = (await persistenceContext.GetAsync<GameClientDefinition>(cancellationToken).ConfigureAwait(false)).ToList();
        var existingServerDefinitions = (await persistenceContext.GetAsync<ConnectServerDefinition>(cancellationToken).ConfigureAwait(false)).ToList();

        var nextServerId = 0;
        var networkPort = 55901;
        if (existingServerDefinitions.Count > 0)
        {
            nextServerId = existingServerDefinitions.Max(s => s.ServerId) + 1;
            networkPort = existingServerDefinitions.Max(s => s.ClientListenerPort) + 1;
        }

        var unusedClient = clients.FirstOrDefault(c => !existingServerDefinitions.Any(s => object.Equals(s.Client, c)));

        this._viewModel = new ConnectServerViewModel
        {
            ServerId = (byte)nextServerId,
            NetworkPort = networkPort,
            Client = unusedClient ?? clients.FirstOrDefault(),
        };

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async ValueTask<ConnectServerDefinition> CreateDefinitionByViewModelAsync(IContext context)
    {
        if (this._viewModel is null)
        {
            throw new InvalidOperationException("View model not initialized.");
        }

        var result = context.CreateNew<ConnectServerDefinition>();
        result.InitializeDefaults();
        result.ServerId = this._viewModel.ServerId;
        result.Description = this._viewModel.Description;
        result.Client = this._viewModel.Client!;
        result.ClientListenerPort = _viewModel.NetworkPort;
        return result;
    }

    private async Task OnSaveButtonClickAsync()
    {
        try
        {
            var gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(false);

            using var saveContext = this.ContextProvider.CreateNewTypedContext(typeof(DataModel.Configuration.ConnectServerDefinition), true, gameConfiguration);

            var existingServerDefinitions = (await saveContext.GetAsync<ConnectServerDefinition>().ConfigureAwait(false)).ToList();
            if (existingServerDefinitions.Any(def => def.ServerId == this._viewModel?.ServerId))
            {
                this.ToastService.ShowError($"Server with Id {this._viewModel?.ServerId} already exists. Please use another value.");
                return;
            }

            if (existingServerDefinitions.Any(def => def.ClientListenerPort == this._viewModel?.NetworkPort))
            {
                this.ToastService.ShowError($"A server with tcp port {this._viewModel?.NetworkPort} already exists. Please use another tcp port.");
                return;
            }

            this._initState = "Creating Configuration ...";
            await this.InvokeAsync(this.StateHasChanged);
            var connectServerDefinition = await this.CreateDefinitionByViewModelAsync(saveContext).ConfigureAwait(false);
            this._initState = "Saving Configuration ...";
            await this.InvokeAsync(this.StateHasChanged);
            var success = await saveContext.SaveChangesAsync().ConfigureAwait(true);

            // if success, init new game server instance
            if (success)
            {
                this.ToastService.ShowSuccess("The connection server configuration has been saved. Initializing connect server ...");
                this._initState = "Initializing Connect Server ...";
                await this.InvokeAsync(this.StateHasChanged);
                await this.ServerInstanceManager.InitializeConnectServerAsync(connectServerDefinition.ConfigurationId);
                this.NavigationManager.NavigateTo("servers");
                return;
            }

            this.ToastService.ShowError("No changes have been saved.");
        }
        catch (Exception ex)
        {
            this.ToastService.ShowError($"An unexpected error occurred: {ex.Message}.");
        }

        this._initState = null;
    }

    /// <summary>
    /// The view model for a <see cref="ConnectServerDefinition"/>.
    /// </summary>
    public class ConnectServerViewModel
    {
        /// <summary>
        /// Gets or sets the server identifier.
        /// </summary>
        public byte ServerId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client which is expected to connect.
        /// </summary>
        [Required]
        public GameClientDefinition? Client { get; set; }

        /// <summary>
        /// Gets or sets the network port on which the server is listening.
        /// </summary>
        [Range(1, ushort.MaxValue)]
        public int NetworkPort { get; set; }
    }
}