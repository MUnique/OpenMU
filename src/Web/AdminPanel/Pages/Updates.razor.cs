// <copyright file="Updates.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The set up page.
/// </summary>
public partial class Updates
{
    private enum UpdateState
    {
        NotStarted,

        Started,

        Installed,

        Failed,
    }

    private bool _isDataInitialized;

    private Exception? _exception;

    private UpdateState _overallState;

    private List<UpdateViewModel> _availableUpdates = new();

    /// <summary>
    /// Gets or sets the setup service.
    /// </summary>
    [Inject]
    public SetupService SetupService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the update manager.
    /// </summary>
    [Inject]
    public DataUpdateService UpdateService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the javascript runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await this.DetermineUpdatesAsync().ConfigureAwait(true);
    }

    private async Task DetermineUpdatesAsync()
    {
        this._isDataInitialized = await this.SetupService.IsDataInitializedAsync().ConfigureAwait(false);
        if (this._isDataInitialized)
        {
            var updates = await this.UpdateService.DetermineAvailableUpdatesAsync().ConfigureAwait(false);
            this._availableUpdates = updates.Select(up => new UpdateViewModel(up)).ToList();
        }
    }

    private async Task OnUpdateClickAsync()
    {
        this._exception = null;
        this._overallState = UpdateState.Started;
        this.StateHasChanged();
        var selectedUpdates = this._availableUpdates.Where(up => up.Selected).Select(up => up.UpdatePlugIn).ToList();
        var progress = new Progress<(UpdateVersion CurrentUpdateVersion, bool IsCompleted)>();
        progress.ProgressChanged += this.OnUpdateProgressChanged;
        var currentUpdateVersion = UpdateVersion.Undefined;
        progress.ProgressChanged += (_, args) => currentUpdateVersion = args.CurrentUpdateVersion;

        try
        {
            await this.UpdateService.ApplyUpdatesAsync(selectedUpdates, progress).ConfigureAwait(true);
            await this.DetermineUpdatesAsync().ConfigureAwait(true);
            this._overallState = UpdateState.Installed;
        }
        catch (Exception ex)
        {
            this._exception = ex;
            this._overallState = UpdateState.Failed;
            if (this._availableUpdates.FirstOrDefault(up => up.Version == currentUpdateVersion) is { } failedUpdate)
            {
                failedUpdate.State = UpdateState.Failed;
            }
        }
        finally
        {
            this.StateHasChanged();
        }
    }

    private void OnUpdateProgressChanged(object? sender, (UpdateVersion CurrentUpdateVersion, bool IsCompleted) e)
    {
        if (this._availableUpdates.FirstOrDefault(up => up.Version == e.CurrentUpdateVersion) is not { } updateViewModel)
        {
            return;
        }

        updateViewModel.State = e.IsCompleted ? UpdateState.Installed : UpdateState.Started;
        this.StateHasChanged();
    }

    /// <summary>
    /// The view model for an update.
    /// </summary>
    private class UpdateViewModel
    {
        private readonly IConfigurationUpdatePlugIn _updatePlugIn;
        private bool _selected;
        private UpdateState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateViewModel"/> class.
        /// </summary>
        /// <param name="updatePlugIn">The update plug in.</param>
        public UpdateViewModel(IConfigurationUpdatePlugIn updatePlugIn)
        {
            this._updatePlugIn = updatePlugIn;
            this.Selected = true;
        }

        public EventCallback<bool> SelectedChanged { get; set; }

        public EventCallback<UpdateState> StateChanged { get; set; }

        public bool Selected
        {
            get => this._selected;
            set
            {
                if (this._selected == value)
                {
                    return;
                }

                this._selected = value;
                if (this.SelectedChanged.HasDelegate)
                {
                    _ = this.SelectedChanged.InvokeAsync(value);
                }
            }
        }

        public UpdateState State
        {
            get => this._state;
            set
            {
                if (this.State == value)
                {
                    return;
                }

                this._state = value;
                if (this.StateChanged.HasDelegate)
                {
                    _ = this.StateChanged.InvokeAsync(value);
                }
            }
        }

        public string Name => this._updatePlugIn.Name;

        public UpdateVersion Version => this._updatePlugIn.Version;

        public string Description => this._updatePlugIn.Description;

        public bool IsMandatory => this._updatePlugIn.IsMandatory;

        public IConfigurationUpdatePlugIn UpdatePlugIn => this._updatePlugIn;
    }
}