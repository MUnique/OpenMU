// <copyright file="EditBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Reflection;
using System.Threading;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Abstract common base class for an edit page.
/// </summary>
public abstract class EditBase : ComponentBase, IAsyncDisposable
{
    private object? _model;
    private Type? _type;
    private bool _isOwningContext;
    private IContext? _persistenceContext;
    private CancellationTokenSource? _disposeCts;
    private DataLoadingState _loadingState;
    private Task? _loadTask;
    private IDisposable? _modalDisposable;
    private IDisposable? _navigationLockDisposable;

    private enum DataLoadingState
    {
        NotLoadedYet,

        LoadingStarted,

        Loading,

        Loaded,

        NotFound,

        Error,
    }

    /// <summary>
    /// Gets or sets the identifier of the object which should be edited.
    /// </summary>
    [Parameter]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Type.FullName"/> of the object which should be edited.
    /// </summary>
    [Parameter]
    public string TypeString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the persistence context provider which loads and saves the object.
    /// </summary>
    [Inject]
    public IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the modal service.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the configuration data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> ConfigDataSource { get; set; } = null!;

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

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<EditBase>? Logger { get; set; }

    /// <summary>
    /// Gets the data source of the type which is edited.
    /// </summary>
    protected virtual IDataSource EditDataSource => this.ConfigDataSource;

    /// <summary>
    /// Gets the model which should be edited.
    /// </summary>
    protected object? Model => this._model;

    /// <summary>
    /// Gets the type.
    /// </summary>
    protected virtual Type? Type => this._type ??= this.DetermineTypeByTypeString();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._navigationLockDisposable?.Dispose();
        this._navigationLockDisposable = null;

        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        this._disposeCts = null;

        await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        await this.EditDataSource.DiscardChangesAsync().ConfigureAwait(true);

        if (this._isOwningContext)
        {
            this._persistenceContext?.Dispose();
        }

