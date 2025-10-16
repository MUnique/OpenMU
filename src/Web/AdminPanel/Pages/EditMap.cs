﻿// <copyright file="EditMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Reflection;
using System.Threading;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Components;
using MUnique.OpenMU.Web.AdminPanel.Shared;

/// <summary>
/// A page, which shows an <see cref="MapEditor"/> for all <see cref="GameConfiguration.Maps"/>.
/// </summary>
[Route("/map-editor")]
[Route("/map-editor/{SelectedMapId:guid}")]
public sealed class EditMap : ComponentBase, IDisposable
{
    private List<GameMapDefinition>? _maps;
    private CancellationTokenSource? _disposeCts;
    private IContext? _context;
    private IDisposable? _navigationLockDisposable;

    /// <summary>
    /// Gets or sets the selected map identifier.
    /// </summary>
    [Parameter]
    public Guid SelectedMapId { get; set; }

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    private IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    private IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the game configuration.
    /// </summary>
    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private ILogger<EditMap> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the java script runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JavaScript { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        this._navigationLockDisposable?.Dispose();
        this._navigationLockDisposable = null;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this._maps is { })
        {
            builder.OpenComponent<Breadcrumb>(0);
            builder.AddAttribute(1, nameof(Breadcrumb.Caption), "Editor de Mapas");
            builder.CloseComponent();
            builder.OpenComponent<CascadingValue<IContext>>(10);
            builder.AddAttribute(12, nameof(CascadingValue<IContext>.Value), this._context);
            builder.AddAttribute(13, nameof(CascadingValue<IContext>.IsFixed), false);
            builder.AddAttribute(14, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
            {
                builder2.OpenComponent(15, typeof(MapEditor));
                builder2.AddAttribute(16, nameof(MapEditor.Maps), this._maps);
                builder2.AddAttribute(17, nameof(MapEditor.SelectedMapId), this.SelectedMapId);
                builder2.AddAttribute(18, nameof(MapEditor.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
                builder2.AddAttribute(19, nameof(MapEditor.SelectedMapChanging), EventCallback.Factory.Create<MapEditor.MapChangingArgs>(this, this.OnSelectedMapChanging));
                builder2.CloseComponent();
            }));

            builder.CloseComponent();
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = new CancellationTokenSource();

        this._context = await this.GameConfigurationSource.GetContextAsync(this._disposeCts.Token).ConfigureAwait(false);

        await base.OnParametersSetAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
        if (this._maps is null)
        {
            this._disposeCts ??= new CancellationTokenSource();
            var cts = this._disposeCts.Token;
            _ = Task.Run(() => this.LoadDataAsync(cts), cts);
        }
    }

    /// <inheritdoc />
    protected override Task OnInitializedAsync()
    {
        this._navigationLockDisposable = this.NavigationManager.RegisterLocationChangingHandler(this.OnBeforeInternalNavigation);
        return base.OnInitializedAsync();
    }

    private async ValueTask OnBeforeInternalNavigation(LocationChangingContext context)
    {
        if (!await this.AllowChangeAsync().ConfigureAwait(false))
        {
            context.PreventNavigation();
        }
    }

    private async Task OnSelectedMapChanging(MapEditor.MapChangingArgs eventArgs)
    {
        eventArgs.Cancel = !await this.AllowChangeAsync().ConfigureAwait(true);
        if (!eventArgs.Cancel)
        {
            this.SelectedMapId = eventArgs.NextMap;
        }
    }

    private async ValueTask<bool> AllowChangeAsync()
    {
        var cancellationToken = this._disposeCts?.Token ?? default;
        var persistenceContext = await this.GameConfigurationSource.GetContextAsync(cancellationToken).ConfigureAwait(true);
        if (persistenceContext?.HasChanges is not true)
        {
            return true;
        }

        var isConfirmed = await this.JavaScript.InvokeAsync<bool>(
                "window.confirm",
                cancellationToken,
                "There are unsaved changes. Are you sure you want to discard them?")
            .ConfigureAwait(true);

        if (!isConfirmed)
        {
            return false;
        }

        await this.GameConfigurationSource.DiscardChangesAsync().ConfigureAwait(true);
        this._maps = null;

        // OnAfterRender will load the maps again ...
        return true;
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        IDisposable? modal = null;
        var showModalTask = this.InvokeAsync(() => modal = this.ModalService.ShowLoadingIndicator());

        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var gameConfig = await this.GameConfigurationSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);
                try
                {
                    this._maps = gameConfig.Maps.OrderBy(c => c.Number).ToList();
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"No se pudieron cargar los mapas: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    await this.ModalService.ShowMessageAsync("Error", "No se pudieron cargar los datos del mapa. Revisa los logs para más detalles.").ConfigureAwait(false);
                }

                await showModalTask.ConfigureAwait(false);
                modal?.Dispose();
                await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
            }
        }
        catch (TargetInvocationException ex) when (ex.InnerException is ObjectDisposedException)
        {
            // See ObjectDisposedException.
        }
        catch (ObjectDisposedException)
        {
            // Happens when the user navigated away (shouldn't happen with the modal loading indicator, but we check it anyway).
            // It would be great to have an async api with cancellation token support in the persistence layer
            // For the moment, we swallow the exception
        }
    }

    private async Task SaveChangesAsync()
    {
        try
        {
            var context = await this.GameConfigurationSource.GetContextAsync().ConfigureAwait(true);
            var success = await context.SaveChangesAsync().ConfigureAwait(true);
            var text = success ? "Los cambios se han guardado." : "No hay cambios para guardar.";
            this.ToastService.ShowSuccess(text);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error al guardar");
            this.ToastService.ShowError($"Ocurrió un error inesperado: {ex.Message}. Mira los logs para más detalles.");
        }
    }
}
