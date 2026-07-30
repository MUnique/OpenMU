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
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Razor page that shows objects of the specified type in a grid.
/// </summary>
public partial class CreateGameServerConfig : ComponentBase, IAsyncDisposable
{
    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;

    private GameServerViewModel? _viewModel;
    private bool _isProcessing;

    /// <summary>
    /// Gets or sets the context provider.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider ContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the server initializer.
    /// </summary>
    [Inject]
    public IGameServerInstanceManager ServerInstanceManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

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

    /// <summary>
    /// Gets or sets the loading overlay service.
    /// </summary>
    [Inject]
    public LoadingOverlayService LoadingService { get; set; } = null!;

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
            // We can ignore that.
        }
        catch
        {
            // And we should not throw exceptions in the dispose method.
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
        using var loading = this.LoadingService.ShowLoadingIndicator();
        cancellationToken.ThrowIfCancellationRequested();

        var gameConfiguration = await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
        using var persistenceContext = this.ContextProvider.CreateNewContext(gameConfiguration);

        var serverConfigs = await persistenceContext.GetAsync<GameServerConfiguration>(cancellationToken).ConfigureAwait(false);
        var clients = await persistenceContext.GetAsync<GameClientDefinition>(cancellationToken).ConfigureAwait(false);
        var existingServerDefinitions = (await persistenceContext.GetAsync<GameServerDefinition>(cancellationToken).ConfigureAwait(false)).ToList();

        var nextServerId = 0;
        var networkPort = 55901;
        if (existingServerDefinitions.Count > 0)
        {
            nextServerId = existingServerDefinitions.Max(s => s.ServerID) + 1;
            networkPort = existingServerDefinitions.Max(s => s.Endpoints.FirstOrDefault()?.NetworkPort ?? 55900) + 1;
        }

        this._viewModel = new GameServerViewModel
        {
            ServerConfiguration = serverConfigs.FirstOrDefault(),
            ServerId = (byte)nextServerId,
            ExperienceRate = 1.0f,
            PvpEnabled = true,
            NetworkPort = networkPort,
            Client = clients.FirstOrDefault(),
        };

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    private async ValueTask<GameServerDefinition> CreateDefinitionByViewModelAsync(IContext context)
    {
        if (this._viewModel is null)
        {
            throw new InvalidOperationException("View model not initialized.");
        }

        var result = context.CreateNew<GameServerDefinition>();
        result.ServerID = this._viewModel.ServerId;
        result.Description = this._viewModel.Description;
        result.PvpEnabled = this._viewModel.PvpEnabled;
        result.ExperienceRate = this._viewModel.ExperienceRate;
        result.GameConfiguration = await this.DataSource.GetOwnerAsync();
        result.ServerConfiguration = this._viewModel.ServerConfiguration!;

        var endpoint = context.CreateNew<GameServerEndpoint>();
        endpoint.NetworkPort = (ushort)this._viewModel.NetworkPort;
        endpoint.Client = this._viewModel.Client!;
        result.Endpoints.Add(endpoint);

        return result;
    }

    private async Task OnSaveButtonClickAsync()
    {
        try
        {
            this._isProcessing = true;
            var gameConfiguration = await this.DataSource.GetOwnerAsync().ConfigureAwait(true);
            using var saveContext = this.ContextProvider.CreateNewTypedContext(typeof(DataModel.Configuration.GameServerDefinition), true, gameConfiguration);

            var existingServerDefinitions = (await saveContext.GetAsync<GameServerDefinition>().ConfigureAwait(true)).ToList();
            if (existingServerDefinitions.Any(def => def.ServerID == this._viewModel?.ServerId))
            {
                this.ToastService.ShowError(string.Format(Resources.ServerWithIdAlreadyExists, this._viewModel?.ServerId));
                return;
            }

            if (existingServerDefinitions.Any(def => def.Endpoints.Any(endpoint => endpoint.NetworkPort == this._viewModel?.NetworkPort)))
            {
                this.ToastService.ShowError(string.Format(Resources.ServerWithPortAlreadyExists, this._viewModel?.NetworkPort));
                return;
            }

            var gameServerDefinition = await this.CreateDefinitionByViewModelAsync(saveContext).ConfigureAwait(true);
            var success = await saveContext.SaveChangesAsync().ConfigureAwait(true);

            if (success)
            {
                this.ToastService.ShowSuccess(Resources.GameServerConfigurationSavedInfo);
                await this.ServerInstanceManager.InitializeGameServerAsync(gameServerDefinition.ServerID).ConfigureAwait(true);
                this.NavigationManager.NavigateTo("servers");
                return;
            }

            this.ToastService.ShowError(Resources.NoChangesSaved);
        }
        catch (Exception ex)
        {
            this.ToastService.ShowError(string.Format(Resources.UnexpectedErrorOccurred, ex.Message));
        }
        finally
        {
            this._isProcessing = false;
        }
    }

    /// <summary>
    /// The view model for a <see cref="GameServerDefinition"/>.
    /// </summary>
    public class GameServerViewModel
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
        /// Gets or sets the experience rate.
        /// </summary>
        /// <value>
        /// The experience rate.
        /// </value>
        [Range(0, float.MaxValue)]
        public float ExperienceRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PVP is enabled on this server.
        /// </summary>
        public bool PvpEnabled { get; set; }

        /// <summary>
        /// Gets or sets the server configuration.
        /// </summary>
        [Required]
        public GameServerConfiguration? ServerConfiguration { get; set; }

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
