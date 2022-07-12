// <copyright file="EditBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Globalization;
using System.Reflection;
using System.Threading;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Abstract common base class for an edit page.
/// </summary>
public abstract class EditBase : ComponentBase, IAsyncDisposable
{
    private static readonly IDictionary<Type, IList<(string Caption, string Path)>> EditorPages =
        new Dictionary<Type, IList<(string, string)>>
        {
            { typeof(GameMapDefinition), new List<(string, string)> { ("Map Editor", "/map-editor/{0}") } },
        };

    private object? _model;
    private Type? _type;
    private IContext? _persistenceContext;
    private CancellationTokenSource? _disposeCts;
    private DataLoadingState _loadingState;
    private Task? _loadTask;
    private IDisposable? _modalDisposable;

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
    public Guid Id
    {
        get;
        set;
    }

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
    /// Gets the model which should be edited.
    /// </summary>
    protected object? Model => this._model;

    /// <summary>
    /// Gets the type.
    /// </summary>
    protected virtual Type? Type => this._type ??= this.DetermineTypeByTypeString();

    [Inject]
    private ILogger<Edit>? Logger { get; set; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();
        this._disposeCts = null;
        await (this._loadTask ?? Task.CompletedTask).ConfigureAwait(false);
        if (this._persistenceContext is IDisposable disposable)
        {
            disposable.Dispose();
            this._persistenceContext = null;
        }
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Model is { })
        {
            var downloadMarkup = this.GetDownloadMarkup();
            var editorsMarkup = this.GetEditorsMarkup();
            builder.AddMarkupContent(0, $"<h1>Edit {this.Type!.Name}</h1>{downloadMarkup}{editorsMarkup}\r\n");
            builder.OpenComponent<CascadingValue<IContext>>(1);
            builder.AddAttribute(2, nameof(CascadingValue<IContext>.Value), this._persistenceContext);
            builder.AddAttribute(3, nameof(CascadingValue<IContext>.IsFixed), true);
            builder.AddAttribute(4, nameof(CascadingValue<IContext>.ChildContent), (RenderFragment)(builder2 =>
            {
                var sequence = 4;
                this.AddFormToRenderTree(builder2, ref sequence);
            }));

            builder.CloseComponent();
        }
    }

    /// <summary>
    /// Adds the form to the render tree.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="currentSequence">The current sequence.</param>
    protected abstract void AddFormToRenderTree(RenderTreeBuilder builder, ref int currentSequence);

    /// <inheritdoc />
    protected override Task OnParametersSetAsync()
    {
        this._disposeCts?.Cancel();
        this._disposeCts?.Dispose();

        this._model = null;
        this._loadingState = DataLoadingState.LoadingStarted;
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (this._loadingState == DataLoadingState.Loaded && this._modalDisposable is { } modal)
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
            });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    protected Task SaveChanges()
    {
        string text;
        try
        {
            text = this._persistenceContext?.SaveChanges() ?? false ? "The changes have been saved." : "There were no changes to save.";
        }
        catch (Exception ex)
        {
            this.Logger?.LogError(ex, $"Error during saving {this.Id}");
            text = $"An unexpected error occured: {ex.Message}.";
        }

        return this.ModalService.ShowMessageAsync("Save", text);
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

    private string? GetEditorsMarkup()
    {
        StringBuilder? stringBuilder = null;
        if (this.Type is not null
            && (EditorPages.TryGetValue(this.Type, out var editors)
                || this.Type.BaseType is { } baseType && EditorPages.TryGetValue(baseType, out editors)))
        {
            foreach (var editor in editors)
            {
                var uri = string.Format(CultureInfo.InvariantCulture, editor.Path, this.Id);
                stringBuilder ??= new StringBuilder();
                stringBuilder.Append($@"<p><a href=""{uri}"">{editor.Caption}</a></p>");
            }
        }

        return stringBuilder?.ToString();
    }

    private Type? DetermineTypeByTypeString()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName?.StartsWith(nameof(MUnique)) ?? false)
            .Select(assembly => assembly.GetType(this.TypeString)).FirstOrDefault(t => t != null);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        if (this.Type is null)
        {
            throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
        }

        var createContextMethod = typeof(IPersistenceContextProvider).GetMethod(nameof(IPersistenceContextProvider.CreateNewTypedContext))!.MakeGenericMethod(this.Type);
        this._persistenceContext = (IContext)createContextMethod.Invoke(this.PersistenceContextProvider, Array.Empty<object>())!;

        var method = typeof(IContext).GetMethod(nameof(IContext.GetByIdAsync))!.MakeGenericMethod(this.Type);
        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this._model = method.Invoke(this._persistenceContext, new object[] { this.Id });
                    this._loadingState = this.Model is not null
                        ? DataLoadingState.Loaded
                        : DataLoadingState.NotFound;
                }
                catch (Exception ex)
                {
                    this._loadingState = DataLoadingState.Error;
                    this.Logger?.LogError(ex, $"Could not load {this.Type.FullName} with {this.Id}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    await this.InvokeAsync(() => this.ModalService.ShowMessageAsync("Error", "Could not load the data. Check the logs for details."));
                }

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
}