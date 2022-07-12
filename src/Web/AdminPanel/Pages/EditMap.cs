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
/// A page, which shows an <see cref="MapEditor"/> for the given <see cref="Id"/> of a <see cref="GameMapDefinition"/>.
/// </summary>
[Route("/map-editor/{id:guid}")]
public sealed class EditMap : ComponentBase, IDisposable
{
    private GameMapDefinition? _model;
    private IContext? _persistenceContext;
    private CancellationTokenSource? _disposeCts;

    /// <summary>
    /// Gets or sets the identifier of the object which should be edited.
    /// </summary>
    [Parameter]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the persistence context provider which loads and saves the object.
    /// </summary>
    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    private IModalService ModalService { get; set; } = null!;

    [Inject]
    private ILogger<EditMap> Logger { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;
        if (this._persistenceContext is IDisposable disposable)
        {
            disposable.Dispose();
            this._persistenceContext = null;
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this._model is { })
        {
            builder.OpenComponent<CascadingValue<IContext>>(1);
            builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this._persistenceContext);
            builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), true);
            builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
            {
                builder2.OpenComponent(5, typeof(MapEditor));
                builder2.AddAttribute(6, nameof(MapEditor.Map), this._model);
                builder2.AddAttribute(7, nameof(MapEditor.OnValidSubmit), EventCallback.Factory.Create(this, this.SaveChangesAsync));
                builder2.CloseComponent();
            }));

            builder.CloseComponent();
        }
    }

    /// <inheritdoc />
    protected override Task OnParametersSetAsync()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        this._model = null;

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (this._model is null)
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

        this._persistenceContext = this.PersistenceContextProvider.CreateNewTypedContext<GameMapDefinition>();
        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this._model = await this._persistenceContext.GetByIdAsync<GameMapDefinition>(this.Id);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, $"Could not load game map with {this.Id}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    await this.ModalService.ShowMessageAsync("Error", "Could not load the map data. Check the logs for details.");
                }

                await showModalTask;
                modal?.Dispose();
                await this.InvokeAsync(this.StateHasChanged);
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

    private Task SaveChangesAsync()
    {
        string text;
        try
        {
            text = this._persistenceContext?.SaveChanges() ?? false ? "The changes have been saved." : "There were no changes to save.";
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error during saving {this.Id}");
            text = $"An unexpected error occured: {ex.Message}.";
        }

        return this.ModalService.ShowMessageAsync("Save", text);
    }
}