        this._persistenceContext = null;
    }

    /// <inheritdoc />
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        this._model = null;
        await (this._disposeCts?.CancelAsync() ?? Task.CompletedTask).ConfigureAwait(false);
        this._disposeCts?.Dispose();
        await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(true);
        await base.SetParametersAsync(parameters).ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        this._loadingState = DataLoadingState.LoadingStarted;
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._type = null;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);

        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Model is null)
        {
            return;
        }

        var downloadMarkup = this.GetDownloadMarkup();
        var editorsMarkup = this.GetEditorsMarkup();
        builder.AddMarkupContent(0, $"<h1>Edit {CaptionHelper.GetTypeCaption(this.Type!)}</h1>{downloadMarkup}{editorsMarkup}\r\n");
        builder.OpenComponent<CascadingValue<IContext>>(1);
        builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this._persistenceContext);
        builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), this._isOwningContext);
        builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
        {
            var sequence = 4;
            this.AddFormToRenderTree(builder2, ref sequence);
        }));

        builder.CloseComponent();
    }

    /// <inheritdoc />
    protected override Task OnInitializedAsync()
    {
        this._navigationLockDisposable = this.NavigationManager.RegisterLocationChangingHandler(this.OnBeforeInternalNavigationAsync);
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Adds the form to the render tree.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="currentSequence">The current sequence.</param>
    protected abstract void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence);
    
    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this._loadingState is not DataLoadingState.Loading && this._modalDisposable is { } modal)
        {
            modal.Dispose();
            this._modalDisposable = null;
        }

        if (this._loadingState == DataLoadingState.LoadingStarted)
        {
            this._loadingState = DataLoadingState.Loading;

            await this.InvokeAsync(() =>
            {
                if (this._loadingState != DataLoadingState.Loaded)
                {
                    this._modalDisposable = this.ModalService.ShowLoadingIndicator();
                    this.StateHasChanged();
                }
            }).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
    }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    protected async Task SaveChangesAsync()
    {
        string text;
        try
        {
            if (this._persistenceContext is { } context)
            {
                var success = await context.SaveChangesAsync().ConfigureAwait(true);
                text = success ? "The changes have been saved." : "There were no changes to save.";
            }
            else
            {
                text = "Failed, context not initialized";
            }
        }
        catch (Exception ex)
        {
            this.Logger?.LogError(ex, $"Error during saving {this.Id}");
            text = $"An unexpected error occured: {ex.Message}.";
        }

        await this.ModalService.ShowMessageAsync("Save", text).ConfigureAwait(true);
    }

    /// <summary>
    /// Gets the optional editors markup for the current type.
    /// </summary>
    /// <returns>The optional editors markup for the current type.</returns>
    protected virtual string? GetEditorsMarkup()
    {
        return null;
    }

    /// <summary>
    /// It loads the owner of the <see cref="EditDataSource" />.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected virtual async ValueTask LoadOwnerAsync(CancellationToken cancellationToken)
    {
        await this.EditDataSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(true);
    }

    private async ValueTask OnBeforeInternalNavigationAsync(LocationChangingContext context)
    {
        if (this._persistenceContext?.HasChanges is true)
        {
            var isConfirmed = await this.JavaScript.InvokeAsync<bool>("window.confirm",
                    "There are unsaved changes. Are you sure you want to discard them?")
                .ConfigureAwait(true);

            if (!isConfirmed)
            {
                context.PreventNavigation();
            }
            else if (this._isOwningContext)
            {
                this._persistenceContext.Dispose();
                this._persistenceContext = null;
            }
            else
            {
                await this.EditDataSource.DiscardChangesAsync().ConfigureAwait(true);
            }
        }
    }

    private string? GetDownloadMarkup()
    {
        if (this.Type is not null && GenericControllerFeatureProvider.SupportedTypes.Any(t => t.Item1 == this.Type))
        {
            var uri = $"/download/{this.Type.Name}/{this.Type.Name}_{this.Id}.json";
            return $"<p>Download as json: <a href=\"{uri}\" download><span class=\"oi oi-data-transfer-download\"></span></a></p>";
        }

        return null;
    }

    private Type? DetermineTypeByTypeString()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName?.StartsWith(nameof(MUnique)) ?? false)
            .Select(assembly => assembly.GetType(this.TypeString)).FirstOrDefault(t => t != null);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (this.Type is null)
            {
                throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
            }

            await this.LoadOwnerAsync(cancellationToken).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();
            if (this.EditDataSource.IsSupporting(this.Type))
            {
                this._isOwningContext = false;
                this._persistenceContext = await this.EditDataSource.GetContextAsync(cancellationToken).ConfigureAwait(true);
            }
            else
            {
                this._isOwningContext = true;
                var gameConfiguration = await this.ConfigDataSource.GetOwnerAsync(Guid.Empty, cancellationToken).ConfigureAwait(true);
                var createContextMethod = typeof(IPersistenceContextProvider).GetMethod(nameof(IPersistenceContextProvider.CreateNewTypedContext))!.MakeGenericMethod(this.Type);
                this._persistenceContext = (IContext)createContextMethod.Invoke(this.PersistenceContextProvider, new object[] { true, gameConfiguration})!;
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (this.EditDataSource.IsSupporting(this.Type))
                {
                    this._model = this.Id == default
                        ? this.EditDataSource.GetAll(this.Type).OfType<object>().FirstOrDefault()
                        : this.EditDataSource.Get(this.Id);
                }
                else
                {
                    this._model = this.Id == default
                        ? (await this._persistenceContext.GetAsync(this.Type, cancellationToken).ConfigureAwait(true)).OfType<object>().FirstOrDefault()
                        : await this._persistenceContext.GetByIdAsync(this.Id, this.Type, cancellationToken).ConfigureAwait(true);
                }

                this._loadingState = this.Model is not null
                    ? DataLoadingState.Loaded
                    : DataLoadingState.NotFound;
            }
            catch (Exception ex)
            {
                this._loadingState = DataLoadingState.Error;
                this.Logger?.LogError(ex, $"Could not load {this.Type.FullName} with {this.Id}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                await this.InvokeAsync(() => this.ModalService.ShowMessageAsync("Error", "Could not load the data. Check the logs for details.")).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // expected when the page is getting disposed.
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
        catch (Exception ex)
        {
            this.Logger?.LogError(ex, "Unexpected error when loading data: {ex}", ex);
        }
    }
}