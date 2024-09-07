// <copyright file="EditConfigGrid.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Razor page which shows objects of the specified type in a grid.
/// </summary>
public partial class EditConfigGrid : ComponentBase, IAsyncDisposable
{
    private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };

    private Task? _loadTask;
    private CancellationTokenSource? _disposeCts;

    private List<ViewModel>? _viewModels;

    /// <summary>
    /// Gets or sets the <see cref="Type.FullName"/> of the object which should be edited.
    /// </summary>
    [Parameter]
    public string TypeString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Inject]
    public IDataSource<GameConfiguration> DataSource { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    private Type? Type { get; set; }

    private IQueryable<ViewModel>? ViewModels
    {
        get
        {
            if (string.IsNullOrWhiteSpace(this.NameFilter))
            {
                return this._viewModels?.AsQueryable();
            }

            return this._viewModels?
                .Where(vm => vm.Name.Contains(this.NameFilter.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .AsQueryable();
        }
    }

    private string? NameFilter { get; set; }

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
        this.NameFilter = string.Empty;
        this.Type = this.DetermineTypeByTypeString();
        var cts = new CancellationTokenSource();
        this._disposeCts = cts;
        this._loadTask = Task.Run(() => this.LoadDataAsync(cts.Token), cts.Token);
        await base.OnParametersSetAsync().ConfigureAwait(true);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (this.Type is null)
        {
            throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
        }

        await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
        cancellationToken.ThrowIfCancellationRequested();
        var data = this.DataSource.GetAll(this.Type!);
        this._viewModels = data.OfType<object>()
            .Select(o => new ViewModel(o))
            .OrderBy(o => o.Name)
            .ToList();

        await this.InvokeAsync(async () =>
        {
            await this._pagination.SetCurrentPageIndexAsync(0).ConfigureAwait(true);
            this.StateHasChanged();
        }).ConfigureAwait(false);
    }

    private Type? DetermineTypeByTypeString()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName?.StartsWith(nameof(MUnique)) ?? false)
            .Select(assembly => assembly.GetType(this.TypeString)).FirstOrDefault(t => t != null);
    }

    /// <summary>
    /// The view model for the grid.
    /// We use this instead of the objects, because it makes the code simpler.
    /// Creating generic components is a bit complicated when you don't
    /// have the type as generic type parameter.
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ViewModel(object parent)
        {
            this.Parent = parent;
            this.Id = parent.GetId();
            this.Name = parent.GetName();
        }

        /// <summary>
        /// Gets the parent object, which is displayed.
        /// </summary>
        [Browsable(false)]
        public object Parent { get; }

        /// <summary>
        /// Gets or sets the identifier of the object.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }
    }
}