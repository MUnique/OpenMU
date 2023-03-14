// <copyright file="EditMap.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Reflection;
using System.Threading;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Components;

/// <summary>
/// A page, which shows an <see cref="MapEditor"/> for all <see cref="GameConfiguration.Maps"/>.
/// </summary>
[Route("/map-editor")]
public sealed class EditMap : ComponentBase, IDisposable
{
    private List<GameMapDefinition>? _maps;
    private CancellationTokenSource? _disposeCts;
    private IContext? _context;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    private IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the game configuration.
    /// </summary>
    [Inject]
    private IDataSource<GameConfiguration> GameConfigurationSource { get; set; } = null!;

    [Inject]
    private ILogger<EditMap> Logger { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this._maps is { })
        {
            builder.OpenComponent<CascadingValue<IContext>>(1);
            builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this._context);
            builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), false);
            builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
            {
                builder2.OpenComponent(5, typeof(MapEditor));
                builder2.AddAttribute(6, nameof(MapEditor.Maps), this._maps);
                builder2.AddAttribute(7, nameof(MapEditor.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
                builder2.CloseComponent();
            }));

            builder.CloseComponent();
        }
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        this._context = await this.GameConfigurationSource.GetContextAsync().ConfigureAwait(false);

        await base.OnParametersSetAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
        if (this._maps is null)
        {
            this._disposeCts = new CancellationTokenSource();
            var cts = this._disposeCts.Token;
            _ = Task.Run(() => this.LoadDataAsync(cts), cts);
        }
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        IDisposable? modal = null;
        var showModalTask = this.InvokeAsync(() => modal = this.ModalService.ShowLoadingIndicator());

        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var gameConfig = await this.GameConfigurationSource.GetOwnerAsync(Guid.Empty);
                try
                {
                    this._maps = gameConfig.Maps.OrderBy(c => c.Number).ToList();
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"Could not load game maps: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    await this.ModalService.ShowMessageAsync("Error", "Could not load the map data. Check the logs for details.").ConfigureAwait(false);
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
        string text;
        try
        {
            var context = await this.GameConfigurationSource.GetContextAsync().ConfigureAwait(true);
            var success = await context.SaveChangesAsync().ConfigureAwait(true);
            text = success ? "The changes have been saved." : "There were no changes to save.";
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error during saving");
            text = $"An unexpected error occured: {ex.Message}.";
        }

        await this.ModalService.ShowMessageAsync("Save", text);
    }
}