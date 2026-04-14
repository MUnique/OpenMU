// <copyright file="EditConfigGrid.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Form.Modal;

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
    /// Gets or sets the toast service.
    /// </summary>
    [Inject]
    public IToastService ToastService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    [Inject]
    public ILogger<EditConfigGrid> Logger { get; set; } = null!;

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

        IEnumerable data;
        var gameConfiguration = await this.DataSource.GetOwnerAsync(default, cancellationToken).ConfigureAwait(true);
        if (this.DataSource.IsSupporting(this.Type))
        {
            cancellationToken.ThrowIfCancellationRequested();
            data = this.DataSource.GetAll(this.Type!);
        }
        else
        {
            using var context = this.PersistenceContextProvider.CreateNewTypedContext(this.Type, true, gameConfiguration);
            data = await context.GetAsync(this.Type, cancellationToken).ConfigureAwait(false);
        }

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

    private async Task OnDeleteButtonClickAsync(ViewModel viewModel)
    {
        try
        {
            var dialogResult = await this.ModalService.ShowQuestionAsync("Are you sure?", $"You're about to delete '{viewModel.Name}. Are you sure?").ConfigureAwait(true);
            if (!dialogResult)
            {
                return;
            }

            var cancellationToken = this._disposeCts?.Token ?? default;
            var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            using var deleteContext = this.PersistenceContextProvider.CreateNewTypedContext(this.Type!, false, gameConfiguration);
            var toDelete = await deleteContext.GetByIdAsync(viewModel.Id, this.Type!, cancellationToken).ConfigureAwait(false);
            if (toDelete is null)
            {
                 this.ToastService.ShowError(string.Format(Resources.CouldNotFindToDelete, viewModel.Name));
                 return;
            }

            await deleteContext.DeleteAsync(toDelete).ConfigureAwait(false);
            await deleteContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await this.DataSource.ForceDiscardChangesAsync().ConfigureAwait(false);
            this.ToastService.ShowSuccess($"Deleted '{viewModel.Name}' successfully.");
            this._viewModels = null;
            this._loadTask = Task.Run(() => this.LoadDataAsync(cancellationToken), cancellationToken);
        }
        catch (Exception ex)
        {
             this.Logger.LogError(ex, "Couldn't delete {viewModelName}, probably because it's referenced by another object.", viewModel.Name);
             this.ToastService.ShowError(Resources.DeleteFailedReferenced);
        }
    }

    private async Task OnCreateButtonClickAsync()
    {
        var cancellationToken = this._disposeCts?.Token ?? default;
        var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        using var creationContext = this.PersistenceContextProvider.CreateNewTypedContext(this.Type!, true, gameConfiguration);
        var newObject = creationContext.CreateNew(this.Type!);
        var parameters = new ModalParameters();
        var modalType = typeof(ModalCreateNew<>).MakeGenericType(this.Type!);

        parameters.Add(nameof(ModalCreateNew<object>.Item), newObject);
        parameters.Add(nameof(ModalCreateNew<object>.PersistenceContext), creationContext);
        var options = new ModalOptions
        {
            DisableBackgroundCancel = true,
        };

        var modal = this.ModalService.Show(modalType, $"Create", parameters, options);
        var result = await modal.Result.ConfigureAwait(false);
        if (!result.Cancelled)
        {
            await creationContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await this.DataSource.ForceDiscardChangesAsync().ConfigureAwait(false);

            this.ToastService.ShowSuccess("New object successfully created.");
            this._viewModels = null;
            this._loadTask = Task.Run(() => this.LoadDataAsync(cancellationToken), cancellationToken);
        }
    }

    private async Task OnDuplicateButtonClickAsync(ViewModel viewModel)
    {
        try
        {
            var cancellationToken = this._disposeCts?.Token ?? default;
            var gameConfiguration = await this.DataSource.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            using var context = this.PersistenceContextProvider.CreateNewTypedContext(this.Type!, true, gameConfiguration);
            var original = await context.GetByIdAsync(viewModel.Id, this.Type!, cancellationToken).ConfigureAwait(false);
            if (original is null)
            {
                this.ToastService.ShowError(string.Format(Resources.CouldNotFindToDuplicate, viewModel.Name));
                return;
            }

            var newObject = await this.DuplicateObjectAsync(original, gameConfiguration, context, viewModel, cancellationToken).ConfigureAwait(false);
            if (newObject is null)
            {
                return;
            }

            var parameters = new ModalParameters();
            var modalType = typeof(ModalCreateNew<>).MakeGenericType(this.Type!);

            parameters.Add(nameof(ModalCreateNew<object>.Item), newObject);
            parameters.Add(nameof(ModalCreateNew<object>.PersistenceContext), context);
            var options = new ModalOptions
            {
                DisableBackgroundCancel = true,
            };

            var modal = this.ModalService.Show(modalType, $"Duplicate '{viewModel.Name}'", parameters, options);
            var result = await modal.Result.ConfigureAwait(false);
            if (!result.Cancelled)
            {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await this.DataSource.ForceDiscardChangesAsync().ConfigureAwait(false);

                this.ToastService.ShowSuccess(string.Format(Resources.DuplicatedSuccessfully, viewModel.Name));
                this._viewModels = null;
                this._loadTask = Task.Run(() => this.LoadDataAsync(cancellationToken), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Error duplicating {viewModelName}.", viewModel.Name);
            this.ToastService.ShowError(string.Format(Resources.ErrorDuplicating, viewModel.Name, ex.Message));
        }
    }

    private async Task<object?> DuplicateObjectAsync(object original, GameConfiguration gameConfiguration, IContext context, ViewModel viewModel, CancellationToken cancellationToken)
    {
        var cloned = this.CreateClone(original, gameConfiguration, viewModel);
        if (cloned is null)
        {
            return null;
        }

        var newObject = context.CreateNew(this.Type!);

        var properties = this.Type!.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.Name != "Id");

        foreach (var prop in properties)
        {
            try
            {
                await this.CopyPropertyAsync(prop, cloned, newObject, context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.Logger.LogDebug(ex, "Skipping property {propName} which can't be copied.", prop.Name);
            }
        }

        return newObject;
    }

    private object? CreateClone(object original, GameConfiguration gameConfiguration, ViewModel viewModel)
    {
        var cloneMethods = this.Type!.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.Name == "Clone").ToList();
        if (cloneMethods.Count == 0)
        {
            this.ToastService.ShowError(string.Format(Resources.TypeDoesNotSupportCloning, this.Type.Name));
            return null;
        }

        var cloneMethod = cloneMethods.FirstOrDefault(m =>
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType == typeof(GameConfiguration));

        if (cloneMethod is null || !this.Type.IsAssignableFrom(cloneMethod.ReturnType))
        {
            this.ToastService.ShowError($"Type '{this.Type.Name}' must have a Clone method that takes '{nameof(GameConfiguration)}' and returns '{this.Type.Name}'.");
            return null;
        }

        var cloned = cloneMethod.Invoke(original, new object[] { gameConfiguration });
        if (cloned is null || !this.Type.IsAssignableFrom(cloned.GetType()))
        {
            this.ToastService.ShowError(string.Format(Resources.FailedToClone, viewModel.Name));
            return null;
        }

        return cloned;
    }

    private async Task CopyPropertyAsync(PropertyInfo prop, object source, object target, IContext context, CancellationToken cancellationToken)
    {
        var sourceValue = prop.GetValue(source);
        var targetValue = prop.GetValue(target);

        if (sourceValue is IEnumerable sourceCollection && sourceValue is not string)
        {
            await this.CopyCollectionPropertyAsync(prop, sourceCollection, targetValue, context, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (prop.CanWrite && prop.GetSetMethod(false) is not null)
        {
            await this.CopySinglePropertyAsync(prop, sourceValue, target, context, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task CopyCollectionPropertyAsync(PropertyInfo prop, IEnumerable sourceCollection, object? targetValue, IContext context, CancellationToken cancellationToken)
    {
        var addMethod = targetValue?.GetType().GetMethod("Add");
        if (addMethod is null)
        {
            return;
        }

        var clearMethod = targetValue!.GetType().GetMethod("Clear");
        clearMethod?.Invoke(targetValue, null);

        var itemType = prop.PropertyType.IsGenericType 
            ? prop.PropertyType.GetGenericArguments().FirstOrDefault() 
            : null;

        foreach (var item in sourceCollection)
        {
            var itemToSet = item;
            if (itemToSet is IIdentifiable identifiableItem && identifiableItem.Id != Guid.Empty && itemType is not null)
            {
                var trackedItem = await context.GetByIdAsync(identifiableItem.Id, itemType, cancellationToken).ConfigureAwait(false);
                if (trackedItem is not null)
                {
                    itemToSet = trackedItem;
                }
            }

            addMethod.Invoke(targetValue, new[] { itemToSet });
        }
    }

    private async Task CopySinglePropertyAsync(PropertyInfo prop, object? sourceValue, object target, IContext context, CancellationToken cancellationToken)
    {
        var sourceValueToSet = sourceValue;
        if (sourceValueToSet is IIdentifiable identifiable && identifiable.Id != Guid.Empty && prop.PropertyType.Namespace?.StartsWith("MUnique.") == true)
        {
            var trackedItem = await context.GetByIdAsync(identifiable.Id, prop.PropertyType, cancellationToken).ConfigureAwait(false);
            if (trackedItem is not null)
            {
                sourceValueToSet = trackedItem;
            }
        }

        prop.SetValue(target, sourceValueToSet);
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